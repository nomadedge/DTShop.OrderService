using DTShop.OrderService.Data.Repositories;
using DTShop.OrderService.RabbitMQ.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTShop.OrderService.RabbitMQ.Consumers
{
    public class PayForOrderConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PayForOrderConsumer> _logger;
        private readonly IRabbitManager _rabbitManager;

        public PayForOrderConsumer(
            IServiceScopeFactory scopeFactory,
            ILogger<PayForOrderConsumer> logger,
            IRabbitManager rabbitManager)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _rabbitManager = rabbitManager;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("PaymentService_OrderExchange", ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare("OrderService_PayForOrderQueue", true, false, false, null);
            _channel.QueueBind("OrderService_PayForOrderQueue", "PaymentService_OrderExchange", "PayForOrder", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body);
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("OrderService_PayForOrderQueue", false, consumer);
            return Task.CompletedTask;
        }

        private async void HandleMessage(string content)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                    var payForOrderDto = JsonConvert.DeserializeObject<PayForOrderDto>(content);
                    _logger.LogInformation("Start updating status and payment info for order with OrderId {OrderId}.",
                        payForOrderDto.OrderId);
                    var order = await orderRepository.PayForOrderAsync(
                        payForOrderDto.OrderId,
                        payForOrderDto.PaymentId,
                        payForOrderDto.Status);
                    var changeStatusDto = new ChangeStatusDto
                    {
                        OrderId = order.OrderId,
                        Status = order.Status.ToString()
                    };
                    _rabbitManager.Publish(changeStatusDto, "OrderService_ChangeOrderStatusExchange", "fanout", "OrderStatusChanged");

                    _logger.LogInformation("Status and payment info for order with OrderId {OrderId} has been successfuly updated.",
                        payForOrderDto.OrderId);
                }
                catch (Exception e)
                {
                    //_logger.LogInformation("Failed to update status and payment info for order with OrderId {OrderId}.",
                    //    payForOrderDto.OrderId);
                }
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

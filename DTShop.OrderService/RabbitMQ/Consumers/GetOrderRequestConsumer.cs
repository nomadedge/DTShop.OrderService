using AutoMapper;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using DTShop.OrderService.Data.Repositories;
using DTShop.OrderService.RabbitMQ.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTShop.OrderService.RabbitMQ.Consumers
{
    public class GetOrderRequestConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderRequestConsumer> _logger;

        public GetOrderRequestConsumer(
            IServiceScopeFactory scopeFactory,
            IMapper mapper,
            ILogger<GetOrderRequestConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("PaymentService_OrderExchange", ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare("OrderService_GetOrderRequestQueue", true, false, false, null);
            _channel.QueueBind("OrderService_GetOrderRequestQueue", "PaymentService_OrderExchange", "GetOrderRequest", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                HandleMessage(ea);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("OrderService_GetOrderRequestQueue", false, consumer);
            return Task.CompletedTask;
        }

        private async void HandleMessage(BasicDeliverEventArgs ea)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var content = Encoding.UTF8.GetString(ea.Body);
                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    //replyProps.Persistent = true;

                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                    var orderRequestDto = JsonConvert.DeserializeObject<OrderRequestDto>(content);
                    _logger.LogInformation("Payment service has started getting order with OrderId {OrderId}",
                        orderRequestDto.OrderId);
                    var order = new Order();
                    try
                    {
                        order = await orderRepository.GetOrderByIdAsync(orderRequestDto.OrderId);
                    }
                    catch (Exception)
                    {
                        order = new Order { OrderId = 0, OrderItems = new List<OrderItem>(), PaymentId = null, Status = new Status { StatusId = 0, Name = "Collecting" }, Username = null };
                    }

                    var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_mapper.Map<Order, OrderModel>(order)));
                    _channel.BasicPublish("", props.ReplyTo, replyProps, sendBytes);

                    _logger.LogInformation("Payment service has successfuly gotten order with OrderId {OrderId}",
                        orderRequestDto.OrderId);
                }
                catch (Exception e)
                {
                    //_logger.LogInformation("{Username} has failed to get order with OrderId {OrderId}",
                    //    orderRequestDto.Username, orderRequestDto.OrderId);
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

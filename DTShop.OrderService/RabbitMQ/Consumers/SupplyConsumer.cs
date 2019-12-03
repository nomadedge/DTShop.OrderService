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
    public class SupplyConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SupplyConsumer> _logger;

        public SupplyConsumer(
            IServiceScopeFactory scopeFactory,
            ILogger<SupplyConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("ItemsSupply", ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare("OrderService_ItemsSupplied", true, false, false, null);
            _channel.QueueBind("OrderService_ItemsSupplied", "ItemsSupply", "ItemsSupplied", null);
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

            _channel.BasicConsume("OrderService_ItemsSupplied", false, consumer);
            return Task.CompletedTask;
        }

        private async void HandleMessage(string content)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var warehouseRepository = scope.ServiceProvider.GetRequiredService<IWarehouseRepository>();
                    var supplyItemsDto = JsonConvert.DeserializeObject<SupplyItemsDto>(content);
                    _logger.LogInformation("Start to adding {Amount} items with ItemId {ItemId}.",
                        supplyItemsDto.Amount, supplyItemsDto.ItemId);
                    await warehouseRepository.SupplyItemsAsync(
                        supplyItemsDto.ItemId,
                        supplyItemsDto.Amount,
                        supplyItemsDto.Name,
                        supplyItemsDto.Price);
                    _logger.LogInformation("Successfuly added {Amount} items with ItemId {ItemId}.",
                        supplyItemsDto.Amount, supplyItemsDto.ItemId);
                }
                catch (Exception e)
                {
                    //_logger.LogInformation("Fail to add {Amount} items with ItemId {ItemId}.",
                    //    supplyItemsDto.Amount, supplyItemsDto.ItemId);
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

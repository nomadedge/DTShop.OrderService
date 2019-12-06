using AutoMapper;
using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using DTShop.OrderService.Data.Repositories;
using DTShop.OrderService.RabbitMQ;
using DTShop.OrderService.RabbitMQ.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTShop.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        private readonly ILogger<OrdersController> _logger;
        private readonly IRabbitManager _rabbitManager;

        public OrdersController(
            IOrderRepository orderRepository,
            IMapper mapper,
            LinkGenerator linkGenerator,
            ILogger<OrdersController> logger,
            IRabbitManager rabbitManager)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
            _logger = logger;
            _rabbitManager = rabbitManager;
        }

        [HttpGet]
        public ActionResult<List<OrderModel>> GetAllOrders()
        {
            _logger.LogInformation("Getting all orders");

            var orders = _orderRepository.GetAllOrders().ToList();

            if (orders.Any())
            {
                return _mapper.Map<List<OrderModel>>(orders);
            }
            return NoContent();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderModel>> GetOrderById(int orderId)
        {
            _logger.LogInformation("Getting order with id {OrderId}", orderId);

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                return _mapper.Map<Order, OrderModel>(order);
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{orderId}/status/{status}")]
        public async Task<ActionResult<OrderModel>> ChangeOrderStatus(int orderId, OrderStatus status)
        {
            _logger.LogInformation("Start to setting order with OrderId {OrderId} status to \"{OrderStatus}\"",
                orderId, status.ToString());

            try
            {
                var order = await _orderRepository.SetOrderStatusAsync(orderId, status);

                _logger.LogInformation("Order with OrderId {OrderId} status is successfuly set to \"{OrderStatus}\"",
                    orderId, status.ToString());

                var changeStatusDto = new ChangeStatusDto
                {
                    OrderId = order.OrderId,
                    Status = order.Status.Name
                };
                _rabbitManager.Publish(changeStatusDto, "OrderService_ChangeOrderStatusExchange", "fanout", "ChangeOrderStatus");

                return _mapper.Map<OrderModel>(order);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogInformation("Fail to set order with OrderId {OrderId} status to \"{OrderStatus}\"",
                    orderId, status.ToString());

                return BadRequest(e.Message);
            }
        }

        [HttpPost("{orderId}/item")]
        public async Task<ActionResult<OrderModel>> AddItemToOrder(int orderId, AddItemModel addItemModel)
        {
            try
            {
                _logger.LogInformation("{Username} has started adding {Amount} items with id {ItemId} to order with id {OrderId}",
                    addItemModel.Username, addItemModel.Amount, addItemModel.ItemId, orderId);

                var order = await _orderRepository.AddItemToOrderAsync(
                    orderId,
                    addItemModel.ItemId,
                    addItemModel.Amount,
                    addItemModel.Username);
                var location = _linkGenerator.GetPathByAction(
                    "GetOrderById",
                    "Orders",
                    new { orderId = order.OrderId });

                var reserveItemsDto = new ReserveItemsDto
                {
                    OrderId = order.OrderId,
                    ItemId = addItemModel.ItemId,
                    Amount = addItemModel.Amount
                };
                _rabbitManager.Publish(reserveItemsDto, "OrderService_ReserveItemsExchange", "direct", "ReserveItems");

                if (orderId == 0)
                {
                    _logger.LogInformation("{Username} has successfuly added {Amount} items with id {ItemId} to a newly created order with id {OrderId}",
                        addItemModel.Username, addItemModel.Amount, addItemModel.ItemId, order.OrderId);

                    return Created(location, _mapper.Map<OrderModel>(order));
                }
                else
                {
                    _logger.LogInformation("{Username} has successfuly added {Amount} items with id {ItemId} to an existing order with id {OrderId}",
                        addItemModel.Username, addItemModel.Amount, addItemModel.ItemId, order.OrderId);

                    return _mapper.Map<OrderModel>(order);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("{Username} has failed to add {Amount} items with id {ItemId} to an order with id {OrderId}",
                    addItemModel.Username, addItemModel.Amount, addItemModel.ItemId, orderId);

                return BadRequest(e.Message);
            }
        }
    }
}
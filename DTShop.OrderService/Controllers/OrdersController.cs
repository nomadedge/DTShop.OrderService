﻿using AutoMapper;
using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using DTShop.OrderService.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

        public OrdersController(
            IOrderRepository orderRepository,
            IMapper mapper,
            LinkGenerator linkGenerator)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public ActionResult<List<OrderModel>> GetAllOrders()
        {
            var orders = _orderRepository.GetAllOrders().ToList();

            if (orders.Any())
            {
                return _mapper.Map<List<OrderModel>>(orders);
            }
            return NoContent();
        }

        [HttpGet("{orderId}")]
        public ActionResult<OrderModel> GetOrderById(int orderId)
        {
            try
            {
                var order = _orderRepository.GetOrderById(orderId);
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
            try
            {
                var order = await _orderRepository.SetOrderStatus(orderId, status);

                return _mapper.Map<OrderModel>(order);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{orderId}/item")]
        public async Task<ActionResult<OrderModel>> AddItemToOrder(int orderId, AddItemModel addItemModel)
        {
            try
            {
                var order = await _orderRepository.AddItemToOrder(
                    orderId,
                    addItemModel.ItemId,
                    addItemModel.Amount,
                    addItemModel.Username);
                var location = _linkGenerator.GetPathByAction(
                    "GetOrderById",
                    "Orders",
                    new { orderId = order.OrderId });

                if (orderId == 0)
                {
                    return Created(location, _mapper.Map<OrderModel>(order));
                }
                else
                {
                    return _mapper.Map<OrderModel>(order);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
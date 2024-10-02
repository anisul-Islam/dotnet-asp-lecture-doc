using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_db_api.EFCore;
using ecommerce_db_api.Models.orders;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_db_api.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
        Task<OrderDto?> UpdateOrderAsync(Guid orderId, CreateOrderDto updateOrderDto);
        Task<bool> DeleteOrderByIdAsync(Guid orderId);
    }
    public class OrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        // Create a new order
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                UserId = createOrderDto.UserId,
                OrderDate = DateTime.UtcNow
            };

            foreach (var orderProductDto in createOrderDto.OrderProducts)
            {
                var orderProduct = new OrderProduct
                {
                    ProductId = orderProductDto.ProductId,
                    Quantity = orderProductDto.Quantity,
                    Price = orderProductDto.Price
                };

                order.OrderProducts.Add(orderProduct);
            }

            await _appDbContext.Orders.AddAsync(order);
            await _appDbContext.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        // Get order by ID
        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _appDbContext.Orders
             .Include(o => o.OrderProducts)
             .ThenInclude(op => op.Product)  // Include Product details
             .Include(o => o.User)  // Include User details
             .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }

        // Update an order
        public async Task<OrderDto?> UpdateOrderAsync(Guid orderId, CreateOrderDto updateOrderDto)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return null;

            // Clear existing products and add updated products
            order.OrderProducts.Clear();
            foreach (var orderProductDto in updateOrderDto.OrderProducts)
            {
                var orderProduct = new OrderProduct
                {
                    ProductId = orderProductDto.ProductId,
                    Quantity = orderProductDto.Quantity,
                    Price = orderProductDto.Price
                };

                order.OrderProducts.Add(orderProduct);
            }

            _appDbContext.Orders.Update(order);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        // Delete an order by ID
        public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return false;

            _appDbContext.Orders.Remove(order);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }

}
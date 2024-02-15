using Azure.Core;
using ETicaretAPI.Application.Abstractions.Services.Order;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
        }

        public async Task CreateOrder(CreateOrder createOrder)
        {
            string orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf(",")+1, orderCode.Length - orderCode.IndexOf(",") - 1);
            await _orderWriteRepository.AddAsync(new Domain.Entities.Order
            {
                Address = createOrder.Address,
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description,
                OrderCode = orderCode
            });

            await _orderWriteRepository.SaveAsync();
        }

        public async Task<ListOrderResponse> GetOrdersAsync(int page, int size)
        {
            var totalCount = _orderReadRepository.GetAll(false).Count();

            var query =  _orderReadRepository.Table.Skip(page * size).Take(size).Include(o => o.Basket)
            .ThenInclude(b => b.User)
                .ThenInclude(b => b.Baskets)
                    .ThenInclude(b => b.BasketItems)
                        .ThenInclude(b => b.Product);

            var data = from order in query
                       join completedOrder in _completedOrderReadRepository.Table
                       on order.Id equals completedOrder.OrderId into co
                       from _co in co.DefaultIfEmpty()
                       select new ListOrder
                       {
                           Id = order.Id.ToString(),
                           CreatedDate = order.CreatedDate,
                           OrderCode = order.OrderCode,
                           TotalPrice = order.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                           Username = order.Basket.User.UserName,
                           Completed = _co != null ? true : false
                       };

            return new ListOrderResponse
            {
                Orders = await data.Select(x => new ListOrder
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    OrderCode = x.OrderCode,
                    TotalPrice = x.TotalPrice,
                    Username = x.Username,
                    Completed = x.Completed
                }).ToListAsync(),
                TotalCount = totalCount
            };
        }
        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var query = _orderReadRepository.Table
                .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                        .ThenInclude(bi => bi.Product);

            var data = await (from order in query
                       join completedOrder in _completedOrderReadRepository.Table
                       on order.Id equals completedOrder.OrderId into co
                       from _co in co.DefaultIfEmpty()
                       select new SingleOrder
                       {
                           Id = order.Id.ToString(),
                           CreatedDate = order.CreatedDate,
                           OrderCode = order.OrderCode,
                           Address = order.Address,
                           Description = order.Description,
                           BasketItems = order.Basket.BasketItems.Select(bi => new SingleOrderBasketItem
                           {
                               Name = bi.Product.Name,
                               Price = bi.Product.Price,
                               Quantity = bi.Quantity
                           }).ToList(),
                           Completed = _co != null ? true : false
                       }).FirstOrDefaultAsync(o => o.Id == id);

            return new SingleOrder
            {
                Id = data.Id,
                CreatedDate = data.CreatedDate,
                OrderCode = data.OrderCode,
                Address = data.Address,
                Description = data.Description,
                BasketItems = data.BasketItems.ToList(),
                Completed = data.Completed
            };
        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table
                .Include(o=> o.Basket)
                .ThenInclude(b=> b.User)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new CompletedOrder
                {
                    OrderId = Guid.Parse(id)
                });

                int ss = await _completedOrderWriteRepository.SaveAsync();

                if (ss > 0)
                {
                    CompletedOrderDTO completedOrder = new CompletedOrderDTO
                    {
                        OrderCode = order.OrderCode,
                        OrderDate = order.CreatedDate,
                        Username = order.Basket.User.UserName,
                        Usersurname = order.Basket.User.NameSurname,
                        Email = order.Basket.User.Email
                    };
                    return (true, completedOrder);
                }
            }

            return (false, new CompletedOrderDTO());

        }
    }
}

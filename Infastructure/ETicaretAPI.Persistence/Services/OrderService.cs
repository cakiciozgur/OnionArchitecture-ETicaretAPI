using Azure.Core;
using ETicaretAPI.Application.Abstractions.Services.Order;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
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
            List<ListOrder> orders = await _orderReadRepository.Table.Skip(page * size).Take(size).Include(o => o.Basket)
            .ThenInclude(b => b.User)
            .ThenInclude(b=> b.Baskets)
            .ThenInclude(b => b.BasketItems)
            .ThenInclude(b => b.Product)
            .Select(x=> new ListOrder
            {
                    Id = x.Id.ToString(),
                    CreatedDate = x.CreatedDate,
                    OrderCode = x.OrderCode,
                    TotalPrice =  x.Basket.BasketItems.Sum(bi=> bi.Product.Price * bi.Quantity),
                    Username  = x.Basket.User.UserName
            }).ToListAsync();

            return new ListOrderResponse
            {
                Orders = orders,
                TotalCount = totalCount
            };
        }
        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = await _orderReadRepository.Table
                .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                        .ThenInclude(bi => bi.Product)
                            .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            return new SingleOrder
            {
                Id = data.Id.ToString(),
                CreatedDate = data.CreatedDate,
                OrderCode = data.OrderCode,
                Address = data.Address,
                Description = data.Description,
                BasketItems = data.Basket.BasketItems.Select(bi => new SingleOrderBasketItem
                {
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    Quantity = bi.Quantity
                }).ToList()
            };
        }
    }
}

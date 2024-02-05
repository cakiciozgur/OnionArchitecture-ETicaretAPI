using ETicaretAPI.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Order
{
    public interface IOrderService
    {
        Task CreateOrder(CreateOrder createOrder);
        Task<ListOrderResponse> GetOrdersAsync(int page, int size);
        Task<SingleOrder> GetOrderByIdAsync(string id);
    }
}

using ETicaretAPI.Application.Abstractions.Services.Order;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQueryRequest, GetOrdersQueryResponse>
    {
        readonly IOrderService _orderService;
        public GetOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetOrdersQueryResponse> Handle(GetOrdersQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _orderService.GetOrdersAsync(request.Page, request.Size);

            int totalCount = response.TotalCount;
            var orders = response.Orders.Select(o => new ListOrder
            {
                Id = o.Id,
                CreatedDate = o.CreatedDate,
                OrderCode = o.OrderCode,
                TotalPrice = o.TotalPrice,
                Username = o.Username
            }).ToList();

            return new GetOrdersQueryResponse
            {
                Orders = orders,
                TotalCount = totalCount
            };
        }
    }
}

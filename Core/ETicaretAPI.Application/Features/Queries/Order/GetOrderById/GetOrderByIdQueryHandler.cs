using ETicaretAPI.Application.Abstractions.Services.Order;
using ETicaretAPI.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQueryRequest, GetOrderByIdQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetOrderByIdQueryResponse> Handle(GetOrderByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _orderService.GetOrderByIdAsync(request.Id);

            return new GetOrderByIdQueryResponse
            {
                Id = response.Id.ToString(),
                CreatedDate = response.CreatedDate,
                OrderCode = response.OrderCode,
                Address = response.Address,
                Description = response.Description,
                BasketItems = response.BasketItems
            };
        }
    }
}

using ETicaretAPI.Application.Abstractions.Services.Mail;
using ETicaretAPI.Application.Abstractions.Services.Order;
using ETicaretAPI.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommandRequest, CompleteOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IMailService _mailService;

        public CompleteOrderCommandHandler(IOrderService orderService, IMailService mailService)
        {
            _orderService = orderService;
            _mailService = mailService;
        }

        public async Task<CompleteOrderCommandResponse> Handle(CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
            (bool success, CompletedOrderDTO completedOrderDTO) = await _orderService.CompleteOrderAsync(request.Id);

            if (success)
            {
                await _mailService.SendCompletedOrderMailAsync(completedOrderDTO.Email, completedOrderDTO.OrderCode, completedOrderDTO.OrderDate, completedOrderDTO.Username, completedOrderDTO.UserNameSurname);
            }

            return new CompleteOrderCommandResponse
            {
                Success = success
            };
        }
    }
}

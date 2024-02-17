using ETicaretAPI.Application.Constants;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Features.Commands.Order.CompleteOrder;
using ETicaretAPI.Application.Features.Commands.Order.CreateOrder;
using ETicaretAPI.Application.Features.Queries.Order.GetOrderById;
using ETicaretAPI.Application.Features.Queries.Order.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class OrdersController : ControllerBase
    {
        readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Orders, ActionType = Application.Enums.ActionType.Writing, Definition = "Create Order")]

        public async Task<IActionResult> CreateOrder(CreateOrderCommandRequest createOrderCommandRequest)
        {
            CreateOrderCommandResponse response = await _mediator.Send(createOrderCommandRequest);
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Orders, ActionType = Application.Enums.ActionType.Reading, Definition = "Get Orders")]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQueryRequest getOrdersQueryRequest)
        {
            GetOrdersQueryResponse response = await _mediator.Send(getOrdersQueryRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Orders, ActionType = Application.Enums.ActionType.Reading, Definition = "Get Order By Id")]
        public async Task<IActionResult> GetOrdersById([FromRoute] GetOrderByIdQueryRequest getOrderByIdQueryRequest)
        {
            GetOrderByIdQueryResponse response = await _mediator.Send(getOrderByIdQueryRequest);
            return Ok(response);
        }

        [HttpGet("complete-order/{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Orders, ActionType = Application.Enums.ActionType.Updating, Definition = "Complete Order")]
        public async Task<IActionResult> CompleteOrder([FromRoute] CompleteOrderCommandRequest completeOrderCommandRequest)
        {
            CompleteOrderCommandResponse response = await _mediator.Send(completeOrderCommandRequest);
            return Ok(response);
        }
    }
}

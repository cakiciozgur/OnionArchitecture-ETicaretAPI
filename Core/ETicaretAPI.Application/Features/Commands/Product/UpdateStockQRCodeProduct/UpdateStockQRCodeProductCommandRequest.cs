using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStockQRCodeProduct
{
    public class UpdateStockQRCodeProductCommandRequest : IRequest<UpdateStockQRCodeProductCommandResponse>
    {
        public string ProductId { get; set; }
        public int Stock { get; set; }
    }
}
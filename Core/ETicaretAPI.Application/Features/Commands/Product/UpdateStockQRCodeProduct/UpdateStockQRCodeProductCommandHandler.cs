using ETicaretAPI.Application.Abstractions.Services.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStockQRCodeProduct
{
    public class UpdateStockQRCodeProductCommandHandler : IRequestHandler<UpdateStockQRCodeProductCommandRequest, UpdateStockQRCodeProductCommandResponse>
    {
        readonly IProductService _productService;

        public UpdateStockQRCodeProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<UpdateStockQRCodeProductCommandResponse> Handle(UpdateStockQRCodeProductCommandRequest request, CancellationToken cancellationToken)
        {
            bool success = await _productService.UpdateStockQRCodeToProductAsync(request.ProductId, request.Stock);

            return new UpdateStockQRCodeProductCommandResponse
            {
                Success = success
            };
        }
    }
}

using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowcaseImage
{
    public class ChangeShowcaseImageCommandHandler : IRequestHandler<ChangeShowcaseImageCommandRequest, ChangeShowcaseImageCommandResponse>
    {
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

        public ChangeShowcaseImageCommandHandler(IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<ChangeShowcaseImageCommandResponse> Handle(ChangeShowcaseImageCommandRequest request, CancellationToken cancellationToken)
        {
            var query = _productImageFileWriteRepository.Table
                .Include(p => p.Products)
                .SelectMany(p => p.Products, (productImageFile, product) => new
                {
                    productImageFile,
                    product
                });
                
            var dataProductId = await query.FirstOrDefaultAsync(p=> p.product.Id == Guid.Parse(request.ProductId) && p.productImageFile.Showcase);

            if(dataProductId != null)
            {
                dataProductId.productImageFile.Showcase = false;
            }

            var dataImageId = await query.FirstOrDefaultAsync(pi => pi.productImageFile.Id == Guid.Parse(request.ImageId));
            if(dataImageId != null)
            {
                dataImageId.productImageFile.Showcase = true;
            }

            await _productImageFileWriteRepository.SaveAsync();

            return new();
        }
    }
}

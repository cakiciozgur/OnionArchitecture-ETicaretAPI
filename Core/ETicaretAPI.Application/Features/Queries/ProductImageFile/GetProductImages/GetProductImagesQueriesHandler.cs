using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages
{
    public class GetProductImagesQueriesHandler : IRequestHandler<GetProductImagesQueriesRequest, List<GetProductImagesQueriesResponse>>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IConfiguration _configuration;
        public GetProductImagesQueriesHandler(IProductReadRepository productReadRepository, IConfiguration configuration)
        {
            _productReadRepository = productReadRepository;
            _configuration = configuration;
        }
        public async Task<List<GetProductImagesQueriesResponse>> Handle(GetProductImagesQueriesRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product? product = await _productReadRepository.Table.Include(p => p.Images).FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));

            List<GetProductImagesQueriesResponse> response = product.Images.Select(i => new GetProductImagesQueriesResponse
            {
                FileName = i.FileName,
                Id = i.Id,
                Path = $"{_configuration["BaseStorageUrl:Local"]}/{i.Path}"
            }).ToList();
            return response;
        }
    }
}

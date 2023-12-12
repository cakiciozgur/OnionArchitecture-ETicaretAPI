using ETicaretAPI.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            _productWriteRepository.AddRangeAsync(new()
            {
                new() { Id = Guid.NewGuid(), Name = "P1", Price = 50, Stock = 21, CreatedDate = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "P2", Price = 10, Stock = 25, CreatedDate = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "P3", Price = 15, Stock = 24, CreatedDate = DateTime.UtcNow }
            });

            var count = await _productWriteRepository.SaveAsync();
        }
    }
}

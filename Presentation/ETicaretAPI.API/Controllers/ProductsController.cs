using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
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
        readonly private IOrderWriteRepository _orderWriteRepository;
        readonly private IOrderReadRepository _orderReadRepository;
        readonly private ICustomerReadRepository _customerReadRepository;
        readonly private ICustomerWriteRepository _customerWriteRepository;
        public ProductsController(
            IProductWriteRepository productWriteRepository, 
            IProductReadRepository productReadRepository, 
            IOrderWriteRepository orderWriteRepository, 
            IOrderReadRepository orderReadRepository, 
            ICustomerWriteRepository customerWriteRepository, 
            ICustomerReadRepository customerReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _customerWriteRepository = customerWriteRepository;
            _customerReadRepository = customerReadRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            var p = await _orderReadRepository.GetByIdAsync("e8c33f3c-037c-4728-92d0-d32f275373a6");
            p.Address = "Aydın";

            await _orderWriteRepository.SaveAsync();
        }
    }
}

using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IFileWriteRepository _fileWriteRepository;
        readonly private IFileReadRepository _fileReadRepository;
        readonly private IProductImageFileReadRepository _productImageFileReadRepository;
        readonly private IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly private IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly private IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly private IStorageService _storageService;
        readonly IConfiguration _configuration;
        public ProductsController(IProductWriteRepository productWriteRepository,
            IProductReadRepository productReadRepository,
            IWebHostEnvironment webHostEnvironment,
            IFileWriteRepository fileWriteRepository,
            IFileReadRepository fileReadRepository,
            IProductImageFileReadRepository productImageFileReadRepository,
            IProductImageFileWriteRepository productImageFileWriteRepository,
            IInvoiceFileReadRepository invoiceFileReadRepository,
            IInvoiceFileWriteRepository invoiceFileWriteRepository,
            IStorageService storageService,
            IConfiguration configuration)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult Get([FromQuery]Pagination pagination)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Select(x=> new
            {
                x.Id,
                x.Name,
                x.Stock,
                x.Price,
                x.CreatedDate,
                x.UpdatedDate
            }).Skip(pagination.Page*pagination.Size).Take(pagination.Size);// sadece verileri göstermek amacıyla çektiğimiz için track edilmesine gerek yok
            return Ok(new { totalCount, products });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var p = _productReadRepository.GetByIdAsync(id,false);
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Post(VMCreateProduct model)
        {
            await _productWriteRepository.AddAsync(new Product
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Put(VMUpdateProduct model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            await _productWriteRepository.SaveAsync();

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(string id)
        {
            var datas = await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product = await _productReadRepository.GetByIdAsync(id);
            await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            {
                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product> { product }
            }).ToList());
            await _productImageFileWriteRepository.SaveAsync();

            //foreach (var data in datas) // diğer bir seçenek
            //{
            //    product.Images.Add(new ProductImageFile
            //    {
            //        FileName = data.fileName,
            //        Path = data.pathOrContainerName,
            //        Storage = _storageService.StorageName,
            //        Products = new List<Product> { product }
            //    });
            //}

            return Ok();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetImages(string id)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.Images).FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            return Ok(product?.Images.Select(p=> new
            {
                Path = $"{_configuration["BaseStorageUrl:Local"]}/{p.Path}",
                p.FileName,
                p.Id
            }));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteImage(string id, string imageId)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.Images).FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            ProductImageFile? productImageFile = product.Images.FirstOrDefault(p => p.Id == Guid.Parse(imageId));

            product.Images.Remove(productImageFile);

            await _productWriteRepository.SaveAsync();

            return Ok();
        }
    }
}

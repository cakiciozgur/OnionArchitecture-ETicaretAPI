using ETicaretAPI.Application.Abstractions.Services.Product;
using ETicaretAPI.Application.Abstractions.Services.QRCode;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class ProductService : IProductService
    {
        readonly IProductWriteRepository _productWriteRepository;
        readonly IProductReadRepository _productReadRepository;
        readonly IQRCodeService _qrCodeService;

        public ProductService(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, IQRCodeService qrCodeService)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _qrCodeService = qrCodeService;
        }
        public async Task<byte[]> QrCodeToProductAsync(string productId)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            if(product == null)
            {
                throw new ProductNotFoundException();
            }

            var plaintObject = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CreatedDate
            };

            string plaintText = JsonSerializer.Serialize(plaintObject);

            return _qrCodeService.GenerateQRCode(plaintText);
        }

        public async Task<bool> UpdateStockQRCodeToProductAsync(string productId, int stock)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ProductNotFoundException();
            }

            product.Stock = stock;

            return (await _productWriteRepository.SaveAsync()) > 0;
        }
    }
}

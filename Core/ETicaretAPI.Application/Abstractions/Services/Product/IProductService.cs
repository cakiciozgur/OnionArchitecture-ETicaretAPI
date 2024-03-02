using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Product
{
    public interface IProductService
    {
        Task<byte[]> QrCodeToProductAsync(string productId);
        Task<bool> UpdateStockQRCodeToProductAsync(string productId, int stock);
    }
}

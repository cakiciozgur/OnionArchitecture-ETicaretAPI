using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Concretes
{
    public class ProductService : IProductService
    {
        List<Product> IProductService.GetProducts()
         => new()
         {
                new() { Id = Guid.NewGuid(), Name = "P1", Price = 50, Stock = 21, CreatedDate = DateTime.Now},
                new() { Id = Guid.NewGuid(), Name = "P2", Price = 10, Stock = 25, CreatedDate = DateTime.Now},
                new() { Id = Guid.NewGuid(), Name = "P3", Price = 15, Stock = 24, CreatedDate = DateTime.Now}
         };
    }
}

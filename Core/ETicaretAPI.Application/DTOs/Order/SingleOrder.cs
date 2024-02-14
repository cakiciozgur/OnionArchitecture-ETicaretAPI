using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Order
{
    public class SingleOrder
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public List<SingleOrderBasketItem> BasketItems { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Completed { get; set; }
    }

    public class SingleOrderBasketItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}

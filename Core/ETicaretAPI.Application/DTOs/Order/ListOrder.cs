using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Order
{
    public class ListOrder
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public string Username { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Completed { get; set; }
    }

    public class ListOrderResponse
    {
        public List<ListOrder> Orders { get; set; }
        public int TotalCount { get; set; }
    }
}

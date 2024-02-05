namespace ETicaretAPI.Application.Features.Queries.Order.GetOrders
{
    public class GetOrdersQueryResponse
    {
        public List<ListOrder> Orders { get; set; } 
        public int TotalCount { get; set; }
    }

    public class ListOrder
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public string Username { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
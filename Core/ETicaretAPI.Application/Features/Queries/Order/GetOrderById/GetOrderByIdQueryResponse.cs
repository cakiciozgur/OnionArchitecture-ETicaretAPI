namespace ETicaretAPI.Application.Features.Queries.Order.GetOrderById
{
    public class GetOrderByIdQueryResponse
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public List<DTOs.Order.SingleOrderBasketItem> BasketItems { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
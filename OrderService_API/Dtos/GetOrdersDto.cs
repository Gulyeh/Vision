namespace OrderService_API.Dtos
{
    public class GetOrdersDto
    {
        public GetOrdersDto()
        {
            Title = string.Empty;
            CouponCode = string.Empty;
            OrderType = string.Empty;
        }

        public Guid Id { get; set; }
        public bool Paid { get; set; }
        public DateTime OrderDate { get; set; }
        public string Title { get; set; }
        public string CouponCode { get; set; }
        public string OrderType { get; set; }
    }
}
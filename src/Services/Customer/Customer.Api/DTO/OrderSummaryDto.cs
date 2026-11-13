namespace Customer.Api.DTO
{
    public class OrderSummaryDto
    {
        public string CustomerName { get; set; }

        public int Age { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSpent { get; set; }
    }
}

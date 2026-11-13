namespace Customer.Api.DTO;

public class CustomerByAgeDto
{
    public string CustomerName { get; set; } = "";

    public int Age { get; set; }

    public int TotalPayments { get; set; }
}

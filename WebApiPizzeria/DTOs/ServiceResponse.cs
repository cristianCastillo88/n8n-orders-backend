namespace WebApiPizzeria.DTOs;

public class ServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public decimal? NewTotal { get; set; }
    public string Currency { get; set; } = "USD";
}

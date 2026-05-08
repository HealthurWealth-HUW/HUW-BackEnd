namespace HealthUrWelath.Application.Orders.Dtos
{
    public class OrderDetailsDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int Quality { get; set; }
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public decimal Total { get; set; }
        public int ProductId { get; set; }
    }
}

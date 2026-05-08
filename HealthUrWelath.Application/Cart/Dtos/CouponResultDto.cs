namespace HealthUrWelath.Application.Cart.Dtos
{
    public class CouponResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CouponCode { get; set; }
        public decimal MinCartValue { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal? CouponPercentage { get; set; }
    }
}
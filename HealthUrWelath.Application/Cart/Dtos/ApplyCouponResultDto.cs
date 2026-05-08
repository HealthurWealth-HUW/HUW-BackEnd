namespace HealthUrWelath.Application.Cart.Dtos
{
    public class ApplyCouponResultDto
    {
        public decimal CartTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}

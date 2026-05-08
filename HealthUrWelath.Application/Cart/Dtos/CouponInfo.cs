namespace HealthUrWelath.Application.Cart.Dtos
{
    public class CouponInfo
    {
        public string Coupon_Code { get; set; }
        public DateTime? Valid_From { get; set; }
        public DateTime? Valid_To { get; set; }
        public bool Status { get; set; }
        public decimal? Min_Cart_Value { get; set; }
        public decimal? Coupon_Amount { get; set; }
        public decimal? Coupon_Percentage { get; set; }
    }
}
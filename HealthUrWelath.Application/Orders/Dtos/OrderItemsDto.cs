namespace HealthUrWelath.Application.Orders.Dtos
{
    public class OrderItemsDto
    {
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public decimal TxnAmount { get; set; }

        public decimal ShippingCharges { get; set; }

        public decimal CouponDiscount { get; set; }

        public bool IsCartTampered { get; set; }

        public string PaymentMode { get; set; }

        public string PaymentStatus { get; set; }

        public string OrderCurrentStatus { get; set; }

        public DateTime OrderDate { get; set; }


        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImgUrl { get; set; }

        public int Quantity { get; set; }


        public decimal ActualAmount { get; set; }

        public decimal ItemCouponDiscount { get; set; }

        public decimal FinalAmount { get; set; }


        public DateTime? ExpectedDeliveryDate { get; set; }
    }
    public class OrdersDto
    {
        public long OrderId { get; set; }

        public long UserId { get; set; }


        public decimal TxnAmount { get; set; }

        public decimal ShippingCharges { get; set; }

        public decimal CouponDiscount { get; set; }

        public bool IsCartTampered { get; set; }

        public string PaymentMode { get; set; }

        public string PaymentStatus { get; set; }

        public string OrderCurrentStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public List<ProductDetails> ProductDetails { get; set; }
    }
    public class ProductDetails
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImgUrl { get; set; }

        public int Quantity { get; set; }

        public decimal ActualAmount { get; set; }

        public decimal ItemCouponDiscount { get; set; }

        public decimal FinalAmount { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
    }
}

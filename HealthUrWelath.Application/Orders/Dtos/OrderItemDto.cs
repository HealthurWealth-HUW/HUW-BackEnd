namespace HealthUrWelath.Application.Orders.Dtos
{
    public sealed class OrderItemDto
    {
        public long ProductId { get; init; }

        public string ProductName { get; init; }

        public string ProductImgUrl { get; init; }

        public int Quantity { get; init; }


        public decimal OriginalPrice { get; init; }

        public decimal Price { get; init; }

        public decimal ProductDiscountPercentage { get; init; }

        public decimal ProductDiscountPerUnit { get; init; }

        public decimal ProductDiscountTotal { get; init; }

        public decimal CouponDiscount { get; init; }


        public decimal GSTPercentage { get; init; }

        public decimal GST { get; init; }

        public decimal CGST { get; init; }

        public decimal SGST { get; init; }

        public decimal IGST { get; init; }


        public decimal Total { get; init; }

        public DateTime? ExpectedDeliveryDate { get; init; }
    }
}

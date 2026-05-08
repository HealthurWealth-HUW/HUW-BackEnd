namespace HealthUrWelath.Application.Orders.Dtos
{
    public sealed class ShippingAddressDto
    {
        public long ShippingAddressId { get; init; }

        public string StreetAddress1 { get; init; }

        public string StreetAddress2 { get; init; }

        public string LandMark { get; init; }

        public string City { get; init; }

        public string StateName { get; init; }

        // CountryId from DB; keep name if needed in future
        public int? CountryId { get; init; }
        public string CountryName { get; init; }

        public string PinCode { get; init; }
    }
}

namespace HealthUrWelath.Application.Addresses.Dtos
{
    public sealed class UserAddressDto
    {
        public long UserAddressId { get; set; }
        public int AddressTypeId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetAddress1 { get; set; } = string.Empty;
        public string StreetAddress2 { get; set; } = string.Empty;
        public string LandMark { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

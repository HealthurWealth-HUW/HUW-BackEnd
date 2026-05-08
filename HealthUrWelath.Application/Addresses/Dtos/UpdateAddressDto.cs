namespace HealthUrWelath.Application.Addresses.Dtos
{
    public sealed record UpdateAddressDto(
         long UserAddressId,
         int AddressTypeId,
         int CountryId,
         int StateId,
         string StateName,
         string City,
         string StreetAddress1,
         string StreetAddress2,
         string LandMark,
         string PinCode
     );
}

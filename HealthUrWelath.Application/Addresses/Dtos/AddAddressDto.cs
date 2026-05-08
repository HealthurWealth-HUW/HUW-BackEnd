namespace HealthUrWelath.Application.Addresses.Dtos
{
    public sealed record AddAddressDto(
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

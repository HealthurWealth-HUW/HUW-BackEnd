namespace HealthUrWealth.Api.Contracts.Auth
{
    public sealed record VerifyOtpRequest(
        string Mobile,
        string Otp,
        Guid? GuestId
     );
}

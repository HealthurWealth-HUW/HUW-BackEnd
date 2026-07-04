namespace HealthUrWelath.Application.Authentication.Dtos
{
    public sealed class OtpValidationResult
    {
        public long UserId { get; init; }
        public bool IsLocked { get; init; }
        public DateTime? LockedUntilUtc { get; init; }
        public int AttemptsRemaining { get; init; }
        public bool IsExpired { get; init; }
    }
}

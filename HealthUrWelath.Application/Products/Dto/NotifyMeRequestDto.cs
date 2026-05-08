namespace HealthUrWelath.Application.Products.Dto
{
    public sealed class NotifyMeRequestDto
    {
        public long ProductId { get; init; }

        public string Name { get; init; }

        public string MobileNumber { get; init; }

        public string Email { get; init; }
    }
}

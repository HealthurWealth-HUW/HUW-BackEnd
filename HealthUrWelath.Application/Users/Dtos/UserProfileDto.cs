namespace HealthUrWelath.Application.Users.Dtos
{
    public sealed class UserProfileDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailId { get; set; } = default!;
        public string MobileNo { get; set; } = default!;
        public string AlternateMobileNo { get; set; } = default!;
    }
}

namespace HealthUrWelath.Application.Authentication.Dtos
{
    public sealed class UserDto
    {
        public long UserId { get; init; }
        public long RoleId { get; init; }
        public string Mobile { get; init; }
        public string? Email { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }

    }
}

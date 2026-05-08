namespace HealthUrWelath.Application.Authentication.Interfaces
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }

        long UserId { get; }
    }
}

using HealthUrWelath.Application.Common.Exceptions;

namespace HealthUrWelath.Application.Common.Security
{
    public static class OwnershipGuard
    {
        public static void EnsureOwner(long entityUserId, long currentUserId)
        {
            if (entityUserId != currentUserId)
                throw new AppException("Unauthorized access", 403);
        }

        public static void EnsureNotNull(object entity, string message = "Resource not found")
        {
            if (entity is null)
                throw new AppException(message, 404);
        }
    }
}

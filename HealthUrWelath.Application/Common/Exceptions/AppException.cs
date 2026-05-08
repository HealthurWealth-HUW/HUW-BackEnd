namespace HealthUrWelath.Application.Common.Exceptions
{
    /// <summary>
    /* Use this for:
    Business rule failures
    Invalid state
    Unauthorized ownership
    Checkout errors*/
    /// </summary>
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

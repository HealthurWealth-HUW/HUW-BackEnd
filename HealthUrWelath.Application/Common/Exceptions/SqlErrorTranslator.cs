using Microsoft.Data.SqlClient;

namespace HealthUrWelath.Application.Common.Exceptions
{
    /// <summary>
    /* Central place to turn a raw SqlException into a clean AppException the UI can
    show as-is, instead of a raw stack trace / SQL message. Stored procedures in this
    codebase signal deliberate business-rule failures via RAISERROR('msg', 16, 1) /
    THROW with no custom message id, which SqlClient surfaces as Number == 50000 -
    that message was written by us and is safe to show. Anything else is a real
    infra/data-integrity failure (constraint violation, deadlock, timeout, etc.) and
    should not be trusted verbatim, so we map only the well-known cases and leave the
    rest untranslated (null) so the caller falls back to a generic 500. */
    /// </summary>
    public static class SqlErrorTranslator
    {
        public static AppException? Translate(SqlException ex)
        {
            switch (ex.Number)
            {
                // RAISERROR/THROW with no custom error number - an intentional business
                // validation raised by our own stored procedures (e.g. "Email already in use").
                case 50000:
                    return new AppException(ex.Message, 400, ex);

                // Unique index / PRIMARY KEY violation.
                case 2601:
                case 2627:
                    return new AppException("A record with the same details already exists.", 409, ex);

                // Foreign key constraint violation.
                case 547:
                    return new AppException("This action cannot be completed because related data is missing or in use.", 409, ex);

                // Deadlock victim.
                case 1205:
                    return new AppException("The operation could not be completed due to a conflict. Please try again.", 409, ex);

                // Command/connection timeout.
                case -2:
                    return new AppException("The request timed out. Please try again.", 504, ex);

                default:
                    return null;
            }
        }
    }
}

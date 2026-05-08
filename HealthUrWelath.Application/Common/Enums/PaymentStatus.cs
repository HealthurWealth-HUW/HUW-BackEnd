namespace HealthUrWelath.Application.Common.Enums
{
    public enum PaymentStatus
    {
        Assigned = -1,
        Initiated = 0,
        Success = 1,
        Failed = 2,
        UserDropped = 3,
        Cancelled = 4,
        Forwarded = 5,
        GatewayFailed = 6,
        SessionExpired = 7,
        RefundInitiated = 8,
        RefundProcessing = 9,
        RefundSuccess = 10,
        RefundFailed = 11,
        PendingVerification = 12,
        Verified = 13,
        RejectedByGateway = 14,
        Closed = 15
    }
}

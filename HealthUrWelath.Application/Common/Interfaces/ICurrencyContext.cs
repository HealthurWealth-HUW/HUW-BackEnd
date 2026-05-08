namespace HealthUrWelath.Application.Common.Interfaces
{
    public interface ICurrencyContext
    {
        string CurrencyCode { get; }
        decimal CurrencyValue { get; }
        string CurrencySymbol { get; }
    }
}

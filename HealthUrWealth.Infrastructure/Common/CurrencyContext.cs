using HealthUrWelath.Application.Common.Interfaces;

namespace HealthUrWealth.Infrastructure.Common
{
    public sealed class CurrencyContext : ICurrencyContext
    {
        public string CurrencyCode => "INR";
        public decimal CurrencyValue => 1;
        public string CurrencySymbol => "₹";
    }
}

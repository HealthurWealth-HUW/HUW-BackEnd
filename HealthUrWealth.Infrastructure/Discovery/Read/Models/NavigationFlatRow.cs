namespace HealthUrWealth.Infrastructure.Discovery.Read.Models
{
    internal sealed class NavigationFlatRow
    {
        public long SuperCategoryId { get; init; }
        public string SuperCategoryName { get; init; }

        public long CategoryId { get; init; }
        public string CategoryName { get; init; }

        public long SubCategoryId { get; init; }
        public string SubCategoryName { get; init; }
    }
}

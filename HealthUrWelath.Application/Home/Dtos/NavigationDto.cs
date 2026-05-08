using System;
using System.Collections.Generic;
using System.Text;

namespace HealthUrWelath.Application.Home.Dtos
{
    public sealed class NavigationDto
    {
        public long SuperCategoryId { get; init; }
        public string SuperCategoryName { get; init; }

        public IReadOnlyList<NavigationCategoryDto> Categories { get; init; }
    }
    public sealed class NavigationCategoryDto
    {
        public long CategoryId { get; init; }
        public string CategoryName { get; init; }

        public IReadOnlyList<NavigationSubCategoryDto> SubCategories { get; init; }
    }
    public sealed class NavigationSubCategoryDto
    {
        public long SubCategoryId { get; init; }
        public string SubCategoryName { get; init; }
    }
}

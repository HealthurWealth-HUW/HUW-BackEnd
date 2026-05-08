using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthUrWelath.Application.Home.Dtos
{
    public sealed class CategoryGroupDto
    {
        public long SuperCategoryId { get; init; }
        public string SuperCategoryName { get; init; }

        public long CategoryId { get; init; }
        public string CategoryName { get; init; }

        public IReadOnlyList<ProductSummaryDto> Products { get; init; }
    }
}

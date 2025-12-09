using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.DTOs
{
    public class MenuDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string BusinessName { get; set; } = null!;
        public string? BusinessLogo { get; set; }
        public string Cuisine { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public WorkingHoursDto? WorkingHours { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public List<MenuItemDto> Items { get; set; } = new();
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
        public TrialDto? Trial { get; set; }
    }
}

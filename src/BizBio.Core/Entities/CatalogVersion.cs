using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    /// <summary>
    /// Supports menu versioning and scheduling for different time periods and events
    /// Examples:
    /// - Breakfast menu (06:00-11:00 daily)
    /// - Lunch special menu (11:00-15:00 weekdays)
    /// - Wedding menu (specific date/time)
    /// - Christmas menu (Dec 24-26)
    /// - Happy hour menu (17:00-19:00)
    /// Priority determines which version takes precedence when multiple versions overlap
    /// </summary>
    public class CatalogVersion : BaseEntity
    {
        public int CatalogId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!; // e.g., "Breakfast Menu", "Christmas Special"

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        // Higher priority wins when multiple versions are valid at the same time
        // Default catalog has priority 0, special menus can have higher priority
        public int Priority { get; set; } = 0;

        // Optional: Time-of-day constraints (stored as TimeSpan or string "HH:mm-HH:mm")
        [MaxLength(50)]
        public string? TimeOfDayConstraint { get; set; } // e.g., "06:00-11:00"

        // Optional: Days of week (bitmask or JSON array)
        [MaxLength(100)]
        public string? DaysOfWeek { get; set; } // e.g., "1,2,3,4,5" for weekdays

        // Navigation properties
        public virtual Catalog Catalog { get; set; } = null!;
    }
}

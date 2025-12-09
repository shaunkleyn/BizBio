namespace BizBio.Core.DTOs;

public class MenuCreationDto
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

public class WorkingHoursDto
{
    public DayScheduleDto Monday { get; set; } = new();
    public DayScheduleDto Tuesday { get; set; } = new();
    public DayScheduleDto Wednesday { get; set; } = new();
    public DayScheduleDto Thursday { get; set; } = new();
    public DayScheduleDto Friday { get; set; } = new();
    public DayScheduleDto Saturday { get; set; } = new();
    public DayScheduleDto Sunday { get; set; } = new();
}

public class DayScheduleDto
{
    public string Open { get; set; } = "09:00";
    public string Close { get; set; } = "17:00";
    public bool Closed { get; set; } = false;
}

public class CategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
}

public class MenuItemDto
{
    public string CategoryId { get; set; } = null!; // Will be mapped to actual category ID
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Allergens { get; set; } = new();
    public List<string> Dietary { get; set; } = new();
    public bool Available { get; set; } = true;
    public bool Featured { get; set; } = false;
}

public class SubscriptionPlanDto
{
    public string PlanId { get; set; } = null!;
    public string PlanName { get; set; } = null!;
    public decimal Price { get; set; }
}

public class TrialDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int DaysRemaining { get; set; }
}

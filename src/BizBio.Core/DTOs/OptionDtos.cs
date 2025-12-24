namespace BizBio.Core.DTOs;

public class CreateOptionDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PriceModifier { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateOptionDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PriceModifier { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateOptionGroupDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinRequired { get; set; } = 1;
    public int MaxAllowed { get; set; } = 1;
    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<int> OptionIds { get; set; } = new();
}

public class UpdateOptionGroupDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinRequired { get; set; }
    public int MaxAllowed { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public List<int> OptionIds { get; set; } = new();
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/dashboard/tables")]
[ApiController]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly IRestaurantTableRepository _tableRepo;
    private readonly IProfileRepository _profileRepo;

    public TablesController(IRestaurantTableRepository tableRepo, IProfileRepository profileRepo)
    {
        _tableRepo = tableRepo;
        _profileRepo = profileRepo;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all tables for a specific profile
    /// </summary>
    /// <param name="profileId">The profile ID to retrieve tables for</param>
    [HttpGet]
    public async Task<IActionResult> GetTables([FromQuery] int profileId)
    {
        var userId = GetUserId();
        var profile = await _profileRepo.GetByIdAsync(profileId);

        if (profile == null || profile.UserId != userId)
            return Forbid();

        var tables = await _tableRepo.GetByProfileIdAsync(profileId);

        return Ok(new { success = true, data = new { tables } });
    }

    /// <summary>
    /// Get a specific table by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTable(int id)
    {
        var userId = GetUserId();
        var table = await _tableRepo.GetByIdAsync(id);

        if (table == null)
            return NotFound(new { success = false, error = "Table not found" });

        var profile = await _profileRepo.GetByIdAsync(table.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        return Ok(new { success = true, data = new { table } });
    }

    /// <summary>
    /// Create a new restaurant table with NFC tag
    /// Requires Menu Pro add-on
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTable([FromBody] CreateTableDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var profile = await _profileRepo.GetByIdAsync(dto.ProfileId);

        if (profile == null || profile.UserId != userId)
            return Forbid();

        if (!profile.HasMenuProAddon)
        {
            return BadRequest(new
            {
                success = false,
                error = "Menu Pro add-on required. Please upgrade to use table features."
            });
        }

        // Validate NFC code uniqueness
        var existingTable = await _tableRepo.GetByNFCCodeAsync(dto.NFCTagCode);
        if (existingTable != null && existingTable.ProfileId != dto.ProfileId)
        {
            return BadRequest(new
            {
                success = false,
                error = "NFC tag code is already in use"
            });
        }

        if (!Enum.TryParse<TableCategory>(dto.Category, true, out var category))
        {
            return BadRequest(new
            {
                success = false,
                error = $"Invalid category. Valid values are: {string.Join(", ", Enum.GetNames(typeof(TableCategory)))}"
            });
        }

        var table = new RestaurantTable
        {
            ProfileId = dto.ProfileId,
            TableNumber = dto.TableNumber,
            TableName = dto.TableName,
            TableCategoryId = (int)category,
            FunFact = dto.FunFact,
            NFCTagCode = dto.NFCTagCode,
            NFCTagTypeId = (int)NFCTagType.Sticker,
            NFCTagStatusId = (int)NFCTagStatus.Active,
            IsActive = true,
            CreatedBy = userId.ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _tableRepo.AddAsync(table);
        await _tableRepo.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            data = new
            {
                table = table,
                nfcUrl = $"https://bizbio.co.za/c/{profile.Slug}?nfc={table.NFCTagCode}"
            }
        });
    }

    /// <summary>
    /// Update a restaurant table
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTable(int id, [FromBody] UpdateTableDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var table = await _tableRepo.GetByIdAsync(id);

        if (table == null)
            return NotFound(new { success = false, error = "Table not found" });

        var profile = await _profileRepo.GetByIdAsync(table.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        if (!string.IsNullOrEmpty(dto.TableName))
            table.TableName = dto.TableName;

        if (!string.IsNullOrEmpty(dto.FunFact))
            table.FunFact = dto.FunFact;

        if (dto.IsActive.HasValue)
            table.IsActive = dto.IsActive.Value;

        table.UpdatedAt = DateTime.UtcNow;

        await _tableRepo.UpdateAsync(table);
        await _tableRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { table } });
    }

    /// <summary>
    /// Delete a restaurant table
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var userId = GetUserId();
        var table = await _tableRepo.GetByIdAsync(id);

        if (table == null)
            return NotFound(new { success = false, error = "Table not found" });

        var profile = await _profileRepo.GetByIdAsync(table.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        await _tableRepo.DeleteAsync(table.Id);
        await _tableRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Table deleted successfully" });
    }
}

/// <summary>
/// Data transfer object for creating a new restaurant table
/// </summary>
public class CreateTableDto
{
    /// <summary>
    /// Profile ID that this table belongs to
    /// </summary>
    public int ProfileId { get; set; }

    /// <summary>
    /// Table number (numeric identifier)
    /// </summary>
    public int TableNumber { get; set; }

    /// <summary>
    /// Display name for the table
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Category of the table (e.g., "Regular", "VIP", "Outdoor")
    /// </summary>
    public string Category { get; set; } = "Regular";

    /// <summary>
    /// Fun fact or description for the table
    /// </summary>
    public string? FunFact { get; set; }

    /// <summary>
    /// NFC tag code for this table
    /// </summary>
    public string NFCTagCode { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating a restaurant table
/// </summary>
public class UpdateTableDto
{
    /// <summary>
    /// Display name for the table
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Fun fact or description for the table
    /// </summary>
    public string? FunFact { get; set; }

    /// <summary>
    /// Whether the table is active
    /// </summary>
    public bool? IsActive { get; set; }
}

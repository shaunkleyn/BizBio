using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/entities")]
[ApiController]
[Authorize]
public class EntitiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EntitiesController> _logger;

    public EntitiesController(
        ApplicationDbContext context,
        ILogger<EntitiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all entities for the current user, optionally filtered by product type
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyEntities([FromQuery] int? productType = null)
    {
        try
        {
            var userId = GetUserId();

            var query = _context.Entities
                .Where(e => e.UserId == userId && e.IsActive)
                .OrderBy(e => e.SortOrder)
                .AsQueryable();

            // Optional: Filter by product type
            // Note: This would require joining with product subscriptions to filter
            // For now, returning all entities

            var entities = await query
                .Select(e => new
                {
                    e.Id,
                    e.UserId,
                    e.EntityType,
                    e.Name,
                    e.Slug,
                    e.Description,
                    e.Logo,
                    e.Address,
                    e.City,
                    e.PostalCode,
                    e.Phone,
                    e.Email,
                    e.Website,
                    e.Currency,
                    e.Timezone,
                    e.SortOrder,
                    e.IsActive,
                    e.CreatedAt,
                    e.UpdatedAt,
                    CatalogCount = e.Catalogs.Count(c => c.IsActive)
                })
                .ToListAsync();

            return Ok(new { success = true, entities });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities for user");
            return StatusCode(500, new { success = false, error = "An error occurred while fetching entities" });
        }
    }

    /// <summary>
    /// Get a specific entity by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntity(int id)
    {
        try
        {
            var userId = GetUserId();

            var entity = await _context.Entities
                .Where(e => e.Id == id && e.UserId == userId)
                .Select(e => new
                {
                    e.Id,
                    e.UserId,
                    e.EntityType,
                    e.Name,
                    e.Slug,
                    e.Description,
                    e.Logo,
                    e.Address,
                    e.City,
                    e.PostalCode,
                    e.Phone,
                    e.Email,
                    e.Website,
                    e.Currency,
                    e.Timezone,
                    e.SortOrder,
                    e.IsActive,
                    e.CreatedAt,
                    e.UpdatedAt,
                    CatalogCount = e.Catalogs.Count(c => c.IsActive),
                    CategoryCount = e.Categories.Count(c => c.IsActive)
                })
                .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            return Ok(new { success = true, entity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity {EntityId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching the entity" });
        }
    }

    /// <summary>
    /// Create a new entity
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEntity([FromBody] CreateEntityRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Check MaxEntities limit for current product subscription
            var subscription = await _context.ProductSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                var tier = await _context.SubscriptionTiers
                    .FirstOrDefaultAsync(t => t.Id == subscription.TierId);

                if (tier != null)
                {
                    var currentEntityCount = await _context.Entities
                        .CountAsync(e => e.UserId == userId && e.IsActive);

                    if (currentEntityCount >= tier.MaxEntities)
                    {
                        return StatusCode(403, new
                        {
                            success = false,
                            error = $"Entity limit reached. Your {tier.TierName} plan allows {tier.MaxEntities} business(es). Please upgrade your subscription to create more businesses.",
                            limitReached = true,
                            currentCount = currentEntityCount,
                            maxAllowed = tier.MaxEntities
                        });
                    }
                }
            }

            // Generate slug if not provided
            var slug = !string.IsNullOrEmpty(request.Slug)
                ? request.Slug
                : GenerateSlug(request.Name);

            // Check if slug is unique
            var slugExists = await _context.Entities
                .AnyAsync(e => e.Slug == slug);

            if (slugExists)
            {
                return BadRequest(new { success = false, error = "An entity with this slug already exists" });
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            var entity = new Entity
            {
                UserId = userId,
                EntityType = (EntityType)request.EntityType,
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                Logo = request.Logo,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                Currency = request.Currency ?? "ZAR",
                Timezone = request.Timezone ?? "Africa/Johannesburg",
                SortOrder = request.SortOrder ?? 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            _context.Entities.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, entity = new
            {
                entity.Id,
                entity.UserId,
                entity.EntityType,
                entity.Name,
                entity.Slug,
                entity.Description,
                entity.Logo,
                entity.Address,
                entity.City,
                entity.PostalCode,
                entity.Phone,
                entity.Email,
                entity.Website,
                entity.Currency,
                entity.Timezone,
                entity.SortOrder,
                entity.IsActive,
                entity.CreatedAt,
                entity.UpdatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            return StatusCode(500, new { success = false, error = "An error occurred while creating the entity" });
        }
    }

    /// <summary>
    /// Update an existing entity
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEntity(int id, [FromBody] UpdateEntityRequest request)
    {
        try
        {
            var userId = GetUserId();

            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            // Update fields
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            entity.Name = request.Name ?? entity.Name;
            entity.EntityType = request.EntityType.HasValue ? (EntityType)request.EntityType.Value : entity.EntityType;
            entity.Description = request.Description ?? entity.Description;
            entity.Logo = request.Logo ?? entity.Logo;
            entity.Address = request.Address ?? entity.Address;
            entity.City = request.City ?? entity.City;
            entity.PostalCode = request.PostalCode ?? entity.PostalCode;
            entity.Phone = request.Phone ?? entity.Phone;
            entity.Email = request.Email ?? entity.Email;
            entity.Website = request.Website ?? entity.Website;
            entity.Currency = request.Currency ?? entity.Currency;
            entity.Timezone = request.Timezone ?? entity.Timezone;
            entity.SortOrder = request.SortOrder ?? entity.SortOrder;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userEmail;

            // Update slug if name changed and slug provided
            if (!string.IsNullOrEmpty(request.Slug) && request.Slug != entity.Slug)
            {
                var slugExists = await _context.Entities
                    .AnyAsync(e => e.Slug == request.Slug && e.Id != id);

                if (slugExists)
                {
                    return BadRequest(new { success = false, error = "An entity with this slug already exists" });
                }

                entity.Slug = request.Slug;
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, entity = new
            {
                entity.Id,
                entity.UserId,
                entity.EntityType,
                entity.Name,
                entity.Slug,
                entity.Description,
                entity.Logo,
                entity.Address,
                entity.City,
                entity.PostalCode,
                entity.Phone,
                entity.Email,
                entity.Website,
                entity.Currency,
                entity.Timezone,
                entity.SortOrder,
                entity.IsActive,
                entity.CreatedAt,
                entity.UpdatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity {EntityId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while updating the entity" });
        }
    }

    /// <summary>
    /// Delete an entity (cascade deletes all catalogs)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntity(int id)
    {
        try
        {
            var userId = GetUserId();

            var entity = await _context.Entities
                .Include(e => e.Catalogs)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            // Soft delete
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Entity deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity {EntityId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while deleting the entity" });
        }
    }

    /// <summary>
    /// Create a new catalog for an entity
    /// </summary>
    [HttpPost("{id}/catalogs")]
    public async Task<IActionResult> CreateCatalog(int id, [FromBody] CreateCatalogRequest request)
    {
        try
        {
            var userId = GetUserId();

            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            // Check MaxCatalogsPerEntity limit based on subscription tier
            var subscription = await _context.ProductSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                var tier = await _context.SubscriptionTiers
                    .FirstOrDefaultAsync(t => t.Id == subscription.TierId);

                if (tier != null)
                {
                    var currentCatalogCount = await _context.Catalogs
                        .CountAsync(c => c.EntityId == id && c.IsActive);

                    if (currentCatalogCount >= tier.MaxCatalogsPerEntity)
                    {
                        return StatusCode(403, new
                        {
                            success = false,
                            error = $"Catalog limit reached for this business. Your {tier.TierName} plan allows {tier.MaxCatalogsPerEntity} catalog(s) per business. Please upgrade your subscription to create more catalogs.",
                            limitReached = true,
                            currentCount = currentCatalogCount,
                            maxAllowed = tier.MaxCatalogsPerEntity
                        });
                    }
                }
            }

            // Generate slug if not provided
            var slug = !string.IsNullOrEmpty(request.Slug)
                ? request.Slug
                : GenerateSlug(request.Name);

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            var catalog = new Catalog
            {
                EntityId = id,
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                SortOrder = request.SortOrder ?? 0,
                ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
                ValidTo = request.ValidTo ?? DateTime.UtcNow.AddYears(10),
                IsPublic = request.IsPublic ?? true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            _context.Catalogs.Add(catalog);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, catalog = new
            {
                catalog.Id,
                catalog.EntityId,
                catalog.Name,
                catalog.Slug,
                catalog.Description,
                catalog.SortOrder,
                catalog.ValidFrom,
                catalog.ValidTo,
                catalog.IsPublic,
                catalog.IsActive,
                catalog.CreatedAt,
                catalog.UpdatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog for entity {EntityId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while creating the catalog" });
        }
    }

    /// <summary>
    /// Get all catalogs for an entity
    /// </summary>
    [HttpGet("{id}/catalogs")]
    public async Task<IActionResult> GetEntityCatalogs(int id)
    {
        try
        {
            var userId = GetUserId();

            var entity = await _context.Entities
                .Where(e => e.Id == id && e.UserId == userId)
                .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            var catalogs = await _context.Catalogs
                .Where(c => c.EntityId == id && c.IsActive)
                .OrderBy(c => c.SortOrder)
                .Select(c => new
                {
                    c.Id,
                    c.EntityId,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.SortOrder,
                    c.ValidFrom,
                    c.ValidTo,
                    c.IsPublic,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt,
                    ItemCount = c.Items.Count(i => i.IsActive),
                    CategoryCount = c.Categories.Count()
                })
                .ToListAsync();

            return Ok(new { success = true, catalogs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalogs for entity {EntityId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching catalogs" });
        }
    }

    private string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            // Remove special characters
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate("", (current, c) => current + c);
    }
}

// DTOs
public class CreateEntityRequest
{
    public int EntityType { get; set; }
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Currency { get; set; }
    public string? Timezone { get; set; }
    public int? SortOrder { get; set; }
}

public class UpdateEntityRequest
{
    public string? Name { get; set; }
    public int? EntityType { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Currency { get; set; }
    public string? Timezone { get; set; }
    public int? SortOrder { get; set; }
}

public class CreateCatalogRequest
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool? IsPublic { get; set; }
}

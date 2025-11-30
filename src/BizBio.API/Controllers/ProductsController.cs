using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Enums;

namespace BizBio.API.Controllers;

[Route("api/v1/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepo;
    private readonly IProductAddOnRepository _addOnRepo;
    private readonly ILogger<ProductsController> _logger;
    private readonly ITelemetryService _telemetry;

    public ProductsController(
        IProductRepository productRepo,
        IProductAddOnRepository addOnRepo,
        ILogger<ProductsController> logger,
        ITelemetryService telemetry)
    {
        _productRepo = productRepo;
        _addOnRepo = addOnRepo;
        _logger = logger;
        _telemetry = telemetry;
    }

    /// <summary>
    /// Get all active products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        _logger.LogInformation("Fetching all active products");

        var products = await _productRepo.GetAllActiveAsync();

        _logger.LogInformation("Successfully retrieved {Count} products", products.Count());
        _telemetry.TrackEvent("ProductsViewed", new Dictionary<string, string>
        {
            { "Count", products.Count().ToString() }
        });

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count()
            }
        });
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        _logger.LogInformation("Fetching product by ID: {Id}", id);

        var product = await _productRepo.GetByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Product not found: {Id}", id);
            return NotFound(new
            {
                success = false,
                message = "Product not found"
            });
        }

        _logger.LogInformation("Successfully retrieved product: {Id}", id);
        _telemetry.TrackEvent("ProductViewed", new Dictionary<string, string>
        {
            { "ProductId", id.ToString() },
            { "ProductName", product.Name }
        });

        return Ok(new
        {
            success = true,
            data = product
        });
    }

    /// <summary>
    /// Get product by SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<IActionResult> GetProductBySKU(string sku)
    {
        _logger.LogInformation("Fetching product by SKU: {SKU}", sku);

        var product = await _productRepo.GetBySKUAsync(sku);

        if (product == null)
        {
            _logger.LogWarning("Product not found: {SKU}", sku);
            return NotFound(new
            {
                success = false,
                message = "Product not found"
            });
        }

        return Ok(new
        {
            success = true,
            data = product
        });
    }

    /// <summary>
    /// Get products by type (subscription, physical, service, digital, add-on)
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetProductsByType(string type)
    {
        _logger.LogInformation("Fetching products by type: {Type}", type);

        if (!Enum.TryParse<ProductType>(type, true, out var productType))
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid product type"
            });
        }

        var products = await _productRepo.GetByTypeAsync(productType);

        _logger.LogInformation("Successfully retrieved {Count} products of type {Type}", products.Count(), type);
        _telemetry.TrackEvent("ProductsByTypeViewed", new Dictionary<string, string>
        {
            { "Type", type },
            { "Count", products.Count().ToString() }
        });

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count(),
                type = type
            }
        });
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetProductsByCategory(string category)
    {
        _logger.LogInformation("Fetching products by category: {Category}", category);

        if (!Enum.TryParse<ProductCategory>(category, true, out var productCategory))
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid product category"
            });
        }

        var products = await _productRepo.GetByCategoryAsync(productCategory);

        _logger.LogInformation("Successfully retrieved {Count} products in category {Category}", products.Count(), category);

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count(),
                category = category
            }
        });
    }

    /// <summary>
    /// Get physical products (NFC tags, wristbands, etc.)
    /// </summary>
    [HttpGet("physical")]
    public async Task<IActionResult> GetPhysicalProducts()
    {
        _logger.LogInformation("Fetching physical products");

        var products = await _productRepo.GetPhysicalProductsAsync();

        _logger.LogInformation("Successfully retrieved {Count} physical products", products.Count());
        _telemetry.TrackEvent("PhysicalProductsViewed", new Dictionary<string, string>
        {
            { "Count", products.Count().ToString() }
        });

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count()
            }
        });
    }

    /// <summary>
    /// Get subscription products
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptionProducts()
    {
        _logger.LogInformation("Fetching subscription products");

        var products = await _productRepo.GetSubscriptionProductsAsync();

        _logger.LogInformation("Successfully retrieved {Count} subscription products", products.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count()
            }
        });
    }

    /// <summary>
    /// Get featured products
    /// </summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts()
    {
        _logger.LogInformation("Fetching featured products");

        var products = await _productRepo.GetFeaturedAsync();

        _logger.LogInformation("Successfully retrieved {Count} featured products", products.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                products = products,
                count = products.Count()
            }
        });
    }

    /// <summary>
    /// Get all active add-ons
    /// </summary>
    [HttpGet("add-ons")]
    public async Task<IActionResult> GetAllAddOns()
    {
        _logger.LogInformation("Fetching all active add-ons");

        var addOns = await _addOnRepo.GetAllActiveAsync();

        _logger.LogInformation("Successfully retrieved {Count} add-ons", addOns.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                addOns = addOns,
                count = addOns.Count()
            }
        });
    }

    /// <summary>
    /// Get add-on by ID
    /// </summary>
    [HttpGet("add-ons/{id}")]
    public async Task<IActionResult> GetAddOnById(int id)
    {
        _logger.LogInformation("Fetching add-on by ID: {Id}", id);

        var addOn = await _addOnRepo.GetByIdAsync(id);

        if (addOn == null)
        {
            _logger.LogWarning("Add-on not found: {Id}", id);
            return NotFound(new
            {
                success = false,
                message = "Add-on not found"
            });
        }

        return Ok(new
        {
            success = true,
            data = addOn
        });
    }

    /// <summary>
    /// Get add-ons for a specific product
    /// </summary>
    [HttpGet("{productId}/add-ons")]
    public async Task<IActionResult> GetProductAddOns(int productId)
    {
        _logger.LogInformation("Fetching add-ons for product: {ProductId}", productId);

        var addOns = await _addOnRepo.GetByProductIdAsync(productId);

        _logger.LogInformation("Successfully retrieved {Count} add-ons for product {ProductId}", addOns.Count(), productId);

        return Ok(new
        {
            success = true,
            data = new
            {
                addOns = addOns,
                count = addOns.Count(),
                productId = productId
            }
        });
    }

    /// <summary>
    /// Get global add-ons (applicable to all products)
    /// </summary>
    [HttpGet("add-ons/global")]
    public async Task<IActionResult> GetGlobalAddOns()
    {
        _logger.LogInformation("Fetching global add-ons");

        var addOns = await _addOnRepo.GetGlobalAddOnsAsync();

        _logger.LogInformation("Successfully retrieved {Count} global add-ons", addOns.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                addOns = addOns,
                count = addOns.Count()
            }
        });
    }

    /// <summary>
    /// Get physical add-ons (additional tags, wristbands, etc.)
    /// </summary>
    [HttpGet("add-ons/physical")]
    public async Task<IActionResult> GetPhysicalAddOns()
    {
        _logger.LogInformation("Fetching physical add-ons");

        var addOns = await _addOnRepo.GetPhysicalAddOnsAsync();

        _logger.LogInformation("Successfully retrieved {Count} physical add-ons", addOns.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                addOns = addOns,
                count = addOns.Count()
            }
        });
    }

    /// <summary>
    /// Get service add-ons (additional storage, users, etc.)
    /// </summary>
    [HttpGet("add-ons/services")]
    public async Task<IActionResult> GetServiceAddOns()
    {
        _logger.LogInformation("Fetching service add-ons");

        var addOns = await _addOnRepo.GetServiceAddOnsAsync();

        _logger.LogInformation("Successfully retrieved {Count} service add-ons", addOns.Count());

        return Ok(new
        {
            success = true,
            data = new
            {
                addOns = addOns,
                count = addOns.Count()
            }
        });
    }
}

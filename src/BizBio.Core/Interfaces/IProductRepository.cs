using BizBio.Core.Entities;
using BizBio.Core.Enums;

namespace BizBio.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySKUAsync(string sku);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllActiveAsync();
    Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType);
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
    Task<IEnumerable<Product>> GetByProductLineAsync(int productLineId);
    Task<IEnumerable<Product>> GetFeaturedAsync();
    Task<IEnumerable<Product>> GetPhysicalProductsAsync();
    Task<IEnumerable<Product>> GetSubscriptionProductsAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<int> SaveChangesAsync();
}

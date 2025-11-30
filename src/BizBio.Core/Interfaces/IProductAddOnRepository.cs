using BizBio.Core.Entities;
using BizBio.Core.Enums;

namespace BizBio.Core.Interfaces;

public interface IProductAddOnRepository
{
    Task<ProductAddOn?> GetByIdAsync(int id);
    Task<ProductAddOn?> GetBySKUAsync(string sku);
    Task<IEnumerable<ProductAddOn>> GetAllAsync();
    Task<IEnumerable<ProductAddOn>> GetAllActiveAsync();
    Task<IEnumerable<ProductAddOn>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductAddOn>> GetByTypeAsync(AddOnType addOnType);
    Task<IEnumerable<ProductAddOn>> GetGlobalAddOnsAsync();
    Task<IEnumerable<ProductAddOn>> GetPhysicalAddOnsAsync();
    Task<IEnumerable<ProductAddOn>> GetServiceAddOnsAsync();
    Task AddAsync(ProductAddOn addOn);
    Task UpdateAsync(ProductAddOn addOn);
    Task<int> SaveChangesAsync();
}

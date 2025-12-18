using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

public interface ICatalogBundleRepository
{
    Task<CatalogBundle?> GetByIdAsync(int id);
    Task<CatalogBundle?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<CatalogBundle>> GetByCatalogIdAsync(int catalogId);
    Task<int> GetBundleCountByCatalogIdAsync(int catalogId);
    Task AddAsync(CatalogBundle bundle);
    Task UpdateAsync(CatalogBundle bundle);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();

    // Bundle Step methods
    Task AddStepAsync(CatalogBundleStep step);
    Task UpdateStepAsync(CatalogBundleStep step);
    Task DeleteStepAsync(int stepId);

    // Bundle Step Product methods
    Task AddStepProductAsync(CatalogBundleStepProduct stepProduct);
    Task DeleteStepProductAsync(int stepId, int productId);

    // Option Group methods
    Task AddOptionGroupAsync(CatalogBundleOptionGroup optionGroup);
    Task UpdateOptionGroupAsync(CatalogBundleOptionGroup optionGroup);
    Task DeleteOptionGroupAsync(int optionGroupId);

    // Option methods
    Task AddOptionAsync(CatalogBundleOption option);
    Task UpdateOptionAsync(CatalogBundleOption option);
    Task DeleteOptionAsync(int optionId);
}

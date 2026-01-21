export const useCategoryApi = () => {
  const api = useApi()

  return {
    // Category operations
    getAllCategories: () => api.get('/api/v1/categories'),
    getCategoriesByEntity: (entityId: number) => api.get(`/api/v1/categories/entity/${entityId}`),
    getCategory: (id: number) => api.get(`/api/v1/categories/${id}`),
    createCategory: (data: any) => api.post('/api/v1/categories', data),
    updateCategory: (id: number, data: any) => api.put(`/api/v1/categories/${id}`, data),
    deleteCategory: (id: number) => api.delete(`/api/v1/categories/${id}`),

    // Catalog category associations
    getCatalogCategories: (catalogId: number) => api.get(`/api/v1/categories/catalog/${catalogId}`),
    addCategoryToCatalog: (categoryId: number, catalogId: number, data: any) =>
      api.post(`/api/v1/categories/${categoryId}/catalogs/${catalogId}`, data),
    removeCategoryFromCatalog: (categoryId: number, catalogId: number) =>
      api.delete(`/api/v1/categories/${categoryId}/catalogs/${catalogId}`)
  }
}

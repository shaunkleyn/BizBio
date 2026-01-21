export const useEntityApi = () => {
  const api = useApi()

  return {
    // Entity operations
    getMyEntities: () => api.get('/api/v1/entities'),
    getEntity: (id: number) => api.get(`/api/v1/entities/${id}`),
    createEntity: (data: any) => api.post('/api/v1/entities', data),
    updateEntity: (id: number, data: any) => api.put(`/api/v1/entities/${id}`, data),
    deleteEntity: (id: number) => api.delete(`/api/v1/entities/${id}`),

    // Catalog operations for entity
    getEntityCatalogs: (id: number) => api.get(`/api/v1/entities/${id}/catalogs`),
    createCatalog: (id: number, data: any) => api.post(`/api/v1/entities/${id}/catalogs`, data)
  }
}

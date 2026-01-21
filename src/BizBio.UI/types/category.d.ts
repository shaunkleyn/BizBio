export interface Category {
  id: number
  entityId: number
  name: string
  slug: string
  description?: string
  icon?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export interface CreateCategoryRequest {
  entityId: number
  name: string
  slug?: string
  description?: string
  icon?: string
  sortOrder?: number
}

export interface UpdateCategoryRequest {
  name?: string
  slug?: string
  description?: string
  icon?: string
  sortOrder?: number
}

export interface CatalogCategory {
  catalogId: number
  categoryId: number
  category?: Category
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export interface AddCategoryToCatalogRequest {
  sortOrder?: number
}

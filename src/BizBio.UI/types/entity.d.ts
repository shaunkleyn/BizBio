export interface Entity {
  id: number
  userId: number
  entityType: EntityType
  name: string
  slug: string
  description?: string
  logo?: string
  contactEmail?: string
  contactPhone?: string
  address?: string
  city?: string
  state?: string
  country?: string
  postalCode?: string
  website?: string
  socialLinks?: string
  businessHours?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export enum EntityType {
  Restaurant = 0,
  Store = 1,
  Venue = 2,
  Organization = 3
}

export interface CreateEntityRequest {
  entityType: EntityType
  name: string
  slug?: string
  description?: string
  logo?: string
  contactEmail?: string
  contactPhone?: string
  address?: string
  city?: string
  state?: string
  country?: string
  postalCode?: string
  website?: string
  socialLinks?: string
  businessHours?: string
}

export interface UpdateEntityRequest {
  name?: string
  slug?: string
  description?: string
  logo?: string
  contactEmail?: string
  contactPhone?: string
  address?: string
  city?: string
  state?: string
  country?: string
  postalCode?: string
  website?: string
  socialLinks?: string
  businessHours?: string
}

export interface Catalog {
  id: number
  entityId: number
  name: string
  slug: string
  description?: string
  sortOrder: number
  validFrom: string
  validTo: string
  isPublic: boolean
  isActive: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export interface CreateCatalogRequest {
  name: string
  slug?: string
  description?: string
  sortOrder?: number
  validFrom?: string
  validTo?: string
  isPublic?: boolean
}

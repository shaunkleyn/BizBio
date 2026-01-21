export interface ProductSubscription {
  id: number
  userId: number
  productType: ProductType
  tierId: number
  tier?: SubscriptionTier
  status: SubscriptionStatus
  startDate: string
  endDate?: string
  trialEndDate: string
  isActive: boolean
  cancelledAt?: string
  cancelReason?: string
  autoRenew: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
  trialDaysRemaining?: number
  daysUntilExpiry?: number
  isInTrial?: boolean
  canUpgrade?: boolean
  canDowngrade?: boolean
}

export enum ProductType {
  Cards = 0,
  Menu = 1,
  Catalog = 2
}

export enum SubscriptionStatus {
  Trial = 0,
  Active = 1,
  Cancelled = 2,
  Expired = 3
}

export interface SubscriptionTier {
  id: number
  productType: ProductType
  name: string
  slug: string
  description?: string
  price: number
  billingPeriod: BillingPeriod
  trialDays: number
  maxEntities: number
  maxCatalogs: number
  maxCategories: number
  maxItems: number
  maxImages: number
  features?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  createdBy: string
  updatedBy: string
}

export enum BillingPeriod {
  Monthly = 0,
  Yearly = 1
}

export interface SubscribeRequest {
  productType: ProductType
  tierId: number
}

export interface CancelSubscriptionRequest {
  cancelReason?: string
}

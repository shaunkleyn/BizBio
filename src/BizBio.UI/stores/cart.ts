import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface CartItemVariant {
  id: number
  name: string
  price: number
}

export interface CartItemOption {
  id: number
  groupName: string
  optionName: string
  price: number
}

export interface CartItem {
  id: string // Unique cart item ID (generated)
  catalogItemId: number
  name: string
  description?: string
  basePrice: number
  quantity: number
  variant?: CartItemVariant
  options: CartItemOption[]
  specialInstructions?: string
  image?: string
  isBundle?: boolean
  bundleSelections?: any // For bundle items
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])
  const isCartOpen = ref(false)

  // Computed
  const itemCount = computed(() => {
    return items.value.reduce((total, item) => total + item.quantity, 0)
  })

  const subtotal = computed(() => {
    return items.value.reduce((total, item) => {
      const variantPrice = item.variant?.price || 0
      const optionsPrice = item.options.reduce((sum, opt) => sum + opt.price, 0)
      const itemTotal = (item.basePrice + variantPrice + optionsPrice) * item.quantity
      return total + itemTotal
    }, 0)
  })

  const deliveryFee = computed(() => 0) // Can be made dynamic later
  const serviceFee = computed(() => 0) // Can be made dynamic later

  const total = computed(() => {
    return subtotal.value + deliveryFee.value + serviceFee.value
  })

  // Actions
  function addItem(item: Omit<CartItem, 'id'>) {
    const cartItem: CartItem = {
      ...item,
      id: generateCartItemId()
    }
    items.value.push(cartItem)
  }

  function removeItem(itemId: string) {
    const index = items.value.findIndex(item => item.id === itemId)
    if (index > -1) {
      items.value.splice(index, 1)
    }
  }

  function updateQuantity(itemId: string, quantity: number) {
    const item = items.value.find(i => i.id === itemId)
    if (item) {
      if (quantity <= 0) {
        removeItem(itemId)
      } else {
        item.quantity = quantity
      }
    }
  }

  function clearCart() {
    items.value = []
  }

  function toggleCart() {
    isCartOpen.value = !isCartOpen.value
  }

  function openCart() {
    isCartOpen.value = true
  }

  function closeCart() {
    isCartOpen.value = false
  }

  function generateCartItemId(): string {
    return `cart-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
  }

  function addBundleItem(bundleData: { bundle: any; configuration: any; totalPrice: number; quantity?: number }) {
    const { bundle, configuration, totalPrice, quantity = 1 } = bundleData

    const cartItem: CartItem = {
      id: generateCartItemId(),
      catalogItemId: bundle.id,
      name: bundle.name,
      description: bundle.description,
      basePrice: totalPrice, // Total includes all options
      quantity,
      options: [], // Options are stored in bundleSelections
      isBundle: true,
      bundleSelections: configuration,
      image: bundle.images ? JSON.parse(bundle.images)[0] : undefined
    }

    items.value.push(cartItem)
  }

  return {
    items,
    isCartOpen,
    itemCount,
    subtotal,
    deliveryFee,
    serviceFee,
    total,
    addItem,
    addBundleItem,
    removeItem,
    updateQuantity,
    clearCart,
    toggleCart,
    openCart,
    closeCart
  }
})

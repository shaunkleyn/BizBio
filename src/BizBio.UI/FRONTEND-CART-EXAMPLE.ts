/**
 * Frontend Cart Implementation Example
 * Shows how to use MRD-style menu API for cart management
 */

// ==================== TYPE DEFINITIONS ====================

interface Menu {
  id: number
  name: string
  sections: MenuSection[]
  options: OptionGroup[]  // ← Global options array
  extras: ExtraGroup[]    // ← Global extras array
}

interface MenuSection {
  id: number
  name: string
  items: MenuItem[]
}

interface MenuItem {
  id: number
  name: string
  description?: string
  variants: Variant[]
  media?: Media
}

interface Variant {
  id: number
  name: string
  price: number
  oldPrice?: number
  default: boolean
  optionIndices: number[]  // ← References menu.options[index]
  extraIndices: number[]   // ← References menu.extras[index]
}

interface OptionGroup {
  id: number
  name: string
  label: string
  minimumSelect: number  // Usually >= 1 (required)
  maximumSelect: number
  items: ModifierItem[]
}

interface ExtraGroup {
  id: number
  name: string
  label: string
  minimumSelect: number  // Usually 0 (optional)
  maximumSelect: number
  items: ModifierItem[]
}

interface ModifierItem {
  id: number
  name: string
  price: number
  default: boolean
}

interface Media {
  baseUrl: string
  rawImage: string
}

// ==================== CART TYPES ====================

interface CartItem {
  menuItemId: number
  menuItemName: string
  variantId: number
  variantName: string
  quantity: number
  basePrice: number

  // Selected modifiers
  selectedOptions: SelectedOption[]
  selectedExtras: SelectedExtra[]

  // Calculated
  itemTotal: number
}

interface SelectedOption {
  groupId: number
  groupName: string
  optionId: number
  optionName: string
  price: number
}

interface SelectedExtra {
  groupId: number
  groupName: string
  extraId: number
  extraName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

// ==================== CART STORE (Vue/Nuxt) ====================

import { defineStore } from 'pinia'

export const useCartStore = defineStore('cart', {
  state: () => ({
    items: [] as CartItem[],
    menu: null as Menu | null,
  }),

  getters: {
    cartSubtotal(): number {
      return this.items.reduce((sum, item) => sum + item.itemTotal, 0)
    },

    cartTotal(): number {
      const deliveryFee = 20 // Could be dynamic
      return this.cartSubtotal + deliveryFee
    },

    cartItemCount(): number {
      return this.items.reduce((sum, item) => sum + item.quantity, 0)
    },
  },

  actions: {
    /**
     * Load menu from API
     */
    async loadMenu(slug: string) {
      const response = await fetch(`/api/v2/menu/by-slug/${slug}`)
      this.menu = await response.json()
    },

    /**
     * Add item to cart with modifier selection
     * This shows the MRD pattern in action!
     */
    async addItemToCart(menuItem: MenuItem, variant: Variant) {
      if (!this.menu) throw new Error('Menu not loaded')

      const selectedOptions: SelectedOption[] = []
      const selectedExtras: SelectedExtra[] = []

      // ==================== HANDLE OPTIONS (Required) ====================

      // variant.optionIndices = [0, 1] means this variant uses:
      // - menu.options[0]
      // - menu.options[1]

      for (const optionIndex of variant.optionIndices) {
        const optionGroup = this.menu.options[optionIndex] // ← Direct array access!

        // Show picker UI to user (modal, dropdown, etc.)
        const selectedOption = await this.showOptionPicker(optionGroup)

        if (!selectedOption) {
          throw new Error(`${optionGroup.label} is required`)
        }

        selectedOptions.push({
          groupId: optionGroup.id,
          groupName: optionGroup.name,
          optionId: selectedOption.id,
          optionName: selectedOption.name,
          price: selectedOption.price,
        })
      }

      // ==================== HANDLE EXTRAS (Optional) ====================

      // variant.extraIndices = [0, 1, 2] means this variant can have:
      // - menu.extras[0]
      // - menu.extras[1]
      // - menu.extras[2]

      for (const extraIndex of variant.extraIndices) {
        const extraGroup = this.menu.extras[extraIndex] // ← Direct array access!

        // Show optional extras picker (user can skip)
        const selectedExtraItems = await this.showExtrasPicker(extraGroup)

        selectedExtraItems.forEach((extra) => {
          selectedExtras.push({
            groupId: extraGroup.id,
            groupName: extraGroup.name,
            extraId: extra.item.id,
            extraName: extra.item.name,
            quantity: extra.quantity,
            unitPrice: extra.item.price,
            totalPrice: extra.item.price * extra.quantity,
          })
        })
      }

      // ==================== CALCULATE TOTAL ====================

      const itemTotal = this.calculateItemTotal(
        variant.price,
        selectedOptions,
        selectedExtras,
        1 // quantity
      )

      // ==================== ADD TO CART ====================

      const cartItem: CartItem = {
        menuItemId: menuItem.id,
        menuItemName: menuItem.name,
        variantId: variant.id,
        variantName: variant.name,
        quantity: 1,
        basePrice: variant.price,
        selectedOptions,
        selectedExtras,
        itemTotal,
      }

      this.items.push(cartItem)
    },

    /**
     * Calculate item total price
     */
    calculateItemTotal(
      basePrice: number,
      options: SelectedOption[],
      extras: SelectedExtra[],
      quantity: number
    ): number {
      let total = basePrice

      // Add option costs
      options.forEach((option) => {
        total += option.price
      })

      // Add extras costs
      extras.forEach((extra) => {
        total += extra.totalPrice
      })

      return total * quantity
    },

    /**
     * Update item quantity
     */
    updateQuantity(index: number, quantity: number) {
      if (quantity <= 0) {
        this.items.splice(index, 1)
        return
      }

      const item = this.items[index]
      item.quantity = quantity

      // Recalculate total
      item.itemTotal = this.calculateItemTotal(
        item.basePrice,
        item.selectedOptions,
        item.selectedExtras,
        quantity
      )
    },

    /**
     * Remove item from cart
     */
    removeItem(index: number) {
      this.items.splice(index, 1)
    },

    /**
     * Clear entire cart
     */
    clearCart() {
      this.items = []
    },

    /**
     * Validate cart with backend and get final totals
     */
    async validateCart() {
      const requestBody = {
        items: this.items.map((item) => ({
          variantId: item.variantId,
          quantity: item.quantity,
          selectedOptions: item.selectedOptions.map((opt) => ({
            optionId: opt.optionId,
          })),
          selectedExtras: item.selectedExtras.map((ext) => ({
            extraId: ext.extraId,
            quantity: ext.quantity,
          })),
        })),
        deliveryFee: 20,
      }

      const response = await fetch('/api/v2/menu/calculate-cart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.error || 'Cart validation failed')
      }

      return await response.json()
    },

    /**
     * Show option picker UI (implement with your UI library)
     */
    async showOptionPicker(optionGroup: OptionGroup): Promise<ModifierItem | null> {
      // Example: Show modal with radio buttons
      return new Promise((resolve) => {
        // Your UI implementation here
        // For demo, just return first item
        const firstItem = optionGroup.items[0]
        resolve(firstItem)
      })
    },

    /**
     * Show extras picker UI (implement with your UI library)
     */
    async showExtrasPicker(
      extraGroup: ExtraGroup
    ): Promise<Array<{ item: ModifierItem; quantity: number }>> {
      // Example: Show modal with checkboxes and quantity pickers
      return new Promise((resolve) => {
        // Your UI implementation here
        // For demo, return empty (user skips all extras)
        resolve([])
      })
    },
  },
})

// ==================== EXAMPLE USAGE IN COMPONENT ====================

/**
 * Example: Product Page Component
 */
/*
<script setup lang="ts">
import { useCartStore } from '~/stores/cart'
import { ref, computed } from 'vue'

const cartStore = useCartStore()
const selectedVariant = ref<Variant | null>(null)

const props = defineProps<{
  menuItem: MenuItem
}>()

// Auto-select default variant
onMounted(() => {
  selectedVariant.value = props.menuItem.variants.find(v => v.default) || props.menuItem.variants[0]
})

async function addToCart() {
  if (!selectedVariant.value) return

  try {
    await cartStore.addItemToCart(props.menuItem, selectedVariant.value)
    // Show success message
    console.log('Added to cart!')
  } catch (error) {
    // Show error message
    console.error('Failed to add to cart:', error)
  }
}

// Preview what options/extras this variant will need
const requiredOptions = computed(() => {
  if (!selectedVariant.value || !cartStore.menu) return []

  return selectedVariant.value.optionIndices.map(index =>
    cartStore.menu!.options[index]
  )
})

const availableExtras = computed(() => {
  if (!selectedVariant.value || !cartStore.menu) return []

  return selectedVariant.value.extraIndices.map(index =>
    cartStore.menu!.extras[index]
  )
})
</script>

<template>
  <div class="product-page">
    <h1>{{ menuItem.name }}</h1>
    <p>{{ menuItem.description }}</p>

    <!-- Variant Selector -->
    <div class="variant-selector">
      <label>Size:</label>
      <select v-model="selectedVariant">
        <option v-for="variant in menuItem.variants" :key="variant.id" :value="variant">
          {{ variant.name }} - R{{ variant.price }}
        </option>
      </select>
    </div>

    <!-- Show what the user will be asked to select -->
    <div v-if="selectedVariant" class="modifier-preview">
      <h3>Customization Options:</h3>

      <div v-if="requiredOptions.length">
        <h4>Required Choices:</h4>
        <ul>
          <li v-for="option in requiredOptions" :key="option.id">
            {{ option.label }}
          </li>
        </ul>
      </div>

      <div v-if="availableExtras.length">
        <h4>Optional Add-ons:</h4>
        <ul>
          <li v-for="extra in availableExtras" :key="extra.id">
            {{ extra.label }}
          </li>
        </ul>
      </div>
    </div>

    <button @click="addToCart" class="add-to-cart-btn">
      Add to Cart - R{{ selectedVariant?.price }}
    </button>
  </div>
</template>
*/

// ==================== EXAMPLE: CART DISPLAY ====================

/**
 * Example: Cart Summary Component
 */
/*
<script setup lang="ts">
import { useCartStore } from '~/stores/cart'

const cartStore = useCartStore()

function formatPrice(amount: number): string {
  return `R${amount.toFixed(2)}`
}

async function checkout() {
  try {
    // Validate cart with backend
    const validation = await cartStore.validateCart()
    console.log('Cart validated:', validation)

    // Proceed to payment
    // navigateTo('/checkout')
  } catch (error) {
    console.error('Cart validation failed:', error)
    alert('There was a problem with your cart. Please review your items.')
  }
}
</script>

<template>
  <div class="cart">
    <h2>Your Cart ({{ cartStore.cartItemCount }} items)</h2>

    <div v-for="(item, index) in cartStore.items" :key="index" class="cart-item">
      <div class="item-details">
        <h3>{{ item.menuItemName }}</h3>
        <p class="variant">{{ item.variantName }}</p>

        <!-- Show selected options -->
        <div v-if="item.selectedOptions.length" class="modifiers">
          <strong>Selections:</strong>
          <ul>
            <li v-for="option in item.selectedOptions" :key="option.optionId">
              {{ option.groupName }}: {{ option.optionName }}
              <span v-if="option.price > 0">(+{{ formatPrice(option.price) }})</span>
            </li>
          </ul>
        </div>

        <!-- Show selected extras -->
        <div v-if="item.selectedExtras.length" class="modifiers">
          <strong>Extras:</strong>
          <ul>
            <li v-for="extra in item.selectedExtras" :key="extra.extraId">
              {{ extra.quantity }}x {{ extra.extraName }} (+{{ formatPrice(extra.totalPrice) }})
            </li>
          </ul>
        </div>
      </div>

      <div class="item-actions">
        <div class="quantity">
          <button @click="cartStore.updateQuantity(index, item.quantity - 1)">-</button>
          <span>{{ item.quantity }}</span>
          <button @click="cartStore.updateQuantity(index, item.quantity + 1)">+</button>
        </div>
        <p class="price">{{ formatPrice(item.itemTotal) }}</p>
        <button @click="cartStore.removeItem(index)" class="remove-btn">Remove</button>
      </div>
    </div>

    <div class="cart-summary">
      <div class="line-item">
        <span>Subtotal:</span>
        <span>{{ formatPrice(cartStore.cartSubtotal) }}</span>
      </div>
      <div class="line-item">
        <span>Delivery Fee:</span>
        <span>{{ formatPrice(20) }}</span>
      </div>
      <div class="line-item total">
        <strong>Total:</strong>
        <strong>{{ formatPrice(cartStore.cartTotal) }}</strong>
      </div>
    </div>

    <button @click="checkout" class="checkout-btn">
      Proceed to Checkout
    </button>
  </div>
</template>
*/

export {}

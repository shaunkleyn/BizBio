<template>
  <div>
    <!-- Overlay -->
    <div
      v-if="cartStore.isCartOpen"
      class="fixed inset-0 bg-black bg-opacity-50 z-40"
      @click="cartStore.closeCart()"
    ></div>

    <!-- Drawer -->
    <div
      :class="[
        'fixed top-0 right-0 h-full w-full md:w-96 mesh-card bg-md-surface shadow-md-5 z-50 transform transition-transform duration-300 flex flex-col',
        cartStore.isCartOpen ? 'translate-x-0' : 'translate-x-full'
      ]"
    >
      <!-- Header -->
      <div class="flex items-center justify-between p-4 border-b border-md-outline-variant flex-shrink-0 bg-gradient-primary">
        <h2 class="text-xl font-bold text-white">Your Cart</h2>
        <button
          @click="cartStore.closeCart()"
          class="w-10 h-10 flex items-center justify-center text-white hover:bg-white/20 rounded-full transition-colors"
        >
          <i class="fas fa-times"></i>
        </button>
      </div>

      <!-- Empty Cart State -->
      <div
        v-if="cartStore.itemCount === 0"
        class="flex-1 flex flex-col items-center justify-center p-8 text-center relative overflow-hidden"
      >
        <div class="absolute inset-0 mesh-bg-2 opacity-20"></div>
        <i class="fas fa-shopping-cart text-6xl text-md-on-surface-variant/30 mb-4 relative z-10"></i>
        <h3 class="text-xl font-semibold text-md-on-surface mb-2 relative z-10">Your cart is empty</h3>
        <p class="text-md-on-surface-variant mb-6 relative z-10">Add some delicious items to get started</p>
        <button
          @click="cartStore.closeCart()"
          class="px-6 py-3 btn-gradient rounded-xl hover:shadow-glow-purple transition-all relative z-10"
        >
          Continue Shopping
        </button>
      </div>

      <!-- Cart Items -->
      <div v-else class="flex-1 overflow-y-auto">
        <div class="p-4 space-y-4">
          <div
            v-for="item in cartStore.items"
            :key="item.id"
            class="bg-gray-50 rounded-lg p-4 border border-gray-200"
          >
            <div class="flex gap-3 mb-3">
              <!-- Item Image -->
              <div class="w-16 h-16 bg-gradient-to-br from-gray-200 to-gray-300 rounded-lg overflow-hidden flex-shrink-0">
                <img
                  v-if="item.image"
                  :src="item.image"
                  :alt="item.name"
                  class="w-full h-full object-cover"
                />
                <div v-else class="w-full h-full flex items-center justify-center">
                  <i class="fas fa-utensils text-gray-400"></i>
                </div>
              </div>

              <!-- Item Info -->
              <div class="flex-1 min-w-0">
                <div class="flex items-start justify-between mb-1">
                  <h3 class="font-semibold text-gray-900 flex-1 pr-2">{{ item.name }}</h3>
                  <button
                    @click="cartStore.removeItem(item.id)"
                    class="text-red-500 hover:text-red-700 flex-shrink-0"
                  >
                    <i class="fas fa-trash-alt text-sm"></i>
                  </button>
                </div>

                <!-- Variant -->
                <div v-if="item.variant" class="text-sm text-gray-600 mb-1">
                  <i class="fas fa-tag mr-1"></i>
                  {{ item.variant.name }}
                </div>

                <!-- Options -->
                <div v-if="item.options && item.options.length > 0" class="text-sm text-gray-600 mb-2">
                  <div v-for="option in item.options" :key="option.id" class="flex items-start gap-1">
                    <i class="fas fa-plus text-xs mt-0.5"></i>
                    <span>{{ option.optionName }}</span>
                  </div>
                </div>

                <!-- Special Instructions -->
                <div v-if="item.specialInstructions" class="text-sm text-gray-600 italic mb-2">
                  <i class="fas fa-comment-dots mr-1"></i>
                  "{{ item.specialInstructions }}"
                </div>

                <!-- Bundle Badge -->
                <div v-if="item.isBundle" class="mb-2">
                  <span class="px-2 py-0.5 bg-orange-100 text-orange-800 text-xs font-semibold rounded">
                    BUNDLE
                  </span>
                </div>

                <!-- Price and Quantity -->
                <div class="flex items-center justify-between mt-2">
                  <!-- Quantity Controls -->
                  <div class="flex items-center gap-2">
                    <button
                      @click="cartStore.updateQuantity(item.id, item.quantity - 1)"
                      class="w-8 h-8 rounded-full border border-[var(--primary-color)] text-[var(--primary-color)] flex items-center justify-center hover:bg-[var(--primary-color)] hover:text-white transition-colors"
                    >
                      <i class="fas fa-minus text-xs"></i>
                    </button>
                    <span class="font-semibold text-gray-900 w-8 text-center">{{ item.quantity }}</span>
                    <button
                      @click="cartStore.updateQuantity(item.id, item.quantity + 1)"
                      class="w-8 h-8 rounded-full border border-[var(--primary-color)] text-[var(--primary-color)] flex items-center justify-center hover:bg-[var(--primary-color)] hover:text-white transition-colors"
                    >
                      <i class="fas fa-plus text-xs"></i>
                    </button>
                  </div>

                  <!-- Item Total -->
                  <div class="font-bold text-[var(--primary-color)]">
                    R{{ calculateItemTotal(item).toFixed(2) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer with Totals and Checkout -->
      <div v-if="cartStore.itemCount > 0" class="border-t border-md-outline-variant p-4 bg-md-surface-container flex-shrink-0">
        <!-- Clear Cart Button -->
        <button
          @click="confirmClearCart"
          class="w-full mb-4 px-4 py-2 text-red-600 border border-red-300 rounded-lg hover:bg-red-50 transition-colors text-sm font-medium"
        >
          <i class="fas fa-trash mr-2"></i>
          Clear Cart
        </button>

        <!-- Totals -->
        <div class="space-y-2 mb-4">
          <div class="flex justify-between text-gray-600">
            <span>Subtotal</span>
            <span>R{{ cartStore.subtotal.toFixed(2) }}</span>
          </div>
          <div v-if="cartStore.deliveryFee > 0" class="flex justify-between text-gray-600">
            <span>Delivery Fee</span>
            <span>R{{ cartStore.deliveryFee.toFixed(2) }}</span>
          </div>
          <div v-if="cartStore.serviceFee > 0" class="flex justify-between text-gray-600">
            <span>Service Fee</span>
            <span>R{{ cartStore.serviceFee.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between text-xl font-bold text-gray-900 pt-2 border-t border-gray-200">
            <span>Total</span>
            <span class="text-[var(--primary-color)]">R{{ cartStore.total.toFixed(2) }}</span>
          </div>
        </div>

        <!-- Checkout Button -->
        <button
          @click="checkout"
          class="w-full py-4 btn-gradient font-semibold rounded-xl hover:shadow-glow-purple transition-all"
        >
          <i class="fas fa-shopping-bag mr-2"></i>
          Proceed to Checkout
        </button>
      </div>
    </div>

    <!-- Clear Cart Confirmation Modal -->
    <div
      v-if="showClearConfirm"
      class="fixed inset-0 bg-black bg-opacity-50 z-60 flex items-center justify-center p-4"
      @click="showClearConfirm = false"
    >
      <div
        class="mesh-card bg-md-surface rounded-2xl p-6 max-w-sm w-full shadow-md-5"
        @click.stop
      >
        <h3 class="text-xl font-bold text-md-on-surface mb-2">Clear Cart?</h3>
        <p class="text-md-on-surface-variant mb-6">
          Are you sure you want to remove all items from your cart? This action cannot be undone.
        </p>
        <div class="flex gap-3">
          <button
            @click="showClearConfirm = false"
            class="flex-1 px-4 py-2 bg-md-surface-container border border-md-outline-variant text-md-on-surface-variant rounded-xl hover:bg-md-surface-container-high transition-colors"
          >
            Cancel
          </button>
          <button
            @click="clearCart"
            class="flex-1 px-4 py-2 bg-gradient-to-r from-red-500 to-red-600 text-white rounded-xl hover:shadow-glow-pink transition-all"
          >
            Clear Cart
          </button>
        </div>
      </div>
    </div>

    <!-- Checkout Modal -->
    <div
      v-if="showCheckoutModal"
      class="fixed inset-0 bg-black bg-opacity-50 z-60 flex items-end md:items-center justify-center"
      @click="showCheckoutModal = false"
    >
      <div
        class="mesh-card bg-md-surface w-full md:max-w-lg md:rounded-2xl rounded-t-2xl max-h-[90vh] overflow-y-auto shadow-md-5"
        @click.stop
      >
        <!-- Header -->
        <div class="sticky top-0 bg-gradient-primary border-b border-md-outline-variant p-4 flex items-center justify-between">
          <h3 class="text-xl font-bold text-white">Checkout</h3>
          <button
            @click="showCheckoutModal = false"
            class="w-8 h-8 flex items-center justify-center text-white hover:bg-white/20 rounded-full transition-colors"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Form -->
        <div class="p-4 space-y-4">
          <!-- Customer Name -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Name <span class="text-red-500">*</span>
            </label>
            <input
              v-model="customerName"
              type="text"
              placeholder="Enter your name"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
            />
          </div>

          <!-- Customer Phone -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Phone Number <span class="text-red-500">*</span>
            </label>
            <input
              v-model="customerPhone"
              type="tel"
              placeholder="Enter your phone number"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
            />
          </div>

          <!-- Customer Email -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Email (Optional)
            </label>
            <input
              v-model="customerEmail"
              type="email"
              placeholder="Enter your email"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
            />
          </div>

          <!-- Delivery Address -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Delivery Address (Optional)
            </label>
            <textarea
              v-model="deliveryAddress"
              placeholder="Enter delivery address"
              rows="2"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent resize-none"
            ></textarea>
          </div>

          <!-- Order Notes -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Order Notes (Optional)
            </label>
            <textarea
              v-model="orderNotes"
              placeholder="Any special requests?"
              rows="3"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent resize-none"
            ></textarea>
          </div>

          <!-- Order Summary -->
          <div class="border-t border-gray-200 pt-4">
            <h4 class="font-semibold text-gray-900 mb-2">Order Summary</h4>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-gray-600">Subtotal</span>
                <span class="font-medium">R{{ cartStore.subtotal.toFixed(2) }}</span>
              </div>
              <div v-if="cartStore.deliveryFee > 0" class="flex justify-between">
                <span class="text-gray-600">Delivery Fee</span>
                <span class="font-medium">R{{ cartStore.deliveryFee.toFixed(2) }}</span>
              </div>
              <div v-if="cartStore.serviceFee > 0" class="flex justify-between">
                <span class="text-gray-600">Service Fee</span>
                <span class="font-medium">R{{ cartStore.serviceFee.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold pt-2 border-t border-gray-200">
                <span>Total</span>
                <span class="text-[var(--primary-color)]">R{{ cartStore.total.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="sticky bottom-0 bg-md-surface-container border-t border-md-outline-variant p-4 flex gap-3">
          <button
            @click="showCheckoutModal = false"
            class="flex-1 px-4 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface-variant rounded-xl hover:bg-md-surface-container-high transition-colors font-medium"
          >
            Cancel
          </button>
          <button
            @click="completeCheckout"
            :disabled="checkoutLoading"
            class="flex-1 px-4 py-3 btn-gradient rounded-xl hover:shadow-glow-purple transition-all font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="checkoutLoading">
              <i class="fas fa-spinner fa-spin mr-2"></i>
              Processing...
            </span>
            <span v-else>
              Place Order
            </span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useCartStore } from '~/stores/cart'
import { useToast } from '~/composables/useToast'

const cartStore = useCartStore()
const toast = useToast()

const showClearConfirm = ref(false)
const showCheckoutModal = ref(false)
const customerName = ref('')
const customerPhone = ref('')
const customerEmail = ref('')
const deliveryAddress = ref('')
const orderNotes = ref('')
const checkoutLoading = ref(false)

function calculateItemTotal(item: any) {
  const variantPrice = item.variant?.price || 0
  const optionsPrice = item.options?.reduce((sum: number, opt: any) => sum + opt.price, 0) || 0
  return (item.basePrice + variantPrice + optionsPrice) * item.quantity
}

function confirmClearCart() {
  showClearConfirm.value = true
}

function clearCart() {
  cartStore.clearCart()
  showClearConfirm.value = false
  toast.success('Cart cleared')
}

function checkout() {
  showCheckoutModal.value = true
}

async function completeCheckout() {
  // Validate customer info
  if (!customerName.value.trim()) {
    toast.error('Please enter your name')
    return
  }

  if (!customerPhone.value.trim()) {
    toast.error('Please enter your phone number')
    return
  }

  try {
    checkoutLoading.value = true

    // Prepare order data
    const orderData = {
      customer: {
        name: customerName.value,
        phone: customerPhone.value,
        email: customerEmail.value || null
      },
      items: cartStore.items.map(item => ({
        catalogItemId: item.catalogItemId,
        name: item.name,
        quantity: item.quantity,
        basePrice: item.basePrice,
        variant: item.variant,
        options: item.options,
        specialInstructions: item.specialInstructions,
        isBundle: item.isBundle,
        bundleSelections: item.bundleSelections
      })),
      deliveryAddress: deliveryAddress.value || null,
      notes: orderNotes.value || null,
      subtotal: cartStore.subtotal,
      deliveryFee: cartStore.deliveryFee,
      serviceFee: cartStore.serviceFee,
      total: cartStore.total,
      orderedAt: new Date().toISOString()
    }

    // For now, save to localStorage and show success
    // In production, this would call an API endpoint
    const orders = JSON.parse(localStorage.getItem('orders') || '[]')
    orders.push(orderData)
    localStorage.setItem('orders', JSON.stringify(orders))

    // Clear cart and close modals
    cartStore.clearCart()
    showCheckoutModal.value = false
    cartStore.closeCart()

    // Show success message
    toast.success('Order placed successfully! We will contact you shortly.')

    // Reset form
    customerName.value = ''
    customerPhone.value = ''
    customerEmail.value = ''
    deliveryAddress.value = ''
    orderNotes.value = ''
  } catch (error) {
    console.error('Checkout error:', error)
    toast.error('Failed to place order. Please try again.')
  } finally {
    checkoutLoading.value = false
  }
}
</script>

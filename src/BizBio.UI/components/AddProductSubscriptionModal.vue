<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" @click.self="closeModal">
    <div class="flex min-h-screen items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" @click="closeModal"></div>

      <!-- Modal -->
      <div class="relative bg-white rounded-lg shadow-xl max-w-6xl w-full max-h-[90vh] overflow-hidden">
        <!-- Header -->
        <div class="bg-gradient-to-r from-purple-600 to-indigo-600 p-6 text-white">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-2xl font-bold">Add New Product</h2>
              <p class="text-sm opacity-90 mt-1">
                Expand your business with additional BizBio products
              </p>
            </div>
            <button
              @click="closeModal"
              class="text-white hover:text-gray-200 transition-colors"
            >
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
              </svg>
            </button>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="p-12 text-center">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600 mx-auto"></div>
          <p class="mt-4 text-gray-600">Loading available products...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="p-12 text-center">
          <div class="text-red-500 mb-4">
            <svg class="w-16 h-16 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
          <p class="text-gray-700 font-medium">{{ error }}</p>
          <button
            @click="loadAvailableProducts"
            class="mt-4 px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
          >
            Try Again
          </button>
        </div>

        <!-- Products List -->
        <div v-else class="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          <!-- Product Selection (Step 1) -->
          <div v-if="!selectedProduct" class="space-y-6">
            <div>
              <h3 class="text-lg font-semibold text-gray-900 mb-4">Choose a Product</h3>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div
                  v-for="product in availableProducts"
                  :key="product.type"
                  @click="selectProduct(product)"
                  class="border-2 border-gray-200 rounded-lg p-6 cursor-pointer hover:border-purple-500 hover:shadow-lg transition-all"
                >
                  <div class="text-center">
                    <!-- Product Icon -->
                    <div class="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-purple-100 to-indigo-100 rounded-full flex items-center justify-center">
                      <component :is="product.icon" class="w-8 h-8 text-purple-600" />
                    </div>

                    <!-- Product Name -->
                    <h4 class="text-xl font-bold text-gray-900 mb-2">{{ product.name }}</h4>
                    <p class="text-sm text-gray-600 mb-4">{{ product.description }}</p>

                    <!-- Key Features -->
                    <div class="text-left space-y-2 mb-4">
                      <div v-for="feature in product.keyFeatures" :key="feature" class="flex items-start text-sm">
                        <svg class="w-4 h-4 text-green-500 mr-2 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                        </svg>
                        <span class="text-gray-700">{{ feature }}</span>
                      </div>
                    </div>

                    <!-- Select Button -->
                    <button class="w-full py-2 px-4 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors font-medium">
                      Select {{ product.name }}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Plan Selection (Step 2) -->
          <div v-else>
            <!-- Back Button -->
            <button
              @click="selectedProduct = null"
              class="mb-6 flex items-center text-gray-600 hover:text-gray-900 transition-colors"
            >
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/>
              </svg>
              Back to Product Selection
            </button>

            <!-- Selected Product Info -->
            <div class="mb-6 p-4 bg-purple-50 border border-purple-200 rounded-lg">
              <div class="flex items-center">
                <component :is="selectedProduct.icon" class="w-10 h-10 text-purple-600 mr-4" />
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ selectedProduct.name }}</h3>
                  <p class="text-sm text-gray-600">{{ selectedProduct.description }}</p>
                </div>
              </div>
            </div>

            <!-- Loading Tiers -->
            <div v-if="loadingTiers" class="p-12 text-center">
              <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600 mx-auto"></div>
              <p class="mt-4 text-gray-600">Loading plans...</p>
            </div>

            <!-- Tier Selection -->
            <div v-else>
              <h3 class="text-lg font-semibold text-gray-900 mb-4">Choose Your Plan</h3>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div
                  v-for="tier in productTiers"
                  :key="tier.id"
                  :class="[
                    'border-2 rounded-lg p-6 transition-all cursor-pointer',
                    tier.id === selectedTierId
                      ? 'border-purple-500 bg-purple-50 shadow-lg transform scale-105'
                      : 'border-gray-200 hover:border-purple-500 hover:shadow-md'
                  ]"
                  @click="selectedTierId = tier.id"
                >
                  <!-- Tier Header -->
                  <div class="mb-4">
                    <h4 class="text-xl font-bold text-gray-900">{{ tier.tierName }}</h4>
                    <p class="text-sm text-gray-600 mt-1">{{ tier.description }}</p>
                  </div>

                  <!-- Pricing -->
                  <div class="mb-6">
                    <div class="flex items-baseline">
                      <span class="text-3xl font-bold text-gray-900">R{{ tier.monthlyPrice }}</span>
                      <span class="text-gray-600 ml-2">/month</span>
                    </div>
                    <div v-if="tier.annualPrice > 0" class="text-sm text-gray-600 mt-1">
                      or R{{ tier.annualPrice }}/year
                    </div>
                    <div v-if="tier.trialDays > 0" class="mt-2">
                      <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        {{ tier.trialDays }}-day free trial
                      </span>
                    </div>
                  </div>

                  <!-- Features -->
                  <div class="space-y-2 mb-6">
                    <div class="flex items-center text-sm">
                      <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                      </svg>
                      <span>{{ tier.maxEntities }} {{ tier.maxEntities === 1 ? 'Business' : 'Businesses' }}</span>
                    </div>
                    <div class="flex items-center text-sm">
                      <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                      </svg>
                      <span>{{ tier.maxCatalogsPerEntity }} Catalogs per business</span>
                    </div>
                    <div class="flex items-center text-sm">
                      <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                      </svg>
                      <span>{{ tier.maxLibraryItems }} Library items</span>
                    </div>
                  </div>

                  <!-- Select Button -->
                  <button
                    @click.stop="selectedTierId = tier.id"
                    :class="[
                      'w-full py-3 px-4 rounded-lg font-medium transition-colors',
                      tier.id === selectedTierId
                        ? 'bg-purple-600 text-white'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    ]"
                  >
                    {{ tier.id === selectedTierId ? 'Selected' : 'Select Plan' }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div v-if="!loading && !error && selectedProduct" class="border-t border-gray-200 p-6 bg-gray-50">
          <div class="flex items-center justify-between">
            <button
              @click="closeModal"
              class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="confirmSubscription"
              :disabled="!selectedTierId || subscribing"
              :class="[
                'px-8 py-2 rounded-lg font-medium transition-colors',
                !selectedTierId || subscribing
                  ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                  : 'bg-purple-600 text-white hover:bg-purple-700'
              ]"
            >
              <span v-if="subscribing">
                <svg class="animate-spin h-5 w-5 inline mr-2" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Subscribing...
              </span>
              <span v-else>Subscribe Now</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, h } from 'vue'

const props = defineProps<{
  isOpen: boolean
  currentSubscriptions: any[]
}>()

const emit = defineEmits(['close', 'subscription-success'])

const loading = ref(false)
const error = ref('')
const selectedProduct = ref<any>(null)
const loadingTiers = ref(false)
const productTiers = ref<any[]>([])
const selectedTierId = ref<number | null>(null)
const subscribing = ref(false)

// Define available products
const allProducts = [
  {
    type: 1, // Menu
    name: 'Menu',
    description: 'Digital menus for restaurants and cafes',
    icon: h('svg', { class: 'w-full h-full', fill: 'currentColor', viewBox: '0 0 20 20' }, [
      h('path', { d: 'M9 2a1 1 0 000 2h2a1 1 0 100-2H9z' }),
      h('path', { 'fill-rule': 'evenodd', d: 'M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z', 'clip-rule': 'evenodd' })
    ]),
    keyFeatures: [
      'QR code menu access',
      'Unlimited menu items',
      'Category management',
      'Real-time updates'
    ]
  },
  {
    type: 0, // Cards (Business Cards)
    name: 'Cards',
    description: 'Digital business cards and contact sharing',
    icon: h('svg', { class: 'w-full h-full', fill: 'currentColor', viewBox: '0 0 20 20' }, [
      h('path', { 'fill-rule': 'evenodd', d: 'M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zm2 6a2 2 0 012-2h8a2 2 0 012 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2v-4zm6 4a2 2 0 100-4 2 2 0 000 4z', 'clip-rule': 'evenodd' })
    ]),
    keyFeatures: [
      'Digital business cards',
      'Contact sharing',
      'Custom branding',
      'Analytics tracking'
    ]
  },
  {
    type: 2, // Retail
    name: 'Retail',
    description: 'E-commerce and online store management',
    icon: h('svg', { class: 'w-full h-full', fill: 'currentColor', viewBox: '0 0 20 20' }, [
      h('path', { 'fill-rule': 'evenodd', d: 'M10 2a4 4 0 00-4 4v1H5a1 1 0 00-.994.89l-1 9A1 1 0 004 18h12a1 1 0 00.994-1.11l-1-9A1 1 0 0015 7h-1V6a4 4 0 00-4-4zm2 5V6a2 2 0 10-4 0v1h4zm-6 3a1 1 0 112 0 1 1 0 01-2 0zm7-1a1 1 0 100 2 1 1 0 000-2z', 'clip-rule': 'evenodd' })
    ]),
    keyFeatures: [
      'Online store',
      'Product catalogs',
      'Inventory management',
      'Order processing'
    ]
  }
]

const availableProducts = computed(() => {
  const subscribedTypes = props.currentSubscriptions.map((s: any) => s.productType)
  return allProducts.filter(p => !subscribedTypes.includes(p.type))
})

const loadAvailableProducts = async () => {
  loading.value = true
  error.value = ''
  // Products are hardcoded, so just simulate loading
  setTimeout(() => {
    loading.value = false
  }, 300)
}

const selectProduct = async (product: any) => {
  selectedProduct.value = product
  await loadProductTiers(product.type)
}

const loadProductTiers = async (productType: number) => {
  loadingTiers.value = true
  error.value = ''

  try {
    const { $api } = useNuxtApp()
    const response = await $api.get(`/api/v1/subscriptions/tiers/${productType}`)

    if (response.success) {
      productTiers.value = response.tiers
    } else {
      error.value = 'Failed to load plans'
    }
  } catch (err: any) {
    console.error('Error loading tiers:', err)
    error.value = err.response?.data?.error || 'Failed to load plans'
  } finally {
    loadingTiers.value = false
  }
}

const confirmSubscription = async () => {
  if (!selectedTierId.value || !selectedProduct.value) return

  subscribing.value = true
  error.value = ''

  try {
    const { $api } = useNuxtApp()
    const response = await $api.post('/api/v1/subscriptions', {
      productType: selectedProduct.value.type,
      tierId: selectedTierId.value
    })

    if (response.success) {
      emit('subscription-success', response.subscription)
      closeModal()
    } else {
      error.value = response.error || 'Failed to create subscription'
    }
  } catch (err: any) {
    console.error('Error creating subscription:', err)
    error.value = err.response?.data?.error || 'Failed to create subscription'
  } finally {
    subscribing.value = false
  }
}

const closeModal = () => {
  if (!subscribing.value) {
    selectedProduct.value = null
    selectedTierId.value = null
    error.value = ''
    emit('close')
  }
}

watch(() => props.isOpen, (newValue) => {
  if (newValue) {
    loadAvailableProducts()
  }
})

onMounted(() => {
  if (props.isOpen) {
    loadAvailableProducts()
  }
})
</script>

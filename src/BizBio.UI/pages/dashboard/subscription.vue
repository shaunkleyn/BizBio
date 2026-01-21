<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="mb-8 flex items-center justify-between">
        <div>
          <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
            Product Subscriptions
          </h1>
          <p class="text-lg text-[var(--gray-text-color)]">
            Manage your product subscriptions and view usage limits
          </p>
        </div>
        <button
          v-if="subscriptions.length > 0 && !isLoading && !error"
          @click="showAddProductModal = true"
          class="px-6 py-3 btn-gradient text-white rounded-lg font-semibold shadow-md hover:shadow-lg transition-all"
        >
          <i class="fas fa-plus mr-2"></i>
          Add Product
        </button>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="text-center py-12">
        <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)] mb-4"></i>
        <p class="text-[var(--gray-text-color)]">Loading your subscriptions...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-xl p-6 mb-8">
        <div class="flex items-center gap-4">
          <i class="fas fa-exclamation-circle text-3xl text-red-500"></i>
          <div>
            <h3 class="text-lg font-bold text-red-800 mb-1">Error Loading Subscriptions</h3>
            <p class="text-red-600">{{ error }}</p>
            <button
              @click="loadSubscriptions"
              class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              <i class="fas fa-redo mr-2"></i>
              Try Again
            </button>
          </div>
        </div>
      </div>

      <!-- Subscriptions List -->
      <div v-else>
        <!-- No Subscriptions -->
        <div v-if="subscriptions.length === 0" class="bg-md-surface rounded-2xl shadow-lg p-12 text-center">
          <div class="inline-flex items-center justify-center w-24 h-24 rounded-full bg-[var(--primary-color)] bg-opacity-10 mb-6">
            <i class="fas fa-shopping-cart text-4xl text-[var(--primary-color)]"></i>
          </div>
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-3">
            No Active Subscriptions
          </h2>
          <p class="text-[var(--gray-text-color)] mb-8 max-w-md mx-auto">
            You don't have any active product subscriptions yet. Subscribe to get started with our products.
          </p>
          <button
            @click="navigateToProducts"
            class="px-6 py-3 btn-gradient text-white rounded-lg font-semibold shadow-md hover:shadow-lg transition-all"
          >
            <i class="fas fa-plus mr-2"></i>
            Browse Products
          </button>
        </div>

        <!-- Active Subscriptions -->
        <div v-else class="space-y-6">
          <div
            v-for="sub in subscriptions"
            :key="sub.id"
            class="bg-md-surface rounded-2xl shadow-lg overflow-hidden hover:shadow-xl transition-shadow"
          >
            <!-- Subscription Header -->
            <div class="bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] p-6">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-4">
                  <div class="w-16 h-16 rounded-full bg-white bg-opacity-20 flex items-center justify-center">
                    <i :class="getProductIcon(sub.productType)" class="text-2xl text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-2xl font-bold text-white mb-1">
                      {{ getProductName(sub.productType) }}
                    </h3>
                    <p class="text-white text-opacity-90">
                      {{ sub.tierName }} Plan
                    </p>
                  </div>
                </div>
                <div :class="[
                  'px-4 py-2 rounded-lg font-semibold text-sm',
                  getStatusColor(sub.status)
                ]">
                  {{ getStatusName(sub.status) }}
                </div>
              </div>
            </div>

            <!-- Subscription Body -->
            <div class="p-6">
              <!-- Trial Info -->
              <div v-if="sub.isTrialActive" class="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
                <div class="flex items-center gap-3">
                  <i class="fas fa-gift text-2xl text-yellow-600"></i>
                  <div class="flex-1">
                    <h4 class="font-bold text-yellow-900 mb-1">
                      Free Trial Active
                    </h4>
                    <p class="text-sm text-yellow-700">
                      {{ sub.trialDaysRemaining }} {{ sub.trialDaysRemaining === 1 ? 'day' : 'days' }} remaining
                      (ends {{ formatDate(sub.trialEndDate) }})
                    </p>
                  </div>
                </div>
              </div>

              <!-- Usage Stats -->
              <div class="grid md:grid-cols-3 gap-6 mb-6">
                <!-- Entities Usage -->
                <div class="bg-gray-50 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-semibold text-gray-600">Businesses</span>
                    <i class="fas fa-building text-gray-400"></i>
                  </div>
                  <div class="flex items-baseline gap-2">
                    <span class="text-3xl font-bold text-[var(--dark-text-color)]">
                      {{ sub.usage?.entities || 0 }}
                    </span>
                    <span class="text-gray-500">
                      / {{ sub.limits?.maxEntities || '∞' }}
                    </span>
                  </div>
                  <div class="mt-3 bg-gray-200 rounded-full h-2 overflow-hidden">
                    <div
                      :style="{ width: getUsagePercentage(sub.usage?.entities, sub.limits?.maxEntities) + '%' }"
                      :class="getUsageBarColor(sub.usage?.entities, sub.limits?.maxEntities)"
                      class="h-full transition-all"
                    ></div>
                  </div>
                </div>

                <!-- Catalogs Usage -->
                <div class="bg-gray-50 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-semibold text-gray-600">Menus</span>
                    <i class="fas fa-book text-gray-400"></i>
                  </div>
                  <div class="flex items-baseline gap-2">
                    <span class="text-3xl font-bold text-[var(--dark-text-color)]">
                      {{ sub.usage?.catalogs || 0 }}
                    </span>
                    <span class="text-gray-500">
                      / {{ sub.limits?.maxCatalogsPerEntity * sub.limits?.maxEntities || '∞' }}
                    </span>
                  </div>
                  <div class="mt-3 bg-gray-200 rounded-full h-2 overflow-hidden">
                    <div
                      :style="{ width: getUsagePercentage(sub.usage?.catalogs, sub.limits?.maxCatalogsPerEntity * sub.limits?.maxEntities) + '%' }"
                      :class="getUsageBarColor(sub.usage?.catalogs, sub.limits?.maxCatalogsPerEntity * sub.limits?.maxEntities)"
                      class="h-full transition-all"
                    ></div>
                  </div>
                </div>

                <!-- Library Items -->
                <div class="bg-gray-50 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-semibold text-gray-600">Library Items</span>
                    <i class="fas fa-utensils text-gray-400"></i>
                  </div>
                  <div class="flex items-baseline gap-2">
                    <span class="text-3xl font-bold text-[var(--dark-text-color)]">
                      0
                    </span>
                    <span class="text-gray-500">
                      / {{ sub.limits?.maxLibraryItems || '∞' }}
                    </span>
                  </div>
                  <div class="mt-3 bg-gray-200 rounded-full h-2 overflow-hidden">
                    <div
                      style="width: 0%"
                      class="bg-green-500 h-full transition-all"
                    ></div>
                  </div>
                </div>
              </div>

              <!-- Plan Limits -->
              <div class="bg-blue-50 rounded-lg p-4 mb-6">
                <h4 class="font-bold text-blue-900 mb-3 flex items-center gap-2">
                  <i class="fas fa-info-circle"></i>
                  Plan Limits
                </h4>
                <div class="grid md:grid-cols-2 gap-3 text-sm">
                  <div class="flex items-center gap-2">
                    <i class="fas fa-check text-blue-600"></i>
                    <span class="text-blue-800">
                      {{ sub.limits?.maxEntities }} {{ sub.limits?.maxEntities === 1 ? 'Business' : 'Businesses' }}
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="fas fa-check text-blue-600"></i>
                    <span class="text-blue-800">
                      {{ sub.limits?.maxCatalogsPerEntity }} {{ sub.limits?.maxCatalogsPerEntity === 1 ? 'Menu' : 'Menus' }} per business
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="fas fa-check text-blue-600"></i>
                    <span class="text-blue-800">
                      {{ sub.limits?.maxLibraryItems }} Library items
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="fas fa-check text-blue-600"></i>
                    <span class="text-blue-800">
                      {{ sub.limits?.maxCategoriesPerCatalog }} Categories per menu
                    </span>
                  </div>
                </div>
              </div>

              <!-- Billing Info -->
              <div v-if="!sub.isTrialActive" class="border-t border-gray-200 pt-4">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm text-gray-600">Billing Cycle</p>
                    <p class="font-bold text-[var(--dark-text-color)]">
                      {{ getBillingCycleName(sub.billingCycle) }}
                    </p>
                  </div>
                  <div v-if="sub.nextBillingDate">
                    <p class="text-sm text-gray-600">Next Billing Date</p>
                    <p class="font-bold text-[var(--dark-text-color)]">
                      {{ formatDate(sub.nextBillingDate) }}
                    </p>
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex gap-3 mt-6">
                <button
                  @click="upgradeSubscription(sub)"
                  class="flex-1 px-4 py-3 bg-[var(--primary-color)] text-white rounded-lg font-semibold hover:bg-[var(--primary-button-hover-bg-color)] transition-colors"
                >
                  <i class="fas fa-arrow-up mr-2"></i>
                  Upgrade Plan
                </button>
                <button
                  @click="cancelSubscription(sub)"
                  v-if="sub.status !== 4"
                  class="px-4 py-3 border-2 border-red-500 text-red-500 rounded-lg font-semibold hover:bg-red-50 transition-colors"
                >
                  <i class="fas fa-times mr-2"></i>
                  Cancel
                </button>
              </div>
            </div>
          </div>

          <!-- Invoice Preview -->
          <div v-if="invoicePreview" class="bg-md-surface rounded-2xl shadow-lg p-6">
            <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
              <i class="fas fa-file-invoice-dollar mr-2 text-[var(--primary-color)]"></i>
              Next Invoice Preview
            </h3>
            <div class="space-y-3 mb-6">
              <div
                v-for="(item, index) in invoicePreview.lineItems"
                :key="index"
                class="flex justify-between items-center py-3 border-b border-gray-200"
              >
                <div>
                  <p class="font-semibold text-[var(--dark-text-color)]">{{ item.productType }}</p>
                  <p class="text-sm text-gray-600">{{ item.tierName }} - {{ item.billingCycle }}</p>
                </div>
                <p class="font-bold text-lg text-[var(--dark-text-color)]">
                  R {{ item.amount.toFixed(2) }}
                </p>
              </div>
            </div>
            <div class="space-y-2 pt-4 border-t-2 border-gray-300">
              <div class="flex justify-between text-gray-600">
                <span>Subtotal</span>
                <span>R {{ invoicePreview.subtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-gray-600">
                <span>VAT (15%)</span>
                <span>R {{ invoicePreview.vat.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-2xl font-bold text-[var(--dark-text-color)] pt-2">
                <span>Total</span>
                <span>R {{ invoicePreview.total.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Upgrade Subscription Modal -->
    <SubscriptionUpgradeModal
      :isOpen="showUpgradeModal"
      :currentSubscription="selectedSubscription"
      :productType="selectedSubscription?.productType || 0"
      @close="showUpgradeModal = false"
      @upgrade-success="handleUpgradeSuccess"
    />

    <!-- Add Product Subscription Modal -->
    <AddProductSubscriptionModal
      :isOpen="showAddProductModal"
      @close="showAddProductModal = false"
      @subscription-success="handleSubscriptionSuccess"
    />
  </NuxtLayout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import SubscriptionUpgradeModal from '~/components/SubscriptionUpgradeModal.vue'
import AddProductSubscriptionModal from '~/components/AddProductSubscriptionModal.vue'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Subscription Management' })

const subscriptionApi = useSubscriptionApi()
const router = useRouter()
const toast = useToast()

const isLoading = ref(false)
const error = ref(null)
const subscriptions = ref([])
const invoicePreview = ref(null)

// Modal state
const showUpgradeModal = ref(false)
const showAddProductModal = ref(false)
const selectedSubscription = ref(null)

const getProductName = (type) => {
  const names = {
    0: 'Connect (Business Cards)',
    1: 'Menu',
    2: 'Retail Catalog'
  }
  return names[type] || 'Unknown Product'
}

const getProductIcon = (type) => {
  const icons = {
    0: 'fas fa-id-card',
    1: 'fas fa-utensils',
    2: 'fas fa-shopping-cart'
  }
  return icons[type] || 'fas fa-box'
}

const getStatusName = (status) => {
  const names = {
    0: 'Inactive',
    1: 'Active',
    2: 'Trial',
    3: 'Past Due',
    4: 'Cancelled',
    5: 'Expired'
  }
  return names[status] || 'Unknown'
}

const getStatusColor = (status) => {
  const colors = {
    0: 'bg-gray-200 text-gray-700',
    1: 'bg-green-100 text-green-800',
    2: 'bg-yellow-100 text-yellow-800',
    3: 'bg-red-100 text-red-800',
    4: 'bg-gray-200 text-gray-700',
    5: 'bg-red-100 text-red-800'
  }
  return colors[status] || 'bg-gray-200 text-gray-700'
}

const getBillingCycleName = (cycle) => {
  const names = {
    0: 'Monthly',
    1: 'Annual'
  }
  return names[cycle] || 'Unknown'
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })
}

const getUsagePercentage = (used, max) => {
  if (!max || max === 0) return 0
  return Math.min((used / max) * 100, 100)
}

const getUsageBarColor = (used, max) => {
  const percentage = getUsagePercentage(used, max)
  if (percentage >= 90) return 'bg-red-500'
  if (percentage >= 75) return 'bg-yellow-500'
  return 'bg-green-500'
}

const loadSubscriptions = async () => {
  isLoading.value = true
  error.value = null

  try {
    const response = await subscriptionApi.getMySubscriptions()
    subscriptions.value = response.data.subscriptions || []

    // Load invoice preview if there are active subscriptions
    if (subscriptions.value.length > 0) {
      try {
        const invoiceResponse = await subscriptionApi.getInvoicePreview()
        invoicePreview.value = invoiceResponse.data.invoice
      } catch (err) {
        console.error('Failed to load invoice preview:', err)
      }
    }
  } catch (err) {
    console.error('Failed to load subscriptions:', err)
    error.value = err.response?.data?.error || err.message || 'Failed to load subscriptions'
  } finally {
    isLoading.value = false
  }
}

const navigateToProducts = () => {
  showAddProductModal.value = true
}

const upgradeSubscription = (sub) => {
  selectedSubscription.value = sub
  showUpgradeModal.value = true
}

const handleUpgradeSuccess = () => {
  toast.success('Subscription upgraded successfully!', 'Success')
  loadSubscriptions()
}

const handleSubscriptionSuccess = (newSubscription) => {
  toast.success(`Successfully subscribed to ${getProductName(newSubscription.productType)}!`, 'Success')
  loadSubscriptions()
}

const cancelSubscription = async (sub) => {
  if (!confirm(`Are you sure you want to cancel your ${getProductName(sub.productType)} subscription?`)) {
    return
  }

  try {
    await subscriptionApi.cancelSubscription(sub.productType)
    toast.success('Subscription cancelled successfully', 'Success')
    loadSubscriptions()
  } catch (err) {
    console.error('Failed to cancel subscription:', err)
    toast.error('Failed to cancel subscription', 'Error')
  }
}

onMounted(() => {
  loadSubscriptions()
})
</script>


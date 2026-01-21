<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" @click.self="closeModal">
    <div class="flex min-h-screen items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" @click="closeModal"></div>

      <!-- Modal -->
      <div class="relative bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
        <!-- Header -->
        <div class="bg-gradient-to-r from-primary to-accent p-6 text-white">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-2xl font-bold">Upgrade Your Plan</h2>
              <p class="text-sm opacity-90 mt-1">
                Get more features and higher limits
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
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p class="mt-4 text-gray-600">Loading available plans...</p>
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
            @click="loadTiers"
            class="mt-4 px-6 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors"
          >
            Try Again
          </button>
        </div>

        <!-- Tiers List -->
        <div v-else class="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          <!-- Current Plan Info -->
          <div v-if="currentSubscription" class="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Current Plan</p>
                <p class="text-lg font-semibold text-gray-900">{{ currentSubscription.tierName }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm text-gray-600">Monthly Cost</p>
                <p class="text-lg font-semibold text-gray-900">
                  R {{ currentSubscription.limits.monthlyPrice || currentTierPrice }}
                </p>
              </div>
            </div>
          </div>

          <!-- Available Tiers -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div
              v-for="tier in availableTiers"
              :key="tier.id"
              :class="[
                'relative border-2 rounded-lg p-6 transition-all cursor-pointer',
                tier.id === currentSubscription?.tierId
                  ? 'border-blue-500 bg-blue-50'
                  : tier.id === selectedTierId
                  ? 'border-primary bg-primary-light shadow-lg transform scale-105'
                  : 'border-gray-200 hover:border-primary hover:shadow-md'
              ]"
              @click="selectTier(tier)"
            >
              <!-- Current Plan Badge -->
              <div v-if="tier.id === currentSubscription?.tierId" class="absolute top-4 right-4">
                <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-blue-500 text-white">
                  Current Plan
                </span>
              </div>

              <!-- Tier Header -->
              <div class="mb-4">
                <h3 class="text-xl font-bold text-gray-900">{{ tier.tierName }}</h3>
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
                  <span class="text-green-600 font-medium">(Save {{ calculateAnnualSavings(tier) }}%)</span>
                </div>
              </div>

              <!-- Features/Limits -->
              <div class="space-y-3 mb-6">
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
                  <span>{{ tier.maxCatalogsPerEntity }} {{ tier.maxCatalogsPerEntity === 1 ? 'Catalog' : 'Catalogs' }} per business</span>
                </div>
                <div class="flex items-center text-sm">
                  <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                  </svg>
                  <span>{{ tier.maxLibraryItems }} Library items</span>
                </div>
                <div class="flex items-center text-sm">
                  <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                  </svg>
                  <span>{{ tier.maxCategoriesPerCatalog }} Categories per catalog</span>
                </div>
                <div v-if="tier.trialDays > 0" class="flex items-center text-sm text-blue-600">
                  <svg class="w-5 h-5 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/>
                  </svg>
                  <span class="font-medium">{{ tier.trialDays }}-day free trial</span>
                </div>
              </div>

              <!-- Select Button -->
              <button
                v-if="tier.id !== currentSubscription?.tierId"
                @click.stop="selectTier(tier)"
                :class="[
                  'w-full py-3 px-4 rounded-lg font-medium transition-colors',
                  tier.id === selectedTierId
                    ? 'bg-primary text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                {{ tier.id === selectedTierId ? 'Selected' : 'Select Plan' }}
              </button>
              <button
                v-else
                disabled
                class="w-full py-3 px-4 rounded-lg font-medium bg-gray-100 text-gray-500 cursor-not-allowed"
              >
                Current Plan
              </button>
            </div>
          </div>

          <!-- Pro-rata Information -->
          <div v-if="selectedTierId && selectedTierId !== currentSubscription?.tierId" class="mt-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
            <div class="flex">
              <svg class="w-5 h-5 text-yellow-600 mr-3 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
              </svg>
              <div>
                <p class="text-sm font-medium text-yellow-800">Pro-rata Billing</p>
                <p class="text-sm text-yellow-700 mt-1">
                  You'll be charged a pro-rated amount for the remainder of your current billing period.
                  Your new plan will take effect immediately.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div v-if="!loading && !error" class="border-t border-gray-200 p-6 bg-gray-50">
          <div class="flex items-center justify-between">
            <button
              @click="closeModal"
              class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="confirmUpgrade"
              :disabled="!selectedTierId || selectedTierId === currentSubscription?.tierId || upgrading"
              :class="[
                'px-8 py-2 rounded-lg font-medium transition-colors',
                !selectedTierId || selectedTierId === currentSubscription?.tierId || upgrading
                  ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                  : 'bg-primary text-white hover:bg-primary-dark'
              ]"
            >
              <span v-if="upgrading">
                <svg class="animate-spin h-5 w-5 inline mr-2" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Upgrading...
              </span>
              <span v-else>Upgrade Now</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

const props = defineProps<{
  isOpen: boolean
  currentSubscription: any
  productType: number
}>()

const emit = defineEmits(['close', 'upgrade-success'])

const loading = ref(false)
const error = ref('')
const tiers = ref<any[]>([])
const selectedTierId = ref<number | null>(null)
const upgrading = ref(false)

const availableTiers = computed(() => {
  return tiers.value.filter(tier => tier.isActive)
})

const currentTierPrice = computed(() => {
  if (!props.currentSubscription) return 0
  const currentTier = tiers.value.find(t => t.id === props.currentSubscription.tierId)
  return currentTier?.monthlyPrice || 0
})

const calculateAnnualSavings = (tier: any) => {
  if (!tier.annualPrice || !tier.monthlyPrice) return 0
  const monthlyTotal = tier.monthlyPrice * 12
  const savings = ((monthlyTotal - tier.annualPrice) / monthlyTotal) * 100
  return Math.round(savings)
}

const loadTiers = async () => {
  loading.value = true
  error.value = ''

  try {
    const { $api } = useNuxtApp()
    const response = await $api.get(`/api/v1/subscriptions/tiers/${props.productType}`)

    if (response.success) {
      tiers.value = response.tiers
    } else {
      error.value = 'Failed to load available plans'
    }
  } catch (err: any) {
    console.error('Error loading tiers:', err)
    error.value = err.response?.data?.error || 'Failed to load available plans'
  } finally {
    loading.value = false
  }
}

const selectTier = (tier: any) => {
  if (tier.id === props.currentSubscription?.tierId) return
  selectedTierId.value = tier.id
}

const confirmUpgrade = async () => {
  if (!selectedTierId.value || selectedTierId.value === props.currentSubscription?.tierId) return

  upgrading.value = true
  error.value = ''

  try {
    const { $api } = useNuxtApp()
    const response = await $api.put(
      `/api/v1/subscriptions/${props.productType}/upgrade`,
      { newTierId: selectedTierId.value }
    )

    if (response.success) {
      emit('upgrade-success')
      closeModal()
    } else {
      error.value = response.error || 'Failed to upgrade subscription'
    }
  } catch (err: any) {
    console.error('Error upgrading subscription:', err)
    error.value = err.response?.data?.error || 'Failed to upgrade subscription'
  } finally {
    upgrading.value = false
  }
}

const closeModal = () => {
  if (!upgrading.value) {
    selectedTierId.value = null
    error.value = ''
    emit('close')
  }
}

watch(() => props.isOpen, (newValue) => {
  if (newValue) {
    loadTiers()
  }
})

onMounted(() => {
  if (props.isOpen) {
    loadTiers()
  }
})
</script>

<style scoped>
.bg-primary-light {
  background-color: rgba(59, 130, 246, 0.05);
}

.bg-primary-dark {
  background-color: rgb(29, 78, 216);
}
</style>

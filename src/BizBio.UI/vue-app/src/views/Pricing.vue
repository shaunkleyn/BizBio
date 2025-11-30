<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '../api/client'

const router = useRouter()
const tiers = ref([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    const response = await apiClient.get('/subscriptions/tiers')
    tiers.value = response.data
  } catch (err) {
    error.value = 'Failed to load subscription tiers'
    console.error(err)
  } finally {
    loading.value = false
  }
})

const subscribe = (tier) => {
  router.push({ name: 'Register', query: { tier: tier.id } })
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(price)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="text-center mb-16">
        <h1 class="text-4xl sm:text-5xl font-heading font-bold text-brand-dark-text mb-4">
          Simple, Transparent Pricing
        </h1>
        <p class="text-xl text-brand-gray-text max-w-2xl mx-auto">
          Choose the perfect plan for your business
        </p>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="text-center py-20">
        <i class="fas fa-spinner fa-spin text-primary text-6xl mb-4"></i>
        <p class="text-brand-gray-text text-lg">Loading pricing plans...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="max-w-2xl mx-auto">
        <div class="bg-red-50 border-l-4 border-red-500 rounded-lg p-6">
          <div class="flex items-start">
            <i class="fas fa-exclamation-circle text-red-500 mt-0.5 mr-3 text-xl"></i>
            <p class="text-red-800 font-medium">{{ error }}</p>
          </div>
        </div>
      </div>

      <!-- Pricing Cards -->
      <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
        <div
          v-for="tier in tiers"
          :key="tier.id"
          class="relative bg-white rounded-xl shadow-lg hover:shadow-2xl transition-all duration-300 hover:-translate-y-2 flex flex-col"
          :class="{ 'border-4 border-primary scale-105': tier.isPopular }"
        >
          <!-- Popular Badge -->
          <div v-if="tier.isPopular" class="absolute -top-4 left-1/2 transform -translate-x-1/2">
            <span class="bg-gradient-to-r from-primary to-accent-purple text-white px-6 py-2 rounded-full text-sm font-bold shadow-lg">
              <i class="fas fa-star mr-1"></i>
              Most Popular
            </span>
          </div>

          <!-- Card Header -->
          <div class="p-8 text-center border-b border-gray-100">
            <h3 class="text-2xl font-heading font-bold text-brand-dark-text mb-2">
              {{ tier.displayName }}
            </h3>
            <p class="text-brand-gray-text text-sm">{{ tier.productLine }}</p>
          </div>

          <!-- Card Body -->
          <div class="p-8 flex-1 flex flex-direction-column">
            <!-- Price -->
            <div class="text-center mb-6">
              <div class="mb-2">
                <span class="text-5xl font-heading font-bold text-brand-dark-text">{{ formatPrice(tier.monthlyPrice) }}</span>
                <span class="text-brand-gray-text text-lg">/month</span>
              </div>
              <div class="text-sm text-brand-gray-text">
                or {{ formatPrice(tier.annualPrice) }}/year
                <span class="ml-2 inline-block bg-green-100 text-green-700 px-2 py-1 rounded text-xs font-semibold">
                  Save {{ tier.annualDiscountPercent }}%
                </span>
              </div>
            </div>

            <!-- Description -->
            <p class="text-center text-brand-gray-text mb-6">{{ tier.description }}</p>

            <!-- Features -->
            <ul class="space-y-3 mb-8 flex-1">
              <li v-if="tier.maxProfiles > 0" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">{{ tier.maxProfiles }} {{ tier.maxProfiles === 1 ? 'Profile' : 'Profiles' }}</span>
              </li>
              <li v-if="tier.maxCatalogItems > 0" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">{{ tier.maxCatalogItems }} Menu/Catalog Items</span>
              </li>
              <li v-if="tier.maxTeamMembers > 0" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">{{ tier.maxTeamMembers }} Team Members</span>
              </li>
              <li v-if="tier.customBranding" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">Custom Branding</span>
              </li>
              <li v-if="tier.analytics" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">Analytics Dashboard</span>
              </li>
              <li v-if="tier.nfcSupport" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">NFC Tag Support</span>
              </li>
              <li v-if="tier.prioritySupport" class="flex items-start">
                <i class="fas fa-check text-primary mt-1 mr-3"></i>
                <span class="text-brand-dark-text">Priority Support</span>
              </li>
            </ul>

            <!-- CTA Button -->
            <button
              @click="subscribe(tier)"
              class="w-full py-3 px-6 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>

      <!-- Footer Info -->
      <div class="text-center mt-16 space-y-4">
        <p class="text-brand-gray-text">
          All plans include a 14-day free trial. No credit card required.
        </p>
        <p class="text-brand-gray-text">
          Need a custom plan?
          <a href="mailto:sales@bizbio.co.za" class="text-primary hover:text-primary-600 font-semibold">
            Contact our sales team
          </a>
        </p>
      </div>
    </div>
  </div>
</template>

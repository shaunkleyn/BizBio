<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Welcome Header -->
      <div class="mb-12">
        <h1 class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
          Welcome back{{ authStore.currentUser ? ', ' + authStore.currentUser.firstName : '' }}!
        </h1>
        <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
          Choose a product to get started
        </p>
      </div>

      <!-- Product Cards -->
      <div class="grid md:grid-cols-3 gap-8 mb-12">
        <!-- Menu Product -->
        <NuxtLink
          to="/menu"
          class="group bg-white rounded-xl shadow-lg p-8 hover:shadow-2xl transition-all transform hover:-translate-y-1"
        >
          <div class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6 group-hover:scale-110 transition-transform">
            <i class="fas fa-utensils text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] text-center mb-3">Digital Menu</h2>
          <p class="text-[var(--gray-text-color)] text-center mb-6">
            Create and manage digital menus for your restaurant or cafe
          </p>
          <div class="flex items-center justify-center gap-2 text-[var(--primary-color)] font-semibold">
            <span>Get Started</span>
            <i class="fas fa-arrow-right group-hover:translate-x-1 transition-transform"></i>
          </div>
        </NuxtLink>

        <!-- Cards Product (Coming Soon) -->
        <div class="bg-white rounded-xl shadow-lg p-8 opacity-60 cursor-not-allowed">
          <div class="bg-gradient-to-br from-blue-500 to-indigo-600 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
            <i class="fas fa-id-card text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] text-center mb-3">Business Cards</h2>
          <p class="text-[var(--gray-text-color)] text-center mb-6">
            Create digital business cards and contact profiles
          </p>
          <div class="flex items-center justify-center">
            <span class="bg-gray-200 text-gray-600 px-4 py-2 rounded-full text-sm font-semibold">
              Coming Soon
            </span>
          </div>
        </div>

        <!-- Catalog Product (Coming Soon) -->
        <div class="bg-white rounded-xl shadow-lg p-8 opacity-60 cursor-not-allowed">
          <div class="bg-gradient-to-br from-orange-500 to-red-600 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
            <i class="fas fa-book text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] text-center mb-3">Product Catalog</h2>
          <p class="text-[var(--gray-text-color)] text-center mb-6">
            Showcase products and services with a digital catalog
          </p>
          <div class="flex items-center justify-center">
            <span class="bg-gray-200 text-gray-600 px-4 py-2 rounded-full text-sm font-semibold">
              Coming Soon
            </span>
          </div>
        </div>
      </div>

      <!-- Subscription Info Banner -->
      <div v-if="subscriptionInfo" class="mb-8">
        <div class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-xl shadow-lg p-6 text-white">
          <div class="flex items-center justify-between flex-wrap gap-4">
            <div class="flex items-center gap-4">
              <div class="bg-white/20 rounded-full p-3">
                <i :class="subscriptionInfo.IsInTrial ? 'fas fa-gift' : 'fas fa-crown'" class="text-2xl"></i>
              </div>
              <div>
                <h3 class="text-lg font-bold mb-1">{{ subscriptionInfo.TierName || 'Free Plan' }}</h3>
                <p class="text-white/80 text-sm">
                  <template v-if="subscriptionInfo.IsInTrial && subscriptionInfo.TrialDaysRemaining !== null">
                    Trial: {{ subscriptionInfo.TrialDaysRemaining }} day{{ subscriptionInfo.TrialDaysRemaining !== 1 ? 's' : '' }} remaining
                  </template>
                  <template v-else>
                    {{ subscriptionInfo.MaxProfiles }} Profile{{ subscriptionInfo.MaxProfiles !== 1 ? 's' : '' }} •
                    {{ subscriptionInfo.MaxCatalogItems }} Items
                  </template>
                </p>
              </div>
            </div>
            <NuxtLink
              to="/dashboard/subscription"
              :class="subscriptionInfo.NeedsPayment ? 'bg-red-500 text-white animate-pulse' : 'bg-white text-[var(--primary-color)]'"
              class="px-6 py-3 rounded-lg hover:bg-opacity-90 transition-all font-semibold"
            >
              <template v-if="subscriptionInfo.NeedsPayment">
                Activate Plan
              </template>
              <template v-else-if="subscriptionInfo.IsInTrial">
                Upgrade Now
              </template>
              <template v-else>
                Manage Plan
              </template>
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Quick Info Cards -->
      <div class="grid md:grid-cols-3 gap-6">
        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-[var(--primary-color)]">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-lg p-3">
              <i class="fas fa-utensils text-[var(--primary-color)] text-xl"></i>
            </div>
            <h3 class="font-bold text-[var(--dark-text-color)]">Digital Menu</h3>
          </div>
          <p class="text-sm text-[var(--gray-text-color)] mb-4">
            Perfect for restaurants, cafes, and food businesses. Create beautiful digital menus with QR codes.
          </p>
          <NuxtLink to="/menu" class="text-sm text-[var(--primary-color)] font-semibold hover:underline">
            Explore Menu <i class="fas fa-arrow-right ml-1"></i>
          </NuxtLink>
        </div>

        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-blue-500 opacity-60">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-blue-500 bg-opacity-10 rounded-lg p-3">
              <i class="fas fa-id-card text-blue-500 text-xl"></i>
            </div>
            <h3 class="font-bold text-[var(--dark-text-color)]">Business Cards</h3>
          </div>
          <p class="text-sm text-[var(--gray-text-color)] mb-4">
            Share your contact information instantly with digital business cards and NFC technology.
          </p>
          <span class="text-sm text-gray-500 font-semibold">Coming Soon</span>
        </div>

        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-orange-500 opacity-60">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-orange-500 bg-opacity-10 rounded-lg p-3">
              <i class="fas fa-book text-orange-500 text-xl"></i>
            </div>
            <h3 class="font-bold text-[var(--dark-text-color)]">Product Catalog</h3>
          </div>
          <p class="text-sm text-[var(--gray-text-color)] mb-4">
            Showcase your products and services with a professional digital catalog for retail businesses.
          </p>
          <span class="text-sm text-gray-500 font-semibold">Coming Soon</span>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({
  middleware: 'auth'
})

const authStore = useAuthStore()
const profilesApi = useProfilesApi()

const subscriptionInfo = ref(null)

onMounted(async () => {
  try {
    const response = await profilesApi.getMyProfiles()
    const profiles = response.data || []

    // Get subscription info from first profile (all profiles share same subscription)
    if (profiles.length > 0 && profiles[0].SubscriptionStatus) {
      subscriptionInfo.value = profiles[0].SubscriptionStatus
    }
  } catch (error) {
    console.error('Failed to load subscription info:', error)
  }
})

useHead({
  title: 'Dashboard',
})
</script>

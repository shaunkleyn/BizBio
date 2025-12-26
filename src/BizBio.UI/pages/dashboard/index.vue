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
          class="group mesh-card bg-md-surface rounded-2xl shadow-md-3 p-8 card-hover"
        >
          <div class="bg-gradient-primary rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6 group-hover:scale-110 transition-transform shadow-glow-purple">
            <i class="fas fa-utensils text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-md-on-surface text-center mb-3">Digital Menu</h2>
          <p class="text-md-on-surface-variant text-center mb-6">
            Create and manage digital menus for your restaurant or cafe
          </p>
          <div class="flex items-center justify-center gap-2 text-md-primary font-semibold">
            <span>Get Started</span>
            <i class="fas fa-arrow-right group-hover:translate-x-1 transition-transform"></i>
          </div>
        </NuxtLink>

        <!-- Cards Product (Coming Soon) -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-8 opacity-60 cursor-not-allowed">
          <div class="bg-gradient-secondary rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6 shadow-glow-pink">
            <i class="fas fa-id-card text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-md-on-surface text-center mb-3">Business Cards</h2>
          <p class="text-md-on-surface-variant text-center mb-6">
            Create digital business cards and contact profiles
          </p>
          <div class="flex items-center justify-center">
            <span class="bg-md-surface-container text-md-on-surface-variant px-4 py-2 rounded-full text-sm font-semibold">
              Coming Soon
            </span>
          </div>
        </div>

        <!-- Catalog Product (Coming Soon) -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-8 opacity-60 cursor-not-allowed">
          <div class="bg-gradient-tertiary rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6 shadow-glow-teal">
            <i class="fas fa-book text-white text-3xl"></i>
          </div>
          <h2 class="text-2xl font-bold text-md-on-surface text-center mb-3">Product Catalog</h2>
          <p class="text-md-on-surface-variant text-center mb-6">
            Showcase products and services with a digital catalog
          </p>
          <div class="flex items-center justify-center">
            <span class="bg-md-surface-container text-md-on-surface-variant px-4 py-2 rounded-full text-sm font-semibold">
              Coming Soon
            </span>
          </div>
        </div>
      </div>

      <!-- Subscription Info Banner -->
      <div v-if="subscriptionInfo" class="mb-8">
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-4 p-6 text-md-on-surface">
          <div class="flex items-center justify-between flex-wrap gap-4">
            <div class="flex items-center gap-4">
              <div class="bg-gradient-primary rounded-full p-3 shadow-glow-purple">
                <i :class="subscriptionInfo.IsInTrial ? 'fas fa-gift' : 'fas fa-crown'" class="text-2xl text-white"></i>
              </div>
              <div>
                <h3 class="text-lg font-bold mb-1">{{ subscriptionInfo.TierName || 'Free Plan' }}</h3>
                <p class="text-md-on-surface-variant text-sm">
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
              :class="subscriptionInfo.NeedsPayment ? 'bg-md-error text-md-on-error animate-pulse shadow-md-3' : 'btn-gradient shadow-md-2'"
              class="px-6 py-3 rounded-xl hover:shadow-md-4 transition-all font-semibold"
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
        <div class="gradient-border mesh-card bg-md-surface shadow-md-3 p-6 card-hover">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-gradient-primary rounded-lg p-3 shadow-glow-purple">
              <i class="fas fa-utensils text-white text-xl"></i>
            </div>
            <h3 class="font-bold text-md-on-surface">Digital Menu</h3>
          </div>
          <p class="text-sm text-md-on-surface-variant mb-4">
            Perfect for restaurants, cafes, and food businesses. Create beautiful digital menus with QR codes.
          </p>
          <NuxtLink to="/menu" class="text-sm gradient-text font-semibold hover:underline">
            Explore Menu <i class="fas fa-arrow-right ml-1"></i>
          </NuxtLink>
        </div>

        <div class="gradient-border mesh-card bg-md-surface shadow-md-3 p-6 opacity-60">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-gradient-secondary rounded-lg p-3 shadow-glow-pink">
              <i class="fas fa-id-card text-white text-xl"></i>
            </div>
            <h3 class="font-bold text-md-on-surface">Business Cards</h3>
          </div>
          <p class="text-sm text-md-on-surface-variant mb-4">
            Share your contact information instantly with digital business cards and NFC technology.
          </p>
          <span class="text-sm text-md-on-surface-variant font-semibold">Coming Soon</span>
        </div>

        <div class="gradient-border mesh-card bg-md-surface shadow-md-3 p-6 opacity-60">
          <div class="flex items-center gap-3 mb-3">
            <div class="bg-gradient-tertiary rounded-lg p-3 shadow-glow-teal">
              <i class="fas fa-book text-white text-xl"></i>
            </div>
            <h3 class="font-bold text-md-on-surface">Product Catalog</h3>
          </div>
          <p class="text-sm text-md-on-surface-variant mb-4">
            Showcase your products and services with a professional digital catalog for retail businesses.
          </p>
          <span class="text-sm text-md-on-surface-variant font-semibold">Coming Soon</span>
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

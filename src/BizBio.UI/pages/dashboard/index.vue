<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Welcome Header -->
      <div class="mb-8">
        <h1 class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
          Welcome back{{ authStore.currentUser ? ', ' + authStore.currentUser.firstName : '' }}!
        </h1>
        <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
          Here's what's happening with your profiles today
        </p>
      </div>

      <!-- Stats Cards -->
      <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <!-- Stat Card 1 - Profile Views -->
        <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full p-3 size-12 text-center">
              <i class="fad fa-eye text-white text-2xl"></i>
            </div>
            <span class="text-[var(--accent3-color)] text-sm font-semibold">
              <i class="fad fa-arrow-up mr-1"></i>12%
            </span>
          </div>
          <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
            {{ stats.totalViews.toLocaleString() }}
          </h3>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
            Total Profile Views
          </p>
        </div>

        <!-- Stat Card 2 - Profile Shares -->
        <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-3 size-12 text-center">
              <i class="fad fa-share-alt text-white text-2xl"></i>
            </div>
            <span class="text-[var(--accent3-color)] text-sm font-semibold">
              <i class="fad fa-arrow-up mr-1"></i>8%
            </span>
          </div>
          <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
            {{ stats.totalShares.toLocaleString() }}
          </h3>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
            Profile Shares
          </p>
        </div>

        <!-- Stat Card 3 - Contact Clicks -->
        <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-full p-3 size-12 text-center">
              <i class="fad fa-mouse-pointer  text-white text-2xl"></i>
            </div>
            <span class="text-[var(--accent3-color)] text-sm font-semibold">
              <i class="fad fa-arrow-up mr-1"></i>24%
            </span>
          </div>
          <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
            {{ stats.totalClicks.toLocaleString() }}
          </h3>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
            Contact Clicks
          </p>
        </div>

        <!-- Stat Card 4 - VCF Downloads -->
        <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-4">
            <div class="bg-[var(--accent4-color)] bg-opacity-10 rounded-full p-3 size-12 text-center">
              <i class="fad fa-download text-white text-2xl"></i>
            </div>
            <span class="text-[var(--accent3-color)] text-sm font-semibold">
              <i class="fad fa-arrow-up mr-1"></i>15%
            </span>
          </div>
          <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
            {{ stats.totalDownloads.toLocaleString() }}
          </h3>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
            VCF Downloads
          </p>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid lg:grid-cols-3 gap-8">
        <!-- Left Column - My Profiles -->
        <div class="lg:col-span-2 space-y-8">
          <!-- My Profiles Section -->
          <div class="bg-white rounded-xl shadow-lg p-6">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                My Profiles
              </h2>
              <NuxtLink
                to="/dashboard/profile"
                class="inline-flex items-center text-[var(--primary-color)] hover:text-[var(--primary-button-hover-bg-color)] font-semibold transition-colors"
              >
                <i class="fas fa-plus-circle mr-2"></i>
                Create New
              </NuxtLink>
            </div>

            <!-- Profile Cards -->
            <div v-if="loading" class="text-center py-8">
              <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)]"></i>
              <p class="text-[var(--gray-text-color)] mt-4">Loading profiles...</p>
            </div>

            <div v-else-if="profiles.length === 0" class="text-center py-12">
              <i class="fas fa-folder-open text-6xl text-[var(--gray-text-color)] opacity-50 mb-6"></i>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">No Profiles Yet</h3>
              <p class="text-[var(--gray-text-color)] mb-8">Create your first profile to get started</p>

              <!-- Product Type Cards -->
              <div class="grid sm:grid-cols-3 gap-4 max-w-3xl mx-auto">
                <!-- Menu Profile Card -->
                <NuxtLink
                  to="/dashboard/menu/create"
                  class="group border-2 border-[var(--light-border-color)] rounded-xl p-6 hover:border-[var(--primary-color)] hover:shadow-lg transition-all"
                >
                  <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4 group-hover:bg-[var(--accent3-color)] transition-colors">
                    <i class="fas fa-utensils text-[var(--accent3-color)] text-2xl group-hover:text-white transition-colors"></i>
                  </div>
                  <h4 class="font-bold text-[var(--dark-text-color)] mb-2">Digital Menu</h4>
                  <p class="text-sm text-[var(--gray-text-color)] mb-3">Create a digital menu for your restaurant</p>
                  <div class="inline-flex items-center text-xs bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)] px-3 py-1 rounded-full font-semibold">
                    <i class="fas fa-gift mr-1"></i>
                    14-day free trial
                  </div>
                </NuxtLink>

                <!-- Connect Profile Card -->
                <div class="border-2 border-[var(--light-border-color)] rounded-xl p-6 opacity-50 cursor-not-allowed">
                  <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-id-card text-[var(--primary-color)] text-2xl"></i>
                  </div>
                  <h4 class="font-bold text-[var(--dark-text-color)] mb-2">Connect Card</h4>
                  <p class="text-sm text-[var(--gray-text-color)] mb-3">Digital business card profile</p>
                  <div class="inline-flex items-center text-xs bg-[var(--gray-text-color)] bg-opacity-10 text-[var(--gray-text-color)] px-3 py-1 rounded-full font-semibold">
                    Coming Soon
                  </div>
                </div>

                <!-- Retail Profile Card -->
                <div class="border-2 border-[var(--light-border-color)] rounded-xl p-6 opacity-50 cursor-not-allowed">
                  <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-store text-[var(--accent2-color)] text-2xl"></i>
                  </div>
                  <h4 class="font-bold text-[var(--dark-text-color)] mb-2">Retail Store</h4>
                  <p class="text-sm text-[var(--gray-text-color)] mb-3">Product catalog for retail</p>
                  <div class="inline-flex items-center text-xs bg-[var(--gray-text-color)] bg-opacity-10 text-[var(--gray-text-color)] px-3 py-1 rounded-full font-semibold">
                    Coming Soon
                  </div>
                </div>
              </div>
            </div>

            <div v-else class="space-y-4">
              <!-- Profile Card -->
              <div
                v-for="profile in profiles"
                :key="profile.id"
                class="border-2 border-[var(--light-border-color)] rounded-lg p-4 hover:border-[var(--primary-color)] transition-colors"
              >
                <div class="flex items-center justify-between mb-3">
                  <div class="flex items-center gap-4">
                    <div class="w-12 h-12 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-full flex items-center justify-center text-white font-bold">
                      <i class="fas fa-briefcase"></i>
                    </div>
                    <div>
                      <h3 class="font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                        {{ profile.name || 'My Profile' }}
                      </h3>
                      <p class="text-sm text-[var(--gray-text-color)]">
                        {{ profile.type || 'Business Profile' }}
                      </p>
                    </div>
                  </div>
                  <div class="flex items-center gap-2">
                    <span class="bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)] px-3 py-1 rounded-full text-xs font-semibold">
                      ACTIVE
                    </span>
                    <button class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] p-2">
                      <i class="fas fa-ellipsis-v"></i>
                    </button>
                  </div>
                </div>

                <!-- Profile Stats -->
                <div class="grid grid-cols-3 gap-4 pt-3 border-t border-[var(--light-border-color)]">
                  <div class="text-center">
                    <div class="text-lg font-bold text-[var(--dark-text-color)]">{{ profile.views || 0 }}</div>
                    <div class="text-xs text-[var(--gray-text-color)]">Views</div>
                  </div>
                  <div class="text-center">
                    <div class="text-lg font-bold text-[var(--dark-text-color)]">{{ profile.shares || 0 }}</div>
                    <div class="text-xs text-[var(--gray-text-color)]">Shares</div>
                  </div>
                  <div class="text-center">
                    <div class="text-lg font-bold text-[var(--dark-text-color)]">{{ profile.clicks || 0 }}</div>
                    <div class="text-xs text-[var(--gray-text-color)]">Clicks</div>
                  </div>
                </div>

                <!-- Action Buttons -->
                <div class="flex gap-2 mt-4">
                  <NuxtLink
                    :to="`/${profile.slug}`"
                    class="flex-1 text-center bg-[var(--primary-color)] text-white px-4 py-2 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors text-sm font-semibold"
                  >
                    <i class="fas fa-eye mr-1"></i>
                    View
                  </NuxtLink>
                  <NuxtLink
                    :to="`/dashboard/profile?id=${profile.id}`"
                    class="flex-1 text-center border-2 border-[var(--primary-color)] text-[var(--primary-color)] px-4 py-2 rounded-lg hover:bg-[var(--primary-color)] hover:text-white transition-colors text-sm font-semibold"
                  >
                    <i class="fas fa-edit mr-1"></i>
                    Edit
                  </NuxtLink>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Activity Section -->
          <div class="bg-white rounded-xl shadow-lg p-6">
            <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-6">
              Recent Activity
            </h2>
            <div class="space-y-4">
              <div v-for="i in 5" :key="i" class="flex items-center gap-4 pb-4 border-b border-[var(--light-border-color)] last:border-0">
                <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-3">
                  <i class="fas fa-eye text-[var(--accent3-color)]"></i>
                </div>
                <div class="flex-1">
                  <p class="text-[var(--dark-text-color)] font-semibold">Profile viewed</p>
                  <p class="text-sm text-[var(--gray-text-color)]">Someone viewed your profile</p>
                </div>
                <div class="text-sm text-[var(--gray-text-color)]">
                  2h ago
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Column - Quick Actions & Plan Info -->
        <div class="space-y-8">
          <!-- Current Plan Card -->
          <div class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-xl shadow-lg p-6 text-white">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-bold">Current Plan</h3>
              <i class="fas fa-crown text-2xl"></i>
            </div>
            <div class="mb-4">
              <div class="text-3xl font-bold mb-1">Free Plan</div>
              <p class="text-white/80 text-sm">1 Profile • Basic Features</p>
            </div>
            <NuxtLink
              to="/dashboard/subscription"
              class="block w-full bg-white text-[var(--primary-color)] text-center px-4 py-3 rounded-lg hover:bg-opacity-90 transition-all font-semibold"
            >
              Upgrade Plan
            </NuxtLink>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl shadow-lg p-6">
            <h3 class="text-lg font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
              Quick Actions
            </h3>
            <div class="space-y-3">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center gap-3 p-3 rounded-lg hover:bg-[var(--light-background-color)] transition-colors"
              >
                <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-lg p-2">
                  <i class="fas fa-plus text-[var(--primary-color)]"></i>
                </div>
                <span class="text-[var(--dark-text-color)] font-semibold">Create Profile</span>
              </NuxtLink>
              <NuxtLink
                to="/dashboard/menu"
                class="flex items-center gap-3 p-3 rounded-lg hover:bg-[var(--light-background-color)] transition-colors"
              >
                <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-lg p-2">
                  <i class="fas fa-utensils text-[var(--accent3-color)]"></i>
                </div>
                <span class="text-[var(--dark-text-color)] font-semibold">Manage Menu</span>
              </NuxtLink>
              <NuxtLink
                to="/dashboard/tables"
                class="flex items-center gap-3 p-3 rounded-lg hover:bg-[var(--light-background-color)] transition-colors"
              >
                <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-lg p-2">
                  <i class="fas fa-qrcode text-[var(--accent2-color)]"></i>
                </div>
                <span class="text-[var(--dark-text-color)] font-semibold">NFC Tables</span>
              </NuxtLink>
            </div>
          </div>

          <!-- NFC Product Promo -->
          <div class="bg-gradient-to-br from-[var(--accent2-color)] to-[var(--accent-color)] rounded-xl shadow-lg p-6 text-white">
            <i class="fas fa-wifi text-4xl mb-4"></i>
            <h3 class="text-xl font-bold mb-2">Get NFC Cards</h3>
            <p class="text-white/90 text-sm mb-4">
              Tap to share your profile instantly with NFC-enabled cards and keychains.
            </p>
            <button class="w-full bg-white text-[var(--accent2-color)] px-4 py-3 rounded-lg hover:bg-opacity-90 transition-all font-semibold">
              Shop Now
            </button>
          </div>
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

const loading = ref(true)
const profiles = ref([])
const stats = ref({
  totalViews: 1284,
  totalShares: 342,
  totalClicks: 678,
  totalDownloads: 156
})

onMounted(async () => {
  try {
    const response = await profilesApi.getMyProfiles()
    profiles.value = response.data || []
  } catch (error) {
    console.error('Failed to load profiles:', error)
  } finally {
    loading.value = false
  }
})

useHead({
  title: 'Dashboard',
})
</script>

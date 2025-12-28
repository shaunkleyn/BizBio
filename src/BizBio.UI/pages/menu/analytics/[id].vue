<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="flex items-center justify-between mb-8">
        <div>
          <NuxtLink
            to="/dashboard"
            class="text-[var(--primary-color)] hover:underline mb-2 inline-block"
          >
            <i class="fas fa-arrow-left mr-2"></i>
            Back to Dashboard
          </NuxtLink>
          <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
            Menu Analytics
          </h1>
          <p v-if="menuName" class="text-xl text-[var(--gray-text-color)] mt-2">
            {{ menuName }}
          </p>
        </div>

        <!-- Date Range Selector -->
        <div class="flex items-center gap-3">
          <select
            v-model="selectedDateRange"
            @change="loadAnalytics"
            class="px-4 py-2 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none"
          >
            <option value="7">Last 7 days</option>
            <option value="30">Last 30 days</option>
            <option value="90">Last 90 days</option>
            <option value="365">Last year</option>
          </select>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex items-center justify-center py-12">
        <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)]"></i>
      </div>

      <!-- Analytics Content -->
      <div v-else-if="analytics" class="space-y-6">
        <!-- Key Metrics Cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Total Views -->
          <div class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-xl p-6 text-white">
            <div class="flex items-center justify-between mb-2">
              <i class="fas fa-eye text-3xl opacity-80"></i>
              <span class="text-sm font-semibold bg-white bg-opacity-20 px-3 py-1 rounded-full">
                {{ analytics.viewsTrend >= 0 ? '+' : '' }}{{ analytics.viewsTrend }}%
              </span>
            </div>
            <h3 class="text-4xl font-bold mb-1">{{ analytics.totalViews.toLocaleString() }}</h3>
            <p class="text-md-on-primary">Total Views</p>
          </div>

          <!-- Unique Visitors -->
          <div class="bg-gradient-to-br bg-gradient-primary rounded-2xl shadow-xl p-6 text-white">
            <div class="flex items-center justify-between mb-2">
              <i class="fas fa-users text-3xl opacity-80"></i>
              <span class="text-sm font-semibold bg-white bg-opacity-20 px-3 py-1 rounded-full">
                {{ analytics.visitorsTrend >= 0 ? '+' : '' }}{{ analytics.visitorsTrend }}%
              </span>
            </div>
            <h3 class="text-4xl font-bold mb-1">{{ analytics.uniqueVisitors.toLocaleString() }}</h3>
            <p class="text-md-on-primary">Unique Visitors</p>
          </div>

          <!-- Avg Time Spent -->
          <div class="bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-xl p-6 text-white">
            <div class="flex items-center justify-between mb-2">
              <i class="fas fa-clock text-3xl opacity-80"></i>
            </div>
            <h3 class="text-4xl font-bold mb-1">{{ formatTime(analytics.avgTimeSpent) }}</h3>
            <p class="text-green-100">Avg Time on Menu</p>
          </div>

          <!-- Total Contact Actions -->
          <div class="bg-gradient-to-br from-orange-500 to-orange-600 rounded-2xl shadow-xl p-6 text-white">
            <div class="flex items-center justify-between mb-2">
              <i class="fas fa-phone text-3xl opacity-80"></i>
            </div>
            <h3 class="text-4xl font-bold mb-1">{{ analytics.totalContactActions.toLocaleString() }}</h3>
            <p class="text-orange-100">Contact Actions</p>
          </div>
        </div>

        <!-- Charts Row -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Views Over Time Chart -->
          <div class="bg-white rounded-2xl shadow-xl p-6">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-4">
              <i class="fas fa-chart-line mr-2 text-[var(--primary-color)]"></i>
              Views Over Time
            </h3>
            <div class="h-64 flex items-center justify-center text-[var(--gray-text-color)]">
              <div class="text-center">
                <i class="fas fa-chart-area text-4xl mb-2 opacity-50"></i>
                <p>Chart visualization (integrate with Chart.js or similar)</p>
              </div>
            </div>
          </div>

          <!-- Device Breakdown -->
          <div class="bg-white rounded-2xl shadow-xl p-6">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-4">
              <i class="fas fa-mobile-alt mr-2 text-[var(--primary-color)]"></i>
              Device Breakdown
            </h3>
            <div class="space-y-4">
              <div v-for="device in analytics.deviceBreakdown" :key="device.type" class="flex items-center justify-between">
                <div class="flex items-center gap-3 flex-1">
                  <i :class="getDeviceIcon(device.type)" class="text-[var(--gray-text-color)] w-6"></i>
                  <span class="capitalize font-medium text-[var(--dark-text-color)]">{{ device.type }}</span>
                </div>
                <div class="flex items-center gap-3">
                  <div class="w-48 bg-[var(--light-background-color)] rounded-full h-3 overflow-hidden">
                    <div
                      :style="{ width: `${device.percentage}%` }"
                      class="h-full bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] transition-all"
                    ></div>
                  </div>
                  <span class="font-bold text-[var(--dark-text-color)] w-16 text-right">{{ device.percentage }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Popular Categories -->
        <div class="bg-white rounded-2xl shadow-xl p-6">
          <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
            <i class="fas fa-fire mr-2 text-orange-500"></i>
            Most Popular Categories
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="(category, index) in analytics.topCategories"
              :key="category.id"
              class="border-2 border-[var(--light-border-color)] rounded-xl p-4 hover:border-[var(--primary-color)] transition-all"
            >
              <div class="flex items-center justify-between mb-3">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-lg flex items-center justify-center text-white font-bold">
                    #{{ index + 1 }}
                  </div>
                  <div>
                    <h4 class="font-bold text-[var(--dark-text-color)]">{{ category.name }}</h4>
                    <p class="text-sm text-[var(--gray-text-color)]">{{ category.views }} views</p>
                  </div>
                </div>
              </div>
              <div class="w-full bg-[var(--light-background-color)] rounded-full h-2 overflow-hidden">
                <div
                  :style="{ width: `${category.percentage}%` }"
                  class="h-full bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)]"
                ></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Popular Menu Items -->
        <div class="bg-white rounded-2xl shadow-xl p-6">
          <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
            <i class="fas fa-star mr-2 text-yellow-500"></i>
            Most Popular Menu Items
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div
              v-for="(item, index) in analytics.topItems"
              :key="item.id"
              class="flex items-center gap-4 border-2 border-[var(--light-border-color)] rounded-xl p-4 hover:border-[var(--primary-color)] transition-all"
            >
              <!-- Rank Badge -->
              <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-lg flex items-center justify-center text-white font-bold text-lg">
                #{{ index + 1 }}
              </div>

              <!-- Item Image -->
              <div v-if="item.imageUrl" class="w-16 h-16 rounded-lg overflow-hidden flex-shrink-0">
                <img :src="item.imageUrl" :alt="item.name" class="w-full h-full object-cover" />
              </div>

              <!-- Item Info -->
              <div class="flex-1 min-w-0">
                <h4 class="font-bold text-[var(--dark-text-color)] truncate">{{ item.name }}</h4>
                <p class="text-sm text-[var(--gray-text-color)]">{{ item.views }} views</p>
                <p class="text-sm font-semibold text-[var(--primary-color)]">R{{ item.price }}</p>
              </div>

              <!-- Click Rate -->
              <div class="text-right flex-shrink-0">
                <div class="text-2xl font-bold text-[var(--primary-color)]">{{ item.clickRate }}%</div>
                <div class="text-xs text-[var(--gray-text-color)]">click rate</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Engagement Metrics -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Scroll Depth -->
          <div class="bg-white rounded-2xl shadow-xl p-6">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
              <i class="fas fa-arrows-alt-v mr-2 text-[var(--primary-color)]"></i>
              Scroll Depth Distribution
            </h3>
            <div class="space-y-4">
              <div v-for="depth in analytics.scrollDepth" :key="depth.percentage" class="flex items-center gap-4">
                <span class="font-semibold text-[var(--dark-text-color)] w-16">{{ depth.percentage }}%</span>
                <div class="flex-1 bg-[var(--light-background-color)] rounded-full h-4 overflow-hidden">
                  <div
                    :style="{ width: `${depth.userPercentage}%` }"
                    class="h-full bg-gradient-to-r bg-gradient-tertiary"
                  ></div>
                </div>
                <span class="text-sm text-[var(--gray-text-color)] w-24 text-right">{{ depth.userPercentage }}% of users</span>
              </div>
            </div>
          </div>

          <!-- Contact Actions Breakdown -->
          <div class="bg-white rounded-2xl shadow-xl p-6">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
              <i class="fas fa-phone-alt mr-2 text-[var(--primary-color)]"></i>
              Contact Actions
            </h3>
            <div class="space-y-4">
              <div v-for="action in analytics.contactActions" :key="action.type" class="flex items-center gap-4">
                <i :class="getContactIcon(action.type)" class="text-[var(--primary-color)] w-6 text-lg"></i>
                <span class="capitalize font-medium text-[var(--dark-text-color)] flex-1">{{ action.type }}</span>
                <span class="text-2xl font-bold text-[var(--primary-color)]">{{ action.count }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Traffic Sources -->
        <div class="bg-white rounded-2xl shadow-xl p-6">
          <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
            <i class="fas fa-globe mr-2 text-[var(--primary-color)]"></i>
            Traffic Sources
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div v-for="source in analytics.trafficSources" :key="source.source" class="border-2 border-[var(--light-border-color)] rounded-xl p-4">
              <div class="flex items-center justify-between mb-2">
                <span class="font-semibold text-[var(--dark-text-color)] capitalize">{{ source.source }}</span>
                <span class="text-sm text-[var(--gray-text-color)]">{{ source.percentage }}%</span>
              </div>
              <div class="text-3xl font-bold text-[var(--primary-color)] mb-1">{{ source.visits }}</div>
              <div class="text-sm text-[var(--gray-text-color)]">visits</div>
            </div>
          </div>
        </div>

        <!-- Peak Hours Heatmap -->
        <div class="bg-white rounded-2xl shadow-xl p-6">
          <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-6">
            <i class="fas fa-calendar-alt mr-2 text-[var(--primary-color)]"></i>
            Peak Viewing Times
          </h3>
          <div class="text-center text-[var(--gray-text-color)] py-8">
            <i class="fas fa-chart-bar text-4xl mb-2 opacity-50"></i>
            <p>Heatmap showing peak viewing hours by day of week</p>
            <p class="text-sm mt-1">(integrate with a heatmap library)</p>
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

const route = useRoute()
const api = useApi()

const menuName = ref('')
const selectedDateRange = ref('30')
const loading = ref(true)
const analytics = ref(null)

// Load analytics data
const loadAnalytics = async () => {
  loading.value = true
  try {
    const response = await api.get(`/analytics/menu/${route.params.id}?days=${selectedDateRange.value}`)
    analytics.value = response.data
    menuName.value = response.data.menuName
  } catch (error) {
    console.error('Failed to load analytics:', error)
  } finally {
    loading.value = false
  }
}

// Helper functions
const formatTime = (seconds) => {
  if (seconds < 60) return `${seconds}s`
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}m ${remainingSeconds}s`
}

const getDeviceIcon = (type) => {
  const icons = {
    mobile: 'fas fa-mobile-alt',
    tablet: 'fas fa-tablet-alt',
    desktop: 'fas fa-desktop'
  }
  return icons[type] || 'fas fa-question'
}

const getContactIcon = (type) => {
  const icons = {
    phone: 'fas fa-phone',
    email: 'fas fa-envelope',
    address: 'fas fa-map-marker-alt'
  }
  return icons[type] || 'fas fa-question'
}

// Load analytics on mount
onMounted(() => {
  loadAnalytics()
})

useHead({
  title: 'Menu Analytics - BizBio',
})
</script>



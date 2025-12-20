<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Active Menus</p>
              <p class="text-3xl font-bold text-gray-900">{{ stats.menus }}</p>
            </div>
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-book-open text-blue-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Total Items</p>
              <p class="text-3xl font-bold text-gray-900">{{ stats.items }}</p>
            </div>
            <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-green-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Categories</p>
              <p class="text-3xl font-bold text-gray-900">{{ stats.categories }}</p>
            </div>
            <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-purple-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">QR Scans</p>
              <p class="text-3xl font-bold text-gray-900">{{ stats.scans }}</p>
            </div>
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Menus -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 mb-8">
        <div class="p-6 border-b border-gray-200">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-bold text-gray-900">Recent Menus</h2>
            <NuxtLink to="/menu/menus" class="text-sm text-[var(--primary-color)] hover:underline">
              View All
            </NuxtLink>
          </div>
        </div>
        <div class="p-6">
          <div v-if="loading" class="text-center py-8">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-[var(--primary-color)] mx-auto"></div>
          </div>
          <div v-else-if="recentMenus.length === 0" class="text-center py-12">
            <i class="fas fa-book-open text-4xl text-gray-300 mb-3"></i>
            <p class="text-gray-600">No menus yet. Create your first menu to get started!</p>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="menu in recentMenus"
              :key="menu.id"
              class="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
              @click="navigateTo(`/menu/${menu.slug}`)"
            >
              <h3 class="font-semibold text-gray-900 mb-1">{{ menu.name }}</h3>
              <p class="text-sm text-gray-600 mb-2">{{ menu.description || 'No description' }}</p>
              <div class="flex items-center justify-between text-xs text-gray-500">
                <span>{{ menu.itemCount || 0 }} items</span>
                <span>Updated {{ formatDate(menu.updatedAt) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <NuxtLink
          to="/menu/library/items"
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-blue-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">Library Items</h3>
              <p class="text-sm text-gray-600">Manage your item library</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/categories"
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-purple-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">Categories</h3>
              <p class="text-sm text-gray-600">Organize your items</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/tables"
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">QR Codes</h3>
              <p class="text-sm text-gray-600">Generate table QR codes</p>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'

definePageMeta({
  layout: 'menu'
})

const loading = ref(false)
const recentMenus = ref<any[]>([])
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0,
  scans: 0
})

// Provide stats to the layout
provide('menuStats', stats)

// Provide page metadata to the layout
provide('pageHeader', {
  title: 'Menu Dashboard',
  description: 'Manage your digital menus and menu items'
})

provide('pageActions', () => h('NuxtLink', {
  to: '/dashboard/menu/create',
  class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold'
}, [
  h('i', { class: 'fas fa-plus mr-2' }),
  'Create Menu'
]))

onMounted(async () => {
  await loadDashboardData()
})

async function loadDashboardData() {
  loading.value = true
  try {
    const menusApi = useMenusApi()
    const libraryItemsApi = useLibraryItemsApi()
    const categoriesApi = useLibraryCategoriesApi()

    // Load all data in parallel
    const [menusResponse, itemsResponse, categoriesResponse] = await Promise.all([
      menusApi.getMyMenus(),
      libraryItemsApi.getItems(),
      categoriesApi.getCategories()
    ])

    // Set recent menus
    const menus = Array.isArray(menusResponse) ? menusResponse : (menusResponse?.data || [])
    recentMenus.value = menus.slice(0, 5) // Get last 5 menus
    stats.value.menus = menus.length

    // Set items count
    const items = Array.isArray(itemsResponse) ? itemsResponse : (itemsResponse?.data || [])
    stats.value.items = items.length

    // Set categories count
    const categories = Array.isArray(categoriesResponse) ? categoriesResponse : (categoriesResponse?.data || [])
    stats.value.categories = categories.length

    // TODO: Add API call for scans when available
    stats.value.scans = 1243
  } catch (error) {
    console.error('Error loading dashboard data:', error)
    // Fallback to empty data on error
    recentMenus.value = []
    stats.value = {
      menus: 0,
      items: 0,
      categories: 0,
      scans: 0
    }
  } finally {
    loading.value = false
  }
}

function formatDate(date: string) {
  if (!date) return 'Recently'
  const d = new Date(date)
  const now = new Date()
  const diffTime = Math.abs(now.getTime() - d.getTime())
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  return d.toLocaleDateString()
}
</script>

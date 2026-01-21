<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Active Menus</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.menus }}</p>
            </div>
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-book-open text-blue-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Total Items</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.items }}</p>
            </div>
            <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-green-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Categories</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.categories }}</p>
            </div>
            <div class="w-12 h-12 bg-md-primary-container rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-md-primary text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">QR Scans</p>
              <p class="text-lg font-bold text-md-on-surface">Coming Soon</p>
            </div>
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Menus -->
      <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant mb-8">
        <div class="p-6 border-b border-md-outline-variant">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-bold text-md-on-surface">Recent Menus</h2>
            <NuxtLink to="/menu" class="text-sm text-[var(--primary-color)] hover:underline">
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
            <p class="text-md-on-surface-variant mb-4">No menus yet. Create your first menu to get started!</p>
            <button
              @click="handleCreateMenu"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold"
            >
              <i class="fas fa-plus mr-2"></i>
              Create Menu
            </button>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="menu in recentMenus"
              :key="menu.id"
              class="border border-md-outline-variant rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
              @click="navigateTo(`/${menu.entitySlug}/${menu.slug}`)"
            >
              <h3 class="font-semibold text-md-on-surface mb-1">{{ menu.name }}</h3>
              <p class="text-sm text-md-on-surface-variant mb-2">{{ menu.description || 'No description' }}</p>
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
          to="/menu/items"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-blue-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">Library Items</h3>
              <p class="text-sm text-md-on-surface-variant">Manage your item library</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/categories"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-md-primary-container rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-md-primary text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">Categories</h3>
              <p class="text-sm text-md-on-surface-variant">Organize your items</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/tables"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">QR Codes</h3>
              <p class="text-sm text-md-on-surface-variant">Generate table QR codes</p>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

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

// Use page metadata composable
const { setPageHeader, setPageActionButton } = usePageMeta()

onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Menu Dashboard',
    description: 'Manage your digital menus and menu items'
  })

  setPageActionButton({
    onClick: () => handleCreateMenu(),
    label: 'Create Menu',
    icon: 'fas fa-plus',
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'
  })

  await loadDashboardData()
})

const handleCreateMenu = () => {
  // Navigate to menus page and open wizard
  navigateTo('/menu?create=true')
}

async function loadDashboardData() {
  loading.value = true
  try {
    const entityApi = useEntityApi()
    const libraryItemsApi = useLibraryItemsApi()
    const categoriesApi = useLibraryCategoriesApi()

    // Get user's entities (which may have catalogs)
    const entitiesResponse = await entityApi.getMyEntities()
    const entities = entitiesResponse?.data?.entities || []

    // For each entity, get its catalogs and build menu list
    const menusList: any[] = []
    for (const entity of entities) {
      const catalogsResponse = await entityApi.getEntityCatalogs(entity.id)
      const catalogs = catalogsResponse?.data?.catalogs || []

      // Add each catalog as a "menu" with entity slug for routing
      catalogs.forEach((catalog: any) => {
        menusList.push({
          id: catalog.id,
          name: catalog.name,
          slug: catalog.slug,
          entitySlug: entity.slug,
          description: catalog.description,
          itemCount: catalog.itemCount || 0,
          updatedAt: catalog.updatedAt
        })
      })
    }

    // Set recent menus
    recentMenus.value = menusList.slice(0, 5) // Get last 5 menus
    stats.value.menus = menusList.length

    // Load items and categories in parallel
    const [itemsResponse, categoriesResponse] = await Promise.all([
      libraryItemsApi.getItems(),
      categoriesApi.getCategories()
    ])

    // Set items count
    const items = (itemsResponse?.data || itemsResponse?.data?.items || [])
    console.log('Library items response:', itemsResponse)
    stats.value.items = items.length

    // Set categories count
    const categories = Array.isArray(categoriesResponse) ? categoriesResponse : (categoriesResponse?.data || [])
    stats.value.categories = categories.length

    // TODO: Add API call for scans when available
    stats.value.scans = '1243'
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






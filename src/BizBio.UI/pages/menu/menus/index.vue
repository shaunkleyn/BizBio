<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Loading State -->
      <div v-if="loading" class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-12 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)] mx-auto"></div>
        <p class="text-md-on-surface-variant mt-4">Loading menus...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="menus.length === 0" class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-12 text-center">
        <i class="fas fa-book-open text-6xl text-gray-300 mb-4"></i>
        <h3 class="text-xl font-bold text-md-on-surface mb-2">No Menus Yet</h3>
        <p class="text-md-on-surface-variant mb-6">Create your first menu to get started organizing your items.</p>
        <NuxtLink
          to="/dashboard/menu/create"
          class="inline-block px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold"
        >
          <i class="fas fa-plus mr-2"></i>
          Create Your First Menu
        </NuxtLink>
      </div>

      <!-- Menus Grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="menu in menus"
          :key="menu.id"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow cursor-pointer"
          @click="navigateTo(`/menu/${menu.slug}`)"
        >
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-bold text-md-on-surface">{{ menu.name }}</h3>
            <div class="flex items-center gap-2" @click.stop>
              <NuxtLink
                :to="`/dashboard/menu/${menu.id}/edit`"
                class="text-md-on-surface-variant hover:text-[var(--primary-color)] transition-colors"
              >
                <i class="fas fa-edit"></i>
              </NuxtLink>
            </div>
          </div>
          <p class="text-sm text-md-on-surface-variant mb-4">{{ menu.description || 'No description' }}</p>
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">{{ menu.itemCount || 0 }} items</span>
            <span class="text-xs text-gray-400">Updated {{ formatDate(menu.updatedAt) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'

definePageMeta({
  layout: 'menu'
})

const loading = ref(true)
const menus = ref<any[]>([])
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0,
  scans: 0
})

// Provide stats to the layout
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActions } = usePageMeta()

onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Menus',
    description: 'Create and manage your digital menus'
  })

  setPageActions(() => h('NuxtLink', {
    to: '/dashboard/menu/create',
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'
  }, [
    h('i', { class: 'fas fa-plus mr-2' }),
    'Create Menu'
  ]))

  await Promise.all([loadMenus(), loadStats()])
})

async function loadMenus() {
  loading.value = true
  try {
    const menusApi = useMenusApi()
    const response = await menusApi.getMyMenus()

    // Handle different response structures
    menus.value = Array.isArray(response) ? response : (response?.data || [])
    stats.value.menus = menus.value.length
  } catch (error) {
    console.error('Failed to load menus:', error)
    menus.value = []
  } finally {
    loading.value = false
  }
}

async function loadStats() {
  try {
    const libraryItemsApi = useLibraryItemsApi()
    const categoriesApi = useLibraryCategoriesApi()

    const [itemsResponse, categoriesResponse] = await Promise.all([
      libraryItemsApi.getItems(),
      categoriesApi.getCategories()
    ])

    const items = Array.isArray(itemsResponse) ? itemsResponse : (itemsResponse?.data || [])
    const categories = Array.isArray(categoriesResponse) ? categoriesResponse : (categoriesResponse?.data || [])

    stats.value.items = items.length
    stats.value.categories = categories.length
  } catch (err) {
    console.error('Error loading stats:', err)
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

useHead({
  title: 'Menus - Menu Dashboard',
})
</script>





<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Loading State -->
      <div v-if="loading" class="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)] mx-auto"></div>
        <p class="text-gray-600 mt-4">Loading categories...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="categories.length === 0" class="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
        <i class="fas fa-layer-group text-6xl text-gray-300 mb-4"></i>
        <h3 class="text-xl font-bold text-gray-900 mb-2">No Categories Yet</h3>
        <p class="text-gray-600 mb-6">Create your first category to start organizing your menu items.</p>
        <button
          @click="showCreateModal = true"
          class="px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold"
        >
          <i class="fas fa-plus mr-2"></i>
          Create Your First Category
        </button>
      </div>

      <!-- Categories Grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="category in categories"
          :key="category.id"
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-bold text-gray-900">{{ category.name }}</h3>
            <div class="flex items-center gap-2">
              <button class="text-gray-600 hover:text-[var(--primary-color)] transition-colors">
                <i class="fas fa-edit"></i>
              </button>
              <button class="text-gray-600 hover:text-red-600 transition-colors">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
          <p class="text-sm text-gray-600 mb-4">{{ category.description || 'No description' }}</p>
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">{{ category.itemCount || 0 }} items</span>
            <span class="bg-[var(--primary-color)] bg-opacity-10 text-[var(--primary-color)] px-3 py-1 rounded-full font-semibold">
              {{ category.displayOrder }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <CategoryModal
      v-if="showCreateModal"
      @close="showCreateModal = false"
      @saved="handleCategorySaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'

definePageMeta({
  layout: 'menu'
})

const loading = ref(true)
const categories = ref<any[]>([])
const showCreateModal = ref(false)
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0,
  scans: 0
})

// Provide stats to the layout
provide('menuStats', stats)

// Provide page metadata
provide('pageHeader', {
  title: 'Categories',
  description: 'Organize your menu items into categories'
})

provide('pageActions', () => h('button', {
  onClick: () => showCreateModal.value = true,
  class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold'
}, [
  h('i', { class: 'fas fa-plus mr-2' }),
  'Create Category'
]))

onMounted(async () => {
  await Promise.all([loadCategories(), loadStats()])
})

async function loadCategories() {
  loading.value = true
  try {
    const categoriesApi = useLibraryCategoriesApi()
    const response = await categoriesApi.getCategories()
    console.log('Categories response:', response)
    // Handle different response structures
    categories.value = Array.isArray(response) ? response : (response?.data || [])
    stats.value.categories = categories.value.length
  } catch (error) {
    console.error('Failed to load categories:', error)
    categories.value = []
  } finally {
    loading.value = false
  }
}

async function loadStats() {
  try {
    const libraryItemsApi = useLibraryItemsApi()
    const response = await libraryItemsApi.getItems()

    const items = Array.isArray(response) ? response : (response?.data || [])
    stats.value.items = items.length
  } catch (err) {
    console.error('Error loading stats:', err)
  }
}

function handleCategorySaved() {
  showCreateModal.value = false
  loadCategories()
}

useHead({
  title: 'Categories - Menu Dashboard',
})
</script>

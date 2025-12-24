<template>
  <div class="modal-overlay fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="$emit('close')">
    <div class="modal-content bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[80vh] flex flex-col">
      <!-- Header -->
      <div class="modal-header p-6 border-b border-gray-200">
        <div class="flex justify-between items-center">
          <h2 class="text-2xl font-bold text-gray-900">Add Items to Menu</h2>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Search and Filters -->
        <div class="mt-4 flex gap-4">
          <div class="flex-1">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search items..."
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
            />
          </div>
          <select
            v-model="selectedCategoryFilter"
            class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
          >
            <option :value="null">All Categories</option>
            <option v-for="cat in libraryCategories" :key="cat.id" :value="cat.id">
              {{ cat.name }}
            </option>
          </select>
        </div>
      </div>

      <!-- Body -->
      <div class="modal-body p-6 overflow-y-auto flex-1">
        <div v-if="loading" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>

        <div v-else-if="filteredItems.length === 0" class="text-center py-12">
          <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
          </svg>
          <p class="text-gray-600 font-medium">No items found</p>
          <p class="text-gray-500 text-sm mt-1">Try adjusting your search or filters</p>
        </div>

        <div v-else class="grid grid-cols-2 gap-4">
          <div
            v-for="item in filteredItems"
            :key="item.id"
            class="item-card border border-gray-200 rounded-lg overflow-hidden hover:shadow-md transition-all cursor-pointer"
            :class="{ 'ring-2 ring-primary': selectedItems.includes(item.id) }"
            @click="toggleItem(item.id)"
          >
            <div v-if="item.images && item.images.length > 0" class="aspect-video bg-gray-100">
              <img :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
            </div>
            <div v-else class="aspect-video bg-gray-100 flex items-center justify-center">
              <svg class="w-12 h-12 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>

            <div class="p-4">
              <div class="flex items-start justify-between mb-2">
                <h3 class="font-semibold text-gray-900">{{ item.name }}</h3>
                <input
                  type="checkbox"
                  :checked="selectedItems.includes(item.id)"
                  class="mt-1 h-5 w-5 text-primary border-gray-300 rounded focus:ring-primary"
                  @click.stop="toggleItem(item.id)"
                />
              </div>
              <p class="text-sm text-gray-500 line-clamp-2">{{ item.description }}</p>
              <div class="mt-2">
                <span class="text-lg font-bold text-primary">R{{ item.price?.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="modal-footer p-6 border-t border-gray-200">
        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Assign to Categories
          </label>
          <div class="flex flex-wrap gap-2">
            <label
              v-for="category in catalogCategories"
              :key="category.id"
              class="flex items-center gap-2 px-3 py-2 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50"
              :class="{ 'bg-primary-50 border-primary': selectedCategories.includes(category.id) }"
            >
              <input
                type="checkbox"
                :value="category.id"
                v-model="selectedCategories"
                class="h-4 w-4 text-primary border-gray-300 rounded focus:ring-primary"
              />
              <span class="text-sm">{{ category.name }}</span>
            </label>
          </div>
        </div>

        <div class="flex justify-between items-center">
          <p class="text-sm text-gray-600">
            {{ selectedItems.length }} item{{ selectedItems.length !== 1 ? 's' : '' }} selected
          </p>
          <div class="flex gap-3">
            <button
              @click="$emit('close')"
              class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="handleAddItems"
              :disabled="selectedItems.length === 0 || selectedCategories.length === 0"
              class="px-6 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              Add Items
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLibraryItemsApi, useLibraryCategoriesApi } from '~/composables/useApi'

const props = defineProps<{
  catalogId: number
}>()

const emit = defineEmits<{
  (e: 'select', itemIds: number[], categoryIds: number[]): void
  (e: 'close'): void
}>()

const libraryItemsApi = useLibraryItemsApi()
const libraryCategoriesApi = useLibraryCategoriesApi()

// State
const loading = ref(true)
const items = ref<any[]>([])
const libraryCategories = ref<any[]>([])
const catalogCategories = ref<any[]>([]) // Categories from the catalog
const searchQuery = ref('')
const selectedCategoryFilter = ref<number | null>(null)
const selectedItems = ref<number[]>([])
const selectedCategories = ref<number[]>([])

// Computed
const filteredItems = computed(() => {
  let filtered = items.value

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(query) ||
      item.description?.toLowerCase().includes(query)
    )
  }

  // Filter by category
  if (selectedCategoryFilter.value) {
    filtered = filtered.filter(item =>
      item.categoryIds?.includes(selectedCategoryFilter.value)
    )
  }

  return filtered
})

// Methods
const loadData = async () => {
  try {
    loading.value = true
    const [itemsResponse, categoriesResponse] = await Promise.all([
      libraryItemsApi.getItems(),
      libraryCategoriesApi.getCategories()
    ])

    if (itemsResponse.success) {
      items.value = itemsResponse.data
    }

    if (categoriesResponse.success) {
      libraryCategories.value = categoriesResponse.data
      catalogCategories.value = categoriesResponse.data // For now, same categories
    }
  } catch (error) {
    console.error('Failed to load items', error)
  } finally {
    loading.value = false
  }
}

const toggleItem = (itemId: number) => {
  const index = selectedItems.value.indexOf(itemId)
  if (index > -1) {
    selectedItems.value.splice(index, 1)
  } else {
    selectedItems.value.push(itemId)
  }
}

const handleAddItems = () => {
  if (selectedItems.value.length === 0 || selectedCategories.value.length === 0) {
    return
  }
  emit('select', selectedItems.value, selectedCategories.value)
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.modal-overlay {
  animation: fadeIn 0.2s ease-in-out;
}

.modal-content {
  animation: slideUp 0.3s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Filters & View Toggle -->
      <div class="mb-8">
        <div class="flex flex-wrap gap-4">
          <div class="flex-1 min-w-64">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search items..."
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
            />
          </div>
          <select
            v-model="selectedCategory"
            class="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
          >
            <option :value="null">All Categories</option>
            <option v-for="category in categories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>
          <!-- View Toggle -->
          <div class="flex border border-gray-300 rounded-lg overflow-hidden">
            <button
              @click="viewMode = 'grid'"
              :class="[
                'px-4 py-2 transition-colors',
                viewMode === 'grid' ? 'bg-[var(--primary-color)] text-white' : 'bg-white text-gray-700 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-th"></i>
            </button>
            <button
              @click="viewMode = 'list'"
              :class="[
                'px-4 py-2 transition-colors border-l border-gray-300',
                viewMode === 'list' ? 'bg-[var(--primary-color)] text-white' : 'bg-white text-gray-700 hover:bg-gray-50'
              ]"
            >
              <i class="fas fa-list"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Bulk Actions Bar -->
      <div
        v-if="selectedItems.length > 0"
        class="mb-4 p-4 bg-blue-50 border border-blue-200 rounded-lg flex items-center justify-between"
      >
        <div class="flex items-center gap-4">
          <span class="font-semibold text-blue-900">
            {{ selectedItems.length }} item{{ selectedItems.length > 1 ? 's' : '' }} selected
          </span>
          <button
            @click="clearSelection"
            class="text-blue-700 hover:text-blue-900 text-sm font-medium"
          >
            Clear selection
          </button>
        </div>
        <div class="flex items-center gap-2">
          <select
            v-model="bulkCategory"
            class="px-3 py-2 border border-gray-300 rounded-lg text-sm"
          >
            <option :value="null">Assign to category...</option>
            <option v-for="category in categories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>
          <button
            @click="applyBulkCategory"
            :disabled="!bulkCategory"
            class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Apply
          </button>
          <button
            @click="showBulkTagModal = true"
            class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors text-sm font-medium"
          >
            <i class="fas fa-tags mr-1"></i>
            Add Tags
          </button>
          <button
            @click="bulkDelete"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors text-sm font-medium"
          >
            <i class="fas fa-trash mr-1"></i>
            Delete
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)]"></div>
      </div>

      <!-- Empty State -->
      <div v-else-if="filteredItems.length === 0" class="text-center py-20">
        <i class="fas fa-box-open text-6xl text-gray-300 mb-4"></i>
        <h3 class="text-xl font-semibold text-gray-900 mb-2">No items found</h3>
        <p class="text-gray-600 mb-6">
          {{ searchQuery ? 'Try adjusting your search' : 'Start by adding your first item' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="showItemModal = true"
          class="px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors"
        >
          <i class="fas fa-plus mr-2"></i>
          Add Your First Item
        </button>
      </div>

      <!-- Grid View -->
      <div v-else-if="viewMode === 'grid'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="item in filteredItems"
          :key="item.id"
          class="bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow overflow-hidden relative"
        >
          <!-- Selection Checkbox -->
          <div class="absolute top-4 left-4 z-10">
            <input
              type="checkbox"
              :checked="selectedItems.includes(item.id)"
              @change="toggleItemSelection(item.id)"
              class="w-5 h-5 text-[var(--primary-color)] rounded"
            />
          </div>

          <!-- Item Image -->
          <div class="h-48 bg-gradient-to-br from-gray-200 to-gray-300 relative">
            <img
              v-if="item.images && item.images.length > 0"
              :src="item.images[0]"
              :alt="item.name"
              class="w-full h-full object-cover"
            />
            <div v-else class="w-full h-full flex items-center justify-center">
              <i class="fas fa-utensils text-6xl text-gray-400"></i>
            </div>
            <div v-if="item.tags && item.tags.length > 0" class="absolute top-2 right-2">
              <span class="px-2 py-1 bg-green-500 text-white text-xs font-semibold rounded">
                {{ item.tags.length }} tag{{ item.tags.length > 1 ? 's' : '' }}
              </span>
            </div>
          </div>

          <!-- Item Info -->
          <div class="p-4">
            <div class="mb-2">
              <h3 class="text-lg font-semibold text-gray-900">{{ item.name }}</h3>
              <p v-if="item.description" class="text-sm text-gray-600 line-clamp-2 mt-1">
                {{ item.description }}
              </p>
            </div>

            <div v-if="item.categoryId" class="mb-2">
              <span class="inline-block px-2 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded">
                {{ getCategoryName(item.categoryId) }}
              </span>
            </div>

            <div class="flex items-center justify-between mb-4">
              <span class="text-xl font-bold text-[var(--primary-color)]">
                R{{ item.price.toFixed(2) }}
              </span>
              <span v-if="item.variants.length > 0" class="text-sm text-gray-600">
                {{ item.variants.length }} variant{{ item.variants.length > 1 ? 's' : '' }}
              </span>
            </div>

            <div v-if="item.tags && item.tags.length > 0" class="flex flex-wrap gap-1 mb-4">
              <span
                v-for="tag in item.tags.slice(0, 3)"
                :key="tag"
                class="px-2 py-0.5 bg-gray-100 text-gray-700 text-xs rounded"
              >
                {{ tag }}
              </span>
              <span v-if="item.tags.length > 3" class="px-2 py-0.5 bg-gray-100 text-gray-700 text-xs rounded">
                +{{ item.tags.length - 3 }}
              </span>
            </div>

            <div class="flex gap-2">
              <button
                @click="editItem(item)"
                class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors text-sm font-medium"
              >
                <i class="fas fa-edit mr-1"></i>
                Edit
              </button>
              <button
                @click="deleteItem(item)"
                class="px-4 py-2 border border-red-300 text-red-600 rounded-lg hover:bg-red-50 transition-colors text-sm font-medium"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else class="bg-white rounded-lg shadow overflow-hidden">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left">
                <input
                  type="checkbox"
                  :checked="isAllSelected"
                  @change="toggleSelectAll"
                  class="w-5 h-5 text-[var(--primary-color)] rounded"
                />
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Item
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Category
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Price
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Tags
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Variants
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr
              v-for="item in filteredItems"
              :key="item.id"
              :class="selectedItems.includes(item.id) ? 'bg-blue-50' : 'hover:bg-gray-50'"
            >
              <td class="px-6 py-4">
                <input
                  type="checkbox"
                  :checked="selectedItems.includes(item.id)"
                  @change="toggleItemSelection(item.id)"
                  class="w-5 h-5 text-[var(--primary-color)] rounded"
                />
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-lg overflow-hidden bg-gray-100 flex-shrink-0">
                    <img
                      v-if="item.images && item.images.length > 0"
                      :src="item.images[0]"
                      :alt="item.name"
                      class="w-full h-full object-cover"
                    />
                    <i v-else class="fas fa-utensils text-gray-400 flex items-center justify-center w-full h-full"></i>
                  </div>
                  <div>
                    <div class="font-semibold text-gray-900">{{ item.name }}</div>
                    <div v-if="item.description" class="text-sm text-gray-600 truncate max-w-xs">
                      {{ item.description }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <span v-if="item.categoryId" class="px-2 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded">
                  {{ getCategoryName(item.categoryId) }}
                </span>
                <span v-else class="text-sm text-gray-500">-</span>
              </td>
              <td class="px-6 py-4">
                <span class="font-semibold text-gray-900">R{{ item.price.toFixed(2) }}</span>
              </td>
              <td class="px-6 py-4">
                <div v-if="item.tags && item.tags.length > 0" class="flex flex-wrap gap-1">
                  <span
                    v-for="tag in item.tags.slice(0, 2)"
                    :key="tag"
                    class="px-2 py-0.5 bg-gray-100 text-gray-700 text-xs rounded"
                  >
                    {{ tag }}
                  </span>
                  <span v-if="item.tags.length > 2" class="px-2 py-0.5 bg-gray-100 text-gray-700 text-xs rounded">
                    +{{ item.tags.length - 2 }}
                  </span>
                </div>
                <span v-else class="text-sm text-gray-500">-</span>
              </td>
              <td class="px-6 py-4">
                <span class="text-sm text-gray-900">{{ item.variants.length || 0 }}</span>
              </td>
              <td class="px-6 py-4 text-right">
                <div class="flex justify-end gap-2">
                  <button
                    @click="editItem(item)"
                    class="px-3 py-1.5 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors text-sm font-medium"
                  >
                    <i class="fas fa-edit mr-1"></i>
                    Edit
                  </button>
                  <button
                    @click="deleteItem(item)"
                    class="px-3 py-1.5 text-red-600 hover:bg-red-50 rounded-lg transition-colors text-sm font-medium"
                  >
                    <i class="fas fa-trash mr-1"></i>
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      </div>
    </div>

    <!-- Item Modal -->
    <ItemFormModal
      v-if="showItemModal"
      :item="editingItem"
      :categories="categories"
      @close="closeItemModal"
      @saved="itemSaved"
    />

    <!-- Category Modal -->
    <CategoryModal
      v-if="showCategoryModal"
      @close="showCategoryModal = false"
      @saved="loadCategories"
    />

    <!-- Bulk Tag Modal -->
    <BulkTagModal
      v-if="showBulkTagModal"
      @close="showBulkTagModal = false"
      @apply="applyBulkTags"
    />
</template>

<script setup lang="ts">
import { ref, computed, onMounted, h } from 'vue'
import { useLibraryItemsApi, useLibraryCategoriesApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

definePageMeta({
  layout: 'menu'
})

const libraryItemsApi = useLibraryItemsApi()
const categoriesApi = useLibraryCategoriesApi()
const toast = useToast()

const loading = ref(false)
const items = ref<any[]>([])
const categories = ref<any[]>([])
const searchQuery = ref('')
const selectedCategory = ref<number | null>(null)
const showItemModal = ref(false)
const showCategoryModal = ref(false)
const showBulkTagModal = ref(false)
const editingItem = ref<any>(null)
const viewMode = ref<'grid' | 'list'>('grid')
const selectedItems = ref<number[]>([])
const bulkCategory = ref<number | null>(null)

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActions } = usePageMeta()

const filteredItems = computed(() => {
  let filtered = items.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(query) ||
      item.description?.toLowerCase().includes(query)
    )
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(item => item.categoryId === selectedCategory.value)
  }

  return filtered
})

const isAllSelected = computed(() => {
  return filteredItems.value.length > 0 && selectedItems.value.length === filteredItems.value.length
})

onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Library Items',
    description: 'Manage your menu items and add them to any menu'
  })

  setPageActions(() => h('button', {
    onClick: () => showItemModal.value = true,
    class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold'
  }, [
    h('i', { class: 'fas fa-plus mr-2' }),
    'Add Item'
  ]))

  await Promise.all([loadItems(), loadCategories()])
})

async function loadItems() {
  try {
    loading.value = true
    const response = await libraryItemsApi.getItems()
    console.log(response);
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.items)) {
      items.value = response.data.items
    } else if (Array.isArray(response.data)) {
      items.value = response.data
    } else {
      items.value = []
    }
    // Update stats
    stats.value.items = items.value.length
  } catch (error) {
    console.error('Error loading items:', error)
    toast.error('Failed to load items')
    items.value = []
  } finally {
    loading.value = false
  }
}

async function loadCategories() {
  try {
    const response = await categoriesApi.getCategories()
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.categories)) {
      categories.value = response.data.categories
    } else if (Array.isArray(response.data)) {
      categories.value = response.data
    } else {
      categories.value = []
    }
    // Update stats
    stats.value.categories = categories.value.length
  } catch (error) {
    console.error('Error loading categories:', error)
    categories.value = []
  }
}

function getCategoryName(categoryId: number) {
  return categories.value.find(c => c.id === categoryId)?.name || 'Uncategorized'
}

function toggleItemSelection(itemId: number) {
  const index = selectedItems.value.indexOf(itemId)
  if (index > -1) {
    selectedItems.value.splice(index, 1)
  } else {
    selectedItems.value.push(itemId)
  }
}

function toggleSelectAll() {
  if (isAllSelected.value) {
    selectedItems.value = []
  } else {
    selectedItems.value = filteredItems.value.map(item => item.id)
  }
}

function clearSelection() {
  selectedItems.value = []
}

async function applyBulkCategory() {
  if (!bulkCategory.value) return

  try {
    for (const itemId of selectedItems.value) {
      await libraryItemsApi.updateItem(itemId, { categoryId: bulkCategory.value })
    }
    toast.success(`Updated ${selectedItems.value.length} items`)
    await loadItems()
    clearSelection()
    bulkCategory.value = null
  } catch (error) {
    console.error('Error updating items:', error)
    toast.error('Failed to update items')
  }
}

async function applyBulkTags(tags: string[]) {
  try {
    for (const itemId of selectedItems.value) {
      const item = items.value.find(i => i.id === itemId)
      const existingTags = item?.tags || []
      const newTags = [...new Set([...existingTags, ...tags])]
      await libraryItemsApi.updateItem(itemId, { tags: newTags })
    }
    toast.success(`Added tags to ${selectedItems.value.length} items`)
    await loadItems()
    clearSelection()
  } catch (error) {
    console.error('Error adding tags:', error)
    toast.error('Failed to add tags')
  }
}

async function bulkDelete() {
  if (!confirm(`Are you sure you want to delete ${selectedItems.value.length} item(s)?`)) return

  try {
    for (const itemId of selectedItems.value) {
      await libraryItemsApi.deleteItem(itemId)
    }
    toast.success(`Deleted ${selectedItems.value.length} items`)
    await loadItems()
    clearSelection()
  } catch (error) {
    console.error('Error deleting items:', error)
    toast.error('Failed to delete items')
  }
}

function editItem(item: any) {
  editingItem.value = item
  showItemModal.value = true
}

async function deleteItem(item: any) {
  if (!confirm(`Are you sure you want to delete "${item.name}"?`)) return

  try {
    await libraryItemsApi.deleteItem(item.id)
    toast.success('Item deleted successfully')
    await loadItems()
  } catch (error) {
    console.error('Error deleting item:', error)
    toast.error('Failed to delete item')
  }
}

function closeItemModal() {
  showItemModal.value = false
  editingItem.value = null
}

async function itemSaved() {
  closeItemModal()
  await loadItems()
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

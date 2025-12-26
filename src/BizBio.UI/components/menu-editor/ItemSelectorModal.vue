<template>
  <div class="modal-overlay fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4 animate-fadeSlide" @click.self="$emit('close')">
    <div class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-4xl w-full max-h-[90vh] flex flex-col overflow-hidden border border-md-outline-variant">
      <!-- Header with Gradient -->
      <div class="modal-header p-6 border-b-2 border-md-primary/20 relative overflow-hidden flex-shrink-0">
        <div class="absolute inset-0 bg-gradient-primary opacity-5"></div>
        <div class="flex justify-between items-center relative z-10">
          <div>
            <h2 class="text-2xl font-heading font-bold gradient-text">Add Items to Menu</h2>
            <p class="text-sm text-md-on-surface-variant mt-1">Select items to add to your menu category</p>
          </div>
          <div class="flex items-center gap-2">
            <button
              @click="showItemFormModal = true"
              class="px-4 py-2 bg-gradient-tertiary text-white rounded-xl hover:shadow-md-3 transition-all font-semibold shadow-md-2 md-ripple flex items-center gap-2"
            >
              <i class="fas fa-plus"></i>
              <span class="hidden sm:inline">Create New Item</span>
            </button>
            <button
              @click="$emit('close')"
              class="w-10 h-10 rounded-full bg-md-error-container text-md-on-error-container hover:bg-md-error hover:text-md-on-error transition-all md-ripple shadow-md-1"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <!-- Search and Filters -->
        <div class="mt-4 space-y-3 relative z-10">
          <div class="flex gap-3 flex-wrap">
            <div class="flex-1 min-w-[200px]">
              <div class="relative">
                <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-md-on-surface-variant"></i>
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search items..."
                  class="w-full pl-11 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all"
                />
              </div>
            </div>
            <select
              v-model="selectedCategoryFilter"
              class="px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all"
            >
              <option :value="null">All Categories</option>
              <option v-for="cat in libraryCategories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>

            <!-- View Toggle -->
            <div class="flex bg-md-surface-container rounded-xl p-1">
              <button
                @click="viewMode = 'table'"
                :class="[
                  'px-4 py-2 text-sm font-medium rounded-lg transition-all md-ripple',
                  viewMode === 'table' ? 'bg-gradient-primary text-white shadow-md-2' : 'text-md-on-surface-variant hover:text-md-primary'
                ]"
              >
                <i class="fas fa-list mr-2"></i>
                Table
              </button>
              <button
                @click="viewMode = 'grid'"
                :class="[
                  'px-4 py-2 text-sm font-medium rounded-lg transition-all md-ripple',
                  viewMode === 'grid' ? 'bg-gradient-secondary text-white shadow-md-2' : 'text-md-on-surface-variant hover:text-md-secondary'
                ]"
              >
                <i class="fas fa-th mr-2"></i>
                Grid
              </button>
            </div>

            <!-- Thumbnail Size Slider (only for grid view) -->
            <div v-if="viewMode === 'grid'" class="flex items-center gap-3 px-4 py-2 bg-md-surface-container border border-md-outline-variant rounded-xl">
              <i class="fas fa-image text-md-on-surface-variant"></i>
              <input
                v-model="thumbnailSize"
                type="range"
                min="80"
                max="200"
                step="20"
                class="w-24 accent-md-primary"
              />
              <span class="text-sm font-medium text-md-on-surface-variant">{{ thumbnailSize }}px</span>
            </div>
          </div>

          <!-- Unassigned Filter -->
          <label class="flex items-center gap-3 cursor-pointer group">
            <input
              type="checkbox"
              v-model="showOnlyUnassigned"
              class="w-5 h-5 text-md-primary bg-md-surface-container border-md-outline rounded-md focus:ring-2 focus:ring-md-primary transition-all"
            />
            <span class="text-sm text-md-on-surface">Show only unassigned items</span>
          </label>
        </div>
      </div>

      <!-- Body -->
      <div class="modal-body p-6 overflow-y-auto flex-1 min-h-0">
        <div v-if="loading" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>

        <div v-else-if="filteredItems.length === 0" class="text-center py-12">
          <svg class="w-16 h-16 text-md-on-surface-variant opacity-50 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
          </svg>
          <p class="text-md-on-surface-variant font-medium">No items found</p>
          <p class="text-md-on-surface-variant text-sm mt-1">Try adjusting your search or filters</p>
        </div>

        <!-- Table View -->
        <div v-else-if="viewMode === 'table'">
          <table class="w-full">
            <thead>
              <tr class="border-b-2 border-md-primary/20">
                <th class="text-left py-2 px-3 font-semibold text-md-on-surface w-8"></th>
                <th class="text-left py-2 px-3 font-semibold text-md-on-surface w-12">Image</th>
                <th class="text-left py-2 px-3 font-semibold text-md-on-surface">Name</th>
                <th class="text-left py-2 px-3 font-semibold text-md-on-surface">Description</th>
                <th class="text-left py-2 px-3 font-semibold text-md-on-surface">Price</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-100">
              <tr
                v-for="item in filteredItems"
                :key="item.id"
                class="hover:bg-md-surface-container-low transition-colors cursor-pointer transition-colors"
                :class="{ 'bg-primary-50': selectedItems.includes(item.id) }"
                @click="toggleItem(item.id)"
              >
                <td class="py-2 px-3">
                  <input
                    type="checkbox"
                    :checked="selectedItems.includes(item.id)"
                    class="h-4 w-4 text-primary border-md-outline rounded focus:ring-primary"
                    @click.stop="toggleItem(item.id)"
                  />
                </td>
                <td class="py-2 px-3">
                  <div class="w-10 h-10 rounded overflow-hidden bg-md-surface-container-low flex items-center justify-center">
                    <img v-if="item.images && item.images.length > 0" :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
                    <svg v-else class="w-6 h-6 text-md-on-surface-variant opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                  </div>
                </td>
                <td class="py-2 px-3">
                  <p class="font-medium text-md-on-surface">{{ item.name }}</p>
                </td>
                <td class="py-2 px-3">
                  <p class="text-sm text-md-on-surface-variant truncate max-w-md">{{ item.description }}</p>
                </td>
                <td class="py-2 px-3">
                  <span class="font-semibold text-primary">R{{ item.price?.toFixed(2) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Grid View -->
        <div v-else class="grid gap-3" :style="{ gridTemplateColumns: `repeat(auto-fill, minmax(${thumbnailSize}px, 1fr))` }">
          <div
            v-for="item in filteredItems"
            :key="item.id"
            class="item-card border border-md-outline-variant rounded-lg overflow-hidden hover:shadow-md transition-all cursor-pointer relative"
            :class="{ 'ring-2 ring-primary': selectedItems.includes(item.id) }"
            @click="toggleItem(item.id)"
          >
            <div class="relative">
              <div v-if="item.images && item.images.length > 0" class="aspect-square bg-md-surface-container-low">
                <img :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
              </div>
              <div v-else class="aspect-square bg-md-surface-container-low flex items-center justify-center">
                <svg class="w-8 h-8 text-md-on-surface-variant opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </div>
              <!-- Price positioned at bottom of image -->
              <div class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent px-3 py-2">
                <span class="text-sm font-bold text-white">R{{ item.price?.toFixed(2) }}</span>
              </div>
            </div>
              
              <!-- Checkbox positioned at top right of image -->
              <div class="absolute top-2 right-2">
                <input
                  type="checkbox"
                  :checked="selectedItems.includes(item.id)"
                  class="h-5 w-5 text-primary border-2 border-white rounded shadow-lg focus:ring-primary bg-md-surface"
                  @click.stop="toggleItem(item.id)"
                />
              </div>

              
            <!-- </div> -->

            <div class="p-3">
              <h3 class="font-medium text-sm text-md-on-surface line-clamp-2">{{ item.name }}</h3>
              </div>
            </div>
          </div>
        </div>
      <!-- </div> -->

      <!-- Footer with Gradient Background -->
      <div class="modal-footer p-6 border-t border-md-outline-variant relative overflow-hidden flex-shrink-0">
        <div class="absolute inset-0 mesh-bg-2 opacity-30"></div>
        <div class="relative z-10 space-y-4">
          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-3 flex items-center gap-2">
              <i class="fas fa-layer-group text-md-primary"></i>
              Assign to Categories
              <span class="text-xs font-normal text-md-on-surface-variant">(Required)</span>
            </label>
            <div class="flex flex-wrap gap-2 max-h-32 overflow-y-auto">
              <label
                v-for="category in catalogCategories"
                :key="category.id"
                class="flex items-center gap-2 px-4 py-2.5 border rounded-xl cursor-pointer transition-all md-ripple"
                :class="selectedCategories.includes(category.id)
                  ? 'bg-gradient-primary text-white border-transparent shadow-glow-purple'
                  : 'bg-md-surface-container border-md-outline-variant hover:border-md-primary hover:shadow-md-1'"
              >
                <input
                  type="checkbox"
                  :value="category.id"
                  v-model="selectedCategories"
                  class="w-4 h-4 rounded accent-md-primary"
                />
                <span class="text-sm font-medium">{{ category.name }}</span>
              </label>
            </div>
          </div>

          <div class="flex justify-between items-center pt-2">
            <div class="flex items-center gap-2 px-4 py-2 bg-md-primary-container rounded-xl">
              <i class="fas fa-check-circle text-md-primary"></i>
              <p class="text-sm font-bold text-md-on-primary-container">
                {{ selectedItems.length }} item{{ selectedItems.length !== 1 ? 's' : '' }} selected
              </p>
            </div>
            <div class="flex gap-3">
              <button
                @click="$emit('close')"
                class="px-6 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl text-md-on-surface-variant font-medium hover:bg-md-surface-container-low transition-colors-high hover:shadow-md-1 transition-all md-ripple"
              >
                Cancel
              </button>
              <button
                @click="handleAddItems"
                :disabled="selectedItems.length === 0 || selectedCategories.length === 0"
                class="px-6 py-3 btn-gradient rounded-xl font-bold shadow-md-2 hover:shadow-glow-purple disabled:opacity-50 disabled:cursor-not-allowed transition-all md-ripple flex items-center gap-2"
              >
                <i class="fas fa-plus-circle"></i>
                Add Items
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Item Form Modal (for creating new items) -->
    <ItemFormModal
      v-if="showItemFormModal"
      :categories="libraryCategories"
      @close="showItemFormModal = false"
      @saved="handleItemCreated"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLibraryItemsApi, useCatalogsApi } from '~/composables/useApi'
import ItemFormModal from '~/components/ItemFormModal.vue'

const props = defineProps<{
  catalogId: number
  preSelectedCategory?: number | null
}>()

const emit = defineEmits<{
  (e: 'select', itemIds: number[], categoryIds: number[]): void
  (e: 'close'): void
}>()

const libraryItemsApi = useLibraryItemsApi()
const catalogsApi = useCatalogsApi()

// State
const loading = ref(true)
const items = ref<any[]>([])
const libraryCategories = ref<any[]>([])
const catalogCategories = ref<any[]>([]) // Categories from the catalog
const searchQuery = ref('')
const selectedCategoryFilter = ref<number | null>(null)
const selectedItems = ref<number[]>([])
const selectedCategories = ref<number[]>([])
const viewMode = ref<'table' | 'grid'>('table')
const showOnlyUnassigned = ref(true) // Default to true to show only unassigned items
const thumbnailSize = ref(120) // px for grid view
const showItemFormModal = ref(false)

// Computed
const filteredItems = computed(() => {
  // Ensure we always have an array
  if (!Array.isArray(items.value)) {
    console.warn('items.value is not an array:', items.value)
    return []
  }

  let filtered = items.value

  // Filter by unassigned status (items not in THIS catalog)
  if (showOnlyUnassigned.value) {
    const catalogItemIds = new Set(
      catalogItems.value.map((item: any) => item.libraryItemId || item.id)
    )
    filtered = filtered.filter(item => !catalogItemIds.has(item.id))
  }

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name?.toLowerCase().includes(query) ||
      item.description?.toLowerCase().includes(query)
    )
  }

  // Filter by category (library category, not catalog category)
  if (selectedCategoryFilter.value) {
    filtered = filtered.filter(item =>
      item.categoryId === selectedCategoryFilter.value
    )
  }

  return filtered
})

// Store catalog items for unassigned filter
const catalogItems = ref<any[]>([])

// Methods
const loadData = async () => {
  try {
    loading.value = true
    const [itemsResponse, catalogResponse] = await Promise.all([
      libraryItemsApi.getItems(),
      catalogsApi.getCatalogDetail(props.catalogId)
    ])

    console.log('Items response:', itemsResponse)
    console.log('Catalog response:', catalogResponse)

    // Handle items response - API returns { success: true, data: { items: [...] } }
    if (itemsResponse) {
      if (itemsResponse.success && itemsResponse.data) {
        // Check if data has items property (LibraryItemsController format)
        if (Array.isArray(itemsResponse.data.items)) {
          items.value = itemsResponse.data.items
        } else if (Array.isArray(itemsResponse.data)) {
          items.value = itemsResponse.data
        } else {
          console.error('Unexpected items response format:', itemsResponse)
          items.value = []
        }
      } else if (Array.isArray(itemsResponse)) {
        items.value = itemsResponse
      } else {
        console.error('Unexpected items response format:', itemsResponse)
        items.value = []
      }
    }

    // Handle catalog categories response
    if (catalogResponse && catalogResponse.success && catalogResponse.data) {
      if (Array.isArray(catalogResponse.data.categories)) {
        catalogCategories.value = catalogResponse.data.categories
        libraryCategories.value = catalogResponse.data.categories
      } else {
        console.error('Unexpected catalog response format:', catalogResponse)
        catalogCategories.value = []
        libraryCategories.value = []
      }

      // Store catalog items for unassigned filter
      if (Array.isArray(catalogResponse.data.items)) {
        catalogItems.value = catalogResponse.data.items
      }
    }

    // Auto-select pre-selected category if provided
    if (props.preSelectedCategory) {
      selectedCategories.value = [props.preSelectedCategory]
    }

    console.log('Items loaded:', items.value.length)
    console.log('Categories loaded:', catalogCategories.value.length)
    console.log('Catalog items:', catalogItems.value.length)
    console.log('Pre-selected category:', props.preSelectedCategory)
  } catch (error) {
    console.error('Failed to load items', error)
    items.value = []
    libraryCategories.value = []
    catalogCategories.value = []
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

const handleItemCreated = async () => {
  showItemFormModal.value = false
  // Reload items to include the newly created item
  await loadData()
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





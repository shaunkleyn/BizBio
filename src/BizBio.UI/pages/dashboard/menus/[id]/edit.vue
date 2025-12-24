<template>
  <NuxtLayout name="dashboard">
    <div class="menu-editor-container">
      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center min-h-screen">
        <div class="text-center">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
          <p class="text-gray-600">Loading menu...</p>
        </div>
      </div>

      <!-- Menu Editor -->
      <div v-else-if="catalog" class="menu-editor">
        <!-- Header -->
        <div class="editor-header bg-white shadow-sm rounded-lg p-6 mb-6">
          <div class="flex justify-between items-center">
            <div>
              <h1 class="text-3xl font-bold text-gray-900">{{ catalog.name }}</h1>
              <p class="text-gray-600 mt-1">Edit your menu structure</p>
            </div>
            <div class="flex gap-3">
              <button
                @click="handleCancel"
                class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
              >
                Cancel
              </button>
              <button
                @click="handleSave"
                :disabled="isSaving || !hasChanges"
                class="px-6 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {{ isSaving ? 'Saving...' : 'Save Changes' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Two-Panel Layout -->
        <div class="editor-panels grid grid-cols-12 gap-6">
          <!-- Left: Category Sidebar -->
          <div class="col-span-3">
            <div class="bg-white shadow-sm rounded-lg p-4">
              <div class="flex justify-between items-center mb-4">
                <h2 class="text-lg font-semibold text-gray-900">Categories</h2>
                <button
                  @click="showCategoryModal = true"
                  class="p-2 text-primary hover:bg-primary-50 rounded-lg transition-colors"
                  title="Add Category"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                  </svg>
                </button>
              </div>

              <div
                ref="categoryList"
                class="category-list space-y-2"
              >
                <div
                  v-for="category in categories"
                  :key="category.id"
                  :data-id="category.id"
                  class="category-item group relative p-3 rounded-lg border cursor-pointer transition-all"
                  :class="{
                    'border-primary bg-primary-50': selectedCategoryId === category.id,
                    'border-gray-200 hover:border-gray-300': selectedCategoryId !== category.id
                  }"
                  @click="selectedCategoryId = category.id"
                >
                  <div class="flex items-center gap-3">
                    <div class="drag-handle cursor-grab active:cursor-grabbing text-gray-400 hover:text-gray-600">
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16" />
                      </svg>
                    </div>
                    <div class="flex-1 min-w-0">
                      <p class="font-medium text-gray-900 truncate">{{ category.name }}</p>
                      <p class="text-sm text-gray-500">{{ category.itemCount }} items</p>
                    </div>
                  </div>
                </div>
              </div>

              <div v-if="categories.length === 0" class="text-center py-8 text-gray-500">
                No categories yet. Add your first category!
              </div>
            </div>
          </div>

          <!-- Right: Items Panel -->
          <div class="col-span-9">
            <div class="bg-white shadow-sm rounded-lg p-6">
              <div class="flex justify-between items-center mb-6">
                <h2 class="text-lg font-semibold text-gray-900">
                  {{ selectedCategory ? selectedCategory.name : 'All Items' }}
                </h2>
                <div class="flex gap-2">
                  <button
                    @click="showItemSelector = true"
                    class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors"
                  >
                    + Add Item
                  </button>
                  <button
                    @click="showBundleSelector = true"
                    class="px-4 py-2 border border-primary text-primary rounded-lg hover:bg-primary-50 transition-colors"
                  >
                    + Add Bundle
                  </button>
                </div>
              </div>

              <!-- Items Grid -->
              <div
                v-if="currentCategoryItems.length > 0 || currentCategoryBundles.length > 0"
                ref="itemsGrid"
                class="items-grid grid grid-cols-3 gap-4"
              >
                <!-- Items -->
                <div
                  v-for="item in currentCategoryItems"
                  :key="'item-' + item.id"
                  :data-id="item.id"
                  :data-type="'item'"
                  class="item-card group relative bg-white border border-gray-200 rounded-lg overflow-hidden hover:shadow-md transition-all"
                >
                  <div class="drag-handle absolute top-2 right-2 p-2 bg-white rounded-lg shadow cursor-grab active:cursor-grabbing opacity-0 group-hover:opacity-100 transition-opacity">
                    <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16" />
                    </svg>
                  </div>

                  <div v-if="item.images && item.images.length > 0" class="aspect-video bg-gray-100">
                    <img :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
                  </div>
                  <div v-else class="aspect-video bg-gray-100 flex items-center justify-center">
                    <svg class="w-12 h-12 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                  </div>

                  <div class="p-4">
                    <h3 class="font-semibold text-gray-900 truncate">{{ item.name }}</h3>
                    <p class="text-sm text-gray-500 truncate">{{ item.description }}</p>
                    <div class="mt-2 flex justify-between items-center">
                      <span class="text-lg font-bold text-primary">R{{ item.price.toFixed(2) }}</span>
                      <div class="flex gap-1">
                        <span v-if="item.hasOptions" class="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">Options</span>
                        <span v-if="item.hasExtras" class="px-2 py-1 bg-green-100 text-green-800 text-xs rounded">Extras</span>
                        <span v-if="item.variantCount > 0" class="px-2 py-1 bg-purple-100 text-purple-800 text-xs rounded">{{ item.variantCount }} vars</span>
                      </div>
                    </div>

                    <div class="mt-3 flex gap-2">
                      <button
                        @click="editItem(item)"
                        class="flex-1 px-3 py-1 text-sm bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors"
                      >
                        Edit
                      </button>
                      <button
                        @click="removeItem(item.id)"
                        class="px-3 py-1 text-sm bg-red-50 text-red-600 rounded hover:bg-red-100 transition-colors"
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Bundles -->
                <div
                  v-for="bundle in currentCategoryBundles"
                  :key="'bundle-' + bundle.id"
                  :data-id="bundle.id"
                  :data-type="'bundle'"
                  class="item-card group relative bg-white border border-gray-200 rounded-lg overflow-hidden hover:shadow-md transition-all"
                >
                  <div class="drag-handle absolute top-2 right-2 p-2 bg-white rounded-lg shadow cursor-grab active:cursor-grabbing opacity-0 group-hover:opacity-100 transition-opacity">
                    <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16" />
                    </svg>
                  </div>

                  <div v-if="bundle.images && bundle.images.length > 0" class="aspect-video bg-gray-100">
                    <img :src="bundle.images[0]" :alt="bundle.name" class="w-full h-full object-cover" />
                  </div>
                  <div v-else class="aspect-video bg-gray-100 flex items-center justify-center">
                    <svg class="w-12 h-12 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                  </div>

                  <div class="p-4">
                    <div class="flex items-center gap-2 mb-2">
                      <span class="px-2 py-1 bg-orange-100 text-orange-800 text-xs font-semibold rounded">BUNDLE</span>
                    </div>
                    <h3 class="font-semibold text-gray-900 truncate">{{ bundle.name }}</h3>
                    <p class="text-sm text-gray-500 truncate">{{ bundle.description }}</p>
                    <div class="mt-2">
                      <span class="text-lg font-bold text-primary">R{{ bundle.basePrice.toFixed(2) }}</span>
                    </div>

                    <div class="mt-3 flex gap-2">
                      <button
                        @click="editBundle(bundle)"
                        class="flex-1 px-3 py-1 text-sm bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors"
                      >
                        Edit
                      </button>
                      <button
                        @click="removeBundle(bundle.id)"
                        class="px-3 py-1 text-sm bg-red-50 text-red-600 rounded hover:bg-red-100 transition-colors"
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Empty State -->
              <div v-else class="text-center py-12">
                <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <p class="text-gray-600 font-medium">No items in this category yet</p>
                <p class="text-gray-500 text-sm mt-1">Add items or bundles to get started</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Modals -->
      <ItemSelectorModal
        v-if="showItemSelector"
        :catalog-id="catalogId"
        @select="addItemsToMenu"
        @close="showItemSelector = false"
      />

      <BundleSelectorModal
        v-if="showBundleSelector"
        :catalog-id="catalogId"
        @select="addBundleToMenu"
        @close="showBundleSelector = false"
      />
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCatalogsApi } from '~/composables/useApi'
import { useDragDrop } from '~/composables/useDragDrop'

const route = useRoute()
const router = useRouter()
const catalogsApi = useCatalogsApi()
const { enableSortable } = useDragDrop()

// State
const catalogId = computed(() => parseInt(route.params.id as string))
const loading = ref(true)
const isSaving = ref(false)
const hasChanges = ref(false)
const catalog = ref<any>(null)
const categories = ref<any[]>([])
const selectedCategoryId = ref<number | null>(null)
const showItemSelector = ref(false)
const showBundleSelector = ref(false)
const showCategoryModal = ref(false)

// Refs for sortable
const categoryList = ref<HTMLElement | null>(null)
const itemsGrid = ref<HTMLElement | null>(null)

// Computed
const selectedCategory = computed(() => {
  return categories.value.find(c => c.id === selectedCategoryId.value)
})

const currentCategoryItems = computed(() => {
  if (!catalog.value || !selectedCategoryId.value) return []
  return catalog.value.items.filter((item: any) =>
    item.categoryIds.includes(selectedCategoryId.value)
  )
})

const currentCategoryBundles = computed(() => {
  if (!catalog.value || !selectedCategoryId.value) return []
  return catalog.value.bundles.filter((bundle: any) =>
    bundle.categoryIds.includes(selectedCategoryId.value)
  )
})

// Load catalog data
const loadCatalog = async () => {
  try {
    loading.value = true
    const response = await catalogsApi.getCatalogDetail(catalogId.value)
    if (response.success) {
      catalog.value = response.data
      categories.value = response.data.categories
      if (categories.value.length > 0 && !selectedCategoryId.value) {
        selectedCategoryId.value = categories.value[0].id
      }

      // Enable drag-drop after DOM updates
      nextTick(() => {
        enableDragDrop()
      })
    }
  } catch (error) {
    console.error('Failed to load catalog', error)
    router.push('/dashboard/menus')
  } finally {
    loading.value = false
  }
}

// Enable drag-and-drop
const enableDragDrop = () => {
  // Categories drag-and-drop
  if (categoryList.value) {
    enableSortable(categoryList.value, {
      onUpdate: handleCategoryReorder
    })
  }

  // Items drag-and-drop
  if (itemsGrid.value) {
    enableSortable(itemsGrid.value, {
      onUpdate: handleItemReorder
    })
  }
}

// Handlers
const handleCategoryReorder = async (event: any) => {
  const { oldIndex, newIndex } = event

  if (oldIndex === newIndex) return

  // Optimistic update
  const items = [...categories.value]
  const [moved] = items.splice(oldIndex, 1)
  items.splice(newIndex, 0, moved)

  // Update sort orders
  const reorderDto = {
    items: items.map((cat, index) => ({
      id: cat.id,
      sortOrder: index
    }))
  }

  categories.value = items
  hasChanges.value = true

  try {
    await catalogsApi.reorderCategories(catalogId.value, reorderDto)
  } catch (error) {
    console.error('Failed to reorder categories', error)
    // Reload on error
    await loadCatalog()
  }
}

const handleItemReorder = async (event: any) => {
  const { oldIndex, newIndex } = event

  if (oldIndex === newIndex) return

  const allItems = [...currentCategoryItems.value, ...currentCategoryBundles.value]
  const [moved] = allItems.splice(oldIndex, 1)
  allItems.splice(newIndex, 0, moved)

  const reorderDto = {
    items: allItems.map((item, index) => ({
      id: item.id,
      categoryId: selectedCategoryId.value,
      sortOrder: index
    }))
  }

  hasChanges.value = true

  try {
    await catalogsApi.reorderItems(catalogId.value, reorderDto)
    await loadCatalog()
  } catch (error) {
    console.error('Failed to reorder items', error)
    await loadCatalog()
  }
}

const addItemsToMenu = async (libraryItemIds: number[], categoryIds: number[]) => {
  try {
    for (const itemId of libraryItemIds) {
      await catalogsApi.addItemToCatalog(catalogId.value, {
        libraryItemId: itemId,
        categoryIds: categoryIds,
        sortOrder: catalog.value.items.length
      })
    }
    await loadCatalog()
    showItemSelector.value = false
  } catch (error) {
    console.error('Failed to add items to menu', error)
  }
}

const addBundleToMenu = async (bundleId: number, categoryIds: number[]) => {
  try {
    await catalogsApi.addBundleToCatalog(catalogId.value, {
      bundleId: bundleId,
      categoryIds: categoryIds,
      sortOrder: catalog.value.bundles.length
    })
    await loadCatalog()
    showBundleSelector.value = false
  } catch (error) {
    console.error('Failed to add bundle to menu', error)
  }
}

const removeItem = async (itemId: number) => {
  if (!confirm('Are you sure you want to remove this item from the menu?')) return

  try {
    await catalogsApi.removeItem(catalogId.value, itemId)
    await loadCatalog()
  } catch (error) {
    console.error('Failed to remove item', error)
  }
}

const removeBundle = async (bundleId: number) => {
  if (!confirm('Are you sure you want to remove this bundle from the menu?')) return

  try {
    await catalogsApi.removeBundle(catalogId.value, bundleId)
    await loadCatalog()
  } catch (error) {
    console.error('Failed to remove bundle', error)
  }
}

const editItem = (item: any) => {
  // TODO: Open item edit modal
  console.log('Edit item', item)
}

const editBundle = (bundle: any) => {
  // TODO: Open bundle edit modal
  console.log('Edit bundle', bundle)
}

const handleSave = async () => {
  isSaving.value = true
  try {
    // All changes are saved optimistically already
    hasChanges.value = false
    router.push('/dashboard/menus')
  } catch (error) {
    console.error('Failed to save changes', error)
  } finally {
    isSaving.value = false
  }
}

const handleCancel = () => {
  if (hasChanges.value && !confirm('You have unsaved changes. Are you sure you want to leave?')) {
    return
  }
  router.push('/dashboard/menus')
}

// Lifecycle
onMounted(() => {
  loadCatalog()
})
</script>

<style scoped>
.menu-editor-container {
  @apply min-h-screen bg-gray-50 p-6;
}

.sortable-ghost {
  @apply opacity-40 bg-blue-100;
}

.sortable-drag {
  @apply opacity-100;
}

.sortable-chosen {
  @apply cursor-grabbing;
}

.drag-handle:active {
  @apply cursor-grabbing;
}
</style>

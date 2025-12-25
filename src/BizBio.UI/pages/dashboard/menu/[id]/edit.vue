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
                    <div v-if="category.icon" class="text-2xl">{{ category.icon }}</div>
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
              <!-- Header with controls -->
              <div class="flex justify-between items-center mb-6">
                <h2 class="text-lg font-semibold text-gray-900">
                  {{ selectedCategory ? selectedCategory.name : 'All Items' }}
                </h2>
                <div class="flex gap-3 items-center">
                  <!-- View Toggle -->
                  <div class="flex border border-gray-300 rounded-lg overflow-hidden">
                    <button
                      @click="viewMode = 'table'"
                      :class="[
                        'px-3 py-1.5 text-sm transition-colors',
                        viewMode === 'table' ? 'bg-primary text-white' : 'bg-white text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M3 14h18m-9-4v8m-7 0h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                      </svg>
                    </button>
                    <button
                      @click="viewMode = 'grid'"
                      :class="[
                        'px-3 py-1.5 text-sm transition-colors',
                        viewMode === 'grid' ? 'bg-primary text-white' : 'bg-white text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
                      </svg>
                    </button>
                  </div>

                  <!-- Thumbnail Size Slider (only for grid view) -->
                  <div v-if="viewMode === 'grid'" class="flex items-center gap-2 px-3 py-1.5 border border-gray-300 rounded-lg">
                    <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    <input
                      v-model="thumbnailSize"
                      type="range"
                      min="100"
                      max="300"
                      step="50"
                      class="w-24"
                    />
                    <span class="text-xs text-gray-600">{{ thumbnailSize }}px</span>
                  </div>

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

              <!-- Table View -->
              <div v-if="viewMode === 'table' && (currentCategoryItems.length > 0 || currentCategoryBundles.length > 0)" class="overflow-x-auto">
                <table class="w-full min-w-[800px]">
                  <thead>
                    <tr class="border-b border-gray-200">
                      <th class="text-left py-3 px-4 font-semibold text-gray-700 w-8"></th>
                      <th class="text-left py-3 px-4 font-semibold text-gray-700">Image</th>
                      <th class="text-left py-3 px-4 font-semibold text-gray-700">Name</th>
                      <th class="text-left py-3 px-4 font-semibold text-gray-700">Description</th>
                      <th class="text-left py-3 px-4 font-semibold text-gray-700">Price</th>
                      <th class="text-left py-3 px-4 font-semibold text-gray-700">Info</th>
                      <th class="text-right py-3 px-4 font-semibold text-gray-700">Actions</th>
                    </tr>
                  </thead>
                  <tbody ref="itemsTable" class="divide-y divide-gray-100">
                    <!-- Items -->
                    <tr
                      v-for="item in currentCategoryItems"
                      :key="'item-' + item.id"
                      :data-id="item.id"
                      :data-type="'item'"
                      class="hover:bg-gray-50 transition-colors group"
                    >
                      <td class="py-3 px-4">
                        <div class="drag-handle cursor-grab active:cursor-grabbing text-gray-400 hover:text-gray-600">
                          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16" />
                          </svg>
                        </div>
                      </td>
                      <td class="py-3 px-4">
                        <div class="w-16 h-16 rounded-lg overflow-hidden bg-gray-100 flex items-center justify-center">
                          <img v-if="item.images && item.images.length > 0" :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
                          <svg v-else class="w-8 h-8 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                          </svg>
                        </div>
                      </td>
                      <td class="py-3 px-4">
                        <p class="font-medium text-gray-900">{{ item.name }}</p>
                      </td>
                      <td class="py-3 px-4 max-w-xs">
                        <p class="text-sm text-gray-600 truncate">{{ item.description }}</p>
                      </td>
                      <td class="py-3 px-4">
                        <span class="font-semibold text-primary">R{{ item.price.toFixed(2) }}</span>
                      </td>
                      <td class="py-3 px-4">
                        <div class="flex gap-1">
                          <span v-if="item.hasOptions" class="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">Opts</span>
                          <span v-if="item.hasExtras" class="px-2 py-1 bg-green-100 text-green-800 text-xs rounded">Extras</span>
                          <span v-if="item.variantCount > 0" class="px-2 py-1 bg-purple-100 text-purple-800 text-xs rounded">{{ item.variantCount }}v</span>
                        </div>
                      </td>
                      <td class="py-3 px-4">
                        <div class="flex justify-end gap-2">
                          <button
                            @click.stop="editItem(item)"
                            class="px-3 py-1 text-sm bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors"
                          >
                            Edit
                          </button>
                          <button
                            @click.stop="removeItem(item.id)"
                            class="px-3 py-1 text-sm bg-red-50 text-red-600 rounded hover:bg-red-100 transition-colors"
                          >
                            Remove
                          </button>
                        </div>
                      </td>
                    </tr>

                    <!-- Bundles -->
                    <tr
                      v-for="bundle in currentCategoryBundles"
                      :key="'bundle-' + bundle.id"
                      :data-id="bundle.id"
                      :data-type="'bundle'"
                      class="hover:bg-gray-50 transition-colors group bg-orange-50/30"
                    >
                      <td class="py-3 px-4">
                        <div class="drag-handle cursor-grab active:cursor-grabbing text-gray-400 hover:text-gray-600">
                          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16" />
                          </svg>
                        </div>
                      </td>
                      <td class="py-3 px-4">
                        <div class="w-16 h-16 rounded-lg overflow-hidden bg-gray-100 flex items-center justify-center">
                          <img v-if="bundle.images && bundle.images.length > 0" :src="bundle.images[0]" :alt="bundle.name" class="w-full h-full object-cover" />
                          <svg v-else class="w-8 h-8 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                          </svg>
                        </div>
                      </td>
                      <td class="py-3 px-4">
                        <div class="flex items-center gap-2">
                          <span class="px-2 py-1 bg-orange-100 text-orange-800 text-xs font-semibold rounded">BUNDLE</span>
                          <p class="font-medium text-gray-900">{{ bundle.name }}</p>
                        </div>
                      </td>
                      <td class="py-3 px-4 max-w-xs">
                        <p class="text-sm text-gray-600 truncate">{{ bundle.description }}</p>
                      </td>
                      <td class="py-3 px-4">
                        <span class="font-semibold text-primary">R{{ bundle.basePrice.toFixed(2) }}</span>
                      </td>
                      <td class="py-3 px-4"></td>
                      <td class="py-3 px-4">
                        <div class="flex justify-end gap-2">
                          <button
                            @click.stop="editBundle(bundle)"
                            class="px-3 py-1 text-sm bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors"
                          >
                            Edit
                          </button>
                          <button
                            @click.stop="removeBundle(bundle.id)"
                            class="px-3 py-1 text-sm bg-red-50 text-red-600 rounded hover:bg-red-100 transition-colors"
                          >
                            Remove
                          </button>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- Grid View -->
              <div
                v-else-if="viewMode === 'grid' && (currentCategoryItems.length > 0 || currentCategoryBundles.length > 0)"
                ref="itemsGrid"
                class="items-grid grid gap-4"
                :style="{ gridTemplateColumns: `repeat(auto-fill, minmax(${thumbnailSize}px, 1fr))` }"
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

      <!-- Confirmation Dialog -->
      <div v-if="confirmDialog.show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="confirmDialog.show = false">
        <div class="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
          <div class="p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ confirmDialog.title }}</h3>
            <p class="text-gray-600 mb-6">{{ confirmDialog.message }}</p>
            <div class="flex justify-end gap-3">
              <button
                @click="confirmDialog.show = false"
                class="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
              >
                Cancel
              </button>
              <button
                @click="confirmDialog.onConfirm"
                class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
              >
                Remove
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCatalogsApi } from '~/composables/useApi'
import { useDragDrop } from '~/composables/useDragDrop'
import ItemSelectorModal from '~/components/menu-editor/ItemSelectorModal.vue'
import BundleSelectorModal from '~/components/menu-editor/BundleSelectorModal.vue'

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
const confirmDialog = ref<{
  show: boolean
  title: string
  message: string
  onConfirm: () => void
}>({
  show: false,
  title: '',
  message: '',
  onConfirm: () => {}
})

// View state
const viewMode = ref<'table' | 'grid'>('table')
const thumbnailSize = ref(150) // px

// Refs for sortable
const categoryList = ref<HTMLElement | null>(null)
const itemsGrid = ref<HTMLElement | null>(null)
const itemsTable = ref<HTMLElement | null>(null)

// Sortable instances
let categorySortable: any = null
let itemsSortable: any = null

// Computed
const selectedCategory = computed(() => {
  return categories.value.find(c => c.id === selectedCategoryId.value)
})

const currentCategoryItems = computed(() => {
  if (!catalog.value || !selectedCategoryId.value) {
    console.log('No catalog or selected category')
    return []
  }

  console.log('Filtering items for category:', selectedCategoryId.value)
  console.log('All items:', catalog.value.items)
  console.log('First item structure:', catalog.value.items[0])

  const filtered = catalog.value.items.filter((item: any) => {
    console.log(`Item ${item.id} (${item.name}) categoryIds:`, item.categoryIds)
    return item.categoryIds && item.categoryIds.includes(selectedCategoryId.value)
  })

  console.log('Filtered items:', filtered)
  return filtered
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
    console.log('Loading catalog ID:', catalogId.value)

    const response = await catalogsApi.getCatalogDetail(catalogId.value)

    console.log('API Response:', response)
    console.log('Response success:', response?.success)
    console.log('Response data:', response?.data)
    console.log('Categories:', response?.data?.categories)

    if (response.success && response.data) {
      catalog.value = response.data
      categories.value = response.data.categories

      console.log('Catalog set:', catalog.value)
      console.log('Categories set:', categories.value)
      console.log('Loading state:', loading.value)

      if (categories.value.length > 0 && !selectedCategoryId.value) {
        selectedCategoryId.value = categories.value[0].id
        console.log('Selected category ID:', selectedCategoryId.value)
      }

      // Enable drag-drop after DOM updates
      nextTick(() => {
        console.log('Enabling drag-drop')
        enableDragDrop()
      })
    } else {
      console.error('Invalid response format:', response)
    }
  } catch (error) {
    console.error('Failed to load catalog', error)
    router.push('/dashboard/menu')
  } finally {
    loading.value = false
    console.log('Loading complete. Catalog:', catalog.value ? 'SET' : 'NULL')
  }
}

// Enable drag-and-drop
const enableDragDrop = () => {
  const { destroySortable } = useDragDrop()

  // Destroy existing instances
  if (categorySortable) {
    destroySortable(categorySortable)
    categorySortable = null
  }
  if (itemsSortable) {
    destroySortable(itemsSortable)
    itemsSortable = null
  }

  // Categories drag-and-drop
  if (categoryList.value) {
    categorySortable = enableSortable(categoryList.value, {
      onUpdate: handleCategoryReorder
    })
  }

  // Items drag-and-drop - for grid view or table view
  const itemsElement = viewMode.value === 'table' ? itemsTable.value : itemsGrid.value
  if (itemsElement) {
    itemsSortable = enableSortable(itemsElement, {
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
    // Don't reload catalog - just re-enable drag-drop
    nextTick(() => {
      enableDragDrop()
    })
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

const removeItem = (itemId: number) => {
  const item = catalog.value.items.find((i: any) => i.id === itemId)
  confirmDialog.value = {
    show: true,
    title: 'Remove Item',
    message: `Are you sure you want to remove "${item?.name}" from this menu?`,
    onConfirm: async () => {
      try {
        await catalogsApi.removeItem(catalogId.value, itemId)
        await loadCatalog()
        confirmDialog.value.show = false
      } catch (error) {
        console.error('Failed to remove item', error)
        confirmDialog.value.show = false
      }
    }
  }
}

const removeBundle = (bundleId: number) => {
  const bundle = catalog.value.bundles.find((b: any) => b.id === bundleId)
  confirmDialog.value = {
    show: true,
    title: 'Remove Bundle',
    message: `Are you sure you want to remove "${bundle?.name}" from this menu?`,
    onConfirm: async () => {
      try {
        await catalogsApi.removeBundle(catalogId.value, bundleId)
        await loadCatalog()
        confirmDialog.value.show = false
      } catch (error) {
        console.error('Failed to remove bundle', error)
        confirmDialog.value.show = false
      }
    }
  }
}

const editItem = (item: any) => {
  // Navigate to library items page with the item ID
  router.push(`/dashboard/library/items/${item.id}`)
}

const editBundle = (bundle: any) => {
  // Navigate to bundles page with the bundle ID
  router.push(`/dashboard/bundles/${bundle.id}`)
}

const handleSave = async () => {
  isSaving.value = true
  try {
    // All changes are saved optimistically already
    hasChanges.value = false
    router.push('/dashboard/menu')
  } catch (error) {
    console.error('Failed to save changes', error)
  } finally {
    isSaving.value = false
  }
}

const handleCancel = () => {
  if (hasChanges.value) {
    confirmDialog.value = {
      show: true,
      title: 'Unsaved Changes',
      message: 'You have unsaved changes. Are you sure you want to leave?',
      onConfirm: () => {
        router.push('/dashboard/menu')
      }
    }
  } else {
    router.push('/dashboard/menu')
  }
}

// Watchers
watch(viewMode, () => {
  nextTick(() => {
    enableDragDrop()
  })
})

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

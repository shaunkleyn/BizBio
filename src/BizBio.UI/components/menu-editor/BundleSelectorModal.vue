<template>
  <div class="modal-overlay fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="$emit('close')">
    <div class="modal-content bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[80vh] flex flex-col">
      <!-- Header -->
      <div class="modal-header p-6 border-b border-gray-200">
        <div class="flex justify-between items-center">
          <h2 class="text-2xl font-bold text-gray-900">Add Bundle to Menu</h2>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Search -->
        <div class="mt-4">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search bundles..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
          />
        </div>
      </div>

      <!-- Body -->
      <div class="modal-body p-6 overflow-y-auto flex-1">
        <div v-if="loading" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>

        <div v-else-if="filteredBundles.length === 0" class="text-center py-12">
          <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
          <p class="text-gray-600 font-medium">No bundles found</p>
          <p class="text-gray-500 text-sm mt-1">Create bundles in your library first</p>
        </div>

        <div v-else class="grid grid-cols-2 gap-4">
          <div
            v-for="bundle in filteredBundles"
            :key="bundle.id"
            class="bundle-card border border-gray-200 rounded-lg overflow-hidden hover:shadow-md transition-all cursor-pointer"
            :class="{ 'ring-2 ring-primary': selectedBundle === bundle.id }"
            @click="selectedBundle = bundle.id"
          >
            <div v-if="bundle.images && bundle.images.length > 0" class="aspect-video bg-gray-100">
              <img :src="bundle.images[0]" :alt="bundle.name" class="w-full h-full object-cover" />
            </div>
            <div v-else class="aspect-video bg-gray-100 flex items-center justify-center">
              <svg class="w-16 h-16 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
              </svg>
            </div>

            <div class="p-4">
              <div class="flex items-start justify-between mb-2">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <span class="px-2 py-1 bg-orange-100 text-orange-800 text-xs font-semibold rounded">BUNDLE</span>
                  </div>
                  <h3 class="font-semibold text-gray-900">{{ bundle.name }}</h3>
                </div>
                <input
                  type="radio"
                  name="bundle-select"
                  :checked="selectedBundle === bundle.id"
                  class="mt-1 h-5 w-5 text-primary border-gray-300 focus:ring-primary"
                  @click.stop="selectedBundle = bundle.id"
                />
              </div>
              <p class="text-sm text-gray-500 line-clamp-2">{{ bundle.description }}</p>
              <div class="mt-2">
                <span class="text-lg font-bold text-primary">R{{ bundle.basePrice?.toFixed(2) }}</span>
              </div>
              <div v-if="bundle.sections && bundle.sections.length > 0" class="mt-2">
                <p class="text-xs text-gray-500">{{ bundle.sections.length }} section{{ bundle.sections.length !== 1 ? 's' : '' }}</p>
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
            {{ selectedBundle ? '1 bundle selected' : 'No bundle selected' }}
          </p>
          <div class="flex gap-3">
            <button
              @click="$emit('close')"
              class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="handleAddBundle"
              :disabled="!selectedBundle || selectedCategories.length === 0"
              class="px-6 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              Add Bundle
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useBundlesApi, useLibraryCategoriesApi } from '~/composables/useApi'

const props = defineProps<{
  catalogId: number
}>()

const emit = defineEmits<{
  (e: 'select', bundleId: number, categoryIds: number[]): void
  (e: 'close'): void
}>()

const bundlesApi = useBundlesApi()
const libraryCategoriesApi = useLibraryCategoriesApi()

// State
const loading = ref(true)
const bundles = ref<any[]>([])
const catalogCategories = ref<any[]>([])
const searchQuery = ref('')
const selectedBundle = ref<number | null>(null)
const selectedCategories = ref<number[]>([])

// Computed
const filteredBundles = computed(() => {
  let filtered = bundles.value

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(bundle =>
      bundle.name.toLowerCase().includes(query) ||
      bundle.description?.toLowerCase().includes(query)
    )
  }

  return filtered
})

// Methods
const loadData = async () => {
  try {
    loading.value = true
    const [bundlesResponse, categoriesResponse] = await Promise.all([
      bundlesApi.getBundles(props.catalogId.toString()),
      libraryCategoriesApi.getCategories()
    ])

    if (bundlesResponse.success) {
      bundles.value = bundlesResponse.data
    }

    if (categoriesResponse.success) {
      catalogCategories.value = categoriesResponse.data
    }
  } catch (error) {
    console.error('Failed to load bundles', error)
  } finally {
    loading.value = false
  }
}

const handleAddBundle = () => {
  if (!selectedBundle.value || selectedCategories.value.length === 0) {
    return
  }
  emit('select', selectedBundle.value, selectedCategories.value)
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

<template>
  <div class="modal-overlay fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4 animate-fadeSlide" @click.self="$emit('close')">
    <div class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-4xl w-full max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant">
      <!-- Header -->
      <div class="modal-header p-6 border-b border-md-outline-variant relative overflow-hidden">
        <div class="absolute inset-0 bg-gradient-secondary opacity-5"></div>
        <div class="flex justify-between items-center relative z-10">
          <div>
            <h2 class="text-2xl font-heading font-bold gradient-text">Add Bundle to Menu</h2>
            <p class="text-sm text-md-on-surface-variant mt-1">Select a bundle to add to your menu category</p>
          </div>
          <button 
            @click="$emit('close')" 
            class="w-10 h-10 rounded-full bg-md-error-container text-md-on-error-container hover:bg-md-error hover:text-md-on-error transition-all md-ripple shadow-md-1"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Search -->
        <div class="mt-4 relative z-10">
          <div class="relative">
            <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-md-on-surface-variant"></i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search bundles..."
              class="w-full pl-11 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-secondary focus:border-md-secondary transition-all"
            />
          </div>
        </div>
      </div>

      <!-- Body -->
      <div class="modal-body p-6 overflow-y-auto flex-1">
        <div v-if="loading" class="flex justify-center items-center py-12">
          <div class="relative">
            <div class="animate-spin rounded-full h-12 w-12 border-4 border-md-secondary border-t-transparent"></div>
            <div class="absolute inset-0 rounded-full bg-gradient-secondary opacity-20 blur-xl"></div>
          </div>
        </div>

        <div v-else-if="filteredBundles.length === 0" class="text-center py-12">
          <div class="w-20 h-20 mx-auto mb-4 rounded-2xl bg-gradient-secondary flex items-center justify-center shadow-glow-pink">
            <i class="fas fa-box-open text-4xl text-white"></i>
          </div>
          <p class="text-md-on-surface font-bold mb-1">No bundles found</p>
          <p class="text-md-on-surface-variant text-sm">Create bundles in your library first</p>
        </div>

        <div v-else class="grid grid-cols-2 gap-4">
          <div
            v-for="bundle in filteredBundles"
            :key="bundle.id"
            class="bundle-card border rounded-2xl overflow-hidden hover:shadow-md-3 transition-all cursor-pointer group"
            :class="selectedBundle === bundle.id 
              ? 'ring-2 ring-md-secondary border-md-secondary shadow-glow-pink' 
              : 'border-md-outline-variant hover:border-md-secondary'"
            @click="selectedBundle = bundle.id"
          >
            <div v-if="bundle.images && bundle.images.length > 0" class="aspect-video bg-md-surface-container relative overflow-hidden">
              <img :src="bundle.images[0]" :alt="bundle.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
              <div v-if="selectedBundle === bundle.id" class="absolute top-2 right-2 w-8 h-8 bg-gradient-secondary rounded-full flex items-center justify-center shadow-glow-pink">
                <i class="fas fa-check text-white"></i>
              </div>
            </div>
            <div v-else class="aspect-video bg-md-surface-container flex items-center justify-center">
              <i class="fas fa-box-open text-6xl text-md-on-surface-variant opacity-30"></i>
            </div>

            <div class="p-4">
              <div class="flex items-start justify-between mb-2">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-2">
                    <span class="px-3 py-1 bg-gradient-to-r from-orange-500 to-amber-500 text-white text-xs font-bold rounded-lg shadow-md-1">
                      <i class="fas fa-layer-group mr-1"></i>BUNDLE
                    </span>
                  </div>
                  <h3 class="font-bold text-md-on-surface">{{ bundle.name }}</h3>
                </div>
                <input
                  type="radio"
                  name="bundle-select"
                  :checked="selectedBundle === bundle.id"
                  class="mt-1 h-5 w-5 accent-md-secondary"
                  @click.stop="selectedBundle = bundle.id"
                />
              </div>
              <p class="text-sm text-md-on-surface-variant line-clamp-2 mb-3">{{ bundle.description }}</p>
              <div class="flex items-center justify-between">
                <span class="text-lg font-bold gradient-text">R{{ bundle.basePrice?.toFixed(2) }}</span>
                <div v-if="bundle.sections && bundle.sections.length > 0" class="px-2 py-1 bg-md-tertiary-container text-md-on-tertiary-container text-xs font-bold rounded-lg">
                  {{ bundle.sections.length }} section{{ bundle.sections.length !== 1 ? 's' : '' }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="modal-footer p-6 border-t border-md-outline-variant relative overflow-hidden">
        <div class="absolute inset-0 mesh-bg-3 opacity-30"></div>
        <div class="relative z-10 space-y-4">
          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-3 flex items-center gap-2">
              <i class="fas fa-layer-group text-md-secondary"></i>
              Assign to Categories
            </label>
            <div class="flex flex-wrap gap-2">
              <label
                v-for="category in catalogCategories"
                :key="category.id"
                class="flex items-center gap-2 px-4 py-2.5 border rounded-xl cursor-pointer transition-all md-ripple"
                :class="selectedCategories.includes(category.id) 
                  ? 'bg-gradient-secondary text-white border-transparent shadow-glow-pink' 
                  : 'bg-md-surface-container border-md-outline-variant hover:border-md-secondary hover:shadow-md-1'"
              >
                <input
                  type="checkbox"
                  :value="category.id"
                  v-model="selectedCategories"
                  class="w-4 h-4 rounded accent-md-secondary"
                />
                <span class="text-sm font-medium">{{ category.name }}</span>
              </label>
            </div>
          </div>

          <div class="flex justify-between items-center pt-2">
            <div class="flex items-center gap-2 px-4 py-2 bg-md-secondary-container rounded-xl">
              <i class="fas fa-check-circle text-md-secondary"></i>
              <p class="text-sm font-bold text-md-on-secondary-container">
                {{ selectedBundle ? '1 bundle selected' : 'No bundle selected' }}
              </p>
            </div>
            <div class="flex gap-3">
              <button
                @click="$emit('close')"
                class="px-6 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl text-md-on-surface-variant font-medium hover:bg-md-surface-container-high hover:shadow-md-1 transition-all md-ripple"
              >
                Cancel
              </button>
              <button
                @click="handleAddBundle"
                :disabled="!selectedBundle || selectedCategories.length === 0"
                class="px-6 py-3 bg-gradient-secondary rounded-xl font-bold shadow-md-2 hover:shadow-glow-pink disabled:opacity-50 disabled:cursor-not-allowed transition-all md-ripple flex items-center gap-2"
              >
                <i class="fas fa-plus-circle"></i>
                Add Bundle
              </button>
            </div>
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
  preSelectedCategory?: number | null
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

    // Auto-select pre-selected category if provided
    if (props.preSelectedCategory) {
      selectedCategories.value = [props.preSelectedCategory]
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

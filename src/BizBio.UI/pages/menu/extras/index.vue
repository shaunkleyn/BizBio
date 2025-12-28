<template>
  <div class="p-4 md:p-8">
    <div class="container mx-auto px-4 py-8">

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
      {{ error }}
    </div>

    <!-- Extras List -->
    <div v-else-if="extras.length > 0" class="mesh-card bg-md-surface rounded-2xl shadow-md-3 overflow-hidden">
      <table class="min-w-full">
        <thead class="bg-gradient-to-r from-md-primary/10 to-md-secondary/10 border-b-2 border-md-primary/20">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-bold text-md-on-surface uppercase tracking-wider">Name</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-md-on-surface uppercase tracking-wider">Code</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-md-on-surface uppercase tracking-wider">Base Price</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-md-on-surface uppercase tracking-wider">Description</th>
            <th class="px-6 py-3 text-right text-xs font-bold text-md-on-surface uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-md-outline-variant">
          <tr v-for="extra in extras" :key="extra.id" class="hover:bg-md-surface-container-low transition-all">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-semibold text-md-on-surface">{{ extra.name }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-md-on-surface-variant">{{ extra.code || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-bold text-md-primary">R{{ extra.basePrice.toFixed(2) }}</div>
            </td>
            <td class="px-6 py-4">
              <div class="text-sm text-md-on-surface-variant truncate max-w-md">{{ extra.description || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right">
              <div class="flex justify-end gap-2">
                <button
                  @click="editExtra(extra)"
                  class="p-2 text-md-primary hover:bg-md-primary-container rounded-xl transition-all shadow-md-1 hover:shadow-md-2"
                  title="Edit"
                >
                  <i class="fas fa-edit"></i>
                </button>
                <button
                  @click="deleteExtra(extra.id)"
                  class="p-2 text-md-error hover:bg-md-error-container rounded-xl transition-all shadow-md-1 hover:shadow-md-2"
                  title="Delete"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Empty State -->
    <div v-else class="bg-md-surface rounded-2xl shadow-md p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-md-on-surface-variant opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-md-on-surface">No extras</h3>
      <p class="mt-1 text-sm text-md-on-surface-variant">Get started by creating a new extra.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Add Extra
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || editingExtra"
      class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
      @click.self="closeModal"
    >
      <div class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-2xl w-full max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant">
        <!-- Header with Gradient -->
        <div class="modal-header p-6">
          <div class="flex justify-between items-center relative z-10">
            <div>
              <h2 class="text-2xl font-heading font-bold gradient-text">
                {{ editingExtra ? 'Edit Extra' : 'Create Extra' }}
              </h2>
              <p class="text-sm text-md-on-surface-variant mt-1">
                {{ editingExtra ? 'Update extra details' : 'Add a new extra option' }}
              </p>
            </div>
            <button
              @click="closeModal"
              class="modal-close-btn md-ripple shadow-md-1"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <!-- Form - Scrollable Body -->
        <form @submit.prevent="saveExtra" class="flex-1 overflow-y-auto p-6 space-y-6">
          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-tag text-md-primary"></i>
              Name <span class="text-md-error">*</span>
            </label>
            <input
              v-model="formData.name"
              type="text"
              required
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
              placeholder="e.g., Extra Cheese, Bacon"
            />
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-barcode text-md-primary"></i>
              Code/SKU
            </label>
            <input
              v-model="formData.code"
              type="text"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
              placeholder="e.g., EXT-CHEESE"
            />
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-dollar-sign text-md-primary"></i>
              Base Price <span class="text-md-error">*</span>
            </label>
            <div class="relative">
              <span class="absolute left-4 top-3.5 text-md-on-surface-variant">R</span>
              <input
                v-model.number="formData.basePrice"
                type="number"
                step="0.01"
                min="0"
                required
                class="w-full pl-10 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                placeholder="0.00"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-align-left text-md-primary"></i>
              Description
            </label>
            <textarea
              v-model="formData.description"
              rows="3"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
              placeholder="Optional description"
            ></textarea>
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-image text-md-primary"></i>
              Image URL
            </label>
            <input
              v-model="formData.imageUrl"
              type="url"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
              placeholder="https://example.com/image.jpg"
            />
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-sort-numeric-down text-md-primary"></i>
              Display Order
            </label>
            <input
              v-model.number="formData.displayOrder"
              type="number"
              min="0"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
              placeholder="0"
            />
          </div>
        </form>

        <!-- Footer with Gradient Background -->
        <div class="modal-footer p-6">
          <div class="relative z-10 flex gap-3">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-6 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all shadow-md-1 md-ripple font-medium"
            >
              Cancel
            </button>
            <button
              @click="saveExtra"
              :disabled="saving"
              class="flex-1 px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-glow-purple transition-all font-bold disabled:opacity-50 disabled:cursor-not-allowed md-ripple flex items-center justify-center gap-2"
            >
              <i v-if="saving" class="fas fa-spinner fa-spin"></i>
              <i v-else class="fas fa-save"></i>
              <span>{{ saving ? 'Saving...' : editingExtra ? 'Update' : 'Create' }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { h } from 'vue'

definePageMeta({
  layout: 'menu'
})

const api = useApi()
const loading = ref(true)
const error = ref('')
const extras = ref<any[]>([])
const showCreateModal = ref(false)
const editingExtra = ref<any>(null)
const saving = ref(false)

const formData = ref({
  name: '',
  code: '',
  basePrice: 0,
  description: '',
  imageUrl: '',
  displayOrder: 0
})

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActions } = usePageMeta()

onMounted(() => {
  // Set page metadata
  setPageHeader({
    title: 'Extras Library',
    description: 'Manage reusable extras/modifiers for your menu items'
  })

  setPageActions(() => h('button', {
    onClick: () => showCreateModal.value = true,
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-bold flex items-center gap-2'
  }, [
    h('i', { class: 'fas fa-plus' }),
    'Add Extra'
  ]))

  fetchExtras()
  loadStats()
})

const fetchExtras = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await api.get('/library/extras')
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        extras.value = response.data
      } else if (response.data && Array.isArray(response.data.extras)) {
        extras.value = response.data.extras
      } else {
        console.warn('Unexpected response structure:', response.data)
        extras.value = []
      }
    } else {
      error.value = 'Failed to load extras'
      extras.value = []
    }
  } catch (err: any) {
    error.value = err.message || 'An error occurred'
    extras.value = []
  } finally {
    loading.value = false
  }
}

const loadStats = async () => {
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

const saveExtra = async () => {
  saving.value = true
  try {
    if (editingExtra.value) {
      const response = await api.put(`/library/extras/${editingExtra.value.id}`, formData.value)
      if (response.success) {
        await fetchExtras()
        closeModal()
      } else {
        alert('Failed to update extra')
      }
    } else {
      const response = await api.post('/library/extras', formData.value)
      if (response.success) {
        await fetchExtras()
        closeModal()
      } else {
        alert('Failed to create extra')
      }
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  } finally {
    saving.value = false
  }
}

const editExtra = (extra: any) => {
  editingExtra.value = extra
  formData.value = {
    name: extra.name,
    code: extra.code || '',
    basePrice: extra.basePrice,
    description: extra.description || '',
    imageUrl: extra.imageUrl || '',
    displayOrder: extra.displayOrder
  }
}

const deleteExtra = async (id: number) => {
  if (!confirm('Are you sure you want to delete this extra?')) return

  try {
    const response = await api.delete(`/library/extras/${id}`)
    if (response.success) {
      await fetchExtras()
    } else {
      alert('Failed to delete extra')
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingExtra.value = null
  formData.value = {
    name: '',
    code: '',
    basePrice: 0,
    description: '',
    imageUrl: '',
    displayOrder: 0
  }
}
</script>







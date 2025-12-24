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

    <!-- Options List -->
    <div v-else-if="options.length > 0" class="bg-white rounded-lg shadow-md overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price Modifier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Display Order</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="option in options" :key="option.id" class="hover:bg-gray-50 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div v-if="option.imageUrl" class="flex-shrink-0 h-10 w-10 mr-3">
                  <img :src="option.imageUrl" :alt="option.name" class="h-10 w-10 rounded-full object-cover" />
                </div>
                <div class="text-sm font-medium text-gray-900">{{ option.name }}</div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-semibold" :class="{
                'text-green-600': option.priceModifier > 0,
                'text-red-600': option.priceModifier < 0,
                'text-gray-500': option.priceModifier === 0
              }">
                {{ option.priceModifier >= 0 ? '+' : '' }}R{{ option.priceModifier.toFixed(2) }}
              </div>
            </td>
            <td class="px-6 py-4">
              <div class="text-sm text-gray-500 truncate max-w-md">{{ option.description || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ option.displayOrder }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button
                @click="editOption(option)"
                class="text-blue-600 hover:text-blue-900 mr-4"
              >
                Edit
              </button>
              <button
                @click="deleteOption(option.id)"
                class="text-red-600 hover:text-red-900"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Empty State -->
    <div v-else class="bg-white rounded-lg shadow-md p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">No options</h3>
      <p class="mt-1 text-sm text-gray-500">Get started by creating a new product option.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Add Option
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || editingOption"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-gray-800">
              {{ editingOption ? 'Edit Option' : 'Create Option' }}
            </h2>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="saveOption" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Name *</label>
              <input
                v-model="formData.name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="e.g., Small, Medium, Large, No Onions"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Price Modifier *
                <span class="text-xs text-gray-500 ml-2">(Use negative values for price reductions)</span>
              </label>
              <input
                v-model.number="formData.priceModifier"
                type="number"
                step="0.01"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="0.00"
              />
              <p class="mt-1 text-xs text-gray-500">
                Positive values add to the price, negative values subtract. Use 0 for no change.
              </p>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
              <textarea
                v-model="formData.description"
                rows="3"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Optional description"
              ></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Image URL</label>
              <input
                v-model="formData.imageUrl"
                type="url"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="https://example.com/image.jpg"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Display Order</label>
              <input
                v-model.number="formData.displayOrder"
                type="number"
                min="0"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="0"
              />
            </div>

            <div class="flex justify-end gap-3 pt-4">
              <button
                type="button"
                @click="closeModal"
                class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ saving ? 'Saving...' : editingOption ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { h } from 'vue'
import { useOptionsApi } from '~/composables/useApi'

definePageMeta({
  layout: 'menu'
})

const optionsApi = useOptionsApi()
const loading = ref(true)
const error = ref('')
const options = ref<any[]>([])
const showCreateModal = ref(false)
const editingOption = ref<any>(null)
const saving = ref(false)

const formData = ref({
  name: '',
  priceModifier: 0,
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
    title: 'Options Library',
    description: 'Manage product customization options (size, modifications, etc.)'
  })

  setPageActions(() => h('button', {
    onClick: () => showCreateModal.value = true,
    class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold'
  }, [
    h('i', { class: 'fas fa-plus mr-2' }),
    'Add Option'
  ]))

  fetchOptions()
  loadStats()
})

const fetchOptions = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await optionsApi.getOptions()
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        options.value = response.data
      } else if (response.data && Array.isArray(response.data.options)) {
        options.value = response.data.options
      } else {
        console.warn('Unexpected response structure:', response.data)
        options.value = []
      }
    } else {
      error.value = 'Failed to load options'
      options.value = []
    }
  } catch (err: any) {
    error.value = err.message || 'An error occurred'
    options.value = []
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

const saveOption = async () => {
  saving.value = true
  try {
    if (editingOption.value) {
      const response = await optionsApi.updateOption(editingOption.value.id, formData.value)
      if (response.success) {
        await fetchOptions()
        closeModal()
      } else {
        alert('Failed to update option')
      }
    } else {
      const response = await optionsApi.createOption(formData.value)
      if (response.success) {
        await fetchOptions()
        closeModal()
      } else {
        alert('Failed to create option')
      }
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  } finally {
    saving.value = false
  }
}

const editOption = (option: any) => {
  editingOption.value = option
  formData.value = {
    name: option.name,
    priceModifier: option.priceModifier,
    description: option.description || '',
    imageUrl: option.imageUrl || '',
    displayOrder: option.displayOrder
  }
}

const deleteOption = async (id: number) => {
  if (!confirm('Are you sure you want to delete this option?')) return

  try {
    const response = await optionsApi.deleteOption(id)
    if (response.success) {
      await fetchOptions()
    } else {
      alert('Failed to delete option')
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingOption.value = null
  formData.value = {
    name: '',
    priceModifier: 0,
    description: '',
    imageUrl: '',
    displayOrder: 0
  }
}
</script>

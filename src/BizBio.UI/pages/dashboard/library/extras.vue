<template>
  <div class="container mx-auto px-4 py-8">
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-800">Extras Library</h1>
        <p class="text-gray-600 mt-2">Manage reusable extras/modifiers for your menu items</p>
      </div>
      <button
        @click="showCreateModal = true"
        class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Add Extra
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
      {{ error }}
    </div>

    <!-- Extras List -->
    <div v-else-if="extras.length > 0" class="bg-white rounded-lg shadow-md overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Code</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Base Price</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="extra in extras" :key="extra.Id" class="hover:bg-gray-50 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ extra.Name }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ extra.Code || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">R{{ extra.BasePrice.toFixed(2) }}</div>
            </td>
            <td class="px-6 py-4">
              <div class="text-sm text-gray-500 truncate max-w-md">{{ extra.Description || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button
                @click="editExtra(extra)"
                class="text-blue-600 hover:text-blue-900 mr-4"
              >
                Edit
              </button>
              <button
                @click="deleteExtra(extra.Id)"
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
      <h3 class="mt-2 text-sm font-medium text-gray-900">No extras</h3>
      <p class="mt-1 text-sm text-gray-500">Get started by creating a new extra.</p>
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
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-gray-800">
              {{ editingExtra ? 'Edit Extra' : 'Create Extra' }}
            </h2>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="saveExtra" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Name *</label>
              <input
                v-model="formData.Name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="e.g., Extra Cheese, Bacon"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Code/SKU</label>
              <input
                v-model="formData.Code"
                type="text"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="e.g., EXT-CHEESE"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Base Price *</label>
              <input
                v-model.number="formData.BasePrice"
                type="number"
                step="0.01"
                min="0"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="0.00"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
              <textarea
                v-model="formData.Description"
                rows="3"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Optional description"
              ></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Image URL</label>
              <input
                v-model="formData.ImageUrl"
                type="url"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="https://example.com/image.jpg"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Display Order</label>
              <input
                v-model.number="formData.DisplayOrder"
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
                {{ saving ? 'Saving...' : editingExtra ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'dashboard'
})

const api = useApi()
const loading = ref(true)
const error = ref('')
const extras = ref<any[]>([])
const showCreateModal = ref(false)
const editingExtra = ref<any>(null)
const saving = ref(false)

const formData = ref({
  Name: '',
  Code: '',
  BasePrice: 0,
  Description: '',
  ImageUrl: '',
  DisplayOrder: 0
})

const fetchExtras = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await api.get('/library/extras')
    if (response.success) {
      extras.value = response.data
    } else {
      error.value = 'Failed to load extras'
    }
  } catch (err: any) {
    error.value = err.message || 'An error occurred'
  } finally {
    loading.value = false
  }
}

const saveExtra = async () => {
  saving.value = true
  try {
    if (editingExtra.value) {
      const response = await api.put(`/library/extras/${editingExtra.value.Id}`, formData.value)
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
    Name: extra.Name,
    Code: extra.Code || '',
    BasePrice: extra.BasePrice,
    Description: extra.Description || '',
    ImageUrl: extra.ImageUrl || '',
    DisplayOrder: extra.DisplayOrder
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
    Name: '',
    Code: '',
    BasePrice: 0,
    Description: '',
    ImageUrl: '',
    DisplayOrder: 0
  }
}

onMounted(() => {
  fetchExtras()
})
</script>

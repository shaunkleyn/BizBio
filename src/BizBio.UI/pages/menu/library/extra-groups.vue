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

    <!-- Extra Groups List -->
    <div v-else-if="extraGroups.length > 0" class="grid gap-6">
      <div
        v-for="group in extraGroups"
        :key="group.id"
        class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
      >
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-xl font-bold text-gray-800">{{ group.name }}</h3>
            <p v-if="group.Description" class="text-gray-600 mt-1">{{ group.description }}</p>
            <div class="flex gap-4 mt-2 text-sm text-gray-500">
              <span>Min: {{ group.minRequired }}</span>
              <span>Max: {{ group.maxAllowed || 'Unlimited' }}</span>
              <span v-if="group.allowMultipleQuantities" class="text-green-600">Multiple Quantities</span>
            </div>
          </div>
          <div class="flex gap-2">
            <button
              @click="editGroup(group)"
              class="text-blue-600 hover:text-blue-900 px-3 py-1"
            >
              Edit
            </button>
            <button
              @click="deleteGroup(group.id)"
              class="text-red-600 hover:text-red-900 px-3 py-1"
            >
              Delete
            </button>
          </div>
        </div>

        <!-- Extras in Group -->
        <div v-if="group.extras && group.extras.length > 0" class="mt-4">
          <h4 class="text-sm font-semibold text-gray-700 mb-2">Extras in this group:</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
            <div
              v-for="item in group.extras"
              :key="item.id"
              class="flex justify-between items-center bg-gray-50 rounded-lg p-3"
            >
              <div>
                <span class="text-sm font-medium text-gray-800">{{ item.extra.name }}</span>
                <span class="text-xs text-gray-500 ml-2">
                  R{{ (item.priceOverride !== null ? item.priceOverride : item.extra.basePrice).toFixed(2) }}
                </span>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="mt-4 text-sm text-gray-500 italic">
          No extras in this group yet
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="bg-white rounded-lg shadow-md p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">No extra groups</h3>
      <p class="mt-1 text-sm text-gray-500">Get started by creating a new extra group.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Add Extra Group
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || editingGroup"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg shadow-xl max-w-3xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-gray-800">
              {{ editingGroup ? 'Edit Extra Group' : 'Create Extra Group' }}
            </h2>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="saveGroup" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Name *</label>
              <input
                v-model="formData.Name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="e.g., Choose Toppings, Add Extra Cheese"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
              <textarea
                v-model="formData.Description"
                rows="2"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Optional description"
              ></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Minimum Required</label>
                <input
                  v-model.number="formData.MinRequired"
                  type="number"
                  min="0"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="0"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Maximum Allowed</label>
                <input
                  v-model.number="formData.MaxAllowed"
                  type="number"
                  min="0"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="0 = Unlimited"
                />
                <p class="text-xs text-gray-500 mt-1">0 = Unlimited</p>
              </div>
            </div>

            <div class="flex items-center">
              <input
                v-model="formData.AllowMultipleQuantities"
                type="checkbox"
                id="allowMultiple"
                class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
              />
              <label for="allowMultiple" class="ml-2 block text-sm text-gray-700">
                Allow multiple quantities of the same extra
              </label>
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

            <!-- Select Extras -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Select Extras</label>
              <div v-if="availableExtras.length > 0" class="border border-gray-300 rounded-lg p-4 max-h-60 overflow-y-auto">
                <div
                  v-for="extra in availableExtras"
                  :key="extra.id"
                  class="flex items-center py-2 hover:bg-gray-50 rounded px-2"
                >
                  <input
                    type="checkbox"
                    :id="`extra-${extra.id}`"
                    :value="extra.id"
                    v-model="formData.ExtraIds"
                    class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <label :for="`extra-${extra.id}`" class="ml-3 flex-1 flex justify-between items-center cursor-pointer">
                    <span class="text-sm text-gray-800">{{ extra.name }}</span>
                    <span class="text-sm text-gray-500">R{{ extra.basePrice.toFixed(2) }}</span>
                  </label>
                </div>
              </div>
              <p v-else class="text-sm text-gray-500 italic">
                No extras available. Create extras first in the Extras Library.
              </p>
            </div>

            <div class="flex justify-end gap-3 pt-4 border-t">
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
                {{ saving ? 'Saving...' : editingGroup ? 'Update' : 'Create' }}
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

definePageMeta({
  layout: 'menu'
})

const api = useApi()
const loading = ref(true)
const error = ref('')
const extraGroups = ref<any[]>([])
const availableExtras = ref<any[]>([])
const showCreateModal = ref(false)
const editingGroup = ref<any>(null)
const saving = ref(false)

const formData = ref({
  Name: '',
  Description: '',
  MinRequired: 0,
  MaxAllowed: 0,
  AllowMultipleQuantities: true,
  DisplayOrder: 0,
  ExtraIds: [] as number[]
})

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Provide page metadata
provide('pageHeader', {
  title: 'Extra Groups Library',
  description: 'Group related extras together (e.g., "Choose Toppings", "Add Extra Cheese")'
})

provide('pageActions', () => h('button', {
  onClick: () => showCreateModal.value = true,
  class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold'
}, [
  h('i', { class: 'fas fa-plus mr-2' }),
  'Add Extra Group'
]))

const fetchExtraGroups = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await api.get('/library/extra-groups')
    console.log('Extra groups response:', response);
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        extraGroups.value = response.data
      } else if (response.data && Array.isArray(response.data.extraGroups)) {
        extraGroups.value = response.data.extraGroups
      } else {
        console.warn('Unexpected response structure:', response.data)
        extraGroups.value = []
      }
    } else {
      error.value = 'Failed to load extra groups'
      extraGroups.value = []
    }
  } catch (err: any) {
    error.value = err.message || 'An error occurred'
    extraGroups.value = []
  } finally {
    loading.value = false
  }
}

const fetchAvailableExtras = async () => {
  try {
    const response = await api.get('/library/extras')
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        availableExtras.value = response.data
      } else if (response.data && Array.isArray(response.data.extras)) {
        availableExtras.value = response.data.extras
      } else {
        availableExtras.value = []
      }
    }
  } catch (err) {
    console.error('Failed to fetch extras', err)
    availableExtras.value = []
  }
}

const saveGroup = async () => {
  saving.value = true
  try {
    if (editingGroup.value) {
      const response = await api.put(`/library/extra-groups/${editingGroup.value.id}`, formData.value)
      if (response.success) {
        await fetchExtraGroups()
        closeModal()
      } else {
        alert('Failed to update extra group')
      }
    } else {
      const response = await api.post('/library/extra-groups', formData.value)
      if (response.success) {
        await fetchExtraGroups()
        closeModal()
      } else {
        alert('Failed to create extra group')
      }
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  } finally {
    saving.value = false
  }
}

const editGroup = (group: any) => {
  editingGroup.value = group
  formData.value = {
    Name: group.name,
    Description: group.description || '',
    MinRequired: group.minRequired,
    MaxAllowed: group.maxAllowed,
    AllowMultipleQuantities: group.allowMultipleQuantities,
    DisplayOrder: group.displayOrder,
    ExtraIds: group.extras ? group.extras.map((item: any) => item.extraId) : []
  }
}

const deleteGroup = async (id: number) => {
  if (!confirm('Are you sure you want to delete this extra group?')) return

  try {
    const response = await api.delete(`/library/extra-groups/${id}`)
    if (response.success) {
      await fetchExtraGroups()
    } else {
      alert('Failed to delete extra group')
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingGroup.value = null
  formData.value = {
    Name: '',
    Description: '',
    MinRequired: 0,
    MaxAllowed: 0,
    AllowMultipleQuantities: true,
    DisplayOrder: 0,
    ExtraIds: []
  }
}

watch([showCreateModal, editingGroup], () => {
  if (showCreateModal.value || editingGroup.value) {
    fetchAvailableExtras()
  }
})

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

onMounted(() => {
  fetchExtraGroups()
  loadStats()
})
</script>

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

    <!-- Option Groups List -->
    <div v-else-if="optionGroups.length > 0" class="grid gap-6">
      <div
        v-for="group in optionGroups"
        :key="group.id"
        class="mesh-card bg-md-surface rounded-2xl shadow-md-2 p-6 hover:shadow-md-4 transition-all border border-md-outline-variant"
      >
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-xl font-bold gradient-text">{{ group.name }}</h3>
            <p v-if="group.description" class="text-md-on-surface-variant mt-1">{{ group.description }}</p>
            <div class="flex gap-4 mt-2 text-sm">
              <span class="px-3 py-1 bg-md-primary-container text-md-on-primary-container rounded-full font-medium">
                <i class="fas fa-arrow-down mr-1"></i>Min: {{ group.minRequired }}
              </span>
              <span class="px-3 py-1 bg-md-secondary-container text-md-on-secondary-container rounded-full font-medium">
                <i class="fas fa-arrow-up mr-1"></i>Max: {{ group.maxAllowed || 'Unlimited' }}
              </span>
              <span v-if="group.isRequired" class="px-3 py-1 bg-md-error-container text-md-on-error-container rounded-full font-medium">
                <i class="fas fa-exclamation-circle mr-1"></i>Required
              </span>
              <span v-else class="px-3 py-1 bg-md-surface-container text-md-on-surface-variant rounded-full font-medium">
                <i class="fas fa-info-circle mr-1"></i>Optional
              </span>
            </div>
          </div>
          <div class="flex gap-2">
            <button
              @click="editGroup(group)"
              class="p-2 text-md-primary hover:bg-md-primary-container rounded-xl transition-all shadow-md-1 hover:shadow-md-2"
              title="Edit"
            >
              <i class="fas fa-edit"></i>
            </button>
            <button
              @click="deleteGroup(group.id)"
              class="p-2 text-md-error hover:bg-md-error-container rounded-xl transition-all shadow-md-1 hover:shadow-md-2"
              title="Delete"
            >
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>

        <!-- Options in Group -->
        <div v-if="group.options && group.options.length > 0" class="mt-4">
          <h4 class="text-sm font-semibold text-md-on-surface mb-2">Options in this group:</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
            <div
              v-for="item in group.options"
              :key="item.id"
              class="flex justify-between items-center bg-md-surface-container rounded-lg p-3"
            >
              <div class="flex-1">
                <span class="text-sm font-semibold text-md-on-surface">{{ item.option.name }}</span>
                <span class="text-xs ml-2" :class="{
                  'text-green-600': item.option.priceModifier > 0,
                  'text-red-600': item.option.priceModifier < 0,
                  'text-gray-500': item.option.priceModifier === 0
                }">
                  {{ item.option.priceModifier >= 0 ? '+' : '' }}R{{ item.option.priceModifier.toFixed(2) }}
                </span>
              </div>
              <div v-if="item.isDefault" class="ml-2">
                <span class="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">Default</span>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="mt-4 text-sm text-gray-500 italic">
          No options in this group yet
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="bg-md-surface rounded-2xl shadow-md p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
      <h3 class="mt-2 text-sm font-semibold text-md-on-surface">No option groups</h3>
      <p class="mt-1 text-sm text-gray-500">Get started by creating a new option group.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Add Option Group
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || editingGroup"
      class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
      @click.self="closeModal"
    >
      <div class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-3xl w-full max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant">
        <div class="modal-header p-6">
          <div class="flex justify-between items-center relative z-10">
            <div>
              <h2 class="text-2xl font-heading font-bold gradient-text">
                {{ editingGroup ? 'Edit Option Group' : 'Create Option Group' }}
              </h2>
              <p class="text-sm text-md-on-surface-variant mt-1">
                {{ editingGroup ? 'Update group details' : 'Create a new option group' }}
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

        <form @submit.prevent="saveGroup" class="flex-1 overflow-y-auto p-6 space-y-6">
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Name *</label>
              <input
                v-model="formData.name"
                type="text"
                required
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                placeholder="e.g., Size, Remove Ingredients, Choose Your Base"
              />
            </div>

            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Description</label>
              <textarea
                v-model="formData.description"
                rows="2"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                placeholder="Optional description"
              ></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-semibold text-md-on-surface mb-2">Minimum Required *</label>
                <input
                  v-model.number="formData.minRequired"
                  type="number"
                  min="0"
                  required
                  class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                  placeholder="1"
                />
                <p class="text-xs text-gray-500 mt-1">1 = Customer must select at least one</p>
              </div>

              <div>
                <label class="block text-sm font-semibold text-md-on-surface mb-2">Maximum Allowed *</label>
                <input
                  v-model.number="formData.maxAllowed"
                  type="number"
                  min="1"
                  required
                  class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                  placeholder="1"
                />
                <p class="text-xs text-gray-500 mt-1">1 = Single choice, 0 = Unlimited</p>
              </div>
            </div>

            <div class="flex items-center">
              <input
                v-model="formData.isRequired"
                type="checkbox"
                id="isRequired"
                class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
              />
              <label for="isRequired" class="ml-2 block text-sm text-md-on-surface">
                This is a required selection
              </label>
            </div>

            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Display Order</label>
              <input
                v-model.number="formData.displayOrder"
                type="number"
                min="0"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary transition-all"
                placeholder="0"
              />
            </div>

            <!-- Select Options -->
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Select Options</label>
              <div v-if="availableOptions.length > 0" class="border border-gray-300 rounded-lg p-4 max-h-60 overflow-y-auto">
                <div
                  v-for="option in availableOptions"
                  :key="option.id"
                  class="flex items-center py-2 hover:bg-md-surface-container rounded px-2"
                >
                  <input
                    type="checkbox"
                    :id="`option-${option.id}`"
                    :value="option.id"
                    v-model="formData.optionIds"
                    class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <label :for="`option-${option.id}`" class="ml-3 flex-1 flex justify-between items-center cursor-pointer">
                    <span class="text-sm text-md-on-surface">{{ option.name }}</span>
                    <span class="text-sm" :class="{
                      'text-green-600': option.priceModifier > 0,
                      'text-red-600': option.priceModifier < 0,
                      'text-gray-500': option.priceModifier === 0
                    }">
                      {{ option.priceModifier >= 0 ? '+' : '' }}R{{ option.priceModifier.toFixed(2) }}
                    </span>
                  </label>
                </div>
              </div>
              <p v-else class="text-sm text-gray-500 italic">
                No options available. Create options first in the Options Library.
              </p>
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
                @click="saveGroup"
                :disabled="saving"
                class="flex-1 px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-glow-purple transition-all font-bold disabled:opacity-50 disabled:cursor-not-allowed md-ripple flex items-center justify-center gap-2"
              >
                <i v-if="saving" class="fas fa-spinner fa-spin"></i>
                <i v-else class="fas fa-save"></i>
                <span>{{ saving ? 'Saving...' : editingGroup ? 'Update' : 'Create' }}</span>
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
import { useOptionsApi, useOptionGroupsApi } from '~/composables/useApi'

definePageMeta({
  layout: 'menu'
})

const optionsApi = useOptionsApi()
const optionGroupsApi = useOptionGroupsApi()
const loading = ref(true)
const error = ref('')
const optionGroups = ref<any[]>([])
const availableOptions = ref<any[]>([])
const showCreateModal = ref(false)
const editingGroup = ref<any>(null)
const saving = ref(false)

const formData = ref({
  name: '',
  description: '',
  minRequired: 1,
  maxAllowed: 1,
  isRequired: true,
  displayOrder: 0,
  optionIds: [] as number[]
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

const fetchOptionGroups = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await optionGroupsApi.getOptionGroups()
    console.log('Option groups response:', response);
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        optionGroups.value = response.data
      } else if (response.data && Array.isArray(response.data.optionGroups)) {
        optionGroups.value = response.data.optionGroups
      } else {
        console.warn('Unexpected response structure:', response.data)
        optionGroups.value = []
      }
    } else {
      error.value = 'Failed to load option groups'
      optionGroups.value = []
    }
  } catch (err: any) {
    error.value = err.message || 'An error occurred'
    optionGroups.value = []
  } finally {
    loading.value = false
  }
}

const fetchAvailableOptions = async () => {
  try {
    const response = await optionsApi.getOptions()
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        availableOptions.value = response.data
      } else if (response.data && Array.isArray(response.data.options)) {
        availableOptions.value = response.data.options
      } else {
        availableOptions.value = []
      }
    }
  } catch (err) {
    console.error('Failed to fetch options', err)
    availableOptions.value = []
  }
}

const saveGroup = async () => {
  saving.value = true
  try {
    if (editingGroup.value) {
      const response = await optionGroupsApi.updateOptionGroup(editingGroup.value.id, formData.value)
      if (response.success) {
        await fetchOptionGroups()
        closeModal()
      } else {
        alert('Failed to update option group')
      }
    } else {
      const response = await optionGroupsApi.createOptionGroup(formData.value)
      if (response.success) {
        await fetchOptionGroups()
        closeModal()
      } else {
        alert('Failed to create option group')
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
    name: group.name,
    description: group.description || '',
    minRequired: group.minRequired,
    maxAllowed: group.maxAllowed,
    isRequired: group.isRequired,
    displayOrder: group.displayOrder,
    optionIds: group.options ? group.options.map((item: any) => item.optionId) : []
  }
}

const deleteGroup = async (id: number) => {
  if (!confirm('Are you sure you want to delete this option group?')) return

  try {
    const response = await optionGroupsApi.deleteOptionGroup(id)
    if (response.success) {
      await fetchOptionGroups()
    } else {
      alert('Failed to delete option group')
    }
  } catch (err: any) {
    alert(err.message || 'An error occurred')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingGroup.value = null
  formData.value = {
    name: '',
    description: '',
    minRequired: 1,
    maxAllowed: 1,
    isRequired: true,
    displayOrder: 0,
    optionIds: []
  }
}

watch([showCreateModal, editingGroup], () => {
  if (showCreateModal.value || editingGroup.value) {
    fetchAvailableOptions()
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
  // Set page metadata
  setPageHeader({
    title: 'Option Groups Library',
    description: 'Group related options together (e.g., "Size Selection", "Customizations")'
  })

  setPageActions(() => h('button', {
    onClick: () => showCreateModal.value = true,
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-bold flex items-center gap-2'
  }, [
    h('i', { class: 'fas fa-plus' }),
    'Add Option Group'
  ]))

  fetchOptionGroups()
  loadStats()
})
</script>











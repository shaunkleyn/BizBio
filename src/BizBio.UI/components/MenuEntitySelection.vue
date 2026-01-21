<template>
  <div class="max-w-4xl mx-auto">
    <div class="text-center mb-12">
      <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
        Select Your Business
      </h1>
      <p class="text-xl text-[var(--gray-text-color)] max-w-2xl mx-auto">
        Choose an existing business or create a new one for your menu.
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)] mb-4"></i>
      <p class="text-[var(--gray-text-color)]">Loading your businesses...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="text-center py-12">
      <i class="fas fa-exclamation-circle text-4xl text-red-500 mb-4"></i>
      <p class="text-red-500 mb-4">{{ error }}</p>
      <button
        @click="loadEntities"
        class="px-6 py-3 btn-gradient text-white rounded-lg font-semibold"
      >
        <i class="fas fa-redo mr-2"></i>
        Try Again
      </button>
    </div>

    <!-- Entity Selection -->
    <div v-else>
      <!-- Existing Entities -->
      <div v-if="entities.length > 0" class="mb-8">
        <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
          Your Businesses
        </h2>
        <div class="grid md:grid-cols-2 gap-6">
          <div
            v-for="entity in entities"
            :key="entity.id"
            @click="selectExistingEntity(entity)"
            :class="[
              'bg-md-surface border-2 rounded-xl p-6 cursor-pointer transition-all duration-300',
              selectedEntity?.id === entity.id
                ? 'border-[var(--primary-color)] ring-4 ring-[var(--primary-color)] ring-opacity-30 shadow-xl'
                : 'border-[var(--light-border-color)] hover:border-[var(--primary-color)] hover:shadow-lg'
            ]"
          >
            <!-- Entity Icon/Type -->
            <div class="flex items-start gap-4">
              <div class="flex-shrink-0">
                <div class="w-16 h-16 rounded-lg bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] flex items-center justify-center">
                  <i :class="getEntityIcon(entity.entityType)" class="text-2xl text-white"></i>
                </div>
              </div>
              <div class="flex-1 min-w-0">
                <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-1 truncate">
                  {{ entity.name }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)] mb-2">
                  {{ getEntityTypeName(entity.entityType) }}
                </p>
                <p v-if="entity.description" class="text-sm text-[var(--gray-text-color)] line-clamp-2">
                  {{ entity.description }}
                </p>
              </div>
            </div>

            <!-- Entity Details -->
            <div class="mt-4 pt-4 border-t border-[var(--light-border-color)]">
              <div class="flex items-center gap-4 text-sm text-[var(--gray-text-color)]">
                <span v-if="entity.catalogCount !== undefined">
                  <i class="fas fa-book mr-1"></i>
                  {{ entity.catalogCount }} {{ entity.catalogCount === 1 ? 'Menu' : 'Menus' }}
                </span>
                <span v-if="entity.city">
                  <i class="fas fa-map-marker-alt mr-1"></i>
                  {{ entity.city }}
                </span>
              </div>
            </div>

            <!-- Selected Badge -->
            <div v-if="selectedEntity?.id === entity.id" class="mt-4">
              <div class="bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)] px-4 py-2 rounded-lg text-center font-semibold">
                <i class="fas fa-check-circle mr-2"></i>
                Selected
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Create New Entity -->
      <div class="mt-8">
        <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
          {{ entities.length > 0 ? 'Or Create New Business' : 'Create Your First Business' }}
        </h2>

        <div
          @click="showCreateForm = !showCreateForm"
          :class="[
            'bg-md-surface border-2 rounded-xl p-6 cursor-pointer transition-all duration-300',
            showCreateForm
              ? 'border-[var(--primary-color)] shadow-lg'
              : 'border-dashed border-[var(--light-border-color)] hover:border-[var(--primary-color)]'
          ]"
        >
          <div class="text-center">
            <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-[var(--primary-color)] bg-opacity-10 mb-4">
              <i :class="showCreateForm ? 'fas fa-minus' : 'fas fa-plus'" class="text-2xl text-[var(--primary-color)]"></i>
            </div>
            <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2">
              {{ showCreateForm ? 'Hide Form' : 'Create New Business' }}
            </h3>
            <p class="text-[var(--gray-text-color)]">
              {{ showCreateForm ? 'Click to hide the form' : 'Click to add a new business' }}
            </p>
          </div>
        </div>

        <!-- Create Entity Form -->
        <div v-if="showCreateForm" class="mt-6 bg-md-surface border border-[var(--light-border-color)] rounded-xl p-6">
          <form @submit.prevent="createNewEntity" class="space-y-6">
            <!-- Entity Type -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Business Type <span class="text-red-500">*</span>
              </label>
              <select
                v-model="newEntity.entityType"
                required
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent bg-white"
              >
                <option value="">Select type...</option>
                <option value="0">Restaurant</option>
                <option value="1">Store</option>
                <option value="2">Venue</option>
                <option value="3">Organization</option>
              </select>
            </div>

            <!-- Business Name -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Business Name <span class="text-red-500">*</span>
              </label>
              <input
                v-model="newEntity.name"
                type="text"
                required
                placeholder="e.g., Joe's Coffee Shop"
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              />
            </div>

            <!-- Description -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Description
              </label>
              <textarea
                v-model="newEntity.description"
                rows="3"
                placeholder="Brief description of your business..."
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              ></textarea>
            </div>

            <!-- City -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                City
              </label>
              <input
                v-model="newEntity.city"
                type="text"
                placeholder="e.g., Cape Town"
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              />
            </div>

            <!-- Phone -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Phone Number
              </label>
              <input
                v-model="newEntity.contactPhone"
                type="tel"
                placeholder="e.g., +27 21 123 4567"
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              />
            </div>

            <!-- Address -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Address
              </label>
              <input
                v-model="newEntity.address"
                type="text"
                placeholder="e.g., 123 Main Street"
                class="w-full px-4 py-3 rounded-lg border border-[var(--light-border-color)] focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              />
            </div>

            <!-- Actions -->
            <div class="flex gap-4">
              <button
                type="submit"
                :disabled="isCreating"
                class="flex-1 px-6 py-3 btn-gradient text-white rounded-lg font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <i v-if="isCreating" class="fas fa-spinner fa-spin mr-2"></i>
                <i v-else class="fas fa-plus mr-2"></i>
                {{ isCreating ? 'Creating...' : 'Create Business' }}
              </button>
              <button
                type="button"
                @click="resetCreateForm"
                class="px-6 py-3 bg-gray-200 text-gray-700 rounded-lg font-semibold hover:bg-gray-300"
              >
                <i class="fas fa-times mr-2"></i>
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Navigation Buttons -->
      <div class="mt-12 flex justify-between">
        <button
          @click="$emit('previous')"
          class="px-8 py-4 bg-gray-200 text-gray-700 rounded-lg font-bold text-lg hover:bg-gray-300 transition-all"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          Back
        </button>
        <button
          @click="proceedToNextStep"
          :disabled="!selectedEntity"
          :class="[
            'px-8 py-4 rounded-lg font-bold text-lg transition-all',
            selectedEntity
              ? 'btn-gradient text-white hover:shadow-lg'
              : 'bg-gray-300 text-gray-500 cursor-not-allowed'
          ]"
        >
          Continue to Plan Selection
          <i class="fas fa-arrow-right ml-2"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  selectedEntity: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['next', 'previous', 'entity-selected'])

const entityApi = useEntityApi()
const toast = useToast()

const isLoading = ref(false)
const error = ref(null)
const entities = ref([])
const showCreateForm = ref(false)
const isCreating = ref(false)
const selectedEntity = ref(props.selectedEntity)

const newEntity = ref({
  entityType: '',
  name: '',
  description: '',
  city: '',
  contactPhone: '',
  address: ''
})

const getEntityIcon = (type) => {
  const icons = {
    0: 'fas fa-utensils',     // Restaurant
    1: 'fas fa-store',         // Store
    2: 'fas fa-building',      // Venue
    3: 'fas fa-briefcase'      // Organization
  }
  return icons[type] || 'fas fa-building'
}

const getEntityTypeName = (type) => {
  const names = {
    0: 'Restaurant',
    1: 'Store',
    2: 'Venue',
    3: 'Organization'
  }
  return names[type] || 'Business'
}

const loadEntities = async () => {
  isLoading.value = true
  error.value = null

  try {
    const response = await entityApi.getMyEntities()
    entities.value = response.data.entities || []
  } catch (err) {
    console.error('Failed to load entities:', err)
    error.value = 'Failed to load your businesses. Please try again.'
    toast.error('Failed to load businesses', 'Error')
  } finally {
    isLoading.value = false
  }
}

const selectExistingEntity = (entity) => {
  selectedEntity.value = entity
  showCreateForm.value = false
  emit('entity-selected', entity)
}

const createNewEntity = async () => {
  if (!newEntity.value.name || newEntity.value.entityType === '') {
    toast.error('Please fill in all required fields', 'Validation Error')
    return
  }

  isCreating.value = true

  try {
    const payload = {
      entityType: parseInt(newEntity.value.entityType),
      name: newEntity.value.name,
      description: newEntity.value.description || undefined,
      city: newEntity.value.city || undefined,
      contactPhone: newEntity.value.contactPhone || undefined,
      address: newEntity.value.address || undefined
    }

    const response = await entityApi.createEntity(payload)

    if (response.data.success) {
      const createdEntity = response.data.entity
      entities.value.push(createdEntity)
      selectedEntity.value = createdEntity
      showCreateForm.value = false
      resetCreateForm()
      toast.success('Business created successfully!', 'Success')
      emit('entity-selected', createdEntity)
    } else {
      throw new Error(response.data.error || 'Failed to create business')
    }
  } catch (err) {
    console.error('Failed to create entity:', err)
    const errorMessage = err.response?.data?.error || err.message || 'Failed to create business'
    toast.error(errorMessage, 'Error')
  } finally {
    isCreating.value = false
  }
}

const resetCreateForm = () => {
  newEntity.value = {
    entityType: '',
    name: '',
    description: '',
    city: '',
    contactPhone: '',
    address: ''
  }
  showCreateForm.value = false
}

const proceedToNextStep = () => {
  if (selectedEntity.value) {
    emit('next')
  }
}

onMounted(() => {
  loadEntities()
})
</script>

<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="flex justify-between items-center mb-8">
        <div>
          <h1 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
            Menu Bundles
          </h1>
          <p class="text-[var(--gray-text-color)] mt-2">
            Create special bundle deals like "Family Meal Deal" with multiple items and options
          </p>
        </div>
        <NuxtLink
          to="/dashboard/bundles/create"
          class="inline-flex items-center px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors"
        >
          <i class="fas fa-plus mr-2"></i>
          Create Bundle
        </NuxtLink>
      </div>

      <!-- Upgrade Notice -->
      <div
        v-if="!hasBundleFeature"
        class="bg-gradient-to-r from-orange-50 to-yellow-50 border-l-4 border-[var(--accent3-color)] p-6 rounded-lg mb-8"
      >
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <i class="fas fa-crown text-[var(--accent3-color)] text-2xl"></i>
          </div>
          <div class="ml-4 flex-1">
            <h3 class="text-lg font-semibold text-[var(--dark-text-color)] mb-2">
              Upgrade Required
            </h3>
            <p class="text-[var(--gray-text-color)] mb-4">
              Bundles are not available in your current subscription tier. Upgrade to create special bundle deals and boost your sales!
            </p>
            <NuxtLink
              to="/dashboard/subscription"
              class="inline-flex items-center px-4 py-2 bg-[var(--accent3-color)] text-white rounded-lg hover:bg-opacity-90 transition-colors"
            >
              Upgrade Now
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)]"></div>
      </div>

      <!-- Empty State -->
      <div
        v-else-if="bundles.length === 0"
        class="text-center py-20 bg-white rounded-lg shadow-sm border border-[var(--light-border-color)]"
      >
        <i class="fas fa-box-open text-6xl text-[var(--gray-text-color)] mb-4"></i>
        <h3 class="text-xl font-semibold text-[var(--dark-text-color)] mb-2">
          No Bundles Yet
        </h3>
        <p class="text-[var(--gray-text-color)] mb-6 max-w-md mx-auto">
          Create your first bundle to offer special deals like "Family Meal Deal" or "Lunch Special"
        </p>
        <NuxtLink
          v-if="hasBundleFeature"
          to="/dashboard/bundles/create"
          class="inline-flex items-center px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors"
        >
          <i class="fas fa-plus mr-2"></i>
          Create Your First Bundle
        </NuxtLink>
      </div>

      <!-- Bundles Grid -->
      <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="bundle in bundles"
          :key="bundle.id"
          class="bg-white rounded-lg shadow-sm border border-[var(--light-border-color)] hover:shadow-md transition-shadow overflow-hidden"
        >
          <!-- Bundle Image -->
          <div class="h-48 bg-gradient-to-br from-[var(--primary-color)] to-[var(--secondary-color)] relative">
            <div v-if="bundle.images" class="h-full w-full">
              <img :src="bundle.images" alt="" class="w-full h-full object-cover" />
            </div>
            <div class="absolute top-4 right-4">
              <span
                :class="[
                  'px-3 py-1 rounded-full text-xs font-semibold',
                  bundle.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                ]"
              >
                {{ bundle.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>

          <!-- Bundle Details -->
          <div class="p-6">
            <h3 class="text-xl font-semibold text-[var(--dark-text-color)] mb-2">
              {{ bundle.name }}
            </h3>
            <p v-if="bundle.description" class="text-[var(--gray-text-color)] text-sm mb-4 line-clamp-2">
              {{ bundle.description }}
            </p>
            <div class="flex items-center justify-between mb-4">
              <div class="text-2xl font-bold text-[var(--primary-color)]">
                R{{ bundle.basePrice.toFixed(2) }}
              </div>
              <div class="text-sm text-[var(--gray-text-color)]">
                {{ bundle.steps?.length || 0 }} steps
              </div>
            </div>

            <!-- Actions -->
            <div class="flex flex-col gap-2">
              <div class="flex gap-2">
                <NuxtLink
                  :to="`/dashboard/bundles/edit/${bundle.id}`"
                  class="flex-1 px-4 py-2 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors text-center text-sm"
                >
                  <i class="fas fa-edit mr-1"></i>
                  Edit
                </NuxtLink>
                <button
                  @click="confirmDelete(bundle)"
                  class="px-4 py-2 bg-red-50 text-red-600 rounded-lg hover:bg-red-100 transition-colors text-sm"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
              <button
                @click="showAddToCategory(bundle)"
                class="w-full px-4 py-2 bg-green-50 text-green-700 rounded-lg hover:bg-green-100 transition-colors text-sm"
              >
                <i class="fas fa-plus-circle mr-1"></i>
                Add to Menu
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div
      v-if="bundleToDelete"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click="bundleToDelete = null"
    >
      <div
        class="bg-white rounded-lg p-6 max-w-md mx-4"
        @click.stop
      >
        <h3 class="text-xl font-semibold text-[var(--dark-text-color)] mb-4">
          Delete Bundle?
        </h3>
        <p class="text-[var(--gray-text-color)] mb-6">
          Are you sure you want to delete "{{ bundleToDelete.name }}"? This action cannot be undone.
        </p>
        <div class="flex gap-3">
          <button
            @click="bundleToDelete = null"
            class="flex-1 px-4 py-2 border border-[var(--light-border-color)] rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="deleteBundle"
            class="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
          >
            Delete
          </button>
        </div>
      </div>
    </div>

    <!-- Add to Category Modal -->
    <div
      v-if="bundleToAdd"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click="bundleToAdd = null"
    >
      <div
        class="bg-white rounded-lg p-6 max-w-md mx-4 w-full"
        @click.stop
      >
        <h3 class="text-xl font-semibold text-[var(--dark-text-color)] mb-4">
          Add "{{ bundleToAdd.name }}" to Menu
        </h3>

        <div class="space-y-4 mb-6">
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Category
            </label>
            <select
              v-model="selectedCategoryId"
              class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
            >
              <option :value="null">No Category</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Sort Order
            </label>
            <input
              v-model.number="sortOrder"
              type="number"
              class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
            />
          </div>
        </div>

        <div class="flex gap-3">
          <button
            @click="bundleToAdd = null"
            class="flex-1 px-4 py-2 border border-[var(--light-border-color)] rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="addToCategory"
            :disabled="addingToCategory"
            class="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
          >
            {{ addingToCategory ? 'Adding...' : 'Add to Menu' }}
          </button>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useBundlesApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

const bundlesApi = useBundlesApi()
const toast = useToast()

const loading = ref(true)
const bundles = ref<any[]>([])
const bundleToDelete = ref<any>(null)
const bundleToAdd = ref<any>(null)
const selectedCategoryId = ref<number | null>(null)
const sortOrder = ref(0)
const addingToCategory = ref(false)
const hasBundleFeature = ref(true) // TODO: Check from user subscription
const catalogId = ref<string>('1') // TODO: Get from user's catalog
const categories = ref<any[]>([]) // TODO: Load from API

onMounted(async () => {
  await loadBundles()
  await loadCategories()
})

async function loadBundles() {
  try {
    loading.value = true
    const response = await bundlesApi.getBundles(catalogId.value)
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.bundles)) {
      bundles.value = response.data.bundles
    } else if (Array.isArray(response.data)) {
      bundles.value = response.data
    } else {
      bundles.value = []
    }
  } catch (error: any) {
    console.error('Error loading bundles:', error)
    if (error.response?.status === 400 && error.response?.data?.error?.includes('not available')) {
      hasBundleFeature.value = false
    } else {
      toast.error('Failed to load bundles')
    }
  } finally {
    loading.value = false
  }
}

function confirmDelete(bundle: any) {
  bundleToDelete.value = bundle
}

async function loadCategories() {
  // TODO: Load categories from API
  // For now, using mock data
  categories.value = [
    { id: 1, name: 'Specials' },
    { id: 2, name: 'Combos' },
    { id: 3, name: 'Family Meals' }
  ]
}

function showAddToCategory(bundle: any) {
  bundleToAdd.value = bundle
  selectedCategoryId.value = null
  sortOrder.value = 0
}

async function addToCategory() {
  if (!bundleToAdd.value) return

  try {
    addingToCategory.value = true
    await bundlesApi.addToCategory(catalogId.value, bundleToAdd.value.id, {
      categoryId: selectedCategoryId.value,
      sortOrder: sortOrder.value
    })
    toast.success('Bundle added to menu successfully!')
    bundleToAdd.value = null
  } catch (error) {
    console.error('Error adding bundle to category:', error)
    toast.error('Failed to add bundle to menu')
  } finally {
    addingToCategory.value = false
  }
}

async function deleteBundle() {
  if (!bundleToDelete.value) return

  try {
    await bundlesApi.deleteBundle(catalogId.value, bundleToDelete.value.id)
    toast.success('Bundle deleted successfully')
    bundles.value = bundles.value.filter(b => b.id !== bundleToDelete.value.id)
    bundleToDelete.value = null
  } catch (error) {
    console.error('Error deleting bundle:', error)
    toast.error('Failed to delete bundle')
  }
}
</script>

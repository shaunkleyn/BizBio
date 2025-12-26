<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Upgrade Notice -->
      <div
        v-if="!hasBundleFeature"
        class="gradient-border mesh-card bg-md-surface p-6 rounded-2xl mb-8 shadow-md-3"
      >
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <div class="bg-gradient-accent rounded-full p-3 shadow-glow-teal">
              <i class="fas fa-crown text-white text-2xl"></i>
            </div>
          </div>
          <div class="ml-4 flex-1">
            <h3 class="text-lg font-semibold text-md-on-surface mb-2">
              Upgrade Required
            </h3>
            <p class="text-md-on-surface-variant mb-4">
              Bundles are not available in your current subscription tier. Upgrade to create special bundle deals and boost your sales!
            </p>
            <NuxtLink
              to="/dashboard/subscription"
              class="btn-gradient inline-flex items-center px-4 py-2 rounded-xl shadow-md-2 hover:shadow-md-4 transition-all text-white"
            >
              Upgrade Now
              <i class="fas fa-arrow-right ml-2"></i>
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-md-primary"></div>
      </div>

      <!-- Empty State -->
      <div
        v-else-if="bundles.length === 0"
        class="text-center py-20 mesh-card bg-md-surface rounded-2xl shadow-md-3"
      >
        <div class="inline-block bg-gradient-primary rounded-full p-6 mb-4 shadow-glow-purple">
          <i class="fas fa-box-open text-6xl text-white"></i>
        </div>
        <h3 class="text-xl font-semibold text-md-on-surface mb-2">
          No Bundles Yet
        </h3>
        <p class="text-md-on-surface-variant mb-6 max-w-md mx-auto">
          Create your first bundle to offer special deals like "Family Meal Deal" or "Lunch Special"
        </p>
        <button
          v-if="hasBundleFeature"
          @click="showCreateModal = true"
          class="btn-gradient inline-flex items-center px-6 py-3 rounded-xl shadow-md-2 hover:shadow-md-4 transition-all text-white"
        >
          <i class="fas fa-plus mr-2"></i>
          Create Your First Bundle
        </button>
      </div>

      <!-- Bundles Grid -->
      <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="bundle in bundles"
          :key="bundle.id"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 card-hover overflow-hidden"
        >
          <!-- Bundle Image -->
          <div class="h-48 bg-gradient-primary relative">
            <div v-if="bundle.images" class="h-full w-full">
              <img :src="bundle.images" alt="" class="w-full h-full object-cover" />
            </div>
            <div class="absolute top-4 right-4">
              <span
                :class="[
                  'px-3 py-1 rounded-full text-xs font-semibold shadow-md-2',
                  bundle.isActive ? 'bg-md-success text-md-on-success' : 'bg-md-surface-container text-md-on-surface-variant'
                ]"
              >
                {{ bundle.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>

          <!-- Bundle Details -->
          <div class="p-6">
            <h3 class="text-xl font-semibold text-md-on-surface mb-2">
              {{ bundle.name }}
            </h3>
            <p v-if="bundle.description" class="text-md-on-surface-variant text-sm mb-4 line-clamp-2">
              {{ bundle.description }}
            </p>
            <div class="flex items-center justify-between mb-4">
              <div class="text-2xl font-bold gradient-text">
                R{{ bundle.basePrice.toFixed(2) }}
              </div>
              <div class="text-sm text-md-on-surface-variant">
                {{ bundle.steps?.length || 0 }} steps
              </div>
            </div>

            <!-- Actions -->
            <div class="flex flex-col gap-2">
              <div class="flex gap-2">
                <NuxtLink
                  :to="`/dashboard/bundles/edit/${bundle.id}`"
                  class="flex-1 px-4 py-2 btn-gradient rounded-xl shadow-md-2 hover:shadow-md-3 transition-all text-center text-sm text-white"
                >
                  <i class="fas fa-edit mr-1"></i>
                  Edit
                </NuxtLink>
                <button
                  @click="confirmDelete(bundle)"
                  class="px-4 py-2 bg-md-error-container text-md-on-error-container rounded-xl hover:bg-md-error hover:text-md-on-error transition-colors text-sm shadow-md-1"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
              <button
                @click="showAddToCategory(bundle)"
                class="w-full px-4 py-2 bg-md-success-container text-md-on-success-container rounded-xl hover:bg-md-success hover:text-md-on-success transition-colors text-sm shadow-md-1"
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
      class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
      @click.self="bundleToDelete = null"
    >
      <div
        class="modal-content mesh-card bg-md-surface rounded-2xl p-6 max-w-md mx-4 shadow-md-5 border border-md-outline-variant"
      >
        <div class="modal-header p-4 -m-6 mb-4">
          <h3 class="text-xl font-bold gradient-text relative z-10">
            Delete Bundle?
          </h3>
        </div>
        <p class="text-md-on-surface-variant mb-6">
          Are you sure you want to delete "{{ bundleToDelete.name }}"? This action cannot be undone.
        </p>
        <div class="flex gap-3">
          <button
            @click="bundleToDelete = null"
            class="flex-1 px-4 py-2 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all shadow-md-1 md-ripple"
          >
            Cancel
          </button>
          <button
            @click="deleteBundle"
            class="flex-1 px-4 py-2 bg-md-error text-md-on-error rounded-xl hover:shadow-md-3 transition-all shadow-md-2 md-ripple"
          >
            <i class="fas fa-trash mr-2"></i>
            Delete
          </button>
        </div>
      </div>
    </div>

    <!-- Add to Category Modal -->
    <div
      v-if="bundleToAdd"
      class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
      @click.self="bundleToAdd = null"
    >
      <div
        class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-md w-full border border-md-outline-variant overflow-hidden"
      >
        <div class="modal-header p-6">
          <h3 class="text-xl font-bold gradient-text relative z-10 flex items-center gap-2">
            <i class="fas fa-plus-circle text-md-primary"></i>
            Add "{{ bundleToAdd.name }}" to Menu
          </h3>
        </div>

        <div class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-layer-group text-md-primary"></i>
              Category
            </label>
            <select
              v-model="selectedCategoryId"
              class="w-full px-4 py-2 bg-md-surface-container text-md-on-surface border border-md-outline-variant rounded-xl focus:outline-none focus:ring-2 focus:ring-md-primary"
            >
              <option :value="null">No Category</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-sort-numeric-down text-md-primary"></i>
              Sort Order
            </label>
            <input
              v-model.number="sortOrder"
              type="number"
              class="w-full px-4 py-2 bg-md-surface-container text-md-on-surface border border-md-outline-variant rounded-xl focus:outline-none focus:ring-2 focus:ring-md-primary"
            />
          </div>
        </div>

        <div class="modal-footer p-6">
          <div class="relative z-10 flex gap-3">
            <button
              @click="bundleToAdd = null"
              class="flex-1 px-4 py-2 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all shadow-md-1 md-ripple"
            >
              Cancel
            </button>
            <button
              @click="addToCategory"
              :disabled="addingToCategory"
              class="flex-1 px-4 py-2 bg-md-success text-md-on-success rounded-xl hover:shadow-md-3 transition-all shadow-md-2 disabled:opacity-50 md-ripple flex items-center justify-center gap-2"
            >
              <i class="fas fa-check-circle"></i>
              {{ addingToCategory ? 'Adding...' : 'Add to Menu' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Bundle Creation Modal -->
    <div
      v-if="showCreateModal"
      class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
      @click.self="showCreateModal = false"
    >
      <div
        class="modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-4xl w-full max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant"
      >
        <!-- Header with Gradient -->
        <div class="modal-header p-6">
          <div class="flex justify-between items-center relative z-10">
            <div>
              <h2 class="text-2xl font-heading font-bold gradient-text">Create Bundle</h2>
              <p class="text-sm text-md-on-surface-variant mt-1">Create a special bundle deal for your menu</p>
            </div>
            <button
              @click="showCreateModal = false"
              class="modal-close-btn md-ripple shadow-md-1"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <!-- Modal Body -->
        <div class="flex-1 overflow-y-auto p-6">
          <!-- Progress Steps -->
          <div class="mb-8">
            <div class="flex items-center justify-center gap-2">
              <div v-for="step in 4" :key="step" class="flex items-center">
                <div :class="[
                  'flex items-center justify-center w-10 h-10 rounded-full font-bold text-sm transition-all',
                  currentStep >= step
                    ? 'btn-gradient text-white shadow-md-2'
                    : 'bg-md-surface-container text-md-on-surface-variant'
                ]">
                  <i v-if="currentStep > step" class="fas fa-check"></i>
                  <span v-else>{{ step }}</span>
                </div>
                <div v-if="step < 4" :class="[
                  'w-12 h-1 transition-all mx-1',
                  currentStep > step ? 'bg-md-primary' : 'bg-md-outline-variant'
                ]"></div>
              </div>
            </div>
            <div class="text-center mt-2 text-sm text-md-on-surface-variant">
              {{ getStepLabel(currentStep) }}
            </div>
          </div>

          <!-- Step 1: Basic Information -->
          <div v-if="currentStep === 1" class="space-y-6">
            <div>
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Bundle Name <span class="text-md-error">*</span>
              </label>
              <input
                v-model="bundleData.name"
                type="text"
                placeholder="e.g., Family Meal Deal"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
              />
            </div>

            <div>
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Description
              </label>
              <textarea
                v-model="bundleData.description"
                rows="3"
                placeholder="Any 2 Large Pizzas + 2L Drink"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
              ></textarea>
            </div>

            <div>
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Base Price <span class="text-md-error">*</span>
              </label>
              <div class="relative">
                <span class="absolute left-4 top-3.5 text-md-on-surface-variant">R</span>
                <input
                  v-model.number="bundleData.basePrice"
                  type="number"
                  step="0.01"
                  placeholder="250.00"
                  class="w-full pl-10 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
                />
              </div>
              <p class="text-sm text-md-on-surface-variant mt-1">
                This is the total price customers pay for the bundle
              </p>
            </div>
          </div>

          <!-- Step 2: Bundle Steps -->
          <div v-if="currentStep === 2" class="space-y-4">
            <div class="flex justify-between items-center">
              <p class="text-sm text-md-on-surface-variant">
                Add steps for customers to make selections (e.g., Choose Pizza 1, Choose Pizza 2)
              </p>
              <button
                @click="addStep"
                class="px-4 py-2 bg-md-primary-container text-md-on-primary-container rounded-xl hover:shadow-md-2 transition-all text-sm font-medium"
              >
                <i class="fas fa-plus mr-2"></i>
                Add Step
              </button>
            </div>

            <div v-if="bundleData.steps.length === 0" class="text-center py-12 bg-md-surface-container rounded-xl border-2 border-dashed border-md-outline-variant">
              <i class="fas fa-layer-group text-4xl text-md-on-surface-variant opacity-50 mb-3 block"></i>
              <p class="text-md-on-surface-variant">No steps added yet</p>
              <button
                @click="addStep"
                class="mt-4 px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2"
              >
                Add Your First Step
              </button>
            </div>

            <div v-else class="space-y-3">
              <div
                v-for="(step, index) in bundleData.steps"
                :key="index"
                class="bg-md-surface-container p-4 rounded-xl border border-md-outline-variant"
              >
                <div class="flex justify-between items-start mb-3">
                  <span class="px-3 py-1 bg-md-primary text-md-on-primary rounded-full text-sm font-bold">
                    Step {{ step.stepNumber }}
                  </span>
                  <button
                    @click="removeStep(index)"
                    class="text-md-error hover:bg-md-error-container p-2 rounded-lg transition-all"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
                <input
                  v-model="step.name"
                  type="text"
                  placeholder="Step name (e.g., Choose Your Pizza)"
                  class="w-full px-4 py-2 bg-md-surface border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
                />
              </div>
            </div>
          </div>

          <!-- Step 3: Assign Products -->
          <div v-if="currentStep === 3" class="space-y-6">
            <p class="text-sm text-md-on-surface-variant">
              Assign products to each step so customers can choose from them
            </p>

            <div v-if="bundleData.steps.length === 0" class="text-center py-12 bg-md-surface-container rounded-xl border-2 border-dashed border-md-outline-variant">
              <i class="fas fa-layer-group text-4xl text-md-on-surface-variant opacity-50 mb-3 block"></i>
              <p class="text-md-on-surface-variant">No steps created yet. Go back to add steps first.</p>
            </div>

            <div v-else class="space-y-6">
              <div
                v-for="(step, stepIndex) in bundleData.steps"
                :key="stepIndex"
                class="bg-md-surface-container border border-md-outline-variant rounded-xl p-5"
              >
                <div class="flex items-center gap-3 mb-4">
                  <div class="w-8 h-8 btn-gradient text-white rounded-full flex items-center justify-center font-bold text-sm shadow-md-2">
                    {{ stepIndex + 1 }}
                  </div>
                  <h3 class="text-lg font-semibold text-md-on-surface">
                    {{ step.name }}
                  </h3>
                </div>

                <!-- Product Selection Dropdown -->
                <div class="mb-3">
                  <label class="block text-sm font-medium text-md-on-surface mb-2">
                    Add Products
                  </label>
                  <select
                    @change="addProductToStep(stepIndex, $event)"
                    class="w-full px-4 py-2.5 bg-md-surface-container-high border border-md-outline-variant rounded-xl text-md-on-surface focus:ring-2 focus:ring-md-primary transition-all"
                  >
                    <option value="">-- Select a product --</option>
                    <option v-for="product in availableProducts" :key="product.id" :value="product.id">
                      {{ product.name }}
                    </option>
                  </select>
                </div>

                <!-- Selected Products List -->
                <div v-if="step.allowedProducts && step.allowedProducts.length > 0" class="space-y-2">
                  <div
                    v-for="(productId, index) in step.allowedProducts"
                    :key="index"
                    class="flex items-center justify-between bg-md-surface p-3 rounded-lg border border-md-outline-variant"
                  >
                    <span class="text-md-on-surface font-medium">
                      {{ getProductName(productId) }}
                    </span>
                    <button
                      @click="removeProductFromStep(stepIndex, index)"
                      class="text-md-error hover:bg-md-error-container p-2 rounded-lg transition-all"
                    >
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>

                <!-- Empty State -->
                <div v-else class="text-center py-6 bg-md-surface rounded-lg border border-md-outline-variant">
                  <p class="text-md-on-surface-variant text-sm">
                    No products assigned yet
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Step 4: Review -->
          <div v-if="currentStep === 4" class="space-y-6">
            <div class="text-center mb-6">
              <i class="fas fa-check-circle text-5xl text-md-success mb-4 block"></i>
              <h3 class="text-xl font-bold text-md-on-surface mb-2">Ready to Create!</h3>
              <p class="text-md-on-surface-variant">
                Review your bundle details below
              </p>
            </div>

            <!-- Bundle Summary -->
            <div class="bg-md-surface-container border border-md-outline-variant rounded-xl p-6 space-y-4">
              <div>
                <span class="text-sm text-md-on-surface-variant">Bundle Name:</span>
                <p class="text-lg font-bold text-md-on-surface">{{ bundleData.name }}</p>
              </div>
              <div v-if="bundleData.description">
                <span class="text-sm text-md-on-surface-variant">Description:</span>
                <p class="text-md-on-surface">{{ bundleData.description }}</p>
              </div>
              <div>
                <span class="text-sm text-md-on-surface-variant">Price:</span>
                <p class="text-2xl font-bold gradient-text">R{{ bundleData.basePrice.toFixed(2) }}</p>
              </div>
              <div>
                <span class="text-sm text-md-on-surface-variant">Steps:</span>
                <div class="mt-2 space-y-2">
                  <div v-for="(step, idx) in bundleData.steps" :key="idx" class="bg-md-surface p-3 rounded-lg">
                    <p class="font-medium text-md-on-surface">{{ idx + 1 }}. {{ step.name }}</p>
                    <p class="text-sm text-md-on-surface-variant mt-1">
                      {{ step.allowedProducts?.length || 0 }} products assigned
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer with Gradient Background -->
        <div class="modal-footer p-6">
          <div class="relative z-10 flex justify-between gap-3">
            <button
              v-if="currentStep > 1"
              @click="currentStep--"
              class="px-6 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl text-md-on-surface font-medium hover:bg-md-surface-container-high hover:shadow-md-1 transition-all md-ripple"
            >
              <i class="fas fa-arrow-left mr-2"></i>
              Back
            </button>
            <button
              v-else
              @click="showCreateModal = false"
              class="px-6 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl text-md-on-surface-variant font-medium hover:bg-md-surface-container-high hover:shadow-md-1 transition-all md-ripple"
            >
              Cancel
            </button>
            
            <button
              v-if="currentStep < 4"
              @click="currentStep++"
              :disabled="!canProceed"
              class="px-6 py-3 btn-gradient rounded-xl font-bold shadow-md-2 hover:shadow-glow-purple transition-all md-ripple flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
              <i class="fas fa-arrow-right"></i>
            </button>
            <button
              v-else
              @click="createBundle"
              :disabled="!canProceed"
              class="px-6 py-3 btn-gradient rounded-xl font-bold shadow-md-2 hover:shadow-glow-purple transition-all md-ripple flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i class="fas fa-save"></i>
              Create Bundle
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
import { useBundlesApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

definePageMeta({
  layout: 'menu'
})

const bundlesApi = useBundlesApi()
const libraryApi = useLibraryItemsApi();
const toast = useToast()

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActions } = usePageMeta()

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
const showCreateModal = ref(false)
const currentStep = ref(1)
const bundleData = ref({
  name: '',
  description: '',
  basePrice: 0,
  slug: '',
  steps: [] as any[]
})

const availableProducts = ref<any[]>([])

const canProceed = computed(() => {
  if (currentStep.value === 1) {
    return bundleData.value.name && bundleData.value.basePrice > 0
  }
  return true
})

onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Bundles',
    description: 'Create special bundle deals like "Family Meal Deal" with multiple items and options'
  })

  setPageActions(() => h('button', {
    onClick: () => showCreateModal.value = true,
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'
  }, [
    h('i', { class: 'fas fa-plus mr-2' }),
    'Create Bundle'
  ]))

  await loadBundles()
  await loadCategories()
  await loadProducts()
})

function getStepLabel(step: number): string {
  const labels = ['Basic Info', 'Add Steps', 'Assign Products', 'Review']
  return labels[step - 1]
}

function addStep() {
  bundleData.value.steps.push({
    stepNumber: bundleData.value.steps.length + 1,
    name: '',
    minSelect: 1,
    maxSelect: 1,
    allowedProducts: []
  })
}

function removeStep(index: number) {
  bundleData.value.steps.splice(index, 1)
  bundleData.value.steps.forEach((step, i) => {
    step.stepNumber = i + 1
  })
}

// Product assignment functions
function addProductToStep(stepIndex: number, event: Event) {
  const target = event.target as HTMLSelectElement
  const productId = parseInt(target.value)
  if (productId && !bundleData.value.steps[stepIndex].allowedProducts.includes(productId)) {
    bundleData.value.steps[stepIndex].allowedProducts.push(productId)
  }
  target.value = '' // Reset select
}

function removeProductFromStep(stepIndex: number, productIndex: number) {
  bundleData.value.steps[stepIndex].allowedProducts.splice(productIndex, 1)
}

function getProductName(productId: number): string {
  const product = availableProducts.value.find(p => p.id === productId)
  return product?.name || 'Unknown Product'
}

async function loadProducts() {
  console.log('Loading products for bundle assignment...')
  try {
    const itemsResponse = await libraryApi.getItems()
    console.log('Library items response:', itemsResponse)
    console.log('Library items response data:', itemsResponse.data)
    if (itemsResponse.success && itemsResponse.data) {
      console.log(itemsResponse.data)
      availableProducts.value = itemsResponse.data.items.map((item: any) => ({
        id: item.id,
        name: item.name
      }))
    }
  } catch (error) {
    console.error('Error loading products:', error)
  }
}

async function createBundle() {
  try {
    if (!bundleData.value.slug && bundleData.value.name) {
      bundleData.value.slug = bundleData.value.name
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '')
    }

    const response = await bundlesApi.createBundle(catalogId.value, bundleData.value)

    if (response.success) {
      toast.success('Bundle created successfully!')
      showCreateModal.value = false
      currentStep.value = 1
      bundleData.value = {
        name: '',
        description: '',
        basePrice: 0,
        slug: '',
        steps: []
      }
      await loadBundles()
    } else {
      toast.error(response.error || 'Failed to create bundle')
    }
  } catch (error) {
    console.error('Error creating bundle:', error)
    toast.error('An error occurred while creating the bundle')
  }
}

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


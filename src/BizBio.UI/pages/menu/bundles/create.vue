<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Progress Steps -->
      <div class="mb-12">
        <div class="flex items-center justify-center">
          <div v-for="step in 4" :key="step" class="flex items-center">
            <!-- Step Circle -->
            <div :class="[
              'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
              currentStep >= step
                ? 'btn-gradient text-white'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)]'
            , 'shadow-md-2 hover:shadow-md-4']">
              <i v-if="currentStep > step" class="fas fa-check"></i>
              <span v-else>{{ step }}</span>
            </div>

            <!-- Step Label -->
            <div class="hidden sm:block ml-3 mr-8">
              <div :class="[
                'text-sm font-semibold',
                currentStep >= step ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'
              ]">
                {{ getStepLabel(step) }}
              </div>
            </div>

            <!-- Connector Line -->
            <div v-if="step < 4" :class="[
              'hidden sm:block w-16 h-1 transition-all',
              currentStep > step ? 'bg-md-primary' : 'bg-[var(--light-border-color)]'
            ]"></div>
          </div>
        </div>
      </div>

      <!-- Step 1: Basic Information -->
      <div v-if="currentStep === 1" class="max-w-3xl mx-auto">
        <div class="bg-md-surface rounded-2xl shadow-sm border border-[var(--light-border-color)] p-8">
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
            Bundle Information
          </h2>

          <div class="space-y-6">
            <!-- Bundle Name -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Bundle Name <span class="text-red-500">*</span>
              </label>
              <input
                v-model="bundleData.name"
                type="text"
                placeholder="e.g., Family Meal Deal"
                class="w-full px-4 py-3 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              />
            </div>

            <!-- Description -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Description
              </label>
              <textarea
                v-model="bundleData.description"
                rows="3"
                placeholder="Any 2 Large Pizzas + 2L Drink"
                class="w-full px-4 py-3 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              ></textarea>
            </div>

            <!-- Base Price -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Base Price <span class="text-red-500">*</span>
              </label>
              <div class="relative">
                <span class="absolute left-4 top-3 text-[var(--gray-text-color)]">R</span>
                <input
                  v-model.number="bundleData.basePrice"
                  type="number"
                  step="0.01"
                  placeholder="250.00"
                  class="w-full pl-10 pr-4 py-3 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                />
              </div>
              <p class="text-sm text-[var(--gray-text-color)] mt-1">
                This is the total price customers pay for the bundle
              </p>
            </div>

            <!-- Slug -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                URL Slug
              </label>
              <input
                v-model="bundleData.slug"
                type="text"
                placeholder="family-meal-deal"
                class="w-full px-4 py-3 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              />
            </div>

            <!-- Image Upload -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Bundle Image
              </label>
              <div class="border-2 border-dashed border-[var(--light-border-color)] rounded-lg p-8 text-center">
                <i class="fas fa-image text-4xl text-[var(--gray-text-color)] mb-3"></i>
                <p class="text-[var(--gray-text-color)] mb-2">Click to upload or drag and drop</p>
                <input type="file" accept="image/*" class="hidden" />
                <button class="text-[var(--primary-color)] hover:underline">Choose Image</button>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <div class="flex justify-between mt-8">
            <NuxtLink
              to="/menu/bundles"
              class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
            >
              Cancel
            </NuxtLink>
            <button
              @click="nextStep"
              :disabled="!bundleData.name || !bundleData.basePrice"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next: Add Steps
              <i class="fas fa-arrow-right ml-2"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Step 2: Bundle Steps -->
      <div v-if="currentStep === 2" class="max-w-4xl mx-auto">
        <div class="bg-md-surface rounded-2xl shadow-sm border border-[var(--light-border-color)] p-8">
          <div class="flex justify-between items-center mb-6">
            <div>
              <h2 class="text-2xl font-bold text-[var(--dark-text-color)]">
                Bundle Steps
              </h2>
              <p class="text-[var(--gray-text-color)] mt-1">
                Add steps for customers to make selections (e.g., Choose Pizza 1, Choose Pizza 2)
              </p>
            </div>
            <button
              @click="addStep"
              class="px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors"
            >
              <i class="fas fa-plus mr-2"></i>
              Add Step
            </button>
          </div>

          <!-- Steps List -->
          <div class="space-y-4 mb-8">
            <div
              v-for="(step, index) in bundleData.steps"
              :key="index"
              class="border border-[var(--light-border-color)] rounded-lg p-6"
            >
              <div class="flex items-start gap-4">
                <div class="flex-shrink-0 w-10 h-10 btn-gradient text-white rounded-full flex items-center justify-center font-bold shadow-md-2 hover:shadow-md-4">
                  {{ index + 1 }}
                </div>
                <div class="flex-1 space-y-4">
                  <!-- Step Name -->
                  <div>
                    <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                      Step Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="step.name"
                      type="text"
                      placeholder="e.g., Choose Pizza 1"
                      class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                    />
                  </div>

                  <!-- Selection Rules -->
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                        Min Select
                      </label>
                      <input
                        v-model.number="step.minSelect"
                        type="number"
                        min="0"
                        class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                      />
                    </div>
                    <div>
                      <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                        Max Select
                      </label>
                      <input
                        v-model.number="step.maxSelect"
                        type="number"
                        min="1"
                        class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                      />
                    </div>
                  </div>
                </div>
                <button
                  @click="removeStep(index)"
                  class="flex-shrink-0 p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-if="bundleData.steps.length === 0" class="text-center py-12 border-2 border-dashed border-[var(--light-border-color)] rounded-lg">
            <i class="fas fa-layer-group text-4xl text-[var(--gray-text-color)] mb-3"></i>
            <p class="text-[var(--gray-text-color)]">No steps added yet. Click "Add Step" to get started.</p>
          </div>

          <!-- Navigation -->
          <div class="flex justify-between mt-8">
            <button
              @click="previousStep"
              class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
            >
              <i class="fas fa-arrow-left mr-2"></i>
              Back
            </button>
            <button
              @click="nextStep"
              :disabled="bundleData.steps.length === 0"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next: Assign Products
              <i class="fas fa-arrow-right ml-2"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Step 3: Product Assignment -->
      <div v-if="currentStep === 3" class="max-w-5xl mx-auto">
        <div class="bg-md-surface rounded-2xl shadow-sm border border-[var(--light-border-color)] p-8">
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
            Assign Products to Steps
          </h2>

          <div class="space-y-8">
            <div
              v-for="(step, stepIndex) in bundleData.steps"
              :key="stepIndex"
              class="border border-[var(--light-border-color)] rounded-lg p-6"
            >
              <div class="flex items-center gap-3 mb-4">
                <div class="w-8 h-8 btn-gradient text-white rounded-full flex items-center justify-center font-bold text-sm shadow-md-2 hover:shadow-md-4">
                  {{ stepIndex + 1 }}
                </div>
                <h3 class="text-lg font-semibold text-[var(--dark-text-color)]">
                  {{ step.name }}
                </h3>
              </div>

              <!-- Product Selection -->
              <div class="mb-4">
                <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                  Select Products Available in This Step
                </label>
                <div class="relative">
                  <select
                    @change="addProductToStep(stepIndex, $event)"
                    class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                  >
                    <option value="">-- Select a product --</option>
                    <option v-for="product in availableProducts" :key="product.id" :value="product.id">
                      {{ product.name }}
                    </option>
                  </select>
                </div>
              </div>

              <!-- Selected Products -->
              <div v-if="step.allowedProducts && step.allowedProducts.length > 0" class="space-y-2">
                <div
                  v-for="(productId, index) in step.allowedProducts"
                  :key="index"
                  class="flex items-center justify-between bg-md-surface-container p-3 rounded-lg"
                >
                  <span class="text-[var(--dark-text-color)]">
                    {{ getProductName(productId) }}
                  </span>
                  <button
                    @click="removeProductFromStep(Number(stepIndex), Number(index))"
                    class="text-red-600 hover:bg-red-50 p-2 rounded transition-colors"
                  >
                    <i class="fas fa-times"></i>
                  </button>
                </div>
              </div>

              <!-- Empty State -->
              <div v-else class="text-center py-6 bg-md-surface-container rounded-lg">
                <p class="text-[var(--gray-text-color)] text-sm">
                  No products assigned to this step yet
                </p>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <div class="flex justify-between mt-8">
            <button
              @click="previousStep"
              class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
            >
              <i class="fas fa-arrow-left mr-2"></i>
              Back
            </button>
            <button
              @click="nextStep"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors"
            >
              Next: Add Options
              <i class="fas fa-arrow-right ml-2"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Step 4: Option Groups & Options -->
      <div v-if="currentStep === 4" class="max-w-6xl mx-auto">
        <div class="bg-md-surface rounded-2xl shadow-sm border border-[var(--light-border-color)] p-8">
          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
            Add Option Groups & Options
          </h2>
          <p class="text-[var(--gray-text-color)] mb-8">
            Add option groups like "Choose Base", "Extra Toppings", etc., with individual options and price modifiers
          </p>

          <div class="space-y-8">
            <div
              v-for="(step, stepIndex) in bundleData.steps"
              :key="stepIndex"
              class="border-2 border-[var(--light-border-color)] rounded-lg p-6"
            >
              <!-- Step Header -->
              <div class="flex items-center justify-between mb-6">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 btn-gradient text-white rounded-full flex items-center justify-center font-bold text-sm shadow-md-2 hover:shadow-md-4">
                    {{ stepIndex + 1 }}
                  </div>
                  <h3 class="text-xl font-semibold text-[var(--dark-text-color)]">
                    {{ step.name }}
                  </h3>
                </div>
                <button
                  @click="addOptionGroup(stepIndex)"
                  class="px-4 py-2 bg-[var(--accent3-color)] text-white rounded-lg hover:bg-opacity-90 transition-colors text-sm"
                >
                  <i class="fas fa-plus mr-2"></i>
                  Add Option Group
                </button>
              </div>

              <!-- Option Groups -->
              <div v-if="step.optionGroups && step.optionGroups.length > 0" class="space-y-6">
                <div
                  v-for="(group, groupIndex) in step.optionGroups"
                  :key="groupIndex"
                  class="bg-md-surface-container rounded-lg p-6"
                >
                  <!-- Group Header -->
                  <div class="flex items-start justify-between mb-4">
                    <div class="flex-1 space-y-4">
                      <div>
                        <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                          Option Group Name
                        </label>
                        <input
                          v-model="group.name"
                          type="text"
                          placeholder="e.g., Choose Your Base"
                          class="w-full px-4 py-2 border border-[var(--light-border-color)] rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] bg-md-surface"
                        />
                      </div>

                      <div class="grid grid-cols-3 gap-4">
                        <div>
                          <label class="flex items-center text-sm">
                            <input
                              v-model="group.isRequired"
                              type="checkbox"
                              class="mr-2"
                            />
                            <span class="font-semibold text-[var(--dark-text-color)]">Required</span>
                          </label>
                        </div>
                        <div>
                          <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-1">
                            Min Select
                          </label>
                          <input
                            v-model.number="group.minSelect"
                            type="number"
                            min="0"
                            class="w-full px-3 py-1 border border-[var(--light-border-color)] rounded focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] bg-md-surface"
                          />
                        </div>
                        <div>
                          <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-1">
                            Max Select
                          </label>
                          <input
                            v-model.number="group.maxSelect"
                            type="number"
                            min="1"
                            class="w-full px-3 py-1 border border-[var(--light-border-color)] rounded focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] bg-md-surface"
                          />
                        </div>
                      </div>
                    </div>
                    <button
                      @click="removeOptionGroup(Number(stepIndex), Number(groupIndex))"
                      class="ml-4 p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>

                  <!-- Options -->
                  <div class="mt-4">
                    <div class="flex items-center justify-between mb-3">
                      <label class="text-sm font-semibold text-[var(--dark-text-color)]">
                        Options
                      </label>
                      <button
                        @click="addOption(Number(stepIndex), Number(groupIndex))"
                        class="text-sm text-[var(--primary-color)] hover:underline"
                      >
                        <i class="fas fa-plus mr-1"></i>
                        Add Option
                      </button>
                    </div>

                    <div v-if="group.options && group.options.length > 0" class="space-y-2">
                      <div
                        v-for="(option, optionIndex) in group.options"
                        :key="optionIndex"
                        class="flex items-center gap-3 bg-md-surface p-3 rounded-lg"
                      >
                        <input
                          v-model="option.name"
                          type="text"
                          placeholder="Option name (e.g., Extra Bacon)"
                          class="flex-1 px-3 py-2 border border-[var(--light-border-color)] rounded focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                        />
                        <div class="w-32">
                          <div class="relative">
                            <span class="absolute left-3 top-2 text-[var(--gray-text-color)]">R</span>
                            <input
                              v-model.number="option.priceModifier"
                              type="number"
                              step="0.01"
                              placeholder="0.00"
                              class="w-full pl-7 pr-3 py-2 border border-[var(--light-border-color)] rounded focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                            />
                          </div>
                        </div>
                        <label class="flex items-center text-sm whitespace-nowrap">
                          <input
                            v-model="option.isDefault"
                            type="checkbox"
                            class="mr-1"
                          />
                          Default
                        </label>
                        <button
                          @click="removeOption(Number(stepIndex), Number(groupIndex), Number(optionIndex))"
                          class="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
                        >
                          <i class="fas fa-times"></i>
                        </button>
                      </div>
                    </div>

                    <div v-else class="text-center py-4 bg-md-surface rounded-2xl border border-dashed border-[var(--light-border-color)]">
                      <p class="text-[var(--gray-text-color)] text-sm">
                        No options added yet
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Empty State -->
              <div v-else class="text-center py-8 border-2 border-dashed border-[var(--light-border-color)] rounded-lg">
                <i class="fas fa-sliders-h text-3xl text-[var(--gray-text-color)] mb-2"></i>
                <p class="text-[var(--gray-text-color)]">
                  No option groups added for this step
                </p>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <div class="flex justify-between mt-8">
            <button
              @click="previousStep"
              class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
            >
              <i class="fas fa-arrow-left mr-2"></i>
              Back
            </button>
            <button
              @click="saveBundle"
              :disabled="saving"
              class="px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i v-if="!saving" class="fas fa-check mr-2"></i>
              <i v-else class="fas fa-spinner fa-spin mr-2"></i>
              {{ saving ? 'Saving...' : 'Save Bundle' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useBundlesApi, useLibraryItemsApi, useCatalogsApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'
import { useRouter } from 'vue-router'

const bundlesApi = useBundlesApi()
const { getLibraryItems } = useLibraryItemsApi()
const { getMyCatalogs } = useCatalogsApi()
const toast = useToast()
const router = useRouter()

const currentStep = ref(1)
const saving = ref(false)
const loading = ref(false)
const catalogId = ref('')

const bundleData = ref({
  name: '',
  description: '',
  basePrice: 0,
  slug: '',
  images: '',
  sortOrder: 0,
  steps: [] as any[]
})

const availableProducts = ref<any[]>([])

// Load catalog and products on mount
onMounted(async () => {
  try {
    loading.value = true

    // Get user's first catalog
    const catalogsResponse = await getMyCatalogs()
    if (catalogsResponse.success && catalogsResponse.data?.length > 0) {
      catalogId.value = catalogsResponse.data[0].id.toString()
    }

    // Load library items
    const itemsResponse = await getLibraryItems()
    if (itemsResponse.success && itemsResponse.data) {
      availableProducts.value = itemsResponse.data.map((item: any) => ({
        id: item.id,
        name: item.name
      }))
    }
  } catch (error) {
    console.error('Error loading data:', error)
    toast.error('Failed to load products')
  } finally {
    loading.value = false
  }
})

function getStepLabel(step: number): string {
  const labels = ['Basic Info', 'Add Steps', 'Assign Products', 'Add Options']
  return labels[step - 1]
}

function nextStep() {
  if (currentStep.value < 4) {
    currentStep.value++
  }
}

function previousStep() {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

// Step Management
function addStep() {
  bundleData.value.steps.push({
    stepNumber: bundleData.value.steps.length + 1,
    name: '',
    minSelect: 1,
    maxSelect: 1,
    allowedProducts: [],
    optionGroups: []
  })
}

function removeStep(index: number) {
  bundleData.value.steps.splice(index, 1)
  // Renumber steps
  bundleData.value.steps.forEach((step, i) => {
    step.stepNumber = i + 1
  })
}

// Product Assignment
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

// Option Group Management
function addOptionGroup(stepIndex: number) {
  if (!bundleData.value.steps[stepIndex].optionGroups) {
    bundleData.value.steps[stepIndex].optionGroups = []
  }
  bundleData.value.steps[stepIndex].optionGroups.push({
    name: '',
    isRequired: false,
    minSelect: 0,
    maxSelect: 10,
    options: []
  })
}

function removeOptionGroup(stepIndex: number, groupIndex: number) {
  bundleData.value.steps[stepIndex].optionGroups.splice(groupIndex, 1)
}

// Option Management
function addOption(stepIndex: number, groupIndex: number) {
  if (!bundleData.value.steps[stepIndex].optionGroups[groupIndex].options) {
    bundleData.value.steps[stepIndex].optionGroups[groupIndex].options = []
  }
  bundleData.value.steps[stepIndex].optionGroups[groupIndex].options.push({
    name: '',
    priceModifier: 0,
    isDefault: false
  })
}

function removeOption(stepIndex: number, groupIndex: number, optionIndex: number) {
  bundleData.value.steps[stepIndex].optionGroups[groupIndex].options.splice(optionIndex, 1)
}

// Save Bundle
async function saveBundle() {
  try {
    saving.value = true

    // Create the bundle
    const bundleResponse = await bundlesApi.createBundle(catalogId.value, {
      name: bundleData.value.name,
      description: bundleData.value.description,
      basePrice: bundleData.value.basePrice,
      slug: bundleData.value.slug,
      images: bundleData.value.images,
      sortOrder: bundleData.value.sortOrder
    })

    const bundleId = bundleResponse.data.data.bundle.id

    // Create steps
    for (const step of bundleData.value.steps) {
      const stepResponse = await bundlesApi.addStep(catalogId.value, bundleId, {
        stepNumber: step.stepNumber,
        name: step.name,
        minSelect: step.minSelect,
        maxSelect: step.maxSelect
      })

      const stepId = stepResponse.data.data.step.id

      // Add products to step
      for (const productId of step.allowedProducts) {
        await bundlesApi.addProductToStep(catalogId.value, bundleId, stepId, {
          productId
        })
      }

      // Add option groups
      if (step.optionGroups) {
        for (const group of step.optionGroups) {
          const groupResponse = await bundlesApi.addOptionGroup(catalogId.value, bundleId, stepId, {
            name: group.name,
            isRequired: group.isRequired,
            minSelect: group.minSelect,
            maxSelect: group.maxSelect
          })

          const groupId = groupResponse.data.data.optionGroup.id

          // Add options to group
          if (group.options) {
            for (const option of group.options) {
              await bundlesApi.addOption(catalogId.value, bundleId, groupId, {
                name: option.name,
                priceModifier: option.priceModifier,
                isDefault: option.isDefault
              })
            }
          }
        }
      }
    }

    toast.success('Bundle created successfully!')
    router.push('/menu/bundles')
  } catch (error: any) {
    console.error('Error saving bundle:', error)
    if (error.response?.data?.error?.includes('not available')) {
      toast.error('Bundles feature is not available in your subscription tier')
    } else {
      toast.error('Failed to save bundle. Please try again.')
    }
  } finally {
    saving.value = false
  }
}
</script>






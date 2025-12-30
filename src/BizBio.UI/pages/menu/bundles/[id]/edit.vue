<template>
  <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
    <!-- Header -->
    <div class="my-8">
      <div class="flex items-center gap-3 mb-2">
        <NuxtLink to="/menu/bundles"
          class="flex flex-row gap-2 items-center text-[#4A90E2] hover:text-[#357ABD] transition-colors">
          <i class="fas fa-arrow-left"></i>
          <p class="text-sm font-medium text-[#4A90E2] tracking-wide uppercase">
            Bundle Editor
          </p>
        </NuxtLink>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-md-primary mx-auto"></div>
      <p class="text-md-on-surface-variant mt-4">Loading bundle...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="text-center py-12">
      <i class="fas fa-exclamation-circle text-5xl text-md-error mb-4"></i>
      <p class="text-md-error">{{ error }}</p>
    </div>

    <!-- Bundle Editor -->
    <div v-else>
      <!-- Basic Information -->
      <BundleCard mesh-card title="Basic Information" icon="fas fa-info-circle">
        <div class="space-y-6">
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">Bundle Name *</label>
            <input 
              v-model="bundleData.name"
              type="text" 
              placeholder="e.g., Family Meal Deal"
              class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent"
            />
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Base Price *</label>
              <div class="relative">
                <span class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 font-semibold">R</span>
                <input 
                  v-model.number="bundleData.basePrice"
                  type="number"
                  class="w-full pl-10 pr-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent font-bold text-lg"
                />
              </div>
            </div>
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">Bundle Type</label>
              <input 
                v-model="bundleData.slug"
                type="text"
                placeholder="meal-deal"
                class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">Description</label>
            <textarea 
              v-model="bundleData.description"
              rows="2" 
              placeholder="Describe this bundle..."
              class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent resize-none"
            ></textarea>
          </div>
        </div>
      </BundleCard>

      <!-- Bundle Steps -->
      <BundleCard 
        title="Bundle Steps" 
        description="Define the choices customers make"
      >
        <template #actions>
          <button
            @click="addStep"
            class="inline-flex items-center gap-2 px-5 py-3 bg-[#1E8E3E] text-white rounded-full font-semibold hover:shadow-lg transition-all duration-200"
          >
            <i class="fas fa-plus"></i>
            <span>Add Step</span>
          </button>
        </template>

        <!-- Empty State -->
        <div v-if="bundleData.steps.length === 0"
          class="text-center py-12 bg-md-surface-container rounded-xl border-2 border-dashed border-md-outline-variant">
          <i class="fas fa-layer-group text-5xl text-md-on-surface-variant opacity-50 mb-4"></i>
          <p class="text-md-on-surface-variant mb-4">No steps added yet</p>
          <button @click="addStep" class="px-6 py-3 bg-[#1E8E3E] text-white rounded-xl shadow-md-2">
            <i class="fas fa-plus mr-2"></i>
            Add Your First Step
          </button>
        </div>

        <!-- Step List -->
        <BundleStep
          v-for="(step, index) in bundleData.steps"
          :key="step.id"
          :step-number="index + 1"
          :name="step.name"
          :description="step.description || ''"
          :is-expanded="expandedStepId === step.id"
          @update:name="updateStepName(step.id, $event)"
          @delete="deleteStep(step.id)"
          @move="moveStep(index)"
        >
          <!-- Allowed Products -->
          <AllowedProductsList
            :products="step.products ?? []"
            :available-products="availableProducts"
            @add-product="addProductToStep(step, $event)"
            @remove-product="removeProductFromStep(step.id, $event)"
            @reorder-product="reorderProduct(step.id, $event)"
          />

          <!-- Option Groups -->
          <OptionGroupList
            :option-groups="step.optionGroups ?? []"
            @add-option-group="addOptionGroup(step.id)"
            @delete-group="deleteOptionGroup(step.id, $event)"
            @update-group-name="updateOptionGroupName($event)"
            @update-group-min="updateOptionGroupMin($event)"
            @update-group-max="updateOptionGroupMax($event)"
            @update-group-required="updateOptionGroupRequired($event)"
            @add-option="addOption($event)"
            @remove-option="removeOption($event)"
          />
        </BundleStep>
      </BundleCard>

      <!-- Summary -->
      <BundleSummaryCard
        :base-price="bundleData.basePrice"
        :steps-count="bundleData.steps.length"
        :products-count="totalProductsCount"
        :option-groups-count="totalOptionGroupsCount"
      />

      <!-- Save Actions -->
      <div class="flex gap-4 justify-end mb-8">
        <button
          @click="$router.back()"
          class="px-6 py-3 border-2 border-md-outline rounded-2xl font-semibold text-md-on-surface hover:bg-md-surface-container transition-colors"
        >
          Cancel
        </button>
        <button
          @click="saveBundle"
          :disabled="saving"
          class="px-8 py-3 bg-[#1E8E3E] text-white rounded-2xl font-semibold hover:shadow-lg transition-all disabled:opacity-50"
        >
          {{ saving ? 'Saving...' : 'Save Bundle' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBundlesApi, useLibraryItemsApi, useLibraryCategoriesApi, useUploadsApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'
import draggable from 'vuedraggable'
import BundleCard from '~/components/bundles/BundleCard.vue'
import BundleStep from '~/components/bundles/BundleStep.vue'
import AllowedProductsList from '~/components/bundles/AllowedProductsList.vue'
import OptionGroupList from '~/components/bundles/OptionGroupList.vue'
import BundleSummaryCard from '~/components/bundles/BundleSummaryCard.vue'

definePageMeta({
  layout: 'menu'
})

const route = useRoute()
const router = useRouter()
const bundlesApi = useBundlesApi()
const libraryApi = useLibraryItemsApi()
const categoriesApi = useLibraryCategoriesApi()
const uploadsApi = useUploadsApi()
const toast = useToast()

const loading = ref(true)
const saving = ref(false)
const error = ref('')
const bundleId = ref(route.params.id as string)
const catalogId = ref('1') // Will be loaded from bundle data

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
const categories = ref<any[]>([])
const fileInput = ref<HTMLInputElement | null>(null)

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader } = usePageMeta()

onMounted(async () => {
  setPageHeader({
    title: 'Edit Bundle',
    description: 'Manage your bundle configuration'
  })

  await loadProducts() // Load products FIRST
  await loadBundle()   // Then load bundle (so products are available for transformation)
})

async function loadBundle() {
  try {
    loading.value = true
    const response = await bundlesApi.getBundle(bundleId.value)

    if (response.success && response.data?.bundle) {
      const bundle = response.data.bundle
      bundleData.value = {
        name: bundle.name,
        description: bundle.description || '',
        basePrice: bundle.basePrice,
        slug: bundle.slug || '',
        images: bundle.images || '',
        sortOrder: bundle.sortOrder || 0,
        steps: (bundle.steps || []).map((step: any) => ({
          ...step,
          // Transform allowedProducts to products array with full product objects
          products: (step.allowedProducts || []).map((ap: any) => {
            const product = availableProducts.value.find(p => p.id === ap.productId)
            return product || { id: ap.productId, name: 'Loading...', productId: ap.productId }
          })
        }))
      }
    } else {
      error.value = 'Bundle not found'
    }
  } catch (err: any) {
    console.error('Error loading bundle:', err)
    error.value = err.response?.data?.error || 'Failed to load bundle'
  } finally {
    loading.value = false
  }
}



const openProductSelector = (stepId: number) => {
  // Open product selection modal
  console.log('Open product selector for step:', stepId)
}

const removeProduct = (stepId: number, productId: number) => {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) {
    step.products = step.products.filter(p => p.id !== productId)
  }
}

const reorderProduct = (stepId: number, { from, to }: { from: number, to: number }) => {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) {
    const [removed] = step.products.splice(from, 1)
    step.products.splice(to, 0, removed)
  }
}

async function loadProducts() {
  try {
    // Load categories first
    const categoriesResponse = await categoriesApi.getCategories()
    console.log('Categories Response:', categoriesResponse)
    
    if (categoriesResponse.success && categoriesResponse.data) {
      categories.value = categoriesResponse.data || []
      console.log('Loaded categories:', categories.value)
    }
    
    // Load products
    const response = await libraryApi.getItems()
    console.log('Products Response:', response)
    
    if (response.success && response.data?.items) {
      availableProducts.value = response.data.items
        .filter((item: any) => item.itemType === 0) // Only regular items, not bundles
        .map((item: any) => {
          // Find category name by categoryId
          const category = categories.value.find(c => c.id === item.categoryId)
          console.log(`Product: ${item.name}, CategoryId: ${item.categoryId}, Found Category:`, category)
          
          return {
            id: item.id,
            name: item.name,
            price: item.price,
            categoryId: item.categoryId,
            categoryName: category?.name || 'Uncategorized',
            description: item.description,
            icon: item.icon || 'fas fa-utensils',
            iconBg: item.iconBg,
            iconColor: item.iconColor
          }
        })
      
      console.log('Final products with categories:', availableProducts.value)
    }
  } catch (err) {
    console.error('Error loading products:', err)
  }
}

function getProductName(productId: number): string {
  const product = availableProducts.value.find(p => p.id === productId)
  return product?.name || 'Unknown Product'
}

function stepHasProduct(step: any, productId: number): boolean {
  if (!step.allowedProducts) return false
  return step.allowedProducts.some((p: any) => (p.productId || p.id) === productId)
}

function addStep() {
  const newStep = {
    id: Date.now(), // Temporary ID for new steps
    stepNumber: bundleData.value.steps.length + 1,
    name: '',
    minSelect: 1,
    maxSelect: 1,
    allowedProducts: [],
    products: [],
    optionGroups: [],
    isNew: true
  }
  bundleData.value.steps.push(newStep)
}

function updateStepName(stepId: number, newName: string) {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) {
    step.name = newName
  }
}

function moveStep(index: number, direction: 'up' | 'down') {
  if (direction === 'up' && index > 0) {
    const temp = bundleData.value.steps[index]
    bundleData.value.steps[index] = bundleData.value.steps[index - 1]
    bundleData.value.steps[index - 1] = temp
  } else if (direction === 'down' && index < bundleData.value.steps.length - 1) {
    const temp = bundleData.value.steps[index]
    bundleData.value.steps[index] = bundleData.value.steps[index + 1]
    bundleData.value.steps[index + 1] = temp
  }

  // Renumber steps after reorder
  bundleData.value.steps.forEach((s, i) => {
    s.stepNumber = i + 1
  })
}

// Track expanded step for accordion
const expandedStepId = ref<number | null>(null)

// Computed properties for summary
const totalProductsCount = computed(() => {
  return bundleData.value.steps.reduce((sum, step) => {
    return sum + (step.products?.length || 0)
  }, 0)
})

const totalOptionGroupsCount = computed(() => {
  return bundleData.value.steps.reduce((sum, step) => {
    return sum + (step.optionGroups?.length || 0)
  }, 0)
})

async function deleteStep(stepId: number) {
  const index = bundleData.value.steps.findIndex(s => s.id === stepId)
  if (index === -1) return

  const step = bundleData.value.steps[index]
  if (!confirm(`Are you sure you want to delete "${step.name || 'this step'}"?`)) return

  try {
    // If step exists in DB, delete it via API
    if (!step.isNew && step.id) {
      await bundlesApi.deleteStep(bundleId.value, step.id.toString())
      toast.success('Step deleted successfully')
    }

    // Remove from local array
    bundleData.value.steps.splice(index, 1)

    // Renumber remaining steps
    bundleData.value.steps.forEach((s, i) => {
      s.stepNumber = i + 1
    })
  } catch (err) {
    console.error('Error deleting step:', err)
    toast.error('Failed to delete step')
  }
}

function addProductToStep(step: any, productId: number) {
  if (productId && !stepHasProduct(step, productId)) {
    // Ensure arrays exist
    if (!step.allowedProducts) {
      step.allowedProducts = []
    }
    if (!step.products) {
      step.products = []
    }
    
    // Add to allowedProducts (for backend)
    step.allowedProducts.push({
      productId,
      isNew: true
    })
    
    // Add to step.products for the component display
    const product = availableProducts.value.find(p => p.id === productId)
    if (product) {
      step.products.push({ ...product })
    }
    
    // Force reactivity by reassigning the arrays
    step.allowedProducts = [...step.allowedProducts]
    step.products = [...step.products]
    
    console.log('Product added:', { productId, step: step.name, products: step.products })
  }
}

async function removeProductFromStep(stepId: number, productId: number) {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (!step) return

  try {
    // Find the product in allowedProducts
    const productIndex = step.allowedProducts?.findIndex((p: any) =>
      (p.productId || p.id) === productId
    )
    if (productIndex === undefined || productIndex === -1) return

    const product = step.allowedProducts[productIndex]

    // If product exists in DB, delete it via API
    if (!product.isNew && step.id && !step.isNew) {
      await bundlesApi.removeProductFromStep(
        bundleId.value,
        step.id.toString(),
        productId.toString()
      )
      toast.success('Product removed from step')
    }

    // Remove from allowedProducts array
    step.allowedProducts.splice(productIndex, 1)

    // Also remove from products array (for display)
    if (step.products) {
      const displayIndex = step.products.findIndex((p: any) => p.id === productId)
      if (displayIndex !== -1) {
        step.products.splice(displayIndex, 1)
      }
    }
  } catch (err) {
    console.error('Error removing product:', err)
    toast.error('Failed to remove product')
  }
}

async function onStepReorder() {
  // Update step numbers based on new order
  bundleData.value.steps.forEach((step, index) => {
    step.stepNumber = index + 1
  })

  try {
    // Send reorder request to backend
    const steps = bundleData.value.steps
      .filter(s => !s.isNew) // Only existing steps
      .map(s => ({
        stepId: s.id,
        stepNumber: s.stepNumber
      }))

    if (steps.length > 0) {
      await bundlesApi.reorderSteps(bundleId.value, { steps })
      toast.success('Steps reordered')
    }
  } catch (err) {
    console.error('Error reordering steps:', err)
    toast.error('Failed to reorder steps')
  }
}

async function handleImageUpload(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  try {
    const response = await uploadsApi.uploadFile(file)
    if (response.success && response.data?.url) {
      bundleData.value.images = response.data.url
      toast.success('Image uploaded successfully')
    }
  } catch (err) {
    console.error('Error uploading image:', err)
    toast.error('Failed to upload image')
  }
}

// Option Group Management Functions
function addOptionGroup(stepId: number) {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (!step) return
  
  if (!step.optionGroups) {
    step.optionGroups = []
  }
  
  const newOptionGroup = {
    id: Date.now(), // Temporary ID
    name: 'New Option Group',
    minSelections: 0,
    maxSelections: 1,
    required: false,
    options: [],
    isNew: true
  }
  
  step.optionGroups.push(newOptionGroup)
  step.optionGroups = [...step.optionGroups] // Force reactivity
  
  console.log('Option group added:', { stepId, optionGroup: newOptionGroup })
}

function deleteOptionGroup(stepId: number, groupId: number) {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (!step || !step.optionGroups) return
  
  step.optionGroups = step.optionGroups.filter(g => g.id !== groupId)
  console.log('Option group deleted:', { stepId, groupId })
}

function updateOptionGroupName({ id, name }: { id: number, name: string }) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === id)
      if (group) {
        group.name = name
      }
    }
  })
}

function updateOptionGroupMin({ id, min }: { id: number, min: number }) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === id)
      if (group) {
        group.minSelections = min
        group.required = min > 0
      }
    }
  })
}

function updateOptionGroupMax({ id, max }: { id: number, max: number }) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === id)
      if (group) {
        group.maxSelections = max
      }
    }
  })
}

function updateOptionGroupRequired({ id, required }: { id: number, required: boolean }) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === id)
      if (group) {
        group.required = required
      }
    }
  })
}

function addOption(groupId: number) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === groupId)
      if (group) {
        const newOption = {
          id: Date.now(),
          name: 'New Option',
          price: 0,
          isNew: true
        }
        if (!group.options) {
          group.options = []
        }
        group.options.push(newOption)
        // Force reactivity
        group.options = [...group.options]
        console.log('Option added:', { groupId, option: newOption })
      }
    }
  })
}

function removeOption({ groupId, optionId }: { groupId: number, optionId: number }) {
  bundleData.value.steps.forEach(step => {
    if (step.optionGroups) {
      const group = step.optionGroups.find(g => g.id === groupId)
      if (group && group.options) {
        group.options = group.options.filter(o => o.id !== optionId)
        console.log('Option removed:', { groupId, optionId })
      }
    }
  })
}

async function saveBundle() {
  try {
    saving.value = true

    // Update basic bundle info
    await bundlesApi.updateBundle(bundleId.value, {
      name: bundleData.value.name,
      description: bundleData.value.description,
      basePrice: bundleData.value.basePrice,
      slug: bundleData.value.slug,
      images: bundleData.value.images,
      sortOrder: bundleData.value.sortOrder
    })

    // Process steps
    for (const step of bundleData.value.steps) {
      let currentStepId = step.id

      if (step.isNew) {
        // Create new step
        const stepResponse = await bundlesApi.addStep(bundleId.value, {
          stepNumber: step.stepNumber,
          name: step.name,
          minSelect: step.minSelect,
          maxSelect: step.maxSelect
        })

        currentStepId = stepResponse.data.step.id
        step.id = currentStepId
        step.isNew = false

        // Add products to the new step
        if (step.allowedProducts) {
          for (const product of step.allowedProducts) {
            await bundlesApi.addProductToStep(
              bundleId.value,
              currentStepId,
              { productId: product.productId || product.id }
            )
          }
        }
      } else {
        // Update existing step
        await bundlesApi.updateStep(bundleId.value, step.id.toString(), {
          name: step.name,
          minSelect: step.minSelect,
          maxSelect: step.maxSelect,
          stepNumber: step.stepNumber
        })

        // Add new products to existing step
        if (step.allowedProducts) {
          for (const product of step.allowedProducts) {
            if (product.isNew) {
              await bundlesApi.addProductToStep(
                bundleId.value,
                step.id,
                { productId: product.productId || product.id }
              )
              product.isNew = false
            }
          }
        }
      }

      // Process option groups for this step
      if (step.optionGroups && step.optionGroups.length > 0) {
        for (const group of step.optionGroups) {
          if (group.isNew) {
            // Create new option group
            const groupResponse = await bundlesApi.addOptionGroup(bundleId.value, currentStepId.toString(), {
              name: group.name,
              minSelections: group.minSelections,
              maxSelections: group.maxSelections,
              required: group.required
            })

            const newGroupId = groupResponse.data?.optionGroup?.id
            if (newGroupId) {
              group.id = newGroupId
              group.isNew = false

              // Add options to the new group
              if (group.options) {
                for (const option of group.options) {
                  await bundlesApi.addOption(bundleId.value, currentStepId.toString(), newGroupId.toString(), {
                    name: option.name,
                    price: option.price
                  })
                }
              }
            }
          } else {
            // Update existing option group
            await bundlesApi.updateOptionGroup(bundleId.value, currentStepId.toString(), group.id.toString(), {
              name: group.name,
              minSelections: group.minSelections,
              maxSelections: group.maxSelections,
              required: group.required
            })

            // Add new options to existing group
            if (group.options) {
              for (const option of group.options) {
                if (option.isNew) {
                  await bundlesApi.addOption(bundleId.value, currentStepId.toString(), group.id.toString(), {
                    name: option.name,
                    price: option.price
                  })
                  option.isNew = false
                }
              }
            }
          }
        }
      }
    }

    toast.success('Bundle updated successfully!')

    // Reload the bundle data to show updated values
    await loadBundle()

    // Navigate to bundles list after a short delay
    setTimeout(() => {
      router.push('/menu/bundles')
    }, 1000)
  } catch (err: any) {
    console.error('Error saving bundle:', err)
    toast.error(err.response?.data?.error || 'Failed to save bundle')
  } finally {
    saving.value = false
  }
}
</script>

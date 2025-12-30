<template>
  <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
    <!-- Header -->
    <div class="my-8">
      <div class="flex items-center gap-3 mb-2">
        <NuxtLink 
          to="/menu/bundles"
          class="flex flex-row gap-2 items-center text-[#4A90E2] hover:text-[#357ABD] transition-colors"
        >
          <i class="fas fa-arrow-left"></i>
          <p class="text-sm font-medium text-[#4A90E2] tracking-wide uppercase">
            Bundle Editor
          </p>
        </NuxtLink>
      </div>
    </div>

    <!-- Basic Information -->
    <BundleCard mesh-card>
      <h2 class="text-xl font-bold text-md-on-surface mb-6 flex items-center gap-2">
        <i class="fas fa-info-circle text-md-primary"></i>
        Basic Information
      </h2>
      
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
            <select
              v-model="bundleData.type"
              class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent bg-md-surface"
            >
              <option>Meal Deal</option>
              <option>Family Package</option>
              <option>Party Bundle</option>
              <option>Custom</option>
            </select>
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
    <BundleCard>
      <div class="flex items-center justify-between mb-6">
        <div>
          <h2 class="text-2xl font-bold text-md-on-surface">Bundle Steps</h2>
          <p class="text-sm text-gray-600 mt-1">
            Define the choices customers make
          </p>
        </div>
        <button
          @click="addStep"
          class="inline-flex items-center gap-2 px-5 py-3 bg-[#1E8E3E] text-white rounded-full font-semibold hover:shadow-lg transition-all duration-200"
        >
          <i class="fas fa-plus"></i>
          <span>Add Step</span>
        </button>
      </div>

      <!-- Step List -->
      <BundleStep
        v-for="(step, index) in bundleData.steps"
        :key="step.id"
        :step-number="index + 1"
        :name="step.name"
        :description="step.description"
        :is-expanded="expandedStepId === step.id"
        @update:name="updateStepName(step.id, $event)"
        @delete="deleteStep(step.id)"
        @move="moveStep(index)"
      >
        <!-- Allowed Products -->
        <AllowedProductsList
          :products="step.products"
          @add-products="openProductSelector(step.id)"
          @remove-product="removeProduct(step.id, $event)"
          @reorder-product="reorderProduct(step.id, $event)"
        />

        <!-- Option Groups -->
        <OptionGroupList
          v-if="step.optionGroups.length > 0"
          :option-groups="step.optionGroups"
          @add-option-group="addOptionGroup(step.id)"
          @delete-group="deleteOptionGroup(step.id, $event)"
          @update-group-name="updateOptionGroupName($event)"
          @update-group-min="updateOptionGroupMin($event)"
          @update-group-max="updateOptionGroupMax($event)"
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
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import BundleCard from '~/components/bundles/BundleCard.vue'
import BundleStep from '~/components/bundles/BundleStep.vue'
import AllowedProductsList from '~/components/bundles/AllowedProductsList.vue'
import OptionGroupList from '~/components/bundles/OptionGroupList.vue'
import BundleSummaryCard from '~/components/bundles/BundleSummaryCard.vue'

interface Option {
  id: number
  name: string
  price: number
}

interface OptionGroup {
  id: number
  name: string
  minSelections: number
  maxSelections: number
  required: boolean
  options: Option[]
}

interface Product {
  id: number
  name: string
  description?: string
  icon?: string
  iconBg?: string
  iconColor?: string
}

interface BundleStep {
  id: number
  name: string
  description: string
  products: Product[]
  optionGroups: OptionGroup[]
}

interface BundleData {
  name: string
  basePrice: number
  type: string
  description: string
  steps: BundleStep[]
}

// State
const bundleData = ref<BundleData>({
  name: 'Family Meal Deal',
  basePrice: 250,
  type: 'Meal Deal',
  description: 'Perfect for families - includes 2 large pizzas, sides and drinks',
  steps: [
    {
      id: 1,
      name: 'Choose First Pizza',
      description: 'Customer selects one product from allowed list',
      products: [
        { id: 1, name: 'Bacon, Avo & Feta Pizza', description: 'Large size included', icon: 'fas fa-pizza-slice', iconBg: '#FCE8E6', iconColor: '#C5221F' },
        { id: 2, name: 'Classic Cheese Pizza', description: 'Large size included', icon: 'fas fa-pizza-slice', iconBg: '#FEF7E0', iconColor: '#F9AB00' },
        { id: 3, name: 'Pepperoni Deluxe Pizza', description: 'Large size included', icon: 'fas fa-pizza-slice', iconBg: '#E8F0FE', iconColor: '#1967D2' }
      ],
      optionGroups: [
        {
          id: 1,
          name: 'Choose Base',
          minSelections: 1,
          maxSelections: 1,
          required: true,
          options: [
            { id: 1, name: 'Gluten Free', price: 0 },
            { id: 2, name: 'Traditional', price: 0 },
            { id: 3, name: 'Pan', price: 0 }
          ]
        },
        {
          id: 2,
          name: 'Extra Meat Toppings',
          minSelections: 0,
          maxSelections: 10,
          required: false,
          options: [
            { id: 4, name: 'Bacon', price: 28.90 },
            { id: 5, name: 'Chicken', price: 28.90 },
            { id: 6, name: 'Ham', price: 28.90 }
          ]
        }
      ]
    }
  ]
})

const expandedStepId = ref<number | null>(1)
const saving = ref(false)

// Computed
const totalProductsCount = computed(() => {
  return bundleData.value.steps.reduce((sum, step) => sum + step.products.length, 0)
})

const totalOptionGroupsCount = computed(() => {
  return bundleData.value.steps.reduce((sum, step) => sum + step.optionGroups.length, 0)
})

// Methods
const addStep = () => {
  const newId = Math.max(...bundleData.value.steps.map(s => s.id), 0) + 1
  bundleData.value.steps.push({
    id: newId,
    name: `Step ${newId}`,
    description: 'Customer selects one product',
    products: [],
    optionGroups: []
  })
  expandedStepId.value = newId
}

const updateStepName = (stepId: number, newName: string) => {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) step.name = newName
}

const deleteStep = (stepId: number) => {
  bundleData.value.steps = bundleData.value.steps.filter(s => s.id !== stepId)
}

const moveStep = (index: number) => {
  // Implement drag/reorder logic
  console.log('Move step at index:', index)
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

const addOptionGroup = (stepId: number) => {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) {
    const newId = Math.max(...step.optionGroups.map(g => g.id), 0) + 1
    step.optionGroups.push({
      id: newId,
      name: 'New Option Group',
      minSelections: 0,
      maxSelections: 1,
      required: false,
      options: []
    })
  }
}

const deleteOptionGroup = (stepId: number, groupId: number) => {
  const step = bundleData.value.steps.find(s => s.id === stepId)
  if (step) {
    step.optionGroups = step.optionGroups.filter(g => g.id !== groupId)
  }
}

const updateOptionGroupName = ({ id, name }: { id: number, name: string }) => {
  // Find and update option group name across all steps
  bundleData.value.steps.forEach(step => {
    const group = step.optionGroups.find(g => g.id === id)
    if (group) group.name = name
  })
}

const updateOptionGroupMin = ({ id, min }: { id: number, min: number }) => {
  bundleData.value.steps.forEach(step => {
    const group = step.optionGroups.find(g => g.id === id)
    if (group) group.minSelections = min
  })
}

const updateOptionGroupMax = ({ id, max }: { id: number, max: number }) => {
  bundleData.value.steps.forEach(step => {
    const group = step.optionGroups.find(g => g.id === id)
    if (group) group.maxSelections = max
  })
}

const addOption = (groupId: number) => {
  // Find group and add option
  bundleData.value.steps.forEach(step => {
    const group = step.optionGroups.find(g => g.id === groupId)
    if (group) {
      const newId = Math.max(...group.options.map(o => o.id), 0) + 1
      group.options.push({
        id: newId,
        name: 'New Option',
        price: 0
      })
    }
  })
}

const removeOption = ({ groupId, optionId }: { groupId: number, optionId: number }) => {
  bundleData.value.steps.forEach(step => {
    const group = step.optionGroups.find(g => g.id === groupId)
    if (group) {
      group.options = group.options.filter(o => o.id !== optionId)
    }
  })
}

const saveBundle = async () => {
  saving.value = true
  try {
    // Save bundle logic here
    console.log('Saving bundle:', bundleData.value)
    await new Promise(resolve => setTimeout(resolve, 1000))
    // Navigate back or show success
  } catch (error) {
    console.error('Failed to save bundle:', error)
  } finally {
    saving.value = false
  }
}
</script>

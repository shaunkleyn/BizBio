<template>
  <div
    class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-end md:items-center justify-center"
    @click="emit('close')"
  >
    <div
      class="mesh-card bg-md-surface w-full md:max-w-2xl md:rounded-2xl max-h-[90vh] overflow-hidden flex flex-col rounded-t-2xl shadow-md-5"
      @click.stop
    >
      <!-- Image -->
      <div v-if="item.images" class="h-64 bg-gradient-to-br from-purple-500 to-pink-500 flex-shrink-0">
        <img :src="item.images" :alt="item.name" class="w-full h-full object-cover" />
      </div>
      <div v-else class="h-64 bg-gradient-primary flex items-center justify-center flex-shrink-0">
        <i class="fas fa-utensils text-6xl text-white/50"></i>
      </div>

      <!-- Close Button -->
      <button
        @click="emit('close')"
        class="absolute top-4 right-4 w-10 h-10 bg-md-surface rounded-full shadow-md-3 flex items-center justify-center hover:bg-md-surface-container-high transition-colors"
      >
        <i class="fas fa-times text-md-on-surface"></i>
      </button>

      <!-- Scrollable Content -->
      <div class="flex-1 overflow-y-auto p-6">
        <!-- Item Info -->
        <div class="mb-6">
          <div class="flex items-start gap-2 mb-2">
            <h2 class="text-2xl font-bold text-gray-900 flex-1">{{ item.name }}</h2>
            <span
              v-if="item.itemType === 1"
              class="px-3 py-1 bg-orange-100 text-orange-800 text-sm font-semibold rounded-full"
            >
              BUNDLE
            </span>
          </div>
          <p v-if="item.description" class="text-gray-600 mb-4">{{ item.description }}</p>
          <div class="text-2xl font-bold text-[var(--primary-color)]">
            R{{ item.price.toFixed(2) }}
          </div>
        </div>

        <!-- Bundle Steps (if bundle) -->
        <div v-if="item.itemType === 1 && bundleSteps.length > 0" class="mb-6">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">Build Your Bundle</h3>
          <div class="space-y-6">
            <div v-for="step in bundleSteps" :key="step.id" class="border border-gray-200 rounded-lg p-4">
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-semibold text-gray-900">
                  Step {{ step.stepNumber }}: {{ step.name }}
                </h4>
                <span class="text-sm text-gray-600">
                  Select {{ step.minSelect }}{{ step.maxSelect !== step.minSelect ? `-${step.maxSelect}` : '' }}
                </span>
              </div>

              <!-- Step Products -->
              <div class="space-y-2">
                <div
                  v-for="product in step.products"
                  :key="product.id"
                  class="flex items-center justify-between p-3 border border-gray-200 rounded-lg hover:border-[var(--primary-color)] cursor-pointer"
                  :class="{ 'border-[var(--primary-color)] bg-blue-50': isProductSelected(step.id, product.id) }"
                  @click="toggleStepProduct(step, product)"
                >
                  <div class="flex-1">
                    <div class="font-medium text-gray-900">{{ product.name }}</div>
                  </div>
                  <div class="flex items-center gap-3">
                    <span v-if="product.priceModifier !== 0" class="text-sm text-gray-600">
                      +R{{ product.priceModifier.toFixed(2) }}
                    </span>
                    <i
                      :class="[
                        'fas',
                        isProductSelected(step.id, product.id) ? 'fa-check-circle text-[var(--primary-color)]' : 'fa-circle text-gray-300'
                      ]"
                    ></i>
                  </div>
                </div>
              </div>

              <!-- Option Groups for this step -->
              <div v-if="step.optionGroups && step.optionGroups.length > 0" class="mt-4 space-y-4">
                <div v-for="optionGroup in step.optionGroups" :key="optionGroup.id" class="pt-4 border-t border-gray-200">
                  <div class="flex items-center justify-between mb-3">
                    <h5 class="font-medium text-gray-900">{{ optionGroup.name }}</h5>
                    <span class="text-sm text-gray-600">
                      {{ optionGroup.isRequired ? 'Required' : 'Optional' }}
                    </span>
                  </div>
                  <div class="space-y-2">
                    <div
                      v-for="option in optionGroup.options"
                      :key="option.id"
                      class="flex items-center justify-between p-2 border border-gray-200 rounded hover:border-[var(--primary-color)] cursor-pointer"
                      :class="{ 'border-[var(--primary-color)] bg-blue-50': isOptionSelected(optionGroup.id, option.id) }"
                      @click="toggleOption(optionGroup, option)"
                    >
                      <div class="flex-1">
                        <div class="text-sm text-gray-900">{{ option.name }}</div>
                      </div>
                      <div class="flex items-center gap-2">
                        <span v-if="option.priceModifier !== 0" class="text-sm text-gray-600">
                          {{ option.priceModifier > 0 ? '+' : '' }}R{{ option.priceModifier.toFixed(2) }}
                        </span>
                        <i
                          :class="[
                            'fas',
                            isOptionSelected(optionGroup.id, option.id) ? 'fa-check-circle text-[var(--primary-color)]' : 'fa-circle text-gray-300'
                          ]"
                        ></i>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Variants (if not bundle) -->
        <div v-if="item.itemType !== 1 && variants.length > 0" class="mb-6">
          <h3 class="text-lg font-semibold text-gray-900 mb-3">Select Size/Variant</h3>
          <div class="grid grid-cols-1 gap-2">
            <button
              v-for="variant in variants"
              :key="variant.id"
              @click="selectedVariant = variant"
              :class="[
                'p-3 border-2 rounded-lg text-left transition-colors',
                selectedVariant?.id === variant.id
                  ? 'border-[var(--primary-color)] bg-blue-50'
                  : 'border-gray-200 hover:border-[var(--primary-color)]'
              ]"
            >
              <div class="flex justify-between items-center">
                <span class="font-medium text-gray-900">{{ variant.name }}</span>
                <span class="font-bold text-[var(--primary-color)]">R{{ variant.price.toFixed(2) }}</span>
              </div>
            </button>
          </div>
        </div>

        <!-- Options (if not bundle) -->
        <div v-if="item.itemType !== 1 && optionGroups.length > 0" class="mb-6">
          <h3 class="text-lg font-semibold text-gray-900 mb-3">Customize Your Order</h3>
          <div class="space-y-4">
            <div v-for="group in optionGroups" :key="group.id">
              <h4 class="font-medium text-gray-900 mb-2">{{ group.name }}</h4>
              <div class="space-y-2">
                <label
                  v-for="option in group.options"
                  :key="option.id"
                  class="flex items-center justify-between p-3 border border-gray-200 rounded-lg cursor-pointer hover:border-[var(--primary-color)] transition-colors"
                >
                  <div class="flex items-center flex-1">
                    <input
                      type="checkbox"
                      :value="option.id"
                      v-model="selectedOptions"
                      class="mr-3 h-5 w-5 text-[var(--primary-color)] rounded"
                    />
                    <span class="text-gray-900">{{ option.name }}</span>
                  </div>
                  <span v-if="option.price !== 0" class="text-gray-600 font-medium">
                    +R{{ option.price.toFixed(2) }}
                  </span>
                </label>
              </div>
            </div>
          </div>
        </div>

        <!-- Special Instructions -->
        <div class="mb-6">
          <label class="block text-lg font-semibold text-gray-900 mb-2">
            Special Instructions
          </label>
          <textarea
            v-model="specialInstructions"
            placeholder="Any special requests? (e.g., no onions, extra sauce)"
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent resize-none"
            rows="3"
          ></textarea>
        </div>
      </div>

      <!-- Footer -->
      <div class="border-t border-gray-200 p-6 bg-white flex-shrink-0">
        <!-- Quantity Selector -->
        <div class="flex items-center justify-between mb-4">
          <span class="text-gray-900 font-medium">Quantity</span>
          <div class="flex items-center gap-4">
            <button
              @click="decrementQuantity"
              :disabled="quantity <= 1"
              class="w-10 h-10 rounded-full border-2 border-[var(--primary-color)] text-[var(--primary-color)] flex items-center justify-center disabled:opacity-50 disabled:cursor-not-allowed hover:bg-[var(--primary-color)] hover:text-white transition-colors"
            >
              <i class="fas fa-minus"></i>
            </button>
            <span class="text-xl font-bold text-gray-900 w-8 text-center">{{ quantity }}</span>
            <button
              @click="incrementQuantity"
              class="w-10 h-10 rounded-full border-2 border-[var(--primary-color)] text-[var(--primary-color)] flex items-center justify-center hover:bg-[var(--primary-color)] hover:text-white transition-colors"
            >
              <i class="fas fa-plus"></i>
            </button>
          </div>
        </div>

        <!-- Add to Cart Button -->
        <button
          @click="addToCart"
          :disabled="!canAddToCart"
          class="w-full py-4 bg-[var(--primary-color)] text-white font-semibold rounded-lg hover:bg-[var(--secondary-color)] transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Add to Cart - R{{ totalPrice.toFixed(2) }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useCartStore } from '~/stores/cart'
import { useToast } from '~/composables/useToast'
import { useMenusApi } from '~/composables/useApi'

const props = defineProps<{
  item: any
  menuSlug: string
}>()

const emit = defineEmits<{
  close: []
}>()

const cartStore = useCartStore()
const toast = useToast()
const menusApi = useMenusApi()

const quantity = ref(1)
const selectedVariant = ref<any>(null)
const selectedOptions = ref<number[]>([])
const specialInstructions = ref('')
const bundleSelections = ref<any>({
  stepProducts: {},
  options: {}
})

const loading = ref(false)
const variants = ref<any[]>([])
const optionGroups = ref<any[]>([])
const bundleSteps = ref<any[]>([])

onMounted(async () => {
  // Load variants, options, and bundle data based on item
  if (props.item.itemType === 1) {
    // Load bundle steps
    await loadBundleSteps()
  } else {
    // Load variants and options for regular items
    await loadVariantsAndOptions()
  }
})

async function loadBundleSteps() {
  try {
    loading.value = true

    if (props.item.bundleId) {
      const response = await menusApi.getBundleDetails(props.menuSlug, props.item.bundleId)
      const bundle = response.data.bundle
      bundleSteps.value = bundle.steps || []
    }
  } catch (error) {
    console.error('Error loading bundle details:', error)
    toast.error('Failed to load bundle details')
  } finally {
    loading.value = false
  }
}

async function loadVariantsAndOptions() {
  try {
    loading.value = true

    const response = await menusApi.getItemDetails(props.menuSlug, props.item.id)
    const itemData = response.data.item

    variants.value = itemData.variants || []

    // Set default variant if exists
    if (variants.value.length > 0) {
      const defaultVariant = variants.value.find(v => v.isDefault)
      selectedVariant.value = defaultVariant || variants.value[0]
    }

    // For now, optionGroups remain empty as they're not in the entity model yet
    optionGroups.value = []
  } catch (error) {
    console.error('Error loading item details:', error)
    toast.error('Failed to load item details')
  } finally {
    loading.value = false
  }
}

function isProductSelected(stepId: number, productId: number) {
  return bundleSelections.value.stepProducts[stepId]?.includes(productId) || false
}

function toggleStepProduct(step: any, product: any) {
  if (!bundleSelections.value.stepProducts[step.id]) {
    bundleSelections.value.stepProducts[step.id] = []
  }

  const selections = bundleSelections.value.stepProducts[step.id]
  const index = selections.indexOf(product.id)

  if (index > -1) {
    selections.splice(index, 1)
  } else {
    // Check max selection
    if (selections.length >= step.maxSelect) {
      if (step.maxSelect === 1) {
        // Replace selection
        bundleSelections.value.stepProducts[step.id] = [product.id]
      } else {
        toast.error(`You can only select up to ${step.maxSelect} items for this step`)
        return
      }
    } else {
      selections.push(product.id)
    }
  }
}

function isOptionSelected(optionGroupId: number, optionId: number) {
  return bundleSelections.value.options[optionGroupId]?.includes(optionId) || false
}

function toggleOption(optionGroup: any, option: any) {
  if (!bundleSelections.value.options[optionGroup.id]) {
    bundleSelections.value.options[optionGroup.id] = []
  }

  const selections = bundleSelections.value.options[optionGroup.id]
  const index = selections.indexOf(option.id)

  if (index > -1) {
    selections.splice(index, 1)
  } else {
    // Check max selection
    if (selections.length >= optionGroup.maxSelect) {
      if (optionGroup.maxSelect === 1) {
        bundleSelections.value.options[optionGroup.id] = [option.id]
      } else {
        toast.error(`You can only select up to ${optionGroup.maxSelect} options`)
        return
      }
    } else {
      selections.push(option.id)
    }
  }
}

const totalPrice = computed(() => {
  let price = props.item.price

  // Add variant price if selected
  if (selectedVariant.value) {
    price = selectedVariant.value.price
  }

  // Add selected options price
  const selectedOptionsData = optionGroups.value.flatMap(group =>
    group.options.filter((opt: any) => selectedOptions.value.includes(opt.id))
  )
  price += selectedOptionsData.reduce((sum: number, opt: any) => sum + opt.price, 0)

  // For bundles, calculate based on selections
  if (props.item.itemType === 1) {
    // Add bundle step products price modifiers
    for (const step of bundleSteps.value) {
      const stepSelections = bundleSelections.value.stepProducts[step.id] || []
      for (const productId of stepSelections) {
        const product = step.products.find((p: any) => p.id === productId)
        if (product && product.priceModifier) {
          price += product.priceModifier
        }
      }
    }

    // Add bundle option groups price modifiers
    for (const step of bundleSteps.value) {
      for (const optionGroup of step.optionGroups || []) {
        const optionSelections = bundleSelections.value.options[optionGroup.id] || []
        for (const optionId of optionSelections) {
          const option = optionGroup.options.find((o: any) => o.id === optionId)
          if (option && option.priceModifier) {
            price += option.priceModifier
          }
        }
      }
    }
  }

  return price * quantity.value
})

const canAddToCart = computed(() => {
  if (props.item.itemType === 1) {
    // Check if all bundle steps have required selections
    for (const step of bundleSteps.value) {
      const selections = bundleSelections.value.stepProducts[step.id] || []
      if (selections.length < step.minSelect) {
        return false
      }
    }
  }
  return true
})

function incrementQuantity() {
  quantity.value++
}

function decrementQuantity() {
  if (quantity.value > 1) {
    quantity.value--
  }
}

function addToCart() {
  const cartItem: any = {
    catalogItemId: props.item.id,
    name: props.item.name,
    description: props.item.description,
    basePrice: props.item.price,
    quantity: quantity.value,
    options: [],
    specialInstructions: specialInstructions.value || undefined,
    image: props.item.images,
    isBundle: props.item.itemType === 1
  }

  // Add variant if selected
  if (selectedVariant.value) {
    cartItem.variant = {
      id: selectedVariant.value.id,
      name: selectedVariant.value.name,
      price: selectedVariant.value.price
    }
  }

  // Add options if selected
  if (selectedOptions.value.length > 0) {
    const selectedOptionsData = optionGroups.value.flatMap(group =>
      group.options.filter((opt: any) => selectedOptions.value.includes(opt.id))
    )
    cartItem.options = selectedOptionsData.map((opt: any) => ({
      id: opt.id,
      groupName: optionGroups.value.find((g: any) => g.options.some((o: any) => o.id === opt.id))?.name || '',
      optionName: opt.name,
      price: opt.price
    }))
  }

  // Add bundle selections if bundle
  if (props.item.itemType === 1) {
    cartItem.bundleSelections = bundleSelections.value
  }

  cartStore.addItem(cartItem)
  toast.success(`${props.item.name} added to cart`)
  emit('close')
}
</script>

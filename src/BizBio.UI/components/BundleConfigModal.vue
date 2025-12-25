<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="isOpen"
        class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50"
        @click.self="close"
      >
        <div class="mesh-card bg-md-surface rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden shadow-md-5">
          <!-- Header -->
          <div class="sticky top-0 bg-md-surface-container border-b border-md-outline-variant px-6 py-4 z-10">
            <div class="flex items-center justify-between">
              <div>
                <h2 class="text-2xl font-bold text-md-on-surface">
                  {{ bundle?.name }}
                </h2>
                <p class="text-sm text-md-on-surface-variant mt-1">
                  {{ bundle?.description }}
                </p>
              </div>
              <button
                @click="close"
                class="p-2 hover:bg-md-surface-container-high rounded-full transition-colors"
              >
                <i class="fas fa-times text-xl text-md-on-surface-variant"></i>
              </button>
            </div>

            <!-- Progress Indicator -->
            <div class="mt-4 flex items-center gap-2">
              <div
                v-for="(step, index) in bundle?.steps"
                :key="step.id"
                :class="[
                  'flex-1 h-2 rounded-full transition-all',
                  currentStepIndex >= index
                    ? 'bg-[var(--primary-color)]'
                    : 'bg-gray-200'
                ]"
              ></div>
            </div>
          </div>

          <!-- Content -->
          <div class="overflow-y-auto px-6 py-6" style="max-height: calc(90vh - 240px)">
            <div v-if="currentStep" class="space-y-6">
              <!-- Step Header -->
              <div class="text-center mb-6">
                <div class="inline-flex items-center justify-center w-12 h-12 bg-[var(--primary-color)] text-white rounded-full font-bold text-lg mb-3">
                  {{ currentStepIndex + 1 }}
                </div>
                <h3 class="text-xl font-bold text-[var(--dark-text-color)]">
                  {{ currentStep.name }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)] mt-1">
                  {{ getSelectionHint(currentStep) }}
                </p>
              </div>

              <!-- Product Selection -->
              <div class="space-y-4">
                <div
                  v-for="product in currentStep.allowedProducts"
                  :key="product.id"
                  @click="selectProduct(product)"
                  :class="[
                    'border-2 rounded-lg p-4 cursor-pointer transition-all',
                    isProductSelected(product)
                      ? 'border-[var(--primary-color)] bg-blue-50'
                      : 'border-gray-200 hover:border-[var(--primary-color)] hover:bg-gray-50'
                  ]"
                >
                  <div class="flex items-center justify-between">
                    <div class="flex items-center gap-4">
                      <div :class="[
                        'w-6 h-6 rounded-full border-2 flex items-center justify-center transition-all',
                        isProductSelected(product)
                          ? 'border-[var(--primary-color)] bg-[var(--primary-color)]'
                          : 'border-gray-300'
                      ]">
                        <i v-if="isProductSelected(product)" class="fas fa-check text-white text-xs"></i>
                      </div>
                      <div>
                        <h4 class="font-semibold text-[var(--dark-text-color)]">
                          {{ product.name }}
                        </h4>
                        <p v-if="product.description" class="text-sm text-[var(--gray-text-color)] mt-1">
                          {{ product.description }}
                        </p>
                      </div>
                    </div>
                    <img
                      v-if="product.imageUrl"
                      :src="product.imageUrl"
                      :alt="product.name"
                      class="w-16 h-16 object-cover rounded-lg"
                    />
                  </div>

                  <!-- Option Groups for Selected Product -->
                  <div v-if="isProductSelected(product) && currentStep.optionGroups?.length > 0" class="mt-4 pt-4 border-t border-gray-200 space-y-4">
                    <div
                      v-for="optionGroup in currentStep.optionGroups"
                      :key="optionGroup.id"
                      class="space-y-3"
                    >
                      <div class="flex items-center justify-between">
                        <h5 class="font-semibold text-[var(--dark-text-color)]">
                          {{ optionGroup.name }}
                          <span v-if="optionGroup.isRequired" class="text-red-500">*</span>
                        </h5>
                        <span class="text-xs text-[var(--gray-text-color)]">
                          {{ getOptionGroupHint(optionGroup) }}
                        </span>
                      </div>

                      <!-- Options -->
                      <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
                        <div
                          v-for="option in optionGroup.options"
                          :key="option.id"
                          @click.stop="toggleOption(product.id, optionGroup, option)"
                          :class="[
                            'flex items-center justify-between p-3 rounded-lg border cursor-pointer transition-all',
                            isOptionSelected(product.id, optionGroup.id, option.id)
                              ? 'border-[var(--primary-color)] bg-blue-50'
                              : 'border-gray-200 hover:border-[var(--primary-color)]'
                          ]"
                        >
                          <div class="flex items-center gap-2">
                            <div v-if="optionGroup.maxSelect === 1" :class="[
                              'w-4 h-4 rounded-full border-2',
                              isOptionSelected(product.id, optionGroup.id, option.id)
                                ? 'border-[var(--primary-color)] bg-[var(--primary-color)]'
                                : 'border-gray-300'
                            ]">
                              <div v-if="isOptionSelected(product.id, optionGroup.id, option.id)" class="w-full h-full flex items-center justify-center">
                                <div class="w-2 h-2 bg-white rounded-full"></div>
                              </div>
                            </div>
                            <div v-else :class="[
                              'w-4 h-4 rounded border-2 flex items-center justify-center',
                              isOptionSelected(product.id, optionGroup.id, option.id)
                                ? 'border-[var(--primary-color)] bg-[var(--primary-color)]'
                                : 'border-gray-300'
                            ]">
                              <i v-if="isOptionSelected(product.id, optionGroup.id, option.id)" class="fas fa-check text-white text-xs"></i>
                            </div>
                            <span class="text-sm text-[var(--dark-text-color)]">
                              {{ option.name }}
                            </span>
                          </div>
                          <span class="text-sm font-semibold" :class="option.priceModifier > 0 ? 'text-[var(--primary-color)]' : 'text-green-600'">
                            {{ option.priceModifier > 0 ? '+R' + option.priceModifier.toFixed(2) : 'Free' }}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="sticky bottom-0 bg-md-surface-container border-t border-md-outline-variant px-6 py-4">
            <!-- Price Summary -->
            <div class="flex items-center justify-between mb-4 pb-4 border-b border-gray-200">
              <div>
                <p class="text-sm text-[var(--gray-text-color)]">Total Price</p>
                <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                  R{{ totalPrice.toFixed(2) }}
                </p>
              </div>
              <div v-if="getOptionsPriceModifier() > 0" class="text-right">
                <p class="text-sm text-[var(--gray-text-color)]">Base: R{{ bundle?.basePrice.toFixed(2) }}</p>
                <p class="text-sm text-green-600">Extras: +R{{ getOptionsPriceModifier().toFixed(2) }}</p>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex items-center gap-4">
              <button
                v-if="currentStepIndex > 0"
                @click="previousStep"
                class="flex-1 px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-gray-50 transition-colors font-semibold"
              >
                <i class="fas fa-arrow-left mr-2"></i>
                Back
              </button>
              <button
                v-if="currentStepIndex < (bundle?.steps?.length || 0) - 1"
                @click="nextStep"
                :disabled="!isCurrentStepValid()"
                class="flex-1 px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Next Step
                <i class="fas fa-arrow-right ml-2"></i>
              </button>
              <button
                v-else
                @click="addToCart"
                :disabled="!isCurrentStepValid() || adding"
                class="flex-1 px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <i v-if="!adding" class="fas fa-shopping-cart mr-2"></i>
                <i v-else class="fas fa-spinner fa-spin mr-2"></i>
                {{ adding ? 'Adding...' : 'Add to Cart' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useToast } from '~/composables/useToast'

interface Props {
  isOpen: boolean
  bundle: any
}

const props = defineProps<Props>()
const emit = defineEmits(['close', 'add-to-cart'])
const toast = useToast()

const currentStepIndex = ref(0)
const adding = ref(false)

// Configuration state: tracks all selections
const configuration = ref<any>({
  steps: []
})

// Initialize configuration when bundle changes
watch(() => props.bundle, (newBundle) => {
  if (newBundle) {
    configuration.value = {
      bundleId: newBundle.id,
      steps: newBundle.steps?.map((step: any) => ({
        stepId: step.id,
        stepName: step.name,
        selectedProducts: [],
        productOptions: {}
      })) || []
    }
    currentStepIndex.value = 0
  }
}, { immediate: true })

const currentStep = computed(() => {
  return props.bundle?.steps?.[currentStepIndex.value]
})

const totalPrice = computed(() => {
  let price = props.bundle?.basePrice || 0
  price += getOptionsPriceModifier()
  return price
})

function getOptionsPriceModifier(): number {
  let total = 0
  configuration.value.steps.forEach((stepConfig: any) => {
    Object.values(stepConfig.productOptions).forEach((productOptions: any) => {
      Object.values(productOptions).forEach((groupOptions: any) => {
        if (Array.isArray(groupOptions)) {
          groupOptions.forEach((option: any) => {
            total += option.priceModifier || 0
          })
        }
      })
    })
  })
  return total
}

function getSelectionHint(step: any): string {
  if (step.minSelect === step.maxSelect) {
    return `Select ${step.minSelect} item${step.minSelect > 1 ? 's' : ''}`
  } else if (step.minSelect === 0) {
    return `Select up to ${step.maxSelect} item${step.maxSelect > 1 ? 's' : ''}`
  } else {
    return `Select ${step.minSelect} to ${step.maxSelect} items`
  }
}

function getOptionGroupHint(group: any): string {
  if (group.minSelect === group.maxSelect) {
    return `Select ${group.minSelect}`
  } else if (group.minSelect === 0) {
    return `Select up to ${group.maxSelect}`
  } else {
    return `Select ${group.minSelect}-${group.maxSelect}`
  }
}

function selectProduct(product: any) {
  const stepConfig = configuration.value.steps[currentStepIndex.value]
  const step = currentStep.value

  if (!stepConfig || !step) return

  const index = stepConfig.selectedProducts.findIndex((p: any) => p.id === product.id)

  if (index >= 0) {
    // Deselect
    stepConfig.selectedProducts.splice(index, 1)
    delete stepConfig.productOptions[product.id]
  } else {
    // Check if we can add more
    if (stepConfig.selectedProducts.length >= step.maxSelect) {
      if (step.maxSelect === 1) {
        // Replace single selection
        stepConfig.selectedProducts = [product]
        stepConfig.productOptions = {}
      } else {
        toast.error(`You can only select up to ${step.maxSelect} items`)
        return
      }
    } else {
      stepConfig.selectedProducts.push(product)
    }

    // Initialize product options with defaults
    if (step.optionGroups?.length > 0) {
      stepConfig.productOptions[product.id] = {}
      step.optionGroups.forEach((group: any) => {
        const defaultOptions = group.options.filter((opt: any) => opt.isDefault)
        if (defaultOptions.length > 0) {
          stepConfig.productOptions[product.id][group.id] = defaultOptions
        }
      })
    }
  }
}

function isProductSelected(product: any): boolean {
  const stepConfig = configuration.value.steps[currentStepIndex.value]
  return stepConfig?.selectedProducts.some((p: any) => p.id === product.id) || false
}

function toggleOption(productId: number, optionGroup: any, option: any) {
  const stepConfig = configuration.value.steps[currentStepIndex.value]
  if (!stepConfig.productOptions[productId]) {
    stepConfig.productOptions[productId] = {}
  }

  const selectedOptions = stepConfig.productOptions[productId][optionGroup.id] || []

  if (optionGroup.maxSelect === 1) {
    // Radio behavior
    stepConfig.productOptions[productId][optionGroup.id] = [option]
  } else {
    // Checkbox behavior
    const index = selectedOptions.findIndex((opt: any) => opt.id === option.id)
    if (index >= 0) {
      selectedOptions.splice(index, 1)
    } else {
      if (selectedOptions.length >= optionGroup.maxSelect) {
        toast.error(`You can only select up to ${optionGroup.maxSelect} options`)
        return
      }
      selectedOptions.push(option)
    }
    stepConfig.productOptions[productId][optionGroup.id] = selectedOptions
  }
}

function isOptionSelected(productId: number, groupId: number, optionId: number): boolean {
  const stepConfig = configuration.value.steps[currentStepIndex.value]
  const selectedOptions = stepConfig?.productOptions[productId]?.[groupId] || []
  return selectedOptions.some((opt: any) => opt.id === optionId)
}

function isCurrentStepValid(): boolean {
  const stepConfig = configuration.value.steps[currentStepIndex.value]
  const step = currentStep.value

  if (!stepConfig || !step) return false

  // Check if minimum products selected
  if (stepConfig.selectedProducts.length < step.minSelect) {
    return false
  }

  // Check if all required option groups are satisfied
  if (step.optionGroups?.length > 0) {
    for (const product of stepConfig.selectedProducts) {
      for (const group of step.optionGroups) {
        if (group.isRequired) {
          const selectedOptions = stepConfig.productOptions[product.id]?.[group.id] || []
          if (selectedOptions.length < group.minSelect) {
            return false
          }
        }
      }
    }
  }

  return true
}

function nextStep() {
  if (isCurrentStepValid() && currentStepIndex.value < (props.bundle?.steps?.length || 0) - 1) {
    currentStepIndex.value++
  }
}

function previousStep() {
  if (currentStepIndex.value > 0) {
    currentStepIndex.value--
  }
}

function addToCart() {
  if (!isCurrentStepValid()) {
    toast.error('Please complete all required selections')
    return
  }

  adding.value = true
  emit('add-to-cart', {
    bundle: props.bundle,
    configuration: configuration.value,
    totalPrice: totalPrice.value
  })

  setTimeout(() => {
    adding.value = false
    close()
  }, 500)
}

function close() {
  emit('close')
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .bg-white,
.modal-leave-active .bg-white {
  transition: transform 0.3s ease;
}

.modal-enter-from .bg-white,
.modal-leave-to .bg-white {
  transform: scale(0.9);
}

.hide-scrollbar::-webkit-scrollbar {
  display: none;
}

.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>

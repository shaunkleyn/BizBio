<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="loading" class="flex items-center justify-center py-12">
        <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)]"></i>
      </div>

      <!-- Loaded Content -->
      <div v-else>
        <!-- Progress Steps -->
        <div class="mb-12">
          <div class="flex items-center justify-center">
            <div v-for="step in 4" :key="step" class="flex items-center">
              <!-- Step Circle -->
              <div :class="[
                'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
                currentStep >= step
                  ? 'bg-[var(--primary-color)] text-white'
                  : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)]'
              ]">
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
                currentStep > step ? 'bg-[var(--primary-color)]' : 'bg-[var(--light-border-color)]'
              ]"></div>
            </div>
          </div>
        </div>

        <!-- Step 1: Basic Information -->
        <div v-if="currentStep === 1" class="max-w-3xl mx-auto">
          <div class="bg-md-surface rounded-2xl shadow-sm border border-[var(--light-border-color)] p-8">
            <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-6">
              Edit Bundle Information
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
            </div>

            <!-- Navigation -->
            <div class="flex justify-between mt-8">
              <NuxtLink
                to="/dashboard/bundles"
                class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
              >
                Cancel
              </NuxtLink>
              <button
                @click="nextStep"
                :disabled="!bundleData.name || !bundleData.basePrice"
                class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Next: Manage Steps
                <i class="fas fa-arrow-right ml-2"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Step 2-4: Same as create.vue -->
        <!-- For brevity, include all remaining steps here -->

        <!-- Navigation at bottom -->
        <div v-if="currentStep > 1" class="max-w-6xl mx-auto mt-8">
          <div class="flex justify-between">
            <button
              @click="previousStep"
              class="px-6 py-3 border border-[var(--light-border-color)] rounded-lg hover:bg-md-surface-container transition-colors"
            >
              <i class="fas fa-arrow-left mr-2"></i>
              Back
            </button>
            <button
              v-if="currentStep < 4"
              @click="nextStep"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors"
            >
              Next
              <i class="fas fa-arrow-right ml-2"></i>
            </button>
            <button
              v-else
              @click="saveBundle"
              :disabled="saving"
              class="px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
            >
              <i v-if="!saving" class="fas fa-check mr-2"></i>
              <i v-else class="fas fa-spinner fa-spin mr-2"></i>
              {{ saving ? 'Saving...' : 'Update Bundle' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useBundlesApi, useCatalogsApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'
import { useRouter, useRoute } from 'vue-router'

const bundlesApi = useBundlesApi()
const { getMyCatalogs } = useCatalogsApi()
const toast = useToast()
const router = useRouter()
const route = useRoute()

const bundleId = computed(() => route.params.id as string)
const currentStep = ref(1)
const saving = ref(false)
const loading = ref(true)
const catalogId = ref('')

const bundleData = ref({
  name: '',
  description: '',
  basePrice: 0,
  slug: '',
  images: '',
  sortOrder: 0,
  isActive: true,
  steps: [] as any[]
})

onMounted(async () => {
  try {
    loading.value = true

    // Get catalog ID
    const catalogsResponse = await getMyCatalogs()
    if (catalogsResponse.success && catalogsResponse.data?.length > 0) {
      catalogId.value = catalogsResponse.data[0].id.toString()
    }

    // Load bundle data
    const bundleResponse = await bundlesApi.getBundle(catalogId.value, bundleId.value)
    if (bundleResponse.success && bundleResponse.data) {
      const bundle = bundleResponse.data
      bundleData.value = {
        name: bundle.name || '',
        description: bundle.description || '',
        basePrice: bundle.basePrice || 0,
        slug: bundle.slug || '',
        images: bundle.images || '',
        sortOrder: bundle.sortOrder || 0,
        isActive: bundle.isActive !== false,
        steps: bundle.steps || []
      }
    }
  } catch (error) {
    console.error('Error loading bundle:', error)
    toast.error('Failed to load bundle')
  } finally {
    loading.value = false
  }
})

function getStepLabel(step: number): string {
  const labels = ['Basic Info', 'Manage Steps', 'Assign Products', 'Manage Options']
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

async function saveBundle() {
  try {
    saving.value = true

    const response = await bundlesApi.updateBundle(catalogId.value, bundleId.value, {
      name: bundleData.value.name,
      description: bundleData.value.description,
      basePrice: bundleData.value.basePrice,
      slug: bundleData.value.slug,
      images: bundleData.value.images,
      sortOrder: bundleData.value.sortOrder,
      isActive: bundleData.value.isActive
    })

    if (response.success) {
      toast.success('Bundle updated successfully!')
      router.push('/dashboard/bundles')
    }
  } catch (error: any) {
    console.error('Error updating bundle:', error)
    toast.error('Failed to update bundle')
  } finally {
    saving.value = false
  }
}
</script>




<template>
  <div
    class="modal-overlay fixed inset-0 flex items-center justify-center z-50 p-4 animate-fadeSlide"
    @click.self="emit('close')"
  >
    <div
      class="modal-content mesh-card bg-md-surface rounded-2xl w-full max-w-4xl max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant shadow-md-5"
    >
      <!-- Header with Gradient -->
      <div class="modal-header p-6">
        <div class="flex justify-between items-center relative z-10">
          <div>
            <h2 class="text-2xl font-heading font-bold gradient-text">
              {{ item ? 'Edit Item' : 'Add New Item' }}
            </h2>
            <p class="text-sm text-md-on-surface-variant mt-1">
              {{ item ? 'Update item details' : 'Create a new menu item' }}
            </p>
          </div>
          <button
            @click="emit('close')"
            class="modal-close-btn md-ripple shadow-md-1"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>

      <!-- Form - Scrollable Body -->
      <form @submit.prevent="saveItem" class="flex-1 overflow-y-auto p-6 space-y-6">
        <!-- Images Upload -->
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-md-on-surface">Images</h3>
            <span class="text-sm text-md-on-surface-variant">
              Max {{ maxImages }} images
            </span>
          </div>

          <!-- Image Upload Area -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <!-- Existing Images -->
            <div
              v-for="(image, index) in form.images"
              :key="index"
              class="relative aspect-square rounded-lg overflow-hidden border-2 border-md-outline-variant hover:border-[var(--primary-color)] transition-colors"
            >
              <img :src="image" alt="Item image" class="w-full h-full object-cover" />
              <button
                type="button"
                @click="removeImage(index)"
                class="absolute top-2 right-2 w-8 h-8 bg-red-500 text-white rounded-full hover:bg-red-600 transition-colors"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Upload Button -->
            <label
              v-if="form.images.length < maxImages"
              class="aspect-square rounded-lg border-2 border-dashed border-md-outline hover:border-[var(--primary-color)] transition-colors cursor-pointer flex flex-col items-center justify-center bg-gray-50 hover:bg-gray-100"
            >
              <i class="fas fa-cloud-upload text-3xl text-md-on-surface-variant opacity-70 mb-2"></i>
              <span class="text-sm text-md-on-surface-variant">Upload Image</span>
              <input
                type="file"
                accept="image/*"
                @change="handleImageUpload"
                class="hidden"
              />
            </label>
          </div>
        </div>

        <!-- Basic Info -->
        <div class="space-y-4">
          <h3 class="text-lg font-semibold text-md-on-surface">Basic Information</h3>

          <!-- Name -->
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">
              Item Name <span class="text-red-500">*</span>
            </label>
            <input
              v-model="form.name"
              type="text"
              required
              placeholder="e.g., Margherita Pizza"
              class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
            />
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">
              Description
            </label>
            <textarea
              v-model="form.description"
              rows="3"
              placeholder="Describe your item..."
              class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] resize-none"
            ></textarea>
          </div>

          <!-- Category & Price -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-md-on-surface mb-1">
                Category
              </label>
              <select
                v-model="form.categoryId"
                class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              >
                <option :value="null">No Category</option>
                <option v-for="category in categories" :key="category.id" :value="category.id">
                  {{ category.name }}
                </option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-md-on-surface mb-1">
                Base Price (R) <span class="text-red-500">*</span>
              </label>
              <input
                v-model.number="form.price"
                type="number"
                step="0.01"
                min="0"
                required
                placeholder="0.00"
                class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              />
            </div>
          </div>
        </div>

        <!-- Allergens & Dietary Tags -->
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-md-on-surface">Allergens & Dietary Info</h3>
          </div>

          <!-- Quick Select Tags -->
          <div class="flex flex-wrap gap-2">
            <button
              v-for="tag in commonTags"
              :key="tag"
              type="button"
              @click="toggleTag(tag)"
              :class="[
                'px-3 py-1.5 rounded-full text-sm font-medium transition-all',
                form.tags.includes(tag)
                  ? 'bg-green-500 text-white shadow-md'
                  : 'bg-gray-100 text-md-on-surface hover:bg-gray-200'
              ]"
            >
              <i v-if="form.tags.includes(tag)" class="fas fa-check mr-1"></i>
              {{ tag }}
            </button>
          </div>

          <!-- Custom Tag Input -->
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">
              Add Custom Tags (press Enter or use comma)
            </label>
            <input
              v-model="tagInput"
              type="text"
              placeholder="Type tag and press Enter or use comma..."
              class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              @keydown.enter.prevent="addTagFromInput"
              @input="handleTagInput"
            />
          </div>

          <!-- Selected Tags as Pills -->
          <div v-if="form.tags.length > 0" class="space-y-2">
            <label class="block text-sm font-medium text-md-on-surface">Selected Tags</label>
            <div class="flex flex-wrap gap-2">
              <span
                v-for="tag in form.tags"
                :key="tag"
                class="inline-flex items-center gap-2 px-3 py-1.5 bg-md-primary-container text-md-on-primary-container rounded-full text-sm font-medium"
              >
                {{ tag }}
                <button
                  type="button"
                  @click="removeTag(tag)"
                  class="hover:text-md-on-primary-container"
                >
                  <i class="fas fa-times text-xs"></i>
                </button>
              </span>
            </div>
          </div>
        </div>

        <!-- Variants -->
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-md-on-surface">Variants (Sizes/Options)</h3>
            <button
              type="button"
              @click="addVariant"
              class="px-4 py-2 bg-md-primary-container0 text-white rounded-lg hover:bg-md-primary transition-colors text-sm"
            >
              <i class="fas fa-plus mr-1"></i>
              Add Variant
            </button>
          </div>

          <div v-if="form.variants.length === 0" class="text-center py-8 bg-gray-50 rounded-lg">
            <i class="fas fa-box text-3xl text-md-on-surface-variant opacity-50 mb-2"></i>
            <p class="text-md-on-surface-variant">No variants added. Click "Add Variant" to add sizes or options.</p>
          </div>

          <div v-else class="space-y-3">
            <div
              v-for="(variant, index) in form.variants"
              :key="index"
              class="border border-md-outline-variant rounded-lg p-4 bg-gray-50"
            >
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label class="block text-sm font-medium text-md-on-surface mb-1">
                    Name <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="variant.title"
                    type="text"
                    required
                    placeholder="e.g., Small, Medium, Large"
                    class="w-full px-3 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-md-on-surface mb-1">
                    Price (R) <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model.number="variant.price"
                    type="number"
                    step="0.01"
                    min="0"
                    required
                    placeholder="0.00"
                    class="w-full px-3 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                  />
                </div>

                <div class="flex items-end gap-2">
                  <div class="flex-1">
                    <label class="flex items-center">
                      <input
                        v-model="variant.isDefault"
                        type="checkbox"
                        @change="updateDefaultVariant(index)"
                        class="mr-2 h-4 w-4 text-[var(--primary-color)] rounded"
                      />
                      <span class="text-sm font-medium text-md-on-surface">Default</span>
                    </label>
                  </div>
                  <button
                    type="button"
                    @click="removeVariant(index)"
                    class="px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Options Selection -->
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-lg font-semibold text-md-on-surface">Options (Required Choices)</h3>
              <p class="text-sm text-md-on-surface-variant mt-1">Select required customization options for this item</p>
            </div>
            <NuxtLink
              to="/menu/library/option-groups"
              target="_blank"
              class="text-sm text-md-tertiary hover:text-md-on-tertiary-container"
            >
              Manage Options
            </NuxtLink>
          </div>

          <div v-if="loadingOptions" class="text-center py-8 bg-gray-50 rounded-lg">
            <i class="fas fa-spinner fa-spin text-2xl text-md-on-surface-variant opacity-70 mb-2"></i>
            <p class="text-md-on-surface-variant">Loading options...</p>
          </div>

          <div v-else-if="availableOptionGroups.length === 0" class="text-center py-8 bg-gray-50 rounded-lg">
            <i class="fas fa-sliders-h text-3xl text-md-on-surface-variant opacity-50 mb-2"></i>
            <p class="text-md-on-surface-variant mb-2">No option groups available</p>
            <NuxtLink
              to="/menu/library/option-groups"
              target="_blank"
              class="text-sm text-md-tertiary hover:text-md-on-tertiary-container"
            >
              Create your first option group
            </NuxtLink>
          </div>

          <div v-else class="space-y-2 max-h-60 overflow-y-auto border border-md-outline-variant rounded-lg p-4">
            <div
              v-for="group in availableOptionGroups"
              :key="group.id"
              class="flex items-start py-2 hover:bg-gray-50 rounded px-2"
            >
              <input
                type="checkbox"
                :id="`option-group-${group.id}`"
                :value="Number(group.id)"
                v-model="form.optionGroupIds"
                class="mt-1 h-4 w-4 text-md-tertiary border-md-outline rounded focus:ring-md-tertiary"
              />
              <label :for="`option-group-${group.id}`" class="ml-3 flex-1 cursor-pointer">
                <div class="text-sm font-medium text-md-on-surface">
                  {{ group.name }}
                  <span class="ml-1 px-2 py-0.5 bg-md-error-container text-md-on-error-container text-xs rounded-full">Required</span>
                </div>
                <div class="text-xs text-md-on-surface-variant">
                  {{ group.description }}
                  <span v-if="group.options && group.options.length > 0" class="ml-1">
                    ({{ group.options.length }} options)
                  </span>
                </div>
              </label>
            </div>
          </div>
        </div>

        <!-- Extras Selection -->
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-lg font-semibold text-md-on-surface">Extras (Optional Add-ons)</h3>
              <p class="text-sm text-md-on-surface-variant mt-1">Select optional extras available for this item</p>
            </div>
            <NuxtLink
              to="/menu/library/extra-groups"
              target="_blank"
              class="text-sm text-md-primary hover:text-md-on-primary-container"
            >
              Manage Extras
            </NuxtLink>
          </div>

          <div v-if="loadingExtras" class="text-center py-8 bg-gray-50 rounded-lg">
            <i class="fas fa-spinner fa-spin text-2xl text-md-on-surface-variant opacity-70 mb-2"></i>
            <p class="text-md-on-surface-variant">Loading extras...</p>
          </div>

          <div v-else-if="availableExtraGroups.length === 0" class="text-center py-8 bg-gray-50 rounded-lg">
            <i class="fas fa-plus-circle text-3xl text-md-on-surface-variant opacity-50 mb-2"></i>
            <p class="text-md-on-surface-variant mb-2">No extra groups available</p>
            <NuxtLink
              to="/menu/library/extra-groups"
              target="_blank"
              class="text-sm text-md-primary hover:text-md-on-primary-container"
            >
              Create your first extra group
            </NuxtLink>
          </div>

          <div v-else class="space-y-2 max-h-60 overflow-y-auto border border-md-outline-variant rounded-lg p-4">
            <div
              v-for="group in availableExtraGroups"
              :key="group.id"
              class="flex items-start py-2 hover:bg-gray-50 rounded px-2"
            >
              <input
                type="checkbox"
                :id="`extra-group-${group.id}`"
                :value="Number(group.id)"
                v-model="form.extraGroupIds"
                class="mt-1 h-4 w-4 text-md-primary border-md-outline rounded focus:ring-md-primary"
              />
              <label :for="`extra-group-${group.id}`" class="ml-3 flex-1 cursor-pointer">
                <div class="text-sm font-medium text-md-on-surface">{{ group.name }}</div>
                <div class="text-xs text-md-on-surface-variant">
                  {{ group.description }}
                  <span v-if="group.extras && group.extras.length > 0" class="ml-1">
                    ({{ group.extras.length }} extras)
                  </span>
                </div>
              </label>
            </div>
          </div>
        </div>

      </form>

      <!-- Footer with Gradient Background -->
      <div class="modal-footer p-6">
        <div class="relative z-10 flex gap-3">
          <button
            type="button"
            @click="emit('close')"
            class="flex-1 px-6 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all shadow-md-1 md-ripple font-medium"
          >
            Cancel
          </button>
          <button
            @click="saveItem"
            :disabled="saving || uploading"
            class="flex-1 px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-glow-purple transition-all font-bold disabled:opacity-50 disabled:cursor-not-allowed md-ripple flex items-center justify-center gap-2"
          >
            <i v-if="saving || uploading" class="fas fa-spinner fa-spin"></i>
            <i v-else class="fas fa-save"></i>
            <span v-if="saving || uploading">
              {{ uploading ? 'Uploading...' : 'Saving...' }}
            </span>
            <span v-else>
              {{ item ? 'Update Item' : 'Create Item' }}
            </span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useLibraryItemsApi, useUploadsApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

const props = defineProps<{
  item?: any
  categories: any[]
}>()

const emit = defineEmits<{
  close: []
  saved: []
}>()

const libraryItemsApi = useLibraryItemsApi()
const uploadsApi = useUploadsApi()
const toast = useToast()

const saving = ref(false)
const uploading = ref(false)
const loadingExtras = ref(false)
const loadingOptions = ref(false)
const tagInput = ref('')
const maxImages = ref(5) // TODO: Get from user's subscription
const availableExtraGroups = ref<any[]>([])
const availableOptionGroups = ref<any[]>([])

const commonTags = [
  'Gluten-Free',
  'Vegan',
  'Vegetarian',
  'Dairy-Free',
  'Nut-Free',
  'Halal',
  'Kosher',
  'Spicy',
  'Organic',
  'Sugar-Free'
]

const form = reactive({
  name: '',
  description: '',
  categoryId: null as number | null,
  price: 0,
  images: [] as string[],
  tags: [] as string[],
  variants: [] as any[],
  optionGroupIds: [] as number[],
  extraGroupIds: [] as number[]
})

onMounted(async () => {
  if (props.item) {
    form.name = props.item.name
    form.description = props.item.description || ''
    form.categoryId = props.item.categoryId
    form.price = props.item.price
    form.images = props.item.images || []
    form.tags = props.item.tags || []
    form.variants = props.item.variants?.map((v: any) => ({ ...v })) || []

    // Load option groups
    form.optionGroupIds = props.item.optionGroups?.map((g: any) => {
      const id = g.optionGroupId || g.OptionGroupId
      return typeof id === 'number' ? id : parseInt(id)
    }).filter((id: any) => !isNaN(id)) || []

    // Load extra groups
    form.extraGroupIds = props.item.extraGroups?.map((g: any) => {
      const id = g.extraGroupId || g.ExtraGroupId
      return typeof id === 'number' ? id : parseInt(id)
    }).filter((id: any) => !isNaN(id)) || []
  }

  // Fetch available groups in parallel
  await Promise.all([
    fetchOptionGroups(),
    fetchExtraGroups()
  ])
})

async function fetchOptionGroups() {
  loadingOptions.value = true
  try {
    const api = useApi()
    const response = await api.get('/library/option-groups')
    console.log('Option groups response:', response)
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        availableOptionGroups.value = response.data
      } else if (response.data && Array.isArray(response.data.optionGroups)) {
        availableOptionGroups.value = response.data.optionGroups
      } else {
        availableOptionGroups.value = []
      }
    }
  } catch (error) {
    console.error('Error fetching option groups:', error)
    availableOptionGroups.value = []
  } finally {
    loadingOptions.value = false
  }
}

async function fetchExtraGroups() {
  loadingExtras.value = true
  try {
    const api = useApi()
    const response = await api.get('/library/extra-groups')
    console.log('Extra groups response:', response)
    if (response.success) {
      // Ensure we always set an array
      if (Array.isArray(response.data)) {
        availableExtraGroups.value = response.data
      } else if (response.data && Array.isArray(response.data.extraGroups)) {
        availableExtraGroups.value = response.data.extraGroups
      } else {
        availableExtraGroups.value = []
      }
    }
  } catch (error) {
    console.error('Error fetching extra groups:', error)
    availableExtraGroups.value = []
  } finally {
    loadingExtras.value = false
  }
}

function toggleTag(tag: string) {
  const index = form.tags.indexOf(tag)
  if (index > -1) {
    form.tags.splice(index, 1)
  } else {
    form.tags.push(tag)
  }
}

function handleTagInput(event: Event) {
  const input = event.target as HTMLInputElement
  const value = input.value

  // Check for comma
  if (value.includes(',')) {
    const tags = value.split(',').map(t => t.trim()).filter(t => t)
    tags.forEach(tag => {
      if (tag && !form.tags.includes(tag)) {
        form.tags.push(tag)
      }
    })
    tagInput.value = ''
  }
}

function addTagFromInput() {
  const tag = tagInput.value.trim()
  if (tag && !form.tags.includes(tag)) {
    form.tags.push(tag)
    tagInput.value = ''
  }
}

function removeTag(tag: string) {
  const index = form.tags.indexOf(tag)
  if (index > -1) {
    form.tags.splice(index, 1)
  }
}

async function handleImageUpload(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  if (form.images.length >= maxImages.value) {
    toast.error(`Maximum ${maxImages.value} images allowed`)
    return
  }

  try {
    uploading.value = true
    const response = await uploadsApi.uploadMenuImage(file)
    const imageUrl = response.data.url || response.url
    form.images.push(imageUrl)
    toast.success('Image uploaded successfully')
  } catch (error) {
    console.error('Error uploading image:', error)
    toast.error('Failed to upload image')
  } finally {
    uploading.value = false
    input.value = '' // Reset input
  }
}

function removeImage(index: number) {
  form.images.splice(index, 1)
}

function addVariant() {
  form.variants.push({
    title: '',
    price: form.price,
    isDefault: form.variants.length === 0
  })
}

function updateDefaultVariant(index: number) {
  // If this variant is set to default, unset others
  if (form.variants[index].isDefault) {
    form.variants.forEach((v, i) => {
      if (i !== index) v.isDefault = false
    })
  }
}

function removeVariant(index: number) {
  form.variants.splice(index, 1)
  // If we removed the default, make the first one default
  if (form.variants.length > 0 && !form.variants.some(v => v.isDefault)) {
    form.variants[0].isDefault = true
  }
}

async function saveItem() {
  try {
    saving.value = true

    const data = {
      name: form.name,
      description: form.description || null,
      categoryId: form.categoryId,
      price: form.price,
      images: form.images.length > 0 ? form.images : null,
      tags: form.tags.length > 0 ? form.tags : null,
      itemType: 0, // Regular item
      sortOrder: 0,
      availableInEventMode: true,
      eventModeOnly: false,
      variants: form.variants.length > 0 ? form.variants.map(v => ({
        title: v.title,
        price: v.price,
        cost: v.cost || null,
        sizeValue: v.sizeValue || null,
        sizeUnit: v.sizeUnit || null,
        unitOfMeasure: v.unitOfMeasure || null,
        sku: v.sku || null,
        barcode: v.barcode || null,
        isDefault: v.isDefault || false,
        weightG: v.weightG || null
      })) : [],
      optionGroupIds: form.optionGroupIds.length > 0
        ? form.optionGroupIds.map(id => Number(id)).filter(id => !isNaN(id))
        : [],
      extraGroupIds: form.extraGroupIds.length > 0
        ? form.extraGroupIds.map(id => Number(id)).filter(id => !isNaN(id))
        : []
    }

    console.log('Saving item with data:', data)

    if (props.item) {
      await libraryItemsApi.updateItem(props.item.id, data)
      toast.success('Item updated successfully')
    } else {
      await libraryItemsApi.createItem(data)
      toast.success('Item created successfully')
    }

    emit('saved')
  } catch (error: any) {
    console.error('Error saving item:', error)
    console.error('Error response:', error.response?.data)
    toast.error(error.response?.data?.title || error.message || 'Failed to save item')
  } finally {
    saving.value = false
  }
}
</script>





<template>
  <BaseModal
    v-model="isOpen"
    title="Copy Item to Another Catalog"
    subtitle="Create a copy of this item in a different catalog"
    size="md"
    :confirm-loading="copying"
    :confirm-disabled="!selectedCatalogId"
    confirm-text="Copy Item"
    @close="handleClose"
    @confirm="copyItem"
  >
    <div class="space-y-4">
      <!-- Item Info -->
      <div class="bg-gray-50 border border-gray-200 rounded-lg p-4">
        <div class="flex items-center gap-3">
          <div v-if="item.images && item.images.length > 0" class="w-16 h-16 rounded-lg overflow-hidden flex-shrink-0">
            <img :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
          </div>
          <div class="flex-1">
            <h4 class="font-semibold text-md-on-surface">{{ item.name }}</h4>
            <p class="text-sm text-md-on-surface-variant">R{{ item.effectivePrice || item.price }}</p>
            <p v-if="item.description" class="text-xs text-md-on-surface-variant mt-1 line-clamp-1">
              {{ item.description }}
            </p>
          </div>
        </div>
      </div>

      <!-- Target Catalog Selection -->
      <div>
        <label class="block text-sm font-medium text-md-on-surface mb-1">
          Select Target Catalog <span class="text-red-500">*</span>
        </label>
        <select
          v-model="selectedCatalogId"
          class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
        >
          <option :value="null">Choose a catalog...</option>
          <option v-for="catalog in availableCatalogs" :key="catalog.id" :value="catalog.id">
            {{ catalog.name }}
          </option>
        </select>
        <p class="text-xs text-md-on-surface-variant mt-1">
          The item will be copied as a new master item (not a reference)
        </p>
      </div>

      <!-- Copy Options -->
      <div class="space-y-2">
        <label class="block text-sm font-medium text-md-on-surface mb-2">
          Copy Options
        </label>
        <label class="flex items-center gap-2">
          <input
            v-model="copyImages"
            type="checkbox"
            class="w-4 h-4 text-[var(--primary-color)] focus:ring-2 focus:ring-[var(--primary-color)] rounded"
          />
          <span class="text-sm text-md-on-surface">Copy images</span>
        </label>
        <label class="flex items-center gap-2">
          <input
            v-model="copyTags"
            type="checkbox"
            class="w-4 h-4 text-[var(--primary-color)] focus:ring-2 focus:ring-[var(--primary-color)] rounded"
          />
          <span class="text-sm text-md-on-surface">Copy tags (allergens, dietary info)</span>
        </label>
        <label class="flex items-center gap-2">
          <input
            v-model="copyVariants"
            type="checkbox"
            class="w-4 h-4 text-[var(--primary-color)] focus:ring-2 focus:ring-[var(--primary-color)] rounded"
          />
          <span class="text-sm text-md-on-surface">Copy variants (sizes, options)</span>
        </label>
      </div>

      <!-- Info Box -->
      <div class="bg-blue-50 border border-blue-200 rounded-lg p-3 flex gap-2">
        <i class="fas fa-info-circle text-blue-600 mt-0.5"></i>
        <div class="text-sm text-blue-900">
          <p class="font-medium mb-1">This will create a new independent item</p>
          <p class="text-xs text-blue-700">
            Changes to the original item will not affect the copy, and vice versa.
            If you want to share an item across catalogs with synchronized updates,
            use the "Reference Item" option when creating a new item instead.
          </p>
        </div>
      </div>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import BaseModal from './BaseModal.vue'
import { useToast } from '~/composables/useToast'

const props = defineProps<{
  modelValue: boolean
  item: any
  entityId: number
  currentCatalogId: number
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'close'): void
  (e: 'copied'): void
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const toast = useToast()

const selectedCatalogId = ref<number | null>(null)
const copying = ref(false)
const copyImages = ref(true)
const copyTags = ref(true)
const copyVariants = ref(true)
const availableCatalogs = ref<any[]>([])

// Load available catalogs when modal opens
watch(() => props.modelValue, async (newVal) => {
  if (newVal) {
    await loadAvailableCatalogs()
  }
})

async function loadAvailableCatalogs() {
  try {
    const api = useApi()
    const response = await api.get(`/api/v1/entities/${props.entityId}/catalogs`)

    if (response.success && response.data?.catalogs) {
      // Filter out the current catalog
      availableCatalogs.value = response.data.catalogs.filter(
        (c: any) => c.id !== props.currentCatalogId && c.isActive
      )
    }
  } catch (error) {
    console.error('Error loading catalogs:', error)
    toast.error('Failed to load available catalogs')
  }
}

async function copyItem() {
  if (!selectedCatalogId.value) return

  try {
    copying.value = true
    const api = useApi()

    const response = await api.post(
      `/api/v1/catalog-items/${props.item.id}/copy-to-catalog`,
      {
        targetCatalogId: selectedCatalogId.value,
        copyImages: copyImages.value,
        copyTags: copyTags.value,
        copyVariants: copyVariants.value
      }
    )

    if (response.success) {
      const targetCatalog = availableCatalogs.value.find(c => c.id === selectedCatalogId.value)
      toast.success(`Item copied to "${targetCatalog?.name || 'catalog'}" successfully`)
      emit('copied')
      handleClose()
    } else {
      toast.error(response.error || 'Failed to copy item')
    }
  } catch (error: any) {
    console.error('Error copying item:', error)
    toast.error(error.response?.data?.error || 'Failed to copy item')
  } finally {
    copying.value = false
  }
}

function handleClose() {
  selectedCatalogId.value = null
  copyImages.value = true
  copyTags.value = true
  copyVariants.value = true
  emit('close')
  emit('update:modelValue', false)
}
</script>

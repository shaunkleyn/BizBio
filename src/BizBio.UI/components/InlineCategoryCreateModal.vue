<template>
  <BaseModal
    v-model="isOpen"
    title="Create New Category"
    subtitle="Quick category creation"
    size="sm"
    :confirm-loading="saving"
    :confirm-disabled="!form.name"
    confirm-text="Create"
    @close="handleClose"
    @confirm="createCategory"
  >
    <div class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-md-on-surface mb-1">
          Category Name <span class="text-red-500">*</span>
        </label>
        <input
          v-model="form.name"
          ref="nameInput"
          type="text"
          required
          placeholder="e.g., Appetizers, Drinks, Desserts"
          class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
          @keydown.enter.prevent="createCategory"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-md-on-surface mb-1">
          Description (Optional)
        </label>
        <textarea
          v-model="form.description"
          rows="2"
          placeholder="Describe this category..."
          class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] resize-none"
        ></textarea>
      </div>

      <div>
        <label class="block text-sm font-medium text-md-on-surface mb-1">
          Icon (Optional)
        </label>
        <input
          v-model="form.icon"
          type="text"
          placeholder="e.g., fa-pizza-slice"
          class="w-full px-4 py-2 border border-md-outline rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
        />
        <p class="text-xs text-md-on-surface-variant mt-1">
          Font Awesome icon class
        </p>
      </div>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, nextTick } from 'vue'
import BaseModal from './BaseModal.vue'
import { useCategoryApi } from '~/composables/useCategoryApi'
import { useToast } from '~/composables/useToast'

const props = defineProps<{
  modelValue: boolean
  entityId: number
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'close'): void
  (e: 'created', category: any): void
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const categoryApi = useCategoryApi()
const toast = useToast()

const saving = ref(false)
const nameInput = ref<HTMLInputElement | null>(null)

const form = reactive({
  name: '',
  description: '',
  icon: ''
})

// Focus the name input when modal opens
watch(() => props.modelValue, async (newVal) => {
  if (newVal) {
    await nextTick()
    nameInput.value?.focus()
  }
})

const createCategory = async () => {
  if (!form.name.trim()) return

  try {
    saving.value = true
    const response = await categoryApi.createCategory({
      entityId: props.entityId,
      name: form.name.trim(),
      description: form.description.trim() || null,
      icon: form.icon.trim() || null,
      slug: form.name.toLowerCase().replace(/\s+/g, '-')
    })

    if (response.success) {
      toast.success('Category created successfully')
      emit('created', response.data.category)
      handleClose()
      resetForm()
    } else {
      toast.error(response.error || 'Failed to create category')
    }
  } catch (error: any) {
    console.error('Error creating category:', error)
    toast.error(error.response?.data?.error || 'Failed to create category')
  } finally {
    saving.value = false
  }
}

const handleClose = () => {
  emit('close')
  emit('update:modelValue', false)
}

const resetForm = () => {
  form.name = ''
  form.description = ''
  form.icon = ''
}
</script>

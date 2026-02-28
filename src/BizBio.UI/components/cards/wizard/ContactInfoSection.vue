<template>
  <div class="space-y-3">
    <div ref="listEl" class="space-y-2">
      <div
        v-for="item in state.contactInfo"
        :key="item.id"
        class="flex items-center gap-2 p-3 rounded-lg border border-md-outline-variant bg-md-surface group"
        :data-id="item.id"
      >
        <span class="drag-handle cursor-grab text-md-on-surface-variant hover:text-md-on-surface p-1 flex-shrink-0">
          <i class="fas fa-grip-vertical text-xs"></i>
        </span>

        <!-- Icon for type -->
        <div class="w-7 h-7 rounded-lg bg-md-surface-container flex items-center justify-center flex-shrink-0">
          <i :class="getTypeIcon(item.type)" class="text-md-primary text-xs"></i>
        </div>

        <!-- Type selector -->
        <select
          v-model="item.type"
          class="text-xs px-2 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface focus:outline-none focus:border-md-secondary flex-shrink-0"
        >
          <option v-for="t in infoTypes" :key="t.value" :value="t.value">{{ t.label }}</option>
        </select>

        <!-- Label -->
        <input
          v-model="item.label"
          type="text"
          placeholder="Label"
          class="w-24 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface flex-shrink-0"
        />

        <!-- Value -->
        <input
          v-model="item.value"
          :type="getInfoInputType(item.type)"
          :placeholder="getInfoPlaceholder(item.type)"
          class="flex-1 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface min-w-0"
        />

        <button
          class="text-md-error opacity-0 group-hover:opacity-100 transition-opacity p-1 hover:bg-md-error-container rounded flex-shrink-0"
          @click="removeContactInfo(item.id)"
        >
          <i class="fas fa-trash-alt text-xs"></i>
        </button>
      </div>
    </div>

    <button
      class="w-full py-2 rounded-lg border border-dashed transition-all text-sm font-medium flex items-center justify-center gap-2"
      :class="canAdd
        ? 'border-md-secondary text-md-secondary hover:bg-md-secondary-container cursor-pointer'
        : 'border-md-outline-variant text-md-on-surface-variant cursor-not-allowed opacity-60'"
      :disabled="!canAdd"
      @click="canAdd && addContactInfo()"
    >
      <i class="fas fa-plus text-xs"></i>
      {{ canAdd ? 'Add Contact Item' : `Max ${maxContactInfo} items on ${state.selectedPlan} plan` }}
    </button>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
  maxContactInfo: number
}>()

const { addContactInfo, removeContactInfo } = useBusinessCardCreation()

const canAdd = computed(() => props.state.contactInfo.length < props.maxContactInfo)

const infoTypes = [
  { value: 'phone', label: 'Phone' },
  { value: 'email', label: 'Email' },
  { value: 'website', label: 'Website' },
  { value: 'address', label: 'Address' },
  { value: 'linkedin', label: 'LinkedIn' },
  { value: 'whatsapp', label: 'WhatsApp' },
  { value: 'birthday', label: 'Birthday' },
  { value: 'custom', label: 'Custom' },
]

const getTypeIcon = (type: string): string => {
  const icons: Record<string, string> = {
    phone: 'fas fa-phone',
    email: 'fas fa-envelope',
    website: 'fas fa-globe',
    address: 'fas fa-map-marker-alt',
    linkedin: 'fab fa-linkedin',
    whatsapp: 'fab fa-whatsapp',
    birthday: 'fas fa-birthday-cake',
    custom: 'fas fa-info-circle',
  }
  return icons[type] ?? 'fas fa-info-circle'
}

const getInfoInputType = (type: string) => {
  if (type === 'email') return 'email'
  if (type === 'phone' || type === 'whatsapp') return 'tel'
  if (type === 'birthday') return 'date'
  return 'text'
}

const getInfoPlaceholder = (type: string): string => {
  const map: Record<string, string> = {
    phone: '+27 82 000 0000',
    email: 'you@example.com',
    website: 'https://yoursite.com',
    address: '123 Main St, Cape Town',
    linkedin: 'https://linkedin.com/in/...',
    whatsapp: '+27 82 000 0000',
    birthday: '',
    custom: 'Value',
  }
  return map[type] ?? ''
}

// Drag and drop
const listEl = ref<HTMLElement>()
const { enableSortable } = useDragDrop()

onMounted(() => {
  if (listEl.value) {
    enableSortable(listEl.value, {
      onEnd: (evt) => {
        const items = [...props.state.contactInfo]
        const [moved] = items.splice(evt.oldIndex!, 1)
        items.splice(evt.newIndex!, 0, moved)
        props.state.contactInfo = items
      }
    })
  }
})
</script>

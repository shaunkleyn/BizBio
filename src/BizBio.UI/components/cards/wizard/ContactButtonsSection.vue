<template>
  <div class="space-y-3">
    <div ref="listEl" class="space-y-2">
      <div
        v-for="btn in state.contactButtons"
        :key="btn.id"
        class="flex items-center gap-2 p-3 rounded-lg border border-md-outline-variant bg-md-surface group"
        :data-id="btn.id"
      >
        <span class="drag-handle cursor-grab text-md-on-surface-variant hover:text-md-on-surface p-1">
          <i class="fas fa-grip-vertical text-xs"></i>
        </span>

        <!-- Type selector -->
        <select
          v-model="btn.type"
          class="text-xs px-2 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface focus:outline-none focus:border-md-secondary flex-shrink-0"
          @change="updateButtonHref(btn)"
        >
          <option v-for="t in buttonTypes" :key="t.value" :value="t.value">{{ t.label }}</option>
        </select>

        <!-- Value input -->
        <input
          v-model="btn.value"
          :type="getInputType(btn.type)"
          :placeholder="getPlaceholder(btn.type)"
          class="flex-1 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface min-w-0"
          @input="updateButtonHref(btn)"
        />

        <!-- Custom label -->
        <input
          v-model="btn.label"
          type="text"
          placeholder="Label"
          class="w-20 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface"
        />

        <button
          class="text-md-error opacity-0 group-hover:opacity-100 transition-opacity p-1 hover:bg-md-error-container rounded"
          @click="removeContactButton(btn.id)"
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
      @click="canAdd && addContactButton()"
    >
      <i class="fas fa-plus text-xs"></i>
      {{ canAdd ? 'Add Button' : `Max ${maxContactButtons} buttons on ${state.selectedPlan} plan` }}
    </button>
  </div>
</template>

<script setup lang="ts">
import type { WizardState, ContactButton } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
  maxContactButtons: number
}>()

const { addContactButton, removeContactButton } = useBusinessCardCreation()

const canAdd = computed(() => props.state.contactButtons.length < props.maxContactButtons)

const buttonTypes = [
  { value: 'phone', label: 'Phone' },
  { value: 'email', label: 'Email' },
  { value: 'whatsapp', label: 'WhatsApp' },
  { value: 'sms', label: 'SMS' },
  { value: 'facebook', label: 'Messenger' },
  { value: 'telegram', label: 'Telegram' },
  { value: 'linkedin', label: 'LinkedIn' },
  { value: 'instagram', label: 'Instagram' },
  { value: 'twitter', label: 'Twitter/X' },
  { value: 'skype', label: 'Skype' },
  { value: 'zoom', label: 'Zoom' },
  { value: 'calendar', label: 'Calendar' },
  { value: 'website', label: 'Website' },
  { value: 'custom', label: 'Custom URL' },
]

const getInputType = (type: string) => {
  if (type === 'email') return 'email'
  if (type === 'phone' || type === 'whatsapp' || type === 'sms') return 'tel'
  return 'url'
}

const getPlaceholder = (type: string): string => {
  const map: Record<string, string> = {
    phone: '+27 82 000 0000',
    email: 'you@example.com',
    whatsapp: '+27 82 000 0000',
    sms: '+27 82 000 0000',
    facebook: 'https://m.me/...',
    telegram: 'https://t.me/...',
    linkedin: 'https://linkedin.com/in/...',
    instagram: 'https://instagram.com/...',
    twitter: 'https://twitter.com/...',
    skype: 'skype:username',
    zoom: 'https://zoom.us/j/...',
    calendar: 'https://calendly.com/...',
    website: 'https://yoursite.com',
    custom: 'https://...',
  }
  return map[type] ?? 'https://...'
}

const updateButtonHref = (btn: ContactButton) => {
  const prefixMap: Record<string, string> = {
    phone: 'tel:',
    email: 'mailto:',
    whatsapp: 'https://wa.me/',
    sms: 'sms:',
  }
  const prefix = prefixMap[btn.type]
  if (prefix && btn.value && !btn.value.startsWith('http')) {
    // Keep raw value, href is used only in template population
  }
}

// Drag and drop
const listEl = ref<HTMLElement>()
const { enableSortable } = useDragDrop()

onMounted(() => {
  if (listEl.value) {
    enableSortable(listEl.value, {
      onEnd: (evt) => {
        const buttons = [...props.state.contactButtons]
        const [moved] = buttons.splice(evt.oldIndex!, 1)
        buttons.splice(evt.newIndex!, 0, moved)
        props.state.contactButtons = buttons
      }
    })
  }
})
</script>

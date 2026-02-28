<template>
  <div class="flex h-full">
    <!-- Left: Tab bar + content (scrollable) -->
    <div class="flex-1 flex flex-col overflow-hidden min-h-0" style="min-width: 0;">
      <!-- Tab bar -->
      <div class="bg-md-surface border-b border-md-outline-variant sticky top-0 z-20">
        <div class="flex">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            class="flex items-center gap-2 px-5 py-3.5 text-sm font-medium border-b-2 transition-all"
            :class="activeTab === tab.id
              ? 'border-md-secondary text-md-secondary'
              : 'border-transparent text-md-on-surface-variant hover:text-md-on-surface hover:border-md-outline'"
            @click="activeTab = tab.id as any"
          >
            <i :class="tab.icon" class="text-xs"></i>
            {{ tab.label }}
          </button>
        </div>
      </div>

      <!-- Tab content (scrollable) -->
      <div class="flex-1 overflow-y-auto">
        <TemplateTab v-if="activeTab === 'template'" :state="state" />
        <ContentTab
          v-else-if="activeTab === 'content'"
          :state="state"
          :max-contact-buttons="maxContactButtons"
          :max-contact-info="maxContactInfo"
          :max-social-links="maxSocialLinks"
        />
        <AppearanceTab v-else-if="activeTab === 'appearance'" :state="state" />
      </div>
    </div>

    <!-- Right: Phone preview -->
    <div
      class="hidden lg:flex flex-col bg-md-surface-container border-l border-md-outline-variant overflow-y-auto py-8 px-6 flex-shrink-0"
      style="width: 340px;"
    >
      <CardPhonePreview :state="state" />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'
import TemplateTab from './TemplateTab.vue'
import ContentTab from './ContentTab.vue'
import AppearanceTab from './AppearanceTab.vue'
import CardPhonePreview from './CardPhonePreview.vue'

const props = defineProps<{
  state: WizardState
  maxContactButtons: number
  maxContactInfo: number
  maxSocialLinks: number
}>()

const activeTab = computed({
  get: () => props.state.activeTab,
  set: (val) => { props.state.activeTab = val }
})

const tabs = [
  { id: 'template', label: 'Template', icon: 'fas fa-palette' },
  { id: 'content', label: 'Content', icon: 'fas fa-edit' },
  { id: 'appearance', label: 'Appearance', icon: 'fas fa-sliders-h' },
]
</script>

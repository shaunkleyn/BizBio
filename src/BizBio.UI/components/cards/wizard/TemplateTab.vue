<template>
  <div class="p-4">
    <p class="text-xs text-md-on-surface-variant mb-4">
      Choose a template for your digital business card.
      <span v-if="state.selectedPlan === 'free'" class="text-md-secondary font-medium">
        Free plan includes 5 templates. Upgrade to unlock all.
      </span>
    </p>

    <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
      <div
        v-for="template in ALL_TEMPLATES"
        :key="template.name"
        class="relative rounded-xl overflow-hidden cursor-pointer border-2 transition-all group"
        :class="[
          state.selectedTemplate === template.name
            ? 'border-md-secondary shadow-md-3'
            : 'border-md-outline-variant hover:border-md-secondary',
          !canUseTemplate(template.name) ? 'opacity-70' : ''
        ]"
        @click="handleTemplateClick(template.name)"
      >
        <!-- Template preview image -->
        <div class="aspect-[9/16] bg-md-surface-container-highest relative overflow-hidden">
          <img
            :src="`/templates/previews/${template.name}.jpg`"
            :alt="template.label"
            class="w-full h-full object-cover"
            loading="lazy"
            @load="imgErrors[template.name] = false"
            @error="imgErrors[template.name] = true"
          />

          <!-- Fallback gradient preview (shown when image errors or hasn't loaded) -->
          <div
            v-show="imgErrors[template.name]"
            class="absolute inset-0 flex items-center justify-center"
            :style="getPreviewStyle(template.name)"
          >
            <div class="text-center p-3">
              <div class="w-8 h-8 rounded-full bg-white/20 mx-auto mb-2 flex items-center justify-center">
                <i class="fas fa-id-card text-white text-sm"></i>
              </div>
              <div class="text-white text-[10px] font-medium opacity-80">{{ template.label }}</div>
            </div>
          </div>

          <!-- Lock overlay for premium templates on free plan -->
          <div
            v-if="!canUseTemplate(template.name)"
            class="absolute inset-0 bg-black/50 flex items-center justify-center"
          >
            <div class="text-center">
              <div class="w-8 h-8 rounded-full bg-white/20 mx-auto mb-1 flex items-center justify-center">
                <i class="fas fa-lock text-white text-sm"></i>
              </div>
              <span class="text-white text-[9px] font-semibold">Solo plan</span>
            </div>
          </div>

          <!-- Selected checkmark -->
          <div
            v-if="state.selectedTemplate === template.name"
            class="absolute top-2 right-2 w-6 h-6 bg-md-secondary rounded-full flex items-center justify-center shadow-md-2"
          >
            <i class="fas fa-check text-white text-xs"></i>
          </div>
        </div>

        <!-- Template name -->
        <div class="p-2 bg-md-surface">
          <p class="text-[11px] font-medium text-md-on-surface text-center leading-tight">
            {{ template.label }}
          </p>
        </div>
      </div>
    </div>

    <!-- Upgrade nudge for free plan -->
    <div v-if="state.selectedPlan === 'free'" class="mt-4 p-3 rounded-xl bg-md-secondary-container border border-md-secondary/20">
      <div class="flex items-center gap-2 text-sm">
        <i class="fas fa-crown text-md-secondary"></i>
        <span class="text-md-on-surface font-medium">Unlock {{ ALL_TEMPLATES.length - FREE_TEMPLATES.length }} more templates</span>
      </div>
      <p class="text-xs text-md-on-surface-variant mt-1 ml-5">Upgrade to Solo plan from R59/mo</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ALL_TEMPLATES, FREE_TEMPLATES, TEMPLATE_DEFAULTS } from '~/composables/useBusinessCardCreation'
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
}>()

const { canUseTemplate } = useBusinessCardCreation()

// Track which template images errored so we can show the fallback
const imgErrors = reactive<Record<string, boolean>>(
  Object.fromEntries(ALL_TEMPLATES.map(t => [t.name, true]))
)

/** Build the fallback gradient from TEMPLATE_DEFAULTS or use a generic gradient */
const getPreviewStyle = (name: string): string => {
  const def = TEMPLATE_DEFAULTS[name]
  if (!def) return 'background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'
  if (def.bgType === 'gradient' && def.bgGradientFrom && def.bgGradientTo) {
    return `background: linear-gradient(${def.bgGradientDir ?? '135deg'}, ${def.bgGradientFrom}, ${def.bgGradientTo});`
  }
  return `background: ${def.bgColor};`
}

const handleTemplateClick = (name: string) => {
  if (!canUseTemplate(name)) return
  props.state.selectedTemplate = name

  // Apply template default appearance settings
  const def = TEMPLATE_DEFAULTS[name]
  if (def) {
    props.state.appearance.primaryColor = def.primaryColor
    props.state.appearance.bgType = def.bgType
    props.state.appearance.bgColor = def.bgColor
    props.state.appearance.cardBgColor = def.cardBgColor
    props.state.appearance.titleColor = def.titleColor
    props.state.appearance.subtitleColor = def.subtitleColor
    props.state.appearance.bodyTextColor = def.bodyTextColor
    props.state.appearance.infoLabelColor = def.infoLabelColor
    props.state.appearance.buttonTextColor = def.buttonTextColor
    props.state.appearance.headingFont = def.headingFont
    props.state.appearance.bodyFont = def.bodyFont
    if (def.bgType === 'gradient') {
      props.state.appearance.bgGradientFrom = def.bgGradientFrom ?? def.bgColor
      props.state.appearance.bgGradientTo = def.bgGradientTo ?? def.bgColor
      props.state.appearance.bgGradientDir = def.bgGradientDir ?? '135deg'
    }
  }
}
</script>

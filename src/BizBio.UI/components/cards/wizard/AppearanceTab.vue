<template>
  <div class="p-3 space-y-2">

    <!-- ── Background ──────────────────────────── -->
    <AppearancePanel title="Background" icon="fas fa-fill-drip" :initial-open="true">
      <div class="space-y-3">
        <!-- BG type toggle -->
        <div class="grid grid-cols-4 gap-1 p-1 bg-md-surface-container rounded-lg">
          <button
            v-for="type in bgTypes"
            :key="type.value"
            class="py-1.5 rounded-md text-xs font-medium transition-all"
            :class="appearance.bgType === type.value
              ? 'bg-md-surface text-md-on-surface shadow-md-1'
              : 'text-md-on-surface-variant hover:text-md-on-surface'"
            @click="appearance.bgType = type.value as any"
          >
            {{ type.label }}
          </button>
        </div>

        <!-- Solid -->
        <div v-if="appearance.bgType === 'solid'">
          <ColorRow label="Color" v-model="appearance.bgColor" />
        </div>

        <!-- Gradient -->
        <div v-else-if="appearance.bgType === 'gradient'" class="space-y-2">
          <ColorRow label="From" v-model="appearance.bgGradientFrom" />
          <ColorRow label="To" v-model="appearance.bgGradientTo" />
          <div class="flex items-center gap-3">
            <label class="text-xs text-md-on-surface-variant w-28 flex-shrink-0">Angle</label>
            <input v-model="appearance.bgGradientDir" type="text" placeholder="135deg"
              class="flex-1 text-xs px-2 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface" />
          </div>
          <div class="h-8 rounded-lg"
            :style="{ background: `linear-gradient(${appearance.bgGradientDir}, ${appearance.bgGradientFrom}, ${appearance.bgGradientTo})` }">
          </div>
        </div>

        <!-- Image -->
        <div v-else-if="appearance.bgType === 'image'">
          <input v-model="appearance.bgImageUrl" type="url" placeholder="https://example.com/bg.jpg"
            class="w-full text-xs px-3 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface" />
        </div>

        <!-- Pattern -->
        <div v-else-if="appearance.bgType === 'pattern'" class="space-y-2">
          <ColorRow label="Base color" v-model="appearance.bgColor" />
          <label class="text-xs text-md-on-surface-variant block">Pattern style</label>
          <div class="grid grid-cols-4 gap-1.5">
            <button
              v-for="pat in CARD_PATTERNS"
              :key="pat.id"
              class="relative h-12 rounded-lg border-2 overflow-hidden transition-all"
              :class="appearance.bgPattern === pat.id
                ? 'border-md-secondary shadow-md-2'
                : 'border-md-outline-variant hover:border-md-secondary'"
              :style="{ background: getPatternPreviewCss(pat.id, appearance.bgColor, appearance.primaryColor) }"
              @click="appearance.bgPattern = pat.id"
            >
              <span class="absolute inset-x-0 bottom-0 bg-black/30 text-white text-[9px] text-center py-0.5 font-medium">{{ pat.label }}</span>
            </button>
          </div>
        </div>
      </div>
    </AppearancePanel>

    <!-- ── Card ──────────────────────────────────── -->
    <AppearancePanel title="Card" icon="fas fa-layer-group" :initial-open="false">
      <div class="space-y-2">
        <ColorRow label="Card background" v-model="appearance.cardBgColor" />
        <div class="flex items-center gap-3">
          <label class="text-xs text-md-on-surface-variant flex-1">Corner radius ({{ appearance.cardBorderRadius }}px)</label>
        </div>
        <input v-model.number="appearance.cardBorderRadius" type="range" min="0" max="40" class="w-full accent-md-secondary" />
        <div class="flex items-center justify-between">
          <label class="text-xs text-md-on-surface-variant">Shadow</label>
          <ToggleSwitch v-model="appearance.cardShadowEnabled" />
        </div>
        <div class="flex items-center justify-between">
          <label class="text-xs text-md-on-surface-variant">Border</label>
          <ToggleSwitch v-model="appearance.cardBorderEnabled" />
        </div>
        <div v-if="appearance.cardBorderEnabled">
          <ColorRow label="Border color" v-model="appearance.cardBorderColor" />
        </div>
      </div>
    </AppearancePanel>

    <!-- ── Typography ────────────────────────────── -->
    <AppearancePanel title="Typography" icon="fas fa-font" :initial-open="true">
      <div class="space-y-2">
        <div class="flex items-center gap-3">
          <label class="text-xs text-md-on-surface-variant w-28 flex-shrink-0">Heading font</label>
          <FontPicker v-model="appearance.headingFont" :fonts="ALL_FONTS" />
        </div>
        <div class="flex items-center gap-3">
          <label class="text-xs text-md-on-surface-variant w-28 flex-shrink-0">Body font</label>
          <FontPicker v-model="appearance.bodyFont" :fonts="ALL_FONTS" />
        </div>
        <ColorRow label="Title color" v-model="appearance.titleColor" />
        <ColorRow label="Subtitle color" v-model="appearance.subtitleColor" />
        <ColorRow label="Body text" v-model="appearance.bodyTextColor" />
        <ColorRow label="Label color" v-model="appearance.infoLabelColor" />
      </div>
    </AppearancePanel>

    <!-- ── Buttons & Icons ───────────────────────── -->
    <AppearancePanel title="Buttons & Icons" icon="fas fa-hand-pointer" :initial-open="false">
      <div class="space-y-3">
        <ColorRow label="Primary color" v-model="appearance.primaryColor" />
        <ColorRow label="Button text" v-model="appearance.buttonTextColor" />

        <!-- Button style visual preview -->
        <div>
          <label class="text-xs text-md-on-surface-variant block mb-2">Button style</label>
          <div class="grid grid-cols-2 gap-1.5">
            <button
              v-for="style in buttonStyles"
              :key="style.value"
              class="flex flex-col items-center gap-1.5 p-2 rounded-lg border-2 transition-all"
              :class="appearance.buttonStyle === style.value
                ? 'border-md-secondary bg-md-secondary-container'
                : 'border-md-outline-variant hover:border-md-secondary'"
              @click="appearance.buttonStyle = style.value as any"
            >
              <div class="w-full flex items-center justify-center py-1 text-[11px] font-semibold"
                :style="buttonPreviewStyle(style.value)">
                Label
              </div>
              <span class="text-[10px] text-md-on-surface-variant">{{ style.label }}</span>
            </button>
          </div>
        </div>

        <!-- Icon style visual preview -->
        <div>
          <label class="text-xs text-md-on-surface-variant block mb-2">Icon button shape</label>
          <div class="grid grid-cols-4 gap-1.5">
            <button
              v-for="style in iconStyles"
              :key="style.value"
              class="flex flex-col items-center gap-1.5 p-2 rounded-lg border-2 transition-all"
              :class="appearance.iconStyle === style.value
                ? 'border-md-secondary bg-md-secondary-container'
                : 'border-md-outline-variant hover:border-md-secondary'"
              @click="appearance.iconStyle = style.value as any"
            >
              <div class="w-8 h-8 flex items-center justify-center"
                :style="iconPreviewStyle(style.value)">
                <i class="fas fa-phone text-[11px]"></i>
              </div>
              <span class="text-[9px] text-md-on-surface-variant font-medium">{{ style.label }}</span>
            </button>
          </div>
        </div>
      </div>
    </AppearancePanel>

    <!-- ── Wallet Branding ───────────────────────── -->
    <AppearancePanel
      v-if="hasWalletSections"
      title="Wallet Branding"
      icon="fas fa-wallet"
      :initial-open="false"
    >
      <div class="space-y-2">
        <ColorRow label="Google Wallet BG" v-model="appearance.walletBgColor" />
        <div>
          <label class="block text-xs text-md-on-surface-variant mb-1">Issuer name</label>
          <input v-model="appearance.walletIssuerName" type="text" placeholder="Your Company Name"
            class="w-full text-xs px-3 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface" />
        </div>
        <div>
          <label class="block text-xs text-md-on-surface-variant mb-1">Hero image URL</label>
          <input v-model="appearance.walletHeroImageUrl" type="url" placeholder="https://…"
            class="w-full text-xs px-3 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface" />
        </div>
        <ColorRow label="Apple Wallet BG" v-model="appearance.appleWalletBgColor" />
      </div>
    </AppearancePanel>

  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'
import { CARD_PATTERNS } from '~/composables/useBusinessCardCreation'
import { getPatternPreviewCss } from '~/composables/useTemplatePopulator'
import AppearancePanel from './AppearancePanel.vue'
import ColorRow from './ColorRow.vue'
import ToggleSwitch from './ToggleSwitch.vue'
import FontPicker from './FontPicker.vue'

const props = defineProps<{
  state: WizardState
}>()

const appearance = computed(() => props.state.appearance)

const hasWalletSections = computed(() =>
  props.state.sections.some(s => s.type === 'google-wallet' || s.type === 'apple-wallet')
)

const bgTypes = [
  { value: 'solid', label: 'Solid' },
  { value: 'gradient', label: 'Gradient' },
  { value: 'image', label: 'Image' },
  { value: 'pattern', label: 'Pattern' },
]

const buttonStyles = [
  { value: 'filled', label: 'Filled' },
  { value: 'outlined', label: 'Outlined' },
  { value: 'ghost', label: 'Ghost' },
  { value: 'pill', label: 'Pill' },
]

const iconStyles = [
  { value: 'circle', label: 'Circle' },
  { value: 'square', label: 'Square' },
  { value: 'rounded', label: 'Round' },
  { value: 'none', label: 'None' },
]

// All available Google Fonts — used as source list for FontPicker
const ALL_FONTS = [
  // Common / popular
  'Inter', 'Poppins', 'Montserrat', 'Raleway', 'Roboto',
  'Lato', 'Open Sans', 'Nunito', 'DM Sans', 'Outfit',
  // Serif / editorial
  'Playfair Display', 'Cormorant Garamond', 'Merriweather', 'Lora', 'EB Garamond',
  // Geometric / modern
  'Plus Jakarta Sans', 'Space Grotesk', 'Josefin Sans', 'Exo 2', 'Barlow',
  // Quirky / display
  'Quicksand', 'Pacifico', 'Dancing Script', 'Oswald', 'Ubuntu',
  // Specialty (used by specific templates)
  'Orbitron', 'Bebas Neue', 'PT Sans',
]

const buttonPreviewStyle = (style: string) => {
  const color = appearance.value.primaryColor
  const text = appearance.value.buttonTextColor || '#ffffff'
  const radiusMap: Record<string, string> = {
    filled: '8px', outlined: '8px', ghost: '8px', pill: '50px',
  }
  const r = radiusMap[style] ?? '8px'
  if (style === 'outlined') {
    return { background: 'transparent', border: `2px solid ${color}`, color, borderRadius: r, padding: '2px 8px' }
  }
  if (style === 'ghost') {
    return { background: 'transparent', border: 'none', color, borderRadius: r, padding: '2px 8px', boxShadow: 'none' }
  }
  return { background: color, color: text, border: 'none', borderRadius: r, padding: '2px 8px' }
}

const iconPreviewStyle = (style: string) => {
  const color = appearance.value.primaryColor
  const radiusMap: Record<string, string> = {
    circle: '50%', square: '4px', rounded: '10px', none: '0',
  }
  const r = radiusMap[style] ?? '50%'
  if (style === 'none') {
    return { background: 'transparent', color, borderRadius: r }
  }
  return { background: color, color: '#fff', borderRadius: r }
}
</script>

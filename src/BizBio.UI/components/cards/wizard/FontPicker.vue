<template>
  <div class="relative flex-1" ref="pickerEl">
    <!-- Trigger button — shows selected font in its own typeface -->
    <button
      type="button"
      class="w-full flex items-center justify-between px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface text-left"
      :style="{ fontFamily: `'${modelValue}', sans-serif` }"
      @click="open = !open"
    >
      <span class="truncate">{{ modelValue }}</span>
      <i class="fas fa-chevron-down text-[9px] text-md-on-surface-variant ml-1 flex-shrink-0 transition-transform"
        :class="open ? 'rotate-180' : ''" />
    </button>

    <!-- Dropdown list -->
    <div
      v-if="open"
      class="absolute z-30 left-0 right-0 mt-1 bg-md-surface border border-md-outline-variant rounded-lg shadow-md-4 overflow-y-auto"
      style="max-height: 220px; top: 100%;"
    >
      <button
        v-for="font in fonts"
        :key="font"
        type="button"
        class="flex w-full items-center gap-2 px-3 py-1.5 text-xs text-left transition-colors hover:bg-md-surface-container"
        :class="modelValue === font ? 'bg-md-secondary-container text-md-secondary font-semibold' : 'text-md-on-surface'"
        :style="{ fontFamily: `'${font}', sans-serif` }"
        @click="select(font)"
      >
        {{ font }}
        <i v-if="modelValue === font" class="fas fa-check text-[9px] ml-auto" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  modelValue: string
  fonts: string[]
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const open = ref(false)
const pickerEl = ref<HTMLElement>()

onMounted(() => {
  // Load all fonts once so they render in their own typeface
  const systemFonts = ['Segoe UI', 'system-ui', '-apple-system', 'Arial', 'sans-serif', 'serif']
  const googleFonts = props.fonts.filter(f => !systemFonts.includes(f))
  if (googleFonts.length && !document.getElementById('__fontpicker_fonts__')) {
    const link = document.createElement('link')
    link.id = '__fontpicker_fonts__'
    link.rel = 'stylesheet'
    const query = googleFonts.map(f => `family=${encodeURIComponent(f)}:wght@400;700`).join('&')
    link.href = `https://fonts.googleapis.com/css2?${query}&display=swap`
    document.head.appendChild(link)
  }
  document.addEventListener('click', handleOutsideClick)
})

onUnmounted(() => {
  document.removeEventListener('click', handleOutsideClick)
})

const select = (font: string) => {
  emit('update:modelValue', font)
  open.value = false
}

const handleOutsideClick = (e: MouseEvent) => {
  if (pickerEl.value && !pickerEl.value.contains(e.target as Node)) {
    open.value = false
  }
}
</script>

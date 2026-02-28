<template>
  <div class="rounded-xl border border-md-outline-variant bg-md-surface overflow-hidden">
    <button
      class="w-full flex items-center gap-3 p-3 text-left"
      @click="open = !open"
    >
      <i :class="icon" class="text-md-secondary text-sm w-4 text-center flex-shrink-0"></i>
      <span class="flex-1 font-medium text-md-on-surface text-sm">{{ title }}</span>
      <i
        class="fas fa-chevron-down text-xs text-md-on-surface-variant transition-transform"
        :class="open ? 'rotate-180' : ''"
      ></i>
    </button>
    <div v-if="open" class="px-3 pb-3 border-t border-md-outline-variant pt-3">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  title: string
  icon: string
  initialOpen?: boolean
}>(), {
  initialOpen: false,
})

const open = ref(props.initialOpen)
</script>

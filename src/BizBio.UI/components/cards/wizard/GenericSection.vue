<template>
  <div class="rounded-xl border border-md-outline-variant bg-md-surface overflow-hidden">
    <!-- Section header -->
    <div class="flex items-center gap-3 p-4 cursor-pointer select-none" @click="collapsed = !collapsed">
      <span class="drag-handle cursor-grab text-md-on-surface-variant hover:text-md-on-surface p-1">
        <i class="fas fa-grip-vertical text-xs"></i>
      </span>
      <div class="flex-1 flex items-center gap-2 min-w-0">
        <i :class="icon" class="text-md-secondary text-sm flex-shrink-0"></i>
        <span class="font-medium text-md-on-surface text-sm">{{ title }}</span>
      </div>
      <button
        v-if="removable"
        class="text-md-error hover:bg-md-error-container p-1 rounded transition-colors"
        @click.stop="$emit('remove')"
      >
        <i class="fas fa-times text-xs"></i>
      </button>
      <i
        class="fas fa-chevron-down text-xs text-md-on-surface-variant transition-transform ml-1"
        :class="collapsed ? '' : 'rotate-180'"
      ></i>
    </div>

    <!-- Content slot -->
    <div v-if="!collapsed" class="px-4 pb-4 border-t border-md-outline-variant pt-3">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  title: string
  icon: string
  removable?: boolean
}>()

defineEmits<{
  remove: []
}>()

const collapsed = ref(false)
</script>

<template>
  <div class="flex items-center gap-3">
    <label class="text-xs text-md-on-surface-variant w-28 flex-shrink-0">{{ label }}</label>
    <input
      :value="modelValue"
      type="color"
      class="w-8 h-8 rounded cursor-pointer border-none flex-shrink-0"
      @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />
    <input
      :value="modelValue"
      type="text"
      class="flex-1 text-xs px-2 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface"
      @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />
  </div>
</template>

<script setup lang="ts">
defineProps<{
  label: string
  modelValue: string
}>()

defineEmits<{
  'update:modelValue': [value: string]
}>()
</script>

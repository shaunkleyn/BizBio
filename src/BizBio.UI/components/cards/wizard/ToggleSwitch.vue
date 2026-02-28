<template>
  <button
    class="relative w-10 h-6 rounded-full transition-all focus:outline-none"
    :class="modelValue ? 'bg-md-secondary' : 'bg-md-outline'"
    @click="$emit('update:modelValue', !modelValue)"
  >
    <div
      class="absolute top-1 w-4 h-4 bg-white rounded-full shadow transition-all"
      :class="modelValue ? 'left-5' : 'left-1'"
    ></div>
  </button>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: boolean
}>()

defineEmits<{
  'update:modelValue': [value: boolean]
}>()
</script>

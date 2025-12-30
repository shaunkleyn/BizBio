<template>
  <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
    <span class="text-sm text-md-on-surface">{{ option.name }}</span>
    <span 
      class="text-sm font-semibold"
      :class="option.price === 0 ? 'text-[#1E8E3E]' : 'text-md-on-surface'"
    >
      {{ formatPrice(option.price) }}
    </span>
  </div>
</template>

<script setup lang="ts">
interface Option {
  id: number
  name: string
  price: number
}

defineProps<{
  option: Option
}>()

const formatPrice = (price: number): string => {
  if (price === 0) return 'Free'
  return `+ R ${price.toFixed(2)}`
}
</script>

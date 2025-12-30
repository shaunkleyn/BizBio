<template>
  <div 
    class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3"
    draggable="true"
  >
    <i class="fas fa-grip-vertical text-gray-400"></i>
    
    <div 
      class="w-10 h-10 rounded-lg flex items-center justify-center"
      :style="{ backgroundColor: product.iconBg || '#FCE8E6' }"
    >
      <i 
        :class="product.icon || 'fas fa-pizza-slice'"
        :style="{ color: product.iconColor || '#C5221F' }"
      ></i>
    </div>
    
    <div class="flex-1">
      <p class="font-semibold text-md-on-surface text-sm">
        {{ product.name }}
      </p>
      <p v-if="product.description" class="text-xs text-gray-600">
        {{ product.description }}
      </p>
    </div>
    
    <button
      @click="$emit('remove')"
      class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center"
    >
      <i class="fas fa-times text-xs"></i>
    </button>
  </div>
</template>

<script setup lang="ts">
interface Product {
  id: number
  name: string
  description?: string
  icon?: string
  iconBg?: string
  iconColor?: string
}

defineProps<{
  product: Product
}>()

defineEmits(['remove', 'reorder'])
</script>

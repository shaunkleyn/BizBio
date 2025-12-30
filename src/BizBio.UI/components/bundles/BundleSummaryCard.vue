<template>
  <div 
    class="bg-gradient-to-br from-[#E6F4EA] to-[#E8F0FE] rounded-3xl p-6 mb-6"
    style="box-shadow:0 1px 3px rgba(0,0,0,0.12),0 1px 2px rgba(0,0,0,0.24)"
  >
    <div class="flex items-start gap-4">
      <div class="w-12 h-12 bg-md-surface rounded-2xl flex items-center justify-center flex-shrink-0">
        <i class="fas fa-check-circle text-[#1E8E3E] text-xl"></i>
      </div>
      <div class="flex-1">
        <h3 class="font-bold text-md-on-surface text-xl mb-2">Bundle Summary</h3>
        <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-4">
          <div>
            <p class="text-xs text-gray-600 mb-1">Base Price</p>
            <p class="font-bold text-md-on-surface text-lg">R {{ basePrice.toFixed(2) }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-600 mb-1">Steps</p>
            <p class="font-bold text-md-on-surface text-lg">{{ stepsCount }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-600 mb-1">Products</p>
            <p class="font-bold text-md-on-surface text-lg">{{ productsCount }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-600 mb-1">Option Groups</p>
            <p class="font-bold text-md-on-surface text-lg">{{ optionGroupsCount }}</p>
          </div>
        </div>
        <p class="text-sm text-md-on-surface">
          Customers will make {{ stepsCount }} choice{{ stepsCount !== 1 ? 's' : '' }}, 
          with customization options available for each selection
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  basePrice: number
  stepsCount: number
  productsCount: number
  optionGroupsCount: number
}>()
</script>

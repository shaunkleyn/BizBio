<template>
  <div 
    class="bg-md-surface-container border border-md-outline-variant rounded-2xl p-4 mb-4"
    role="allowed-products"
  >
    <div class="flex items-center justify-between">
      <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
        <i class="fas fa-check-circle text-[#1E8E3E]"></i>
        Products in this step ({{ products.length }})
      </h4>
      
      <!-- New Searchable Multi-Select Dropdown -->
      <ProductSelectorDropdown
        :available-products="availableProducts"
        :already-added-ids="products.map(p => p.id)"
        @add-products="handleAddProducts"
      />
    </div>
    <p class="text-sm text-gray-600 mb-3">Define the list of products the customer may choose from</p>
    <div class="space-y-2">
      <ProductItem
        v-for="(product, index) in products"
        :key="product.id"
        :product="product"
        @remove="$emit('remove-product', product.id)"
        @reorder="$emit('reorder-product', { from: index, to: $event })"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import ProductItem from './ProductItem.vue'
import ProductSelectorDropdown from './ProductSelectorDropdown.vue'

interface Product {
  id: number
  name: string
  description?: string
  icon?: string
  iconBg?: string
  iconColor?: string
}

const props = defineProps<{
  products: Product[]
  availableProducts: Product[]
}>()

const emit = defineEmits(['add-product', 'remove-product', 'reorder-product'])

const handleAddProducts = (productIds: number[]) => {
  // Emit each product ID individually
  productIds.forEach(productId => {
    emit('add-product', productId)
  })
}

const isProductAdded = (productId: number): boolean => {
  return props.products.some(p => p.id === productId)
}
</script>

<template>
  <div class="bg-md-surface-container-low border-b border-md-outline-variant sticky top-16 z-40 py-4 px-4 md:px-8 shadow-md-1">
    <!-- Page Header and Actions Section -->
    <div class="flex items-center justify-between flex-wrap gap-4">
      <!-- Page Title and Description (Left) -->
      <div class="flex-1 min-w-0">
        <h1 class="text-2xl md:text-3xl font-heading font-bold text-md-on-surface">{{ pageHeader.title }}</h1>
        <p class="text-sm md:text-base text-md-on-surface-variant mt-1 font-medium">{{ pageHeader.description }}</p>
      </div>

      <!-- Action Buttons (Right) -->
      <div class="flex items-center gap-2 flex-shrink-0">
        <component v-if="pageActions" :is="pageActions()" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// const route = useRoute()
// const dropdownOpen = ref(false)
// const dropdownRef = ref<HTMLElement | null>(null)

// Use page metadata composable
const { pageHeader, pageActions } = usePageMeta()

// const products = [
//   {
//     id: 'menu',
//     name: 'Menu',
//     subtitle: 'Digital Menus',
//     description: 'Create and manage digital menus',
//     icon: 'fas fa-utensils',
//     path: '/menu'
//   },
//   {
//     id: 'cards',
//     name: 'Business Cards',
//     subtitle: 'Digital Cards',
//     description: 'Create digital business cards',
//     icon: 'fas fa-id-card',
//     path: '/cards'
//   },
//   {
//     id: 'catalog',
//     name: 'Catalog',
//     subtitle: 'Product Catalog',
//     description: 'Manage product catalogs',
//     icon: 'fas fa-book',
//     path: '/catalog'
//   }
// ]

// const currentProduct = computed(() => {
//   const path = route.path
//   if (path.startsWith('/menu')) return products[0]
//   if (path.startsWith('/cards')) return products[1]
//   if (path.startsWith('/catalog')) return products[2]
//   return products[0] // default
// })

// Close dropdown when clicking outside
// const handleClickOutside = (event: MouseEvent) => {
//   if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
//     dropdownOpen.value = false
//   }
// }

// onMounted(() => {
//   document.addEventListener('click', handleClickOutside)
// })

// onUnmounted(() => {
//   document.removeEventListener('click', handleClickOutside)
// })
</script>

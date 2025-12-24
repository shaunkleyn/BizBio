<template>
  <div class="bg-[var(--light-background-color)] border-b border-[var(--light-border-color)] sticky top-20 z-40 py-6 px-4 md:px-8">
    <!-- Product Dropdown Section -->
    <!-- <div class="flex items-center justify-between mb-4">
      <div class="relative" ref="dropdownRef">
        <button
          @click="dropdownOpen = !dropdownOpen"
          class="flex items-center gap-3 px-4 py-2 bg-[var(--medium-background-color)] rounded-lg hover:bg-[var(--medium-background-color)] hover:opacity-80 transition-all"
        >
        <div class="w-10 h-10 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent2-color)] rounded-lg flex items-center justify-center">

          <i :class="[currentProduct.icon, 'text-[var(--primary-color)] w-10 h-10']"></i>
        </div>
          <div class="text-left">
            <div class="font-semibold text-[var(--dark-text-color)]">{{ currentProduct.name }}</div>
            <div class="text-xs text-[var(--gray-text-color)]">{{ currentProduct.subtitle }}</div>
          </div>
          <i class="fas fa-chevron-down text-[var(--gray-text-color)] text-xs ml-2"></i>
        </button> -->

        <!-- Dropdown Menu -->
        <!-- <div
          v-if="dropdownOpen"
          class="absolute left-0 mt-2 w-64 bg-[var(--light-background-color)] rounded-lg shadow-lg border border-[var(--light-border-color)] py-2 z-50"
        >
          <NuxtLink
            v-for="product in products"
            :key="product.id"
            :to="product.path"
            @click="dropdownOpen = false"
            class="flex items-center gap-3 px-4 py-3 hover:bg-[var(--medium-background-color)] transition-colors"
          >
            <div class="w-10 h-10 rounded-lg flex items-center justify-center"
              :class="product.id === currentProduct.id ? 'bg-[var(--primary-color)] bg-opacity-10' : 'bg-[var(--medium-background-color)]'"
            >
              <i :class="[product.icon, product.id === currentProduct.id ? 'text-[var(--primary-color)]' : 'text-[var(--gray-text-color)]']"></i>
            </div>
            <div class="flex-1">
              <div class="font-semibold text-[var(--dark-text-color)]">{{ product.name }}</div>
              <div class="text-xs text-[var(--gray-text-color)]">{{ product.description }}</div>
            </div>
            <i v-if="product.id === currentProduct.id" class="fas fa-check text-[var(--primary-color)]"></i>
          </NuxtLink>
        </div>
      </div>
    </div> -->

    <!-- Page Header and Actions Section -->
    <div class="flex items-center justify-between flex-wrap gap-4">
      <!-- Page Title and Description (Left) -->
      <div class="flex-1 min-w-0">
        <h1 class="text-2xl md:text-3xl font-bold text-[var(--dark-text-color)]">{{ pageHeader.title }}</h1>
        <p class="text-sm md:text-base text-[var(--gray-text-color)] mt-1">{{ pageHeader.description }}</p>
      </div>

      <!-- Action Buttons (Right) -->
      <div class="flex items-center gap-3 flex-shrink-0">
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

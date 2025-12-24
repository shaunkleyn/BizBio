<template>

  <aside class="w-64 bg-[var(--light-background-color)] sticky top-20 border-r border-[var(--light-border-color)] hidden lg:block overflow-y-auto self-start" style="max-height: calc(100vh - 5rem);">
    <div class="p-6">
      <!-- Product Header with Dropdown -->
      <div class="relative mb-8" ref="dropdownRef">
        <button
          @click="toggleDropdown"
          class="w-full flex items-center gap-3 p-3 hover:bg-[var(--medium-background-color)] rounded-lg transition-all"
        >
          <div class="w-10 h-10 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent2-color)] rounded-lg flex items-center justify-center flex-shrink-0">
            <i :class="[currentProduct.icon, 'text-white']"></i>
          </div>
          <div class="flex-1 text-left min-w-0">
            <h2 class="font-bold text-[var(--dark-text-color)]">{{ currentProduct.name }}</h2>
            <p class="text-xs text-[var(--gray-text-color)]">{{ currentProduct.subtitle }}</p>
          </div>
          <i class="fas fa-chevron-down text-[var(--gray-text-color)] text-xs flex-shrink-0"></i>
        </button>
      </div>

      <!-- Navigation Links -->
      <nav class="space-y-2">
        <NuxtLink
          to="/menu"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu') && !route.path.includes('/menu/')
              ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
              : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
          ]"
        >
          <i class="fas fa-th-large w-5"></i>
          <span>Overview</span>
        </NuxtLink>

        <NuxtLink
          to="/menu/menus"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu/menus')
              ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
              : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
          ]"
        >
          <i class="fas fa-book-open w-5"></i>
          <span>Menus</span>
        </NuxtLink>
        <NuxtLink
            to="/dashboard/bundles"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
              route.path === '/dashboard/bundles'
                ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
                : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
            ]"
          >
            <i class="fas fa-utensils w-5"></i>
            <span>Bundles</span>
          </NuxtLink>

        <!-- Library Submenu -->
        <div>
          <div class="text-xs font-semibold text-[var(--gray-text-color)] uppercase px-4 py-2 mt-4 mb-2">Library</div>

          <NuxtLink
            to="/menu/library/items"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
              route.path === '/menu/library/items'
                ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
                : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
            ]"
          >
            <i class="fas fa-utensils w-5"></i>
            <span>Items</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/extras"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
              route.path === '/menu/library/extras'
                ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
                : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
            ]"
          >
            <i class="fas fa-plus-circle w-5"></i>
            <span>Extras</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/extra-groups"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
              route.path === '/menu/library/extra-groups'
                ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
                : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
            ]"
          >
            <i class="fas fa-object-group w-5"></i>
            <span>Extra Groups</span>
          </NuxtLink>
        </div>

        <NuxtLink
          to="/menu/categories"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu/categories')
              ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
              : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
          ]"
        >
          <i class="fas fa-layer-group w-5"></i>
          <span>Categories</span>
        </NuxtLink>
        
        <NuxtLink
          v-if="!true"
          to="/menu/events"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu/events')
              ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
              : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
          ]"
        >
          <i class="fas fa-calendar-alt w-5"></i>
          <span>Events</span>
        </NuxtLink>
        <span v-else class="flex items-center gap-3 px-4 py-3 rounded-lg transition-colors text-[var(--dark-text-color)] opacity-50 cursor-not-allowed hover:bg-[var(--medium-background-color)]">
          <i class="fas fa-calendar-alt w-5"></i>
          Events
        </span>

        <NuxtLink
         v-if="!true"
          to="/menu/tables"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu/tables')
              ? 'bg-[var(--primary-color)] bg-opacity-10 text-white font-semibold'
              : 'text-[var(--dark-text-color)] hover:bg-[var(--medium-background-color)]'
          ]"
        >
          <i class="fas fa-qrcode w-5"></i>
          <span>QR Codes</span>
        </NuxtLink>
         <span v-else class="flex items-center gap-3 px-4 py-3 rounded-lg transition-colors text-[var(--dark-text-color)] opacity-50 cursor-not-allowed hover:bg-[var(--medium-background-color)]">
          <i class="fas fa-qrcode w-5"></i>
          QR Codes
        </span>
      </nav>

      <!-- Stats Section -->
      <div class="mt-8 pt-6 border-t border-[var(--light-border-color)]">
        <h3 class="text-xs font-semibold text-[var(--gray-text-color)] uppercase mb-3">Quick Stats</h3>
        <div class="space-y-2">
          <div class="flex justify-between text-sm">
            <span class="text-[var(--gray-text-color)]">Active Menus</span>
            <span class="font-semibold text-[var(--dark-text-color)]">{{ stats?.menus || 0 }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-[var(--gray-text-color)]">Total Items</span>
            <span class="font-semibold text-[var(--dark-text-color)]">{{ stats?.items || 0 }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-[var(--gray-text-color)]">Categories</span>
            <span class="font-semibold text-[var(--dark-text-color)]">{{ stats?.categories || 0 }}</span>
          </div>
        </div>
      </div>
    </div>
  </aside>

  <!-- Dropdown Menu (Teleported to body to escape overflow) -->
  <Teleport to="body">
    <div
      v-if="dropdownOpen"
      :style="{ position: 'fixed', top: `${dropdownPosition.top}px`, left: `${dropdownPosition.left}px`, zIndex: 9999 }"
      class="w-80 bg-[var(--light-background-color)] rounded-lg shadow-xl border border-[var(--light-border-color)] py-2"
    >
      <NuxtLink
        v-for="product in products"
        :key="product.id"
        :to="product.path"
        @click="dropdownOpen = false"
        class="flex items-center gap-3 px-4 py-3 hover:bg-[var(--medium-background-color)] transition-colors"
      >
        <div class="w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0"
          :class="product.id === currentProduct.id ? 'bg-[var(--primary-color)] bg-opacity-10' : 'bg-[var(--medium-background-color)]'"
        >
          <i :class="[product.icon, product.id === currentProduct.id ? 'text-[var(--primary-color)]' : 'text-[var(--gray-text-color)]', 'text-xl']"></i>
        </div>
        <div class="flex-1">
          <div class="font-semibold text-[var(--dark-text-color)]">{{ product.name }}</div>
          <div class="text-xs text-[var(--gray-text-color)]">{{ product.description }}</div>
        </div>
        <i v-if="product.id === currentProduct.id" class="fas fa-check text-[var(--primary-color)] flex-shrink-0"></i>
      </NuxtLink>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted, onUnmounted } from 'vue'
  const route = useRoute()
  const { pageHeader, pageActions } = usePageMeta()

const props = defineProps<{
  stats?: {
    menus?: number
    items?: number
    categories?: number
  }
}>()

const isActive = (path: string) => {
  return route.path.startsWith(path)
}

const dropdownOpen = ref(false)
const dropdownRef = ref<HTMLElement | null>(null)
const dropdownPosition = ref({ top: 0, left: 0 })

// Use page metadata composable

const products = [
  {
    id: 'menu',
    name: 'Menu',
    subtitle: 'Digital Menus',
    description: 'Create and manage digital menus',
    icon: 'fas fa-utensils',
    path: '/menu'
  },
  {
    id: 'cards',
    name: 'Business Cards',
    subtitle: 'Digital Cards',
    description: 'Create digital business cards',
    icon: 'fas fa-id-card',
    path: '/cards'
  },
  {
    id: 'catalog',
    name: 'Catalog',
    subtitle: 'Product Catalog',
    description: 'Manage product catalogs',
    icon: 'fas fa-book',
    path: '/catalog'
  }
]

const currentProduct = computed(() => {
  const path = route.path
  if (path.startsWith('/menu')) return products[0]
  if (path.startsWith('/cards')) return products[1]
  if (path.startsWith('/catalog')) return products[2]
  return products[0] // default
})

function toggleDropdown() {
  if (!dropdownRef.value) return
  
  if (!dropdownOpen.value) {
    const rect = dropdownRef.value.getBoundingClientRect()
    dropdownPosition.value = {
      top: rect.bottom + 8,
      left: rect.left
    }
  }
  
  dropdownOpen.value = !dropdownOpen.value
}

// Close dropdown when clicking outside
const handleClickOutside = (event: MouseEvent) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    // Check if click is not on the teleported dropdown
    const target = event.target as HTMLElement
    if (!target.closest('.w-80.bg-\\[var\\(--light-background-color\\)\\]')) {
      dropdownOpen.value = false
    }
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

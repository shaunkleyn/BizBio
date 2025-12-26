<template>

  <aside class="w-64 bg-md-surface shadow-md-2 sticky top-16 border-r border-md-outline-variant hidden lg:block overflow-y-auto self-start" style="max-height: calc(100vh - 4rem);">
    <div class="p-4">
      <!-- Product Header with Gradient -->
      <div class="relative mb-6 gradient-border p-4 rounded-xl animate-scale-in">
        <div class="flex items-center gap-3">
          <div class="w-12 h-12 bg-gradient-primary rounded-xl flex items-center justify-center flex-shrink-0 shadow-glow-purple">
            <i :class="[currentProduct.icon, 'text-white text-xl']"></i>
          </div>
          <div class="flex-1 min-w-0">
            <h2 class="font-bold text-md-on-surface text-base">{{ currentProduct.name }}</h2>
            <p class="text-xs text-md-on-surface-variant">{{ currentProduct.subtitle }}</p>
          </div>
        </div>
      </div>

      <!-- Navigation Links -->
      <nav class="space-y-1">
        <NuxtLink
          to="/menu"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
            isActive('/menu') && !route.path.includes('/menu/')
              ? 'bg-gradient-primary text-white shadow-glow-purple'
              : 'text-md-on-surface-variant hover:text-md-primary'
          ]"
        >
          <div :class="[
            'absolute inset-0 bg-md-primary-container opacity-0 group-hover:opacity-100 transition-opacity',
            isActive('/menu') && !route.path.includes('/menu/') ? 'opacity-0' : ''
          ]"></div>
          <i class="fas fa-th-large w-5 relative z-10"></i>
          <span class="text-sm relative z-10">Overview</span>
        </NuxtLink>

        <NuxtLink
          to="/menu/menus"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
            isActive('/menu/menus')
              ? 'bg-gradient-secondary text-white shadow-glow-pink'
              : 'text-md-on-surface-variant hover:text-md-secondary'
          ]"
        >
          <div :class="[
            'absolute inset-0 bg-md-secondary-container opacity-0 group-hover:opacity-100 transition-opacity',
            isActive('/menu/menus') ? 'opacity-0' : ''
          ]"></div>
          <i class="fas fa-book-open w-5 relative z-10"></i>
          <span class="text-sm relative z-10">Menus</span>
        </NuxtLink>

        <NuxtLink
            to="/dashboard/bundles"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/dashboard/bundles'
                ? 'bg-gradient-tertiary text-white shadow-glow-teal'
                : 'text-md-on-surface-variant hover:text-md-tertiary'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-tertiary-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/dashboard/bundles' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-layer-group w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Bundles</span>
          </NuxtLink>

        <!-- Library Submenu -->
        <div class="mt-6">
          <div class="flex items-center gap-2 px-4 py-2 mb-2">
            <div class="h-px flex-1 bg-gradient-primary"></div>
            <span class="text-xs font-bold text-md-on-surface-variant uppercase tracking-wider">Library</span>
            <div class="h-px flex-1 bg-gradient-primary"></div>
          </div>

          <NuxtLink
            to="/menu/library/items"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/menu/library/items'
                ? 'bg-gradient-accent text-white shadow-md-2'
                : 'text-md-on-surface-variant hover:text-md-accent'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-accent-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/menu/library/items' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-utensils w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Items</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/extras"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/menu/library/extras'
                ? 'bg-gradient-primary text-white shadow-glow-purple'
                : 'text-md-on-surface-variant hover:text-md-primary'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-primary-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/menu/library/extras' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-plus-circle w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Extras</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/extra-groups"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/menu/library/extra-groups'
                ? 'bg-gradient-secondary text-white shadow-glow-pink'
                : 'text-md-on-surface-variant hover:text-md-secondary'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-secondary-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/menu/library/extra-groups' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-object-group w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Extra Groups</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/options"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/menu/library/options'
                ? 'bg-gradient-tertiary text-white shadow-glow-teal'
                : 'text-md-on-surface-variant hover:text-md-tertiary'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-tertiary-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/menu/library/options' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-sliders-h w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Options</span>
          </NuxtLink>

          <NuxtLink
            to="/menu/library/option-groups"
            :class="[
              'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium md-ripple group relative overflow-hidden',
              route.path === '/menu/library/option-groups'
                ? 'bg-gradient-accent text-white shadow-md-2'
                : 'text-md-on-surface-variant hover:text-md-accent'
            ]"
          >
            <div :class="[
              'absolute inset-0 bg-md-accent-container opacity-0 group-hover:opacity-100 transition-opacity',
              route.path === '/menu/library/option-groups' ? 'opacity-0' : ''
            ]"></div>
            <i class="fas fa-layer-group w-5 relative z-10"></i>
            <span class="text-sm relative z-10">Option Groups</span>
          </NuxtLink>
        </div>

        <NuxtLink
          to="/menu/categories"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
            isActive('/menu/categories')
              ? 'bg-md-primary-container text-white font-semibold'
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
              ? 'bg-md-primary-container text-white font-semibold'
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
              ? 'bg-md-primary-container text-white font-semibold'
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
          :class="product.id === currentProduct.id ? 'bg-md-primary-container' : 'bg-[var(--medium-background-color)]'"
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


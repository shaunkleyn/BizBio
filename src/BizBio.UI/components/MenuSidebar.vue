<template>

  <aside class="w-64 bg-[var(--light-background-color)] h-screen sticky top-0 border-r border-[var(--light-border-color)] hidden lg:block overflow-y-auto">
    <div class="p-6">
      <!-- Product Header -->
      <div class="flex items-center gap-3 mb-8">
        <div class="w-10 h-10 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent2-color)] rounded-lg flex items-center justify-center">
          <i class="fas fa-utensils text-white"></i>
        </div>
        <div>
          <h2 class="font-bold text-[var(--dark-text-color)]">Menu Pro</h2>
          <p class="text-xs text-[var(--gray-text-color)]">Dashboard</p>
        </div>
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

        <NuxtLink
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
</template>

<script setup lang="ts">
const route = useRoute()

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
</script>

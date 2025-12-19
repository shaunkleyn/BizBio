<template>
  <div class="min-h-screen bg-gray-50">
    <div class="flex">
      <!-- Left Sidebar -->
      <div
        :class="[
          'bg-white border-r border-gray-200 transition-all duration-300 flex-shrink-0 sticky top-0 h-screen overflow-y-auto',
          sidebarOpen ? 'w-64' : 'w-16'
        ]"
      >
        <!-- Sidebar Header -->
        <div class="p-4 border-b border-gray-200 flex items-center justify-between">
          <h2 v-if="sidebarOpen" class="font-bold text-gray-900">Library</h2>
          <button
            @click="sidebarOpen = !sidebarOpen"
            class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i :class="sidebarOpen ? 'fas fa-chevron-left' : 'fas fa-chevron-right'" class="text-gray-600"></i>
          </button>
        </div>

        <!-- Navigation Items -->
        <nav class="p-2">
          <!-- Items Section -->
          <div class="mb-2">
            <NuxtLink
              to="/dashboard/library/items"
              :class="[
                'flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors',
                isActive('/dashboard/library/items')
                  ? 'bg-blue-50 text-blue-700 font-semibold'
                  : 'text-gray-700 hover:bg-gray-100'
              ]"
            >
              <i class="fas fa-utensils w-5"></i>
              <span v-if="sidebarOpen">Items</span>
            </NuxtLink>
          </div>

          <!-- Categories -->
          <div class="mb-2">
            <button
              @click="emit('show-category-modal')"
              :class="[
                'w-full flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors',
                'text-gray-700 hover:bg-gray-100'
              ]"
            >
              <i class="fas fa-folder w-5"></i>
              <span v-if="sidebarOpen" class="flex-1 text-left">Categories</span>
              <span v-if="sidebarOpen && categoryCount > 0" class="text-xs bg-gray-200 px-2 py-0.5 rounded-full">
                {{ categoryCount }}
              </span>
            </button>
          </div>

          <!-- Extras -->
          <div class="mb-2">
            <NuxtLink
              to="/dashboard/library/extras"
              :class="[
                'flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors',
                isActive('/dashboard/library/extras')
                  ? 'bg-blue-50 text-blue-700 font-semibold'
                  : 'text-gray-700 hover:bg-gray-100'
              ]"
            >
              <i class="fas fa-plus-circle w-5"></i>
              <span v-if="sidebarOpen">Extras</span>
            </NuxtLink>
          </div>

          <!-- Extra Groups -->
          <div class="mb-2">
            <NuxtLink
              to="/dashboard/library/extra-groups"
              :class="[
                'flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors',
                isActive('/dashboard/library/extra-groups')
                  ? 'bg-blue-50 text-blue-700 font-semibold'
                  : 'text-gray-700 hover:bg-gray-100'
              ]"
            >
              <i class="fas fa-layer-group w-5"></i>
              <span v-if="sidebarOpen">Extra Groups</span>
            </NuxtLink>
          </div>
        </nav>

        <!-- Quick Stats (when expanded) -->
        <div v-if="sidebarOpen && stats" class="p-4 border-t border-gray-200 mt-4">
          <h3 class="text-xs font-semibold text-gray-500 uppercase mb-3">Quick Stats</h3>
          <div class="space-y-2">
            <div v-if="stats.items !== undefined" class="flex justify-between text-sm">
              <span class="text-gray-600">Total Items</span>
              <span class="font-semibold text-gray-900">{{ stats.items }}</span>
            </div>
            <div v-if="stats.categories !== undefined" class="flex justify-between text-sm">
              <span class="text-gray-600">Categories</span>
              <span class="font-semibold text-gray-900">{{ stats.categories }}</span>
            </div>
            <div v-if="stats.extras !== undefined" class="flex justify-between text-sm">
              <span class="text-gray-600">Extras</span>
              <span class="font-semibold text-gray-900">{{ stats.extras }}</span>
            </div>
            <div v-if="stats.extraGroups !== undefined" class="flex justify-between text-sm">
              <span class="text-gray-600">Extra Groups</span>
              <span class="font-semibold text-gray-900">{{ stats.extraGroups }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Slot -->
      <div class="flex-1">
        <slot />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const props = defineProps<{
  categoryCount?: number
  stats?: {
    items?: number
    categories?: number
    extras?: number
    extraGroups?: number
  }
}>()

const emit = defineEmits<{
  'show-category-modal': []
}>()

const route = useRoute()
const sidebarOpen = ref(true)

const isActive = (path: string) => {
  return route.path === path
}

onMounted(() => {
  // Collapse sidebar on mobile by default
  if (import.meta.client && window.innerWidth < 768) {
    sidebarOpen.value = false
  }
})
</script>

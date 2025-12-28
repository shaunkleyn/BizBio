<template>
  <div>
    <div class="relative inline-block text-left" v-click-outside="close">
      <div>
        <button
          type="button"
          @click="toggle"
          class="inline-flex items-center justify-between w-full rounded-md border border-gray-300 dark:border-gray-600 shadow-sm px-4 py-2 bg-white dark:bg-gray-800 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          :class="{ 'min-w-[200px]': !compact }"
        >
          <div class="flex items-center">
            <svg class="h-5 w-5 mr-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
            <span class="truncate">
              {{ selectedRestaurant?.name || 'Select Restaurant' }}
            </span>
          </div>
          <svg class="ml-2 h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
          </svg>
        </button>
      </div>

    <!-- Dropdown -->
    <transition
      enter-active-class="transition ease-out duration-100"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div
        v-if="isOpen"
        class="origin-top-right absolute right-0 mt-2 w-64 rounded-md shadow-lg bg-white dark:bg-gray-800 ring-1 ring-black ring-opacity-5 focus:outline-none z-10"
      >
        <div class="py-1">
          <!-- Loading State -->
          <div v-if="loading" class="px-4 py-3 text-sm text-gray-500 dark:text-gray-400 text-center">
            Loading restaurants...
          </div>

          <!-- No Restaurants -->
          <div v-else-if="restaurants.length === 0" class="px-4 py-3">
            <p class="text-sm text-gray-500 dark:text-gray-400 text-center mb-2">
              No restaurants yet
            </p>
            <button
              @click="createRestaurant"
              class="w-full text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
            >
              + Create Restaurant
            </button>
          </div>

          <!-- Restaurant List -->
          <div v-else>
            <button
              v-for="restaurant in restaurants"
              :key="restaurant.id"
              @click="selectRestaurant(restaurant)"
              class="w-full text-left px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center justify-between"
              :class="{
                'bg-blue-50 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400': restaurant.id === modelValue
              }"
            >
              <div class="flex-1">
                <div class="font-medium">{{ restaurant.name }}</div>
                <div class="text-xs text-gray-500 dark:text-gray-400">
                  {{ restaurant.profileCount }} menu{{ restaurant.profileCount !== 1 ? 's' : '' }}
                </div>
              </div>
              <svg
                v-if="restaurant.id === modelValue"
                class="h-5 w-5 text-blue-600 dark:text-blue-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
              </svg>
            </button>

            <!-- Add Restaurant Button -->
            <div class="border-t border-gray-200 dark:border-gray-700 mt-1 pt-1">
              <button
                @click="createRestaurant"
                class="w-full text-left px-4 py-2 text-sm text-blue-600 dark:text-blue-400 hover:bg-gray-100 dark:hover:bg-gray-700"
              >
                + Create New Restaurant
              </button>
            </div>
          </div>
        </div>
      </div>
    </transition>
    </div>

    <!-- Restaurant Form Modal (higher z-index than MenuCreationModal) -->
    <RestaurantFormModal
      v-if="showRestaurantModal"
      @close="showRestaurantModal = false"
      @saved="handleRestaurantSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

interface Restaurant {
  id: number
  name: string
  description?: string
  profileCount: number
}

const props = defineProps<{
  modelValue?: number | null
  compact?: boolean
}>()

const emit = defineEmits(['update:modelValue', 'change'])

const { $api } = useNuxtApp()

const isOpen = ref(false)
const loading = ref(false)
const restaurants = ref<Restaurant[]>([])
const showRestaurantModal = ref(false)

const selectedRestaurant = computed(() => {
  return restaurants.value.find(r => r.id === props.modelValue)
})

const fetchRestaurants = async () => {
  try {
    loading.value = true
    const response = await $api.get('/restaurants')
    restaurants.value = response.data.data || []
  } catch (error) {
    console.error('Failed to fetch restaurants:', error)
  } finally {
    loading.value = false
  }
}

const toggle = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value && restaurants.value.length === 0) {
    fetchRestaurants()
  }
}

const close = () => {
  isOpen.value = false
}

const selectRestaurant = (restaurant: Restaurant) => {
  emit('update:modelValue', restaurant.id)
  emit('change', restaurant)
  close()
}

const createRestaurant = () => {
  close()
  showRestaurantModal.value = true
}

const handleRestaurantSaved = async () => {
  showRestaurantModal.value = false
  await fetchRestaurants()
  // Auto-select the newly created restaurant
  if (restaurants.value.length > 0) {
    const newRestaurant = restaurants.value[restaurants.value.length - 1]
    selectRestaurant(newRestaurant)
  }
}

// Click outside directive
const vClickOutside = {
  mounted(el: HTMLElement, binding: any) {
    el.clickOutsideEvent = (event: Event) => {
      if (!(el === event.target || el.contains(event.target as Node))) {
        binding.value()
      }
    }
    document.addEventListener('click', el.clickOutsideEvent)
  },
  unmounted(el: HTMLElement & { clickOutsideEvent?: any }) {
    if (el.clickOutsideEvent) {
      document.removeEventListener('click', el.clickOutsideEvent)
    }
  }
}

onMounted(() => {
  fetchRestaurants()
})
</script>

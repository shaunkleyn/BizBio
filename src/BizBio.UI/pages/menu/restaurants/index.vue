<template>

  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-store text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              3
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Restaurants
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +12%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-utensils text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              18
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Active Menus
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +8%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-yellow-500 to-orange-500 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-box text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              247
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Menu Items
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +25%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-purple-500 to-pink-500 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-puzzle-piece text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              42
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Shared Items
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +34%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>
        </div>

  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
              My Restaurants
            </h1>
            <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
              Manage your restaurants and their menus
            </p>
          </div>
          <button
            @click="openCreateModal"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            Add Restaurant
          </button>
        </div>

        <!-- Subscription Info -->
        <div v-if="subscriptionInfo" class="mt-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <svg class="h-5 w-5 text-blue-600 dark:text-blue-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <span class="text-sm text-blue-800 dark:text-blue-300">
                {{ restaurants.length }} / {{ subscriptionInfo.maxRestaurants }} restaurants used
              </span>
            </div>
            <NuxtLink
              to="/dashboard/subscription"
              class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
            >
              Upgrade Plan →
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="text-center py-12">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        <p class="mt-4 text-gray-600 dark:text-gray-400">Loading restaurants...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="restaurants.length === 0" class="text-center py-12">
        <svg class="mx-auto h-24 w-24 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
        <h3 class="mt-4 text-lg font-medium text-gray-900 dark:text-white">No restaurants yet</h3>
        <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
          Get started by creating your first restaurant
        </p>
        <button
          @click="openCreateModal"
          class="mt-6 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          Create Restaurant
        </button>
      </div>

      <!-- Restaurant Grid -->
      <div v-else class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div
          v-for="restaurant in restaurants"
          :key="restaurant.id"
          class="bg-white dark:bg-gray-800 rounded-lg shadow hover:shadow-lg transition-shadow duration-200 overflow-hidden"
        >
          <!-- Restaurant Header -->
          <div class="p-6">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                  {{ restaurant.name }}
                </h3>
                <p v-if="restaurant.description" class="mt-1 text-sm text-gray-600 dark:text-gray-400 line-clamp-2">
                  {{ restaurant.description }}
                </p>
              </div>
              <div class="ml-4">
                <button
                  @click="openEditModal(restaurant)"
                  class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
                >
                  <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                  </svg>
                </button>
              </div>
            </div>

            <!-- Restaurant Info -->
            <div class="mt-4 space-y-2">
              <div v-if="restaurant.address" class="flex items-center text-sm text-gray-600 dark:text-gray-400">
                <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                {{ restaurant.city || restaurant.address }}
              </div>
              <div v-if="restaurant.phone" class="flex items-center text-sm text-gray-600 dark:text-gray-400">
                <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
                {{ restaurant.phone }}
              </div>
            </div>

            <!-- Menu Count -->
            <div class="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">
                  {{ restaurant.profileCount }} menu{{ restaurant.profileCount !== 1 ? 's' : '' }}
                </span>
                <NuxtLink
                  :to="`/dashboard/restaurants/${restaurant.id}/menus`"
                  class="text-sm font-medium text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
                >
                  Manage Menus →
                </NuxtLink>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="bg-gray-50 dark:bg-gray-700/50 px-6 py-3 flex justify-between items-center">
            <button
              @click="viewRestaurantMenus(restaurant.id)"
              class="text-sm text-gray-700 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white"
            >
              View Menus
            </button>
            <button
              @click="confirmDelete(restaurant)"
              class="text-sm text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Restaurant Form Modal -->
    <RestaurantFormModal
      v-if="showModal"
      :restaurant="selectedRestaurant"
      @close="closeModal"
      @saved="handleRestaurantSaved"
    />

    <!-- Delete Confirmation Modal -->
    <ConfirmModal
      v-if="showDeleteModal"
      title="Delete Restaurant"
      :message="`Are you sure you want to delete ${restaurantToDelete?.name}? This will also delete all associated menus and cannot be undone.`"
      confirm-text="Delete"
      confirm-class="bg-red-600 hover:bg-red-700"
      @confirm="deleteRestaurant"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup lang="ts">
import { useNuxtApp } from 'nuxt/app'
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { usePageMeta } from '../../../composables/usePageMeta'

const router = useRouter()
const { $api } = useNuxtApp()

definePageMeta({
  layout: 'menu'
})


interface Restaurant {
  id: number
  name: string
  description?: string
  logo?: string
  address?: string
  city?: string
  state?: string
  country?: string
  phone?: string
  email?: string
  profileCount: number
  isActive: boolean
}

const restaurants = ref<Restaurant[]>([])
const loading = ref(true)
const showModal = ref(false)
const showDeleteModal = ref(false)
const selectedRestaurant = ref<Restaurant | null>(null)
const restaurantToDelete = ref<Restaurant | null>(null)
const subscriptionInfo = ref<any>(null)

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

const fetchSubscriptionInfo = async () => {
  try {
    const response = await $api.get('/subscriptions/my-subscription')
    if (response.data.data) {
      subscriptionInfo.value = {
        maxRestaurants: response.data.data.tier?.maxRestaurants || 1,
        maxLibraryItems: response.data.data.tier?.maxLibraryItems || 50
      }
    }
  } catch (error) {
    console.error('Failed to fetch subscription info:', error)
  }
}

const openCreateModal = () => {
  if (subscriptionInfo.value && restaurants.value.length >= subscriptionInfo.value.maxRestaurants) {
    alert(`You have reached your restaurant limit (${subscriptionInfo.value.maxRestaurants}). Please upgrade your subscription.`)
    return
  }
  selectedRestaurant.value = null
  showModal.value = true
}

const openEditModal = (restaurant: Restaurant) => {
  selectedRestaurant.value = restaurant
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  selectedRestaurant.value = null
}

const handleRestaurantSaved = () => {
  closeModal()
  fetchRestaurants()
}

const confirmDelete = (restaurant: Restaurant) => {
  restaurantToDelete.value = restaurant
  showDeleteModal.value = true
}

const deleteRestaurant = async () => {
  if (!restaurantToDelete.value) return

  try {
    await $api.delete(`/restaurants/${restaurantToDelete.value.id}`)
    showDeleteModal.value = false
    restaurantToDelete.value = null
    await fetchRestaurants()
  } catch (error) {
    console.error('Failed to delete restaurant:', error)
    alert('Failed to delete restaurant. Please try again.')
  }
}

const viewRestaurantMenus = (restaurantId: number) => {
  router.push(`/dashboard/restaurants/${restaurantId}/menus`)
}

const { setPageHeader, setPageActionButton } = usePageMeta()


onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Ressy',
    description: 'Create special bundle deals like "Family Meal Deal" with multiple items and options'
  })

  setPageActions(() => h('button', {
    onClick: () => {},
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'
  }, [
    h('i', { class: 'fas fa-plus mr-2' }),
    'Create Bundle'
  ]))

  fetchRestaurants()
  fetchSubscriptionInfo()
})

usePageMeta({
  title: 'My 2Restaurants',
  description: 'Manage your restaurants'
})
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center h-screen bg-white">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mx-auto mb-4"></div>
        <p class="text-gray-600 font-medium">Loading menu...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex flex-col justify-center items-center h-screen px-4 bg-white">
      <i class="fas fa-exclamation-circle text-6xl text-red-500 mb-4"></i>
      <h2 class="text-2xl font-bold text-gray-900 mb-2">Menu Not Found</h2>
      <p class="text-gray-600 text-center">{{ error }}</p>
    </div>

    <!-- Menu Content -->
    <div v-else>
      <!-- Hero Section with Restaurant Image -->
      <div v-if="menuData?.coverImage" class="relative h-56 md:h-72 bg-gradient-to-br from-gray-200 to-gray-300">
        <img
          :src="menuData.coverImage"
          :alt="menuData.name"
          class="w-full h-full object-cover"
        />
        <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent"></div>
        <button
          @click="goBack"
          class="absolute top-4 left-4 w-10 h-10 bg-white bg-opacity-95 rounded-full shadow-lg flex items-center justify-center hover:bg-opacity-100 transition-all"
        >
          <i class="fas fa-arrow-left text-gray-900"></i>
        </button>
        <button
          v-if="isAuthenticated"
          @click="toggleEditMode"
          class="absolute top-4 right-4 px-4 py-2 bg-white bg-opacity-95 rounded-full shadow-lg hover:bg-opacity-100 transition-all text-sm font-semibold text-gray-900"
        >
          <i class="fas fa-edit mr-2"></i>
          {{ editMode ? 'Done' : 'Edit' }}
        </button>
      </div>

      <!-- Restaurant Info Header -->
      <div class="bg-white border-b border-gray-200">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <!-- Restaurant Name and Rating -->
          <div class="mb-4">
            <h1 class="text-3xl md:text-4xl font-bold text-gray-900 mb-3">
              {{ menuData?.name || 'Menu' }}
            </h1>
            <div v-if="menuData?.rating" class="flex items-center gap-2 mb-3">
              <div class="flex items-center gap-1.5 bg-gray-50 px-3 py-1.5 rounded-full">
                <i class="fas fa-star text-yellow-500 text-xs"></i>
                <span class="font-bold text-gray-900">{{ menuData.rating }}</span>
              </div>
              <span v-if="menuData?.reviewCount" class="text-sm text-gray-600">
                {{ menuData.reviewCount.toLocaleString() }} reviews
              </span>
            </div>
          </div>

          <!-- Operating Hours -->
          <div v-if="menuData?.operatingHours" class="mb-4 pb-4 border-b border-gray-200">
            <div class="flex items-start gap-3">
              <i class="fas fa-clock text-gray-400 mt-1"></i>
              <div class="flex-1">
                <div class="text-sm text-gray-900 font-medium mb-2">Operating Hours</div>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-2 text-sm">
                  <div v-for="(hours, day) in menuData.operatingHours" :key="day" class="flex justify-between">
                    <span class="text-gray-600">{{ day }}</span>
                    <span class="text-gray-900 font-medium">{{ hours }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Address -->
          <div v-if="menuData?.address" class="mb-4 pb-4 border-b border-gray-200">
            <div class="flex items-start gap-3">
              <i class="fas fa-map-marker-alt text-gray-400 mt-1"></i>
              <div class="flex-1">
                <div class="text-sm text-gray-900 font-medium mb-1">Address</div>
                <div class="text-sm text-gray-600">{{ menuData.address }}</div>
              </div>
            </div>
          </div>

          <!-- Cuisine Tags -->
          <div v-if="menuData?.cuisineTags && menuData.cuisineTags.length > 0" class="mb-4">
            <div class="flex flex-wrap gap-2">
              <span
                v-for="(tag, index) in menuData.cuisineTags"
                :key="index"
                class="px-3 py-1.5 bg-gray-100 text-gray-700 text-xs font-medium rounded-full"
              >
                {{ tag }}
              </span>
            </div>
          </div>

          <!-- Delivery Info Banner -->
          <div v-if="menuData?.deliveryInfo" class="mt-4 p-4 bg-blue-50 border border-blue-200 rounded-lg">
            <div class="flex items-center gap-3 text-sm text-blue-900">
              <i class="fas fa-motorcycle text-blue-600"></i>
              <span class="font-medium">{{ menuData.deliveryInfo }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Category Tabs (Sticky) -->
      <div v-if="categories.length > 0" class="sticky top-0 z-30 bg-white border-b border-gray-200 shadow-sm">
        <div class="max-w-7xl mx-auto">
          <div class="overflow-x-auto hide-scrollbar">
            <div class="flex space-x-1 px-4 sm:px-6 lg:px-8 py-2 min-w-max">
              <button
                v-for="category in categories"
                :key="category.id"
                :data-category-id="category.id"
                @click="scrollToCategory(category.id)"
                :class="[
                  'px-4 py-2.5 text-sm font-medium whitespace-nowrap transition-all relative',
                  activeCategory === category.id
                    ? 'text-gray-900 border-b-2 border-gray-900'
                    : 'text-gray-600 hover:text-gray-900 border-b-2 border-transparent'
                ]"
              >
                {{ category.name }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Highlights Carousel -->
      <div v-if="highlightedItems.length > 0" class="bg-white py-4 sm:py-6 border-b border-gray-200">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 class="text-xl sm:text-2xl font-bold text-gray-900 mb-3 sm:mb-4">Highlights</h2>
          <div class="overflow-x-auto hide-scrollbar -mx-4 px-4 sm:mx-0 sm:px-0">
            <div class="flex gap-3 sm:gap-4 pb-2">
              <div
                v-for="item in highlightedItems"
                :key="item.id"
                class="flex-shrink-0 w-56 sm:w-64 md:w-72 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow border border-gray-200 overflow-hidden cursor-pointer"
                @click="openItemDetail(item)"
              >
                <!-- Item Image -->
                <div class="relative w-full h-36 sm:h-40 md:h-44 bg-gradient-to-br from-gray-100 to-gray-200 overflow-hidden group">
                  <img
                    v-if="item.images && item.images.length > 0"
                    :src="item.images[0]"
                    :alt="item.name"
                    class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                  <div v-else class="w-full h-full flex items-center justify-center">
                    <i class="fas fa-utensils text-4xl sm:text-5xl text-gray-300"></i>
                  </div>
                  <span
                    v-if="item.badge"
                    class="absolute top-2 left-2 px-2 py-0.5 bg-green-500 text-white text-xs font-bold rounded-full shadow-lg"
                  >
                    {{ item.badge }}
                  </span>
                </div>

                <!-- Item Info -->
                <div class="p-3 sm:p-4">
                  <h3 class="text-base sm:text-lg font-bold text-gray-900 mb-1 line-clamp-1">{{ item.name }}</h3>
                  <p v-if="item.description" class="text-xs sm:text-sm text-gray-600 line-clamp-2 mb-2 sm:mb-3 leading-relaxed">
                    {{ item.description }}
                  </p>
                  <div class="flex items-center justify-between">
                    <div class="flex flex-col">
                      <span class="text-xs text-gray-500 font-medium">From</span>
                      <span class="text-base sm:text-lg font-bold text-gray-900">R{{ item.price.toFixed(2) }}</span>
                    </div>
                    <button
                      @click.stop="openItemDetail(item)"
                      class="px-4 sm:px-6 py-2 sm:py-2.5 bg-gray-900 text-white text-xs sm:text-sm font-semibold rounded-lg hover:bg-gray-800 transition-colors active:scale-95 transform"
                    >
                      Add
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Most Ordered Carousel -->
      <div v-if="mostOrderedItems.length > 0" class="bg-white py-4 sm:py-6 border-b border-gray-200">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 class="text-xl sm:text-2xl font-bold text-gray-900 mb-3 sm:mb-4">Most Ordered 💯</h2>
          <div class="overflow-x-auto hide-scrollbar -mx-4 px-4 sm:mx-0 sm:px-0">
            <div class="flex gap-3 sm:gap-4 pb-2">
              <div
                v-for="item in mostOrderedItems"
                :key="item.id"
                class="flex-shrink-0 w-56 sm:w-64 md:w-72 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow border border-gray-200 overflow-hidden cursor-pointer"
                @click="openItemDetail(item)"
              >
                <!-- Item Image -->
                <div class="relative w-full h-36 sm:h-40 md:h-44 bg-gradient-to-br from-gray-100 to-gray-200 overflow-hidden group">
                  <img
                    v-if="item.images && item.images.length > 0"
                    :src="item.images[0]"
                    :alt="item.name"
                    class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                  <div v-else class="w-full h-full flex items-center justify-center">
                    <i class="fas fa-utensils text-4xl sm:text-5xl text-gray-300"></i>
                  </div>
                  <span
                    v-if="item.badge"
                    class="absolute top-2 left-2 px-2 py-0.5 bg-orange-500 text-white text-xs font-bold rounded-full shadow-lg"
                  >
                    {{ item.badge }}
                  </span>
                </div>

                <!-- Item Info -->
                <div class="p-3 sm:p-4">
                  <h3 class="text-base sm:text-lg font-bold text-gray-900 mb-1 line-clamp-1">{{ item.name }}</h3>
                  <p v-if="item.description" class="text-xs sm:text-sm text-gray-600 line-clamp-2 mb-2 sm:mb-3 leading-relaxed">
                    {{ item.description }}
                  </p>
                  <div class="flex items-center justify-between">
                    <div class="flex flex-col">
                      <span class="text-xs text-gray-500 font-medium">From</span>
                      <span class="text-base sm:text-lg font-bold text-gray-900">R{{ item.price.toFixed(2) }}</span>
                    </div>
                    <button
                      @click.stop="openItemDetail(item)"
                      class="px-4 sm:px-6 py-2 sm:py-2.5 bg-gray-900 text-white text-xs sm:text-sm font-semibold rounded-lg hover:bg-gray-800 transition-colors active:scale-95 transform"
                    >
                      Add
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Menu Items by Category -->
      <div class="max-w-7xl mx-auto pb-24 bg-gray-50">
        <div
          v-for="category in categories"
          :key="category.id"
          :id="`category-${category.id}`"
          class="mb-8"
        >
          <h2 class="text-2xl font-bold text-gray-900 px-4 sm:px-6 lg:px-8 py-6 bg-white sticky top-[52px] z-10 border-b border-gray-100">
            {{ category.name }}
          </h2>

          <!-- Grid Layout for Menu Items -->
          <div class="px-4 sm:px-6 lg:px-8 pt-3 sm:pt-4 grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4">
            <div
              v-for="item in getCategoryItems(category.id)"
              :key="item.id"
              class="bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow overflow-hidden border border-gray-200"
            >
              <!-- Item Image -->
              <div
                @click="openItemDetail(item)"
                class="relative w-full h-32 sm:h-40 md:h-48 bg-gradient-to-br from-gray-100 to-gray-200 cursor-pointer overflow-hidden group"
              >
                <img
                  v-if="item.images && item.images.length > 0"
                  :src="item.images[0]"
                  :alt="item.name"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                />
                <div v-else class="w-full h-full flex items-center justify-center">
                  <i class="fas fa-utensils text-3xl sm:text-4xl md:text-5xl text-gray-300"></i>
                </div>
                <span
                  v-if="item.itemType === 1"
                  class="absolute top-2 left-2 px-2 py-0.5 sm:px-2.5 sm:py-1 bg-orange-500 text-white text-xs font-bold rounded-full shadow-lg"
                >
                  BUNDLE
                </span>
              </div>

              <!-- Item Info -->
              <div class="p-2 sm:p-3 md:p-4">
                <div class="mb-2">
                  <h3
                    @click="openItemDetail(item)"
                    class="text-sm sm:text-base md:text-lg font-bold text-gray-900 mb-1 cursor-pointer hover:text-gray-700 transition-colors line-clamp-1"
                  >
                    {{ item.name }}
                  </h3>
                  <p v-if="item.description" class="text-xs sm:text-sm text-gray-600 line-clamp-2 leading-relaxed hidden sm:block">
                    {{ item.description }}
                  </p>
                </div>

                <!-- Price and Add Button -->
                <div class="flex items-center justify-between mt-2 sm:mt-3 pt-2 sm:pt-3 border-t border-gray-100">
                  <div class="flex flex-col">
                    <span class="text-xs text-gray-500 font-medium">From</span>
                    <span class="text-sm sm:text-base md:text-lg font-bold text-gray-900">R{{ item.price.toFixed(2) }}</span>
                  </div>
                  <button
                    @click="openItemDetail(item)"
                    class="px-3 sm:px-4 md:px-6 py-1.5 sm:py-2 md:py-2.5 bg-gray-900 text-white text-xs sm:text-sm font-semibold rounded-lg hover:bg-gray-800 transition-colors active:scale-95 transform"
                  >
                    Add
                  </button>
                </div>

                <!-- Edit Button (when in edit mode) -->
                <button
                  v-if="editMode"
                  @click.stop="editItem(item)"
                  class="mt-2 sm:mt-3 w-full px-2 sm:px-3 py-1.5 sm:py-2 bg-blue-600 text-white text-xs sm:text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                >
                  <i class="fas fa-edit mr-1"></i>
                  Edit Item
                </button>
              </div>
            </div>
          </div>

          <!-- Empty Category -->
          <div v-if="getCategoryItems(category.id).length === 0" class="text-center py-16 text-gray-500 bg-white mx-4 sm:mx-6 lg:mx-8 rounded-lg">
            <i class="fas fa-box-open text-6xl mb-4 opacity-30"></i>
            <p class="text-base font-medium">No items in this category</p>
          </div>
        </div>
      </div>

      <!-- Floating Cart Button -->
      <div
        v-if="cartStore.itemCount > 0"
        @click="cartStore.openCart()"
        class="fixed bottom-6 left-4 right-4 md:left-auto md:right-6 md:max-w-sm z-40 bg-gray-900 text-white rounded-xl shadow-2xl px-5 py-4 cursor-pointer hover:bg-gray-800 transition-all active:scale-95 transform"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
              <i class="fas fa-shopping-cart text-base"></i>
            </div>
            <div class="flex flex-col">
              <span class="font-semibold text-base">{{ cartStore.itemCount }} {{ cartStore.itemCount === 1 ? 'item' : 'items' }}</span>
              <span class="text-xs text-gray-300">View cart</span>
            </div>
          </div>
          <div class="font-bold text-xl">R{{ cartStore.total.toFixed(2) }}</div>
        </div>
      </div>
    </div>

    <!-- Item Detail Modal -->
    <ItemDetailModal
      v-if="selectedItem && !selectedItem.isBundle"
      :item="selectedItem"
      :menu-slug="route.params.slug as string"
      :edit-mode="editMode"
      @close="selectedItem = null"
      @edit="editItem"
    />

    <!-- Bundle Configuration Modal -->
    <BundleConfigModal
      v-if="selectedBundle"
      :is-open="!!selectedBundle"
      :bundle="selectedBundle"
      @close="selectedBundle = null"
      @add-to-cart="handleBundleAddToCart"
    />

    <!-- Cart Drawer -->
    <CartDrawer />

    <!-- Item Form Modal (for editing) -->
    <ItemFormModal
      v-if="editingItem"
      :item="editingItem"
      @close="editingItem = null"
      @saved="handleItemSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMenusApi } from '~/composables/useApi'
import { useCartStore } from '~/stores/cart'
import { useAuthStore } from '~/stores/auth'
import { useToast } from '~/composables/useToast'

const route = useRoute()
const router = useRouter()
const menusApi = useMenusApi()
const cartStore = useCartStore()
const authStore = useAuthStore()
const toast = useToast()

const loading = ref(true)
const error = ref<string | null>(null)
const menuData = ref<any>(null)
const items = ref<any[]>([])
const selectedItem = ref<any>(null)
const selectedBundle = ref<any>(null)
const activeCategory = ref<number | null>(null)
const editMode = ref(false)
const editingItem = ref<any>(null)

const isAuthenticated = computed(() => authStore.isAuthenticated)

const categories = computed(() => {
  if (!menuData.value?.categories) return []
  return menuData.value.categories.sort((a: any, b: any) => a.sortOrder - b.sortOrder)
})

const highlightedItems = computed(() => {
  // Get items marked as highlighted/featured
  return items.value
    .filter(item => item.isActive && item.isFeatured)
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .slice(0, 5) // Limit to 5 items
})

const mostOrderedItems = computed(() => {
  // Get items marked as most ordered or popular
  return items.value
    .filter(item => item.isActive && (item.isPopular || item.orderCount > 0))
    .sort((a, b) => (b.orderCount || 0) - (a.orderCount || 0))
    .slice(0, 5) // Limit to 5 items
})

// Watch active category and scroll tab into view
watch(activeCategory, async (newCategoryId) => {
  if (newCategoryId) {
    await nextTick()
    const activeButton = document.querySelector(`button[data-category-id="${newCategoryId}"]`)
    if (activeButton) {
      activeButton.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' })
    }
  }
})

onMounted(async () => {
  // Redirect to new entity/catalog routing structure
  await redirectToNewRouting()
})

onUnmounted(() => {
  cleanupScrollSpy()
})

async function redirectToNewRouting() {
  try {
    loading.value = true
    const slug = route.params.slug as string

    // Try to find the catalog by slug and get its entity
    const api = useApi()

    // First, get the menu data to find entity information
    const response = await menusApi.getMenuBySlug(slug)

    if (response?.data?.restaurant && response.data.restaurant.entitySlug && response.data.restaurant.catalogSlug) {
      // If backend provides entity and catalog slugs, use them
      await router.replace(`/${response.data.restaurant.entitySlug}/${response.data.restaurant.catalogSlug}`)
    } else if (response?.data?.restaurant?.catalogId) {
      // Fallback: try to get catalog details
      const catalogId = response.data.restaurant.catalogId
      const catalogResponse = await api.get(`/api/v1/catalogs/${catalogId}`)

      if (catalogResponse?.data?.catalog && catalogResponse.data.catalog.entity) {
        const entitySlug = catalogResponse.data.catalog.entity.slug
        const catalogSlug = catalogResponse.data.catalog.slug || slug
        await router.replace(`/${entitySlug}/${catalogSlug}`)
      } else {
        // Can't determine new route, show error
        error.value = 'This menu is using the old format. Please contact support to migrate.'
      }
    } else {
      // Can't determine new route, show error
      error.value = 'This menu is using the old format. Please contact support to migrate.'
    }
  } catch (err: any) {
    console.error('Error redirecting to new routing:', err)
    error.value = 'Failed to load menu. Please try again.'
  } finally {
    loading.value = false
  }
}

async function loadMenu() {
  // This function is now replaced by redirectToNewRouting
  // Keeping it for reference, but it's no longer called
  try {
    loading.value = true
    const slug = route.params.slug as string
    const response = await menusApi.getMenuBySlug(slug)
    console.log('Menu response:', response)
    menuData.value = response.data.restaurant
    items.value = response.data.menu || []

    // Set first category as active
    if (categories.value.length > 0) {
      activeCategory.value = categories.value[0].id
    }
  } catch (err: any) {
    console.error('Error loading menu:', err)
    error.value = err.response?.data?.error || 'Failed to load menu'
  } finally {
    loading.value = false
  }
}

function getCategoryItems(categoryId: number) {
  return items.value
    .filter(item => item.categoryId === categoryId && item.isActive)
    .sort((a, b) => a.sortOrder - b.sortOrder)
}

function scrollToCategory(categoryId: number) {
  const element = document.getElementById(`category-${categoryId}`)
  if (element) {
    // Account for sticky category tabs (52px + border)
    const tabsHeight = 52
    const offset = tabsHeight + 5 // Extra padding
    const elementPosition = element.getBoundingClientRect().top
    const offsetPosition = elementPosition + window.pageYOffset - offset

    window.scrollTo({
      top: offsetPosition,
      behavior: 'smooth'
    })

    // Update active category immediately for better UX
    activeCategory.value = categoryId
  }
}

let scrollListener: (() => void) | null = null

function setupScrollSpy() {
  scrollListener = () => {
    const categoryElements = categories.value.map(cat => ({
      id: cat.id,
      element: document.getElementById(`category-${cat.id}`)
    }))

    // Account for sticky category tabs when detecting active section
    const headerOffset = 120 // Category tabs + category title
    const scrollPosition = window.scrollY + headerOffset

    for (let i = categoryElements.length - 1; i >= 0; i--) {
      const cat = categoryElements[i]
      if (cat.element && cat.element.offsetTop <= scrollPosition) {
        if (activeCategory.value !== cat.id) {
          activeCategory.value = cat.id
        }
        break
      }
    }
  }

  window.addEventListener('scroll', scrollListener, { passive: true })
}

function cleanupScrollSpy() {
  if (scrollListener) {
    window.removeEventListener('scroll', scrollListener)
  }
}

function openItemDetail(item: any) {
  if (item.itemType === 1) {
    // Bundle item - open bundle configuration modal
    openBundleConfig(item)
  } else {
    // Regular item - open item detail modal
    selectedItem.value = item
  }
}

async function openBundleConfig(item: any) {
  // TODO: Load full bundle data with steps and options from API
  // For now, assume item has bundleId and we need to fetch the bundle details
  try {
    // Placeholder: In production, fetch bundle data from API
    // const response = await bundlesApi.getBundle(catalogId, item.bundleId)
    // selectedBundle.value = response.data

    // For now, set a mock bundle structure
    selectedBundle.value = {
      id: item.id,
      name: item.name,
      description: item.description,
      basePrice: item.price,
      images: item.images,
      steps: item.bundleSteps || [] // Assuming bundle steps are loaded with the item
    }
  } catch (error) {
    console.error('Error loading bundle:', error)
    toast.error('Failed to load bundle configuration')
  }
}

function handleBundleAddToCart(bundleData: any) {
  cartStore.addBundleItem(bundleData)
  toast.success(`${bundleData.bundle.name} added to cart!`)
  cartStore.openCart()
}

function quickAddToCart(item: any) {
  cartStore.addItem({
    catalogItemId: item.id,
    name: item.name,
    description: item.description,
    basePrice: item.price,
    quantity: 1,
    options: [],
    image: item.images,
    isBundle: item.itemType === 1
  })
  toast.success(`${item.name} added to cart`)
}

function goBack() {
  router.back()
}

function toggleEditMode() {
  editMode.value = !editMode.value
  if (!editMode.value) {
    editingItem.value = null
  }
}

function editItem(item: any) {
  editingItem.value = item
}

async function handleItemSaved() {
  editingItem.value = null
  await loadMenu()
  toast.success('Item updated successfully')
}
</script>

<style scoped>
.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.hide-scrollbar::-webkit-scrollbar {
  display: none;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>




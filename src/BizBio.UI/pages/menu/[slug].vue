<template>
  <div class="min-h-screen bg-[var(--light-background-color)]">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center h-screen">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)]"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex flex-col justify-center items-center h-screen px-4">
      <i class="fas fa-exclamation-circle text-6xl text-red-500 mb-4"></i>
      <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">Menu Not Found</h2>
      <p class="text-[var(--gray-text-color)] text-center">{{ error }}</p>
    </div>

    <!-- Menu Content -->
    <div v-else>
      <!-- Hero Section with Restaurant Image -->
      <div v-if="menuData?.coverImage" class="relative h-48 md:h-64 bg-gradient-to-br from-gray-300 to-gray-400">
        <img
          :src="menuData.coverImage"
          :alt="menuData.name"
          class="w-full h-full object-cover"
        />
        <button
          @click="goBack"
          class="absolute top-4 left-4 w-10 h-10 bg-white bg-opacity-90 rounded-full shadow-lg flex items-center justify-center hover:bg-opacity-100 transition-all"
        >
          <i class="fas fa-arrow-left text-gray-700"></i>
        </button>
        <button
          v-if="isAuthenticated"
          @click="toggleEditMode"
          class="absolute top-4 right-4 px-4 py-2 bg-white bg-opacity-90 rounded-full shadow-lg hover:bg-opacity-100 transition-all text-sm font-medium"
        >
          <i class="fas fa-edit mr-2"></i>
          {{ editMode ? 'Done' : 'Edit' }}
        </button>
      </div>

      <!-- Restaurant Info Header -->
      <div class="bg-white shadow-sm">
        <div class="max-w-7xl mx-auto px-4 py-4">
          <div class="flex items-start justify-between mb-2">
            <div class="flex-1">
              <h1 class="text-2xl md:text-3xl font-bold text-[var(--dark-text-color)] mb-1">
                {{ menuData?.name || 'Menu' }}
              </h1>
              <p v-if="menuData?.businessName" class="text-sm text-[var(--gray-text-color)] mb-2">
                {{ menuData.businessName }}
              </p>
              <div class="flex items-center gap-4 text-sm text-[var(--gray-text-color)] flex-wrap">
                <div v-if="menuData?.rating" class="flex items-center gap-1">
                  <i class="fas fa-star text-yellow-500"></i>
                  <span class="font-semibold">{{ menuData.rating }}</span>
                  <span v-if="menuData?.reviewCount">({{ menuData.reviewCount }})</span>
                </div>
                <div v-if="menuData?.deliveryTime">
                  <i class="fas fa-clock mr-1"></i>
                  {{ menuData.deliveryTime }}
                </div>
                <div v-if="menuData?.distance">
                  <i class="fas fa-map-marker-alt mr-1"></i>
                  {{ menuData.distance }}
                </div>
              </div>
            </div>
            <button
              @click="cartStore.toggleCart()"
              class="relative ml-4 p-3 bg-[var(--primary-color)] text-white rounded-full hover:opacity-90 transition-opacity flex-shrink-0"
            >
              <i class="fas fa-search"></i>
            </button>
          </div>

          <p v-if="menuData?.description" class="text-sm text-[var(--gray-text-color)] mt-2">
            {{ menuData.description }}
          </p>

          <!-- Delivery Info Banner -->
          <div v-if="menuData?.deliveryInfo" class="mt-3 p-3 bg-red-50 border-l-4 border-red-500 rounded">
            <div class="flex items-center gap-2 text-sm text-red-700">
              <i class="fas fa-motorcycle"></i>
              <span class="font-medium">{{ menuData.deliveryInfo }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Category Tabs (Sticky) -->
      <div v-if="categories.length > 0" class="sticky top-0 z-40 bg-white shadow-sm">
        <div class="overflow-x-auto hide-scrollbar">
          <div class="flex space-x-1 px-4 py-3 min-w-max border-b border-gray-200">
            <button
              v-for="category in categories"
              :key="category.id"
              @click="scrollToCategory(category.id)"
              :class="[
                'px-4 py-2 text-sm font-medium whitespace-nowrap transition-all relative',
                activeCategory === category.id
                  ? 'text-[var(--primary-color)]'
                  : 'text-[var(--gray-text-color)] hover:text-[var(--dark-text-color)]'
              ]"
            >
              {{ category.name }}
              <div
                v-if="activeCategory === category.id"
                class="absolute bottom-0 left-0 right-0 h-0.5 bg-[var(--primary-color)]"
              ></div>
            </button>
          </div>
        </div>
      </div>

      <!-- Menu Items by Category -->
      <div class="max-w-7xl mx-auto pb-24">
        <div
          v-for="category in categories"
          :key="category.id"
          :id="`category-${category.id}`"
          class="mb-6"
        >
          <h2 class="text-xl font-bold text-[var(--dark-text-color)] px-4 py-4 bg-[var(--light-background-color)] sticky top-14 z-10">
            {{ category.name }}
          </h2>

          <div class="bg-white divide-y divide-gray-100">
            <div
              v-for="item in getCategoryItems(category.id)"
              :key="item.id"
              @click="openItemDetail(item)"
              class="px-4 py-4 hover:bg-gray-50 transition-colors cursor-pointer relative"
            >
              <div class="flex gap-4">
                <!-- Item Info -->
                <div class="flex-1 min-w-0">
                  <div class="flex items-start gap-2 mb-1">
                    <h3 class="text-base font-semibold text-[var(--dark-text-color)]">{{ item.name }}</h3>
                    <span
                      v-if="item.itemType === 1"
                      class="px-2 py-0.5 bg-orange-100 text-orange-700 text-xs font-semibold rounded flex-shrink-0"
                    >
                      BUNDLE
                    </span>
                  </div>
                  <p v-if="item.description" class="text-sm text-[var(--gray-text-color)] line-clamp-2 mb-2">
                    {{ item.description }}
                  </p>
                  <div class="flex items-center gap-2">
                    <span class="text-base font-bold text-[var(--dark-text-color)]">
                      From R{{ item.price.toFixed(2) }}
                    </span>
                  </div>
                  <!-- Edit Button (when in edit mode) -->
                  <button
                    v-if="editMode"
                    @click.stop="editItem(item)"
                    class="mt-2 px-3 py-1.5 bg-blue-600 text-white text-xs font-medium rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    <i class="fas fa-edit mr-1"></i>
                    Edit Item
                  </button>
                </div>

                <!-- Item Image -->
                <div class="flex-shrink-0 w-24 h-24 md:w-32 md:h-32 bg-gradient-to-br from-gray-200 to-gray-300 rounded-lg overflow-hidden">
                  <img
                    v-if="item.images"
                    :src="item.images"
                    :alt="item.name"
                    class="w-full h-full object-cover"
                  />
                  <div v-else class="w-full h-full flex items-center justify-center">
                    <i class="fas fa-utensils text-2xl md:text-3xl text-gray-400"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty Category -->
          <div v-if="getCategoryItems(category.id).length === 0" class="text-center py-12 text-[var(--gray-text-color)] bg-white">
            <i class="fas fa-box-open text-5xl mb-3 opacity-50"></i>
            <p class="text-sm">No items in this category</p>
          </div>
        </div>
      </div>

      <!-- Floating Cart Button (Mobile) -->
      <div
        v-if="cartStore.itemCount > 0"
        @click="cartStore.openCart()"
        class="fixed bottom-6 left-4 right-4 md:left-auto md:right-6 md:max-w-md z-30 bg-[var(--primary-color)] text-white rounded-2xl shadow-2xl px-6 py-4 cursor-pointer hover:opacity-95 transition-opacity"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
              <i class="fas fa-shopping-cart text-lg"></i>
            </div>
            <span class="font-semibold text-lg">{{ cartStore.itemCount }} {{ cartStore.itemCount === 1 ? 'item' : 'items' }}</span>
          </div>
          <div class="font-bold text-xl">R{{ cartStore.total.toFixed(2) }}</div>
        </div>
      </div>
    </div>

    <!-- Item Detail Modal -->
    <ItemDetailModal
      v-if="selectedItem"
      :item="selectedItem"
      :menu-slug="route.params.slug as string"
      :edit-mode="editMode"
      @close="selectedItem = null"
      @edit="editItem"
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
import { ref, computed, onMounted, onUnmounted } from 'vue'
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
const activeCategory = ref<number | null>(null)
const editMode = ref(false)
const editingItem = ref<any>(null)

const isAuthenticated = computed(() => authStore.isAuthenticated)

const categories = computed(() => {
  if (!menuData.value?.categories) return []
  return menuData.value.categories.sort((a: any, b: any) => a.sortOrder - b.sortOrder)
})

onMounted(async () => {
  await loadMenu()
  setupScrollSpy()
})

onUnmounted(() => {
  cleanupScrollSpy()
})

async function loadMenu() {
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
    const offset = 60 // Adjust for sticky headers
    const elementPosition = element.getBoundingClientRect().top
    const offsetPosition = elementPosition + window.pageYOffset - offset

    window.scrollTo({
      top: offsetPosition,
      behavior: 'smooth'
    })
  }
}

let scrollListener: (() => void) | null = null

function setupScrollSpy() {
  scrollListener = () => {
    const categoryElements = categories.value.map(cat => ({
      id: cat.id,
      element: document.getElementById(`category-${cat.id}`)
    }))

    const scrollPosition = window.scrollY + 150

    for (let i = categoryElements.length - 1; i >= 0; i--) {
      const cat = categoryElements[i]
      if (cat.element && cat.element.offsetTop <= scrollPosition) {
        activeCategory.value = cat.id
        break
      }
    }
  }

  window.addEventListener('scroll', scrollListener)
}

function cleanupScrollSpy() {
  if (scrollListener) {
    window.removeEventListener('scroll', scrollListener)
  }
}

function openItemDetail(item: any) {
  selectedItem.value = item
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

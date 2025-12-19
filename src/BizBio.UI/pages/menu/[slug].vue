<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center h-screen">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)]"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex flex-col justify-center items-center h-screen px-4">
      <i class="fas fa-exclamation-circle text-6xl text-red-500 mb-4"></i>
      <h2 class="text-2xl font-bold text-gray-800 mb-2">Menu Not Found</h2>
      <p class="text-gray-600 text-center">{{ error }}</p>
    </div>

    <!-- Menu Content -->
    <div v-else>
      <!-- Sticky Header -->
      <div class="sticky top-0 z-40 bg-white shadow-sm">
        <div class="max-w-7xl mx-auto px-4 py-4">
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <h1 class="text-xl font-bold text-gray-900">{{ menuData?.name || 'Menu' }}</h1>
              <p v-if="menuData?.businessName" class="text-sm text-gray-600 truncate">{{ menuData.businessName }}</p>
            </div>
            <button
              @click="cartStore.toggleCart()"
              class="relative ml-4 p-3 bg-[var(--primary-color)] text-white rounded-full hover:bg-[var(--secondary-color)] transition-colors"
            >
              <i class="fas fa-shopping-cart"></i>
              <span
                v-if="cartStore.itemCount > 0"
                class="absolute -top-1 -right-1 bg-red-500 text-white text-xs font-bold rounded-full h-5 w-5 flex items-center justify-center"
              >
                {{ cartStore.itemCount }}
              </span>
            </button>
          </div>
        </div>

        <!-- Category Tabs -->
        <div v-if="categories.length > 0" class="overflow-x-auto hide-scrollbar border-t border-gray-200">
          <div class="flex space-x-2 px-4 py-2 min-w-max">
            <button
              v-for="category in categories"
              :key="category.id"
              @click="scrollToCategory(category.id)"
              :class="[
                'px-4 py-2 rounded-full text-sm font-medium whitespace-nowrap transition-colors',
                activeCategory === category.id
                  ? 'bg-[var(--primary-color)] text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              {{ category.name }}
            </button>
          </div>
        </div>
      </div>

      <!-- Menu Items by Category -->
      <div class="max-w-7xl mx-auto px-4 py-6 pb-24">
        <div
          v-for="category in categories"
          :key="category.id"
          :id="`category-${category.id}`"
          class="mb-8"
        >
          <h2 class="text-2xl font-bold text-gray-900 mb-4 sticky top-32 bg-gray-50 py-2 z-10">
            {{ category.name }}
          </h2>

          <div class="space-y-4">
            <div
              v-for="item in getCategoryItems(category.id)"
              :key="item.id"
              @click="openItemDetail(item)"
              class="bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow cursor-pointer overflow-hidden"
            >
              <div class="flex p-4">
                <!-- Item Info -->
                <div class="flex-1 min-w-0 pr-4">
                  <div class="flex items-start gap-2 mb-1">
                    <h3 class="text-lg font-semibold text-gray-900">{{ item.name }}</h3>
                    <span
                      v-if="item.itemType === 1"
                      class="px-2 py-0.5 bg-orange-100 text-orange-800 text-xs font-semibold rounded"
                    >
                      BUNDLE
                    </span>
                  </div>
                  <p v-if="item.description" class="text-sm text-gray-600 line-clamp-2 mb-2">
                    {{ item.description }}
                  </p>
                  <div class="flex items-center justify-between">
                    <span class="text-lg font-bold text-[var(--primary-color)]">
                      R{{ item.price.toFixed(2) }}
                    </span>
                    <!-- Quick Add Button for simple items -->
                    <button
                      v-if="!item.hasVariants && !item.hasOptions && item.itemType !== 1"
                      @click.stop="quickAddToCart(item)"
                      class="px-4 py-2 bg-[var(--primary-color)] text-white text-sm font-medium rounded-lg hover:bg-[var(--secondary-color)] transition-colors"
                    >
                      <i class="fas fa-plus mr-1"></i>
                      Add
                    </button>
                  </div>
                </div>

                <!-- Item Image -->
                <div class="flex-shrink-0 w-24 h-24 bg-gradient-to-br from-gray-200 to-gray-300 rounded-lg overflow-hidden">
                  <img
                    v-if="item.images"
                    :src="item.images"
                    :alt="item.name"
                    class="w-full h-full object-cover"
                  />
                  <div v-else class="w-full h-full flex items-center justify-center">
                    <i class="fas fa-utensils text-2xl text-gray-400"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty Category -->
          <div v-if="getCategoryItems(category.id).length === 0" class="text-center py-8 text-gray-500">
            <i class="fas fa-box-open text-4xl mb-2"></i>
            <p>No items in this category</p>
          </div>
        </div>
      </div>

      <!-- Floating Cart Button (Mobile) -->
      <div
        v-if="cartStore.itemCount > 0"
        @click="cartStore.openCart()"
        class="fixed bottom-6 left-4 right-4 md:left-auto md:right-6 md:w-auto z-30 bg-[var(--primary-color)] text-white rounded-full shadow-lg px-6 py-4 cursor-pointer hover:bg-[var(--secondary-color)] transition-colors"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <i class="fas fa-shopping-cart text-xl mr-3"></i>
            <span class="font-semibold">{{ cartStore.itemCount }} {{ cartStore.itemCount === 1 ? 'item' : 'items' }}</span>
          </div>
          <div class="font-bold text-lg">R{{ cartStore.total.toFixed(2) }}</div>
        </div>
      </div>
    </div>

    <!-- Item Detail Modal -->
    <ItemDetailModal
      v-if="selectedItem"
      :item="selectedItem"
      :menu-slug="route.params.slug as string"
      @close="selectedItem = null"
    />

    <!-- Cart Drawer -->
    <CartDrawer />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { useMenusApi } from '~/composables/useApi'
import { useCartStore } from '~/stores/cart'
import { useToast } from '~/composables/useToast'

const route = useRoute()
const menusApi = useMenusApi()
const cartStore = useCartStore()
const toast = useToast()

const loading = ref(true)
const error = ref<string | null>(null)
const menuData = ref<any>(null)
const items = ref<any[]>([])
const selectedItem = ref<any>(null)
const activeCategory = ref<number | null>(null)

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
    menuData.value = response.data.menu
    items.value = response.data.items || []

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
    const offset = 140 // Header + tabs height
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

    const scrollPosition = window.scrollY + 200

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

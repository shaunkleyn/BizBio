<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center h-screen bg-white">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mx-auto mb-4"></div>
        <p class="text-gray-600 font-medium">Loading catalog...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex flex-col justify-center items-center h-screen px-4 bg-white">
      <i class="fas fa-exclamation-circle text-6xl text-red-500 mb-4"></i>
      <h2 class="text-2xl font-bold text-gray-900 mb-2">Catalog Not Found</h2>
      <p class="text-gray-600 text-center">{{ error }}</p>
      <button
        @click="router.push('/')"
        class="mt-6 px-6 py-3 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
      >
        Go to Home
      </button>
    </div>

    <!-- Catalog Content -->
    <div v-else>
      <!-- Hero Section with Cover Image -->
      <div v-if="entityData?.logo || catalogData?.coverImage" class="relative h-56 md:h-72 bg-gradient-to-br from-gray-200 to-gray-300">
        <img
          :src="catalogData?.coverImage || entityData?.logo"
          :alt="catalogData?.name || entityData?.name"
          class="w-full h-full object-cover"
        />
        <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent"></div>

        <!-- Navigation Buttons -->
        <div class="absolute top-4 left-4 right-4 flex items-center justify-between">
          <button
            @click="router.back()"
            class="w-10 h-10 bg-white bg-opacity-95 rounded-full shadow-lg flex items-center justify-center hover:bg-opacity-100 transition-all"
          >
            <i class="fas fa-arrow-left text-gray-900"></i>
          </button>

          <!-- Catalog Switcher (if multiple catalogs) -->
          <div v-if="otherCatalogs.length > 0" class="relative">
            <select
              v-model="selectedCatalogId"
              @change="switchCatalog"
              class="pl-4 pr-10 py-2 bg-white bg-opacity-95 rounded-full shadow-lg text-sm font-semibold text-gray-900 border-none focus:outline-none focus:ring-2 focus:ring-gray-900"
            >
              <option :value="catalogData.id">{{ catalogData.name }}</option>
              <option v-for="catalog in otherCatalogs" :key="catalog.id" :value="catalog.id">
                {{ catalog.name }}
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Entity Info Header -->
      <div class="bg-white border-b border-gray-200">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <!-- Entity Name and Catalog Name -->
          <div class="mb-4">
            <h1 class="text-3xl md:text-4xl font-bold text-gray-900 mb-2">
              {{ entityData?.name || 'Business' }}
            </h1>
            <h2 class="text-xl md:text-2xl font-semibold text-gray-700">
              {{ catalogData?.name || 'Catalog' }}
            </h2>
          </div>

          <!-- Entity Description -->
          <p v-if="entityData?.description" class="text-gray-600 mb-4">
            {{ entityData.description }}
          </p>

          <!-- Address & Contact -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div v-if="entityData?.address" class="flex items-start gap-3">
              <i class="fas fa-map-marker-alt text-gray-400 mt-1"></i>
              <div>
                <div class="text-sm font-medium text-gray-900">Address</div>
                <div class="text-sm text-gray-600">
                  {{ entityData.address }}
                  <span v-if="entityData.city">, {{ entityData.city }}</span>
                  <span v-if="entityData.postalCode"> {{ entityData.postalCode }}</span>
                </div>
              </div>
            </div>

            <div v-if="entityData?.phone || entityData?.email" class="flex items-start gap-3">
              <i class="fas fa-phone text-gray-400 mt-1"></i>
              <div>
                <div class="text-sm font-medium text-gray-900">Contact</div>
                <div class="text-sm text-gray-600">
                  <div v-if="entityData.phone">{{ entityData.phone }}</div>
                  <div v-if="entityData.email">{{ entityData.email }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Category Navigation (Sticky) -->
      <div v-if="categories.length > 0" class="sticky top-0 z-30 bg-white border-b border-gray-200">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex gap-2 overflow-x-auto py-3 scrollbar-hide">
            <button
              v-for="category in categories"
              :key="category.id"
              :data-category-id="category.id"
              @click="scrollToCategory(category.id)"
              :class="[
                'px-4 py-2 rounded-full font-medium text-sm whitespace-nowrap transition-colors',
                activeCategory === category.id
                  ? 'bg-gray-900 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              <i v-if="category.icon" :class="['fas', category.icon, 'mr-2']"></i>
              {{ category.name }}
            </button>
          </div>
        </div>
      </div>

      <!-- Catalog Items by Category -->
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div v-for="category in categories" :key="category.id" :id="`category-${category.id}`" class="mb-12">
          <h3 class="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-3">
            <i v-if="category.icon" :class="['fas', category.icon, 'text-gray-600']"></i>
            {{ category.name }}
          </h3>

          <!-- Items Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div
              v-for="item in getCategoryItems(category.id)"
              :key="item.id"
              class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow cursor-pointer"
              @click="openItemDetail(item)"
            >
              <!-- Item Image -->
              <div v-if="item.images && item.images.length > 0" class="aspect-video bg-gray-200">
                <img :src="item.images[0]" :alt="item.name" class="w-full h-full object-cover" />
              </div>
              <div v-else class="aspect-video bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
                <i class="fas fa-image text-4xl text-gray-400"></i>
              </div>

              <!-- Item Content -->
              <div class="p-4">
                <h4 class="text-lg font-semibold text-gray-900 mb-2">{{ item.name }}</h4>
                <p v-if="item.description" class="text-sm text-gray-600 mb-3 line-clamp-2">
                  {{ item.description }}
                </p>

                <!-- Tags -->
                <div v-if="item.tags && item.tags.length > 0" class="flex flex-wrap gap-2 mb-3">
                  <span
                    v-for="tag in item.tags.slice(0, 3)"
                    :key="tag"
                    class="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded-full"
                  >
                    {{ tag }}
                  </span>
                </div>

                <!-- Price -->
                <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                  <div class="flex flex-col">
                    <span v-if="item.isSharedItem && item.priceOverride" class="text-xs text-gray-500 line-through">
                      R{{ item.price.toFixed(2) }}
                    </span>
                    <span class="text-xl font-bold text-gray-900">
                      R{{ item.effectivePrice?.toFixed(2) || item.price.toFixed(2) }}
                    </span>
                  </div>
                  <button
                    class="px-4 py-2 bg-gray-900 text-white text-sm font-semibold rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    View
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty Category -->
          <div v-if="getCategoryItems(category.id).length === 0" class="text-center py-16 text-gray-500 bg-white rounded-lg">
            <i class="fas fa-box-open text-6xl mb-4 opacity-30"></i>
            <p class="text-base font-medium">No items in this category</p>
          </div>
        </div>

        <!-- No Categories -->
        <div v-if="categories.length === 0" class="text-center py-20">
          <i class="fas fa-folder-open text-6xl text-gray-300 mb-4"></i>
          <h3 class="text-xl font-semibold text-gray-700 mb-2">No Categories Yet</h3>
          <p class="text-gray-500">This catalog doesn't have any categories or items yet.</p>
        </div>
      </div>
    </div>

    <!-- Item Detail Modal (placeholder) -->
    <div v-if="selectedItem" class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4" @click="selectedItem = null">
      <div class="bg-white rounded-lg max-w-2xl w-full p-6" @click.stop>
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-2xl font-bold">{{ selectedItem.name }}</h3>
          <button @click="selectedItem = null" class="text-gray-500 hover:text-gray-700">
            <i class="fas fa-times text-xl"></i>
          </button>
        </div>
        <p class="text-gray-600">{{ selectedItem.description }}</p>
        <div class="mt-6 text-2xl font-bold">R{{ selectedItem.effectivePrice?.toFixed(2) || selectedItem.price.toFixed(2) }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useHead } from '#app'

definePageMeta({
  layout: false,
  middleware: []
})

const route = useRoute()
const router = useRouter()

const loading = ref(true)
const error = ref<string | null>(null)
const entityData = ref<any>(null)
const catalogData = ref<any>(null)
const categories = ref<any[]>([])
const items = ref<any[]>([])
const allCatalogs = ref<any[]>([])
const activeCategory = ref<number | null>(null)
const selectedItem = ref<any>(null)
const selectedCatalogId = ref<number | null>(null)

const otherCatalogs = computed(() => {
  if (!catalogData.value) return []
  return allCatalogs.value.filter(c => c.id !== catalogData.value.id && c.isActive)
})

// SEO Meta Tags
useHead(() => ({
  title: `${catalogData.value?.name || 'Catalog'} - ${entityData.value?.name || 'BizBio'}`,
  meta: [
    { name: 'description', content: catalogData.value?.description || entityData.value?.description || 'View our catalog' },
    { property: 'og:title', content: `${catalogData.value?.name || 'Catalog'} - ${entityData.value?.name}` },
    { property: 'og:description', content: catalogData.value?.description || entityData.value?.description },
    { property: 'og:image', content: catalogData.value?.coverImage || entityData.value?.logo },
    { property: 'og:type', content: 'website' },
    { name: 'twitter:card', content: 'summary_large_image' },
    { name: 'twitter:title', content: `${catalogData.value?.name} - ${entityData.value?.name}` },
    { name: 'twitter:description', content: catalogData.value?.description || entityData.value?.description },
    { name: 'twitter:image', content: catalogData.value?.coverImage || entityData.value?.logo }
  ]
}))

onMounted(async () => {
  await loadCatalog()
  setupScrollSpy()
})

onUnmounted(() => {
  cleanupScrollSpy()
})

async function loadCatalog() {
  try {
    loading.value = true
    const entitySlug = route.params.entitySlug as string
    const catalogSlug = route.params.catalogSlug as string

    const api = useApi()

    // Load entity
    const entityResponse = await api.get(`/api/v1/entities/by-slug/${entitySlug}`)
    if (!entityResponse.success || !entityResponse.data?.entity) {
      error.value = 'Entity not found'
      return
    }
    entityData.value = entityResponse.data.entity

    // Load all catalogs for this entity
    const catalogsResponse = await api.get(`/api/v1/entities/${entityData.value.id}/catalogs`)
    if (catalogsResponse.success && catalogsResponse.data?.catalogs) {
      allCatalogs.value = catalogsResponse.data.catalogs
    }

    // Find the specific catalog
    const catalog = allCatalogs.value.find(c => c.slug === catalogSlug && c.isActive)
    if (!catalog) {
      error.value = 'Catalog not found'
      return
    }
    catalogData.value = catalog
    selectedCatalogId.value = catalog.id

    // Load categories for this catalog
    const categoriesResponse = await api.get(`/api/v1/categories/catalog/${catalog.id}`)
    if (categoriesResponse.success && categoriesResponse.data?.categories) {
      categories.value = categoriesResponse.data.categories.sort((a: any, b: any) => a.sortOrder - b.sortOrder)
    }

    // Load items for this catalog
    const itemsResponse = await api.get(`/api/v1/catalogs/${catalog.id}/items`)
    if (itemsResponse.success && itemsResponse.data?.items) {
      items.value = itemsResponse.data.items
    }

    // Set first category as active
    if (categories.value.length > 0) {
      activeCategory.value = categories.value[0].id
    }
  } catch (err: any) {
    console.error('Error loading catalog:', err)
    error.value = err.response?.data?.error || 'Failed to load catalog'
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
    const tabsHeight = 52
    const offset = tabsHeight + 16
    const elementPosition = element.getBoundingClientRect().top
    const offsetPosition = elementPosition + window.pageYOffset - offset

    window.scrollTo({
      top: offsetPosition,
      behavior: 'smooth'
    })
  }
}

function openItemDetail(item: any) {
  selectedItem.value = item
}

async function switchCatalog() {
  if (!selectedCatalogId.value) return
  const catalog = allCatalogs.value.find(c => c.id === selectedCatalogId.value)
  if (catalog) {
    await router.push(`/${entityData.value.slug}/${catalog.slug}`)
    // Reload catalog data
    await loadCatalog()
  }
}

// Scroll spy functionality
let scrollSpyObserver: IntersectionObserver | null = null

function setupScrollSpy() {
  const options = {
    root: null,
    rootMargin: '-20% 0px -70% 0px',
    threshold: 0
  }

  scrollSpyObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        const categoryId = parseInt(entry.target.id.replace('category-', ''))
        activeCategory.value = categoryId
      }
    })
  }, options)

  // Observe all category sections
  nextTick(() => {
    categories.value.forEach(category => {
      const element = document.getElementById(`category-${category.id}`)
      if (element && scrollSpyObserver) {
        scrollSpyObserver.observe(element)
      }
    })
  })
}

function cleanupScrollSpy() {
  if (scrollSpyObserver) {
    scrollSpyObserver.disconnect()
    scrollSpyObserver = null
  }
}

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
</script>

<style scoped>
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

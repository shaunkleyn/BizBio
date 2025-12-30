<template>
  <div class="relative" v-click-outside="close">
    <!-- Trigger Button -->
    <button
      type="button"
      @click="toggle"
      class="inline-flex items-center gap-2 px-4 py-2 bg-md-surface-container-high border border-md-outline-variant rounded-xl text-sm focus:ring-2 focus:ring-md-primary hover:bg-md-surface-container transition-colors"
    >
      <i class="fas fa-plus text-xs"></i>
      <span>Add Products</span>
      <span v-if="selectedCount > 0" class="ml-1 px-2 py-0.5 bg-md-primary text-md-on-primary rounded-full text-xs font-bold">
        {{ selectedCount }}
      </span>
      <i class="fas fa-chevron-down text-xs ml-1"></i>
    </button>

    <!-- Dropdown Panel -->
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
        class="absolute right-0 mt-2 w-80 bg-md-surface rounded-xl shadow-lg border border-md-outline-variant z-50 max-h-96 overflow-hidden flex flex-col"
      >
        <!-- Search Box -->
        <div class="p-3 border-b border-md-outline-variant">
          <div class="relative">
            <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-md-on-surface-variant text-sm"></i>
            <input
              ref="searchInput"
              v-model="searchQuery"
              type="text"
              placeholder="Search products..."
              class="w-full pl-9 pr-3 py-2 bg-md-surface-container border border-md-outline-variant rounded-lg text-sm focus:ring-2 focus:ring-md-primary focus:outline-none"
            />
          </div>
        </div>

        <!-- Categories & Products List -->
        <div class="overflow-y-auto flex-1 p-2">
          <div v-if="filteredCategories.length === 0" class="text-center py-8 text-md-on-surface-variant text-sm">
            <i class="fas fa-search text-2xl mb-2 opacity-50"></i>
            <p>No products found</p>
          </div>

          <div v-else class="space-y-1">
            <!-- Category with Products -->
            <div v-for="category in filteredCategories" :key="category.id" class="mb-2">
              <!-- Category Header (Checkbox) -->
              <div
                class="flex items-center gap-2 px-3 py-2 hover:bg-md-surface-container rounded-lg cursor-pointer transition-colors"
                @click="toggleCategory(category)"
              >
                <input
                  type="checkbox"
                  :checked="isCategorySelected(category)"
                  :indeterminate.prop="isCategoryIndeterminate(category)"
                  @click.stop="toggleCategory(category)"
                  class="w-4 h-4 rounded border-md-outline text-md-primary focus:ring-2 focus:ring-md-primary"
                />
                <i 
                  class="fas fa-chevron-right text-xs text-md-on-surface-variant transition-transform"
                  :class="{ 'rotate-90': expandedCategories.has(category.id) }"
                  @click.stop="toggleCategoryExpand(category.id)"
                ></i>
                <span class="font-semibold text-md-on-surface text-sm flex-1">{{ category.name }}</span>
                <span class="text-xs text-md-on-surface-variant">{{ category.products.length }}</span>
              </div>

              <!-- Products in Category (Collapsible) -->
              <transition
                enter-active-class="transition-all duration-200"
                enter-from-class="max-h-0 opacity-0"
                enter-to-class="max-h-96 opacity-100"
                leave-active-class="transition-all duration-200"
                leave-from-class="max-h-96 opacity-100"
                leave-to-class="max-h-0 opacity-0"
              >
                <div v-if="expandedCategories.has(category.id)" class="ml-6 mt-1 space-y-1">
                  <div
                    v-for="product in category.products"
                    :key="product.id"
                    class="flex items-center gap-2 px-3 py-2 hover:bg-md-surface-container rounded-lg cursor-pointer transition-colors"
                    :class="{ 'bg-md-surface-container': tempSelected.has(product.id) }"
                    @click="toggleProduct(product.id)"
                  >
                    <input
                      type="checkbox"
                      :checked="tempSelected.has(product.id)"
                      @click.stop="toggleProduct(product.id)"
                      class="w-4 h-4 rounded border-md-outline text-md-primary focus:ring-2 focus:ring-md-primary"
                    />
                    <i class="fas fa-utensils text-md-on-surface-variant text-xs"></i>
                    <span class="text-sm text-md-on-surface flex-1">{{ product.name }}</span>
                    <span v-if="product.price" class="text-xs text-md-on-surface-variant">R{{ product.price }}</span>
                  </div>
                </div>
              </transition>
            </div>
          </div>
        </div>

        <!-- Footer Actions -->
        <div class="p-3 border-t border-md-outline-variant flex items-center justify-between bg-md-surface-container">
          <button
            @click="clearSelection"
            class="text-sm text-md-on-surface-variant hover:text-md-on-surface"
          >
            Clear
          </button>
          <div class="flex gap-2">
            <button
              @click="close"
              class="px-4 py-2 text-sm text-md-on-surface hover:bg-md-surface-container rounded-lg transition-colors"
            >
              Cancel
            </button>
            <button
              @click="applySelection"
              :disabled="tempSelected.size === 0"
              class="px-4 py-2 text-sm bg-md-primary text-md-on-primary rounded-lg hover:bg-md-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Add Selected ({{ tempSelected.size }})
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

interface Product {
  id: number
  name: string
  price?: number
  categoryId?: number
  categoryName?: string
}

interface Category {
  id: number | string
  name: string
  products: Product[]
}

const props = defineProps<{
  availableProducts: Product[]
  alreadyAddedIds: number[]
}>()

const emit = defineEmits(['add-products'])

const isOpen = ref(false)
const searchQuery = ref('')
const searchInput = ref<HTMLInputElement | null>(null)
const tempSelected = ref(new Set<number>())
const expandedCategories = ref(new Set<number | string>())

const selectedCount = computed(() => tempSelected.value.size)

// Group products by category
const categorizedProducts = computed(() => {
  const categoryMap = new Map<number | string, Category>()
  
  props.availableProducts.forEach(product => {
    const categoryId = product.categoryId || 'uncategorized'
    const categoryName = product.categoryName || 'Uncategorized'
    
    if (!categoryMap.has(categoryId)) {
      categoryMap.set(categoryId, {
        id: categoryId,
        name: categoryName,
        products: []
      })
    }
    
    // Only add if not already in the step
    if (!props.alreadyAddedIds.includes(product.id)) {
      categoryMap.get(categoryId)!.products.push(product)
    }
  })
  
  // Convert to array and sort categories alphabetically
  const categories = Array.from(categoryMap.values())
    .filter(cat => cat.products.length > 0)
    .sort((a, b) => a.name.localeCompare(b.name))
  
  // Sort products within each category alphabetically
  categories.forEach(category => {
    category.products.sort((a, b) => a.name.localeCompare(b.name))
  })
  
  return categories
})

// Filter categories based on search
const filteredCategories = computed(() => {
  if (!searchQuery.value.trim()) {
    return categorizedProducts.value
  }
  
  const query = searchQuery.value.toLowerCase()
  return categorizedProducts.value
    .map(category => {
      // Check if category name matches
      const categoryMatches = category.name.toLowerCase().includes(query)
      
      // Filter products that match
      const matchingProducts = category.products.filter(product =>
        product.name.toLowerCase().includes(query)
      )
      
      // If category name matches, show all products in that category
      // Otherwise, only show matching products
      return {
        ...category,
        products: categoryMatches ? category.products : matchingProducts
      }
    })
    .filter(category => category.products.length > 0)
})

const toggle = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    setTimeout(() => searchInput.value?.focus(), 100)
  }
}

const close = () => {
  isOpen.value = false
  searchQuery.value = ''
  tempSelected.value.clear()
}

const toggleProduct = (productId: number) => {
  if (tempSelected.value.has(productId)) {
    tempSelected.value.delete(productId)
  } else {
    tempSelected.value.add(productId)
  }
}

const toggleCategory = (category: Category) => {
  const allSelected = category.products.every(p => tempSelected.value.has(p.id))
  
  if (allSelected) {
    // Deselect all
    category.products.forEach(p => tempSelected.value.delete(p.id))
  } else {
    // Select all
    category.products.forEach(p => tempSelected.value.add(p.id))
  }
}

const toggleCategoryExpand = (categoryId: number | string) => {
  if (expandedCategories.value.has(categoryId)) {
    expandedCategories.value.delete(categoryId)
  } else {
    expandedCategories.value.add(categoryId)
  }
}

const isCategorySelected = (category: Category): boolean => {
  return category.products.length > 0 && category.products.every(p => tempSelected.value.has(p.id))
}

const isCategoryIndeterminate = (category: Category): boolean => {
  const selectedCount = category.products.filter(p => tempSelected.value.has(p.id)).length
  return selectedCount > 0 && selectedCount < category.products.length
}

const clearSelection = () => {
  tempSelected.value.clear()
}

const applySelection = () => {
  const selectedIds = Array.from(tempSelected.value)
  emit('add-products', selectedIds)
  close()
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

// Auto-expand first category when opened
watch(isOpen, (newVal) => {
  if (newVal && filteredCategories.value.length > 0) {
    filteredCategories.value.forEach(cat => {
      expandedCategories.value.add(cat.id)
    })
  }
})
</script>

<style scoped>
/* Indeterminate checkbox state */
input[type="checkbox"]:indeterminate {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 16 16'%3e%3cpath stroke='white' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M4 8h8'/%3e%3c/svg%3e");
  background-color: currentColor;
  border-color: transparent;
}
</style>

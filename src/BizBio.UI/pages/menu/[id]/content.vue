<template>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-6 flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-md-on-surface">Menu Content Editor</h1>
          <p class="text-md-on-surface-variant mt-1">Manage categories, items, and bundles</p>
        </div>
        <NuxtLink
          :to="`/menu/${catalogId}/edit`"
          class="px-4 py-2 text-md-on-surface border border-gray-300 rounded-lg hover:bg-md-surface-container"
        >
          <i class="fas fa-cog mr-2"></i>Menu Settings
        </NuxtLink>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-12 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)] mx-auto"></div>
        <p class="text-md-on-surface-variant mt-4">Loading menu...</p>
      </div>

      <!-- Menu Editor -->
      <div v-else class="space-y-6">
        <!-- Categories Section -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-md-on-surface">Categories</h2>
            <button
              @click="showAddCategoryDialog = true"
              class="px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors text-sm"
            >
              <i class="fas fa-plus mr-2"></i>Add Category
            </button>
          </div>

          <div v-if="categories.length === 0" class="text-center py-8 text-gray-500">
            No categories yet. Add your first category to organize your menu.
          </div>

          <div v-else class="space-y-2">
            <div
              v-for="(category, index) in categories"
              :key="category.id"
              class="flex items-center gap-3 p-4 bg-md-surface-container rounded-lg border border-md-outline-variant hover:border-[var(--primary-color)] transition-colors"
            >
              <i class="fas fa-grip-vertical text-md-on-surface-variant opacity-70"></i>
              <i v-if="category.icon" :class="category.icon" class="text-lg text-md-on-surface-variant"></i>
              <div class="flex-1">
                <h3 class="font-semibold text-md-on-surface">{{ category.name }}</h3>
                <p v-if="category.description" class="text-sm text-md-on-surface-variant">{{ category.description }}</p>
                <p class="text-xs text-gray-500 mt-1">{{ category.itemCount || 0 }} items</p>
              </div>
              <div class="flex items-center gap-2">
                <button
                  @click="editCategory(category)"
                  class="p-2 text-md-on-surface-variant hover:text-[var(--primary-color)] transition-colors"
                >
                  <i class="fas fa-edit"></i>
                </button>
                <button
                  @click="deleteCategory(category.id)"
                  class="p-2 text-md-on-surface-variant hover:text-red-500 transition-colors"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Items Section -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-md-on-surface">Menu Items</h2>
            <button
              @click="openAddItemDialog"
              class="px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors text-sm"
            >
              <i class="fas fa-plus mr-2"></i>Add Item
            </button>
          </div>

          <div v-if="items.length === 0" class="text-center py-8 text-gray-500">
            No items yet. Add items from your library to the menu.
          </div>

          <div v-else class="space-y-2">
            <div
              v-for="item in items"
              :key="item.id"
              class="flex items-center gap-3 p-4 bg-md-surface-container rounded-lg border border-md-outline-variant hover:border-[var(--primary-color)] transition-colors"
            >
              <i class="fas fa-grip-vertical text-md-on-surface-variant opacity-70"></i>
              <img
                v-if="item.images && item.images.length > 0"
                :src="item.images[0]"
                :alt="item.name"
                class="w-16 h-16 object-cover rounded-lg"
              />
              <div v-else class="w-16 h-16 bg-md-surface-container rounded-lg flex items-center justify-center">
                <i class="fas fa-utensils text-md-on-surface-variant opacity-70"></i>
              </div>
              <div class="flex-1">
                <h3 class="font-semibold text-md-on-surface">{{ item.name }}</h3>
                <p v-if="item.description" class="text-sm text-md-on-surface-variant line-clamp-1">{{ item.description }}</p>
                <div class="flex items-center gap-2 mt-1">
                  <span class="text-sm font-semibold text-[var(--primary-color)]">R{{ item.price }}</span>
                  <span
                    v-for="categoryId in item.categoryIds"
                    :key="categoryId"
                    class="text-xs px-2 py-1 bg-md-primary-container text-md-on-primary-container rounded"
                  >
                    {{ getCategoryName(categoryId) }}
                  </span>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <button
                  @click="editItemCategories(item)"
                  class="p-2 text-md-on-surface-variant hover:text-[var(--primary-color)] transition-colors"
                  title="Edit categories"
                >
                  <i class="fas fa-tags"></i>
                </button>
                <button
                  @click="removeItem(item.id)"
                  class="p-2 text-md-on-surface-variant hover:text-red-500 transition-colors"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Bundles Section -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-md-on-surface">Bundles</h2>
            <button
              @click="openAddBundleDialog"
              class="px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors text-sm"
            >
              <i class="fas fa-plus mr-2"></i>Add Bundle
            </button>
          </div>

          <div v-if="bundles.length === 0" class="text-center py-8 text-gray-500">
            No bundles yet. Add bundles to create combo deals.
          </div>

          <div v-else class="space-y-2">
            <div
              v-for="bundle in bundles"
              :key="bundle.id"
              class="flex items-center gap-3 p-4 bg-md-surface-container rounded-lg border border-md-outline-variant hover:border-[var(--primary-color)] transition-colors"
            >
              <i class="fas fa-grip-vertical text-md-on-surface-variant opacity-70"></i>
              <img
                v-if="bundle.images && bundle.images.length > 0"
                :src="bundle.images[0]"
                :alt="bundle.name"
                class="w-16 h-16 object-cover rounded-lg"
              />
              <div v-else class="w-16 h-16 bg-md-surface-container rounded-lg flex items-center justify-center">
                <i class="fas fa-box-open text-md-on-surface-variant opacity-70"></i>
              </div>
              <div class="flex-1">
                <h3 class="font-semibold text-md-on-surface">{{ bundle.name }}</h3>
                <p v-if="bundle.description" class="text-sm text-md-on-surface-variant line-clamp-1">{{ bundle.description }}</p>
                <div class="flex items-center gap-2 mt-1">
                  <span class="text-sm font-semibold text-[var(--primary-color)]">R{{ bundle.basePrice }}</span>
                  <span
                    v-for="categoryId in bundle.categoryIds"
                    :key="categoryId"
                    class="text-xs px-2 py-1 bg-md-primary-container text-md-on-primary-container rounded"
                  >
                    {{ getCategoryName(categoryId) }}
                  </span>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <button
                  @click="editBundleCategories(bundle)"
                  class="p-2 text-md-on-surface-variant hover:text-[var(--primary-color)] transition-colors"
                  title="Edit categories"
                >
                  <i class="fas fa-tags"></i>
                </button>
                <button
                  @click="removeBundle(bundle.id)"
                  class="p-2 text-md-on-surface-variant hover:text-red-500 transition-colors"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Dialogs would go here - simplified for now -->
    <!-- Add Category Dialog -->
    <div v-if="showAddCategoryDialog" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="closeAddCategoryDialog">
      <div class="bg-md-surface rounded-2xl p-6 max-w-md w-full mx-4">
        <h3 class="text-xl font-bold mb-4">{{ editingCategory ? 'Edit Category' : 'Add Category' }}</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">Name</label>
            <input
              v-model="categoryForm.name"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              placeholder="e.g., Starters, Main Course"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">Description (optional)</label>
            <textarea
              v-model="categoryForm.description"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              rows="2"
              placeholder="Brief description"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-md-on-surface mb-1">Icon (optional)</label>
            <input
              v-model="categoryForm.icon"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              placeholder="e.g., fas fa-pizza-slice"
            />
          </div>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button
            @click="closeAddCategoryDialog"
            class="px-4 py-2 text-md-on-surface hover:bg-md-surface-container-low rounded-lg transition-colors"
          >
            Cancel
          </button>
          <button
            @click="saveCategory"
            class="px-4 py-2 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors"
          >
            {{ editingCategory ? 'Update' : 'Add' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth'
})

const route = useRoute()
const catalogId = computed(() => parseInt(route.params.id as string))

const loading = ref(true)
const categories = ref<any[]>([])
const items = ref<any[]>([])
const bundles = ref<any[]>([])

// Dialogs
const showAddCategoryDialog = ref(false)
const showAddItemDialog = ref(false)
const showAddBundleDialog = ref(false)

// Forms
const categoryForm = ref({ name: '', description: '', icon: '' })
const editingCategory = ref<any>(null)

onMounted(async () => {
  await loadMenu()
})

async function loadMenu() {
  loading.value = true
  try {
    const response = await $fetch(`/api/v1/menus/${catalogId.value}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    })

    if (response.success && response.data) {
      categories.value = response.data.categories || []
      items.value = response.data.items || []
      bundles.value = response.data.bundles || []
    }
  } catch (error) {
    console.error('Failed to load menu:', error)
  } finally {
    loading.value = false
  }
}

// Category functions
async function saveCategory() {
  try {
    if (editingCategory.value) {
      await $fetch(`/api/v1/menu-editor/categories/${editingCategory.value.id}`, {
        method: 'PUT',
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        body: categoryForm.value
      })
    } else {
      const response = await $fetch(`/api/v1/menu-editor/catalogs/${catalogId.value}/categories`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        body: { ...categoryForm.value, sortOrder: categories.value.length }
      })
      if (response.success && response.data) {
        categories.value.push({ ...response.data, itemCount: 0 })
      }
    }
    closeAddCategoryDialog()
    await loadMenu()
  } catch (error) {
    console.error('Failed to save category:', error)
  }
}

function editCategory(category: any) {
  editingCategory.value = category
  categoryForm.value = {
    name: category.name,
    description: category.description || '',
    icon: category.icon || ''
  }
  showAddCategoryDialog.value = true
}

async function deleteCategory(categoryId: number) {
  if (!confirm('Are you sure you want to delete this category?')) return

  try {
    await $fetch(`/api/v1/menu-editor/categories/${categoryId}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
    })
    await loadMenu()
  } catch (error) {
    console.error('Failed to delete category:', error)
  }
}

function closeAddCategoryDialog() {
  showAddCategoryDialog.value = false
  editingCategory.value = null
  categoryForm.value = { name: '', description: '', icon: '' }
}

// Item functions
function openAddItemDialog() {
  showAddItemDialog.value = true
  // Load library items would go here
}

function editItemCategories(item: any) {
  // Open edit categories dialog
}

async function removeItem(itemId: number) {
  if (!confirm('Remove this item from the menu?')) return

  try {
    await $fetch(`/api/v1/menu-editor/items/${itemId}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
    })
    await loadMenu()
  } catch (error) {
    console.error('Failed to remove item:', error)
  }
}

// Bundle functions
function openAddBundleDialog() {
  showAddBundleDialog.value = true
  // Load library bundles would go here
}

function editBundleCategories(bundle: any) {
  // Open edit categories dialog
}

async function removeBundle(bundleId: number) {
  if (!confirm('Remove this bundle from the menu?')) return

  try {
    await $fetch(`/api/v1/menu-editor/bundles/${bundleId}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
    })
    await loadMenu()
  } catch (error) {
    console.error('Failed to remove bundle:', error)
  }
}

// Utility
function getCategoryName(categoryId: number): string {
  const category = categories.value.find(c => c.id === categoryId)
  return category ? category.name : 'Unknown'
}

useHead({
  title: 'Menu Content Editor - Dashboard'
})
</script>







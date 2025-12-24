<template>
  <div class="p-4 md:p-8">
    <div class="max-w-4xl mx-auto">
      <!-- Loading State -->
      <div v-if="loading" class="flex flex-col justify-center items-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)] mb-4"></div>
        <p class="text-[var(--gray-text-color)]">Loading menu...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <i class="fas fa-exclamation-circle text-4xl text-red-500 mb-3"></i>
        <h3 class="text-xl font-semibold text-red-900 mb-2">Error Loading Menu</h3>
        <p class="text-red-700">{{ error }}</p>
        <NuxtLink
          to="/menu/menus"
          class="inline-block mt-4 px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Back to Menus
        </NuxtLink>
      </div>

      <!-- Edit Form -->
      <div v-else>
        <!-- Form -->
        <form @submit.prevent="saveMenu" class="bg-white rounded-lg shadow-md p-8">
          <div class="space-y-6">
          <!-- Menu Name -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Menu Name *
            </label>
            <input
              v-model="formData.name"
              type="text"
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              placeholder="e.g., Lunch Menu, Dinner Menu"
            />
          </div>
          </div>

          <!-- Slug -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              URL Slug *
            </label>
            <div class="flex items-center gap-2">
              <span class="text-[var(--gray-text-color)]">menu/</span>
              <input
                v-model="formData.slug"
                type="text"
                required
                pattern="[a-z0-9-]+"
                class="flex-1 px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                placeholder="lunch-menu"
              />
            </div>
            <p class="text-xs text-[var(--gray-text-color)] mt-1">
              Lowercase letters, numbers, and hyphens only
            </p>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Description
            </label>
            <textarea
              v-model="formData.description"
              rows="3"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              placeholder="A brief description of your menu"
            ></textarea>
          </div>

          <!-- Cover Image URL -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Cover Image URL
            </label>
            <input
              v-model="formData.coverImage"
              type="url"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              placeholder="https://example.com/image.jpg"
            />
          </div>

          <!-- Active Status -->
          <div class="flex items-center gap-3">
            <input
              v-model="formData.isActive"
              type="checkbox"
              id="isActive"
              class="w-5 h-5 text-[var(--primary-color)] rounded focus:ring-2 focus:ring-[var(--primary-color)]"
            />
            <label for="isActive" class="text-sm font-semibold text-[var(--dark-text-color)]">
              Active (visible to public)
            </label>
          </div>

          <!-- Delete Button -->
          <div class="mt-8 pt-6 border-t border-gray-200">
            <button
              type="button"
              @click="deleteMenu"
              class="px-6 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-semibold"
            >
              <i class="fas fa-trash mr-2"></i>
              Delete Menu
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMenusApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

definePageMeta({
  layout: 'menu'
})

const route = useRoute()
const router = useRouter()
const menusApi = useMenusApi()
const toast = useToast()

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActions } = usePageMeta()

const loading = ref(true)
const saving = ref(false)
const error = ref('')
const menuId = computed(() => parseInt(route.params.id as string))

const formData = ref({
  name: '',
  slug: '',
  description: '',
  coverImage: '',
  isActive: true
})

onMounted(async () => {
  await loadMenu()
  
  // Set page metadata after loading
  setPageHeader({
    title: 'Edit Menu',
    description: 'Update your menu details and settings'
  })

  setPageActions(() => h('div', { class: 'flex gap-3' }, [
    h('NuxtLink', {
      to: '/menu/menus',
      class: 'px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors font-semibold inline-flex items-center'
    }, 'Cancel'),
    h('button', {
      onClick: () => saveMenu(),
      type: 'button',
      class: 'px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold inline-flex items-center'
    }, [
      h('i', { class: 'fas fa-save mr-2' }),
      'Save Changes'
    ])
  ]))
})

async function loadMenu() {
  try {
    loading.value = true
    const response = await menusApi.getMenuById(menuId.value)

    if (response.success && response.data) {
      const menu = response.data
      formData.value = {
        name: menu.name || '',
        slug: menu.slug || '',
        description: menu.description || '',
        coverImage: menu.coverImage || '',
        isActive: menu.isActive !== false
      }
    } else {
      error.value = 'Menu not found'
    }
  } catch (err: any) {
    console.error('Error loading menu:', err)
    error.value = err.response?.data?.error || 'Failed to load menu'
  } finally {
    loading.value = false
  }
}

async function saveMenu() {
  try {
    saving.value = true
    const response = await menusApi.updateMenu(menuId.value, formData.value)

    if (response.success) {
      toast.success('Menu updated successfully')
      router.push('/menu/menus')
    } else {
      toast.error('Failed to update menu')
    }
  } catch (err: any) {
    console.error('Error saving menu:', err)
    toast.error(err.response?.data?.error || 'Failed to save menu')
  } finally {
    saving.value = false
  }
}

async function deleteMenu() {
  if (!confirm('Are you sure you want to delete this menu? This action cannot be undone.')) {
    return
  }

  try {
    const response = await menusApi.deleteMenu(menuId.value)

    if (response.success) {
      toast.success('Menu deleted successfully')
      router.push('/menu/menus')
    } else {
      toast.error('Failed to delete menu')
    }
  } catch (err: any) {
    console.error('Error deleting menu:', err)
    toast.error(err.response?.data?.error || 'Failed to delete menu')
  }
}
</script>

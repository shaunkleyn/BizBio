<template>
  <div
    class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
    @click="emit('close')"
  >
    <div
      class="bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col"
      @click.stop
    >
      <!-- Header -->
      <div class="p-6 border-b border-gray-200 flex items-center justify-between">
        <h2 class="text-2xl font-bold text-gray-900">Manage Categories</h2>
        <button
          @click="emit('close')"
          class="w-10 h-10 flex items-center justify-center text-gray-600 hover:bg-gray-100 rounded-full transition-colors"
        >
          <i class="fas fa-times"></i>
        </button>
      </div>

      <!-- Add Category Form -->
      <div class="p-6 border-b border-gray-200 bg-gray-50">
        <form @submit.prevent="addCategory" class="flex gap-3">
          <input
            v-model="newCategory.name"
            type="text"
            placeholder="Category name..."
            required
            class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
          />
          <button
            type="submit"
            :disabled="saving"
            class="px-6 py-2 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] transition-colors font-semibold disabled:opacity-50"
          >
            <i v-if="!saving" class="fas fa-plus mr-2"></i>
            <i v-else class="fas fa-spinner fa-spin mr-2"></i>
            Add
          </button>
        </form>
      </div>

      <!-- Categories List -->
      <div class="flex-1 overflow-y-auto p-6">
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-[var(--primary-color)]"></div>
        </div>

        <div v-else-if="categories.length === 0" class="text-center py-8">
          <i class="fas fa-folder-open text-4xl text-gray-300 mb-2"></i>
          <p class="text-gray-600">No categories yet. Add your first category above.</p>
        </div>

        <div v-else class="space-y-2">
          <div
            v-for="category in sortedCategories"
            :key="category.id"
            class="flex items-center justify-between p-4 bg-white border border-gray-200 rounded-lg hover:border-[var(--primary-color)] transition-colors"
          >
            <div class="flex-1">
              <div class="flex items-center gap-3">
                <i v-if="category.icon" :class="['fas', category.icon, 'text-gray-600']"></i>
                <div>
                  <h3 class="font-semibold text-gray-900">{{ category.name }}</h3>
                  <p v-if="category.description" class="text-sm text-gray-600">
                    {{ category.description }}
                  </p>
                  <p class="text-xs text-gray-500 mt-1">
                    {{ category.itemCount }} item{{ category.itemCount !== 1 ? 's' : '' }}
                  </p>
                </div>
              </div>
            </div>

            <div class="flex items-center gap-2">
              <button
                @click="editCategory(category)"
                class="px-3 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                title="Edit"
              >
                <i class="fas fa-edit"></i>
              </button>
              <button
                @click="deleteCategory(category)"
                class="px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                title="Delete"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Edit Category Modal -->
      <div
        v-if="editingCategory"
        class="absolute inset-0 bg-white z-10 p-6"
      >
        <div class="mb-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-xl font-bold text-gray-900">Edit Category</h3>
            <button
              @click="editingCategory = null"
              class="w-8 h-8 flex items-center justify-center text-gray-600 hover:bg-gray-100 rounded-full"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>

          <form @submit.prevent="updateCategory" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Category Name <span class="text-red-500">*</span>
              </label>
              <input
                v-model="editForm.name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Description
              </label>
              <textarea
                v-model="editForm.description"
                rows="3"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] resize-none"
              ></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Icon (Font Awesome class)
              </label>
              <input
                v-model="editForm.icon"
                type="text"
                placeholder="e.g., fa-pizza-slice"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
              />
              <p class="text-xs text-gray-500 mt-1">
                Find icons at <a href="https://fontawesome.com/icons" target="_blank" class="text-blue-600 hover:underline">fontawesome.com</a>
              </p>
            </div>

            <div class="flex gap-3 pt-4">
              <button
                type="button"
                @click="editingCategory = null"
                class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="flex-1 px-4 py-2 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--secondary-color)] disabled:opacity-50"
              >
                <span v-if="saving">
                  <i class="fas fa-spinner fa-spin mr-2"></i>
                  Saving...
                </span>
                <span v-else>Update</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useLibraryCategoriesApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'

const emit = defineEmits<{
  close: []
  saved: []
}>()

const categoriesApi = useLibraryCategoriesApi()
const toast = useToast()

const loading = ref(false)
const saving = ref(false)
const categories = ref<any[]>([])
const editingCategory = ref<any>(null)

const newCategory = reactive({
  name: '',
  description: ''
})

const editForm = reactive({
  name: '',
  description: '',
  icon: ''
})

const sortedCategories = computed(() => {
  return [...categories.value].sort((a, b) => a.sortOrder - b.sortOrder || a.name.localeCompare(b.name))
})

onMounted(async () => {
  await loadCategories()
})

async function loadCategories() {
  try {
    loading.value = true
    const response = await categoriesApi.getCategories()
    categories.value = response.data.data.categories
  } catch (error) {
    console.error('Error loading categories:', error)
    toast.error('Failed to load categories')
  } finally {
    loading.value = false
  }
}

async function addCategory() {
  try {
    saving.value = true
    await categoriesApi.createCategory({
      name: newCategory.name,
      description: newCategory.description || null,
      sortOrder: categories.value.length
    })
    toast.success('Category added successfully')
    newCategory.name = ''
    newCategory.description = ''
    await loadCategories()
    emit('saved')
  } catch (error) {
    console.error('Error adding category:', error)
    toast.error('Failed to add category')
  } finally {
    saving.value = false
  }
}

function editCategory(category: any) {
  editingCategory.value = category
  editForm.name = category.name
  editForm.description = category.description || ''
  editForm.icon = category.icon || ''
}

async function updateCategory() {
  if (!editingCategory.value) return

  try {
    saving.value = true
    await categoriesApi.updateCategory(editingCategory.value.id, {
      name: editForm.name,
      description: editForm.description || null,
      icon: editForm.icon || null
    })
    toast.success('Category updated successfully')
    editingCategory.value = null
    await loadCategories()
    emit('saved')
  } catch (error) {
    console.error('Error updating category:', error)
    toast.error('Failed to update category')
  } finally {
    saving.value = false
  }
}

async function deleteCategory(category: any) {
  if (category.itemCount > 0) {
    toast.error('Cannot delete category with items. Please move or delete items first.')
    return
  }

  if (!confirm(`Are you sure you want to delete "${category.name}"?`)) return

  try {
    await categoriesApi.deleteCategory(category.id)
    toast.success('Category deleted successfully')
    await loadCategories()
    emit('saved')
  } catch (error: any) {
    console.error('Error deleting category:', error)
    toast.error(error.response?.data?.error || 'Failed to delete category')
  }
}
</script>

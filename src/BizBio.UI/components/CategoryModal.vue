<template>
  <div
    class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4 animate-fadeSlide"
    @click="emit('close')"
  >
    <div
      class="mesh-card bg-md-surface rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col border border-md-outline-variant shadow-md-5"
      @click.stop
    >
      <!-- Header -->
      <div class="p-6 border-b border-md-outline-variant flex items-center justify-between relative overflow-hidden">
        <div class="absolute inset-0 bg-gradient-primary opacity-5"></div>
        <div class="flex items-center gap-3 relative z-10">
          <div class="w-12 h-12 rounded-2xl bg-gradient-primary flex items-center justify-center shadow-glow-purple">
            <i class="fas fa-layer-group text-white text-xl"></i>
          </div>
          <h2 class="text-2xl font-heading font-bold gradient-text">Manage Categories</h2>
        </div>
        <button
          @click="emit('close')"
          class="w-10 h-10 flex items-center justify-center bg-md-error-container text-md-on-error-container hover:bg-md-error hover:text-md-on-error rounded-xl transition-all md-ripple shadow-md-1 relative z-10"
        >
          <i class="fas fa-times"></i>
        </button>
      </div>

      <!-- Add Category Form -->
      <div class="p-6 border-b border-md-outline-variant relative overflow-hidden">
        <div class="absolute inset-0 mesh-bg-2 opacity-20"></div>
        <form @submit.prevent="addCategory" class="flex gap-3 relative z-10">
          <div class="flex-1 relative">
            <i class="fas fa-tag absolute left-4 top-1/2 -translate-y-1/2 text-md-on-surface-variant"></i>
            <input
              v-model="newCategory.name"
              type="text"
              placeholder="Category name..."
              required
              class="w-full pl-11 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all"
            />
          </div>
          <button
            type="submit"
            :disabled="saving"
            class="px-6 py-3 btn-gradient rounded-xl font-bold shadow-md-2 hover:shadow-glow-purple transition-all disabled:opacity-50 md-ripple flex items-center gap-2"
          >
            <i v-if="!saving" class="fas fa-plus"></i>
            <i v-else class="fas fa-spinner fa-spin"></i>
            Add
          </button>
        </form>
      </div>

      <!-- Categories List -->
      <div class="flex-1 overflow-y-auto p-6">
        <div v-if="loading" class="flex justify-center py-12">
          <div class="relative">
            <div class="animate-spin rounded-full h-12 w-12 border-4 border-md-primary border-t-transparent"></div>
            <div class="absolute inset-0 rounded-full bg-gradient-primary opacity-20 blur-xl"></div>
          </div>
        </div>

        <div v-else-if="categories.length === 0" class="text-center py-12">
          <div class="w-20 h-20 mx-auto mb-4 rounded-2xl bg-gradient-secondary flex items-center justify-center shadow-glow-pink">
            <i class="fas fa-folder-open text-4xl text-white"></i>
          </div>
          <p class="text-md-on-surface-variant font-medium">No categories yet. Add your first category above.</p>
        </div>

        <div v-else class="space-y-3">
          <div
            v-for="category in sortedCategories"
            :key="category.id"
            class="group flex items-center justify-between p-4 bg-md-surface-container border border-md-outline-variant rounded-xl hover:border-md-primary hover:shadow-md-2 transition-all"
          >
            <div class="flex-1">
              <div class="flex items-center gap-3">
                <div v-if="category.icon" class="w-10 h-10 rounded-xl bg-gradient-primary flex items-center justify-center shadow-md-1">
                  <i :class="['fas', category.icon, 'text-white']"></i>
                </div>
                <div>
                  <h3 class="font-bold text-md-on-surface">{{ category.name }}</h3>
                  <p v-if="category.description" class="text-sm text-md-on-surface-variant">
                    {{ category.description }}
                  </p>
                  <div class="flex items-center gap-2 mt-1">
                    <span class="px-2 py-0.5 bg-md-primary-container text-md-primary text-xs font-bold rounded-lg">
                      {{ category.itemCount }} item{{ category.itemCount !== 1 ? 's' : '' }}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <div class="flex items-center gap-2">
              <button
                @click="editCategory(category)"
                class="px-3 py-2 text-md-primary hover:bg-md-primary-container rounded-xl transition-all md-ripple"
                title="Edit"
              >
                <i class="fas fa-edit"></i>
              </button>
              <button
                @click="deleteCategory(category)"
                class="px-3 py-2 text-md-error hover:bg-md-error-container rounded-xl transition-all md-ripple"
                title="Delete"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- Edit Category Modal (Nested Modal) -->
    <div
      v-if="editingCategory"
      class="fixed inset-0 bg-black/70 backdrop-blur-sm z-[60] flex items-center justify-center p-4 animate-fadeSlide"
      @click="editingCategory = null"
    >
      <div
        class="mesh-card bg-md-surface rounded-2xl w-full max-w-lg p-6 border border-md-outline-variant shadow-md-5"
        @click.stop
      >
        <div class="flex items-center justify-between mb-6 relative overflow-hidden">
          <div class="absolute inset-0 bg-gradient-secondary opacity-5"></div>
          <h3 class="text-xl font-heading font-bold gradient-text relative z-10">Edit Category</h3>
          <button
            @click="editingCategory = null"
            class="w-10 h-10 flex items-center justify-center bg-md-error-container text-md-on-error-container hover:bg-md-error hover:text-md-on-error rounded-xl transition-all md-ripple shadow-md-1 relative z-10"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>

        <form @submit.prevent="updateCategory" class="space-y-4">
          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-tag text-md-primary"></i>
              Category Name <span class="text-md-error">*</span>
            </label>
            <input
              v-model="editForm.name"
              type="text"
              required
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all"
            />
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-align-left text-md-secondary"></i>
              Description
            </label>
            <textarea
              v-model="editForm.description"
              rows="3"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all resize-none"
            ></textarea>
          </div>

          <div>
            <label class="block text-sm font-bold text-md-on-surface mb-2 flex items-center gap-2">
              <i class="fas fa-icons text-md-tertiary"></i>
              Icon (Font Awesome class)
            </label>
            <input
              v-model="editForm.icon"
              type="text"
              placeholder="e.g., fa-pizza-slice"
              class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary focus:border-md-primary transition-all"
            />
            <p class="text-xs text-md-on-surface-variant mt-2 flex items-center gap-2">
              <i class="fas fa-info-circle"></i>
              Find icons at <a href="https://fontawesome.com/icons" target="_blank" class="text-md-primary hover:underline font-medium">fontawesome.com</a>
            </p>
          </div>

          <div class="flex gap-3 pt-4">
            <button
              type="button"
              @click="editingCategory = null"
              class="flex-1 px-5 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl hover:bg-md-surface-container-high hover:shadow-md-1 transition-all font-medium md-ripple"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="saving"
              class="flex-1 px-5 py-3 btn-gradient rounded-xl font-bold shadow-md-2 hover:shadow-glow-purple transition-all disabled:opacity-50 md-ripple flex items-center justify-center gap-2"
            >
              <i v-if="saving" class="fas fa-spinner fa-spin"></i>
              <i v-else class="fas fa-save"></i>
              <span v-if="saving">Saving...</span>
              <span v-else>Update</span>
            </button>
          </div>
        </form>
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
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.categories)) {
      categories.value = response.data.categories
    } else if (Array.isArray(response.data)) {
      categories.value = response.data
    } else {
      categories.value = []
    }
  } catch (error) {
    console.error('Error loading categories:', error)
    toast.error('Failed to load categories')
    categories.value = []
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

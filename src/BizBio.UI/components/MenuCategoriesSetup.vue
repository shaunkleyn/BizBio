<template>
  <div class="max-w-5xl mx-auto">
    <div class="text-center mb-8">
      <h2 class="text-3xl font-bold text-md-on-surface font-[var(--font-family-heading)] mb-4">
        Create Menu Categories
      </h2>
      <p class="text-md-on-surface-variant">
        Organize your menu into categories (e.g., Appetizers, Main Courses, Desserts)
      </p>
    </div>

    <div class="bg-md-surface rounded-2xl shadow-xl p-8">
      <!-- Plan Limit Info -->
      <div class="bg-md-primary-container border-2 border-md-primary rounded-xl p-4 mb-6 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <i class="fas fa-info-circle text-md-primary text-xl"></i>
          <div class="text-sm">
            <span class="font-semibold text-md-on-surface">
              {{ menuData.categories.length }} / {{ menuData.selectedPlan?.limits.categories }}
            </span>
            <span class="text-md-on-surface-variant"> categories used</span>
          </div>
        </div>
        <div v-if="menuData.categories.length >= menuData.selectedPlan?.limits.categories" class="text-sm text-[var(--accent-color)] font-semibold">
          <i class="fas fa-exclamation-triangle mr-1"></i>
          Limit reached
        </div>
      </div>

      <!-- Category List -->
      <div v-if="menuData.categories.length > 0" class="space-y-3 mb-6">
        <div
          v-for="(category, index) in menuData.categories"
          :key="category.id"
          class="flex items-center gap-4 p-4 border-2 border-md-outline-variant rounded-xl hover:border-[var(--primary-color)] transition-colors"
        >
          <!-- Icon -->
          <div class="w-12 h-12 bg-md-primary bg-opacity-10 rounded-xl flex items-center justify-center">
            <i :class="['text-[var(--primary-color)] text-xl', category.icon]"></i>
          </div>

          <!-- Info -->
          <div class="flex-1">
            <h4 class="font-bold text-md-on-surface">{{ category.name }}</h4>
            <p class="text-sm text-md-on-surface-variant">{{ category.description || 'No description' }}</p>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-2">
            <button
              @click="editCategory(category)"
              class="p-2 text-[var(--primary-color)] hover:bg-md-primary hover:bg-opacity-10 rounded-xl transition-colors"
            >
              <i class="fas fa-edit"></i>
            </button>
            <button
              @click="removeCategory(category.id)"
              class="p-2 text-[var(--accent-color)] hover:bg-[var(--accent-color)] hover:bg-opacity-10 rounded-xl transition-colors"
            >
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-12 border-2 border-dashed border-md-outline-variant rounded-xl mb-6">
        <i class="fas fa-folder-open text-6xl text-md-on-surface-variant opacity-50 mb-4"></i>
        <p class="text-md-on-surface-variant">No categories yet. Add your first category below.</p>
      </div>

      <!-- Add Category Button -->
      <button
        v-if="menuData.categories.length < menuData.selectedPlan?.limits.categories"
        @click="showAddCategoryModal = true"
        class="w-full py-4 border-2 border-dashed border-[var(--primary-color)] text-[var(--primary-color)] rounded-xl hover:bg-md-primary hover:bg-opacity-5 transition-all font-semibold"
      >
        <i class="fas fa-plus-circle mr-2"></i>
        Add Category
      </button>

      <!-- Action Buttons -->
      <div class="flex items-center justify-between mt-8 pt-6 border-t-2 border-md-outline-variant">
        <button
          @click="$emit('previous')"
          class="px-6 py-3 border-2 border-md-outline-variant text-md-on-surface rounded-xl hover:border-[var(--primary-color)] transition-colors font-semibold"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          Back
        </button>
        <button
          @click="handleNext"
          :disabled="menuData.categories.length === 0"
          :class="[
            'px-8 py-3 rounded-xl font-semibold transition-all',
            menuData.categories.length > 0
              ? 'bg-md-primary text-md-on-primary hover:bg-[var(--primary-button-hover-bg-color)]'
              : 'bg-[var(--light-border-color)] text-md-on-surface-variant cursor-not-allowed'
          ]"
        >
          Continue to Menu Items
          <i class="fas fa-arrow-right ml-2"></i>
        </button>
      </div>
    </div>

    <!-- Add/Edit Category Modal -->
    <div
      v-if="showAddCategoryModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="closeModal"
    >
      <div class="bg-md-surface rounded-2xl shadow-2xl max-w-md w-full max-h-[90vh] flex flex-col">
        <!-- Modal Header - Fixed -->
        <div class="flex-shrink-0 p-6 border-b border-md-outline-variant">
          <div class="flex items-center justify-between">
            <h3 class="text-xl font-bold text-md-on-surface">
              {{ editingCategory ? 'Edit Category' : 'Add New Category' }}
            </h3>
            <button @click="closeModal" type="button" class="text-md-on-surface-variant hover:text-md-on-surface">
              <i class="fas fa-times text-xl"></i>
            </button>
          </div>
        </div>

        <!-- Modal Body - Scrollable -->
        <div class="flex-1 overflow-y-auto p-6">
          <form @submit.prevent="handleSaveCategory" id="categoryForm">
          <!-- Category Name -->
          <div class="mb-4">
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Category Name <span class="text-[var(--accent-color)]">*</span>
            </label>
            <input
              v-model="newCategory.name"
              type="text"
              required
              placeholder="e.g., Appetizers, Main Courses"
              class="w-full px-4 py-3 border-2 border-md-outline-variant rounded-xl focus:border-[var(--primary-color)] focus:outline-none"
            />
          </div>

          <!-- Description -->
          <div class="mb-4">
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Description
            </label>
            <textarea
              v-model="newCategory.description"
              rows="3"
              placeholder="Brief description of this category"
              class="w-full px-4 py-3 border-2 border-md-outline-variant rounded-xl focus:border-[var(--primary-color)] focus:outline-none resize-none"
            ></textarea>
          </div>

          <!-- Icon Selection -->
          <div class="mb-6">
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Category Icon
            </label>
            <div class="grid grid-cols-6 gap-2">
              <button
                v-for="icon in categoryIcons"
                :key="icon"
                type="button"
                @click="newCategory.icon = icon"
                :class="[
                  'p-3 rounded-xl border-2 transition-all',
                  newCategory.icon === icon
                    ? 'border-[var(--primary-color)] bg-md-primary bg-opacity-10'
                    : 'border-md-outline-variant hover:border-[var(--primary-color)]'
                ]"
              >
                <i :class="[icon, 'text-xl', newCategory.icon === icon ? 'text-[var(--primary-color)]' : 'text-md-on-surface-variant']"></i>
              </button>
            </div>
          </div>
          </form>
        </div>

        <!-- Modal Footer - Fixed -->
        <div class="flex-shrink-0 p-6 border-t border-md-outline-variant">
          <div class="flex gap-3">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-4 py-3 border-2 border-md-outline-variant text-md-on-surface rounded-xl hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              Cancel
            </button>
            <button
              type="submit"
              form="categoryForm"
              class="flex-1 px-4 py-3 bg-md-primary text-md-on-primary rounded-xl hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
            >
              <i class="fas fa-check mr-2"></i>
              {{ editingCategory ? 'Update' : 'Add' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Confirmation Dialog -->
    <ConfirmDialog
      :is-open="showDeleteConfirm"
      title="Delete Category"
      message="Are you sure you want to delete this category? All items in this category will also be removed."
      confirm-text="Delete"
      cancel-text="Cancel"
      variant="danger"
      @confirm="confirmDeleteCategory"
      @cancel="showDeleteConfirm = false"
    />
  </div>
</template>

<script setup>
const emit = defineEmits(['next', 'previous'])
const { menuData, addCategory, removeCategory: removeCategoryFromStore } = useMenuCreation()
const toast = useToast()

const showAddCategoryModal = ref(false)
const editingCategory = ref(null)
const showDeleteConfirm = ref(false)
const categoryToDelete = ref(null)
const newCategory = ref({
  name: '',
  description: '',
  icon: 'fas fa-utensils'
})

const categoryIcons = [
  'fas fa-utensils',
  'fas fa-pizza-slice',
  'fas fa-hamburger',
  'fas fa-drumstick-bite',
  'fas fa-fish',
  'fas fa-carrot',
  'fas fa-cheese',
  'fas fa-ice-cream',
  'fas fa-coffee',
  'fas fa-wine-glass',
  'fas fa-beer',
  'fas fa-cocktail'
]

const editCategory = (category) => {
  editingCategory.value = category
  newCategory.value = {
    name: category.name,
    description: category.description,
    icon: category.icon
  }
  showAddCategoryModal.value = true
}

const removeCategory = (categoryId) => {
  categoryToDelete.value = categoryId
  showDeleteConfirm.value = true
}

const confirmDeleteCategory = () => {
  if (categoryToDelete.value) {
    removeCategoryFromStore(categoryToDelete.value)
    categoryToDelete.value = null
  }
  showDeleteConfirm.value = false
}

const handleSaveCategory = () => {
  try {
    if (editingCategory.value) {
      // Update existing category
      const index = menuData.value.categories.findIndex(c => c.id === editingCategory.value.id)
      if (index !== -1) {
        menuData.value.categories[index] = {
          ...menuData.value.categories[index],
          ...newCategory.value
        }
      }
    } else {
      // Add new category
      addCategory(newCategory.value)
    }
    closeModal()
  } catch (error) {
    toast.error(error.message, 'Error')
  }
}

const closeModal = () => {
  showAddCategoryModal.value = false
  editingCategory.value = null
  newCategory.value = {
    name: '',
    description: '',
    icon: 'fas fa-utensils'
  }
}

const handleNext = () => {
  if (menuData.value.categories.length > 0) {
    emit('next')
  }
}
</script>




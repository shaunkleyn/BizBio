<template>
  <div class="max-w-6xl mx-auto">
    <div class="text-center mb-8">
      <h2 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
        Add Menu Items
      </h2>
      <p class="text-[var(--gray-text-color)]">
        Add dishes and items to your menu categories
      </p>
    </div>

    <div class="bg-white rounded-2xl shadow-xl p-8">
      <!-- Plan Limit Info -->
      <div class="bg-blue-50 border-2 border-blue-200 rounded-xl p-4 mb-6 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <i class="fas fa-info-circle text-blue-600 text-xl"></i>
          <div class="text-sm">
            <span class="font-semibold text-[var(--dark-text-color)]">
              {{ menuData.menuItems.length }} / {{ menuData.selectedPlan?.limits.totalItems }}
            </span>
            <span class="text-[var(--gray-text-color)]"> items used</span>
          </div>
        </div>
        <div v-if="menuData.menuItems.length >= menuData.selectedPlan?.limits.totalItems" class="text-sm text-[var(--accent-color)] font-semibold">
          <i class="fas fa-exclamation-triangle mr-1"></i>
          Limit reached
        </div>
      </div>

      <!-- Category Tabs -->
      <div class="border-b-2 border-[var(--light-border-color)] mb-6">
        <div class="flex gap-2 overflow-x-auto pb-2">
          <button
            v-for="category in menuData.categories"
            :key="category.id"
            @click="selectedCategoryId = category.id"
            :class="[
              'px-4 py-2 rounded-t-lg font-semibold whitespace-nowrap transition-all',
              selectedCategoryId === category.id
                ? 'bg-[var(--primary-color)] text-white'
                : 'text-[var(--gray-text-color)] hover:text-[var(--primary-color)] hover:bg-[var(--light-background-color)]'
            ]"
          >
            <i :class="[category.icon, 'mr-2']"></i>
            {{ category.name }}
            <span class="ml-2 text-xs opacity-75">
              ({{ getCategoryItemCount(category.id) }})
            </span>
          </button>
        </div>
      </div>

      <!-- Menu Items List -->
      <div v-if="selectedCategoryItems.length > 0" class="grid md:grid-cols-2 gap-4 mb-6">
        <div
          v-for="item in selectedCategoryItems"
          :key="item.id"
          class="border-2 border-[var(--light-border-color)] rounded-xl p-4 hover:border-[var(--primary-color)] transition-colors"
        >
          <div class="flex gap-4">
            <!-- Image -->
            <div class="w-20 h-20 rounded-lg overflow-hidden bg-[var(--light-background-color)] flex-shrink-0">
              <img
                v-if="item.imageUrl"
                :src="item.imageUrl"
                :alt="item.name"
                class="w-full h-full object-cover"
              />
              <div v-else class="w-full h-full flex items-center justify-center">
                <i class="fas fa-image text-[var(--gray-text-color)] text-2xl"></i>
              </div>
            </div>

            <!-- Info -->
            <div class="flex-1 min-w-0">
              <div class="flex items-start justify-between gap-2 mb-1">
                <h4 class="font-bold text-[var(--dark-text-color)] truncate">{{ item.name }}</h4>
                <div class="flex-shrink-0 font-bold text-[var(--primary-color)]">
                  R{{ item.price.toFixed(2) }}
                </div>
              </div>
              <p class="text-sm text-[var(--gray-text-color)] line-clamp-2 mb-2">
                {{ item.description || 'No description' }}
              </p>

              <!-- Badges -->
              <div class="flex flex-wrap gap-1 mb-2">
                <span
                  v-for="diet in item.dietary"
                  :key="diet"
                  class="text-xs px-2 py-1 bg-green-100 text-green-700 rounded-full"
                >
                  {{ diet }}
                </span>
              </div>

              <!-- Actions -->
              <div class="flex gap-2">
                <button
                  @click="editItem(item)"
                  class="text-xs px-3 py-1 text-[var(--primary-color)] hover:bg-[var(--primary-color)] hover:bg-opacity-10 rounded transition-colors"
                >
                  <i class="fas fa-edit mr-1"></i>Edit
                </button>
                <button
                  @click="removeItem(item.id)"
                  class="text-xs px-3 py-1 text-[var(--accent-color)] hover:bg-[var(--accent-color)] hover:bg-opacity-10 rounded transition-colors"
                >
                  <i class="fas fa-trash mr-1"></i>Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-12 border-2 border-dashed border-[var(--light-border-color)] rounded-xl mb-6">
        <i class="fas fa-utensils text-6xl text-[var(--gray-text-color)] opacity-50 mb-4"></i>
        <p class="text-[var(--gray-text-color)]">No items in this category yet. Add your first item below.</p>
      </div>

      <!-- Add Item Button -->
      <button
        v-if="menuData.menuItems.length < menuData.selectedPlan?.limits.totalItems"
        @click="showAddItemModal = true"
        class="w-full py-4 border-2 border-dashed border-[var(--primary-color)] text-[var(--primary-color)] rounded-xl hover:bg-[var(--primary-color)] hover:bg-opacity-5 transition-all font-semibold"
      >
        <i class="fas fa-plus-circle mr-2"></i>
        Add Item to {{ selectedCategoryName }}
      </button>

      <!-- Action Buttons -->
      <div class="flex items-center justify-between mt-8 pt-6 border-t-2 border-[var(--light-border-color)]">
        <button
          @click="$emit('previous')"
          class="px-6 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          Back
        </button>
        <button
          @click="handleComplete"
          class="px-8 py-3 bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] text-white rounded-lg hover:shadow-lg transition-all font-bold text-lg"
        >
          <i class="fas fa-check-circle mr-2"></i>
          Complete Setup
        </button>
      </div>
    </div>

    <!-- Add/Edit Item Modal -->
    <div
      v-if="showAddItemModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 overflow-y-auto"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-2xl shadow-2xl max-w-2xl w-full p-6 my-8">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-[var(--dark-text-color)]">
            {{ editingItem ? 'Edit Menu Item' : 'Add New Menu Item' }}
          </h3>
          <button @click="closeModal" class="text-[var(--gray-text-color)] hover:text-[var(--dark-text-color)]">
            <i class="fas fa-times text-xl"></i>
          </button>
        </div>

        <form @submit.prevent="handleSaveItem">
          <div class="grid md:grid-cols-2 gap-4">
            <!-- Item Name -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Item Name <span class="text-[var(--accent-color)]">*</span>
              </label>
              <input
                v-model="newItem.name"
                type="text"
                required
                placeholder="e.g., Margherita Pizza"
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none"
              />
            </div>

            <!-- Price -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Price (R) <span class="text-[var(--accent-color)]">*</span>
              </label>
              <input
                v-model.number="newItem.price"
                type="number"
                step="0.01"
                min="0"
                required
                placeholder="0.00"
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none"
              />
            </div>

            <!-- Category -->
            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Category <span class="text-[var(--accent-color)]">*</span>
              </label>
              <select
                v-model="newItem.categoryId"
                required
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none"
              >
                <option v-for="cat in menuData.categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }}
                </option>
              </select>
            </div>

            <!-- Description -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Description
              </label>
              <textarea
                v-model="newItem.description"
                rows="3"
                placeholder="Describe this menu item..."
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none resize-none"
              ></textarea>
            </div>

            <!-- Image Upload -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Item Image
              </label>
              <div class="flex items-center gap-4">
                <div
                  v-if="itemImagePreview"
                  class="w-24 h-24 rounded-lg border-2 border-[var(--light-border-color)] overflow-hidden"
                >
                  <img :src="itemImagePreview" alt="Item preview" class="w-full h-full object-cover" />
                </div>
                <div
                  v-else
                  class="w-24 h-24 rounded-lg border-2 border-dashed border-[var(--light-border-color)] flex items-center justify-center bg-[var(--light-background-color)]"
                >
                  <i class="fas fa-image text-3xl text-[var(--gray-text-color)]"></i>
                </div>
                <div>
                  <input
                    ref="itemImageInput"
                    type="file"
                    accept="image/*"
                    class="hidden"
                    @change="handleItemImageUpload"
                  />
                  <button
                    type="button"
                    @click="$refs.itemImageInput.click()"
                    class="bg-[var(--primary-color)] text-white px-4 py-2 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors"
                  >
                    <i class="fas fa-upload mr-2"></i>
                    Upload Image
                  </button>
                </div>
              </div>
            </div>

            <!-- Dietary Options -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Dietary Options
              </label>
              <div class="flex flex-wrap gap-2">
                <label
                  v-for="option in dietaryOptions"
                  :key="option"
                  class="flex items-center gap-2 px-3 py-2 border-2 border-[var(--light-border-color)] rounded-lg cursor-pointer hover:border-[var(--primary-color)] transition-colors"
                >
                  <input
                    v-model="newItem.dietary"
                    type="checkbox"
                    :value="option"
                    class="w-4 h-4 text-[var(--primary-color)] rounded focus:ring-[var(--primary-color)]"
                  />
                  <span class="text-sm">{{ option }}</span>
                </label>
              </div>
            </div>

            <!-- Allergens -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
                Allergens
              </label>
              <div class="flex flex-wrap gap-2">
                <label
                  v-for="allergen in commonAllergens"
                  :key="allergen"
                  class="flex items-center gap-2 px-3 py-2 border-2 border-[var(--light-border-color)] rounded-lg cursor-pointer hover:border-[var(--accent-color)] transition-colors"
                >
                  <input
                    v-model="newItem.allergens"
                    type="checkbox"
                    :value="allergen"
                    class="w-4 h-4 text-[var(--accent-color)] rounded focus:ring-[var(--accent-color)]"
                  />
                  <span class="text-sm">{{ allergen }}</span>
                </label>
              </div>
            </div>
          </div>

          <!-- Buttons -->
          <div class="flex gap-3 mt-6">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-4 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="flex-1 px-4 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
            >
              <i class="fas fa-check mr-2"></i>
              {{ editingItem ? 'Update' : 'Add' }} Item
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
const emit = defineEmits(['previous', 'complete'])
const { menuData, addMenuItem, removeMenuItem } = useMenuCreation()

const showAddItemModal = ref(false)
const editingItem = ref(null)
const selectedCategoryId = ref(menuData.value.categories[0]?.id || '')
const itemImagePreview = ref('')

const newItem = ref({
  name: '',
  description: '',
  price: 0,
  categoryId: selectedCategoryId.value,
  image: null,
  imageUrl: '',
  allergens: [],
  dietary: []
})

const dietaryOptions = ['Vegetarian', 'Vegan', 'Gluten-Free', 'Dairy-Free', 'Keto', 'Halal', 'Kosher']
const commonAllergens = ['Nuts', 'Dairy', 'Eggs', 'Soy', 'Wheat', 'Fish', 'Shellfish', 'Sesame']

const selectedCategoryItems = computed(() => {
  return menuData.value.menuItems.filter(item => item.categoryId === selectedCategoryId.value)
})

const selectedCategoryName = computed(() => {
  const category = menuData.value.categories.find(c => c.id === selectedCategoryId.value)
  return category?.name || ''
})

const getCategoryItemCount = (categoryId) => {
  return menuData.value.menuItems.filter(item => item.categoryId === categoryId).length
}

const editItem = (item) => {
  editingItem.value = item
  newItem.value = {
    name: item.name,
    description: item.description,
    price: item.price,
    categoryId: item.categoryId,
    image: item.image,
    imageUrl: item.imageUrl,
    allergens: [...item.allergens],
    dietary: [...item.dietary]
  }
  itemImagePreview.value = item.imageUrl
  showAddItemModal.value = true
}

const removeItem = (itemId) => {
  if (confirm('Are you sure you want to delete this item?')) {
    removeMenuItem(itemId)
  }
}

const handleItemImageUpload = (event) => {
  const file = event.target.files[0]
  if (file) {
    newItem.value.image = file
    const reader = new FileReader()
    reader.onload = (e) => {
      itemImagePreview.value = e.target.result
      newItem.value.imageUrl = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

const handleSaveItem = () => {
  try {
    if (editingItem.value) {
      // Update existing item
      const index = menuData.value.menuItems.findIndex(i => i.id === editingItem.value.id)
      if (index !== -1) {
        menuData.value.menuItems[index] = {
          ...menuData.value.menuItems[index],
          ...newItem.value
        }
      }
    } else {
      // Add new item
      addMenuItem(newItem.value)
    }
    closeModal()
  } catch (error) {
    alert(error.message)
  }
}

const closeModal = () => {
  showAddItemModal.value = false
  editingItem.value = null
  newItem.value = {
    name: '',
    description: '',
    price: 0,
    categoryId: selectedCategoryId.value,
    image: null,
    imageUrl: '',
    allergens: [],
    dietary: []
  }
  itemImagePreview.value = ''
}

const handleComplete = () => {
  if (confirm('Are you ready to complete your menu setup and start your 14-day free trial?')) {
    emit('complete')
  }
}

// Watch for category change
watch(selectedCategoryId, (newVal) => {
  newItem.value.categoryId = newVal
})

// Set initial category when component mounts
onMounted(() => {
  if (menuData.value.categories.length > 0 && !selectedCategoryId.value) {
    selectedCategoryId.value = menuData.value.categories[0].id
  }
})
</script>

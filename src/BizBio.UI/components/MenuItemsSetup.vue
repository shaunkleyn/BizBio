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

      <!-- CSV Import Section (Professional & Premium only) -->
      <div v-if="canImportCSV" class="bg-gradient-to-r from-[var(--primary-color)] from-opacity-5 to-[var(--accent3-color)] to-opacity-5 rounded-xl border-2 border-[var(--primary-color)] border-opacity-20 p-6 mb-6">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1">
            <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2 flex items-center gap-2">
              <i class="fas fa-file-csv text-[var(--primary-color)]"></i>
              Bulk Import from CSV
              <span class="text-xs font-semibold px-2 py-1 bg-[var(--primary-color)] text-white rounded-full">PRO</span>
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] mb-4">
              Save time by importing multiple menu items at once from a CSV file
            </p>
            <div class="flex flex-wrap gap-3">
              <button
                @click="csvImport.downloadTemplate()"
                class="px-4 py-2 border-2 border-[var(--primary-color)] text-[var(--primary-color)] rounded-lg hover:bg-[var(--primary-color)] hover:text-white transition-colors font-semibold"
              >
                <i class="fas fa-download mr-2"></i>
                Download Template
              </button>
              <button
                @click="$refs.csvFileInput.click()"
                class="px-4 py-2 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
              >
                <i class="fas fa-upload mr-2"></i>
                Import CSV
              </button>
              <input
                ref="csvFileInput"
                type="file"
                accept=".csv"
                class="hidden"
                @change="handleCSVUpload"
              />
            </div>
          </div>
          <div class="text-right">
            <i class="fas fa-table text-6xl text-[var(--primary-color)] opacity-20"></i>
          </div>
        </div>
      </div>

      <!-- Upgrade Prompt for Starter Plan -->
      <div v-else class="bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl border-2 border-gray-200 p-6 mb-6">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1">
            <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2 flex items-center gap-2">
              <i class="fas fa-lock text-gray-400"></i>
              CSV Import - Professional Feature
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] mb-4">
              Upgrade to Professional or Premium plan to import menu items in bulk from CSV files
            </p>
            <button
              @click="showUpgradeModal = true"
              class="px-4 py-2 bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] text-white rounded-lg hover:shadow-lg transition-all font-semibold"
            >
              <i class="fas fa-rocket mr-2"></i>
              Upgrade Plan
            </button>
          </div>
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
          :disabled="props.isSubmitting"
          :class="[
            'px-8 py-3 rounded-lg transition-all font-bold text-lg',
            props.isSubmitting
              ? 'bg-[var(--light-border-color)] text-[var(--gray-text-color)] cursor-not-allowed'
              : 'bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] text-white hover:shadow-lg'
          ]"
        >
          <i :class="[props.isSubmitting ? 'fas fa-spinner fa-spin' : 'fas fa-check-circle', 'mr-2']"></i>
          {{ props.isSubmitting ? 'Creating Menu...' : 'Complete Setup' }}
        </button>
      </div>
    </div>

    <!-- Add/Edit Item Modal -->
    <div
      v-if="showAddItemModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] flex flex-col">
        <!-- Modal Header - Fixed -->
        <div class="flex-shrink-0 p-6 border-b border-[var(--light-border-color)]">
          <div class="flex items-center justify-between">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)]">
              {{ editingItem ? 'Edit Menu Item' : 'Add New Menu Item' }}
            </h3>
            <button @click="closeModal" type="button" class="text-[var(--gray-text-color)] hover:text-[var(--dark-text-color)]">
              <i class="fas fa-times text-xl"></i>
            </button>
          </div>
        </div>

        <!-- Modal Body - Scrollable -->
        <div class="flex-1 overflow-y-auto p-6">
          <form @submit.prevent="handleSaveItem" id="itemForm">
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
          </form>
        </div>

        <!-- Modal Footer - Fixed -->
        <div class="flex-shrink-0 p-6 border-t border-[var(--light-border-color)]">
          <div class="flex gap-3">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-4 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              Cancel
            </button>
            <button
              type="submit"
              form="itemForm"
              class="flex-1 px-4 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
            >
              <i class="fas fa-check mr-2"></i>
              {{ editingItem ? 'Update' : 'Add' }} Item
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Confirmation Dialogs -->
    <ConfirmDialog
      :is-open="showDeleteConfirm"
      title="Delete Menu Item"
      message="Are you sure you want to delete this item? This action cannot be undone."
      confirm-text="Delete"
      cancel-text="Cancel"
      variant="danger"
      @confirm="confirmDeleteItem"
      @cancel="showDeleteConfirm = false"
    />

    <ConfirmDialog
      :is-open="showCompleteConfirm"
      title="Start Your Free Trial"
      message="Are you ready to complete your menu setup and start your 14-day free trial?"
      confirm-text="Start Trial"
      cancel-text="Not Yet"
      variant="primary"
      @confirm="confirmComplete"
      @cancel="showCompleteConfirm = false"
    />

    <!-- CSV Import Preview Modal -->
    <div
      v-if="showCSVPreview"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="closeCSVPreview"
    >
      <div class="bg-white rounded-2xl shadow-2xl max-w-4xl w-full max-h-[90vh] flex flex-col">
        <!-- Modal Header -->
        <div class="flex-shrink-0 p-6 border-b border-[var(--light-border-color)]">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)]">
                CSV Import Preview
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mt-1">
                Review items before importing
              </p>
            </div>
            <button @click="closeCSVPreview" type="button" class="text-[var(--gray-text-color)] hover:text-[var(--dark-text-color)]">
              <i class="fas fa-times text-xl"></i>
            </button>
          </div>
        </div>

        <!-- Modal Body -->
        <div class="flex-1 overflow-y-auto p-6">
          <!-- Import Summary -->
          <div class="grid grid-cols-2 gap-4 mb-6">
            <div class="bg-green-50 border-2 border-green-200 rounded-lg p-4">
              <div class="flex items-center gap-3">
                <i class="fas fa-check-circle text-green-600 text-2xl"></i>
                <div>
                  <div class="text-2xl font-bold text-green-600">{{ csvPreviewData.valid.length }}</div>
                  <div class="text-sm text-green-700">Valid Items</div>
                </div>
              </div>
            </div>
            <div class="bg-red-50 border-2 border-red-200 rounded-lg p-4">
              <div class="flex items-center gap-3">
                <i class="fas fa-exclamation-triangle text-red-600 text-2xl"></i>
                <div>
                  <div class="text-2xl font-bold text-red-600">{{ csvPreviewData.errors.length }}</div>
                  <div class="text-sm text-red-700">Errors</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Errors Section -->
          <div v-if="csvPreviewData.errors.length > 0" class="mb-6">
            <h4 class="text-lg font-bold text-[var(--accent-color)] mb-3">
              <i class="fas fa-exclamation-circle mr-2"></i>
              Errors Found
            </h4>
            <div class="space-y-2 max-h-48 overflow-y-auto">
              <div
                v-for="error in csvPreviewData.errors"
                :key="error.row"
                class="bg-red-50 border-l-4 border-red-500 p-3 rounded"
              >
                <div class="font-semibold text-red-800">Row {{ error.row }}:</div>
                <ul class="text-sm text-red-700 ml-4 mt-1">
                  <li v-for="(err, idx) in error.errors" :key="idx">• {{ err }}</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- Valid Items Preview -->
          <div v-if="csvPreviewData.valid.length > 0">
            <h4 class="text-lg font-bold text-[var(--dark-text-color)] mb-3">
              <i class="fas fa-check-circle mr-2 text-green-600"></i>
              Items to Import ({{ csvPreviewData.valid.length }})
            </h4>
            <div class="space-y-2 max-h-96 overflow-y-auto">
              <div
                v-for="(item, idx) in csvPreviewData.valid"
                :key="idx"
                class="border-2 border-[var(--light-border-color)] rounded-lg p-3 hover:border-[var(--primary-color)] transition-colors"
              >
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="font-bold text-[var(--dark-text-color)]">{{ item.name }}</div>
                    <div class="text-sm text-[var(--gray-text-color)]">{{ item.description }}</div>
                    <div class="flex flex-wrap gap-2 mt-2">
                      <span class="text-xs px-2 py-1 bg-blue-100 text-blue-700 rounded-full">
                        {{ getCategoryName(item.categoryId) }}
                      </span>
                      <span
                        v-for="diet in item.dietary"
                        :key="diet"
                        class="text-xs px-2 py-1 bg-green-100 text-green-700 rounded-full"
                      >
                        {{ diet }}
                      </span>
                    </div>
                  </div>
                  <div class="text-right ml-4">
                    <div class="text-lg font-bold text-[var(--primary-color)]">R{{ item.price.toFixed(2) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Modal Footer -->
        <div class="flex-shrink-0 p-6 border-t border-[var(--light-border-color)]">
          <div class="flex gap-3 justify-end">
            <button
              type="button"
              @click="closeCSVPreview"
              class="px-6 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              Cancel
            </button>
            <button
              v-if="csvPreviewData.valid.length > 0"
              type="button"
              @click="confirmCSVImport"
              class="px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
            >
              <i class="fas fa-check mr-2"></i>
              Import {{ csvPreviewData.valid.length }} Items
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Upgrade Modal -->
    <div
      v-if="showUpgradeModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showUpgradeModal = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full p-8">
        <div class="text-center">
          <div class="w-16 h-16 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-full flex items-center justify-center mx-auto mb-4">
            <i class="fas fa-rocket text-3xl text-white"></i>
          </div>
          <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
            Upgrade to Professional
          </h3>
          <p class="text-[var(--gray-text-color)] mb-6">
            Get access to CSV import and many more features with Professional or Premium plan
          </p>
          <div class="flex gap-3">
            <button
              @click="showUpgradeModal = false"
              class="flex-1 px-4 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              Maybe Later
            </button>
            <NuxtLink
              to="/pricing"
              class="flex-1 px-4 py-3 bg-gradient-to-r from-[var(--primary-color)] to-[var(--accent3-color)] text-white rounded-lg hover:shadow-lg transition-all font-semibold text-center"
            >
              View Plans
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  isSubmitting: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['previous', 'complete'])
const { menuData, addMenuItem, removeMenuItem } = useMenuCreation()
const csvImport = useCsvImport()
const toast = useToast()

const showAddItemModal = ref(false)
const editingItem = ref(null)
const selectedCategoryId = ref(menuData.value.categories[0]?.id || '')
const itemImagePreview = ref('')
const showDeleteConfirm = ref(false)
const showCompleteConfirm = ref(false)
const itemToDelete = ref(null)

// CSV Import state
const showCSVPreview = ref(false)
const showUpgradeModal = ref(false)
const csvPreviewData = ref({ valid: [], errors: [] })

// Check if current plan supports CSV import
const canImportCSV = computed(() => {
  const planId = menuData.value.selectedPlan?.id
  return planId === 'menu-professional' || planId === 'menu-premium'
})

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
  itemToDelete.value = itemId
  showDeleteConfirm.value = true
}

const confirmDeleteItem = () => {
  if (itemToDelete.value) {
    removeMenuItem(itemToDelete.value)
    itemToDelete.value = null
  }
  showDeleteConfirm.value = false
}

const handleItemImageUpload = async (event) => {
  const file = event.target.files[0]
  if (file) {
    try {
      const { optimizeMenuImage } = useImageOptimization()
      const { file: optimizedFile, previewUrl } = await optimizeMenuImage(file)

      newItem.value.image = optimizedFile
      itemImagePreview.value = previewUrl
      newItem.value.imageUrl = previewUrl
    } catch (error) {
      console.error('Failed to optimize image:', error)
      toast.error('Failed to process image. Please try another image.', 'Upload Error')
    }
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
    toast.error(error.message, 'Error')
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
  showCompleteConfirm.value = true
}

const confirmComplete = () => {
  showCompleteConfirm.value = false
  emit('complete')
}

// CSV Import handlers
const handleCSVUpload = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  try {
    // Import and validate CSV
    const result = await csvImport.importFromCSV(file, menuData.value.categories)

    csvPreviewData.value = result

    if (result.valid.length === 0 && result.errors.length === 0) {
      toast.error('No data found in CSV file', 'Import Error')
      return
    }

    // Show preview modal
    showCSVPreview.value = true
  } catch (error) {
    console.error('CSV import error:', error)
    toast.error(error.message || 'Failed to import CSV file', 'Import Error')
  } finally {
    // Reset file input
    event.target.value = ''
  }
}

const confirmCSVImport = () => {
  try {
    // Check if adding these items would exceed plan limits
    const totalItems = menuData.value.menuItems.length + csvPreviewData.value.valid.length
    const planLimit = menuData.value.selectedPlan?.limits.totalItems

    if (totalItems > planLimit) {
      toast.error(
        `Cannot import ${csvPreviewData.value.valid.length} items. This would exceed your plan limit of ${planLimit} items. You currently have ${menuData.value.menuItems.length} items.`,
        'Plan Limit Exceeded'
      )
      return
    }

    // Add all valid items
    csvPreviewData.value.valid.forEach(item => {
      try {
        addMenuItem(item)
      } catch (error) {
        console.error('Error adding item:', error)
      }
    })

    toast.success(
      `Successfully imported ${csvPreviewData.value.valid.length} items`,
      'Import Successful'
    )

    closeCSVPreview()
  } catch (error) {
    console.error('Import error:', error)
    toast.error(error.message || 'Failed to import items', 'Import Error')
  }
}

const closeCSVPreview = () => {
  showCSVPreview.value = false
  csvPreviewData.value = { valid: [], errors: [] }
}

const getCategoryName = (categoryId) => {
  const category = menuData.value.categories.find(c => c.id === categoryId)
  return category?.name || 'Unknown'
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

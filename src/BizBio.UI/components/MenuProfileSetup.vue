<template>
  <div class="mx-auto">
    <div class="text-center mb-8">
      <h2 class="text-3xl font-bold text-md-on-background font-[var(--font-family-heading)] mb-4 gradient-text">
        Set Up Your Menu Profile
      </h2>
      <p class="text-md-on-surface-variant">
        Tell us about your business and menu
      </p>
    </div>

    <div class="mesh-card bg-md-surface rounded-2xl shadow-md-5 p-8">
      <!-- Trial Info Banner -->
      <div class="gradient-border mesh-card bg-md-surface text-md-on-surface rounded-2xl p-4 mb-8 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="bg-gradient-primary rounded-full p-2 shadow-glow-purple">
            <i class="fas fa-gift text-2xl text-white"></i>
          </div>
          <div>
            <div class="font-bold">{{ menuData.selectedPlan?.displayName }} - Free Trial</div>
            <div class="text-sm text-md-on-surface-variant">{{ menuData.trial.daysRemaining }} days remaining</div>
          </div>
        </div>
        <div class="text-right">
          <div class="text-sm text-md-on-surface-variant">After trial</div>
          <div class="font-bold text-lg gradient-text">R{{ menuData.selectedPlan?.price }}/mo</div>
        </div>
      </div>

      <form @submit.prevent="handleNext">
        <!-- Restaurant Selection -->
        <div class="mb-8">
          <label class="block text-sm font-semibold text-md-on-surface mb-2">
            Restaurant <span class="text-md-error">*</span>
          </label>
          <p class="text-xs text-md-on-surface-variant mb-3">
            Select which restaurant this menu belongs to
          </p>
          <RestaurantSelector
            v-model="menuData.menuProfile.restaurantId"
            @change="handleRestaurantChange"
          />
        </div>

        <!-- Business Logo Upload -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-md-on-surface mb-2">
            Business Logo
          </label>
          <div class="flex items-center gap-4">
            <div
              v-if="logoPreview"
              class="w-24 h-24 rounded-2xl border-2 border-md-outline overflow-hidden shadow-md-2"
            >
              <img :src="logoPreview" alt="Logo preview" class="w-full h-full object-cover" />
            </div>
            <div
              v-else
              class="w-24 h-24 rounded-2xl border-2 border-dashed border-md-outline-variant flex items-center justify-center bg-md-surface-container"
            >
              <i class="fas fa-image text-3xl text-md-on-surface-variant"></i>
            </div>
            <div class="flex-1">
              <input
                ref="logoInput"
                type="file"
                accept="image/*"
                class="hidden"
                @change="handleLogoUpload"
              />
              <button
                type="button"
                @click="$refs.logoInput.click()"
                class="btn-gradient text-white px-4 py-2 rounded-xl hover:shadow-md-3 transition-all font-semibold shadow-md-2"
              >
                <i class="fas fa-upload mr-2"></i>
                Upload Logo
              </button>
              <p class="text-xs text-md-on-surface-variant mt-2">
                Recommended: Square image, min 200x200px
              </p>
            </div>
          </div>
        </div>

        <div class="grid md:grid-cols-2 gap-6">
          <!-- Menu Name -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Menu Name <span class="text-md-error">*</span>
            </label>
            <input
              v-model="menuData.menuProfile.name"
              type="text"
              required
              placeholder="e.g., Summer Menu 2024"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- Business Name -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Business Name <span class="text-md-error">*</span>
            </label>
            <input
              v-model="menuData.menuProfile.businessName"
              type="text"
              required
              placeholder="e.g., The Golden Spoon"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- Cuisine Type -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Cuisine Type <span class="text-md-error">*</span>
            </label>
            <select
              v-model="menuData.menuProfile.cuisine"
              required
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            >
              <option value="">Select cuisine</option>
              <option value="italian">Italian</option>
              <option value="chinese">Chinese</option>
              <option value="indian">Indian</option>
              <option value="japanese">Japanese</option>
              <option value="mexican">Mexican</option>
              <option value="french">French</option>
              <option value="thai">Thai</option>
              <option value="american">American</option>
              <option value="mediterranean">Mediterranean</option>
              <option value="fusion">Fusion</option>
              <option value="other">Other</option>
            </select>
          </div>

          <!-- Phone Number -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Phone Number
            </label>
            <input
              v-model="menuData.menuProfile.phoneNumber"
              type="tel"
              placeholder="+27 12 345 6789"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- Email -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Email Address
            </label>
            <input
              v-model="menuData.menuProfile.email"
              type="email"
              placeholder="info@restaurant.com"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- Address -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Address
            </label>
            <input
              v-model="menuData.menuProfile.address"
              type="text"
              placeholder="123 Main Street"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- City -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              City
            </label>
            <input
              v-model="menuData.menuProfile.city"
              type="text"
              placeholder="Johannesburg"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            />
          </div>

          <!-- Country -->
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">
              Country
            </label>
            <select
              v-model="menuData.menuProfile.country"
              class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
            >
              <option value="">Select country</option>
              <option value="ZA">South Africa</option>
              <option value="US">United States</option>
              <option value="GB">United Kingdom</option>
              <option value="AU">Australia</option>
              <option value="CA">Canada</option>
            </select>
          </div>
        </div>

        <!-- Description -->
        <div class="mt-6">
          <label class="block text-sm font-semibold text-md-on-surface mb-2">
            Menu Description
          </label>
          <textarea
            v-model="menuData.menuProfile.description"
            rows="4"
            placeholder="Tell your customers about your menu..."
            class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors resize-none"
          ></textarea>
        </div>

        <!-- SEO Settings -->
        <div class="mt-8 p-6 bg-gradient-to-r from-[var(--primary-color)] from-opacity-5 to-[var(--accent3-color)] to-opacity-5 rounded-xl border-2 border-[var(--primary-color)] border-opacity-20">
          <div class="flex items-center justify-between mb-4">
            <div>
              <h3 class="text-lg font-bold text-[var(--dark-text-color)] flex items-center gap-2">
                <i class="fas fa-search text-[var(--primary-color)]"></i>
                Search Engine Optimization (SEO)
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mt-1">
                Make your menu discoverable on Google and other search engines
              </p>
            </div>
            <label class="relative inline-flex items-center cursor-pointer">
              <input
                v-model="menuData.menuProfile.enableSEO"
                type="checkbox"
                class="sr-only peer"
                @change="handleSEOToggle"
              />
              <div class="w-14 h-7 bg-md-outline-variant peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-md-primary peer-focus:ring-opacity-30 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-md-on-primary after:content-[''] after:absolute after:top-0.5 after:start-[4px] after:bg-md-surface-container-highest after:border-md-outline after:border after:rounded-full after:h-6 after:w-6 after:transition-all peer-checked:bg-md-primary"></div>
            </label>
          </div>

          <!-- SEO Fields (shown when enabled) -->
          <div v-show="menuData.menuProfile.enableSEO" class="space-y-4 mt-6 pt-6 border-t-2 border-[var(--primary-color)] border-opacity-20">
            <!-- URL Slug -->
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">
                Menu URL Slug
                <span class="text-[var(--gray-text-color)] font-normal ml-2">(Auto-generated from menu name)</span>
              </label>
              <div class="flex items-center gap-2">
                <span class="text-sm text-[var(--gray-text-color)]">yoursite.com/menu/</span>
                <input
                  v-model="menuData.menuProfile.slug"
                  type="text"
                  placeholder="summer-menu-2024"
                  class="flex-1 px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
                  @blur="validateSlug"
                />
              </div>
              <p class="text-xs text-[var(--gray-text-color)] mt-1">
                <i class="fas fa-info-circle mr-1"></i>
                Use lowercase letters, numbers, and hyphens only
              </p>
            </div>

            <!-- Meta Title -->
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">
                SEO Title
                <span class="text-[var(--gray-text-color)] font-normal ml-2">(Optional - defaults to menu name)</span>
              </label>
              <input
                v-model="menuData.menuProfile.metaTitle"
                type="text"
                :placeholder="`${menuData.menuProfile.name || 'Your Menu'} - ${menuData.menuProfile.businessName || 'Your Business'}`"
                maxlength="60"
                class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
              />
              <p class="text-xs text-[var(--gray-text-color)] mt-1">
                {{ menuData.menuProfile.metaTitle.length }}/60 characters - This appears in search results
              </p>
            </div>

            <!-- Meta Description -->
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">
                SEO Description
                <span class="text-[var(--gray-text-color)] font-normal ml-2">(Optional - defaults to menu description)</span>
              </label>
              <textarea
                v-model="menuData.menuProfile.metaDescription"
                rows="3"
                :placeholder="`Discover our delicious ${menuData.menuProfile.cuisine || 'cuisine'} menu at ${menuData.menuProfile.businessName || 'our restaurant'}. View our full menu with prices and order online.`"
                maxlength="160"
                class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors resize-none"
              ></textarea>
              <p class="text-xs text-[var(--gray-text-color)] mt-1">
                {{ menuData.menuProfile.metaDescription.length }}/160 characters - This appears below your title in search results
              </p>
            </div>

            <!-- Keywords -->
            <div>
              <label class="block text-sm font-semibold text-md-on-surface mb-2">
                Keywords
                <span class="text-[var(--gray-text-color)] font-normal ml-2">(Optional - comma separated)</span>
              </label>
              <input
                v-model="menuData.menuProfile.keywords"
                type="text"
                placeholder="restaurant menu, italian food, pizza, pasta, delivery"
                class="w-full px-4 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl focus:border-md-primary focus:outline-none transition-colors"
              />
              <p class="text-xs text-[var(--gray-text-color)] mt-1">
                <i class="fas fa-lightbulb mr-1"></i>
                Add relevant keywords to help customers find your menu
              </p>
            </div>

            <!-- SEO Preview -->
            <div class="mt-6 p-4 bg-md-surface-container rounded-2xl border-2 border-md-outline-variant">
              <div class="text-xs text-[var(--gray-text-color)] mb-2 uppercase font-semibold">
                <i class="fas fa-eye mr-1"></i>
                Google Search Preview
              </div>
              <div class="space-y-1">
                <div class="text-md-primary text-lg font-medium line-clamp-1">
                  {{ menuData.menuProfile.metaTitle || `${menuData.menuProfile.name || 'Your Menu'} - ${menuData.menuProfile.businessName || 'Your Business'}` }}
                </div>
                <div class="text-green-700 text-sm">
                  yoursite.com/menu/{{ menuData.menuProfile.slug || 'menu-slug' }}
                </div>
                <div class="text-sm text-[var(--gray-text-color)] line-clamp-2">
                  {{ menuData.menuProfile.metaDescription || menuData.menuProfile.description || `Discover our delicious menu at ${menuData.menuProfile.businessName || 'our restaurant'}. View our full menu with prices.` }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Working Hours (Optional - collapsed by default) -->
        <div class="mt-6">
          <button
            type="button"
            @click="showWorkingHours = !showWorkingHours"
            class="flex items-center justify-between w-full text-left py-2 text-[var(--dark-text-color)] font-semibold"
          >
            <span>Working Hours (Optional)</span>
            <i :class="['fas transition-transform', showWorkingHours ? 'fa-chevron-up' : 'fa-chevron-down']"></i>
          </button>

          <div v-show="showWorkingHours" class="mt-4 space-y-3">
            <div
              v-for="day in ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday']"
              :key="day"
              class="flex items-center gap-4"
            >
              <div class="w-28 capitalize font-medium text-[var(--dark-text-color)]">
                {{ day }}
              </div>
              <input
                v-model="menuData.menuProfile.workingHours[day].open"
                type="time"
                :disabled="menuData.menuProfile.workingHours[day].closed"
                class="px-3 py-2 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none disabled:bg-[var(--light-background-color)] disabled:text-[var(--gray-text-color)]"
              />
              <span class="text-[var(--gray-text-color)]">to</span>
              <input
                v-model="menuData.menuProfile.workingHours[day].close"
                type="time"
                :disabled="menuData.menuProfile.workingHours[day].closed"
                class="px-3 py-2 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none disabled:bg-[var(--light-background-color)] disabled:text-[var(--gray-text-color)]"
              />
              <label class="flex items-center gap-2 cursor-pointer">
                <input
                  v-model="menuData.menuProfile.workingHours[day].closed"
                  type="checkbox"
                  class="w-4 h-4 text-[var(--primary-color)] rounded focus:ring-[var(--primary-color)]"
                />
                <span class="text-sm text-[var(--gray-text-color)]">Closed</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex items-center justify-between mt-8 pt-6 border-t-2 border-[var(--light-border-color)]">
          <button
            type="button"
            @click="$emit('previous')"
            class="px-6 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl hover:bg-md-surface-container-high transition-colors font-semibold shadow-md-1"
          >
            <i class="fas fa-arrow-left mr-2"></i>
            Back
          </button>
          <button
            type="submit"
            :disabled="!canProceed"
            :class="[
              'px-8 py-3 rounded-xl font-semibold transition-all shadow-md-2',
              canProceed
                ? 'btn-gradient text-white hover:shadow-md-4'
                : 'bg-md-surface-container text-md-on-surface-variant cursor-not-allowed opacity-50'
            ]"
          >
            Continue to Categories
            <i class="fas fa-arrow-right ml-2"></i>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits(['next', 'previous'])
const { menuData, generateSlug } = useMenuCreation()
const toast = useToast()

const logoPreview = ref('')
const showWorkingHours = ref(false)

const canProceed = computed(() => {
  return menuData.value.menuProfile.name &&
         menuData.value.menuProfile.businessName &&
         menuData.value.menuProfile.cuisine
})

const handleLogoUpload = async (event) => {
  const file = event.target.files[0]
  if (file) {
    try {
      const { optimizeLogo } = useImageOptimization()
      const { file: optimizedFile, previewUrl } = await optimizeLogo(file)

      menuData.value.menuProfile.businessLogo = optimizedFile
      logoPreview.value = previewUrl
      menuData.value.menuProfile.businessLogoUrl = previewUrl
    } catch (error) {
      console.error('Failed to optimize logo:', error)
      toast.error('Failed to process image. Please try another image.', 'Upload Error')
    }
  }
}

// Handle SEO toggle - auto-generate slug when enabled
const handleSEOToggle = () => {
  if (menuData.value.menuProfile.enableSEO && !menuData.value.menuProfile.slug) {
    // Auto-generate slug from menu name and business name
    const slugBase = menuData.value.menuProfile.name || menuData.value.menuProfile.businessName || 'menu'
    menuData.value.menuProfile.slug = generateSlug(slugBase)
  }
}

// Validate and clean up slug
const validateSlug = () => {
  if (menuData.value.menuProfile.slug) {
    menuData.value.menuProfile.slug = generateSlug(menuData.value.menuProfile.slug)
  }
}

// Watch menu name changes to update slug automatically
watch(() => menuData.value.menuProfile.name, (newName) => {
  if (menuData.value.menuProfile.enableSEO && newName && !menuData.value.menuProfile.slug) {
    menuData.value.menuProfile.slug = generateSlug(newName)
  }
})

const handleRestaurantChange = (restaurant: any) => {
  console.log('Selected restaurant:', restaurant)
}

const handleNext = () => {
  if (canProceed.value) {
    // Validate restaurant selection
    if (!menuData.value.menuProfile.restaurantId) {
      alert('Please select a restaurant for this menu')
      return
    }
    emit('next')
  }
}
</script>




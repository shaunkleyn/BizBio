<template>
  <div class="max-w-4xl mx-auto">
    <div class="text-center mb-8">
      <h2 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
        Set Up Your Menu Profile
      </h2>
      <p class="text-[var(--gray-text-color)]">
        Tell us about your business and menu
      </p>
    </div>

    <div class="bg-white rounded-2xl shadow-xl p-8">
      <!-- Trial Info Banner -->
      <div class="bg-gradient-to-r from-[var(--accent3-color)] to-[var(--primary-color)] text-white rounded-xl p-4 mb-8 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <i class="fas fa-gift text-2xl"></i>
          <div>
            <div class="font-bold">{{ menuData.selectedPlan?.displayName }} - Free Trial</div>
            <div class="text-sm text-white/80">{{ menuData.trial.daysRemaining }} days remaining</div>
          </div>
        </div>
        <div class="text-right">
          <div class="text-sm text-white/80">After trial</div>
          <div class="font-bold text-lg">R{{ menuData.selectedPlan?.price }}/mo</div>
        </div>
      </div>

      <form @submit.prevent="handleNext">
        <!-- Business Logo Upload -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
            Business Logo
          </label>
          <div class="flex items-center gap-4">
            <div
              v-if="logoPreview"
              class="w-24 h-24 rounded-xl border-2 border-[var(--light-border-color)] overflow-hidden"
            >
              <img :src="logoPreview" alt="Logo preview" class="w-full h-full object-cover" />
            </div>
            <div
              v-else
              class="w-24 h-24 rounded-xl border-2 border-dashed border-[var(--light-border-color)] flex items-center justify-center bg-[var(--light-background-color)]"
            >
              <i class="fas fa-image text-3xl text-[var(--gray-text-color)]"></i>
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
                class="bg-[var(--primary-color)] text-white px-4 py-2 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
              >
                <i class="fas fa-upload mr-2"></i>
                Upload Logo
              </button>
              <p class="text-xs text-[var(--gray-text-color)] mt-2">
                Recommended: Square image, min 200x200px
              </p>
            </div>
          </div>
        </div>

        <div class="grid md:grid-cols-2 gap-6">
          <!-- Menu Name -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Menu Name <span class="text-[var(--accent-color)]">*</span>
            </label>
            <input
              v-model="menuData.menuProfile.name"
              type="text"
              required
              placeholder="e.g., Summer Menu 2024"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- Business Name -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Business Name <span class="text-[var(--accent-color)]">*</span>
            </label>
            <input
              v-model="menuData.menuProfile.businessName"
              type="text"
              required
              placeholder="e.g., The Golden Spoon"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- Cuisine Type -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Cuisine Type <span class="text-[var(--accent-color)]">*</span>
            </label>
            <select
              v-model="menuData.menuProfile.cuisine"
              required
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
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
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Phone Number
            </label>
            <input
              v-model="menuData.menuProfile.phoneNumber"
              type="tel"
              placeholder="+27 12 345 6789"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- Email -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Email Address
            </label>
            <input
              v-model="menuData.menuProfile.email"
              type="email"
              placeholder="info@restaurant.com"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- Address -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Address
            </label>
            <input
              v-model="menuData.menuProfile.address"
              type="text"
              placeholder="123 Main Street"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- City -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              City
            </label>
            <input
              v-model="menuData.menuProfile.city"
              type="text"
              placeholder="Johannesburg"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
            />
          </div>

          <!-- Country -->
          <div>
            <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
              Country
            </label>
            <select
              v-model="menuData.menuProfile.country"
              class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors"
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
          <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">
            Menu Description
          </label>
          <textarea
            v-model="menuData.menuProfile.description"
            rows="4"
            placeholder="Tell your customers about your menu..."
            class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:border-[var(--primary-color)] focus:outline-none transition-colors resize-none"
          ></textarea>
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
            class="px-6 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
          >
            <i class="fas fa-arrow-left mr-2"></i>
            Back
          </button>
          <button
            type="submit"
            :disabled="!canProceed"
            :class="[
              'px-8 py-3 rounded-lg font-semibold transition-all',
              canProceed
                ? 'bg-[var(--primary-color)] text-white hover:bg-[var(--primary-button-hover-bg-color)]'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)] cursor-not-allowed'
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

<script setup>
const emit = defineEmits(['next', 'previous'])
const { menuData } = useMenuCreation()

const logoPreview = ref('')
const showWorkingHours = ref(false)

const canProceed = computed(() => {
  return menuData.value.menuProfile.name &&
         menuData.value.menuProfile.businessName &&
         menuData.value.menuProfile.cuisine
})

const handleLogoUpload = (event) => {
  const file = event.target.files[0]
  if (file) {
    menuData.value.menuProfile.businessLogo = file
    const reader = new FileReader()
    reader.onload = (e) => {
      logoPreview.value = e.target.result
      menuData.value.menuProfile.businessLogoUrl = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

const handleNext = () => {
  if (canProceed.value) {
    emit('next')
  }
}
</script>

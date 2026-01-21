<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8">
    <!-- Loading State -->
    <div v-if="checkingSubscription" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--primary-color)] mx-auto"></div>
      <p class="text-md-on-surface-variant mt-4">Checking subscription...</p>
    </div>

    <!-- Upgrade Required (At Menu Limit) -->
    <div v-else-if="needsUpgrade && !showUpgradePlans" class="max-w-3xl mx-auto">
      <div class="text-center mb-8">
        <div class="w-20 h-20 bg-orange-100 rounded-full flex items-center justify-center mx-auto mb-4">
          <i class="fas fa-crown text-orange-600 text-3xl"></i>
        </div>
        <h2 class="text-3xl font-bold text-md-on-surface font-[var(--font-family-heading)] mb-4">
          Menu Limit Reached
        </h2>
        <p class="text-lg text-md-on-surface-variant mb-2">
          You've reached the maximum number of menus allowed on your current plan
        </p>
        <p class="text-sm text-md-on-surface-variant">
          <strong>{{ currentSubscription.plan.displayName }}</strong> allows {{ currentSubscription.plan.limits.maxMenus }} menu{{ currentSubscription.plan.limits.maxMenus === 1 ? '' : 's' }}
        </p>
      </div>

      <div class="mesh-card bg-md-surface-container rounded-2xl p-6 mb-6">
        <div class="flex items-start gap-4">
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0">
            <i class="fas fa-info-circle text-blue-600 text-xl"></i>
          </div>
          <div>
            <h3 class="font-semibold text-md-on-surface mb-2">Current Usage:</h3>
            <ul class="text-sm text-md-on-surface-variant space-y-1">
              <li><strong>Active Menus:</strong> {{ menuCount }} / {{ currentSubscription.plan.limits.maxMenus }}</li>
              <li><strong>Trial Days Remaining:</strong> {{ currentSubscription.trialDaysRemaining }} days</li>
            </ul>
          </div>
        </div>
      </div>

      <div class="bg-gradient-to-r from-blue-50 to-purple-50 rounded-2xl p-6 border-2 border-blue-200 mb-8">
        <h3 class="font-semibold text-md-on-surface mb-3 flex items-center gap-2">
          <i class="fas fa-sparkles text-blue-600"></i>
          Upgrade Benefits
        </h3>
        <ul class="space-y-2 text-sm text-md-on-surface-variant">
          <li class="flex items-start gap-2">
            <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
            <span>Create additional menus for different locations or occasions</span>
          </li>
          <li class="flex items-start gap-2">
            <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
            <span>Your trial period continues - upgrade now and pay only after trial ends</span>
          </li>
          <li class="flex items-start gap-2">
            <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
            <span>All existing menus and data remain unchanged</span>
          </li>
        </ul>
      </div>

      <div class="flex gap-4 justify-center">
        <button
          @click="handleCancel"
          class="px-6 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline-variant rounded-xl hover:bg-md-surface-container-high transition-colors font-semibold"
        >
          Cancel
        </button>
        <button
          @click="showUpgradePlans = true"
          class="px-8 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold"
        >
          <i class="fas fa-crown mr-2"></i>
          View Upgrade Options
        </button>
      </div>
    </div>

    <!-- Upgrade Plans Selection -->
    <div v-else-if="showUpgradePlans" class="mx-auto">
      <div class="text-center mb-8">
        <h2 class="text-3xl font-bold text-md-on-surface font-[var(--font-family-heading)] mb-2">
          Upgrade Your Plan
        </h2>
        <p class="text-md-on-surface-variant">
          Choose a plan with more menus. Your trial continues - pay after {{ currentSubscription.trialDaysRemaining }} days.
        </p>
      </div>

      <div class="grid md:grid-cols-2 gap-8 mx-auto mb-8">
        <div
          v-for="plan in upgradeOptions"
          :key="plan.id"
          @click="handleUpgradeSelect(plan)"
          :class="[
            'rounded-2xl shadow-xl p-8 transition-all duration-300 cursor-pointer',
            plan.popular
              ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative ring-4 ring-[var(--accent3-color)] ring-opacity-50'
              : 'bg-md-surface border-2 hover:border-[var(--primary-color)] hover:shadow-2xl border-[var(--light-border-color)]'
          ]"
        >
          <!-- Popular Badge -->
          <div
            v-if="plan.popular"
            class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold"
          >
            RECOMMENDED
          </div>

          <div class="text-center mb-6">
            <h3 :class="[
              'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
              plan.popular ? '' : 'text-[var(--dark-text-color)]'
            ]">
              {{ plan.displayName }}
            </h3>
            <p :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
              {{ plan.description }}
            </p>
          </div>

          <div class="text-center mb-6">
            <div class="flex items-baseline justify-center gap-2">
              <span class="text-sm" :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                R
              </span>
              <span :class="['text-5xl font-bold', plan.popular ? '' : 'text-[var(--dark-text-color)]']">
                {{ plan.price }}
              </span>
              <span :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">/month</span>
            </div>
            <p class="text-sm mt-2" :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
              After {{ currentSubscription.trialDaysRemaining }}-day trial
            </p>
          </div>

          <ul class="space-y-3 mb-8">
            <li
              v-for="(feature, fIndex) in plan.features"
              :key="fIndex"
              class="flex items-start gap-3"
            >
              <i :class="[
                'fas fa-check-circle mt-1',
                plan.popular ? 'text-white' : 'text-[var(--accent3-color)]'
              ]"></i>
              <span :class="plan.popular ? 'text-white' : 'text-[var(--dark-text-color)]'">
                {{ feature }}
              </span>
            </li>
          </ul>

          <button
            :class="[
              'w-full px-6 py-3 rounded-lg transition-all font-semibold',
              plan.popular
                ? 'bg-md-surface text-[var(--primary-color)] hover:bg-opacity-90 shadow-lg'
                : 'btn-gradient text-white hover:bg-[var(--primary-button-hover-bg-color)]'
            , 'shadow-md-2 hover:shadow-md-4']"
          >
            <i class="fas fa-arrow-up mr-2"></i>
            Upgrade to {{ plan.displayName }}
          </button>
        </div>
      </div>

      <div class="text-center">
        <button
          @click="showUpgradePlans = false"
          class="text-md-on-surface-variant hover:text-md-on-surface underline"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          Go Back
        </button>
      </div>
    </div>

      <!-- Trial Status Banner (shown when user has active trial) -->
      <div v-if="hasActiveSubscription && currentSubscription" class=" mx-auto mb-8">
        <div class="gradient-border mesh-card bg-gradient-to-r from-blue-50 to-purple-50 rounded-2xl p-4 flex items-center justify-between shadow-md-2">
          <div class="flex items-center gap-3">
            <div class="bg-gradient-primary rounded-full p-2 shadow-glow-purple">
              <i class="fas fa-gift text-2xl text-white"></i>
            </div>
            <div>
              <div class="font-bold text-md-on-surface">{{ currentSubscription.plan.displayName }} - Free Trial Active</div>
              <div class="text-sm text-md-on-surface-variant">
                <i class="fas fa-clock mr-1"></i>
                {{ currentSubscription.trialDaysRemaining }} days remaining until billing starts
              </div>
            </div>
          </div>
          <div class="text-right hidden sm:block">
            <div class="text-sm text-md-on-surface-variant">After trial</div>
            <div class="font-bold text-lg gradient-text">R{{ currentSubscription.plan.price }}/mo</div>
          </div>
        </div>
      </div>

      <!-- Progress Steps (skip step 1 if user has subscription) -->

          <!-- Normal Creation Flow -->
      <div v-else>
        <div class="bg-white rounded-3xl p-6 mb-6" style="box-shadow: 0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24);">
        <div class="flex items-center justify-between">
          <div  v-for="step in effectiveSteps" :key="step" :class="['flex flex-row' , effectiveSteps > step ? 'w-100 flex-1' : '',  'items-center']">
            <div >
            <div class="flex flex-col text-center items-center gap-2">
            <div :class="['w-10 h-10 ', effectiveCurrentStep < step ? 'bg-gray-200 text-gray-500' : 'bg-[#4A90E2] text-white', 'rounded-full flex items-center justify-center font-bold']">
              {{ step }}
            </div>
            <div>
              <p class="font-semibold text-gray-900">
                {{ getEffectiveStepLabel(step) }}
              </p>
              <p class="text-xs text-gray-500">Restaurant details</p>
            </div>
          </div>
            </div>
          <div :class="['h-px bg-gray-200 mx-4', effectiveSteps > step ? 'w-100 flex-1' : 'hidden']"></div>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-3xl p-6 mb-6" style="box-shadow: 0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24);">
      <div class="flex items-center justify-between">
        <div v-for="step in effectiveSteps" :key="step" :class="['flex flex-row',  effectiveSteps > step ? 'flex-1 items-center' : '']">
          <div :class="['flex items-center gap-3']">
              <div :class="['w-10 h-10',effectiveCurrentStep < step ? 'bg-gray-200 text-gray-500' : 'bg-[#4A90E2] text-white', 'rounded-full flex items-center justify-center font-bold']">
                {{ step }}
              </div>
              <div>
                <p class="font-semibold text-gray-900">{{ getEffectiveStepLabel(step) }}</p>
                <p class="text-xs text-gray-500">Restaurant details</p>
            </div>
          </div>
            <div :class="['h-px bg-gray-200 mx-4', effectiveSteps > step ? 'w-100 flex-1' : 'hidden']"></div>
        </div>
      </div>
    </div>

     <div class="mb-12">
        <div class="flex items-center justify-center">
          <div v-for="step in effectiveSteps" :key="step" class="flex items-center">
            <!-- Step Circle -->
            <div :class="[
              'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
              effectiveCurrentStep >= step
                ? 'btn-gradient text-white'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)]'
            , 'shadow-md-2 hover:shadow-md-4']">
              <i v-if="effectiveCurrentStep > step" class="fas fa-check"></i>
              <span v-else>{{ step }}</span>
            </div>

            <!-- Step Label -->
            <div class="hidden sm:block ml-3 mr-8">
              <div :class="[
                'text-sm font-semibold',
                effectiveCurrentStep >= step ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'
              ]">
                {{ getEffectiveStepLabel(step) }}
              </div>
            </div>

            <!-- Connector Line -->
            <div v-if="step < effectiveSteps" :class="[
              'hidden sm:block w-16 h-1 transition-all',
              effectiveCurrentStep > step ? 'bg-md-primary' : 'bg-[var(--light-border-color)]'
            ]"></div>
          </div>
        </div>
      </div>

      <!-- Step 1: Plan Selection (only for new users without subscription) -->
      <div v-if="currentStep === 1 && !hasActiveSubscription">
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Choose Your Menu Plan
          </h1>
          <p class="text-xl text-[var(--gray-text-color)] max-w-2xl mx-auto">
            Start your 14-day free trial. No credit card required.
          </p>
        </div>

        <!-- Trial Information Banner -->
        <div class="max-w-4xl mx-auto mb-8">
          <div class="bg-gradient-to-r from-green-50 to-blue-50 rounded-2xl p-6 border-2 border-green-200">
            <div class="flex items-start gap-4">
              <div class="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                <i class="fas fa-gift text-white text-xl"></i>
              </div>
              <div class="flex-1">
                <h3 class="font-bold text-lg text-md-on-surface mb-2">
                  <i class="fas fa-party-horn mr-2"></i>
                  14-Day Free Trial Included
                </h3>
                <ul class="space-y-2 text-sm text-md-on-surface-variant">
                  <li class="flex items-start gap-2">
                    <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
                    <span><strong>Full access</strong> to all features for 14 days - no restrictions</span>
                  </li>
                  <li class="flex items-start gap-2">
                    <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
                    <span><strong>No credit card required</strong> - start creating your menu immediately</span>
                  </li>
                  <li class="flex items-start gap-2">
                    <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
                    <span><strong>Cancel anytime</strong> during the trial - no charges if you decide it's not for you</span>
                  </li>
                  <li class="flex items-start gap-2">
                    <i class="fas fa-check-circle text-green-600 mt-0.5"></i>
                    <span><strong>Billing starts automatically</strong> after 14 days at the selected plan rate</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <!-- Pricing Cards -->
        <div class="grid md:grid-cols-3 gap-8  mx-auto mb-8">
          <div
            v-for="plan in menuPlans"
            :key="plan.id"
            @click="handlePlanSelect(plan)"
            :class="[
              'rounded-2xl shadow-xl p-8 transition-all duration-300 cursor-pointer',
              plan.popular
                ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
                : 'bg-md-surface border-2 hover:border-[var(--primary-color)] hover:shadow-2xl',
              menuData.selectedPlan?.id === plan.id
                ? 'ring-4 ring-[var(--accent3-color)] ring-opacity-50'
                : 'border-[var(--light-border-color)]'
            ]"
          >
            <!-- Popular Badge -->
            <div
              v-if="plan.popular"
              class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold"
            >
              MOST POPULAR
            </div>

            <!-- Trial Badge -->
            <div class="flex justify-center mb-4">
              <div :class="[
                'inline-flex items-center px-4 py-2 rounded-full text-sm font-bold',
                plan.popular
                  ? 'bg-white bg-opacity-20 text-white border-2 border-white border-opacity-30'
                  : 'bg-green-500 text-white shadow-md'
              ]">
                <i class="fas fa-gift mr-2"></i>
                {{ plan.trialDays }} Days FREE
              </div>
            </div>

            <div class="text-center mb-6">
              <h3 :class="[
                'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
                plan.popular ? '' : 'text-[var(--dark-text-color)]'
              ]">
                {{ plan.displayName }}
              </h3>
              <p :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                {{ plan.description }}
              </p>
            </div>

            <div class="text-center mb-6">
              <div class="mb-2">
                <span :class="['text-3xl font-bold line-through opacity-60', plan.popular ? 'text-white/60' : 'text-gray-400']">
                  R{{ plan.price }}
                </span>
              </div>
              <div class="mb-2">
                <span :class="['text-6xl font-bold', plan.popular ? 'text-white' : 'text-green-600']">
                  FREE
                </span>
              </div>
              <p class="text-sm font-semibold" :class="plan.popular ? 'text-white/90' : 'text-md-on-surface'">
                for {{ plan.trialDays }} days
              </p>
              <p class="text-xs mt-1" :class="plan.popular ? 'text-white/70' : 'text-[var(--gray-text-color)]'">
                Then R{{ plan.price }}/month
              </p>
            </div>

            <ul class="space-y-3 mb-8">
              <li
                v-for="(feature, fIndex) in plan.features"
                :key="fIndex"
                class="flex items-start gap-3"
              >
                <i :class="[
                  'fas fa-check-circle mt-1',
                  plan.popular ? 'text-white' : 'text-[var(--accent3-color)]'
                ]"></i>
                <span :class="plan.popular ? 'text-white' : 'text-[var(--dark-text-color)]'">
                  {{ feature }}
                </span>
              </li>
            </ul>

            <button
              :class="[
                'w-full px-6 py-3 rounded-lg transition-all font-semibold',
                plan.popular
                  ? 'bg-md-surface text-[var(--primary-color)] hover:bg-opacity-90 shadow-lg'
                  : 'btn-gradient text-white hover:bg-[var(--primary-button-hover-bg-color)]'
              , 'shadow-md-2 hover:shadow-md-4']"
            >
              <template v-if="menuData.selectedPlan?.id === plan.id">
                <i class="fas fa-check-circle mr-2"></i>
                Selected
              </template>
              <template v-else>
                <i class="fas fa-rocket mr-2"></i>
                Start Free Trial
              </template>
            </button>
          </div>
        </div>

        <!-- Next Button -->
        <div class="text-center">
          <button
            @click="nextStep"
            :disabled="!canProceedToNextStep"
            :class="[
              'px-8 py-4 rounded-lg font-bold text-lg transition-all',
              canProceedToNextStep
                ? 'btn-gradient text-white hover:bg-[var(--primary-button-hover-bg-color)] shadow-lg'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)] cursor-not-allowed'
            , 'shadow-md-2 hover:shadow-md-4']"
          >
            <i class="fas fa-rocket mr-2"></i>
            Start Free Trial
            <i class="fas fa-arrow-right ml-2"></i>
          </button>
          <p class="text-xs text-md-on-surface-variant mt-3">
            <i class="fas fa-shield-check mr-1"></i>
            No credit card required • Cancel anytime during trial
          </p>
        </div>
      </div>

      <!-- Step 2: Menu Profile Setup -->
      <MenuProfileSetup
        v-else-if="currentStep === 2"
        @next="nextStep"
        @previous="previousStep"
      />

      <!-- Step 3: Categories Setup -->
      <MenuCategoriesSetup
        v-else-if="currentStep === 3"
        @next="nextStep"
        @previous="previousStep"
      />

      <!-- Step 4: Menu Items Setup -->
      <MenuItemsSetup
        v-else-if="currentStep === 4"
        :is-submitting="isSubmitting"
        @previous="previousStep"
        @complete="handleComplete"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits(['created', 'cancel'])

const {
  menuData,
  currentStep,
  menuPlans,
  selectPlan,
  nextStep,
  previousStep,
  canProceedToNextStep,
  resetMenuCreation,
  goToStep
} = useMenuCreation()

const menusApi = useMenusApi()
const uploadsApi = useUploadsApi()
const { setLocalStorage, getLocalStorage } = useCookieConsent()
const isSubmitting = ref(false)
const checkingSubscription = ref(false)
const currentSubscription = ref<any>(null)
const hasActiveSubscription = ref(false)
const menuCount = ref(0)
const needsUpgrade = ref(false)
const showUpgradePlans = ref(false)

// Computed properties for step handling
const effectiveSteps = computed(() => hasActiveSubscription.value ? 3 : 4)
const effectiveCurrentStep = computed(() => {
  if (hasActiveSubscription.value && currentStep.value > 1) {
    return currentStep.value - 1
  }
  return currentStep.value
})

const upgradeOptions = computed(() => {
  if (!currentSubscription.value) return []

  const currentMaxMenus = currentSubscription.value.plan.limits.maxMenus
  return menuPlans.filter(plan => plan.limits.maxMenus > currentMaxMenus)
})

const getStepLabel = (step: number) => {
  const labels: Record<number, string> = {
    1: 'Choose Plan',
    2: 'Menu Info',
    3: 'Categories',
    4: 'Menu Items'
  }
  return labels[step]
}

const getEffectiveStepLabel = (step: number) => {
  if (hasActiveSubscription.value) {
    // Skip "Choose Plan" step
    const labels: Record<number, string> = {
      1: 'Menu Info',
      2: 'Categories',
      3: 'Menu Items'
    }
    return labels[step]
  }
  return getStepLabel(step)
}

const handlePlanSelect = (plan: any) => {
  selectPlan(plan)

  // Save plan selection to localStorage (consent-aware)
  setLocalStorage('userPlanId', plan.id, false) // functional, not analytics
  setLocalStorage('trialStartDate', new Date().toISOString(), false)
}

const handleUpgradeSelect = async (plan: any) => {
  try {
    // Update menu data with new plan (trial is preserved)
    selectPlan(plan)

    // Save upgraded plan to localStorage (consent-aware)
    setLocalStorage('userPlanId', plan.id, false)

    // Update current subscription object
    if (currentSubscription.value) {
      currentSubscription.value.planId = plan.id
      currentSubscription.value.plan = plan
    }

    // Reset state
    needsUpgrade.value = false
    showUpgradePlans.value = false
    hasActiveSubscription.value = true

    // Move to menu profile setup
    goToStep(2)

    const toast = useToast()
    toast.success(`Upgraded to ${plan.displayName}! Your trial continues.`, 'Success')

    // Note: The actual subscription update will happen when the menu is created
    // The new menu will be created with the upgraded plan
  } catch (error) {
    console.error('Failed to upgrade plan:', error)
    const toast = useToast()
    toast.error('Failed to upgrade plan. Please try again.', 'Error')
  }
}

const handleCancel = () => {
  emit('cancel')
}

const handleComplete = async () => {
  if (isSubmitting.value) return

  isSubmitting.value = true

  try {
    const { $trackEvent } = useNuxtApp()

    // Upload business logo if provided
    let logoUrl = menuData.value.menuProfile.businessLogoUrl
    if (menuData.value.menuProfile.businessLogo && menuData.value.menuProfile.businessLogo instanceof File) {
      try {
        const logoResponse = await uploadsApi.uploadLogo(menuData.value.menuProfile.businessLogo)
        logoUrl = logoResponse.data.url
      } catch (error) {
        console.error('Failed to upload logo:', error)
      }
    }

    // Upload menu item images
    const itemsWithUploadedImages = await Promise.all(
      menuData.value.menuItems.map(async (item: any) => {
        if (item.image && item.image instanceof File) {
          try {
            const imageResponse = await uploadsApi.uploadMenuImage(item.image)
            return {
              ...item,
              imageUrl: imageResponse.data.url,
              image: undefined // Remove the File object
            }
          } catch (error) {
            console.error(`Failed to upload image for ${item.name}:`, error)
            return { ...item, image: undefined }
          }
        }
        return { ...item, image: undefined }
      })
    )

    // Prepare menu data for API
    const menuPayload = {
      name: menuData.value.menuProfile.name,
      description: menuData.value.menuProfile.description,
      businessName: menuData.value.menuProfile.businessName,
      businessLogo: logoUrl,
      cuisine: menuData.value.menuProfile.cuisine,
      phoneNumber: menuData.value.menuProfile.phoneNumber,
      email: menuData.value.menuProfile.email,
      address: menuData.value.menuProfile.address,
      city: menuData.value.menuProfile.city,
      country: menuData.value.menuProfile.country,
      workingHours: menuData.value.menuProfile.workingHours,
      // SEO fields
      enableSEO: menuData.value.menuProfile.enableSEO,
      slug: menuData.value.menuProfile.slug,
      metaTitle: menuData.value.menuProfile.metaTitle,
      metaDescription: menuData.value.menuProfile.metaDescription,
      keywords: menuData.value.menuProfile.keywords,
      categories: menuData.value.categories.map((cat: any) => ({
        name: cat.name,
        description: cat.description,
        icon: cat.icon,
        order: cat.order
      })),
      items: itemsWithUploadedImages.map((item: any) => ({
        categoryId: item.categoryId,
        name: item.name,
        description: item.description,
        price: item.price,
        imageUrl: item.imageUrl,
        allergens: item.allergens,
        dietary: item.dietary,
        available: item.available,
        featured: item.featured
      })),
      subscriptionPlan: {
        planId: menuData.value.selectedPlan.id,
        planName: menuData.value.selectedPlan.name,
        price: menuData.value.selectedPlan.price
      },
      trial: menuData.value.trial
    }

    // Save menu to backend
    const response = await menusApi.createMenu(menuPayload)

    // Track successful menu creation
    if ($trackEvent) {
      $trackEvent('Menu Created', {
        menuName: menuData.value.menuProfile.name,
        categoriesCount: menuData.value.categories.length,
        itemsCount: menuData.value.menuItems.length,
        plan: menuData.value.selectedPlan.name
      })
    }

    // Reset menu creation state
    resetMenuCreation()

    // Emit success
    emit('created', response)

    // Show success toast
    const toast = useToast()
    toast.success('Menu created successfully!', 'Success')
  } catch (error) {
    console.error('Failed to create menu:', error)
    const toast = useToast()
    toast.error('Failed to create menu. Please try again.', 'Error')
  } finally {
    isSubmitting.value = false
  }
}

// Check subscription status when component mounts
const checkSubscriptionStatus = async () => {
  checkingSubscription.value = true
  try {
    // Get existing menus to determine subscription status
    const menusResponse = await menusApi.getMyMenus()
    const menus = Array.isArray(menusResponse) ? menusResponse : (menusResponse?.data || [])
    menuCount.value = menus.length

    // Check if user has any existing menus (which means they have a subscription)
    if (menus.length > 0) {
      // User has existing menus - they have a subscription
      // Try to get saved plan from localStorage (consent-aware)
      const savedPlanId = getLocalStorage('userPlanId', false)
      let plan = savedPlanId ? menuPlans.find(p => p.id === savedPlanId) : null

      // If no saved plan, default to Professional (most common)
      if (!plan) {
        plan = menuPlans.find(p => p.id === 'menu-professional') || menuPlans[1]
      }

      // Get trial info from localStorage (consent-aware) or calculate from first menu
      const savedTrialStart = getLocalStorage('trialStartDate', false)
      const trialStartDate = savedTrialStart ? new Date(savedTrialStart) : new Date(menus[0].createdAt || Date.now())
      const trialEndDate = new Date(trialStartDate.getTime() + (plan.trialDays * 24 * 60 * 60 * 1000))
      const trialDaysRemaining = Math.max(0, Math.ceil((trialEndDate.getTime() - Date.now()) / (24 * 60 * 60 * 1000)))

      // Build subscription object
      currentSubscription.value = {
        planId: plan.id,
        plan: plan,
        trialStartDate: trialStartDate,
        trialEndDate: trialEndDate,
        trialDaysRemaining: trialDaysRemaining
      }

      hasActiveSubscription.value = true
      menuData.value.selectedPlan = plan
      menuData.value.trial = {
        startDate: trialStartDate,
        endDate: trialEndDate,
        daysRemaining: trialDaysRemaining
      }

      // Check if user has reached menu limit
      const maxMenus = plan.limits.maxMenus
      if (menuCount.value >= maxMenus) {
        needsUpgrade.value = true
      } else {
        // User has subscription and is below limit - skip to step 2
        goToStep(2)
      }
    } else {
      // No menus - new user, start from step 1
      hasActiveSubscription.value = false
      goToStep(1)
    }
  } catch (error) {
    console.error('Error checking subscription:', error)
    // If error, assume no subscription and start from step 1
    hasActiveSubscription.value = false
    goToStep(1)
  } finally {
    checkingSubscription.value = false
  }
}

// Initialize on component mount
onMounted(() => {
  checkSubscriptionStatus()
})

// Reset when component unmounts
onUnmounted(() => {
  resetMenuCreation()
  needsUpgrade.value = false
  showUpgradePlans.value = false
})
</script>

<style scoped>
.gradient-border {
  position: relative;
  border: 2px solid transparent;
  background-clip: padding-box;
}

.gradient-border::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: -1;
  margin: -2px;
  border-radius: inherit;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.shadow-glow-purple {
  box-shadow: 0 0 20px rgba(102, 126, 234, 0.4);
}
</style>

<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Progress Steps -->
      <div class="mb-12">
        <div class="flex items-center justify-center">
          <div v-for="step in 4" :key="step" class="flex items-center">
            <!-- Step Circle -->
            <div :class="[
              'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
              currentStep >= step
                ? 'bg-[var(--primary-color)] text-white'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)]'
            ]">
              <i v-if="currentStep > step" class="fas fa-check"></i>
              <span v-else>{{ step }}</span>
            </div>

            <!-- Step Label -->
            <div class="hidden sm:block ml-3 mr-8">
              <div :class="[
                'text-sm font-semibold',
                currentStep >= step ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'
              ]">
                {{ getStepLabel(step) }}
              </div>
            </div>

            <!-- Connector Line -->
            <div v-if="step < 4" :class="[
              'hidden sm:block w-16 h-1 transition-all',
              currentStep > step ? 'bg-[var(--primary-color)]' : 'bg-[var(--light-border-color)]'
            ]"></div>
          </div>
        </div>
      </div>

      <!-- Step 1: Plan Selection -->
      <div v-if="currentStep === 1">
        <div class="text-center mb-12">
          <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Choose Your Menu Plan
          </h1>
          <p class="text-xl text-[var(--gray-text-color)] max-w-2xl mx-auto">
            Start your 14-day free trial. No credit card required.
          </p>
        </div>

        <!-- Pricing Cards -->
        <div class="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto mb-8">
          <div
            v-for="plan in menuPlans"
            :key="plan.id"
            @click="handlePlanSelect(plan)"
            :class="[
              'rounded-2xl shadow-xl p-8 transition-all duration-300 cursor-pointer',
              plan.popular
                ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
                : 'bg-white border-2 hover:border-[var(--primary-color)] hover:shadow-2xl',
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
                  ? 'bg-white bg-opacity-20 text-white'
                  : 'bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)]'
              ]">
                <i class="fas fa-gift mr-2"></i>
                {{ plan.trialDays }}-Day Free Trial
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
                After trial ends
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
                  ? 'bg-white text-[var(--primary-color)] hover:bg-opacity-90 shadow-lg'
                  : 'bg-[var(--primary-color)] text-white hover:bg-[var(--primary-button-hover-bg-color)]'
              ]"
            >
              <i class="fas fa-check mr-2"></i>
              {{ menuData.selectedPlan?.id === plan.id ? 'Selected' : 'Select Plan' }}
            </button>
          </div>
        </div>

        <!-- Next Button -->
        <div class="flex justify-center">
          <button
            @click="nextStep"
            :disabled="!canProceedToNextStep"
            :class="[
              'px-8 py-4 rounded-lg font-bold text-lg transition-all',
              canProceedToNextStep
                ? 'bg-[var(--primary-color)] text-white hover:bg-[var(--primary-button-hover-bg-color)] shadow-lg'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)] cursor-not-allowed'
            ]"
          >
            Continue to Menu Setup
            <i class="fas fa-arrow-right ml-2"></i>
          </button>
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
        @previous="previousStep"
        @complete="handleComplete"
      />
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({
  middleware: 'auth'
})

const router = useRouter()
const {
  menuData,
  currentStep,
  menuPlans,
  selectPlan,
  nextStep,
  previousStep,
  canProceedToNextStep
} = useMenuCreation()

const getStepLabel = (step) => {
  const labels = {
    1: 'Choose Plan',
    2: 'Menu Info',
    3: 'Categories',
    4: 'Menu Items'
  }
  return labels[step]
}

const handlePlanSelect = (plan) => {
  selectPlan(plan)
}

const handleComplete = async () => {
  // Here you would save the menu to the API
  // For now, we'll just navigate to the dashboard
  router.push('/dashboard')
}

useHead({
  title: 'Create Menu Profile',
})
</script>

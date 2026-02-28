<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
    <!-- Mode: Help Me Choose quiz -->
    <div v-if="mode === 'quiz'">
      <div class="text-center mb-10">
        <h2 class="text-2xl sm:text-3xl font-bold text-md-on-surface mb-3">Help Me Choose</h2>
        <p class="text-md-on-surface-variant">Answer 4 quick questions to find the right plan for you</p>
      </div>
      <HelpMeChooseFlow @recommend="onRecommend" />
    </div>

    <!-- Mode: Recommendation result -->
    <div v-else-if="mode === 'recommend' && recommendedPlan">
      <div class="text-center mb-10">
        <div class="inline-flex items-center gap-2 bg-md-secondary-container text-md-secondary px-4 py-2 rounded-full text-sm font-semibold mb-4">
          <i class="fas fa-magic"></i>
          Based on your answers
        </div>
        <h2 class="text-2xl sm:text-3xl font-bold text-md-on-surface mb-3">We recommend the {{ recommendedPlan.name }} plan</h2>
        <p class="text-md-on-surface-variant">Here's what you get:</p>
      </div>

      <!-- Recommended plan card -->
      <div class="max-w-md mx-auto mb-8">
        <div
          class="mesh-card bg-md-surface rounded-2xl shadow-md-4 p-8 border-2 border-md-secondary relative"
          @click="selectAndProceed(recommendedPlan.id)"
        >
          <div class="absolute -top-3 left-1/2 -translate-x-1/2 bg-md-secondary text-white px-4 py-1 rounded-full text-xs font-bold uppercase tracking-wider">
            Recommended
          </div>
          <div class="text-center mb-6">
            <h3 class="text-2xl font-bold text-md-on-surface mb-1">{{ recommendedPlan.name }}</h3>
            <div class="text-3xl font-black text-md-secondary">{{ recommendedPlan.priceLabel }}</div>
            <p class="text-sm text-md-on-surface-variant mt-1">{{ recommendedPlan.description }}</p>
          </div>
          <ul class="space-y-2 mb-8">
            <li v-for="feature in recommendedPlan.features" :key="feature" class="flex items-center gap-2 text-sm text-md-on-surface">
              <i class="fas fa-check text-md-secondary text-xs flex-shrink-0"></i>
              {{ feature }}
            </li>
          </ul>
          <button
            class="w-full btn-gradient py-3 rounded-xl text-white font-bold shadow-md-2 hover:shadow-md-4 transition-all"
            @click.stop="selectAndProceed(recommendedPlan.id)"
          >
            Start with {{ recommendedPlan.name }} Plan
          </button>
        </div>
      </div>

      <div class="text-center">
        <button
          class="text-sm text-md-primary hover:underline"
          @click="mode = 'plans'"
        >
          See all plans →
        </button>
      </div>
    </div>

    <!-- Mode: All plans grid -->
    <div v-else>
      <div class="text-center mb-10">
        <h2 class="text-2xl sm:text-3xl font-bold text-md-on-surface mb-3">Choose your plan</h2>
        <p class="text-md-on-surface-variant">
          Not sure?
          <button class="text-md-primary hover:underline font-medium ml-1" @click="mode = 'quiz'">
            Help me choose →
          </button>
        </p>
      </div>

      <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <div
          v-for="plan in CARD_PLANS"
          :key="plan.id"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6 border-2 transition-all cursor-pointer hover:shadow-md-5 relative"
          :class="selectedPlan === plan.id
            ? 'border-md-secondary shadow-md-4'
            : 'border-md-outline-variant hover:border-md-secondary'"
          @click="selectAndProceed(plan.id)"
        >
          <!-- Popular badge -->
          <div
            v-if="plan.popular"
            class="absolute -top-3 left-1/2 -translate-x-1/2 bg-gradient-secondary text-white px-4 py-1 rounded-full text-xs font-bold uppercase tracking-wider"
          >
            Most Popular
          </div>

          <!-- Selected indicator -->
          <div
            v-if="selectedPlan === plan.id"
            class="absolute top-3 right-3 w-6 h-6 bg-md-secondary rounded-full flex items-center justify-center"
          >
            <i class="fas fa-check text-white text-xs"></i>
          </div>

          <div class="text-center mb-4">
            <h3 class="text-lg font-bold text-md-on-surface mb-1">{{ plan.name }}</h3>
            <div class="text-2xl font-black text-md-secondary">{{ plan.priceLabel }}</div>
            <p class="text-xs text-md-on-surface-variant mt-1">{{ plan.description }}</p>
          </div>

          <ul class="space-y-1.5 mb-6">
            <li
              v-for="feature in plan.features"
              :key="feature"
              class="flex items-start gap-2 text-xs text-md-on-surface"
            >
              <i class="fas fa-check text-md-secondary text-[10px] mt-0.5 flex-shrink-0"></i>
              {{ feature }}
            </li>
          </ul>

          <button
            class="w-full py-2.5 rounded-xl text-sm font-semibold transition-all"
            :class="selectedPlan === plan.id || plan.popular
              ? 'btn-gradient text-white shadow-md-2'
              : 'border border-md-outline text-md-on-surface hover:bg-md-surface-container'"
          >
            {{ plan.price === 0 ? 'Start Free' : `Choose ${plan.name}` }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { CARD_PLANS, type PlanId } from '~/composables/useBusinessCardCreation'
import HelpMeChooseFlow from './HelpMeChooseFlow.vue'

const emit = defineEmits<{
  'plan-selected': [planId: PlanId]
}>()

const props = defineProps<{
  selectedPlan: PlanId | null
}>()

type Mode = 'quiz' | 'recommend' | 'plans'
const mode = ref<Mode>('quiz')
const recommendedPlan = ref<typeof CARD_PLANS[0] | null>(null)

const onRecommend = (planId: PlanId) => {
  recommendedPlan.value = CARD_PLANS.find(p => p.id === planId) ?? CARD_PLANS[0]
  mode.value = 'recommend'
}

const selectAndProceed = (planId: PlanId) => {
  emit('plan-selected', planId)
}
</script>

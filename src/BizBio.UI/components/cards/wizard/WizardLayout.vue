<template>
  <div class="h-screen bg-md-background flex flex-col">
    <!-- Header with progress -->
    <div class="bg-md-surface border-b border-md-outline-variant sticky top-0 z-40 shadow-md-2">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-3">
            <NuxtLink to="/dashboard" class="text-md-on-surface-variant hover:text-md-on-surface transition-colors">
              <i class="fas fa-arrow-left text-sm"></i>
            </NuxtLink>
            <div class="flex items-center gap-2">
              <div class="bg-gradient-secondary rounded-lg p-2 shadow-glow-pink">
                <i class="fas fa-id-card text-white text-sm"></i>
              </div>
              <span class="font-bold text-md-on-surface">Business Card Creator</span>
            </div>
          </div>
          <div class="text-sm text-md-on-surface-variant">
            Step {{ currentStep }} of 3
          </div>
        </div>

        <!-- Step indicators -->
        <div class="flex items-center gap-2">
          <template v-for="(step, idx) in steps" :key="step.id">
            <!-- Step pill -->
            <button
              class="flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-medium transition-all"
              :class="getStepClass(step.id)"
              :disabled="step.id > currentStep"
              @click="step.id < currentStep && $emit('go-to-step', step.id)"
            >
              <span
                class="w-5 h-5 rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0"
                :class="getStepCircleClass(step.id)"
              >
                <i v-if="step.id < currentStep" class="fas fa-check text-[10px]"></i>
                <span v-else>{{ step.id }}</span>
              </span>
              <span class="hidden sm:block">{{ step.label }}</span>
            </button>

            <!-- Connector line -->
            <div
              v-if="idx < steps.length - 1"
              class="flex-1 h-0.5 rounded-full transition-all"
              :class="currentStep > step.id ? 'bg-md-secondary' : 'bg-md-outline-variant'"
            ></div>
          </template>
        </div>

        <!-- Progress bar -->
        <div class="mt-3 h-1 bg-md-surface-container-highest rounded-full overflow-hidden">
          <div
            class="h-full bg-gradient-secondary rounded-full transition-all duration-500"
            :style="{ width: progressWidth }"
          ></div>
        </div>
      </div>
    </div>

    <!-- Step content -->
    <div class="flex-1 min-h-0 overflow-y-auto">
      <slot />
    </div>

    <!-- Bottom navigation -->
    <div
      v-if="showNav"
      class="bg-md-surface border-t border-md-outline-variant sticky bottom-0 z-40"
    >
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
        <button
          v-if="currentStep > 1"
          class="flex items-center gap-2 px-6 py-3 rounded-xl border border-md-outline text-md-on-surface hover:bg-md-surface-container transition-all font-semibold"
          @click="$emit('prev')"
        >
          <i class="fas fa-arrow-left text-sm"></i>
          Back
        </button>
        <div v-else></div>

        <button
          class="flex items-center gap-2 px-8 py-3 rounded-xl font-semibold transition-all shadow-md-2 hover:shadow-md-4"
          :class="canProceed
            ? 'btn-gradient text-white'
            : 'bg-md-surface-container text-md-on-surface-variant cursor-not-allowed'"
          :disabled="!canProceed"
          @click="canProceed && $emit('next')"
        >
          <span>{{ nextLabel }}</span>
          <i class="fas fa-arrow-right text-sm"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  currentStep: 1 | 2 | 3
  canProceed?: boolean
  showNav?: boolean
  nextLabel?: string
}>(), {
  canProceed: false,
  showNav: true,
  nextLabel: 'Next',
})

defineEmits<{
  next: []
  prev: []
  'go-to-step': [step: number]
}>()

const steps = [
  { id: 1, label: 'Choose Plan' },
  { id: 2, label: 'Build Card' },
  { id: 3, label: 'Publish' },
]

const progressWidth = computed(() => {
  const pct = ((props.currentStep - 1) / (steps.length - 1)) * 100
  return `${pct}%`
})

const getStepClass = (stepId: number) => {
  if (stepId < props.currentStep) return 'text-md-secondary cursor-pointer hover:bg-md-secondary-container'
  if (stepId === props.currentStep) return 'text-md-secondary bg-md-secondary-container'
  return 'text-md-on-surface-variant cursor-not-allowed opacity-50'
}

const getStepCircleClass = (stepId: number) => {
  if (stepId < props.currentStep) return 'bg-md-secondary text-white'
  if (stepId === props.currentStep) return 'bg-md-secondary text-white'
  return 'bg-md-surface-container-highest text-md-on-surface-variant'
}
</script>

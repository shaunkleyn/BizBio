<template>
  <div class="max-w-xl mx-auto">
    <!-- Progress dots -->
    <div class="flex justify-center gap-2 mb-8">
      <div
        v-for="(_, i) in questions"
        :key="i"
        class="w-2 h-2 rounded-full transition-all"
        :class="i <= currentQ ? 'bg-md-secondary w-6' : 'bg-md-outline-variant'"
      ></div>
    </div>

    <!-- Question card -->
    <transition name="slide-fade" mode="out-in">
      <div :key="currentQ" class="text-center">
        <p class="text-sm text-md-secondary font-semibold mb-2 uppercase tracking-wider">
          Question {{ currentQ + 1 }} of {{ questions.length }}
        </p>
        <h3 class="text-xl font-bold text-md-on-surface mb-8">
          {{ questions[currentQ].question }}
        </h3>

        <div class="grid gap-3">
          <button
            v-for="(option, oi) in questions[currentQ].options"
            :key="oi"
            class="p-4 rounded-xl border-2 text-left transition-all hover:border-md-secondary hover:bg-md-secondary-container group"
            :class="answers[currentQ] === oi
              ? 'border-md-secondary bg-md-secondary-container'
              : 'border-md-outline-variant bg-md-surface'"
            @click="selectAnswer(oi)"
          >
            <div class="flex items-center gap-3">
              <div
                class="w-5 h-5 rounded-full border-2 flex items-center justify-center flex-shrink-0 transition-all"
                :class="answers[currentQ] === oi
                  ? 'border-md-secondary bg-md-secondary'
                  : 'border-md-outline-variant'"
              >
                <div v-if="answers[currentQ] === oi" class="w-2 h-2 rounded-full bg-white"></div>
              </div>
              <span class="font-medium text-md-on-surface">{{ option.label }}</span>
            </div>
          </button>
        </div>

        <div class="flex items-center justify-between mt-8">
          <button
            v-if="currentQ > 0"
            class="text-sm text-md-on-surface-variant hover:text-md-on-surface transition-colors flex items-center gap-1"
            @click="currentQ--"
          >
            <i class="fas fa-arrow-left text-xs"></i> Back
          </button>
          <div v-else></div>

          <button
            v-if="answers[currentQ] !== undefined && currentQ < questions.length - 1"
            class="btn-gradient px-6 py-2 rounded-xl text-white font-semibold text-sm shadow-md-2 hover:shadow-md-4 transition-all flex items-center gap-2"
            @click="currentQ++"
          >
            Next <i class="fas fa-arrow-right text-xs"></i>
          </button>

          <button
            v-else-if="answers[currentQ] !== undefined && currentQ === questions.length - 1"
            class="btn-gradient px-6 py-2 rounded-xl text-white font-semibold text-sm shadow-md-2 hover:shadow-md-4 transition-all flex items-center gap-2"
            @click="finish"
          >
            See Recommendation <i class="fas fa-magic text-xs"></i>
          </button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import type { PlanId } from '~/composables/useBusinessCardCreation'

const emit = defineEmits<{
  recommend: [plan: PlanId]
}>()

const questions = [
  {
    question: 'How many people need a digital card?',
    options: [
      { label: 'Just me', value: 1 },
      { label: '2–5 people', value: 5 },
      { label: '6–20 people', value: 20 },
    ],
  },
  {
    question: 'Do you need analytics? (who viewed your card, where from, what device)',
    options: [
      { label: 'No, not needed', value: 0 },
      { label: 'Yes, I\'d like basic stats', value: 1 },
      { label: 'Yes, I need detailed analytics', value: 2 },
    ],
  },
  {
    question: 'Do you want custom branding — your own colours, fonts, and logo?',
    options: [
      { label: 'No, defaults are fine', value: 0 },
      { label: 'Yes, custom colours', value: 1 },
      { label: 'Yes, full custom branding', value: 2 },
    ],
  },
  {
    question: 'Do you need Google or Apple Wallet integration?',
    options: [
      { label: 'No thanks', value: 0 },
      { label: 'Yes, at least one', value: 1 },
      { label: 'Yes, both', value: 2 },
    ],
  },
]

const currentQ = ref(0)
const answers = ref<(number | undefined)[]>(Array(questions.length).fill(undefined))

const selectAnswer = (optionIdx: number) => {
  answers.value[currentQ.value] = optionIdx

  // Auto-advance after short delay
  if (currentQ.value < questions.length - 1) {
    setTimeout(() => {
      currentQ.value++
    }, 300)
  }
}

const finish = () => {
  const [people, analytics, branding, wallet] = answers.value.map(a => {
    return a !== undefined ? questions[answers.value.indexOf(a)]?.options[a]?.value ?? 0 : 0
  })

  // Recalculate from raw answer indices
  const peopleVal = questions[0].options[answers.value[0] ?? 0].value
  const analyticsVal = questions[1].options[answers.value[1] ?? 0].value
  const brandingVal = questions[2].options[answers.value[2] ?? 0].value
  const walletVal = questions[3].options[answers.value[3] ?? 0].value

  let plan: PlanId = 'free'

  if (peopleVal >= 6) {
    plan = 'business'
  } else if (peopleVal >= 2) {
    plan = 'team'
  } else if (analyticsVal > 0 || brandingVal > 0 || walletVal > 0) {
    plan = 'solo'
  } else {
    plan = 'free'
  }

  emit('recommend', plan)
}
</script>

<style scoped>
.slide-fade-enter-active,
.slide-fade-leave-active {
  transition: all 0.25s ease;
}
.slide-fade-enter-from {
  opacity: 0;
  transform: translateX(20px);
}
.slide-fade-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}
</style>

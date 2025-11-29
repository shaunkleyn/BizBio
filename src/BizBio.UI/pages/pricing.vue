<template>
  <div>
    <!-- Pricing Section -->
    <section class="bg-gradient-to-br from-[var(--light-background-color)] to-white py-16 sm:py-20 lg:py-24">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Header -->
        <div class="text-center mb-16">
          <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Simple, Transparent Pricing
          </h1>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto">
            Choose the perfect plan for your business. Upgrade, downgrade, or cancel anytime.
          </p>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="text-center py-12">
          <i class="fas fa-spinner fa-spin text-6xl text-[var(--primary-color)] mb-4"></i>
          <p class="text-[var(--gray-text-color)]">Loading pricing plans...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="max-w-md mx-auto bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-xl p-8 text-center">
          <i class="fas fa-exclamation-circle text-[var(--accent-color)] text-5xl mb-4"></i>
          <p class="text-[var(--dark-text-color)] mb-4">{{ error }}</p>
          <button
            @click="loadPricingTiers"
            class="bg-[var(--primary-color)] text-white px-6 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
          >
            Try Again
          </button>
        </div>

        <!-- Pricing Cards -->
        <div v-else class="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
          <div
            v-for="(tier, index) in pricingTiers"
            :key="tier.id"
            :class="[
              'rounded-2xl shadow-xl p-8 transition-all duration-300',
              tier.popular
                ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
                : 'bg-white border-2 border-[var(--light-border-color)] hover:border-[var(--primary-color)]'
            ]"
          >
            <!-- Popular Badge -->
            <div v-if="tier.popular" class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold">
              POPULAR
            </div>

            <div class="text-center mb-6">
              <h3 :class="[
                'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
                tier.popular ? '' : 'text-[var(--dark-text-color)]'
              ]">
                {{ tier.tierName }}
              </h3>
              <div class="flex items-center justify-center gap-2 mb-4">
                <span :class="['text-5xl font-bold', tier.popular ? '' : 'text-[var(--dark-text-color)]']">
                  ${{ tier.price }}
                </span>
                <span :class="tier.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">/month</span>
              </div>
              <p :class="tier.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                {{ tier.description }}
              </p>
            </div>

            <ul class="space-y-4 mb-8">
              <li
                v-for="(feature, fIndex) in tier.features"
                :key="fIndex"
                :class="[
                  'flex items-start gap-3',
                  feature.included ? '' : 'opacity-50'
                ]"
              >
                <i :class="[
                  'mt-1',
                  feature.included
                    ? `fas fa-check-circle ${tier.popular ? 'text-white' : 'text-[var(--accent3-color)]'}`
                    : 'fas fa-times-circle text-[var(--gray-text-color)]'
                ]"></i>
                <span :class="tier.popular ? '' : feature.included ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'">
                  {{ feature.text }}
                </span>
              </li>
            </ul>

            <NuxtLink
              :to="tier.buttonLink"
              :class="[
                'block w-full text-center px-6 py-3 rounded-lg transition-all font-semibold',
                tier.popular
                  ? 'bg-white text-[var(--primary-color)] hover:bg-opacity-90 shadow-lg'
                  : tier.price === 0
                    ? 'border-2 border-[var(--primary-color)] text-[var(--primary-color)] hover:bg-[var(--primary-color)] hover:text-white'
                    : 'border-2 border-[var(--primary-color)] text-[var(--primary-color)] hover:bg-[var(--primary-color)] hover:text-white'
              ]"
            >
              {{ tier.buttonText }}
            </NuxtLink>
          </div>
        </div>

        <!-- FAQ Section -->
        <div class="mt-20 max-w-3xl mx-auto">
          <h2 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] text-center mb-12">
            Frequently Asked Questions
          </h2>

          <div class="space-y-6">
            <div class="bg-white rounded-xl p-6 shadow-md">
              <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2">
                Can I change plans later?
              </h3>
              <p class="text-[var(--gray-text-color)]">
                Yes! You can upgrade, downgrade, or cancel your plan at any time. Changes take effect immediately.
              </p>
            </div>

            <div class="bg-white rounded-xl p-6 shadow-md">
              <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2">
                Is there a free trial?
              </h3>
              <p class="text-[var(--gray-text-color)]">
                The Professional plan includes a 14-day free trial. No credit card required to start.
              </p>
            </div>

            <div class="bg-white rounded-xl p-6 shadow-md">
              <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-2">
                What payment methods do you accept?
              </h3>
              <p class="text-[var(--gray-text-color)]">
                We accept all major credit cards, PayPal, and bank transfers for annual plans.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const subscriptionsApi = useSubscriptionsApi()

const loading = ref(true)
const error = ref(null)
const pricingTiers = ref([])

const loadPricingTiers = async () => {
  loading.value = true
  error.value = null

  try {
    const response = await subscriptionsApi.getTiers()
    console.log(response);
    pricingTiers.value = response.data || []
  } catch (err) {
    console.error('Failed to load pricing tiers:', err)

    // Fallback to default tiers if API fails
    pricingTiers.value = [
      {
        id: '1',
        name: 'Free',
        price: 0,
        description: 'Perfect for trying out',
        features: [
          { text: '1 Business Profile', included: true },
          { text: 'Basic analytics', included: true },
          { text: 'QR code sharing', included: true },
          { text: 'No custom branding', included: false }
        ],
        buttonText: 'Get Started',
        buttonLink: '/register',
        popular: false
      },
      {
        id: '2',
        name: 'Professional',
        price: 9,
        description: 'For serious professionals',
        features: [
          { text: '5 Business Profiles', included: true },
          { text: 'Advanced analytics', included: true },
          { text: 'Custom branding', included: true },
          { text: 'Priority support', included: true },
          { text: 'NFC card integration', included: true }
        ],
        buttonText: 'Start Free Trial',
        buttonLink: '/register',
        popular: true
      },
      {
        id: '3',
        name: 'Business',
        price: 29,
        description: 'For growing teams',
        features: [
          { text: 'Unlimited profiles', included: true },
          { text: 'Team management', included: true },
          { text: 'White-label options', included: true },
          { text: 'API access', included: true },
          { text: 'Dedicated support', included: true }
        ],
        buttonText: 'Contact Sales',
        buttonLink: '/contact',
        popular: false
      }
    ]

    error.value = 'Using default pricing. Could not connect to API.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadPricingTiers()
})

useHead({
  title: 'Pricing',
})
</script>

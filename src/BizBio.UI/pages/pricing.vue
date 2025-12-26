<template>
  <div>
    <!-- Hero Section -->
    <section class="relative bg-gradient-to-br from-blue-50 to-purple-50 overflow-hidden py-20">
      <div class="absolute top-20 right-10 w-72 h-72 bg-blue-600 rounded-full opacity-10 blur-3xl"></div>
      <div class="absolute bottom-20 left-10 w-96 h-96 bg-purple-600 rounded-full opacity-10 blur-3xl"></div>
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center relative">
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-md-on-surface mb-6">
          Simple, Transparent Pricing
        </h1>
        <p class="text-xl text-md-on-surface-variant max-w-3xl mx-auto mb-8">
          Choose the perfect plan for your business. Upgrade, downgrade, or cancel anytime.
        </p>
      </div>
    </section>
    <section>
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

        <!-- Loading State -->
        <div v-if="loading" class="text-center py-12">
          <i class="fas fa-spinner fa-spin text-6xl text-md-primary mb-4"></i>
          <p class="text-[var(--gray-text-color)]">Loading pricing plans...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error"
          class="max-w-md mx-auto bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-xl p-8 text-center">
          <i class="fas fa-exclamation-circle text-[var(--accent-color)] text-5xl mb-4"></i>
          <p class="text-[var(--dark-text-color)] mb-4">{{ error }}</p>
          <button @click="loadPricingTiers"
            class="bg-md-primary text-white px-6 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold">
            Try Again
          </button>
        </div>


        <!-- Pricing Cards -->
        <div v-else class="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
          <div v-for="(tier, index) in pricingTiers" :key="tier.id" :class="[
            'rounded-2xl shadow-xl p-8 transition-all duration-300',
            tier.popular
              ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
              : 'bg-white border-2 border-[var(--light-border-color)] hover:border-md-primary'
          ]">
            <!-- Popular Badge -->
            <div v-if="tier.popular"
              class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold">
              POPULAR
            </div>

            <div class="text-center mb-6">
              <h3 :class="[
                'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
                tier.popular ? '' : 'text-[var(--dark-text-color)]'
              ]">
                {{ tier.data }}
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
              <li v-for="(feature, fIndex) in tier.features" :key="fIndex" :class="[
                'flex items-start gap-3',
                feature.included ? '' : 'opacity-50'
              ]">
                <i :class="[
                  'mt-1',
                  feature.included
                    ? `fas fa-check-circle ${tier.popular ? 'text-white' : 'text-[var(--accent3-color)]'}`
                    : 'fas fa-times-circle text-[var(--gray-text-color)]'
                ]"></i>
                <span
                  :class="tier.popular ? '' : feature.included ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'">
                  {{ feature.text }}
                </span>
              </li>
            </ul>

            <NuxtLink :to="tier.buttonLink" :class="[
              'block w-full text-center px-6 py-3 rounded-lg transition-all font-semibold',
              tier.popular
                ? 'bg-white text-md-primary hover:bg-opacity-90 shadow-lg'
                : tier.price === 0
                  ? 'border-2 border-md-primary text-md-primary hover:bg-md-primary hover:text-white'
                  : 'border-2 border-md-primary text-md-primary hover:bg-md-primary hover:text-white'
            ]">
              {{ tier.buttonText }}
            </NuxtLink>
          </div>
        </div>

        <!-- FAQ Section -->
        <div class="mt-20 max-w-3xl mx-auto">
          <h2
            class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] text-center mb-12">
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

    <section class="code-section bg-[var(--light-background-color)] min-h-screen pb-12" id="s4wbit">

      <div
        class="bg-gradient-to-br from-[var(--dark-background-color)] to-[var(--primary-color)] text-white py-16 px-4">
        <div class="max-w-7xl mx-auto text-center">
          <h1 class="text-4xl lg:text-5xl font-bold font-[var(--font-family-heading)] mb-4">
            Choose Your Perfect Plan
          </h1>
          <p class="text-xl text-white text-opacity-90 mb-8">
            Flexible pricing for businesses of all sizes
          </p>

          <div class="inline-flex items-center gap-4 bg-white bg-opacity-10 backdrop-blur-sm rounded-full p-2">
            <button id="monthly-toggle"
              class="px-6 py-2 rounded-full font-semibold bg-white text-md-primary transition-all">
              Monthly
            </button>
            <button id="annual-toggle"
              class="px-6 py-2 rounded-full font-semibold text-white hover:bg-white hover:bg-opacity-10 transition-all">
              Annual
              <span class="ml-2 text-xs bg-[var(--accent3-color)] px-2 py-1 rounded-full">Save 20%</span>
            </button>
          </div>
        </div>
      </div>

      <div class="px-4 py-12 -mt-20">
        <div class="max-w-7xl mx-auto">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div  v-for="(tier, index) in pricingTiers" :key="tier.id" 
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  {{ tier.displayName }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  {{ tier.description }}
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R{{ tier.monthlyPrice }}</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
                <!-- <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  ${{ tier.monthlyPrice }}
                </p> -->
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Up to 5 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Digital menu (1 menu)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Email support</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>
            <div v-for="(tier, index) in pricingTiers" :key="tier.id" :class="[
            'rounded-2xl shadow-xl p-8 transition-all duration-300',
            tier.popular
              ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
              : 'bg-white border-2 border-[var(--light-border-color)] hover:border-md-primary'
          ]">
          
            <!-- Popular Badge -->
            <div v-if="tier.popular"
              class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold">
              POPULAR
            </div>

            <div class="text-center mb-6">
              <h3 :class="[
                'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
                tier.popular ? '' : 'text-[var(--dark-text-color)]'
              ]">
                {{ tier.displayName }}
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
              <li v-for="(feature, fIndex) in tier.features" :key="fIndex" :class="[
                'flex items-start gap-3',
                feature.included ? '' : 'opacity-50'
              ]">
                <i :class="[
                  'mt-1',
                  feature.included
                    ? `fas fa-check-circle ${tier.popular ? 'text-white' : 'text-[var(--accent3-color)]'}`
                    : 'fas fa-times-circle text-[var(--gray-text-color)]'
                ]"></i>
                <span
                  :class="tier.popular ? '' : feature.included ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'">
                  {{ feature.text }}
                </span>
              </li>
            </ul>

            <NuxtLink :to="tier.buttonLink" :class="[
              'block w-full text-center px-6 py-3 rounded-lg transition-all font-semibold',
              tier.popular
                ? 'bg-white text-md-primary hover:bg-opacity-90 shadow-lg'
                : tier.price === 0
                  ? 'border-2 border-md-primary text-md-primary hover:bg-md-primary hover:text-white'
                  : 'border-2 border-md-primary text-md-primary hover:bg-md-primary hover:text-white'
            ]">
              {{ tier.buttonText }}
            </NuxtLink>
          </div>
            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Starter
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  Perfect for small businesses
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R299</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
                <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  R2,871/year (save R717)
                </p>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Up to 5 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Digital menu (1 menu)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Email support</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-md-primary p-8 hover:shadow-2xl transition-all relative transform lg:scale-105">
              <div class="absolute -top-4 left-1/2 -translate-x-1/2">
                <span class="bg-md-primary text-white px-4 py-1 rounded-full text-xs font-bold">
                  MOST POPULAR
                </span>
              </div>
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-md-primary bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-star text-md-primary text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Professional
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  Best for growing restaurants
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R599</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
                <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  R5,751/year (save R1,437)
                </p>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Up to 15 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Digital menu (3 menus)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Advanced analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Event management</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Priority support</span>
                </li>
              </ul>
              <button
                class="w-full bg-md-primary text-white py-3 rounded-lg font-bold hover:bg-[var(--primary-button-hover-bg-color)] transition-colors">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent2-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-building text-[var(--accent2-color)] text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Business
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  For established venues
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R999</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
                <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  R9,591/year (save R2,397)
                </p>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>Up to 30 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>Unlimited menus</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>Advanced analytics + reports</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>Full event management</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>Custom branding</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent2-color)] mt-1" aria-hidden="true"></i>
                  <span>24/7 priority support</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent2-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>

            <div
              class="bg-gradient-to-br from-[var(--accent4-color)] to-[var(--accent-color)] text-white rounded-2xl p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-white bg-opacity-20 backdrop-blur-sm rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-crown text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold mb-2">Enterprise</h3>
                <p class="text-sm text-white text-opacity-90">Custom solutions</p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold">Custom</span>
                <p class="text-sm text-white text-opacity-90 mt-2">
                  Tailored pricing for your needs
                </p>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>Unlimited everything</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>Multi-location support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>White-label options</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>API access</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>Custom integrations</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>Dedicated account manager</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-white mt-1" aria-hidden="true"></i>
                  <span>SLA guarantee</span>
                </li>
              </ul>
              <button
                class="w-full bg-white text-[var(--accent-color)] py-3 rounded-lg font-bold hover:bg-opacity-90 transition-opacity">
                Contact Sales
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="px-4 py-12">
        <div class="max-w-7xl mx-auto">
          <div class="text-center mb-12">
            <h2
              class="text-3xl lg:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
              Enhance Your Plan with Add-ons
            </h2>
            <p class="text-lg text-[var(--gray-text-color)]">
              Customize your experience with optional features
            </p>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-[var(--accent3-color)] bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-table text-[var(--accent3-color)] text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                Extra Tables
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                Add 10 more table personalities
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R99</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
            </div>

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-md-primary bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-chart-line text-md-primary text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                Advanced Analytics
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                Deep insights &amp; custom reports
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R149</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
            </div>

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-[var(--accent4-color)] bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-nfc-symbol text-[var(--accent4-color)] text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                NFC Tags Pack
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                10 premium NFC tags delivered
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R499</span>
                <span class="text-[var(--gray-text-color)]">one-time</span>
              </div>
            </div>

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-[var(--accent2-color)] bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-sms text-[var(--accent2-color)] text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                SMS Notifications
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                Automated reservation reminders
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R79</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
            </div>

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-[var(--accent3-color)] bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-globe text-[var(--accent3-color)] text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                Custom Domain
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                Use your own domain name
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R199</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
            </div>

            <div
              class="bg-white rounded-xl border border-[var(--light-border-color)] p-6 hover:shadow-lg transition-all">
              <div class="flex items-start justify-between mb-4">
                <div
                  class="w-12 h-12 bg-md-primary bg-opacity-10 rounded-lg flex items-center justify-center">
                  <i class="fas fa-headset text-md-primary text-xl" aria-hidden="true"></i>
                </div>
                <label class="flex items-center cursor-pointer">
                  <input type="checkbox" class="w-5 h-5 text-md-primary rounded" value="on">
                </label>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">
                Priority Support
              </h3>
              <p class="text-sm text-[var(--gray-text-color)] mb-4">
                24/7 dedicated support team
              </p>
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-[var(--dark-text-color)]">R299</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
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
    console.log(response.data);
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.tiers)) {
      pricingTiers.value = response.data.tiers
    } else if (Array.isArray(response.data)) {
      pricingTiers.value = response.data
    } else {
      pricingTiers.value = []
    }
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





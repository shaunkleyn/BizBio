<template>
  <div>
    <section class="bg-[var(--light-background-color)] py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h1 class="text-4xl sm:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Help Center
          </h1>
          <p class="text-lg text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Find answers to common questions
          </p>
        </div>

        <div class="space-y-4">
          <div v-for="(item, index) in faqItems" :key="index" class="bg-white rounded-xl p-6 shadow-md">
            <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-3">
              {{ item.question }}
            </h3>
            <p class="text-[var(--gray-text-color)]">
              {{ item.answer }}
            </p>
          </div>
        </div>

        <div class="mt-12 text-center">
          <p class="text-[var(--gray-text-color)] mb-4">Can't find what you're looking for?</p>
          <NuxtLink
            to="/contact"
            class="inline-flex items-center bg-[var(--primary-color)] text-white px-8 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
          >
            Contact Support
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const faqItems = [
  {
    question: 'How do I create a business profile?',
    answer: 'Sign up for an account, navigate to your dashboard, and click "Create New Profile". Fill in your business details and customize your profile to match your brand.'
  },
  {
    question: 'What is an NFC card?',
    answer: 'NFC (Near Field Communication) cards allow people to tap their phone on your card to instantly view your digital profile. It\'s a modern, contactless way to share your business information.'
  },
  {
    question: 'Can I have multiple profiles?',
    answer: 'Yes! Depending on your plan, you can create multiple profiles for different businesses, personal brands, or use cases.'
  },
  {
    question: 'How do I share my profile?',
    answer: 'You can share your profile via a unique URL, QR code, or NFC tap. All sharing options are available in your profile dashboard.'
  },
  {
    question: 'Is there a mobile app?',
    answer: 'Our platform is fully responsive and works great on mobile browsers. A dedicated mobile app is coming soon!'
  }
]

useHead({
  title: 'Help Center',
})
</script>

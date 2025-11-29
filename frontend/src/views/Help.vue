<script setup>
import { ref } from 'vue'

const faqs = ref([
  {
    question: 'How do I create a digital business card?',
    answer: 'Simply sign up for a free account, choose a template, and fill in your information. Your digital business card will be live in minutes!'
  },
  {
    question: 'Can I use NFC technology?',
    answer: 'Yes! You can order NFC cards or stickers from our store and link them to your profile for tap-to-share functionality.'
  },
  {
    question: 'Is there a free plan?',
    answer: 'Yes, we offer a free plan with basic features. You can upgrade anytime to access premium features.'
  },
  {
    question: 'How do I share my profile?',
    answer: 'You can share your profile via QR code, NFC tap, or a direct link. All sharing methods are available in your dashboard.'
  }
])
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-heading font-bold text-brand-dark-text mb-4">
          Help Center
        </h1>
        <p class="text-xl text-brand-gray-text">
          Find answers to common questions
        </p>
      </div>

      <div class="space-y-4">
        <div
          v-for="(faq, index) in faqs"
          :key="index"
          class="bg-white rounded-lg p-6 shadow-sm"
        >
          <h3 class="font-heading font-bold text-brand-dark-text mb-2">
            {{ faq.question }}
          </h3>
          <p class="text-brand-gray-text">
            {{ faq.answer }}
          </p>
        </div>
      </div>

      <div class="mt-12 bg-primary/5 rounded-lg p-8 text-center">
        <h3 class="text-xl font-heading font-bold text-brand-dark-text mb-4">
          Still need help?
        </h3>
        <router-link
          to="/contact"
          class="inline-flex items-center gap-2 px-8 py-3 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold"
        >
          <i class="fas fa-envelope"></i>
          Contact Support
        </router-link>
      </div>
    </div>
  </div>
</template>

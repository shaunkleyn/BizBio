<script setup>
import { ref } from 'vue'

const form = ref({
  name: '',
  email: '',
  subject: '',
  message: ''
})

const handleSubmit = () => {
  // TODO: Implement contact form submission
  console.log('Contact form submitted:', form.value)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-heading font-bold text-brand-dark-text mb-4">
          Contact Us
        </h1>
        <p class="text-xl text-brand-gray-text">
          We'd love to hear from you
        </p>
      </div>

      <div class="bg-white rounded-lg p-8 shadow">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="grid sm:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-900 mb-2">Name</label>
              <input
                v-model="form.name"
                type="text"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary"
                placeholder="Your name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-900 mb-2">Email</label>
              <input
                v-model="form.email"
                type="email"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary"
                placeholder="your@email.com"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-900 mb-2">Subject</label>
            <input
              v-model="form.subject"
              type="text"
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary"
              placeholder="How can we help?"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-900 mb-2">Message</label>
            <textarea
              v-model="form.message"
              required
              rows="6"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary"
              placeholder="Tell us more..."
            ></textarea>
          </div>

          <button
            type="submit"
            class="w-full px-8 py-4 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold text-lg"
          >
            <i class="fas fa-paper-plane mr-2"></i>
            Send Message
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

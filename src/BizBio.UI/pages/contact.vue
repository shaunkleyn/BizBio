<template>
  <div>
    <section class="bg-gradient-to-br from-[var(--light-background-color)] to-white py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h1 class="text-4xl sm:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Contact Us
          </h1>
          <p class="text-lg text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Have a question? We're here to help.
          </p>
        </div>

        <div class="bg-white rounded-2xl shadow-2xl p-8">
          <div v-if="submitted" class="text-center py-12">
            <i class="fas fa-check-circle text-6xl text-[var(--accent3-color)] mb-4"></i>
            <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">Message Sent!</h2>
            <p class="text-[var(--gray-text-color)]">We'll get back to you within 24 hours.</p>
          </div>

          <form v-else @submit.prevent="handleSubmit" class="space-y-6">
            <div class="grid sm:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Name</label>
                <input
                  type="text"
                  v-model="form.name"
                  required
                  class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
                />
              </div>
              <div>
                <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Email</label>
                <input
                  type="email"
                  v-model="form.email"
                  required
                  class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Subject</label>
              <input
                type="text"
                v-model="form.subject"
                required
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
              />
            </div>

            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Message</label>
              <textarea
                v-model="form.message"
                required
                rows="6"
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
              ></textarea>
            </div>

            <button
              type="submit"
              class="w-full bg-[var(--primary-color)] text-white px-6 py-4 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold text-lg"
            >
              Send Message
            </button>
          </form>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const form = ref({
  name: '',
  email: '',
  subject: '',
  message: ''
})

const submitted = ref(false)

const handleSubmit = () => {
  submitted.value = true
}

useHead({
  title: 'Contact Us',
})
</script>

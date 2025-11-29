<template>
  <div>
    <section class="bg-white py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-8">
          Privacy Policy
        </h1>

        <div class="prose max-w-none">
          <p class="text-[var(--gray-text-color)] mb-6">Last updated: {{ new Date().toLocaleDateString() }}</p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">1. Information We Collect</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            We collect information you provide directly to us, such as when you create an account, create a profile, or contact us for support.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">2. How We Use Your Information</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            We use the information we collect to provide, maintain, and improve our services, to communicate with you, and to protect BizBio and our users.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">3. Information Sharing</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            We do not share your personal information with third parties except as described in this policy or with your consent.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">4. Security</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            We take reasonable measures to help protect your personal information from loss, theft, misuse, and unauthorized access.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">5. Your Rights</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            You have the right to access, update, or delete your personal information at any time through your account settings.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">6. Contact Us</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            If you have questions about this Privacy Policy, please <NuxtLink to="/contact" class="text-[var(--primary-color)] hover:underline">contact us</NuxtLink>.
          </p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
useHead({
  title: 'Privacy Policy',
})
</script>

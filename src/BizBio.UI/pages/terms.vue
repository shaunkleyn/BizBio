<template>
  <div>
    <section class="bg-white py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-8">
          Terms of Service
        </h1>

        <div class="prose max-w-none">
          <p class="text-[var(--gray-text-color)] mb-6">Last updated: {{ new Date().toLocaleDateString() }}</p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">1. Acceptance of Terms</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            By accessing and using BizBio, you accept and agree to be bound by the terms and provision of this agreement.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">2. Use License</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            Permission is granted to use BizBio for creating and managing digital business profiles for commercial purposes.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">3. User Accounts</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            You are responsible for maintaining the confidentiality of your account and password and for restricting access to your account.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">4. Content</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            You retain all rights to the content you upload to BizBio. By uploading content, you grant us a license to display and distribute that content through our platform.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">5. Termination</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            We may terminate or suspend your account at any time for violations of these terms.
          </p>

          <h2 class="text-2xl font-bold text-[var(--dark-text-color)] mt-8 mb-4">6. Contact</h2>
          <p class="text-[var(--gray-text-color)] mb-6">
            For questions about these Terms of Service, please <NuxtLink to="/contact" class="text-[var(--primary-color)] hover:underline">contact us</NuxtLink>.
          </p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
useHead({
  title: 'Terms of Service',
})
</script>

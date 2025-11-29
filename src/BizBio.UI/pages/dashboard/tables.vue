<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-4xl mx-auto px-4">
      <h1 class="text-3xl font-bold text-[var(--dark-text-color)] mb-8">NFC Tables Manager</h1>
      <div class="bg-white rounded-xl shadow-lg p-8">
        <p class="text-[var(--gray-text-color)]">NFC tables manager coming soon...</p>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({ middleware: 'auth' })
useHead({ title: 'NFC Tables' })
</script>

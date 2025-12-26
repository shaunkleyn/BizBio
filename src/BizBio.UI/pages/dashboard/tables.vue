<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-4xl mx-auto px-4">
      <h1 class="text-3xl font-bold text-md-on-background mb-8 gradient-text">NFC Tables Manager</h1>
      <div class="mesh-card bg-md-surface rounded-2xl shadow-md-4 p-8">
        <p class="text-md-on-surface-variant">NFC tables manager coming soon...</p>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({ middleware: 'auth' })
useHead({ title: 'NFC Tables' })
</script>

<template>
  <div class="min-h-screen bg-[var(--light-background-color)]">
    <!-- Global Header -->
    <GlobalHeader />

    <!-- Main Content -->
    <main class="py-8">
      <slot />
    </main>
  </div>
</template>

<script setup>
// Provide navigation links for dashboard
provide('globalNavLinks', [
  { to: '/dashboard', label: 'Overview', icon: 'fas fa-th-large' },
  { to: '/dashboard/profile', label: 'Profile', icon: 'fas fa-user' },
  { to: '/dashboard/subscription', label: 'Subscription', icon: 'fas fa-credit-card' }
])
</script>

<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-4xl mx-auto px-4">
      <h1 class="text-3xl font-bold text-md-on-background mb-8 gradient-text">Menu Management</h1>
      <div class="mesh-card bg-md-surface rounded-2xl shadow-md-4 p-8">
        <div class="text-center mb-6">
          <div class="inline-block bg-gradient-primary p-6 rounded-full mb-4 shadow-glow-purple">
            <i class="fas fa-utensils text-6xl text-white"></i>
          </div>
          <p class="text-md-on-surface-variant mb-6">Create and manage your digital menus</p>
          <NuxtLink
            to="/dashboard/menu/create"
            class="btn-gradient inline-block px-8 py-3 rounded-xl hover:shadow-md-4 transition-all font-semibold text-white"
          >
            <i class="fas fa-plus-circle mr-2"></i>
            Create New Menu
          </NuxtLink>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({ middleware: 'auth' })
useHead({ title: 'Menu Management' })
</script>

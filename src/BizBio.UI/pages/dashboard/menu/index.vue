<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-4xl mx-auto px-4">
      <h1 class="text-3xl font-bold text-[var(--dark-text-color)] mb-8">Menu Management</h1>
      <div class="bg-white rounded-xl shadow-lg p-8">
        <div class="text-center mb-6">
          <i class="fas fa-utensils text-6xl text-[var(--primary-color)] mb-4"></i>
          <p class="text-[var(--gray-text-color)] mb-6">Create and manage your digital menus</p>
          <NuxtLink
            to="/dashboard/menu/create"
            class="inline-block bg-[var(--primary-color)] text-white px-8 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
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

<template>
  <div class="min-h-screen bg-[var(--light-background-color)]">
    <!-- Dashboard Header -->
    <header class="bg-white shadow-md">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-20">
          <!-- Logo -->
          <NuxtLink to="/dashboard" class="flex-shrink-0">
            <img
              src="/151.avif"
              alt="BizBio"
              class="h-16"
            />
          </NuxtLink>

          <!-- Desktop Navigation -->
          <nav class="hidden lg:flex items-center space-x-6">
            <NuxtLink
              to="/dashboard"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-th-large mr-2"></i>
              Overview
            </NuxtLink>
            <NuxtLink
              to="/dashboard/profile"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-user mr-2"></i>
              Profile
            </NuxtLink>
            <NuxtLink
              to="/dashboard/menu"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-utensils mr-2"></i>
              Menu
            </NuxtLink>
            <NuxtLink
              to="/dashboard/tables"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-qrcode mr-2"></i>
              Tables
            </NuxtLink>
            <NuxtLink
              to="/dashboard/subscription"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-credit-card mr-2"></i>
              Subscription
            </NuxtLink>
            <button
              @click="handleLogout"
              class="text-[var(--gray-text-color)] hover:text-[var(--accent-color)] transition-colors duration-200 font-[var(--font-family-body)]"
            >
              <i class="fas fa-sign-out-alt mr-2"></i>
              Logout
            </button>
          </nav>

          <!-- Mobile Menu Toggle -->
          <button
            @click="mobileMenuOpen = !mobileMenuOpen"
            class="lg:hidden text-[var(--dark-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200"
          >
            <i class="fas text-2xl" :class="mobileMenuOpen ? 'fa-times' : 'fa-bars'"></i>
          </button>
        </div>

        <!-- Mobile Navigation -->
        <nav v-if="mobileMenuOpen" class="lg:hidden pb-4">
          <div class="flex flex-col space-y-3">
            <NuxtLink
              to="/dashboard"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
              @click="mobileMenuOpen = false"
            >
              <i class="fas fa-th-large mr-2"></i>
              Overview
            </NuxtLink>
            <NuxtLink
              to="/dashboard/profile"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
              @click="mobileMenuOpen = false"
            >
              <i class="fas fa-user mr-2"></i>
              Profile
            </NuxtLink>
            <NuxtLink
              to="/dashboard/menu"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
              @click="mobileMenuOpen = false"
            >
              <i class="fas fa-utensils mr-2"></i>
              Menu
            </NuxtLink>
            <NuxtLink
              to="/dashboard/tables"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
              @click="mobileMenuOpen = false"
            >
              <i class="fas fa-qrcode mr-2"></i>
              Tables
            </NuxtLink>
            <NuxtLink
              to="/dashboard/subscription"
              class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
              @click="mobileMenuOpen = false"
            >
              <i class="fas fa-credit-card mr-2"></i>
              Subscription
            </NuxtLink>
            <button
              @click="handleLogout"
              class="text-[var(--accent-color)] hover:text-[var(--accent-color)] transition-colors duration-200 py-2 text-left"
            >
              <i class="fas fa-sign-out-alt mr-2"></i>
              Logout
            </button>
          </div>
        </nav>
      </div>
    </header>

    <!-- Main Content -->
    <main class="py-8">
      <slot />
    </main>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
const router = useRouter()
const mobileMenuOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  mobileMenuOpen = false
}

// Ensure user is authenticated
definePageMeta({
  middleware: 'auth'
})
</script>

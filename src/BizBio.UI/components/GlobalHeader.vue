<template>
  <header class="bg-[var(--light-background-color)] shadow-md sticky top-0 z-50 border-b border-[var(--light-border-color)]">
    <div class="mx-auto px-4 sm:px-6">
      <div class="flex justify-between items-center h-20">
        <!-- Logo -->
        <NuxtLink to="/dashboard" class="flex-shrink-0">
          <img
            src="/logo.svg"
            alt="BizBio"
            class="h-10"
          />
        </NuxtLink>

        <!-- Desktop Navigation -->
        <nav class="hidden lg:flex items-center space-x-6">
          <NuxtLink
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
          >
            <i :class="[link.icon, 'mr-2']"></i>
            {{ link.label }}
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
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
            @click="mobileMenuOpen = false"
          >
            <i :class="[link.icon, 'mr-2']"></i>
            {{ link.label }}
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
</template>

<script setup lang="ts">
import { inject, ref } from 'vue'

interface NavLink {
  to: string
  label: string
  icon: string
}

const authStore = useAuthStore()
const router = useRouter()
const mobileMenuOpen = ref(false)

// Inject navigation links from the layout/page
const defaultNavLinks: NavLink[] = [
  { to: '/dashboard', label: 'Overview', icon: 'fas fa-th-large' },
  { to: '/dashboard/profile', label: 'Profile', icon: 'fas fa-user' },
  { to: '/dashboard/subscription', label: 'Subscription', icon: 'fas fa-credit-card' }
]

const navLinks = inject<NavLink[]>('globalNavLinks', defaultNavLinks)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  mobileMenuOpen.value = false
}
</script>

<template>
  <header class="glass-effect sticky top-0 z-50">
    <div class="mx-auto px-4 sm:px-6">
      <div class="flex justify-between items-center h-16">
        <!-- Logo with Gradient -->
        <NuxtLink to="/dashboard" class="flex-shrink-0 flex items-center gap-3 group">
          <div class="relative">
            <div class="w-11 h-11 rounded-xl bg-gradient-primary flex items-center justify-center shadow-md-2 group-hover:shadow-glow-purple transition-all group-hover:scale-105">
              <span class="text-white text-xl font-bold">B</span>
            </div>
            <div class="absolute -top-1 -right-1 w-3 h-3 bg-md-accent rounded-full shadow-glow-pink"></div>
          </div>
          <div class="hidden sm:block">
            <span class="gradient-text font-heading font-bold text-2xl">BizBio</span>
            <div class="h-0.5 w-0 group-hover:w-full bg-gradient-primary transition-all duration-300"></div>
          </div>
        </NuxtLink>

        <!-- Desktop Navigation -->
        <nav class="hidden lg:flex items-center gap-1">
          <button
            v-for="link in navLinks"
            :key="link.to"
            @click="handleNavigation(link.to)"
            class="relative px-5 py-2.5 rounded-xl text-md-on-surface-variant hover:text-md-primary font-medium transition-all duration-200 group md-ripple"
          >
            <span class="relative z-10 flex items-center gap-2">
              <i :class="link.icon"></i>
              {{ link.label }}
            </span>
            <div class="absolute inset-0 bg-md-primary-container opacity-0 group-hover:opacity-100 rounded-xl transition-opacity"></div>
          </button>
          <div class="w-px h-6 bg-md-outline-variant mx-2"></div>
          <button
            @click="handleLogout"
            class="relative px-5 py-2.5 rounded-xl text-md-error font-medium transition-all duration-200 group md-ripple"
          >
            <span class="relative z-10 flex items-center gap-2">
              <i class="fas fa-sign-out-alt"></i>
              Logout
            </span>
            <div class="absolute inset-0 bg-md-error-container opacity-0 group-hover:opacity-100 rounded-xl transition-opacity"></div>
          </button>
        </nav>

        <!-- Mobile Menu Toggle -->
        <button
          @click="mobileMenuOpen = !mobileMenuOpen"
          class="lg:hidden w-12 h-12 rounded-xl bg-gradient-primary text-white shadow-md-2 hover:shadow-glow-purple transition-all duration-200 flex items-center justify-center md-ripple"
        >
          <i class="fas text-lg transition-transform duration-200" :class="mobileMenuOpen ? 'fa-times rotate-90' : 'fa-bars'"></i>
        </button>
      </div>

      <!-- Mobile Navigation -->
      <Transition
        enter-active-class="transition-all duration-200"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
      >
        <nav v-if="mobileMenuOpen" class="lg:hidden pb-4 space-y-1 mt-2">
          <button
            v-for="link in navLinks"
            :key="link.to"
            @click="handleNavigation(link.to)"
            class="w-full flex items-center gap-3 px-4 py-3 rounded-xl text-md-on-surface hover:bg-md-primary-container hover:text-md-on-primary-container transition-all duration-200 md-ripple"
          >
            <i :class="[link.icon, 'w-5']"></i>
            {{ link.label }}
          </button>
          <div class="h-px bg-md-outline-variant my-2"></div>
          <button
            @click="handleLogout"
            class="w-full flex items-center gap-3 px-4 py-3 rounded-xl text-md-error hover:bg-md-error-container hover:text-md-on-error-container transition-all duration-200 md-ripple"
          >
            <i class="fas fa-sign-out-alt w-5"></i>
            Logout
          </button>
        </nav>
      </Transition>
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

const handleNavigation = (path: string) => {
  mobileMenuOpen.value = false
  router.push(path)
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  mobileMenuOpen.value = false
}
</script>

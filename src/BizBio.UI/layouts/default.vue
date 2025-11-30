<template>
  <div class="min-h-screen flex flex-col">
    <!-- Global Header -->
    <header id="global-header" class="bg-white bg-opacity-70 bg-blur-[10px] dark:bg-[var(--dark-bg)] shadow-md sticky top-0 z-50  transparent-navbar">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-20">
          <!-- Logo -->
          <NuxtLink to="/" class="flex-shrink-0 justify-between items-center flex flex-row">
            <img
              src="/logo.png"
              alt="BizBio"
              class="h-20 py-4"
            />
            <a class="text-4xl monterrat text-[var(--primary-color)]">BiZBiO</a>
          </NuxtLink>

          <!-- Desktop Navigation -->
          <nav class="hidden lg:flex items-center space-x-8">
            <template v-if="!authStore.isAuthenticated">
              <NuxtLink
                to="/login"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
              >
                Login
              </NuxtLink>
              <NuxtLink
                to="/products"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
              >
                Products
              </NuxtLink>
              <NuxtLink
                to="/pricing"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">
                Pricing
              </NuxtLink>
              <NuxtLink
                to="/search"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
              >
                Businesses
              </NuxtLink>
              <NuxtLink
                to="/help"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">
                Help Center
              </NuxtLink>
              <NuxtLink
                to="/register"
                class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-3 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)]"
              >
                Claim Your Profile
              </NuxtLink>
            </template>
            <template v-else>
              <NuxtLink
                to="/dashboard"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
              >
                Dashboard
              </NuxtLink>
              <NuxtLink
                to="/categories"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]"
              >
                Browse
              </NuxtLink>
              <button
                @click="handleLogout"
                class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-3 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)]"
              >
                Logout
              </button>
            </template>
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
            <template v-if="!authStore.isAuthenticated">
              <NuxtLink
                to="/login"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Login
              </NuxtLink>
              <NuxtLink
                to="/categories"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Business Categories
              </NuxtLink>
              <NuxtLink
                to="/search"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Search Businesses
              </NuxtLink>
              <NuxtLink
                to="/help"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Help Center
              </NuxtLink>
              <NuxtLink
                to="/register"
                class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-3 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold text-center"
                @click="mobileMenuOpen = false"
              >
                Claim Your Profile
              </NuxtLink>
            </template>
            <template v-else>
              <NuxtLink
                to="/dashboard"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Dashboard
              </NuxtLink>
              <NuxtLink
                to="/categories"
                class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 py-2"
                @click="mobileMenuOpen = false"
              >
                Browse
              </NuxtLink>
              <button
                @click="handleLogout"
                class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-3 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold"
              >
                Logout
              </button>
            </template>
          </div>
        </nav>
      </div>
    </header>

    <!-- Main Content -->
    <main class="flex-1">
      <slot />
    </main>

    <!-- Footer -->
    <footer class="bg-[var(--dark-background-color)] text-white py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
          <!-- Company Info -->
          <div>
            <h3 class="text-xl font-bold mb-4 font-[var(--font-family-heading)]">BizBio</h3>
            <p class="text-gray-400 mb-4">
              Create and share your digital business profile effortlessly.
            </p>
          </div>

          <!-- Quick Links -->
          <div>
            <h4 class="text-lg font-semibold mb-4 font-[var(--font-family-heading)]">Quick Links</h4>
            <ul class="space-y-2">
              <li>
                <NuxtLink to="/pricing" class="text-gray-400 hover:text-white transition-colors">
                  Pricing
                </NuxtLink>
              </li>
              <li>
                <NuxtLink to="/categories" class="text-gray-400 hover:text-white transition-colors">
                  Categories
                </NuxtLink>
              </li>
              <li>
                <NuxtLink to="/help" class="text-gray-400 hover:text-white transition-colors">
                  Help Center
                </NuxtLink>
              </li>
            </ul>
          </div>

          <!-- Legal -->
          <div>
            <h4 class="text-lg font-semibold mb-4 font-[var(--font-family-heading)]">Legal</h4>
            <ul class="space-y-2">
              <li>
                <NuxtLink to="/terms" class="text-gray-400 hover:text-white transition-colors">
                  Terms of Service
                </NuxtLink>
              </li>
              <li>
                <NuxtLink to="/privacy" class="text-gray-400 hover:text-white transition-colors">
                  Privacy Policy
                </NuxtLink>
              </li>
            </ul>
          </div>

          <!-- Contact -->
          <div>
            <h4 class="text-lg font-semibold mb-4 font-[var(--font-family-heading)]">Contact</h4>
            <ul class="space-y-2">
              <li>
                <NuxtLink to="/contact" class="text-gray-400 hover:text-white transition-colors">
                  Contact Us
                </NuxtLink>
              </li>
            </ul>
          </div>
        </div>

        <div class="border-t border-gray-700 mt-8 pt-8 text-center text-gray-400">
          <p>&copy; {{ new Date().getFullYear() }} BizBio. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
const router = useRouter()
const mobileMenuOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  mobileMenuOpen.value = false
}
</script>

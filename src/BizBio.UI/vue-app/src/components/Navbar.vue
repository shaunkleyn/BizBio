<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const isAuthenticated = computed(() => authStore.isAuthenticated)
const currentUser = computed(() => authStore.currentUser)

const logout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
            <header id="global-header" class="code-section bg-white shadow-md sticky top-0 z-50">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <div class="flex justify-between items-center h-28">
                <!-- Logo -->
                <a href="https://app.landingsite.ai/" class="flex-shrink-0">
                  <img src="/src/assets/images/logo.png" alt="busprofile" class="h-28 py-2" data-logo
                    data-sf-original-src="https://imagedelivery.net/xaKlCos5cTg_1RWzIu_h-A/57adc6f7-d5b3-4905-5aa2-8c11e8559b00/public">
                </a>

                <!-- Desktop Navigation -->
                <nav class="hidden lg:flex items-center space-x-8">
                  <a href="https://app.landingsite.ai/login"
                    class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">Login</a>
                  <a href="https://app.landingsite.ai/categories"
                    class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">Business
                    Categories</a>
                  <a href="https://app.landingsite.ai/search"
                    class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">Search
                    Businesses</a>
                  <a href="https://app.landingsite.ai/help"
                    class="text-[var(--gray-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 font-[var(--font-family-body)]">Help
                    Center</a>
                  <a href="https://app.landingsite.ai/register"
                    class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-3 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)]">Claim
                    Your Profile</a>
                </nav>

                <!-- Mobile Menu Toggle -->
                <button data-landingsite-mobile-menu-toggle
                  class="lg:hidden text-[var(--dark-text-color)] hover:text-[var(--primary-color)] transition-colors duration-200 sf-hidden">

                </button>
              </div>

              <!-- Mobile Navigation -->
              <nav data-landingsite-mobile-menu class="hidden lg:hidden pb-4 sf-hidden">

              </nav>
            </div>
          </header>
  <header class="bg-white shadow-md sticky top-0 z-50">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="flex justify-between items-center h-20">
                <div class="flex-shrink-0">
                    <a href="index.html" class="text-2xl font-bold text-gray-900">
                        <i class="fas fa-qrcode text-blue-600 mr-2"></i>BizBio
                    </a>
                </div>
                <nav class="hidden lg:flex items-center space-x-8">
                    <a href="index.html" class="text-gray-600 hover:text-blue-600 transition-colors duration-200">Home</a>
                    <a href="products.html" class="text-blue-600 font-semibold">Products</a>
                    <a href="pricing.html" class="text-gray-600 hover:text-blue-600 transition-colors duration-200">Pricing</a>
                    <a href="contact.html" class="text-gray-600 hover:text-blue-600 transition-colors duration-200">Contact</a>
                    <a href="#" class="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-all duration-200 font-semibold">Get Started</a>
                </nav>
            </div>
        </div>
    </header>
</template>

<style scoped>
.navbar {
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.navbar-brand {
  font-weight: bold;
  font-size: 1.5rem;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #5568d3 0%, #63398b 100%);
}
</style>

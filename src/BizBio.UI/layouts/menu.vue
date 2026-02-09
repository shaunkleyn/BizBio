<!-- <template>
  <div>
    <NuxtLayout name="LayoutA">
      <div>
        <Sidebar/>
        <div>
          <InnerMenuBar/>
          <slot />
        </div>
      </div>
    </NuxtLayout>
  </div>
</template> -->

<template>
  <div class="min-h-screen bg-[var(--medium-background-color)]">
    <!-- Global Header -->
    <Default>
      <div class="flex">
        <!-- Menu Sidebar -->
        <MenuSidebar :stats="stats" />

        <!-- Main Content Area -->
        <div class="flex-1 mesh-bg">
          <!-- Product Switcher with Page Header -->
          <ProductSwitcher />

          <!-- Main Content -->
          <!-- <main> -->
            <div class="p-4 md:p-8">
              <slot/>
            </div>
          <!-- </main> -->
        </div>
      </div>
    </Default>
    
  </div>
</template>

<script setup lang="ts">
import { inject, ref, type Ref } from 'vue'
import Default from './default.vue'
const authStore = useAuthStore()
const router = useRouter()
const mobileMenuOpen = ref(false)
const { initConsent } = useCookieConsent()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  mobileMenuOpen.value = false
}

// Initialize cookie consent on mount
onMounted(() => {
  initConsent()
})
// Provide navigation links for menu section
provide('globalNavLinks', [
  { to: '/dashboard', label: 'Dashboard', icon: 'fas fa-home' },
  { to: '/menu', label: 'Menu', icon: 'fas fa-utensils' },
  { to: '/dashboard/profile', label: 'Profile', icon: 'fas fa-user' },
  { to: '/dashboard/subscription', label: 'Subscription', icon: 'fas fa-credit-card' }
])

// Inject stats from pages that provide them
const stats = inject<Ref<{
  menus?: number
  items?: number
  categories?: number
}>>('menuStats', ref({
  menus: 0,
  items: 0,
  categories: 0
}))
</script>

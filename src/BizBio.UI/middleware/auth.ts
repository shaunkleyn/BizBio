export default defineNuxtRouteMiddleware((to) => {
  // Skip auth check on server side
  if (import.meta.server) {
    return
  }

  const authStore = useAuthStore()

  // Initialize auth from localStorage if not already initialized
  if (!authStore.isAuthenticated) {
    authStore.initAuth()
  }

  // Check authentication after init
  if (!authStore.isAuthenticated) {
    return navigateTo({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  }
})

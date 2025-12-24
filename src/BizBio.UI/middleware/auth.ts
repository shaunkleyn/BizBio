export default defineNuxtRouteMiddleware((to) => {
  // Skip auth check on server side
  if (import.meta.server) {
    return
  }

  const authStore = useAuthStore()

  // Always initialize auth from localStorage on first access
  authStore.initAuth()

  // Check authentication after init
  if (!authStore.isAuthenticated) {
    return navigateTo({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  }
})

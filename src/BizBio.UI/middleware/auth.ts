export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore()

  // If user is not authenticated, redirect to login
  if (!authStore.isAuthenticated) {
    return navigateTo({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  }
})

export default defineNuxtPlugin(() => {
  if (import.meta.client) {
    const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)')

    const updateDarkMode = (e: MediaQueryList | MediaQueryListEvent) => {
      if (e.matches) {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
    }

    // Apply dark mode on initial load
    updateDarkMode(darkModeQuery)

    // Listen for changes in system preference
    darkModeQuery.addEventListener('change', updateDarkMode)
  }
})

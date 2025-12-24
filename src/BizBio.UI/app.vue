<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
    <ToastContainer />
  </div>
</template>

<script setup>
// Initialize auth store on app load
const authStore = useAuthStore()

if (import.meta.client) {
  authStore.initAuth()
}

// Set page transition
useHead({
  titleTemplate: (titleChunk) => {
    return titleChunk ? `${titleChunk} - BizBio` : 'BizBio - Digital Business Profiles';
  }
})
</script>

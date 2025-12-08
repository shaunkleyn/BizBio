<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
    <ToastContainer />
  </div>
</template>

<script setup>
// Set page transition
useHead({
  titleTemplate: (titleChunk) => {
    return titleChunk ? `${titleChunk} - BizBio` : 'BizBio - Digital Business Profiles';
  }
})
</script>

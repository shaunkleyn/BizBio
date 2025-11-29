<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<script setup>
// Set page transition
useHead({
  titleTemplate: (titleChunk) => {
    return titleChunk ? `${titleChunk} - BizBio` : 'BizBio - Digital Business Profiles';
  }
})
</script>

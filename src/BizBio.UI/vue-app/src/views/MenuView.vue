<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const menu = ref(null)
const loading = ref(true)

onMounted(async () => {
  const profileSlug = route.params.profileSlug
  // TODO: Fetch menu data from API
  loading.value = false
})
</script>

<template>
  <div class="container py-5">
    <div v-if="loading">
      <i class="fas fa-spinner fa-spin"></i> Loading menu...
    </div>
    <div v-else>
      <h2>Menu Viewer</h2>
      <p class="text-muted">Dynamic menu viewer (Coming soon)</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const menu = ref(null)
const table = ref(null)
const loading = ref(true)

onMounted(async () => {
  const { profileSlug, tableId } = route.params
  // TODO: Fetch menu and table data from API
  // TODO: Track NFC scan
  loading.value = false
})
</script>

<template>
  <div class="container py-5">
    <div v-if="loading">
      <i class="fas fa-spinner fa-spin"></i> Loading...
    </div>
    <div v-else>
      <h2>Table Menu View</h2>
      <p class="text-muted">NFC table menu viewer (Coming soon)</p>
    </div>
  </div>
</template>

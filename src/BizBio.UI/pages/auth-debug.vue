<template>
  <div class="p-8">
    <h1 class="text-2xl font-bold mb-4">Auth Debug</h1>
    <div class="bg-white p-4 rounded shadow">
      <h2 class="font-bold mb-2">Auth Store Status:</h2>
      <pre class="bg-gray-100 p-2 rounded">{{ authStatus }}</pre>

      <h2 class="font-bold mb-2 mt-4">LocalStorage:</h2>
      <pre class="bg-gray-100 p-2 rounded">{{ localStorageData }}</pre>

      <button
        @click="initAuth"
        class="mt-4 bg-blue-500 text-white px-4 py-2 rounded"
      >
        Init Auth
      </button>

      <NuxtLink
        to="/dashboard/menu/create"
        class="mt-4 ml-2 inline-block bg-green-500 text-white px-4 py-2 rounded"
      >
        Go to Menu Create
      </NuxtLink>
    </div>
  </div>
</template>

<script setup>
const authStore = useAuthStore()

const authStatus = computed(() => ({
  isAuthenticated: authStore.isAuthenticated,
  user: authStore.user,
  token: authStore.token ? `${authStore.token.substring(0, 20)}...` : null
}))

const localStorageData = ref({})

onMounted(() => {
  if (import.meta.client) {
    localStorageData.value = {
      token: localStorage.getItem('token') ? `${localStorage.getItem('token').substring(0, 20)}...` : null,
      user: localStorage.getItem('user')
    }
  }
})

const initAuth = () => {
  authStore.initAuth()
  // Refresh display
  if (import.meta.client) {
    localStorageData.value = {
      token: localStorage.getItem('token') ? `${localStorage.getItem('token').substring(0, 20)}...` : null,
      user: localStorage.getItem('user')
    }
  }
}

useHead({ title: 'Auth Debug' })
</script>

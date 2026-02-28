<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <!-- Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-md-on-surface mb-1">My Business Cards</h1>
        <p class="text-md-on-surface-variant">Manage your digital business cards</p>
      </div>
      <NuxtLink
        to="/dashboard/cards/create"
        class="btn-gradient px-6 py-3 rounded-xl text-white font-semibold shadow-md-2 hover:shadow-md-4 transition-all flex items-center gap-2"
      >
        <i class="fas fa-plus"></i>
        Create New Card
      </NuxtLink>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="text-center py-16">
      <div class="inline-block animate-spin rounded-full h-10 w-10 border-b-2 border-md-secondary mb-4"></div>
      <p class="text-md-on-surface-variant">Loading your cards...</p>
    </div>

    <!-- Empty state -->
    <div
      v-else-if="cards.length === 0"
      class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-16 text-center"
    >
      <div class="bg-gradient-secondary rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6 shadow-glow-pink">
        <i class="fas fa-id-card text-white text-3xl"></i>
      </div>
      <h2 class="text-2xl font-bold text-md-on-surface mb-3">No cards yet</h2>
      <p class="text-md-on-surface-variant mb-8 max-w-md mx-auto">
        Create your first digital business card and start sharing your contact information in style.
      </p>
      <NuxtLink
        to="/dashboard/cards/create"
        class="btn-gradient px-8 py-4 rounded-xl text-white font-bold shadow-md-2 hover:shadow-md-4 transition-all inline-flex items-center gap-2"
      >
        <i class="fas fa-magic"></i>
        Create My First Card
      </NuxtLink>
    </div>

    <!-- Cards grid -->
    <div v-else class="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="card in cards"
        :key="card.id"
        class="mesh-card bg-md-surface rounded-2xl shadow-md-3 overflow-hidden border border-md-outline-variant hover:shadow-md-4 transition-all group"
      >
        <!-- Card preview thumbnail -->
        <div class="h-40 bg-gradient-to-br from-md-secondary-container to-md-primary-container flex items-center justify-center relative">
          <i class="fas fa-id-card text-md-secondary/40 text-6xl"></i>
          <div class="absolute top-3 right-3">
            <span class="bg-md-surface/90 text-md-on-surface px-2 py-1 rounded-full text-xs font-semibold">
              {{ card.plan || 'Free' }}
            </span>
          </div>
        </div>

        <div class="p-5">
          <h3 class="font-bold text-md-on-surface text-lg mb-1">
            {{ card.basicInfo?.firstName }} {{ card.basicInfo?.lastName }}
          </h3>
          <p v-if="card.basicInfo?.headline" class="text-sm text-md-on-surface-variant mb-1">
            {{ card.basicInfo.headline }}
          </p>
          <p v-if="card.basicInfo?.company" class="text-xs text-md-on-surface-variant mb-3">
            {{ card.basicInfo.company }}
          </p>

          <!-- Card URL -->
          <div class="flex items-center gap-2 p-2 bg-md-surface-container rounded-lg mb-4">
            <i class="fas fa-link text-md-on-surface-variant text-xs"></i>
            <span class="text-xs text-md-on-surface truncate">bizbio.co.za/{{ card.slug }}</span>
          </div>

          <!-- Actions -->
          <div class="flex gap-2">
            <a
              :href="`/${card.slug}`"
              target="_blank"
              class="flex-1 flex items-center justify-center gap-1 py-2 rounded-xl border border-md-outline text-md-on-surface hover:bg-md-surface-container transition-all text-xs font-medium"
            >
              <i class="fas fa-external-link-alt text-xs"></i>
              View
            </a>
            <NuxtLink
              :to="`/dashboard/cards/${card.id}/edit`"
              class="flex-1 flex items-center justify-center gap-1 py-2 rounded-xl btn-gradient text-white shadow-md-1 hover:shadow-md-3 transition-all text-xs font-medium"
            >
              <i class="fas fa-edit text-xs"></i>
              Edit
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
})

useHead({ title: 'My Business Cards' })

const profilesApi = useProfilesApi()
const router = useRouter()

const loading = ref(true)
const cards = ref<any[]>([])

onMounted(async () => {
  try {
    const response = await profilesApi.getMyProfiles() as any
    const profiles = response?.data || response || []

    // Filter to card-type profiles only (type=card or has template field)
    cards.value = profiles.filter((p: any) => p.type === 'card' || p.template)
  } catch (err) {
    console.error('Failed to load cards:', err)
    cards.value = []
  } finally {
    loading.value = false
  }
})
</script>

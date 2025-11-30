<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import profilesApi from '../api/profiles'

const route = useRoute()
const profile = ref(null)
const loading = ref(true)
const error = ref('')

const profileColors = computed(() => {
  if (!profile.value) return { primary: '#A275EA', secondary: '#764ba2' }
  return {
    primary: profile.value.primaryColor || '#A275EA',
    secondary: profile.value.secondaryColor || '#764ba2'
  }
})

onMounted(async () => {
  try {
    const slug = route.params.slug
    const response = await profilesApi.getBySlug(slug)
    profile.value = response.data

    // Track view
    await profilesApi.trackView(slug)
  } catch (err) {
    error.value = err.response?.data?.message || 'Profile not found'
    console.error(err)
  } finally {
    loading.value = false
  }
})

const downloadVCard = () => {
  // TODO: Implement vCard download
  const p = profile.value
  const vcard = `BEGIN:VCARD
VERSION:3.0
FN:${p.fullName}
ORG:${p.companyName || ''}
TITLE:${p.jobTitle || ''}
TEL:${p.phoneNumber || ''}
EMAIL:${p.email || ''}
URL:${p.website || ''}
END:VCARD`

  const blob = new Blob([vcard], { type: 'text/vcard' })
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `${p.firstName}_${p.lastName}.vcf`
  link.click()
}

const openLink = (url) => {
  window.open(url, '_blank')
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-primary text-6xl mb-4"></i>
        <p class="text-brand-gray-text text-lg">Loading profile...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex items-center justify-center min-h-screen px-4">
      <div class="max-w-md w-full">
        <div class="bg-white rounded-lg shadow-lg p-8 text-center">
          <i class="fas fa-user-slash text-gray-300 text-6xl mb-4"></i>
          <h2 class="text-2xl font-heading font-bold text-brand-dark-text mb-2">
            Profile Not Found
          </h2>
          <p class="text-brand-gray-text mb-6">{{ error }}</p>
          <router-link
            to="/"
            class="inline-flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold"
          >
            <i class="fas fa-home"></i>
            Back to Home
          </router-link>
        </div>
      </div>
    </div>

    <!-- Profile Display -->
    <div v-else class="py-12 px-4">
      <div class="max-w-4xl mx-auto">
        <!-- Profile Card -->
        <div
          class="bg-white rounded-2xl shadow-2xl overflow-hidden"
          :style="{ borderTop: `8px solid ${profileColors.primary}` }"
        >
          <!-- Header with gradient -->
          <div
            class="h-32 bg-gradient-to-r relative"
            :style="{ backgroundImage: `linear-gradient(to right, ${profileColors.primary}, ${profileColors.secondary})` }"
          ></div>

          <!-- Profile Content -->
          <div class="px-8 pb-8">
            <!-- Avatar -->
            <div class="relative -mt-16 mb-4">
              <div class="w-32 h-32 rounded-full border-4 border-white shadow-xl bg-white flex items-center justify-center overflow-hidden">
                <img v-if="profile.logoUrl" :src="profile.logoUrl" alt="Profile" class="w-full h-full object-cover" />
                <i v-else class="fas fa-user text-gray-300 text-5xl"></i>
              </div>
            </div>

            <!-- Name and Title -->
            <div class="mb-6">
              <h1 class="text-3xl font-heading font-bold text-brand-dark-text mb-2">
                {{ profile.fullName }}
              </h1>
              <p v-if="profile.jobTitle" class="text-xl text-brand-gray-text mb-1">
                {{ profile.jobTitle }}
              </p>
              <p v-if="profile.companyName" class="text-lg text-brand-gray-text">
                {{ profile.companyName }}
              </p>
            </div>

            <!-- Bio -->
            <p v-if="profile.bio" class="text-brand-gray-text mb-6 leading-relaxed">
              {{ profile.bio }}
            </p>

            <!-- Contact Information -->
            <div class="grid sm:grid-cols-2 gap-4 mb-6">
              <a
                v-if="profile.phoneNumber"
                :href="`tel:${profile.phoneNumber}`"
                class="flex items-center gap-3 p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-all"
              >
                <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                  <i class="fas fa-phone text-primary"></i>
                </div>
                <div>
                  <p class="text-xs text-brand-gray-text">Phone</p>
                  <p class="text-brand-dark-text font-semibold">{{ profile.phoneNumber }}</p>
                </div>
              </a>

              <a
                v-if="profile.email"
                :href="`mailto:${profile.email}`"
                class="flex items-center gap-3 p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-all"
              >
                <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                  <i class="fas fa-envelope text-primary"></i>
                </div>
                <div>
                  <p class="text-xs text-brand-gray-text">Email</p>
                  <p class="text-brand-dark-text font-semibold">{{ profile.email }}</p>
                </div>
              </a>

              <a
                v-if="profile.website"
                :href="profile.website"
                target="_blank"
                class="flex items-center gap-3 p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-all"
              >
                <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                  <i class="fas fa-globe text-primary"></i>
                </div>
                <div>
                  <p class="text-xs text-brand-gray-text">Website</p>
                  <p class="text-brand-dark-text font-semibold">{{ profile.website }}</p>
                </div>
              </a>

              <a
                v-if="profile.address"
                :href="`https://maps.google.com/?q=${encodeURIComponent(profile.address)}`"
                target="_blank"
                class="flex items-center gap-3 p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-all"
              >
                <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                  <i class="fas fa-map-marker-alt text-primary"></i>
                </div>
                <div>
                  <p class="text-xs text-brand-gray-text">Address</p>
                  <p class="text-brand-dark-text font-semibold">{{ profile.address }}</p>
                </div>
              </a>
            </div>

            <!-- Social Links -->
            <div v-if="profile.socialLinks && profile.socialLinks.length > 0" class="mb-6">
              <h3 class="text-lg font-heading font-bold text-brand-dark-text mb-3">Connect</h3>
              <div class="flex flex-wrap gap-3">
                <a
                  v-for="(link, index) in profile.socialLinks"
                  :key="index"
                  :href="link.url"
                  target="_blank"
                  class="w-12 h-12 rounded-full flex items-center justify-center transition-all hover:scale-110"
                  :style="{ backgroundColor: profileColors.primary }"
                >
                  <i :class="`fab fa-${link.platform} text-white text-xl`"></i>
                </a>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex flex-col sm:flex-row gap-4">
              <button
                @click="downloadVCard"
                class="flex-1 flex items-center justify-center gap-2 py-3 px-6 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold"
              >
                <i class="fas fa-download"></i>
                Save Contact
              </button>
              <button
                class="flex-1 flex items-center justify-center gap-2 py-3 px-6 bg-white text-primary border-2 border-primary rounded-lg hover:bg-primary/5 transition-all font-semibold"
                @click="() => {}"
              >
                <i class="fas fa-share-alt"></i>
                Share Profile
              </button>
            </div>
          </div>
        </div>

        <!-- Powered by -->
        <div class="text-center mt-8">
          <p class="text-brand-gray-text text-sm">
            Powered by
            <router-link to="/" class="text-primary hover:text-primary-600 font-semibold">
              BizBio
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

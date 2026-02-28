<template>
  <div class="space-y-4">
    <!-- Photo upload -->
    <div class="flex items-start gap-4">
      <div class="relative flex-shrink-0">
        <div
          class="w-20 h-20 rounded-full bg-md-surface-container border-2 border-dashed border-md-outline-variant flex items-center justify-center overflow-hidden cursor-pointer hover:border-md-secondary transition-colors"
          @click="triggerPhotoUpload"
        >
          <img v-if="state.photoUrl" :src="state.photoUrl" alt="Profile" class="w-full h-full object-cover" />
          <i v-else class="fas fa-camera text-md-on-surface-variant text-xl"></i>
        </div>
        <input
          ref="photoInput"
          type="file"
          accept="image/*"
          class="hidden"
          @change="handlePhotoUpload"
        />
        <button
          v-if="state.photoUrl"
          class="absolute -top-1 -right-1 w-5 h-5 bg-md-error text-white rounded-full flex items-center justify-center text-xs hover:opacity-90"
          @click.stop="state.photoUrl = ''"
        >
          <i class="fas fa-times"></i>
        </button>
      </div>
      <div class="flex-1 space-y-1">
        <p class="text-sm font-medium text-md-on-surface">Profile Photo</p>
        <p class="text-xs text-md-on-surface-variant">Click to upload or paste an image URL</p>
        <input
          v-model="state.photoUrl"
          type="url"
          placeholder="https://example.com/photo.jpg"
          class="w-full px-3 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface"
        />
      </div>
    </div>

    <!-- Name fields -->
    <div class="grid grid-cols-2 gap-3">
      <div>
        <label class="block text-xs font-medium text-md-on-surface-variant mb-1">
          First Name <span class="text-md-error">*</span>
        </label>
        <input
          v-model="state.firstName"
          type="text"
          placeholder="Jane"
          class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm"
        />
      </div>
      <div>
        <label class="block text-xs font-medium text-md-on-surface-variant mb-1">Last Name</label>
        <input
          v-model="state.lastName"
          type="text"
          placeholder="Doe"
          class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm"
        />
      </div>
    </div>

    <!-- Headline -->
    <div>
      <label class="block text-xs font-medium text-md-on-surface-variant mb-1">Headline / Job Title</label>
      <input
        v-model="state.headline"
        type="text"
        placeholder="Senior Software Engineer"
        class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm"
      />
    </div>

    <!-- Company -->
    <div>
      <label class="block text-xs font-medium text-md-on-surface-variant mb-1">Company / Organisation</label>
      <input
        v-model="state.company"
        type="text"
        placeholder="Acme Corp"
        class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm"
      />
    </div>

    <!-- Bio -->
    <div>
      <label class="block text-xs font-medium text-md-on-surface-variant mb-1">
        Bio / About
        <span class="text-md-on-surface-variant font-normal ml-1">({{ state.bio.length }}/300)</span>
      </label>
      <textarea
        v-model="state.bio"
        placeholder="Tell people a bit about yourself..."
        rows="3"
        maxlength="300"
        class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm resize-none"
      ></textarea>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
}>()

const photoInput = ref<HTMLInputElement>()
const uploadsApi = useUploadsApi()

const triggerPhotoUpload = () => {
  photoInput.value?.click()
}

const handlePhotoUpload = async (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file) return

  try {
    const result = await uploadsApi.uploadFile(file)
    if (result?.url) {
      props.state.photoUrl = result.url
    }
  } catch {
    // Fallback to object URL for preview
    props.state.photoUrl = URL.createObjectURL(file)
  }
}
</script>

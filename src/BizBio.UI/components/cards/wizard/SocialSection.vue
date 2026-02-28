<template>
  <div class="space-y-3">
    <div ref="listEl" class="space-y-2">
      <div
        v-for="link in state.socialLinks"
        :key="link.id"
        class="flex items-center gap-2 p-3 rounded-lg border border-md-outline-variant bg-md-surface group"
        :data-id="link.id"
      >
        <span class="drag-handle cursor-grab text-md-on-surface-variant hover:text-md-on-surface p-1 flex-shrink-0">
          <i class="fas fa-grip-vertical text-xs"></i>
        </span>

        <!-- Platform icon -->
        <div class="w-7 h-7 rounded-lg bg-md-surface-container flex items-center justify-center flex-shrink-0">
          <i :class="getPlatformIcon(link.platform)" class="text-md-primary text-xs"></i>
        </div>

        <!-- Platform selector -->
        <select
          v-model="link.platform"
          class="text-xs px-2 py-1.5 rounded-lg border border-md-outline-variant bg-md-surface text-md-on-surface focus:outline-none focus:border-md-secondary flex-shrink-0"
        >
          <option v-for="p in platforms" :key="p.value" :value="p.value">{{ p.label }}</option>
        </select>

        <!-- URL input -->
        <input
          v-model="link.url"
          type="url"
          :placeholder="getPlatformPlaceholder(link.platform)"
          class="flex-1 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface min-w-0"
        />

        <button
          class="text-md-error opacity-0 group-hover:opacity-100 transition-opacity p-1 hover:bg-md-error-container rounded flex-shrink-0"
          @click="removeSocialLink(link.id)"
        >
          <i class="fas fa-trash-alt text-xs"></i>
        </button>
      </div>
    </div>

    <button
      class="w-full py-2 rounded-lg border border-dashed transition-all text-sm font-medium flex items-center justify-center gap-2"
      :class="canAdd
        ? 'border-md-secondary text-md-secondary hover:bg-md-secondary-container cursor-pointer'
        : 'border-md-outline-variant text-md-on-surface-variant cursor-not-allowed opacity-60'"
      :disabled="!canAdd"
      @click="canAdd && addSocialLink()"
    >
      <i class="fas fa-plus text-xs"></i>
      {{ canAdd ? 'Add Social Link' : `Max ${maxSocialLinks} links on ${state.selectedPlan} plan` }}
    </button>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
  maxSocialLinks: number
}>()

const { addSocialLink, removeSocialLink } = useBusinessCardCreation()

const canAdd = computed(() => props.state.socialLinks.length < props.maxSocialLinks)

const platforms = [
  { value: 'linkedin', label: 'LinkedIn' },
  { value: 'facebook', label: 'Facebook' },
  { value: 'instagram', label: 'Instagram' },
  { value: 'twitter', label: 'Twitter/X' },
  { value: 'tiktok', label: 'TikTok' },
  { value: 'youtube', label: 'YouTube' },
  { value: 'pinterest', label: 'Pinterest' },
  { value: 'snapchat', label: 'Snapchat' },
  { value: 'github', label: 'GitHub' },
  { value: 'dribbble', label: 'Dribbble' },
  { value: 'behance', label: 'Behance' },
  { value: 'whatsapp', label: 'WhatsApp' },
  { value: 'telegram', label: 'Telegram' },
  { value: 'discord', label: 'Discord' },
  { value: 'spotify', label: 'Spotify' },
  { value: 'soundcloud', label: 'SoundCloud' },
  { value: 'twitch', label: 'Twitch' },
]

const platformIcons: Record<string, string> = {
  linkedin: 'fab fa-linkedin',
  facebook: 'fab fa-facebook',
  instagram: 'fab fa-instagram',
  twitter: 'fab fa-x-twitter',
  tiktok: 'fab fa-tiktok',
  youtube: 'fab fa-youtube',
  pinterest: 'fab fa-pinterest',
  snapchat: 'fab fa-snapchat',
  github: 'fab fa-github',
  dribbble: 'fab fa-dribbble',
  behance: 'fab fa-behance',
  whatsapp: 'fab fa-whatsapp',
  telegram: 'fab fa-telegram',
  discord: 'fab fa-discord',
  spotify: 'fab fa-spotify',
  soundcloud: 'fab fa-soundcloud',
  twitch: 'fab fa-twitch',
}

const getPlatformIcon = (platform: string) => platformIcons[platform] ?? 'fas fa-link'

const getPlatformPlaceholder = (platform: string): string => {
  const map: Record<string, string> = {
    linkedin: 'https://linkedin.com/in/username',
    facebook: 'https://facebook.com/username',
    instagram: 'https://instagram.com/username',
    twitter: 'https://twitter.com/username',
    tiktok: 'https://tiktok.com/@username',
    youtube: 'https://youtube.com/@channel',
    github: 'https://github.com/username',
    whatsapp: 'https://wa.me/27820000000',
    telegram: 'https://t.me/username',
    discord: 'https://discord.gg/invite',
  }
  return map[platform] ?? 'https://...'
}

// Drag and drop
const listEl = ref<HTMLElement>()
const { enableSortable } = useDragDrop()

onMounted(() => {
  if (listEl.value) {
    enableSortable(listEl.value, {
      onEnd: (evt) => {
        const links = [...props.state.socialLinks]
        const [moved] = links.splice(evt.oldIndex!, 1)
        links.splice(evt.newIndex!, 0, moved)
        props.state.socialLinks = links
      }
    })
  }
})
</script>

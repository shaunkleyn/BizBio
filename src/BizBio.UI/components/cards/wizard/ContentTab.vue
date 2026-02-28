<template>
  <div class="p-4 space-y-4">
    <!-- Fixed sections -->
    <GenericSection title="Basic Info" icon="fas fa-user" :removable="false">
      <BasicInfoSection :state="state" />
    </GenericSection>

    <GenericSection title="Contact Buttons" icon="fas fa-hand-pointer" :removable="false">
      <ContactButtonsSection :state="state" :max-contact-buttons="maxContactButtons" />
    </GenericSection>

    <GenericSection title="Contact Info" icon="fas fa-address-card" :removable="false">
      <ContactInfoSection :state="state" :max-contact-info="maxContactInfo" />
    </GenericSection>

    <GenericSection title="Social Links" icon="fas fa-share-alt" :removable="false">
      <SocialSection :state="state" :max-social-links="maxSocialLinks" />
    </GenericSection>

    <!-- Optional sections (sortable) -->
    <div ref="sectionsEl">
      <GenericSection
        v-for="section in sortedSections"
        :key="section.id"
        :title="getSectionDef(section.type)?.label ?? section.type"
        :icon="getSectionDef(section.type)?.icon ?? 'fas fa-puzzle-piece'"
        :removable="true"
        class="mb-3"
        @remove="removeSection(section.id)"
      >
        <!-- Section header input (shown for all optional sections) -->
        <div class="mb-3 pb-3 border-b border-md-outline-variant">
          <label class="text-xs text-md-on-surface-variant block mb-1">Section header <span class="opacity-60">(optional)</span></label>
          <input
            v-model="section.data.header"
            type="text"
            placeholder="e.g. Connect With Me, My Skills…"
            class="w-full px-3 py-1.5 text-sm rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface"
          />
        </div>

        <!-- Toggle sections -->
        <div v-if="section.type === 'save-contact' || section.type === 'share'">
          <label class="flex items-center gap-3 cursor-pointer">
            <div
              class="relative w-10 h-6 rounded-full transition-all"
              :class="section.data.enabled ? 'bg-md-secondary' : 'bg-md-outline'"
              @click="section.data.enabled = !section.data.enabled"
            >
              <div
                class="absolute top-1 w-4 h-4 bg-white rounded-full shadow transition-all"
                :class="section.data.enabled ? 'left-5' : 'left-1'"
              ></div>
            </div>
            <span class="text-sm text-md-on-surface">{{ section.data.enabled ? 'Enabled' : 'Disabled' }}</span>
          </label>
        </div>

        <!-- Bio section -->
        <div v-else-if="section.type === 'bio'">
          <textarea
            v-model="section.data.text"
            placeholder="Your extended bio..."
            rows="4"
            class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm resize-none"
          ></textarea>
        </div>

        <!-- Map section -->
        <div v-else-if="section.type === 'map'">
          <input
            v-model="section.data.address"
            type="text"
            placeholder="123 Main St, Cape Town, South Africa"
            class="w-full px-3 py-2 rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface text-sm"
          />
        </div>

        <!-- Skills section -->
        <div v-else-if="section.type === 'skills'">
          <div class="flex flex-wrap gap-2 mb-2">
            <span
              v-for="(skill, i) in (section.data.skills ?? [])"
              :key="i"
              class="inline-flex items-center gap-1 px-2 py-1 bg-md-secondary-container text-md-secondary rounded-full text-xs"
            >
              {{ skill }}
              <button @click="removeSkill(section, i)"><i class="fas fa-times text-[9px]"></i></button>
            </span>
          </div>
          <div class="flex gap-2">
            <input
              v-model="newSkill"
              type="text"
              placeholder="Add a skill..."
              class="flex-1 px-3 py-1.5 text-sm rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface"
              @keydown.enter.prevent="addSkill(section)"
            />
            <button
              class="px-3 py-1.5 bg-md-secondary text-white rounded-lg text-sm"
              @click="addSkill(section)"
            >Add</button>
          </div>
        </div>

        <!-- Links section -->
        <div v-else-if="section.type === 'links'">
          <div class="space-y-2 mb-2">
            <div
              v-for="(link, i) in (section.data.links ?? [])"
              :key="i"
              class="flex items-center gap-2"
            >
              <input v-model="link.title" type="text" placeholder="Title" class="w-1/3 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface" />
              <input v-model="link.url" type="url" placeholder="https://..." class="flex-1 px-2 py-1.5 text-xs rounded-lg border border-md-outline-variant bg-md-surface focus:outline-none focus:border-md-secondary text-md-on-surface" />
              <button class="text-md-error p-1" @click="removeLink(section, i)"><i class="fas fa-trash text-xs"></i></button>
            </div>
          </div>
          <button
            class="text-sm text-md-secondary hover:underline flex items-center gap-1"
            @click="addLink(section)"
          >
            <i class="fas fa-plus text-xs"></i> Add Link
          </button>
        </div>

        <!-- Wallet sections -->
        <div v-else-if="section.type === 'google-wallet' || section.type === 'apple-wallet'">
          <p class="text-xs text-md-on-surface-variant">Configure branding in the Appearance tab under "Wallet Branding".</p>
        </div>

        <!-- Gallery section -->
        <div v-else-if="section.type === 'gallery'">
          <p class="text-xs text-md-on-surface-variant">Gallery image upload coming soon.</p>
        </div>
      </GenericSection>
    </div>

    <!-- Add section button -->
    <div class="relative">
      <button
        class="w-full py-3 rounded-xl border-2 border-dashed border-md-secondary text-md-secondary hover:bg-md-secondary-container transition-all font-medium flex items-center justify-center gap-2"
        @click="showSectionMenu = !showSectionMenu"
      >
        <i class="fas fa-plus"></i>
        Add Section
      </button>

      <!-- Section picker dropdown -->
      <div
        v-if="showSectionMenu"
        class="absolute bottom-full mb-2 left-0 right-0 bg-md-surface rounded-xl shadow-md-4 border border-md-outline-variant overflow-hidden z-10"
      >
        <div
          v-for="def in OPTIONAL_SECTION_DEFS"
          :key="def.type"
          class="flex items-center gap-3 px-4 py-3 hover:bg-md-surface-container transition-colors cursor-pointer"
          :class="[
            isSectionAdded(def.type) ? 'opacity-40 cursor-not-allowed' : '',
            !canAddSectionCheck(def.type).allowed ? 'opacity-60' : ''
          ]"
          @click="handleAddSection(def.type as any)"
        >
          <i :class="def.icon" class="text-md-secondary text-sm w-5 text-center"></i>
          <span class="flex-1 text-sm text-md-on-surface">{{ def.label }}</span>
          <span v-if="isSectionAdded(def.type)" class="text-xs text-md-on-surface-variant">Added</span>
          <span v-else-if="!canAddSectionCheck(def.type).allowed" class="text-xs text-md-error">
            {{ canAddSectionCheck(def.type).reason }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { OPTIONAL_SECTION_DEFS } from '~/composables/useBusinessCardCreation'
import type { WizardState, CardSection } from '~/composables/useBusinessCardCreation'
import GenericSection from './GenericSection.vue'
import BasicInfoSection from './BasicInfoSection.vue'
import ContactButtonsSection from './ContactButtonsSection.vue'
import ContactInfoSection from './ContactInfoSection.vue'
import SocialSection from './SocialSection.vue'

const props = defineProps<{
  state: WizardState
  maxContactButtons: number
  maxContactInfo: number
  maxSocialLinks: number
}>()

const {
  addSection,
  removeSection,
  canAddSection,
} = useBusinessCardCreation()

const canAddSectionCheck = (type: string) => canAddSection(type)

const showSectionMenu = ref(false)
const newSkill = ref('')

const sortedSections = computed(() =>
  [...props.state.sections].sort((a, b) => a.sortOrder - b.sortOrder)
)

const getSectionDef = (type: string) =>
  OPTIONAL_SECTION_DEFS.find(d => d.type === type)

const isSectionAdded = (type: string) =>
  props.state.sections.some(s => s.type === type)

const handleAddSection = (type: CardSection['type']) => {
  if (isSectionAdded(type)) return
  const check = canAddSection(type)
  if (!check.allowed) return
  addSection(type)
  showSectionMenu.value = false
}

const addSkill = (section: CardSection) => {
  if (!newSkill.value.trim()) return
  if (!section.data.skills) section.data.skills = []
  section.data.skills.push(newSkill.value.trim())
  newSkill.value = ''
}

const removeSkill = (section: CardSection, index: number) => {
  section.data.skills?.splice(index, 1)
}

const addLink = (section: CardSection) => {
  if (!section.data.links) section.data.links = []
  section.data.links.push({ title: '', url: '' })
}

const removeLink = (section: CardSection, index: number) => {
  section.data.links?.splice(index, 1)
}

// Close dropdown when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    if (!(e.target as HTMLElement).closest('.relative')) {
      showSectionMenu.value = false
    }
  })
})
</script>

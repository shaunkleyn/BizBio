<template>
  <div class="glass-effect border-b border-md-outline-variant sticky top-16 z-40 py-4 px-4 md:px-8 shadow-md-1">
    <!-- Page Header and Actions Section -->
    <div class="flex items-center justify-between flex-wrap gap-4">
      <!-- Page Title and Description (Left) -->
      <div class="flex-1 min-w-0">
        <!-- Editable Title -->
        <div v-if="isEditing" class="flex items-center gap-2">
          <input
            ref="editInput"
            v-model="editValue"
            type="text"
            class="text-2xl md:text-3xl font-heading font-bold text-md-on-surface bg-md-surface border-2 border-primary rounded-lg px-3 py-1 focus:outline-none focus:ring-2 focus:ring-primary"
            @keyup.enter="handleSave"
            @keyup.escape="handleCancel"
          />
          <button
            @click="handleSave"
            :disabled="isSaving || !editValue.trim()"
            class="p-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            title="Save"
          >
            <i v-if="isSaving" class="fas fa-spinner fa-spin"></i>
            <i v-else class="fas fa-check"></i>
          </button>
          <button
            @click="handleCancel"
            :disabled="isSaving"
            class="p-2 bg-md-surface-container text-md-on-surface border border-md-outline rounded-lg hover:bg-md-surface-container-high transition-colors disabled:opacity-50"
            title="Cancel"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Non-Editable Title -->
        <h1
          v-else
          :class="[
            'text-2xl md:text-3xl font-heading font-bold text-md-on-surface',
            pageHeader.editable ? 'cursor-pointer hover:text-primary transition-colors flex items-center gap-2 group' : ''
          ]"
          @click="handleStartEdit"
        >
          {{ pageHeader.title }}
          <i v-if="pageHeader.editable" class="fas fa-edit text-lg opacity-0 group-hover:opacity-100 transition-opacity"></i>
        </h1>

        <p class="text-sm md:text-base text-md-on-surface-variant mt-1 font-medium">{{ pageHeader.description }}</p>
      </div>

      <!-- Action Buttons (Right) -->
      <div class="flex items-center gap-2 flex-shrink-0">
        <!-- Custom actions from page header -->
        <template v-if="pageHeader.actions">
          <component
            v-for="(action, index) in pageHeader.actions"
            :key="index"
            :is="action"
          />
        </template>

        <!-- Page action button (new simple approach) -->
        <NuxtLink
          v-if="pageActionButton && pageActionButton.to"
          :to="pageActionButton.to"
          :class="pageActionButton.class || 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'"
        >
          <i v-if="pageActionButton.icon" :class="pageActionButton.icon + ' mr-2'"></i>
          {{ pageActionButton.label }}
        </NuxtLink>
        <button
          v-else-if="pageActionButton && pageActionButton.onClick"
          @click="pageActionButton.onClick"
          :class="pageActionButton.class || 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'"
        >
          <i v-if="pageActionButton.icon" :class="pageActionButton.icon + ' mr-2'"></i>
          {{ pageActionButton.label }}
        </button>

        <!-- Page actions (legacy render functions) -->
        <component v-if="pageActions" :is="pageActions()" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, watch } from 'vue'

// Use page metadata composable
const { pageHeader, pageActions, pageActionButton } = usePageMeta()

// Inline editing state
const isEditing = ref(false)
const editValue = ref('')
const isSaving = ref(false)
const editInput = ref<HTMLInputElement | null>(null)

// Watch for page header changes and reset edit state
watch(() => pageHeader.value.title, (newTitle, oldTitle) => {
  // If title changes while editing (e.g., navigation), cancel edit mode
  if (isEditing.value && newTitle !== editValue.value) {
    isEditing.value = false
    editValue.value = ''
    isSaving.value = false
  }
})

// Watch for editable flag changes
watch(() => pageHeader.value.editable, (isEditable) => {
  // If page becomes non-editable, cancel any active editing
  if (!isEditable && isEditing.value) {
    isEditing.value = false
    editValue.value = ''
    isSaving.value = false
  }
})

// Handle starting edit mode
const handleStartEdit = () => {
  if (!pageHeader.value.editable) return

  editValue.value = pageHeader.value.title
  isEditing.value = true

  // Focus the input after it's rendered
  nextTick(() => {
    editInput.value?.focus()
    editInput.value?.select()
  })
}

// Handle saving the new name
const handleSave = async () => {
  if (!editValue.value.trim() || !pageHeader.value.onRename) return

  try {
    isSaving.value = true
    await pageHeader.value.onRename(editValue.value.trim())
    isEditing.value = false
  } catch (error) {
    console.error('Failed to rename:', error)
    // Keep editing mode open on error so user can try again
  } finally {
    isSaving.value = false
  }
}

// Handle canceling the edit
const handleCancel = () => {
  isEditing.value = false
  editValue.value = ''
}
</script>

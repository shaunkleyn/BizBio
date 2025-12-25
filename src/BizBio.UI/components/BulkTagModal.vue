<template>
  <div
    class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
    @click="emit('close')"
  >
    <div
      class="mesh-card bg-md-surface rounded-2xl w-full max-w-md shadow-md-5"
      @click.stop
    >
      <!-- Header -->
      <div class="p-6 border-b border-md-outline-variant relative overflow-hidden">
        <div class="absolute inset-0 mesh-bg-2 opacity-10"></div>
        <h2 class="text-2xl font-bold text-md-on-surface relative z-10">Add Tags to Selected Items</h2>
        <p class="text-sm text-md-on-surface-variant mt-1 relative z-10">Select tags to add to all selected items</p>
      </div>

      <!-- Content -->
      <div class="p-6 space-y-4">
        <!-- Common Tags -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Quick Select</label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="tag in commonTags"
              :key="tag"
              type="button"
              @click="toggleTag(tag)"
              :class="[
                'px-3 py-1.5 rounded-full text-sm font-medium transition-all',
                selectedTags.includes(tag)
                  ? 'bg-green-500 text-white shadow-md'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              <i v-if="selectedTags.includes(tag)" class="fas fa-check mr-1"></i>
              {{ tag }}
            </button>
          </div>
        </div>

        <!-- Custom Tag Input -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Custom Tags (press Enter or comma)
          </label>
          <input
            v-model="customTagInput"
            type="text"
            placeholder="Type tag and press Enter or comma..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
            @keydown.enter.prevent="addCustomTag"
            @input="handleTagInput"
          />
        </div>

        <!-- Selected Tags -->
        <div v-if="selectedTags.length > 0" class="space-y-2">
          <label class="block text-sm font-medium text-gray-700">Tags to Add</label>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="tag in selectedTags"
              :key="tag"
              class="inline-flex items-center gap-2 px-3 py-1.5 bg-blue-100 text-blue-800 rounded-full text-sm font-medium"
            >
              {{ tag }}
              <button
                type="button"
                @click="removeTag(tag)"
                class="hover:text-blue-900"
              >
                <i class="fas fa-times text-xs"></i>
              </button>
            </span>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="p-6 border-t border-md-outline-variant flex gap-3">
        <button
          @click="emit('close')"
          class="flex-1 px-4 py-2 bg-md-surface-container border border-md-outline-variant text-md-on-surface-variant rounded-xl hover:bg-md-surface-container-high transition-colors font-medium"
        >
          Cancel
        </button>
        <button
          @click="applyTags"
          :disabled="selectedTags.length === 0"
          class="flex-1 px-4 py-2 btn-gradient rounded-xl hover:shadow-glow-purple transition-all font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Add {{ selectedTags.length }} Tag{{ selectedTags.length > 1 ? 's' : '' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const emit = defineEmits<{
  close: []
  apply: [tags: string[]]
}>()

const customTagInput = ref('')
const selectedTags = ref<string[]>([])

const commonTags = [
  'Gluten-Free',
  'Vegan',
  'Vegetarian',
  'Dairy-Free',
  'Nut-Free',
  'Halal',
  'Kosher',
  'Spicy',
  'Organic',
  'Sugar-Free'
]

function toggleTag(tag: string) {
  const index = selectedTags.value.indexOf(tag)
  if (index > -1) {
    selectedTags.value.splice(index, 1)
  } else {
    selectedTags.value.push(tag)
  }
}

function handleTagInput(event: Event) {
  const input = event.target as HTMLInputElement
  const value = input.value

  if (value.includes(',')) {
    const tags = value.split(',').map(t => t.trim()).filter(t => t)
    tags.forEach(tag => {
      if (tag && !selectedTags.value.includes(tag)) {
        selectedTags.value.push(tag)
      }
    })
    customTagInput.value = ''
  }
}

function addCustomTag() {
  const tag = customTagInput.value.trim()
  if (tag && !selectedTags.value.includes(tag)) {
    selectedTags.value.push(tag)
    customTagInput.value = ''
  }
}

function removeTag(tag: string) {
  const index = selectedTags.value.indexOf(tag)
  if (index > -1) {
    selectedTags.value.splice(index, 1)
  }
}

function applyTags() {
  emit('apply', selectedTags.value)
  emit('close')
}
</script>

<template>
  <div 
    class=" border border-md-outline-variant rounded-xl p-6"
    :class="[
      isExpanded ? 'border-[#1E8E3E]' : 'border-gray-200',
      'hover:border-[#1E8E3E]'
    ]"
    role="step"
  >
    <!-- Step Header -->
    <div class="flex items-start justify-between mb-4">
      <div class="flex items-center gap-3 flex-1">
        <!-- Drag Handle -->
                    <div
                      class="drag-handle cursor-move p-2 hover:bg-md-surface-container-high rounded transition-colors">
                      <i class="fas fa-grip-vertical text-md-on-surface-variant"></i>
                    </div>
        <div 
          class="w-10 h-10 rounded-full flex items-center justify-center font-bold text-blue-600"
          role="step-number"
          :class="isExpanded ? 'bg-[#1E8E3E]' : 'bg-blue-100'"
        >
          {{ stepNumber }}
        </div>
        <div class="flex-1">
          <input 
            type="text" 
            :placeholder="`Enter a title for step ${stepNumber}`"
            v-model="localName"
            @blur="$emit('update:name', localName)"
            :class="`w-full px-4 py-2 bg-md-surface border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary ${localName !== '' ? 'font-semibold' : ''}`"
          />
          <p class="text-sm text-gray-600 mt-1">
            {{ description }}
          </p>
        </div>
      </div>
      
      <div class="flex gap-2">
        <button 
          @click="$emit('move')"
          class="w-8 h-8 rounded-lg hover:bg-[#F5F5F5] text-gray-600 flex items-center justify-center"
        >
          <i class="fas fa-arrows-alt text-sm"></i>
        </button>
        <button 
          @click="$emit('delete')"
          class="w-8 h-8 rounded-lg hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center"
        >
          <i class="fas fa-trash text-sm"></i>
        </button>
      </div>
    </div>

    <!-- Step Content (slot for AllowedProducts and OptionGroups) -->
    <slot />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  stepNumber: number
  name: string
  description: string
  isExpanded?: boolean
}>()

const emit = defineEmits(['update:name', 'delete', 'move'])

const localName = ref(props.name)

watch(() => props.name, (newName) => {
  localName.value = newName
})
</script>

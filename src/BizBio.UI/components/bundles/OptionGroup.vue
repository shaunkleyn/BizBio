<template>
  <div class="bg-md-surface border border-md-outline-variant rounded-xl p-4" role="option">
    <div class="flex items-start justify-between mb-3">
      <div class="flex-1">
        <div class="flex flex-col sm:flex-row items-start sm:items-center gap-2 mb-2">
          <input 
            type="text" 
            v-model="localName"
            @blur="$emit('update:name', localName)"
            placeholder="Option Group Name"
            class="flex-1 px-4 py-2 bg-md-surface border text-sm border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary font-semibold"
          />
          
          <!-- Required/Optional Toggle -->
          <div class="inline-flex rounded-lg border border-md-outline-variant overflow-hidden">
            <button
              type="button"
              @click="toggleRequired(true)"
              :class="[
                'px-3 py-1.5 text-xs font-bold transition-colors',
                optionGroup.required
                  ? 'bg-[#C5221F] text-white'
                  : 'bg-md-surface text-md-on-surface-variant hover:bg-md-surface-container'
              ]"
            >
              REQUIRED
            </button>
            <button
              type="button"
              @click="toggleRequired(false)"
              :class="[
                'px-3 py-1.5 text-xs font-bold transition-colors',
                !optionGroup.required
                  ? 'bg-[#1E8E3E] text-white'
                  : 'bg-md-surface text-md-on-surface-variant hover:bg-md-surface-container'
              ]"
            >
              OPTIONAL
            </button>
          </div>
        </div>
        
        <!-- Min/Max Selection Instructions -->
        <div class="flex items-center flex-wrap gap-1 text-xs text-md-on-surface-variant px-1">
          <span>Customer {{ optionGroup.required ? 'must' : 'may' }} choose</span>
          
          <select
            :value="selectionMode"
            @change="updateSelectionMode($event)"
            class="px-2 py-0.5 border border-md-outline-variant rounded bg-md-surface text-md-on-surface font-semibold focus:ring-2 focus:ring-md-primary focus:outline-none text-xs"
          >
            <option value="exact">exactly</option>
            <option value="at-least">at least</option>
            <option value="up-to">up to</option>
            <option value="between">between</option>
          </select>
          
          <!-- First input (or only input for exact/at-least/up-to) -->
          <input
            v-if="selectionMode !== 'up-to'"
            type="number"
            :value="optionGroup.minSelections"
            @input="updateMin($event)"
            :min="optionGroup.required ? 1 : 0"
            :max="selectionMode === 'between' ? optionGroup.maxSelections : undefined"
            class="w-12 px-2 py-0.5 text-center border border-md-outline-variant rounded bg-md-surface text-md-on-surface font-semibold focus:ring-2 focus:ring-md-primary focus:outline-none"
          />
          
          <!-- "and" text for between mode -->
          <span v-if="selectionMode === 'between'">and</span>
          
          <!-- Second input (for up-to and between modes) -->
          <input
            v-if="selectionMode === 'up-to' || selectionMode === 'between'"
            type="number"
            :value="optionGroup.maxSelections"
            @input="updateMax($event)"
            :min="selectionMode === 'between' ? optionGroup.minSelections : 1"
            class="w-12 px-2 py-0.5 text-center border border-md-outline-variant rounded bg-md-surface text-md-on-surface font-semibold focus:ring-2 focus:ring-md-primary focus:outline-none"
          />
          
          <span>{{ getOptionText() }}</span>
        </div>
      </div>
      <button
        @click="$emit('delete')"
        class="w-6 h-6 ml-2 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center flex-shrink-0"
      >
        <i class="fas fa-trash text-xs"></i>
      </button>
    </div>
    
    <div class="space-y-1">
      <OptionItem
        v-for="option in optionGroup.options"
        :key="option.id"
        :option="option"
        @remove="$emit('remove-option', option.id)"
      />
      
      <button 
        @click="$emit('add-option')"
        class="w-full py-2 text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold"
      >
        + Add Option
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import OptionItem from './OptionItem.vue'

interface Option {
  id: number
  name: string
  price: number
}

interface OptionGroupType {
  id: number
  name: string
  minSelections: number
  maxSelections: number
  required: boolean
  options: Option[]
}

const props = defineProps<{
  optionGroup: OptionGroupType
}>()

const emit = defineEmits(['update:name', 'update:min', 'update:max', 'update:required', 'add-option', 'remove-option', 'delete'])

const localName = ref(props.optionGroup.name)

watch(() => props.optionGroup.name, (newName) => {
  localName.value = newName
})

// Determine selection mode based on min/max values
const selectionMode = computed(() => {
  const min = props.optionGroup.minSelections
  const max = props.optionGroup.maxSelections
  
  if (min === max) return 'exact'
  if (min === 0) return 'up-to'
  if (max === 999 || max > 100) return 'at-least' // Assume large max means "at least"
  return 'between'
})

const updateSelectionMode = (event: Event) => {
  const mode = (event.target as HTMLSelectElement).value
  const currentMin = props.optionGroup.minSelections
  const currentMax = props.optionGroup.maxSelections
  
  switch (mode) {
    case 'exact':
      // Set min = max
      emit('update:max', { id: props.optionGroup.id, max: currentMin })
      break
    case 'at-least':
      // Set max to a large number
      emit('update:max', { id: props.optionGroup.id, max: 999 })
      break
    case 'up-to':
      // Set min to 0
      emit('update:min', { id: props.optionGroup.id, min: 0 })
      if (props.optionGroup.required) {
        emit('update:required', { id: props.optionGroup.id, required: false })
      }
      break
    case 'between':
      // Ensure different values
      if (currentMin === currentMax) {
        emit('update:max', { id: props.optionGroup.id, max: currentMin + 1 })
      }
      if (currentMax > 100) {
        emit('update:max', { id: props.optionGroup.id, max: currentMin + 5 })
      }
      break
  }
}

const getOptionText = () => {
  const mode = selectionMode.value
  const min = props.optionGroup.minSelections
  const max = props.optionGroup.maxSelections
  
  switch (mode) {
    case 'exact':
      return min === 1 ? 'option' : 'options'
    case 'at-least':
      return min === 1 ? 'option' : 'options'
    case 'up-to':
      return max === 1 ? 'option' : 'options'
    case 'between':
      return max === 1 ? 'option' : 'options'
    default:
      return 'options'
  }
}

const toggleRequired = (required: boolean) => {
  emit('update:required', { id: props.optionGroup.id, required })
  // If required, ensure minSelections is at least 1
  if (required && props.optionGroup.minSelections === 0) {
    emit('update:min', { id: props.optionGroup.id, min: 1 })
  }
}

const updateMin = (event: Event) => {
  const value = parseInt((event.target as HTMLInputElement).value)
  if (!isNaN(value) && value >= 0) {
    emit('update:min', { id: props.optionGroup.id, min: value })
    
    // If setting required mode via min > 0
    if (value > 0 && !props.optionGroup.required) {
      emit('update:required', { id: props.optionGroup.id, required: true })
    }
  }
}

const updateMax = (event: Event) => {
  const value = parseInt((event.target as HTMLInputElement).value)
  if (!isNaN(value) && value >= 1) {
    emit('update:max', { id: props.optionGroup.id, max: value })
  }
}

</script>

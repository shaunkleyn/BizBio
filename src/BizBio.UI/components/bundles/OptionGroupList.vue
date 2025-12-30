<template>
  <div 
    class="bg-md-surface  border border-md-outline-variant rounded-2xl p-4"
    role="option-group"
  >
    <div class="flex items-center justify-between {{ optionGroups.length > 0 ? 'mb-3' : '' }}">
      <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
        <i class="fas fa-sliders-h text-[#4A90E2]"></i>
        Option Groups ({{ optionGroups.length }})
      </h4>
      <button 
        @click="$emit('add-option-group')"
        class="text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold"
      >
        <i class="fas fa-plus mr-1"></i>Add Option Group
      </button>
    </div>

    <div class="space-y-3">
      <OptionGroup
        v-for="group in optionGroups"
        :key="group.id"
        :option-group="group"
        @update:name="$emit('update-group-name', { id: group.id, name: $event })"
        @update:min="$emit('update-group-min', $event)"
        @update:max="$emit('update-group-max', $event)"
        @update:required="$emit('update-group-required', $event)"
        @add-option="$emit('add-option', group.id)"
        @remove-option="$emit('remove-option', { groupId: group.id, optionId: $event })"
        @delete="$emit('delete-group', group.id)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import OptionGroup from './OptionGroup.vue'

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

defineProps<{
  optionGroups: OptionGroupType[]
}>()

defineEmits([
  'add-option-group',
  'delete-group',
  'update-group-name',
  'update-group-min',
  'update-group-max',
  'update-group-required',
  'add-option',
  'remove-option'
])
</script>

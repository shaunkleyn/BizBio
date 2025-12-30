<template>
  <div 
    class="bg-md-surface rounded-3xl p-8 mb-6" 
    :class="meshCard ? 'mesh-card' : ''"
    style="box-shadow:0 1px 3px rgba(0,0,0,0.12),0 1px 2px rgba(0,0,0,0.24)"
  >
    <!-- Optional Header -->
    <div v-if="title || description || $slots.header || $slots.actions" class="mb-6">
      <slot name="header">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1">
            <h2 v-if="title" class="text-xl font-bold text-md-on-surface flex items-center gap-2">
              <i v-if="icon" :class="icon" class="text-md-primary"></i>
              {{ title }}
            </h2>
            <p v-if="description" class="text-sm text-gray-600 mt-1">
              {{ description }}
            </p>
          </div>
          <!-- Action slot for buttons/controls -->
          <slot name="actions" />
        </div>
      </slot>
    </div>

    <!-- Main Content -->
    <slot />
  </div>
</template>

<script setup lang="ts">
defineProps<{
  meshCard?: boolean
  title?: string
  description?: string
  icon?: string
}>()
</script>

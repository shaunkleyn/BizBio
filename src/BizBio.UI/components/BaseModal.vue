<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="modal-overlay fixed inset-0 flex items-center justify-center p-4 animate-fadeSlide"
      :style="{ zIndex: currentZIndex }"
      @click.self="handleClose"
    >
      <!-- Backdrop with dynamic opacity -->
      <div
        class="fixed inset-0 bg-black transition-opacity"
        :style="{ opacity: backdropOpacity }"
      ></div>

      <!-- Modal Content (positioned above backdrop) -->
      <div
        :class="[
          'modal-content mesh-card bg-md-surface rounded-2xl shadow-md-5 w-full max-h-[85vh] flex flex-col overflow-hidden border border-md-outline-variant relative',
          sizeClass
        ]"
      >
      <!-- Header with Gradient -->
      <div class="modal-header p-6">
        <div class="flex justify-between items-center relative z-10">
          <div>
            <h2 class="text-2xl font-heading font-bold gradient-text">
              {{ title }}
            </h2>
            <p v-if="subtitle" class="text-sm text-md-on-surface-variant mt-1">
              {{ subtitle }}
            </p>
          </div>
          <button
            v-if="showClose"
            @click="handleClose"
            class="modal-close-btn md-ripple shadow-md-1"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>

      <!-- Scrollable Body -->
      <div class="flex-1 overflow-y-auto p-6">
        <slot></slot>
      </div>

      <!-- Footer with Gradient Background -->
      <div v-if="$slots.footer || showFooter" class="modal-footer p-6">
        <div class="relative z-10">
          <slot name="footer">
            <div class="flex gap-3" :class="footerAlign === 'right' ? 'justify-end' : footerAlign === 'between' ? 'justify-between' : 'justify-center'">
              <button
                v-if="showCancel"
                @click="handleClose"
                class="px-6 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all shadow-md-1 md-ripple font-medium"
                :class="cancelButtonClass"
              >
                {{ cancelText }}
              </button>
              <button
                v-if="showConfirm"
                @click="handleConfirm"
                :disabled="confirmDisabled"
                class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-glow-purple transition-all font-bold disabled:opacity-50 disabled:cursor-not-allowed md-ripple flex items-center justify-center gap-2"
                :class="confirmButtonClass"
              >
                <i v-if="confirmLoading" class="fas fa-spinner fa-spin"></i>
                <i v-else-if="confirmIcon" :class="confirmIcon"></i>
                <span>{{ confirmText }}</span>
              </button>
            </div>
          </slot>
        </div>
      </div>
    </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue'
import { useModalStack } from '~/composables/useModalStack'

interface Props {
  modelValue: boolean
  title: string
  subtitle?: string
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full'
  showClose?: boolean
  showFooter?: boolean
  showCancel?: boolean
  showConfirm?: boolean
  cancelText?: string
  confirmText?: string
  confirmIcon?: string
  confirmLoading?: boolean
  confirmDisabled?: boolean
  footerAlign?: 'left' | 'center' | 'right' | 'between'
  cancelButtonClass?: string
  confirmButtonClass?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  showClose: true,
  showFooter: true,
  showCancel: true,
  showConfirm: true,
  cancelText: 'Cancel',
  confirmText: 'Confirm',
  confirmIcon: 'fas fa-check',
  confirmLoading: false,
  confirmDisabled: false,
  footerAlign: 'right',
  cancelButtonClass: '',
  confirmButtonClass: ''
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'close'): void
  (e: 'confirm'): void
}>()

const { currentZIndex, backdropOpacity, registerModal, unregisterModal } = useModalStack()

// Register/unregister modal when it opens/closes
watch(() => props.modelValue, (isOpen) => {
  if (isOpen) {
    registerModal(handleClose)
  } else {
    unregisterModal()
  }
})

const sizeClass = computed(() => {
  const sizes = {
    sm: 'max-w-md',
    md: 'max-w-2xl',
    lg: 'max-w-4xl',
    xl: 'max-w-6xl',
    full: 'max-w-7xl'
  }
  return sizes[props.size]
})

function handleClose() {
  emit('update:modelValue', false)
  emit('close')
}

function handleConfirm() {
  emit('confirm')
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="isOpen"
        class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-[100] p-4 animate-fadeSlide"
        @click.self="handleCancel"
      >
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-5 max-w-md w-full border border-md-outline-variant overflow-hidden">
          <!-- Header -->
          <div class="p-6 border-b border-md-outline-variant relative overflow-hidden">
            <div class="absolute inset-0 bg-gradient-primary opacity-5"></div>
            <div class="flex items-center gap-4 relative z-10">
              <div
                :class="[
                  'w-14 h-14 rounded-2xl flex items-center justify-center shadow-md-2',
                  variant === 'danger' ? 'bg-gradient-to-br from-red-500 to-red-600' : 'bg-gradient-to-br from-blue-500 to-purple-600'
                ]"
              >
                <i
                  :class="[
                    'text-2xl text-white',
                    variant === 'danger' ? 'fas fa-exclamation-triangle' : 'fas fa-question-circle'
                  ]"
                ></i>
              </div>
              <h3 class="text-xl font-heading font-bold text-md-on-surface flex-1">
                {{ title }}
              </h3>
            </div>
          </div>

          <!-- Body -->
          <div class="p-6 relative overflow-hidden">
            <div class="absolute inset-0 mesh-bg-2 opacity-20"></div>
            <p class="text-md-on-surface-variant relative z-10 leading-relaxed">{{ message }}</p>
          </div>

          <!-- Footer -->
          <div class="p-6 border-t border-md-outline-variant flex gap-3">
            <button
              @click="handleCancel"
              class="flex-1 px-5 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface-variant rounded-xl hover:bg-md-surface-container-high hover:shadow-md-1 transition-all font-semibold md-ripple"
            >
              {{ cancelText }}
            </button>
            <button
              @click="handleConfirm"
              :class="[
                'flex-1 px-5 py-3 rounded-xl transition-all font-bold shadow-md-2 md-ripple flex items-center justify-center gap-2',
                variant === 'danger'
                  ? 'bg-gradient-to-r from-red-500 to-red-600 text-white hover:shadow-glow-pink'
                  : 'btn-gradient hover:shadow-glow-purple'
              ]"
            >
              <i :class="variant === 'danger' ? 'fas fa-trash' : 'fas fa-check'"></i>
              {{ confirmText }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: 'Confirm'
  },
  message: {
    type: String,
    required: true
  },
  confirmText: {
    type: String,
    default: 'Confirm'
  },
  cancelText: {
    type: String,
    default: 'Cancel'
  },
  variant: {
    type: String,
    default: 'primary', // 'primary' or 'danger'
    validator: (value) => ['primary', 'danger'].includes(value)
  }
})

const emit = defineEmits(['confirm', 'cancel'])

const handleConfirm = () => {
  emit('confirm')
}

const handleCancel = () => {
  emit('cancel')
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .bg-white,
.modal-leave-active .bg-white {
  transition: transform 0.2s ease;
}

.modal-enter-from .bg-white,
.modal-leave-to .bg-white {
  transform: scale(0.95);
}
</style>

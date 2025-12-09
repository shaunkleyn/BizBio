<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="isOpen"
        class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[100] p-4"
        @click.self="handleCancel"
      >
        <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
          <!-- Header -->
          <div class="p-6 border-b border-[var(--light-border-color)]">
            <div class="flex items-center gap-4">
              <div
                :class="[
                  'w-12 h-12 rounded-full flex items-center justify-center',
                  variant === 'danger' ? 'bg-red-100' : 'bg-blue-100'
                ]"
              >
                <i
                  :class="[
                    'text-2xl',
                    variant === 'danger' ? 'fas fa-exclamation-triangle text-red-600' : 'fas fa-question-circle text-blue-600'
                  ]"
                ></i>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] flex-1">
                {{ title }}
              </h3>
            </div>
          </div>

          <!-- Body -->
          <div class="p-6">
            <p class="text-[var(--dark-text-color)]">{{ message }}</p>
          </div>

          <!-- Footer -->
          <div class="p-6 border-t border-[var(--light-border-color)] flex gap-3">
            <button
              @click="handleCancel"
              class="flex-1 px-4 py-3 border-2 border-[var(--light-border-color)] text-[var(--dark-text-color)] rounded-lg hover:border-[var(--primary-color)] transition-colors font-semibold"
            >
              {{ cancelText }}
            </button>
            <button
              @click="handleConfirm"
              :class="[
                'flex-1 px-4 py-3 rounded-lg transition-colors font-semibold',
                variant === 'danger'
                  ? 'bg-red-600 text-white hover:bg-red-700'
                  : 'bg-[var(--primary-color)] text-white hover:bg-[var(--primary-button-hover-bg-color)]'
              ]"
            >
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

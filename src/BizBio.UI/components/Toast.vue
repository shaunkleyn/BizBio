<template>
  <Teleport to="body">
    <Transition name="toast">
      <div
        v-if="isVisible"
        :class="[
          'fixed top-4 right-4 z-[101] max-w-md w-full shadow-md-5 rounded-2xl overflow-hidden glass-effect animate-fadeSlide',
          typeClass
        ]"
      >
        <div class="p-4 flex items-center gap-3">
          <div class="flex-shrink-0">
            <div :class="[iconContainerClass, 'p-2 rounded-full']">
              <i :class="[iconClass, 'text-xl']"></i>
            </div>
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-semibold" v-if="title">{{ title }}</p>
            <p :class="title ? 'text-sm' : ''">{{ message }}</p>
          </div>
          <button
            @click="close"
            class="flex-shrink-0 opacity-70 hover:opacity-100 transition-opacity"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
const props = defineProps({
  message: {
    type: String,
    required: true
  },
  title: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'info',
    validator: (value) => ['success', 'error', 'warning', 'info'].includes(value)
  },
  duration: {
    type: Number,
    default: 5000
  }
})

const emit = defineEmits(['close'])

const isVisible = ref(true)
let timeoutId = null

const typeClass = computed(() => {
  const classes = {
    success: 'bg-md-success-container text-md-on-success-container border-l-4 border-md-success',
    error: 'bg-md-error-container text-md-on-error-container border-l-4 border-md-error',
    warning: 'bg-md-accent-container text-md-on-accent-container border-l-4 border-md-accent',
    info: 'bg-md-primary-container text-md-on-primary-container border-l-4 border-md-primary'
  }
  return classes[props.type]
})

const iconContainerClass = computed(() => {
  const classes = {
    success: 'bg-md-success shadow-glow-teal',
    error: 'bg-md-error shadow-glow-pink',
    warning: 'bg-md-accent',
    info: 'bg-md-primary shadow-glow-purple'
  }
  return classes[props.type]
})

const iconClass = computed(() => {
  const icons = {
    success: 'fas fa-check-circle text-white',
    error: 'fas fa-exclamation-circle text-white',
    warning: 'fas fa-exclamation-triangle text-white',
    info: 'fas fa-info-circle text-white'
  }
  return icons[props.type]
})

const close = () => {
  isVisible.value = false
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
  emit('close')
}

onMounted(() => {
  if (props.duration > 0) {
    timeoutId = setTimeout(() => {
      close()
    }, props.duration)
  }
})

onUnmounted(() => {
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
})
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}
</style>

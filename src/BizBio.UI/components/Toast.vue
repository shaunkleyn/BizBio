<template>
  <Teleport to="body">
    <Transition name="toast">
      <div
        v-if="isVisible"
        :class="[
          'fixed top-4 right-4 z-[101] max-w-md w-full shadow-2xl rounded-xl overflow-hidden',
          typeClass
        ]"
      >
        <div class="p-4 flex items-center gap-3">
          <div class="flex-shrink-0">
            <i :class="[iconClass, 'text-2xl']"></i>
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
    success: 'bg-green-500 text-white',
    error: 'bg-red-600 text-white',
    warning: 'bg-yellow-500 text-white',
    info: 'bg-blue-600 text-white'
  }
  return classes[props.type]
})

const iconClass = computed(() => {
  const icons = {
    success: 'fas fa-check-circle',
    error: 'fas fa-exclamation-circle',
    warning: 'fas fa-exclamation-triangle',
    info: 'fas fa-info-circle'
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
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}
</style>

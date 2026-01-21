import { ref, computed, onMounted, onUnmounted } from 'vue'

interface ModalStackItem {
  id: string
  closeHandler: () => void
}

const modalStack = ref<ModalStackItem[]>([])
const BASE_Z_INDEX = 1000

export const useModalStack = () => {
  const modalId = ref<string>('')

  const currentZIndex = computed(() => {
    const index = modalStack.value.findIndex(item => item.id === modalId.value)
    if (index === -1) return BASE_Z_INDEX
    return BASE_Z_INDEX + index
  })

  const isTopmost = computed(() => {
    if (modalStack.value.length === 0) return false
    return modalStack.value[modalStack.value.length - 1].id === modalId.value
  })

  const backdropOpacity = computed(() => {
    const index = modalStack.value.findIndex(item => item.id === modalId.value)
    if (index === -1) return 0.5
    // Each layer gets slightly less opacity to show stacking
    return 0.3 + (index * 0.1)
  })

  const registerModal = (closeHandler: () => void) => {
    modalId.value = `modal-${Date.now()}-${Math.random()}`
    modalStack.value.push({
      id: modalId.value,
      closeHandler
    })
  }

  const unregisterModal = () => {
    const index = modalStack.value.findIndex(item => item.id === modalId.value)
    if (index !== -1) {
      modalStack.value.splice(index, 1)
    }
  }

  const handleEscapeKey = (event: KeyboardEvent) => {
    if (event.key === 'Escape' && isTopmost.value) {
      const modal = modalStack.value[modalStack.value.length - 1]
      if (modal && modal.closeHandler) {
        modal.closeHandler()
      }
    }
  }

  onMounted(() => {
    document.addEventListener('keydown', handleEscapeKey)
  })

  onUnmounted(() => {
    document.removeEventListener('keydown', handleEscapeKey)
    unregisterModal()
  })

  return {
    modalId,
    currentZIndex,
    isTopmost,
    backdropOpacity,
    registerModal,
    unregisterModal,
    stackSize: computed(() => modalStack.value.length)
  }
}

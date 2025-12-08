import { ref } from 'vue'

interface ToastOptions {
  message: string
  title?: string
  type?: 'success' | 'error' | 'warning' | 'info'
  duration?: number
}

const toasts = ref<Array<ToastOptions & { id: number }>>([])
let toastId = 0

export const useToast = () => {
  const show = (options: ToastOptions) => {
    const id = ++toastId
    toasts.value.push({
      id,
      ...options
    })

    // Auto-remove after duration
    if (options.duration !== 0) {
      setTimeout(() => {
        remove(id)
      }, options.duration || 5000)
    }

    return id
  }

  const success = (message: string, title?: string, duration?: number) => {
    return show({ message, title, type: 'success', duration })
  }

  const error = (message: string, title?: string, duration?: number) => {
    return show({ message, title, type: 'error', duration })
  }

  const warning = (message: string, title?: string, duration?: number) => {
    return show({ message, title, type: 'warning', duration })
  }

  const info = (message: string, title?: string, duration?: number) => {
    return show({ message, title, type: 'info', duration })
  }

  const remove = (id: number) => {
    const index = toasts.value.findIndex(t => t.id === id)
    if (index !== -1) {
      toasts.value.splice(index, 1)
    }
  }

  return {
    toasts,
    show,
    success,
    error,
    warning,
    info,
    remove
  }
}

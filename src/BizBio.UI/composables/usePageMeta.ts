import { ref, type VNode } from 'vue'

interface PageMeta {
  title: string
  description: string
  editable?: boolean
  onRename?: (newName: string) => Promise<void>
  actions?: VNode[]
}

interface PageActionButton {
  to?: string
  label: string
  icon?: string
  class?: string
  onClick?: () => void
}

const pageHeader = ref<PageMeta>({ title: 'Dashboard', description: 'Manage your content' })
const pageActions = ref<(() => VNode) | null>(null)
const pageActionButton = ref<PageActionButton | null>(null)

export const usePageMeta = () => {
  const setPageHeader = (meta: PageMeta) => {
    pageHeader.value = meta
  }

  const setPageActions = (actions: (() => VNode) | null) => {
    pageActions.value = actions
  }

  const setPageActionButton = (button: PageActionButton | null) => {
    pageActionButton.value = button
  }

  const clearPageMeta = () => {
    pageHeader.value = { title: 'Dashboard', description: 'Manage your content' }
    pageActions.value = null
    pageActionButton.value = null
  }

  return {
    pageHeader,
    pageActions,
    pageActionButton,
    setPageHeader,
    setPageActions,
    setPageActionButton,
    clearPageMeta
  }
}

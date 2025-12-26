import { ref, type VNode } from 'vue'

interface PageMeta {
  title: string
  description: string
  editable?: boolean
  onRename?: (newName: string) => Promise<void>
  actions?: VNode[]
}

const pageHeader = ref<PageMeta>({ title: 'Dashboard', description: 'Manage your content' })
const pageActions = ref<(() => VNode) | null>(null)

export const usePageMeta = () => {
  const setPageHeader = (meta: PageMeta) => {
    pageHeader.value = meta
  }

  const setPageActions = (actions: (() => VNode) | null) => {
    pageActions.value = actions
  }

  const clearPageMeta = () => {
    pageHeader.value = { title: 'Dashboard', description: 'Manage your content' }
    pageActions.value = null
  }

  return {
    pageHeader,
    pageActions,
    setPageHeader,
    setPageActions,
    clearPageMeta
  }
}

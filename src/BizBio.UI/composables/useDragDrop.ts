import Sortable, { type SortableOptions } from 'sortablejs'

export interface DragDropOptions extends Partial<SortableOptions> {
  onUpdate?: (event: Sortable.SortableEvent) => void
  onAdd?: (event: Sortable.SortableEvent) => void
  onRemove?: (event: Sortable.SortableEvent) => void
}

export function useDragDrop() {
  /**
   * Enable drag-and-drop sorting on an HTML element
   * @param element - The HTML element or selector string
   * @param options - Sortable options and event handlers
   * @returns Sortable instance
   */
  const enableSortable = (
    element: HTMLElement | string,
    options: DragDropOptions = {}
  ): Sortable | null => {
    let el: HTMLElement | null = null

    if (typeof element === 'string') {
      el = document.querySelector(element)
    } else {
      el = element
    }

    if (!el) {
      console.warn('useDragDrop: Element not found', element)
      return null
    }

    const { onUpdate, onAdd, onRemove, ...sortableOptions } = options

    const sortable = Sortable.create(el, {
      animation: 150,
      handle: '.drag-handle',
      ghostClass: 'sortable-ghost',
      dragClass: 'sortable-drag',
      chosenClass: 'sortable-chosen',
      ...sortableOptions,
      onEnd: (event) => {
        if (onUpdate) {
          onUpdate(event)
        }
        if (sortableOptions.onEnd) {
          sortableOptions.onEnd(event)
        }
      },
      onAdd: (event) => {
        if (onAdd) {
          onAdd(event)
        }
        if (sortableOptions.onAdd) {
          sortableOptions.onAdd(event)
        }
      },
      onRemove: (event) => {
        if (onRemove) {
          onRemove(event)
        }
        if (sortableOptions.onRemove) {
          sortableOptions.onRemove(event)
        }
      }
    })

    return sortable
  }

  /**
   * Enable drag-and-drop with cross-list support
   * @param elements - Array of elements or selector strings
   * @param groupName - Shared group name for cross-list dragging
   * @param options - Sortable options and event handlers
   * @returns Array of Sortable instances
   */
  const enableCrossList = (
    elements: (HTMLElement | string)[],
    groupName: string,
    options: DragDropOptions = {}
  ): Sortable[] => {
    const sortables: Sortable[] = []

    elements.forEach((element) => {
      const sortable = enableSortable(element, {
        ...options,
        group: groupName
      })
      if (sortable) {
        sortables.push(sortable)
      }
    })

    return sortables
  }

  /**
   * Destroy a Sortable instance
   * @param sortable - The Sortable instance to destroy
   */
  const destroySortable = (sortable: Sortable | null) => {
    if (sortable) {
      sortable.destroy()
    }
  }

  return {
    enableSortable,
    enableCrossList,
    destroySortable
  }
}

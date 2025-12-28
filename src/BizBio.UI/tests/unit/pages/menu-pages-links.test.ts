import { describe, it, expect } from 'vitest'
import { readFileSync } from 'fs'
import { resolve } from 'path'

describe('Menu Pages Internal Links', () => {
  const pagesDir = resolve(process.cwd(), 'pages/menu')

  describe('Menu Index Page (/menu/index.vue)', () => {
    const content = readFileSync(resolve(pagesDir, 'index.vue'), 'utf-8')

    it('should link to /menu/create for creating new menus', () => {
      expect(content).toContain('to="/menu/create"')
      expect(content).not.toContain('to="/dashboard/menu/create"')
    })

    it('should link to /menu/[id]/edit for editing menus', () => {
      expect(content).toContain(':to="`/menu/${menu.id}/edit`"')
      expect(content).not.toContain('/dashboard/menu/${menu.id}/edit')
    })

    it('should NOT contain any old /dashboard/menu/ links', () => {
      expect(content).not.toContain('/dashboard/menu/')
    })
  })

  describe('Menu Dashboard Page (/menu/dashboard/index.vue)', () => {
    const content = readFileSync(resolve(pagesDir, 'dashboard/index.vue'), 'utf-8')

    it('should link to /menu/create for creating menus', () => {
      expect(content).toContain("to: '/menu/create'")
      expect(content).not.toContain("to: '/dashboard/menu/create'")
    })

    it('should link to /menu for viewing all menus', () => {
      expect(content).toContain('to="/menu"')
    })

    it('should link to /menu/items for library items', () => {
      expect(content).toContain('to="/menu/items"')
      expect(content).not.toContain('to="/menu/library/items"')
    })

    it('should NOT contain any /menu/library/ links', () => {
      expect(content).not.toContain('/menu/library/')
    })
  })

  describe('Menu Content Page (/menu/[id]/content.vue)', () => {
    const content = readFileSync(resolve(pagesDir, '[id]/content.vue'), 'utf-8')

    it('should link to /menu/[id]/edit for menu settings', () => {
      expect(content).toContain(':to="`/menu/${catalogId}/edit`"')
      expect(content).not.toContain('/dashboard/menu/${catalogId}/edit')
    })

    it('should NOT contain any /dashboard/menu/ links', () => {
      expect(content).not.toContain('/dashboard/menu/')
    })
  })

  describe('Bundles Pages', () => {
    describe('Bundles Index (/menu/bundles/index.vue)', () => {
      const content = readFileSync(resolve(pagesDir, 'bundles/index.vue'), 'utf-8')

      it('should link to /menu/bundles/[id]/edit for editing bundles', () => {
        expect(content).toContain(':to="`/menu/bundles/${bundle.id}/edit`"')
        expect(content).not.toContain('/dashboard/bundles/')
      })

      it('should NOT contain any /dashboard/bundles links', () => {
        expect(content).not.toContain('/dashboard/bundles')
      })
    })

    describe('Bundle Create Page (/menu/bundles/create.vue)', () => {
      const content = readFileSync(resolve(pagesDir, 'bundles/create.vue'), 'utf-8')

      it('should navigate to /menu/bundles after creation', () => {
        expect(content).toContain("'/menu/bundles'")
        expect(content).not.toContain("'/dashboard/bundles'")
      })
    })

    describe('Bundle Edit Page (/menu/bundles/[id]/edit.vue)', () => {
      const content = readFileSync(resolve(pagesDir, 'bundles/[id]/edit.vue'), 'utf-8')

      it('should link back to /menu/bundles', () => {
        expect(content).toContain('to="/menu/bundles"')
        expect(content).not.toContain('to="/dashboard/bundles"')
      })

      it('should navigate to /menu/bundles after updates', () => {
        expect(content).toContain("'/menu/bundles'")
      })
    })
  })
})

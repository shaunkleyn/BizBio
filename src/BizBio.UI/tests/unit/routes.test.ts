import { describe, it, expect } from 'vitest'
import { existsSync } from 'fs'
import { resolve } from 'path'

describe('Menu Product Routes', () => {
  const pagesDir = resolve(process.cwd(), 'pages/menu')

  describe('Menu Root Routes', () => {
    it('should have menu index page at /menu/index.vue', () => {
      const filePath = resolve(pagesDir, 'index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have menu create page at /menu/create.vue', () => {
      const filePath = resolve(pagesDir, 'create.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have menu dashboard page at /menu/dashboard/index.vue', () => {
      const filePath = resolve(pagesDir, 'dashboard/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have public menu view at /menu/[slug].vue', () => {
      const filePath = resolve(pagesDir, '[slug].vue')
      expect(existsSync(filePath)).toBe(true)
    })
  })

  describe('Menu Edit Routes', () => {
    it('should have menu edit page at /menu/[id]/edit.vue', () => {
      const filePath = resolve(pagesDir, '[id]/edit.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have menu content page at /menu/[id]/content.vue', () => {
      const filePath = resolve(pagesDir, '[id]/content.vue')
      expect(existsSync(filePath)).toBe(true)
    })
  })

  describe('Bundle Routes', () => {
    it('should have bundles index page at /menu/bundles/index.vue', () => {
      const filePath = resolve(pagesDir, 'bundles/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have bundle create page at /menu/bundles/create.vue', () => {
      const filePath = resolve(pagesDir, 'bundles/create.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have bundle edit page at /menu/bundles/[id]/edit.vue', () => {
      const filePath = resolve(pagesDir, 'bundles/[id]/edit.vue')
      expect(existsSync(filePath)).toBe(true)
    })
  })

  describe('Library Item Routes', () => {
    it('should have items index page at /menu/items/index.vue', () => {
      const filePath = resolve(pagesDir, 'items/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have extras index page at /menu/extras/index.vue', () => {
      const filePath = resolve(pagesDir, 'extras/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have extra groups index page at /menu/extras/groups/index.vue', () => {
      const filePath = resolve(pagesDir, 'extras/groups/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have options index page at /menu/options/index.vue', () => {
      const filePath = resolve(pagesDir, 'options/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have option groups index page at /menu/options/groups/index.vue', () => {
      const filePath = resolve(pagesDir, 'options/groups/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })
  })

  describe('Other Menu Routes', () => {
    it('should have categories page at /menu/categories/index.vue', () => {
      const filePath = resolve(pagesDir, 'categories/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have restaurants page at /menu/restaurants/index.vue', () => {
      const filePath = resolve(pagesDir, 'restaurants/index.vue')
      expect(existsSync(filePath)).toBe(true)
    })

    it('should have analytics page at /menu/analytics/[id].vue', () => {
      const filePath = resolve(pagesDir, 'analytics/[id].vue')
      expect(existsSync(filePath)).toBe(true)
    })
  })

  describe('Old Routes Should Not Exist', () => {
    it('should NOT have /dashboard/menu directory', () => {
      const filePath = resolve(process.cwd(), 'pages/dashboard/menu')
      // Directory might exist but should be empty or not contain index.vue
      const indexPath = resolve(filePath, 'index.vue')
      const createPath = resolve(filePath, 'create.vue')
      expect(existsSync(indexPath) && existsSync(createPath)).toBe(false)
    })

    it('should NOT have /dashboard/bundles directory', () => {
      const filePath = resolve(process.cwd(), 'pages/dashboard/bundles')
      expect(existsSync(filePath)).toBe(false)
    })

    it('should NOT have /dashboard/restaurants directory', () => {
      const filePath = resolve(process.cwd(), 'pages/dashboard/restaurants')
      expect(existsSync(filePath)).toBe(false)
    })

    it('should NOT have /menu/library directory with old files', () => {
      const baseDir = resolve(pagesDir, 'library')
      if (existsSync(baseDir)) {
        expect(existsSync(resolve(baseDir, 'items.vue'))).toBe(false)
        expect(existsSync(resolve(baseDir, 'extras.vue'))).toBe(false)
        expect(existsSync(resolve(baseDir, 'options.vue'))).toBe(false)
      }
    })

    it('should NOT have /menu/menus directory', () => {
      const filePath = resolve(pagesDir, 'menus')
      expect(existsSync(filePath)).toBe(false)
    })
  })
})

import { describe, it, expect } from 'vitest'
import { readFileSync } from 'fs'
import { resolve } from 'path'

describe('Component Internal Links', () => {
  const componentsDir = resolve(process.cwd(), 'components')

  describe('ItemFormModal Component', () => {
    const content = readFileSync(resolve(componentsDir, 'ItemFormModal.vue'), 'utf-8')

    it('should link to /menu/options/groups for option groups', () => {
      expect(content).toContain('to="/menu/options/groups"')
      expect(content).not.toContain('to="/menu/library/option-groups"')
    })

    it('should link to /menu/extras/groups for extra groups', () => {
      expect(content).toContain('to="/menu/extras/groups"')
      expect(content).not.toContain('to="/menu/library/extra-groups"')
    })

    it('should NOT contain any /menu/library/ links', () => {
      const libraryLinks = content.match(/to="\/menu\/library\//g)
      expect(libraryLinks).toBeNull()
    })
  })

  describe('RestaurantSelector Component', () => {
    const content = readFileSync(resolve(componentsDir, 'RestaurantSelector.vue'), 'utf-8')

    it('should navigate to /menu/restaurants when creating restaurants', () => {
      expect(content).toContain("'/menu/restaurants'")
      expect(content).not.toContain("'/dashboard/restaurants'")
    })

    it('should NOT contain any /dashboard/restaurants references', () => {
      expect(content).not.toContain('/dashboard/restaurants')
    })
  })

  describe('MenuSidebar Component', () => {
    const content = readFileSync(resolve(componentsDir, 'MenuSidebar.vue'), 'utf-8')

    it('should have all new menu product routes', () => {
      const requiredRoutes = [
        'to="/menu/dashboard"',
        'to="/menu"',
        'to="/menu/bundles"',
        'to="/menu/restaurants"',
        'to="/menu/items"',
        'to="/menu/extras"',
        'to="/menu/extras/groups"',
        'to="/menu/options"',
        'to="/menu/options/groups"'
      ]

      requiredRoutes.forEach(route => {
        expect(content).toContain(route)
      })
    })

    it('should NOT have any old route patterns', () => {
      const oldRoutes = [
        '/dashboard/menu/',
        '/dashboard/bundles',
        '/dashboard/restaurants',
        '/menu/library/',
        '/menu/menus'
      ]

      oldRoutes.forEach(route => {
        expect(content).not.toContain(route)
      })
    })
  })
})

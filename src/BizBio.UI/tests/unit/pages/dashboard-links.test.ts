import { describe, it, expect } from 'vitest'
import { fileURLToPath } from 'node:url'
import { readFileSync } from 'fs'
import { resolve } from 'path'

describe('Dashboard Page Links', () => {
  const dashboardPath = resolve(process.cwd(), 'pages/dashboard/index.vue')
  const dashboardContent = readFileSync(dashboardPath, 'utf-8')

  describe('Restaurant Management Links', () => {
    it('should link to /menu/restaurants for "Manage All"', () => {
      expect(dashboardContent).toContain('to="/menu/restaurants"')
      expect(dashboardContent).not.toContain('to="/dashboard/restaurants"')
    })

    it('should link to /menu/restaurants for "Create Restaurant"', () => {
      const createRestaurantMatches = dashboardContent.match(/to="\/menu\/restaurants"/g)
      expect(createRestaurantMatches).toBeTruthy()
      expect(createRestaurantMatches!.length).toBeGreaterThan(0)
    })

    it('should use template literals for menu edit links with /menu/[id]/edit pattern', () => {
      expect(dashboardContent).toContain(':to="`/menu/${menu.id}/edit`"')
      expect(dashboardContent).not.toContain('/dashboard/menu/')
    })

    it('should NOT contain any /dashboard/restaurants links', () => {
      expect(dashboardContent).not.toContain('/dashboard/restaurants')
    })
  })

  describe('Menu View Links', () => {
    it('should use /menu?restaurant query param for viewing restaurant menus', () => {
      const viewAllPattern = /to="`\/menu\?restaurant=\$\{restaurant\.id\}`"/
      expect(viewAllPattern.test(dashboardContent)).toBe(true)
    })

    it('should NOT use old /dashboard/restaurants/[id]/menus pattern', () => {
      expect(dashboardContent).not.toContain('/dashboard/restaurants/${restaurant.id}/menus')
    })
  })
})

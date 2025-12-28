import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import MenuSidebar from '../../../components/MenuSidebar.vue'

// Mock the composables
vi.mock('#app', () => ({
  useRoute: vi.fn(() => ({
    path: '/menu/dashboard'
  })),
  provide: vi.fn()
}))

vi.mock('../../../composables/usePageMeta', () => ({
  usePageMeta: vi.fn(() => ({
    pageHeader: { value: null },
    pageActions: { value: null }
  }))
}))

describe('MenuSidebar Navigation Links', () => {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/menu/dashboard', component: { template: '<div>Dashboard</div>' } },
      { path: '/menu', component: { template: '<div>Menus</div>' } },
      { path: '/menu/bundles', component: { template: '<div>Bundles</div>' } },
      { path: '/menu/restaurants', component: { template: '<div>Restaurants</div>' } },
      { path: '/menu/items', component: { template: '<div>Items</div>' } },
      { path: '/menu/extras', component: { template: '<div>Extras</div>' } },
      { path: '/menu/extras/groups', component: { template: '<div>Extra Groups</div>' } },
      { path: '/menu/options', component: { template: '<div>Options</div>' } },
      { path: '/menu/options/groups', component: { template: '<div>Option Groups</div>' } },
      { path: '/menu/categories', component: { template: '<div>Categories</div>' } },
    ]
  })

  const createWrapper = (props = {}) => {
    return mount(MenuSidebar, {
      global: {
        plugins: [router],
        stubs: {
          NuxtLink: {
            template: '<a :href="to"><slot /></a>',
            props: ['to']
          },
          Teleport: {
            template: '<div><slot /></div>'
          }
        }
      },
      props
    })
  }

  describe('Main Navigation Links', () => {
    it('should have Overview link pointing to /menu/dashboard', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const overviewLink = links.find(link => link.text().includes('Overview'))

      expect(overviewLink?.exists()).toBe(true)
      expect(overviewLink?.attributes('href')).toBe('/menu/dashboard')
    })

    it('should have Menus link pointing to /menu', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const menusLink = links.find(link => link.text().includes('Menus'))

      expect(menusLink?.exists()).toBe(true)
      expect(menusLink?.attributes('href')).toBe('/menu')
    })

    it('should have Bundles link pointing to /menu/bundles', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const bundlesLink = links.find(link => link.text().includes('Bundles'))

      expect(bundlesLink?.exists()).toBe(true)
      expect(bundlesLink?.attributes('href')).toBe('/menu/bundles')
    })

    it('should have Restaurants link pointing to /menu/restaurants', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const restaurantsLink = links.find(link => link.text().includes('Restaurants'))

      expect(restaurantsLink?.exists()).toBe(true)
      expect(restaurantsLink?.attributes('href')).toBe('/menu/restaurants')
    })
  })

  describe('Library Navigation Links', () => {
    it('should have Items link pointing to /menu/items', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const itemsLink = links.find(link => link.text().trim() === 'Items')

      expect(itemsLink?.exists()).toBe(true)
      expect(itemsLink?.attributes('href')).toBe('/menu/items')
    })

    it('should have Extras link pointing to /menu/extras', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const extrasLink = links.find(link => link.text().includes('Extras') && !link.text().includes('Groups'))

      expect(extrasLink?.exists()).toBe(true)
      expect(extrasLink?.attributes('href')).toBe('/menu/extras')
    })

    it('should have Extra Groups link pointing to /menu/extras/groups', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const extraGroupsLink = links.find(link => link.text().includes('Extra Groups'))

      expect(extraGroupsLink?.exists()).toBe(true)
      expect(extraGroupsLink?.attributes('href')).toBe('/menu/extras/groups')
    })

    it('should have Options link pointing to /menu/options', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const optionsLink = links.find(link => link.text().trim() === 'Options')

      expect(optionsLink?.exists()).toBe(true)
      expect(optionsLink?.attributes('href')).toBe('/menu/options')
    })

    it('should have Option Groups link pointing to /menu/options/groups', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const optionGroupsLink = links.find(link => link.text().includes('Option Groups'))

      expect(optionGroupsLink?.exists()).toBe(true)
      expect(optionGroupsLink?.attributes('href')).toBe('/menu/options/groups')
    })
  })

  describe('Old Routes Should Not Exist', () => {
    it('should NOT have any links to /dashboard/menu/', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const oldLinks = links.filter(link => link.attributes('href')?.includes('/dashboard/menu/'))

      expect(oldLinks.length).toBe(0)
    })

    it('should NOT have any links to /dashboard/bundles', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const oldLinks = links.filter(link => link.attributes('href') === '/dashboard/bundles')

      expect(oldLinks.length).toBe(0)
    })

    it('should NOT have any links to /dashboard/restaurants', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const oldLinks = links.filter(link => link.attributes('href') === '/dashboard/restaurants')

      expect(oldLinks.length).toBe(0)
    })

    it('should NOT have any links to /menu/library/', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const oldLinks = links.filter(link => link.attributes('href')?.includes('/menu/library/'))

      expect(oldLinks.length).toBe(0)
    })

    it('should NOT have any links to /menu/menus', () => {
      const wrapper = createWrapper()
      const links = wrapper.findAll('a')
      const oldLinks = links.filter(link => link.attributes('href') === '/menu/menus')

      expect(oldLinks.length).toBe(0)
    })
  })

  describe('Navigation Structure', () => {
    it('should have all required navigation sections', () => {
      const wrapper = createWrapper()
      const html = wrapper.html()

      // Check for main sections
      expect(html).toContain('Overview')
      expect(html).toContain('Menus')
      expect(html).toContain('Bundles')
      expect(html).toContain('Restaurants')

      // Check for library section
      expect(html).toContain('Library')
      expect(html).toContain('Items')
      expect(html).toContain('Extras')
      expect(html).toContain('Options')
    })
  })
})

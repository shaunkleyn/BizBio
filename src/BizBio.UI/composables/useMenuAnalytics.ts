export const useMenuAnalytics = () => {
  const api = useApi()
  const { $trackEvent, $trackPageView } = useNuxtApp()

  // Track menu page view
  const trackMenuView = async (menuId: string, menuSlug: string, menuData: any) => {
    try {
      const viewData = {
        menuId,
        slug: menuSlug,
        timestamp: new Date().toISOString(),
        // Browser & Device Info
        userAgent: navigator.userAgent,
        deviceType: getDeviceType(),
        screenResolution: `${window.screen.width}x${window.screen.height}`,
        viewport: `${window.innerWidth}x${window.innerHeight}`,
        // Location (if available)
        language: navigator.language,
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
        // Referrer
        referrer: document.referrer || 'direct',
        // Menu Info
        businessName: menuData.businessName,
        cuisine: menuData.cuisine,
        categoriesCount: menuData.categories?.length || 0,
        itemsCount: menuData.items?.length || 0
      }

      // Send to backend
      await api.post('/analytics/menu/view', viewData)

      // Track in Application Insights & GA4
      if ($trackPageView) {
        $trackPageView(`/menu/${menuSlug}`, {
          menuId,
          businessName: menuData.businessName,
          cuisine: menuData.cuisine
        })
      }

      if ($trackEvent) {
        $trackEvent('Menu Viewed', {
          menuId,
          menuSlug,
          businessName: menuData.businessName,
          cuisine: menuData.cuisine,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track menu view:', error)
    }
  }

  // Track category view (when user scrolls to or clicks on a category)
  const trackCategoryView = async (menuId: string, categoryId: string, categoryName: string) => {
    try {
      const viewData = {
        menuId,
        categoryId,
        categoryName,
        timestamp: new Date().toISOString(),
        deviceType: getDeviceType()
      }

      await api.post('/analytics/menu/category-view', viewData)

      if ($trackEvent) {
        $trackEvent('Category Viewed', {
          menuId,
          categoryId,
          categoryName,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track category view:', error)
    }
  }

  // Track menu item view/click
  const trackItemView = async (menuId: string, itemId: string, itemData: any) => {
    try {
      const viewData = {
        menuId,
        itemId,
        itemName: itemData.name,
        categoryId: itemData.categoryId,
        price: itemData.price,
        timestamp: new Date().toISOString(),
        deviceType: getDeviceType()
      }

      await api.post('/analytics/menu/item-view', viewData)

      if ($trackEvent) {
        $trackEvent('Menu Item Viewed', {
          menuId,
          itemId,
          itemName: itemData.name,
          categoryId: itemData.categoryId,
          price: itemData.price,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track item view:', error)
    }
  }

  // Track time spent on menu
  const trackTimeSpent = async (menuId: string, timeInSeconds: number) => {
    try {
      const timeData = {
        menuId,
        timeSpent: timeInSeconds,
        timestamp: new Date().toISOString(),
        deviceType: getDeviceType()
      }

      await api.post('/analytics/menu/time-spent', timeData)

      if ($trackEvent) {
        $trackEvent('Menu Time Spent', {
          menuId,
          timeSpent: timeInSeconds,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track time spent:', error)
    }
  }

  // Track scroll depth
  const trackScrollDepth = async (menuId: string, scrollPercentage: number) => {
    try {
      const scrollData = {
        menuId,
        scrollDepth: scrollPercentage,
        timestamp: new Date().toISOString(),
        deviceType: getDeviceType()
      }

      await api.post('/analytics/menu/scroll-depth', scrollData)

      if ($trackEvent) {
        $trackEvent('Menu Scroll Depth', {
          menuId,
          scrollDepth: scrollPercentage,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track scroll depth:', error)
    }
  }

  // Track social share
  const trackSocialShare = async (menuId: string, platform: string) => {
    try {
      const shareData = {
        menuId,
        platform,
        timestamp: new Date().toISOString()
      }

      await api.post('/analytics/menu/social-share', shareData)

      if ($trackEvent) {
        $trackEvent('Menu Shared', {
          menuId,
          platform
        })
      }
    } catch (error) {
      console.error('Failed to track social share:', error)
    }
  }

  // Track contact action (phone call, email, etc.)
  const trackContactAction = async (menuId: string, actionType: 'phone' | 'email' | 'address') => {
    try {
      const actionData = {
        menuId,
        actionType,
        timestamp: new Date().toISOString(),
        deviceType: getDeviceType()
      }

      await api.post('/analytics/menu/contact-action', actionData)

      if ($trackEvent) {
        $trackEvent('Menu Contact Action', {
          menuId,
          actionType,
          deviceType: getDeviceType()
        })
      }
    } catch (error) {
      console.error('Failed to track contact action:', error)
    }
  }

  // Track search within menu (if you add search functionality)
  const trackMenuSearch = async (menuId: string, searchQuery: string, resultsCount: number) => {
    try {
      const searchData = {
        menuId,
        searchQuery,
        resultsCount,
        timestamp: new Date().toISOString()
      }

      await api.post('/analytics/menu/search', searchData)

      if ($trackEvent) {
        $trackEvent('Menu Searched', {
          menuId,
          searchQuery,
          resultsCount
        })
      }
    } catch (error) {
      console.error('Failed to track menu search:', error)
    }
  }

  // Get device type
  const getDeviceType = (): 'mobile' | 'tablet' | 'desktop' => {
    const width = window.innerWidth
    if (width < 768) return 'mobile'
    if (width < 1024) return 'tablet'
    return 'desktop'
  }

  // Setup intersection observer for category tracking
  const setupCategoryTracking = (menuId: string, categories: any[]) => {
    if (typeof window === 'undefined' || !window.IntersectionObserver) return

    const trackedCategories = new Set<string>()

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting && entry.intersectionRatio > 0.5) {
            const categoryId = entry.target.getAttribute('data-category-id')
            const categoryName = entry.target.getAttribute('data-category-name')

            if (categoryId && !trackedCategories.has(categoryId)) {
              trackedCategories.add(categoryId)
              trackCategoryView(menuId, categoryId, categoryName || '')
            }
          }
        })
      },
      {
        threshold: 0.5 // Category is considered viewed when 50% visible
      }
    )

    return observer
  }

  // Setup scroll depth tracking
  const setupScrollTracking = (menuId: string) => {
    if (typeof window === 'undefined') return

    let maxScrollDepth = 0
    const milestones = [25, 50, 75, 100]
    const trackedMilestones = new Set<number>()

    const handleScroll = () => {
      const windowHeight = window.innerHeight
      const documentHeight = document.documentElement.scrollHeight
      const scrollTop = window.scrollY || document.documentElement.scrollTop

      const scrollPercentage = Math.round((scrollTop / (documentHeight - windowHeight)) * 100)

      if (scrollPercentage > maxScrollDepth) {
        maxScrollDepth = scrollPercentage

        // Track milestones
        milestones.forEach((milestone) => {
          if (scrollPercentage >= milestone && !trackedMilestones.has(milestone)) {
            trackedMilestones.add(milestone)
            trackScrollDepth(menuId, milestone)
          }
        })
      }
    }

    let scrollTimeout: NodeJS.Timeout
    const debouncedScroll = () => {
      clearTimeout(scrollTimeout)
      scrollTimeout = setTimeout(handleScroll, 100)
    }

    window.addEventListener('scroll', debouncedScroll, { passive: true })

    return () => {
      window.removeEventListener('scroll', debouncedScroll)
      clearTimeout(scrollTimeout)
    }
  }

  // Setup time tracking
  const setupTimeTracking = (menuId: string) => {
    if (typeof window === 'undefined') return

    const startTime = Date.now()
    let isActive = true

    // Track visibility changes
    const handleVisibilityChange = () => {
      isActive = !document.hidden
    }

    document.addEventListener('visibilitychange', handleVisibilityChange)

    // Send time spent when user leaves
    const sendTimeSpent = () => {
      if (isActive) {
        const timeSpent = Math.round((Date.now() - startTime) / 1000)
        if (timeSpent > 0) {
          trackTimeSpent(menuId, timeSpent)
        }
      }
    }

    window.addEventListener('beforeunload', sendTimeSpent)

    // Also send every 30 seconds for long sessions
    const interval = setInterval(() => {
      if (isActive) {
        const timeSpent = Math.round((Date.now() - startTime) / 1000)
        trackTimeSpent(menuId, timeSpent)
      }
    }, 30000)

    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange)
      window.removeEventListener('beforeunload', sendTimeSpent)
      clearInterval(interval)
      sendTimeSpent()
    }
  }

  return {
    trackMenuView,
    trackCategoryView,
    trackItemView,
    trackTimeSpent,
    trackScrollDepth,
    trackSocialShare,
    trackContactAction,
    trackMenuSearch,
    setupCategoryTracking,
    setupScrollTracking,
    setupTimeTracking,
    getDeviceType
  }
}

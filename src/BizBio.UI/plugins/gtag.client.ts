export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  const gaId = config.public.googleAnalyticsId as string

  if (!gaId || gaId === 'G-XXXXXXXXXX') {
    console.warn('Google Analytics ID not configured')
    return {
      provide: {
        gtag: () => {},
        gtagEvent: () => {},
        gtagPageView: () => {}
      }
    }
  }

  // Load Google Analytics script
  if (process.client) {
    // Add gtag script
    const script = document.createElement('script')
    script.async = true
    script.src = `https://www.googletagmanager.com/gtag/js?id=${gaId}`
    document.head.appendChild(script)

    // Initialize gtag
    window.dataLayer = window.dataLayer || []
    function gtag(...args: any[]) {
      window.dataLayer.push(args)
    }
    
    gtag('js', new Date())
    gtag('config', gaId, {
      send_page_view: false, // We'll manually track page views
      cookie_flags: 'SameSite=None;Secure',
      anonymize_ip: true // GDPR compliance
    })

    // Track page views on route change
    nuxtApp.hook('page:finish', () => {
      const route = useRoute()
      gtag('event', 'page_view', {
        page_title: document.title,
        page_location: window.location.href,
        page_path: route.fullPath
      })
    })

    // Helper functions
    const gtagEvent = (
      eventName: string,
      eventParams?: {
        event_category?: string
        event_label?: string
        value?: number
        [key: string]: any
      }
    ) => {
      gtag('event', eventName, eventParams)
    }

    const gtagPageView = (pagePath?: string, pageTitle?: string) => {
      gtag('event', 'page_view', {
        page_path: pagePath || window.location.pathname,
        page_title: pageTitle || document.title,
        page_location: window.location.href
      })
    }

    return {
      provide: {
        gtag,
        gtagEvent,
        gtagPageView
      }
    }
  }

  // Return no-op functions for SSR
  return {
    provide: {
      gtag: () => {},
      gtagEvent: () => {},
      gtagPageView: () => {}
    }
  }
})

// Type augmentation for window
declare global {
  interface Window {
    dataLayer: any[]
  }
}

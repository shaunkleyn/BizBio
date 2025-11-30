export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  const connectionString = config.public.appInsightsConnectionString as string

  if (!connectionString || connectionString === '') {
    console.warn('Application Insights connection string not configured')
    return {
      provide: {
        appInsights: null,
        trackEvent: () => {},
        trackPageView: () => {},
        trackException: () => {},
        trackTrace: () => {},
        trackDependency: () => {}
      }
    }
  }

  // Dynamic import to avoid SSR bundling issues
  let appInsights: any = null

  // Helper functions that will work once appInsights is loaded
  const trackEvent = (name: string, properties?: Record<string, any>) => {
    if (appInsights) {
      appInsights.trackEvent({ name }, properties)
    }
  }

  const trackPageView = (name?: string, uri?: string, properties?: Record<string, any>) => {
    if (appInsights) {
      appInsights.trackPageView({ name, uri }, properties)
    }
  }

  const trackException = (error: Error, properties?: Record<string, any>) => {
    if (appInsights) {
      appInsights.trackException({
        exception: error,
        severityLevel: 3,
        properties
      })
    }
  }

  const trackTrace = (message: string, properties?: Record<string, any>, severityLevel: number = 1) => {
    if (appInsights) {
      appInsights.trackTrace({ message, severityLevel }, properties)
    }
  }

  const trackDependency = (
    id: string,
    method: string,
    absoluteUrl: string,
    duration: number,
    success: boolean,
    resultCode: number,
    properties?: Record<string, any>
  ) => {
    if (appInsights) {
      appInsights.trackDependencyData({
        id,
        name: absoluteUrl,
        duration,
        success,
        responseCode: resultCode,
        properties
      })
    }
  }

  // Initialize Application Insights dynamically
  import('@microsoft/applicationinsights-web').then(({ ApplicationInsights }) => {
    appInsights = new ApplicationInsights({
      config: {
        connectionString,
        enableAutoRouteTracking: true,
        enableCorsCorrelation: true,
        enableRequestHeaderTracking: true,
        enableResponseHeaderTracking: true,
        enableAjaxErrorStatusText: true,
        enableUnhandledPromiseRejectionTracking: true,
        disableFetchTracking: false,
        disableAjaxTracking: false,
        autoTrackPageVisitTime: true,
        
        maxBatchSizeInBytes: 10000,
        maxBatchInterval: 15000,
        disableExceptionTracking: false,
        
        enableDebug: false,
        loggingLevelConsole: 0,
        loggingLevelTelemetry: 1,
        
        isCookieUseDisabled: false,
        cookieDomain: config.public.cookieDomain || undefined,
      }
    })

    appInsights.loadAppInsights()

    // Set authenticated user context if available
    try {
      const authStore = useAuthStore()
      if (authStore.isAuthenticated && authStore.user) {
        appInsights.setAuthenticatedUserContext(
          authStore.user.id?.toString() || 'unknown',
          authStore.user.email || undefined,
          true
        )
      }

      // Watch for auth changes
      watch(() => authStore.isAuthenticated, (isAuth: boolean) => {
        if (isAuth && authStore.user) {
          appInsights.setAuthenticatedUserContext(
            authStore.user.id?.toString() || 'unknown',
            authStore.user.email || undefined,
            true
          )
        } else {
          appInsights.clearAuthenticatedUserContext()
        }
      })
    } catch (err) {
      console.debug('Auth store not available for tracking')
    }

    // Track page views on route change
    nuxtApp.hook('page:finish', () => {
      const route = useRoute()
      if (appInsights) {
        appInsights.trackPageView({
          name: route.name?.toString() || 'Unknown',
          uri: route.fullPath
        })
      }
    })

    // Global error handler
    nuxtApp.hook('vue:error', (error: any, instance: any, info: any) => {
      if (appInsights) {
        appInsights.trackException({
          exception: error as Error,
          severityLevel: 3,
          properties: {
            component: instance?.$options?.name || 'Unknown',
            info,
            route: useRoute().fullPath
          }
        })
      }
    })

    // Track unhandled errors
    if (process.client) {
      window.addEventListener('error', (event) => {
        if (appInsights) {
          appInsights.trackException({
            exception: event.error || new Error(event.message),
            severityLevel: 3,
            properties: {
              type: 'unhandled-error',
              filename: event.filename,
              lineno: event.lineno,
              colno: event.colno
            }
          })
        }
      })

      window.addEventListener('unhandledrejection', (event) => {
        if (appInsights) {
          appInsights.trackException({
            exception: event.reason instanceof Error ? event.reason : new Error(String(event.reason)),
            severityLevel: 3,
            properties: {
              type: 'unhandled-promise-rejection'
            }
          })
        }
      })
    }
  }).catch((err) => {
    console.error('Failed to load Application Insights:', err)
  })

  return {
    provide: {
      appInsights,
      trackEvent,
      trackPageView,
      trackException,
      trackTrace,
      trackDependency
    }
  }
})

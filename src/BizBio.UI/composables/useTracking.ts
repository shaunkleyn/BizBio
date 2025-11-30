/**
 * Composable for Application Insights tracking
 * Provides easy access to tracking methods throughout the application
 */
export const useTracking = () => {
  const { $appInsights, $trackEvent, $trackException, $trackTrace, $trackDependency } = useNuxtApp()
  const { $gtagEvent } = useNuxtApp()

  /**
   * Track a custom event
   * Sends to both Application Insights and Google Analytics
   */
  const trackEvent = (
    name: string,
    properties?: Record<string, any>,
    category?: string,
    label?: string,
    value?: number
  ) => {
    // Application Insights
    if ($trackEvent) {
      $trackEvent(name, properties)
    }

    // Google Analytics
    if ($gtagEvent) {
      $gtagEvent(name, {
        event_category: category,
        event_label: label,
        value,
        ...properties
      })
    }
  }

  /**
   * Track page view
   * Usually automatic but useful for SPA route changes
   */
  const trackPageView = (pagePath?: string, pageTitle?: string, properties?: Record<string, any>) => {
    if ($appInsights && typeof ($appInsights as any).trackPageView === 'function') {
      ($appInsights as any).trackPageView({
        name: pageTitle || document.title,
        uri: pagePath || window.location.pathname,
        properties
      })
    }

    if ($gtagEvent) {
      $gtagEvent('page_view', {
        page_path: pagePath || window.location.pathname,
        page_title: pageTitle || document.title,
        page_location: window.location.href
      })
    }
  }

  /**
   * Track an exception/error
   */
  const trackException = (error: Error, properties?: Record<string, any>) => {
    if ($trackException) {
      $trackException(error, properties)
    }

    // Also send to GA as an event
    if ($gtagEvent) {
      $gtagEvent('exception', {
        description: error.message,
        fatal: false,
        ...properties
      })
    }
  }

  /**
   * Track a trace/log message
   */
  const trackTrace = (message: string, properties?: Record<string, any>, severityLevel: number = 1) => {
    if ($trackTrace) {
      $trackTrace(message, properties, severityLevel)
    }
  }

  /**
   * Track a dependency (API call, database query, etc.)
   */
  const trackDependency = (
    id: string,
    method: string,
    absoluteUrl: string,
    duration: number,
    success: boolean,
    resultCode: number,
    properties?: Record<string, any>
  ) => {
    if ($trackDependency) {
      $trackDependency(id, method, absoluteUrl, duration, success, resultCode, properties)
    }
  }

  /**
   * Track user interactions
   */
  const trackUserAction = (action: string, target: string, properties?: Record<string, any>) => {
    trackEvent('user_action', {
      action,
      target,
      ...properties
    }, 'User Interaction', `${action}_${target}`)
  }

  /**
   * Track form submissions
   */
  const trackFormSubmission = (formName: string, success: boolean, properties?: Record<string, any>) => {
    trackEvent('form_submission', {
      form_name: formName,
      success,
      ...properties
    }, 'Form', formName, success ? 1 : 0)
  }

  /**
   * Track authentication events
   */
  const trackAuth = (action: 'login' | 'logout' | 'register' | 'password_reset', method?: string, success: boolean = true) => {
    trackEvent('auth_event', {
      action,
      method,
      success
    }, 'Authentication', action, success ? 1 : 0)
  }

  /**
   * Track search events
   */
  const trackSearch = (searchTerm: string, resultsCount?: number, filters?: Record<string, any>) => {
    trackEvent('search', {
      search_term: searchTerm,
      results_count: resultsCount,
      filters
    }, 'Search', searchTerm, resultsCount)
  }

  /**
   * Track e-commerce/business interactions
   */
  const trackBusinessView = (businessId: string, businessName: string, category?: string) => {
    trackEvent('view_business', {
      business_id: businessId,
      business_name: businessName,
      category
    }, 'Business', 'View')
  }

  const trackBusinessContact = (businessId: string, contactMethod: 'email' | 'phone' | 'website' | 'social') => {
    trackEvent('contact_business', {
      business_id: businessId,
      contact_method: contactMethod
    }, 'Business', 'Contact')
  }

  /**
   * Track performance metrics
   */
  const trackPerformance = (metricName: string, value: number, unit: string = 'ms') => {
    trackEvent('performance_metric', {
      metric_name: metricName,
      value,
      unit
    }, 'Performance', metricName, value)
  }

  /**
   * Track API errors
   */
  const trackApiError = (endpoint: string, statusCode: number, errorMessage: string, duration?: number) => {
    trackEvent('api_error', {
      endpoint,
      status_code: statusCode,
      error_message: errorMessage,
      duration
    }, 'API', 'Error', statusCode)

    // Also track as exception
    trackException(new Error(`API Error: ${endpoint} - ${errorMessage}`), {
      endpoint,
      statusCode,
      duration
    })
  }

  return {
    trackEvent,
    trackPageView,
    trackException,
    trackTrace,
    trackDependency,
    trackUserAction,
    trackFormSubmission,
    trackAuth,
    trackSearch,
    trackBusinessView,
    trackBusinessContact,
    trackPerformance,
    trackApiError
  }
}

/**
 * Cookie Consent Management Composable
 * Handles consent for functional and analytics cookies
 * GDPR/POPIA compliant
 */

export interface CookieConsent {
  functional: boolean  // Required for app to work
  analytics: boolean   // Optional - device, browser, location data
  timestamp: string
  version: string      // Track consent version for future changes
}

const CONSENT_KEY = 'bizbio_cookie_consent'
const CONSENT_VERSION = '1.0'

export const useCookieConsent = () => {
  const consentGiven = useState<CookieConsent | null>('cookieConsent', () => null)
  const showBanner = useState('showCookieBanner', () => false)

  /**
   * Initialize consent state from localStorage
   */
  const initConsent = () => {
    if (typeof window === 'undefined') return

    const saved = localStorage.getItem(CONSENT_KEY)
    if (saved) {
      try {
        const consent = JSON.parse(saved) as CookieConsent

        // Check if consent version matches
        if (consent.version === CONSENT_VERSION) {
          consentGiven.value = consent
          showBanner.value = false
        } else {
          // Version changed - need new consent
          showBanner.value = true
        }
      } catch (error) {
        console.error('Error parsing cookie consent:', error)
        showBanner.value = true
      }
    } else {
      // No consent saved - show banner
      showBanner.value = true
    }
  }

  /**
   * Save consent preferences
   */
  const saveConsent = (functional: boolean, analytics: boolean) => {
    const consent: CookieConsent = {
      functional,
      analytics,
      timestamp: new Date().toISOString(),
      version: CONSENT_VERSION
    }

    consentGiven.value = consent
    showBanner.value = false

    if (typeof window !== 'undefined') {
      localStorage.setItem(CONSENT_KEY, JSON.stringify(consent))
    }
  }

  /**
   * Accept all cookies
   */
  const acceptAll = () => {
    saveConsent(true, true)
  }

  /**
   * Accept only functional (necessary) cookies
   */
  const acceptNecessary = () => {
    saveConsent(true, false)
  }

  /**
   * Reject optional cookies (keep only functional)
   */
  const rejectOptional = () => {
    saveConsent(true, false)
  }

  /**
   * Check if we can use functional storage (always true if consent given)
   */
  const canUseFunctional = computed(() => {
    return consentGiven.value?.functional ?? false
  })

  /**
   * Check if we can use analytics
   */
  const canUseAnalytics = computed(() => {
    return consentGiven.value?.analytics ?? false
  })

  /**
   * Check if consent has been given (either choice)
   */
  const hasConsent = computed(() => {
    return consentGiven.value !== null
  })

  /**
   * Revoke consent and clear data
   */
  const revokeConsent = () => {
    if (typeof window !== 'undefined') {
      localStorage.removeItem(CONSENT_KEY)

      // Clear analytics data if user revokes
      if (!canUseAnalytics.value) {
        // Clear any analytics-related localStorage items
        const keysToRemove = []
        for (let i = 0; i < localStorage.length; i++) {
          const key = localStorage.key(i)
          if (key && (key.startsWith('_ga') || key.startsWith('analytics_'))) {
            keysToRemove.push(key)
          }
        }
        keysToRemove.forEach(key => localStorage.removeItem(key))
      }
    }

    consentGiven.value = null
    showBanner.value = true
  }

  /**
   * Safe localStorage setter - only stores if consent given
   */
  const setLocalStorage = (key: string, value: string, requiresAnalytics = false) => {
    if (typeof window === 'undefined') return false

    // Check appropriate consent
    if (requiresAnalytics && !canUseAnalytics.value) {
      console.warn(`Cannot store ${key} - analytics consent not given`)
      return false
    }

    if (!requiresAnalytics && !canUseFunctional.value) {
      console.warn(`Cannot store ${key} - functional consent not given`)
      return false
    }

    try {
      localStorage.setItem(key, value)
      return true
    } catch (error) {
      console.error('Error storing data:', error)
      return false
    }
  }

  /**
   * Safe localStorage getter
   */
  const getLocalStorage = (key: string, requiresAnalytics = false): string | null => {
    if (typeof window === 'undefined') return null

    // Check appropriate consent
    if (requiresAnalytics && !canUseAnalytics.value) {
      return null
    }

    if (!requiresAnalytics && !canUseFunctional.value) {
      return null
    }

    return localStorage.getItem(key)
  }

  return {
    consentGiven,
    showBanner,
    canUseFunctional,
    canUseAnalytics,
    hasConsent,
    initConsent,
    saveConsent,
    acceptAll,
    acceptNecessary,
    rejectOptional,
    revokeConsent,
    setLocalStorage,
    getLocalStorage
  }
}

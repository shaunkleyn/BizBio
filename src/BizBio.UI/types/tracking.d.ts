import type { ApplicationInsights } from '@microsoft/applicationinsights-web'

declare module '#app' {
  interface NuxtApp {
    $appInsights: ApplicationInsights | null
    $trackEvent: (name: string, properties?: Record<string, any>) => void
    $trackPageView: (name?: string, uri?: string, properties?: Record<string, any>) => void
    $trackException: (error: Error, properties?: Record<string, any>) => void
    $trackTrace: (message: string, properties?: Record<string, any>, severityLevel?: number) => void
    $trackDependency: (
      id: string,
      method: string,
      absoluteUrl: string,
      duration: number,
      success: boolean,
      resultCode: number,
      properties?: Record<string, any>
    ) => void
    $gtag: (...args: any[]) => void
    $gtagEvent: (eventName: string, eventParams?: Record<string, any>) => void
    $gtagPageView: (pagePath?: string, pageTitle?: string) => void
  }
}

declare module 'vue' {
  interface ComponentCustomProperties {
    $appInsights: ApplicationInsights | null
    $trackEvent: (name: string, properties?: Record<string, any>) => void
    $trackPageView: (name?: string, uri?: string, properties?: Record<string, any>) => void
    $trackException: (error: Error, properties?: Record<string, any>) => void
    $trackTrace: (message: string, properties?: Record<string, any>, severityLevel?: number) => void
    $trackDependency: (
      id: string,
      method: string,
      absoluteUrl: string,
      duration: number,
      success: boolean,
      resultCode: number,
      properties?: Record<string, any>
    ) => void
    $gtag: (...args: any[]) => void
    $gtagEvent: (eventName: string, eventParams?: Record<string, any>) => void
    $gtagPageView: (pagePath?: string, pageTitle?: string) => void
  }
}

// Extend axios config to include metadata
declare module 'axios' {
  export interface InternalAxiosRequestConfig {
    metadata?: {
      startTime: number
    }
  }
}

export {}

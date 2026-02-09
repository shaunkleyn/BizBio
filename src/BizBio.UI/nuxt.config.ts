import { defineNuxtConfig } from "nuxt/config";

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-11-23',
  devtools: {
    enabled: false,
  },
  ssr: true,
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@nuxt/image',
  ],
  css: ['~/assets/css/main.css', '~/assets/fonts/css/all.css'],
  app: {
    head: {
      title: 'BizBio - Digital Business Profiles',
      meta: [
        {
          charset: 'utf-8',
        },
        {
          name: 'viewport',
          content: 'width=device-width, initial-scale=1',
        },
        {
          name: 'description',
          content: 'Create and share your digital business profile with BizBio',
        },
      ],
      link: [
        {
          rel: 'stylesheet',
          href: 'https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&family=Open+Sans:wght@300;400;500;600;700&display=swap',
        },
        {
          rel: 'stylesheet',
          href: 'https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Open+Sans:ital,wght@0,300..800;1,300..800&display=swap',
        },
        {
          rel: 'apple-touch-icon',
          sizes: '180x180',
          href: '/apple-touch-icon.png',
        },
        {
          rel: 'icon',
          type: 'image/png',
          sizes: '32x32',
          href: '/favicon-32x32.png',
        },
        {
          rel: 'icon',
          type: 'image/png',
          sizes: '16x16',
          href: '/favicon-16x16.png',
        },
        {
          rel: 'manifest',
          href: '/site.webmanifest',
        },
      ],
    },
  },
  runtimeConfig: {
    public: {
      apiUrl:
        process.env.NUXT_PUBLIC_API_URL || 'https://localhost:5001/api/v1',
      appInsightsConnectionString:
        process.env.NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING || '',
      googleAnalyticsId:
        process.env.NUXT_PUBLIC_GOOGLE_ANALYTICS_ID || 'G-XXXXXXXXXX',
      cookieDomain: process.env.NUXT_PUBLIC_COOKIE_DOMAIN || undefined,
    },
  },
  nitro: {
    compressPublicAssets: true,
    preset: 'node-server',
    minify: false,
    sourceMap: false,
    experimental: {
      wasm: false,
    },
  },
  image: {
    quality: 80,
    formats: ['webp', 'avif', 'png', 'jpg'],
    screens: {
      xs: 320,
      sm: 640,
      md: 768,
      lg: 1024,
      xl: 1280,
      xxl: 1536,
    },
  },
  routeRules: {
    '/.well-known/**': { ssr: false, redirect: '/' },
    '/': {
      prerender: true,
    },
    '/pricing': {
      prerender: true,
    },
    '/categories': {
      prerender: true,
    },
    '/help': {
      prerender: true,
    },
    '/contact': {
      prerender: true,
    },
    '/terms': {
      prerender: true,
    },
    '/privacy': {
      prerender: true,
    },
    '/dashboard/**': {
      ssr: false,
    },
    '/login': {
      ssr: true,
    },
    '/register': {
      ssr: true,
    },
    '/forgot-password': {
      ssr: true,
    },
    '/reset-password': {
      ssr: true,
    },
    '/verify-email': {
      ssr: true,
    },
    '/search': {
      ssr: true,
      cache: {
        maxAge: 60 * 10,
      },
    },
    '/products': {
      ssr: true,
      cache: {
        maxAge: 60 * 15,
      },
    },
  },
  // ❌ REMOVED: darkMode: 'class' - this is NOT a valid Nuxt option!
  experimental: {
    payloadExtraction: false,
    renderJsonPayloads: true,
    typedPages: true,
  },
  router: {
    options: {
      strict: false
    }
  },
  vite: {
    server: {
      hmr: {
        protocol: 'ws',
        host: 'localhost',
      },
      watch: {
        usePolling: false,
        ignored: ['**/node_modules/**', '**/.git/**', '**/.nuxt/**', '**/dist/**']
      }
    },
    clearScreen: false,
    optimizeDeps: {
      exclude: ['vue', 'pinia']
    },
    build: {
      rollupOptions: {
        output: {
          manualChunks: (id: string | string[]) => {
            if (id.includes('node_modules')) {
              if (id.includes('pinia')) {
                return 'pinia';
              }
            }
          },
        },
      },
    },
  },
  // Improve dev server stability
  devServer: {
    port: 3000
  },
  hooks: {
    // Prevent duplicate middleware registration
    'nitro:init': () => {},
  },
});
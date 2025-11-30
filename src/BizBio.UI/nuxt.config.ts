// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-11-23',

  devtools: { enabled: true },

  // SSR Configuration - enabled by default but explicit for clarity
  ssr: true,

  modules: [
    '~/modules/fix-windows-path',
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@nuxt/image'
  ],

  css: [
   // '@fortawesome/fontawesome-free/css/all.min.css',
    '~/assets/css/main.css',
    '~/assets/fonts/css/all.css',

  ],

  app: {
    head: {
      title: 'BizBio - Digital Business Profiles',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        {
          name: 'description',
          content: 'Create and share your digital business profile with BizBio'
        }
      ],
      link: [
        {
          rel: 'stylesheet',
          href: 'https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&family=Open+Sans:wght@300;400;500;600;700&display=swap'
        },
      ]
    }
  },

  runtimeConfig: {
    public: {
      apiUrl: process.env.NUXT_PUBLIC_API_URL || 'http://localhost:5000/api',
      appInsightsConnectionString: process.env.NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING || '',
      googleAnalyticsId: process.env.NUXT_PUBLIC_GOOGLE_ANALYTICS_ID || 'G-XXXXXXXXXX',
      cookieDomain: process.env.NUXT_PUBLIC_COOKIE_DOMAIN || undefined
    }
  },

  // Performance optimizations
  nitro: {
    compressPublicAssets: true,
    preset: 'node-server', // Optimized for Node.js server deployment
    
    // Production optimizations
    // Note: minify disabled due to Rollup bug on Windows with Nuxt 3.20.1
    // Re-enable after upgrading to a fixed version
    minify: false,
    sourceMap: false,
    
    // Server configuration for VPS deployment
    experimental: {
      wasm: false // Disable if not needed to reduce memory
    }
  },

  // Image optimization
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

  // Route rules for better performance
  routeRules: {
    // Static pages - prerendered at build time
    '/': { prerender: true },
    '/pricing': { prerender: true },
    '/categories': { prerender: true },
    '/help': { prerender: true },
    '/contact': { prerender: true },
    '/terms': { prerender: true },
    '/privacy': { prerender: true },
    
    // API routes - never cached
    '/api/**': { cors: true, headers: { 'cache-control': 'no-cache' } },
    
    // Dashboard - SSR with short cache
    '/dashboard/**': { ssr: true, cache: { maxAge: 60 * 5 } }, // 5 minutes
    
    // Auth pages - SSR only, no cache
    '/login': { ssr: true },
    '/register': { ssr: true },
    '/forgot-password': { ssr: true },
    '/reset-password': { ssr: true },
    '/verify-email': { ssr: true },
    
    // Search and products - SSR with cache
    '/search': { ssr: true, cache: { maxAge: 60 * 10 } }, // 10 minutes
    '/products': { ssr: true, cache: { maxAge: 60 * 15 } }, // 15 minutes
  },
  
  darkMode: 'class',

  // Production build optimizations
  experimental: {
    payloadExtraction: false, // Can help with performance on VPS
    renderJsonPayloads: true,
    typedPages: true,
  },

  // Optimize builds for production
  vite: {
    build: {
      rollupOptions: {
        output: {
          manualChunks: (id) => {
            // Only split chunks for client-side modules
            if (id.includes('node_modules')) {
              if (id.includes('pinia')) return 'pinia';
              // Skip axios as it's external on server-side
            }
          }
        }
      }
    }
  },
})

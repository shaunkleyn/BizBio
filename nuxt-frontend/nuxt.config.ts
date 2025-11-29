// https://nuxt.com/docs/api/configuration/nuxt-config
// nuxt.config.ts
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  // 1. Rendering Mode (Kept as client-side only based on your ssr: false)
  ssr: false,

  // 2. Modules (Combined buildModules and modules)
  modules: [
    '@nuxtjs/tailwindcss', // Check documentation for specific v3 installation steps
    '@vite-pwa/nuxt',
    '@pinia/nuxt',
    '@nuxtjs/tailwindcss',
    '@nuxtjs/pwa'
    // Install a Nuxt 3 PWA module if you need that functionality
  ],

  // 3. Global Head (Meta Tag, Link, Script, Title)
  app: {
    head: {
      title: 'EnBizCard - An Open-Source Digital Business Card Generator', // Default title
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',

      // 1. Meta Tags (Your old 'meta' array)
      meta: [
        {
          name: 'viewport',
          content: 'width=device-width, initial-scale=1'
        },
        {
          charset: 'utf-8'
        },
        { name: 'theme-color', content: '#111827' }, // Theme color is a meta tag
        { name: 'author', content: 'Vishnu Raghav' },
        
        // Open Graph (OG) tags: must use 'property' or the dedicated useSeoMeta
        { property: 'og:type', content: 'website' },
        { property: 'og:title', content: 'EnBizCard - An Open-Source Digital Business Card Generator' },
        { property: 'og:description', content: 'EnBizCard helps you create...' },
        { property: 'og:image', content: '/maskable_512.png' },
      ],

      // 2. Link Tags (Your old 'link' array)
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico?v=2' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/apple-touch-icon.png' },
        // ... (include all other link tags, including the manifest link if not using a PWA module)
      ],

      // 3. Script Tags
      script: [
        { src: '/qrcode.min.js', tagPosition: 'bodyClose' }, // Use tagPosition for scripts
      ],
    }
  },

  // 4. Component Auto-Importing
  components: true,
  // Global CSS/Libraries
  css: [
    // Include bootstrap CSS globally
    'bootstrap/dist/css/bootstrap.min.css', 
    // Include the Font Awesome CSS (if you use it via CSS)
    '@fortawesome/fontawesome-svg-core/styles.css',
  ],
  pwa: {
    manifest: {
      name: 'EnBizCard - An Open-Source Digital Business Card Generator',
      short_name: 'EnBizCard',
      description: 'EnBizCard helps you create...',
      theme_color: '#111827',
      display: 'standalone', // Your existing setting
      lang: 'en',
      
      // Icons array: IMPORTANT! Icons must be in the public/ folder.
      icons: [
        {
          src: '/icon_192.png',
          sizes: '192x192',
          type: 'image/png',
        },
        // ... (map all your icon entries here)
        {
          src: '/maskable_512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'maskable', // Include the 'purpose' property
        },
      ],
    },
    // Optional: Enable PWA in development mode for easier testing
    devOptions: {
      enabled: true,
      type: 'module'
    },
    // Optional: Configure caching strategies using Workbox options
    // workbox: { ... } 
  }

  // 5. Build/Vite Overrides (If needed)
  // build: { ... } logic should be replaced by the 'vite' property if necessary
  // vite: {
  //   // Custom Vite configuration goes here
  // }
  
  // 6. Telemetry is false by default in Nuxt 3 if not specified
})
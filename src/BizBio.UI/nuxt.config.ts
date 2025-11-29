// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-11-23',

  devtools: { enabled: true },

  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@nuxt/image'
  ],

  css: [
    '@fortawesome/fontawesome-free/css/all.min.css',
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
        }
      ]
    }
  },

  runtimeConfig: {
    public: {
      apiUrl: process.env.NUXT_PUBLIC_API_URL || 'http://localhost:5000/api'
    }
  },

  // Performance optimizations
  nitro: {
    compressPublicAssets: true,
  },

  // Image optimization
  image: {
    quality: 80,
    formats: ['webp', 'avif', 'png', 'jpg']
  },

  // Route rules for better performance
  routeRules: {
    '/': { prerender: true },
    '/pricing': { prerender: true },
    '/categories': { prerender: true },
    '/help': { prerender: true },
    '/contact': { prerender: true },
    '/terms': { prerender: true },
    '/privacy': { prerender: true },
  }
})

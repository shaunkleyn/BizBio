/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './components/**/*.{js,vue,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './plugins/**/*.{js,ts}',
    './app.vue',
    './error.vue',
  ],
  theme: {
    extend: {
      colors: {
        // Vibrant Color System
        'md-primary': 'var(--md-primary)',
        'md-on-primary': 'var(--md-on-primary)',
        'md-primary-container': 'var(--md-primary-container)',
        'md-on-primary-container': 'var(--md-on-primary-container)',
        'md-secondary': 'var(--md-secondary)',
        'md-on-secondary': 'var(--md-on-secondary)',
        'md-secondary-container': 'var(--md-secondary-container)',
        'md-on-secondary-container': 'var(--md-on-secondary-container)',
        'md-tertiary': 'var(--md-tertiary)',
        'md-on-tertiary': 'var(--md-on-tertiary)',
        'md-tertiary-container': 'var(--md-tertiary-container)',
        'md-on-tertiary-container': 'var(--md-on-tertiary-container)',
        'md-accent': 'var(--md-accent)',
        'md-on-accent': 'var(--md-on-accent)',
        'md-accent-container': 'var(--md-accent-container)',
        'md-on-accent-container': 'var(--md-on-accent-container)',
        'md-error': 'var(--md-error)',
        'md-on-error': 'var(--md-on-error)',
        'md-error-container': 'var(--md-error-container)',
        'md-on-error-container': 'var(--md-on-error-container)',
        'md-success': 'var(--md-success)',
        'md-on-success': 'var(--md-on-success)',
        'md-success-container': 'var(--md-success-container)',
        'md-on-success-container': 'var(--md-on-success-container)',
        'md-background': 'var(--md-background)',
        'md-on-background': 'var(--md-on-background)',
        'md-surface': 'var(--md-surface)',
        'md-on-surface': 'var(--md-on-surface)',
        'md-surface-variant': 'var(--md-surface-variant)',
        'md-on-surface-variant': 'var(--md-on-surface-variant)',
        'md-outline': 'var(--md-outline)',
        'md-outline-variant': 'var(--md-outline-variant)',
        'md-surface-container-lowest': 'var(--md-surface-container-lowest)',
        'md-surface-container-low': 'var(--md-surface-container-low)',
        'md-surface-container': 'var(--md-surface-container)',
        'md-surface-container-high': 'var(--md-surface-container-high)',
        'md-surface-container-highest': 'var(--md-surface-container-highest)',
        
        // Legacy support
        primary: 'var(--primary-color)',
        secondary: 'var(--secondary-color)',
        accent: {
          DEFAULT: 'var(--md-error)',
          2: 'var(--md-secondary)',
          3: 'var(--md-tertiary)',
          4: 'var(--md-accent)'
        },
        'dark-text': 'var(--dark-text-color)',
        'gray-text': 'var(--gray-text-color)',
        'light-text': 'var(--light-text-color)',
        'dark-bg': 'var(--dark-background-color)',
        'light-bg': 'var(--light-background-color)',
        'medium-bg': 'var(--medium-background-color)',
        'dark-border': 'var(--dark-border-color)',
        'light-border': 'var(--light-border-color)'
      },
      fontFamily: {
        heading: ['Montserrat', 'sans-serif'],
        body: ['Open Sans', 'sans-serif']
      },
      borderRadius: {
        'button': '12px',
        'md-sm': '8px',
        'md-md': '12px',
        'md-lg': '16px',
        'md-xl': '24px',
        'md-2xl': '32px',
      },
      boxShadow: {
        'md-1': 'var(--md-elevation-1)',
        'md-2': 'var(--md-elevation-2)',
        'md-3': 'var(--md-elevation-3)',
        'md-4': 'var(--md-elevation-4)',
        'md-5': 'var(--md-elevation-5)',
        'glow-purple': '0 0 20px rgba(124, 58, 237, 0.4)',
        'glow-pink': '0 0 20px rgba(236, 72, 153, 0.4)',
        'glow-teal': '0 0 20px rgba(20, 184, 166, 0.4)',
      },
      backgroundImage: {
        'gradient-primary': 'var(--gradient-primary)',
        'gradient-secondary': 'var(--gradient-secondary)',
        'gradient-tertiary': 'var(--gradient-tertiary)',
        'gradient-accent': 'var(--gradient-accent)',
        'gradient-hero': 'var(--gradient-hero)',
      },
      keyframes: {
        fadeSlide: {
          '0%': { opacity: 0, transform: 'translateY(20px)' },
          '100%': { opacity: 1, transform: 'translateY(0)' },
        },
        shimmer: {
          '0%': { backgroundPosition: '-1000px 0' },
          '100%': { backgroundPosition: '1000px 0' },
        },
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        pulse: {
          '0%, 100%': { opacity: 1 },
          '50%': { opacity: 0.8 },
        },
        'scale-in': {
          '0%': { transform: 'scale(0.9)', opacity: 0 },
          '100%': { transform: 'scale(1)', opacity: 1 },
        },
      },
      animation: {
        fadeSlide: 'fadeSlide 0.6s cubic-bezier(0.4, 0, 0.2, 1) forwards',
        shimmer: 'shimmer 2s infinite linear',
        float: 'float 3s ease-in-out infinite',
        pulse: 'pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'scale-in': 'scale-in 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
      },
      
    },

  },
  darkMode: 'class',
  plugins: [],
}

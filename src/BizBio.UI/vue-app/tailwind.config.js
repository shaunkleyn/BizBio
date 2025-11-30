/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: 'rgb(162, 117, 234)',
          50: '#f5f3ff',
          100: '#ede9fe',
          200: '#ddd6fe',
          300: '#c4b5fd',
          400: '#a78bfa',
          500: 'rgb(162, 117, 234)',
          600: '#8b5cf6',
          700: '#7c3aed',
          800: '#6d28d9',
          900: '#5b21b6',
        },
        accent: {
          red: '#C0392B',
          purple: '#A569BD',
          teal: '#3FB6A8',
          yellow: '#F1C40F',
        },
        brand: {
          'dark-text': '#333333',
          'gray-text': '#777777',
          'light-bg': '#F5FAFF',
          'medium-bg': '#EAF2FB',
          'dark-bg': '#1C2833',
          'dark-border': '#4A6DA8',
          'light-border': '#CCD5E9',
        },
      },
      fontFamily: {
        sans: ['Open Sans', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
        heading: ['Montserrat', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
      },
      borderRadius: {
        'brand': '6px',
      },
    },
  },
  plugins: [],
}


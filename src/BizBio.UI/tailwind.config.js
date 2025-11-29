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
        primary: '#4A90E2',
        accent: {
          DEFAULT: '#C0392B',
          2: '#A569BD',
          3: '#3FB6A8',
          4: '#F1C40F'
        },
        'dark-text': '#333333',
        'gray-text': '#777777',
        'light-text': '#FFFFFF',
        'dark-bg': '#1C2833',
        'light-bg': '#F5FAFF',
        'medium-bg': '#EAF2FB',
        'dark-border': '#4A6DA8',
        'light-border': '#CCD5E9'
      },
      fontFamily: {
        heading: ['Montserrat', 'sans-serif'],
        body: ['Open Sans', 'sans-serif']
      },
      borderRadius: {
        'button': '6px'
      }
    },
  },
  plugins: [],
}

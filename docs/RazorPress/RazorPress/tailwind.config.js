/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  content: [
    './**/*.{html,cshtml,razor,cs}',
    './_pages/**/*.md',
  ],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: ['Satoshi', ...defaultTheme.fontFamily.sans],
      },
      spacing: {
        18: '4.5rem',
        112: '28rem',
        120: '30rem',
      },
      colors: {
        'accent-1': '#FAFAFA',
        'accent-2': '#EAEAEA',
        danger: 'rgb(153 27 27)',
        success: 'rgb(22 101 52)',
      },
    },
  },
  plugins: [],
}
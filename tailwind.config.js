/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,html}",
    "./wwwroot/**/*.{js,cshtml,html}"
  ],
  theme: {
    extend: {
      colors: {
        preto: '#051211',
        'verde-petroleo': '#0B2B28',
        'verde-floresta': '#1B4D46',
        'verde-agua': '#4A8B82',
        branco: '#FFFFFF',
      }
    },
  },
  plugins: [],
}
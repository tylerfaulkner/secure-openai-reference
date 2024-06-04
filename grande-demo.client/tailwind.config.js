/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
  theme: {
      extend: {
          colors: {
              "background": '#f2f2f3',
              "border-color": '#ACB6C8',
              'card-color': '#E4E5E7',
              'primary': '#4787C1'
          },
      },
  },
  plugins: [],
}


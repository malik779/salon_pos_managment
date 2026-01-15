module.exports = {
  content: ["./src/**/*.{html,ts}",
    "./src/styles/**/*.{scss,css}"  
  ],
  theme: {
    extend: {
      container: {
        center: true,
        padding: "1rem",
      },
    },
  },
  darkMode: ['class', '[data-theme="dark"]'],
};

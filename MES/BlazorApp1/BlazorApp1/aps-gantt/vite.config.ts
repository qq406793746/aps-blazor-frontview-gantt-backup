import { defineConfig } from "vite";

export default defineConfig({
  build: {
    outDir: "../wwwroot/aps-gantt",
    emptyOutDir: true,
    sourcemap: false,
    cssCodeSplit: false,
    lib: {
      entry: "src/main.ts",
      name: "ApsResourceGantt",
      formats: ["iife"],
      fileName: () => "aps-resource-gantt.js"
    },
    rollupOptions: {
      output: {
        assetFileNames: "aps-resource-gantt.[ext]"
      }
    }
  }
});

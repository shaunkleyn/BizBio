import { defineNuxtModule } from '@nuxt/kit';

export default defineNuxtModule({
  meta: {
    name: 'fix-windows-path',
  },
  setup(options, nuxt) {
    // Workaround for Windows paths with spaces causing issues in Nuxt components transform
    // The issue is in @nuxt/kit where it passes virtual module IDs to the ignore package
    // which expects relative file paths

    // Disable the components auto-import transform for virtual modules
    nuxt.hook('modules:done', () => {
      nuxt.options.components = nuxt.options.components || { dirs: [] };

      // Add a custom transformInclude filter to skip virtual modules
      nuxt.hook('vite:extendConfig', (config) => {
        if (!config.plugins) config.plugins = [];

        // Find and modify the nuxt:components:imports plugin
        const componentsIndex = config.plugins.findIndex((p: any) =>
          p && p.name === 'nuxt:components:imports'
        );

        if (componentsIndex !== -1) {
          const originalPlugin = config.plugins[componentsIndex] as any;

          // Wrap the transformInclude function to skip virtual modules
          if (originalPlugin && typeof originalPlugin.transformInclude === 'function') {
            const originalTransformInclude = originalPlugin.transformInclude;
            originalPlugin.transformInclude = function(id: string) {
              // Skip virtual modules that cause path issues
              if (
                id.includes(':') || // Virtual module prefixes
                id.startsWith('\0') || // Rollup virtual modules
                id.includes(' vite/') || // Malformed Vite paths
                id.includes(' plugin-') || // Malformed plugin paths
                id.startsWith('vite/') || // Vite internal modules
                id.startsWith('plugin-') // Plugin virtual modules
              ) {
                return false;
              }
              return originalTransformInclude.call(this, id);
            };
          }
        }
      });
    });
  },
});

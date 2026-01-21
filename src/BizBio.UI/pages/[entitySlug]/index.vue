<template>
  <div>
    <!-- This page redirects to the catalog viewer -->
    <!-- It handles /{entity_slug} format for single-catalog entities -->
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

definePageMeta({
  layout: false,
  middleware: []
})

const route = useRoute()
const router = useRouter()

onMounted(async () => {
  const entitySlug = route.params.entitySlug as string

  // Check if slug contains underscore (entity_catalog format)
  if (entitySlug.includes('_')) {
    const parts = entitySlug.split('_')
    if (parts.length === 2) {
      // Redirect to /{entitySlug}/{catalogSlug} format
      await router.push(`/${parts[0]}/${parts[1]}`)
      return
    }
  }

  // Otherwise, load default catalog for entity
  try {
    const api = useApi()
    const response = await api.get(`/api/v1/entities/by-slug/${entitySlug}`)

    if (response.success && response.data?.entity) {
      const entity = response.data.entity

      // Get catalogs for this entity
      const catalogsResponse = await api.get(`/api/v1/entities/${entity.id}/catalogs`)

      if (catalogsResponse.success && catalogsResponse.data?.catalogs) {
        const catalogs = catalogsResponse.data.catalogs.filter((c: any) => c.isActive)

        if (catalogs.length === 0) {
          // No active catalogs
          await router.push({ path: '/404', replace: true })
        } else if (catalogs.length === 1) {
          // Single catalog - redirect to it
          await router.push(`/${entitySlug}/${catalogs[0].slug}`)
        } else {
          // Multiple catalogs - redirect to first one (sorted by sortOrder)
          const sortedCatalogs = catalogs.sort((a: any, b: any) => a.sortOrder - b.sortOrder)
          await router.push(`/${entitySlug}/${sortedCatalogs[0].slug}`)
        }
      } else {
        await router.push({ path: '/404', replace: true })
      }
    } else {
      await router.push({ path: '/404', replace: true })
    }
  } catch (error) {
    console.error('Error loading entity:', error)
    await router.push({ path: '/404', replace: true })
  }
})
</script>

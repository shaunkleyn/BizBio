<script setup>
import { ref } from 'vue'

const searchQuery = ref('')
const searchResults = ref([])
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-heading font-bold text-brand-dark-text mb-4">
          Search Businesses
        </h1>
        <p class="text-xl text-brand-gray-text">
          Find professionals and businesses near you
        </p>
      </div>

      <div class="bg-white rounded-lg p-4 shadow mb-8">
        <div class="flex gap-4">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search by name, category, or location..."
            class="flex-1 px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary"
          />
          <button class="px-8 py-3 bg-primary text-white rounded-lg hover:bg-primary-600 transition-all shadow hover:shadow-lg font-semibold">
            <i class="fas fa-search mr-2"></i>
            Search
          </button>
        </div>
      </div>

      <div v-if="searchResults.length === 0" class="text-center py-12">
        <i class="fas fa-search text-gray-300 text-6xl mb-4"></i>
        <p class="text-brand-gray-text">Enter a search term to find businesses</p>
      </div>
    </div>
  </div>
</template>

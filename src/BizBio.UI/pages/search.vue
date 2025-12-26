<template>
  <div>
    <section class="bg-[var(--light-background-color)] py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Header -->
        <div class="text-center mb-12">
          <h1 class="text-4xl sm:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Search Businesses
          </h1>
          <p class="text-lg text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Find the perfect business or professional for your needs
          </p>
        </div>

        <!-- Search Bar -->
        <div class="max-w-3xl mx-auto mb-12">
          <div class="bg-white rounded-xl shadow-lg p-6">
            <div class="flex gap-4">
              <div class="flex-1">
                <div class="relative">
                  <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <i class="fas fa-search text-[var(--gray-text-color)]"></i>
                  </div>
                  <input
                    type="text"
                    v-model="searchQuery"
                    placeholder="Search businesses, services, or professionals..."
                    class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-md-primary transition-colors"
                    @keyup.enter="handleSearch"
                  />
                </div>
              </div>
              <button
                @click="handleSearch"
                class="bg-md-primary text-white px-8 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
              >
                Search
              </button>
            </div>
          </div>
        </div>

        <!-- Results or Empty State -->
        <div v-if="searching" class="text-center py-12">
          <i class="fas fa-spinner fa-spin text-4xl text-md-primary"></i>
          <p class="text-[var(--gray-text-color)] mt-4">Searching...</p>
        </div>

        <div v-else-if="results.length === 0 && hasSearched" class="text-center py-12">
          <i class="fas fa-search text-6xl text-[var(--gray-text-color)] opacity-50 mb-4"></i>
          <p class="text-xl text-[var(--gray-text-color)] mb-2">No results found</p>
          <p class="text-[var(--gray-text-color)]">Try adjusting your search terms</p>
        </div>

        <div v-else-if="!hasSearched" class="text-center py-12">
          <i class="fas fa-search text-6xl text-[var(--gray-text-color)] opacity-50 mb-4"></i>
          <p class="text-xl text-[var(--dark-text-color)] mb-2">Start searching</p>
          <p class="text-[var(--gray-text-color)]">Enter a business name, service, or category above</p>
        </div>

        <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          <!-- Result cards would go here -->
          <div v-for="result in results" :key="result.id" class="bg-white rounded-xl p-6 shadow-lg">
            <h3 class="text-xl font-bold text-[var(--dark-text-color)] mb-2">{{ result.name }}</h3>
            <p class="text-[var(--gray-text-color)] mb-4">{{ result.description }}</p>
            <NuxtLink
              :to="`/${result.slug}`"
              class="text-md-primary hover:underline font-semibold"
            >
              View Profile →
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const searchQuery = ref('')
const searching = ref(false)
const hasSearched = ref(false)
const results = ref([])

const handleSearch = async () => {
  if (!searchQuery.value.trim()) return

  searching.value = true
  hasSearched.value = true

  // Simulate API call
  setTimeout(() => {
    results.value = []
    searching.value = false
  }, 1000)
}

useHead({
  title: 'Search Businesses',
})
</script>


<template>
  <div>
    <section class="bg-[var(--light-background-color)] py-16 sm:py-20 lg:py-24 min-h-screen">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Header -->
        <div class="text-center mb-16">
          <h1 class="text-4xl sm:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Business Categories
          </h1>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto">
            Browse businesses by category and connect with professionals in your area.
          </p>
        </div>

        <!-- Categories Grid -->
        <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <!-- Category Card -->
          <NuxtLink
            v-for="category in categories"
            :key="category.name"
            :to="`/search?category=${category.slug}`"
            class="bg-white rounded-xl p-8 shadow-lg hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 group"
          >
            <div class="flex items-center gap-4 mb-4">
              <div :class="`bg-[var(--${category.color})] bg-opacity-10 rounded-full p-4`">
                <i :class="`fas ${category.icon} text-[var(--${category.color})] text-3xl`"></i>
              </div>
              <div class="flex-1">
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] group-hover:text-[var(--primary-color)] transition-colors">
                  {{ category.name }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  {{ category.count }} businesses
                </p>
              </div>
              <i class="fas fa-arrow-right text-[var(--gray-text-color)] group-hover:text-[var(--primary-color)] group-hover:translate-x-2 transition-all"></i>
            </div>
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const categories = [
  { name: 'Real Estate', slug: 'real-estate', icon: 'fa-home', color: 'primary-color', count: 234 },
  { name: 'Financial Services', slug: 'financial', icon: 'fa-chart-line', color: 'accent3-color', count: 189 },
  { name: 'Healthcare', slug: 'healthcare', icon: 'fa-heartbeat', color: 'accent-color', count: 156 },
  { name: 'Legal Services', slug: 'legal', icon: 'fa-balance-scale', color: 'accent2-color', count: 142 },
  { name: 'Technology', slug: 'technology', icon: 'fa-laptop-code', color: 'primary-color', count: 298 },
  { name: 'Consulting', slug: 'consulting', icon: 'fa-briefcase', color: 'accent3-color', count: 176 },
  { name: 'Construction', slug: 'construction', icon: 'fa-hard-hat', color: 'accent4-color', count: 218 },
  { name: 'Hospitality', slug: 'hospitality', icon: 'fa-concierge-bell', color: 'accent-color', count: 167 },
  { name: 'Education', slug: 'education', icon: 'fa-graduation-cap', color: 'accent2-color', count: 134 },
]

useHead({
  title: 'Business Categories',
})
</script>

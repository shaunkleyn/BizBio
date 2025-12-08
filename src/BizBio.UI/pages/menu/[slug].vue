<template>
  <div class="min-h-screen bg-[var(--light-background-color)]">
    <!-- Loading State -->
    <div v-if="pending" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-4xl text-[var(--primary-color)] mb-4"></i>
        <p class="text-[var(--gray-text-color)]">Loading menu...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex items-center justify-center min-h-screen px-4">
      <div class="text-center max-w-md">
        <i class="fas fa-exclamation-triangle text-6xl text-[var(--accent-color)] mb-4"></i>
        <h1 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">Menu Not Found</h1>
        <p class="text-[var(--gray-text-color)] mb-6">
          Sorry, we couldn't find the menu you're looking for.
        </p>
        <NuxtLink
          to="/"
          class="inline-block px-6 py-3 bg-[var(--primary-color)] text-white rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors"
        >
          <i class="fas fa-home mr-2"></i>
          Go to Homepage
        </NuxtLink>
      </div>
    </div>

    <!-- Menu Content -->
    <div v-else-if="menu" class="max-w-6xl mx-auto px-4 py-8">
      <!-- Menu Header -->
      <div class="bg-white rounded-2xl shadow-xl p-8 mb-8">
        <div class="flex flex-col md:flex-row gap-6 items-start md:items-center">
          <!-- Business Logo -->
          <div v-if="menu.businessLogo" class="w-32 h-32 rounded-xl overflow-hidden flex-shrink-0 border-2 border-[var(--light-border-color)]">
            <img :src="menu.businessLogo" :alt="menu.businessName" class="w-full h-full object-cover" />
          </div>

          <!-- Business Info -->
          <div class="flex-1">
            <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
              {{ menu.name }}
            </h1>
            <p class="text-xl text-[var(--primary-color)] font-semibold mb-3">
              {{ menu.businessName }}
            </p>
            <p v-if="menu.description" class="text-[var(--gray-text-color)] mb-4">
              {{ menu.description }}
            </p>

            <!-- Cuisine Badge -->
            <div class="flex flex-wrap gap-2 mb-4">
              <span class="px-3 py-1 bg-[var(--primary-color)] bg-opacity-10 text-[var(--primary-color)] rounded-full text-sm font-semibold capitalize">
                <i class="fas fa-utensils mr-1"></i>
                {{ menu.cuisine }}
              </span>
            </div>

            <!-- Contact Info -->
            <div class="flex flex-wrap gap-4 text-sm text-[var(--gray-text-color)]">
              <a
                v-if="menu.phoneNumber"
                :href="`tel:${menu.phoneNumber}`"
                @click="handleContactAction('phone')"
                class="hover:text-[var(--primary-color)] transition-colors"
              >
                <i class="fas fa-phone mr-2"></i>{{ menu.phoneNumber }}
              </a>
              <a
                v-if="menu.email"
                :href="`mailto:${menu.email}`"
                @click="handleContactAction('email')"
                class="hover:text-[var(--primary-color)] transition-colors"
              >
                <i class="fas fa-envelope mr-2"></i>{{ menu.email }}
              </a>
              <span v-if="menu.address" @click="handleContactAction('address')" class="cursor-pointer hover:text-[var(--primary-color)] transition-colors">
                <i class="fas fa-map-marker-alt mr-2"></i>{{ menu.address }}<span v-if="menu.city">, {{ menu.city }}</span>
              </span>
            </div>
          </div>
        </div>

        <!-- Working Hours -->
        <div v-if="menu.workingHours" class="mt-6 pt-6 border-t-2 border-[var(--light-border-color)]">
          <h3 class="text-lg font-bold text-[var(--dark-text-color)] mb-3">
            <i class="fas fa-clock mr-2 text-[var(--primary-color)]"></i>
            Opening Hours
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
            <div
              v-for="(hours, day) in menu.workingHours"
              :key="day"
              class="flex justify-between items-center text-sm"
            >
              <span class="capitalize font-medium text-[var(--dark-text-color)]">{{ day }}:</span>
              <span v-if="hours.closed" class="text-[var(--accent-color)]">Closed</span>
              <span v-else class="text-[var(--gray-text-color)]">{{ hours.open }} - {{ hours.close }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Menu Categories and Items -->
      <div v-if="menu.categories && menu.categories.length > 0" class="space-y-8">
        <div
          v-for="category in menu.categories"
          :key="category.id"
          :data-category-id="category.id"
          :data-category-name="category.name"
          class="bg-white rounded-2xl shadow-xl p-6 category-section"
        >
          <!-- Category Header -->
          <div class="flex items-center gap-3 mb-6 pb-4 border-b-2 border-[var(--light-border-color)]">
            <div class="w-12 h-12 bg-[var(--primary-color)] bg-opacity-10 rounded-lg flex items-center justify-center">
              <i :class="[category.icon, 'text-[var(--primary-color)] text-xl']"></i>
            </div>
            <div>
              <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                {{ category.name }}
              </h2>
              <p v-if="category.description" class="text-sm text-[var(--gray-text-color)]">
                {{ category.description }}
              </p>
            </div>
          </div>

          <!-- Category Items -->
          <div class="grid md:grid-cols-2 gap-4">
            <div
              v-for="item in getCategoryItems(category.id)"
              :key="item.id"
              @click="handleItemClick(item)"
              class="border-2 border-[var(--light-border-color)] rounded-xl p-4 hover:border-[var(--primary-color)] transition-all cursor-pointer"
            >
              <div class="flex gap-4">
                <!-- Item Image -->
                <div v-if="item.imageUrl" class="w-24 h-24 rounded-lg overflow-hidden flex-shrink-0">
                  <img :src="item.imageUrl" :alt="item.name" class="w-full h-full object-cover" />
                </div>

                <!-- Item Info -->
                <div class="flex-1 min-w-0">
                  <div class="flex items-start justify-between gap-2 mb-1">
                    <h3 class="font-bold text-[var(--dark-text-color)]">{{ item.name }}</h3>
                    <span class="text-[var(--primary-color)] font-bold flex-shrink-0">R{{ item.price.toFixed(2) }}</span>
                  </div>
                  <p v-if="item.description" class="text-sm text-[var(--gray-text-color)] mb-2">
                    {{ item.description }}
                  </p>

                  <!-- Dietary & Allergen Info -->
                  <div class="flex flex-wrap gap-1">
                    <span
                      v-for="diet in item.dietary"
                      :key="diet"
                      class="text-xs px-2 py-1 bg-green-100 text-green-700 rounded-full"
                    >
                      {{ diet }}
                    </span>
                    <span
                      v-for="allergen in item.allergens"
                      :key="allergen"
                      class="text-xs px-2 py-1 bg-red-100 text-red-700 rounded-full"
                    >
                      {{ allergen }}
                    </span>
                  </div>

                  <!-- Availability -->
                  <div v-if="!item.available" class="mt-2">
                    <span class="text-xs px-2 py-1 bg-gray-200 text-gray-600 rounded-full">
                      Currently Unavailable
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="mt-12 text-center text-sm text-[var(--gray-text-color)]">
        <p>Powered by <a href="/" class="text-[var(--primary-color)] hover:underline">BizBio</a></p>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const menusApi = useMenusApi()
const analytics = useMenuAnalytics()

// Fetch menu data by slug
const { data: menu, pending, error } = await useAsyncData(
  `menu-${route.params.slug}`,
  () => menusApi.getMenuBySlug(route.params.slug)
)

// Helper function to get items for a specific category
const getCategoryItems = (categoryId) => {
  if (!menu.value?.items) return []
  return menu.value.items.filter(item => item.categoryId === categoryId)
}

// Analytics event handlers
const handleItemClick = (item) => {
  if (menu.value?.id) {
    analytics.trackItemView(menu.value.id, item.id, item)
  }
}

const handleContactAction = (actionType) => {
  if (menu.value?.id) {
    analytics.trackContactAction(menu.value.id, actionType)
  }
}

// Setup analytics tracking on client side only
onMounted(() => {
  if (!menu.value) return

  // Track initial page view
  analytics.trackMenuView(menu.value.id, String(route.params.slug), menu.value)

  // Setup category tracking with intersection observer
  const categoryObserver = analytics.setupCategoryTracking(menu.value.id, menu.value.categories || [])
  if (categoryObserver) {
    nextTick(() => {
      const categoryElements = document.querySelectorAll('.category-section')
      categoryElements.forEach(el => categoryObserver.observe(el))
    })
  }

  // Setup scroll depth tracking
  const cleanupScroll = analytics.setupScrollTracking(menu.value.id)

  // Setup time tracking
  const cleanupTime = analytics.setupTimeTracking(menu.value.id)

  // Cleanup on unmount
  onUnmounted(() => {
    if (categoryObserver) {
      const categoryElements = document.querySelectorAll('.category-section')
      categoryElements.forEach(el => categoryObserver.unobserve(el))
    }
    if (cleanupScroll) cleanupScroll()
    if (cleanupTime) cleanupTime()
  })
})

// SEO Meta Tags
if (menu.value) {
  const metaTitle = menu.value.metaTitle || `${menu.value.name} - ${menu.value.businessName}`
  const metaDescription = menu.value.metaDescription || menu.value.description || `View the menu at ${menu.value.businessName}. ${menu.value.cuisine} cuisine with a variety of delicious options.`
  const keywords = menu.value.keywords || `${menu.value.cuisine}, restaurant, menu, ${menu.value.businessName}, food`

  useSeoMeta({
    title: metaTitle,
    description: metaDescription,
    keywords: keywords,

    // OpenGraph tags for social media
    ogTitle: metaTitle,
    ogDescription: metaDescription,
    ogImage: menu.value.businessLogo || '/default-menu-image.jpg',
    ogType: 'website',
    ogUrl: `https://yourdomain.com/menu/${route.params.slug}`,

    // Twitter Card tags
    twitterCard: 'summary_large_image',
    twitterTitle: metaTitle,
    twitterDescription: metaDescription,
    twitterImage: menu.value.businessLogo || '/default-menu-image.jpg',

    // Additional meta tags
    robots: menu.value.enableSEO ? 'index, follow' : 'noindex, nofollow',
    author: menu.value.businessName,

    // Canonical URL
    canonical: `https://yourdomain.com/menu/${route.params.slug}`
  })

  // Schema.org Structured Data (JSON-LD)
  useHead({
    script: [
      {
        type: 'application/ld+json',
        children: JSON.stringify({
          '@context': 'https://schema.org',
          '@type': 'Restaurant',
          name: menu.value.businessName,
          description: menu.value.description,
          image: menu.value.businessLogo,
          address: menu.value.address ? {
            '@type': 'PostalAddress',
            streetAddress: menu.value.address,
            addressLocality: menu.value.city,
            addressCountry: menu.value.country
          } : undefined,
          telephone: menu.value.phoneNumber,
          email: menu.value.email,
          servesCuisine: menu.value.cuisine,
          priceRange: '$$',
          openingHoursSpecification: menu.value.workingHours ? Object.entries(menu.value.workingHours)
            .filter(([_, hours]) => !hours.closed)
            .map(([day, hours]) => ({
              '@type': 'OpeningHoursSpecification',
              dayOfWeek: day.charAt(0).toUpperCase() + day.slice(1),
              opens: hours.open,
              closes: hours.close
            })) : undefined,
          hasMenu: {
            '@type': 'Menu',
            name: menu.value.name,
            description: menu.value.description,
            hasMenuSection: menu.value.categories?.map(category => ({
              '@type': 'MenuSection',
              name: category.name,
              description: category.description,
              hasMenuItem: getCategoryItems(category.id).map(item => ({
                '@type': 'MenuItem',
                name: item.name,
                description: item.description,
                image: item.imageUrl,
                offers: {
                  '@type': 'Offer',
                  price: item.price,
                  priceCurrency: 'ZAR',
                  availability: item.available ? 'https://schema.org/InStock' : 'https://schema.org/OutOfStock'
                },
                suitableForDiet: item.dietary?.map(diet => {
                  const dietMap = {
                    'Vegetarian': 'https://schema.org/VegetarianDiet',
                    'Vegan': 'https://schema.org/VeganDiet',
                    'Gluten-Free': 'https://schema.org/GlutenFreeDiet',
                    'Halal': 'https://schema.org/HalalDiet',
                    'Kosher': 'https://schema.org/KosherDiet'
                  }
                  return dietMap[diet]
                }).filter(Boolean)
              }))
            }))
          }
        })
      }
    ]
  })
}
</script>

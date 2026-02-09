<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-book-open text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              8
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Active Menus
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +12%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-utensils text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              142
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Menu Items
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +8%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-yellow-500 to-orange-500 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-calendar-check text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              15
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Scheduled Events
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +25%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="w-14 h-14 bg-gradient-to-br from-purple-500 to-pink-500 rounded-2xl flex items-center justify-center shadow-lg">
                <i class="fas fa-eye text-white text-xl" aria-hidden="true"></i>
              </div>
            </div>
            <h3 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">
              2.4K
            </h3>
            <p class="text-sm text-[var(--gray-text-color)] font-semibold">
              Menu Views
            </p>
            <div class="mt-3 flex items-center gap-1 text-xs">
              <span class="text-green-600 font-bold flex items-center gap-1">
                <i class="fas fa-arrow-up" aria-hidden="true"></i>
                +34%
              </span>
              <span class="text-[var(--gray-text-color)]">vs last month</span>
            </div>
          </div>
        </div>
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
          <div class="lg:col-span-2 bg-white rounded-2xl p-8 shadow-lg" id="active-menus-overview">
            <div class="flex items-center justify-between mb-6">
              <div>
                <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
                  Currently Active Menus
                </h2>
                <p class="text-sm text-[var(--gray-text-color)]">
                  All menus being displayed right now across your restaurants
                </p>
              </div>
              <button class="px-4 py-2 bg-gray-100 text-[var(--gray-text-color)] rounded-xl font-semibold hover:bg-gray-200 transition-all text-sm">
                <i class="fas fa-filter mr-2" aria-hidden="true"></i>Filter
              </button>
            </div>

            <div class="space-y-4">
              <!-- Active Menu Card 1 -->
              <div class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent2-color)] rounded-2xl p-6 text-white shadow-xl relative overflow-hidden hover:shadow-2xl transition-all">
                <div class="absolute top-0 right-0 w-32 h-32 bg-white opacity-10 rounded-full -mr-16 -mt-16"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-white opacity-10 rounded-full -ml-12 -mb-12"></div>

                <div class="relative">
                  <div class="flex items-start justify-between mb-4">
                    <div class="flex items-center gap-3">
                      <div class="w-12 h-12 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl flex items-center justify-center shadow-lg">
                        <i class="fas fa-store text-white" aria-hidden="true"></i>
                      </div>
                      <div>
                        <p class="text-xs text-white text-opacity-70 font-semibold mb-1">
                          The Grand Dining Room
                        </p>
                        <h3 class="text-2xl font-bold">Full Dinner Menu</h3>
                      </div>
                    </div>
                    <span class="bg-white bg-opacity-20 backdrop-blur-sm px-3 py-1.5 rounded-xl text-xs font-bold shadow-lg">
                      <i class="fas fa-clock mr-1" aria-hidden="true"></i>2:45 remaining
                    </span>
                  </div>

                  <p class="text-white text-opacity-90 text-sm mb-4">
                    Active until 10:00 PM • Daily Service
                  </p>

                  <div class="grid grid-cols-3 gap-3 mb-4">
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">87</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Items
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">12</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Categories
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">324</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Views Today
                      </p>
                    </div>
                  </div>

                  <div class="flex gap-2">
                    <button class="flex-1 bg-white text-[var(--primary-color)] py-2.5 rounded-xl text-sm font-bold hover:shadow-lg transition-all">
                      <i class="fas fa-edit mr-1" aria-hidden="true"></i>Edit
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-eye" aria-hidden="true"></i>
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-ellipsis-v" aria-hidden="true"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Active Menu Card 2 -->
              <div class="bg-gradient-to-br from-orange-500 to-yellow-500 rounded-2xl p-6 text-white shadow-xl relative overflow-hidden hover:shadow-2xl transition-all">
                <div class="absolute top-0 right-0 w-32 h-32 bg-white opacity-10 rounded-full -mr-16 -mt-16"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-white opacity-10 rounded-full -ml-12 -mb-12"></div>

                <div class="relative">
                  <div class="flex items-start justify-between mb-4">
                    <div class="flex items-center gap-3">
                      <div class="w-12 h-12 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl flex items-center justify-center shadow-lg">
                        <i class="fas fa-store text-white" aria-hidden="true"></i>
                      </div>
                      <div>
                        <p class="text-xs text-white text-opacity-70 font-semibold mb-1">
                          Coastal Bistro
                        </p>
                        <h3 class="text-2xl font-bold">Breakfast Menu</h3>
                      </div>
                    </div>
                    <span class="bg-white bg-opacity-20 backdrop-blur-sm px-3 py-1.5 rounded-xl text-xs font-bold shadow-lg">
                      <i class="fas fa-clock mr-1" aria-hidden="true"></i>1:15 remaining
                    </span>
                  </div>

                  <p class="text-white text-opacity-90 text-sm mb-4">
                    Active until 11:00 AM • Morning Service
                  </p>

                  <div class="grid grid-cols-3 gap-3 mb-4">
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">42</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Items
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">8</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Categories
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">156</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Views Today
                      </p>
                    </div>
                  </div>

                  <div class="flex gap-2">
                    <button class="flex-1 bg-white text-orange-600 py-2.5 rounded-xl text-sm font-bold hover:shadow-lg transition-all">
                      <i class="fas fa-edit mr-1" aria-hidden="true"></i>Edit
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-eye" aria-hidden="true"></i>
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-ellipsis-v" aria-hidden="true"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Active Menu Card 3 -->
              <div class="bg-gradient-to-br from-purple-500 to-pink-500 rounded-2xl p-6 text-white shadow-xl relative overflow-hidden hover:shadow-2xl transition-all">
                <div class="absolute top-0 right-0 w-32 h-32 bg-white opacity-10 rounded-full -mr-16 -mt-16"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-white opacity-10 rounded-full -ml-12 -mb-12"></div>

                <div class="relative">
                  <div class="flex items-start justify-between mb-4">
                    <div class="flex items-center gap-3">
                      <div class="w-12 h-12 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl flex items-center justify-center shadow-lg">
                        <i class="fas fa-store text-white" aria-hidden="true"></i>
                      </div>
                      <div>
                        <p class="text-xs text-white text-opacity-70 font-semibold mb-1">
                          Sunset Grill &amp; Bar
                        </p>
                        <h3 class="text-2xl font-bold">Happy Hour Menu</h3>
                      </div>
                    </div>
                    <span class="bg-white bg-opacity-20 backdrop-blur-sm px-3 py-1.5 rounded-xl text-xs font-bold shadow-lg">
                      <i class="fas fa-circle mr-1" aria-hidden="true"></i>Active Now
                    </span>
                  </div>

                  <p class="text-white text-opacity-90 text-sm mb-4">
                    4:00 PM - 7:00 PM • Special Pricing
                  </p>

                  <div class="grid grid-cols-3 gap-3 mb-4">
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">28</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Items
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">6</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Categories
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">189</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Views Today
                      </p>
                    </div>
                  </div>

                  <div class="flex gap-2">
                    <button class="flex-1 bg-white text-purple-600 py-2.5 rounded-xl text-sm font-bold hover:shadow-lg transition-all">
                      <i class="fas fa-edit mr-1" aria-hidden="true"></i>Edit
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-eye" aria-hidden="true"></i>
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-ellipsis-v" aria-hidden="true"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Active Menu Card 4 -->
              <div class="bg-gradient-to-br from-green-500 to-green-600 rounded-2xl p-6 text-white shadow-xl relative overflow-hidden hover:shadow-2xl transition-all">
                <div class="absolute top-0 right-0 w-32 h-32 bg-white opacity-10 rounded-full -mr-16 -mt-16"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-white opacity-10 rounded-full -ml-12 -mb-12"></div>

                <div class="relative">
                  <div class="flex items-start justify-between mb-4">
                    <div class="flex items-center gap-3">
                      <div class="w-12 h-12 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl flex items-center justify-center shadow-lg">
                        <i class="fas fa-store text-white" aria-hidden="true"></i>
                      </div>
                      <div>
                        <p class="text-xs text-white text-opacity-70 font-semibold mb-1">
                          Urban Eats Café
                        </p>
                        <h3 class="text-2xl font-bold">Lunch Specials</h3>
                      </div>
                    </div>
                    <span class="bg-white bg-opacity-20 backdrop-blur-sm px-3 py-1.5 rounded-xl text-xs font-bold shadow-lg">
                      <i class="fas fa-circle mr-1" aria-hidden="true"></i>Active Now
                    </span>
                  </div>

                  <p class="text-white text-opacity-90 text-sm mb-4">
                    11:30 AM - 2:30 PM • Daily Specials
                  </p>

                  <div class="grid grid-cols-3 gap-3 mb-4">
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">35</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Items
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">7</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Categories
                      </p>
                    </div>
                    <div class="bg-white bg-opacity-10 backdrop-blur-sm rounded-xl p-3 text-center">
                      <p class="text-2xl font-bold">203</p>
                      <p class="text-white text-opacity-70 text-xs font-semibold">
                        Views Today
                      </p>
                    </div>
                  </div>

                  <div class="flex gap-2">
                    <button class="flex-1 bg-white text-green-600 py-2.5 rounded-xl text-sm font-bold hover:shadow-lg transition-all">
                      <i class="fas fa-edit mr-1" aria-hidden="true"></i>Edit
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-eye" aria-hidden="true"></i>
                    </button>
                    <button class="px-4 bg-white bg-opacity-20 backdrop-blur-sm rounded-xl hover:bg-opacity-30 transition-all">
                      <i class="fas fa-ellipsis-v" aria-hidden="true"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- View All Link -->
            <div class="mt-6 text-center">
              <button class="text-[var(--primary-color)] font-semibold hover:underline text-sm">
                View All Active Menus <i class="fas fa-arrow-right ml-1" aria-hidden="true"></i>
              </button>
            </div>
          </div>
          <div class="bg-white rounded-2xl p-8 shadow-lg sticky top-40 self-start" id="menu-quick-actions">
            <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-6">
              Quick Actions
            </h2>

            <div class="space-y-3">
              <button class="w-full flex items-center gap-4 p-4 bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl hover:shadow-md transition-all text-left transform hover:-translate-y-0.5" onclick="window.location.href='/menu-create'">
                <div class="w-12 h-12 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl flex items-center justify-center flex-shrink-0 shadow-md">
                  <i class="fas fa-plus text-white text-lg" aria-hidden="true"></i>
                </div>
                <div>
                  <p class="font-bold text-[var(--dark-text-color)]">
                    New Menu
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Create a new menu
                  </p>
                </div>
              </button>

              <button class="w-full flex items-center gap-4 p-4 bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl hover:shadow-md transition-all text-left transform hover:-translate-y-0.5">
                <div class="w-12 h-12 bg-gradient-to-br from-green-500 to-green-600 rounded-xl flex items-center justify-center flex-shrink-0 shadow-md">
                  <i class="fas fa-utensils text-white text-lg" aria-hidden="true"></i>
                </div>
                <div>
                  <p class="font-bold text-[var(--dark-text-color)]">
                    Add Item
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Add new menu item
                  </p>
                </div>
              </button>

              <button class="w-full flex items-center gap-4 p-4 bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl hover:shadow-md transition-all text-left transform hover:-translate-y-0.5">
                <div class="w-12 h-12 bg-gradient-to-br from-orange-500 to-yellow-500 rounded-xl flex items-center justify-center flex-shrink-0 shadow-md">
                  <i class="fas fa-calendar-plus text-white text-lg" aria-hidden="true"></i>
                </div>
                <div>
                  <p class="font-bold text-[var(--dark-text-color)]">
                    Schedule Event
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Create new event
                  </p>
                </div>
              </button>

              <button class="w-full flex items-center gap-4 p-4 bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl hover:shadow-md transition-all text-left transform hover:-translate-y-0.5">
                <div class="w-12 h-12 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center flex-shrink-0 shadow-md">
                  <i class="fas fa-copy text-white text-lg" aria-hidden="true"></i>
                </div>
                <div>
                  <p class="font-bold text-[var(--dark-text-color)]">
                    Duplicate Menu
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Copy existing menu
                  </p>
                </div>
              </button>
            </div>
          </div>
      </div>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Active Menus</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.menus }}</p>
            </div>
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-book-open text-blue-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Total Items</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.items }}</p>
            </div>
            <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-green-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">Categories</p>
              <p class="text-3xl font-bold text-md-on-surface">{{ stats.categories }}</p>
            </div>
            <div class="w-12 h-12 bg-md-primary-container rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-md-primary text-xl"></i>
            </div>
          </div>
        </div>

        <div class="bg-md-surface rounded-2xl shadow-sm p-6 border border-md-outline-variant">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-md-on-surface-variant mb-1">QR Scans</p>
              <p class="text-lg font-bold text-md-on-surface">Coming Soon</p>
            </div>
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Menus -->
      <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant mb-8">
        <div class="p-6 border-b border-md-outline-variant">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-bold text-md-on-surface">Recent Menus</h2>
            <NuxtLink to="/menu" class="text-sm text-[var(--primary-color)] hover:underline">
              View All
            </NuxtLink>
          </div>
        </div>
        <div class="p-6">
          <div v-if="loading" class="text-center py-8">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-[var(--primary-color)] mx-auto"></div>
          </div>
          <div v-else-if="recentMenus.length === 0" class="text-center py-12">
            <i class="fas fa-book-open text-4xl text-gray-300 mb-3"></i>
            <p class="text-md-on-surface-variant mb-4">No menus yet. Create your first menu to get started!</p>
            <button
              @click="handleCreateMenu"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold"
            >
              <i class="fas fa-plus mr-2"></i>
              Create Menu
            </button>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="menu in recentMenus"
              :key="menu.id"
              class="border border-md-outline-variant rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
              @click="navigateTo(`/${menu.entitySlug}/${menu.slug}`)"
            >
              <h3 class="font-semibold text-md-on-surface mb-1">{{ menu.name }}</h3>
              <p class="text-sm text-md-on-surface-variant mb-2">{{ menu.description || 'No description' }}</p>
              <div class="flex items-center justify-between text-xs text-gray-500">
                <span>{{ menu.itemCount || 0 }} items</span>
                <span>Updated {{ formatDate(menu.updatedAt) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <NuxtLink
          to="/menu/items"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-utensils text-blue-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">Library Items</h3>
              <p class="text-sm text-md-on-surface-variant">Manage your item library</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/categories"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-md-primary-container rounded-lg flex items-center justify-center">
              <i class="fas fa-layer-group text-md-primary text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">Categories</h3>
              <p class="text-sm text-md-on-surface-variant">Organize your items</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/menu/tables"
          class="mesh-card bg-md-surface rounded-2xl shadow-md-3 border border-md-outline-variant p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <i class="fas fa-qrcode text-orange-600 text-xl"></i>
            </div>
            <div>
              <h3 class="font-semibold text-md-on-surface">QR Codes</h3>
              <p class="text-sm text-md-on-surface-variant">Generate table QR codes</p>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'

definePageMeta({
  layout: 'menu'
})

const loading = ref(false)
const recentMenus = ref<any[]>([])
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0,
  scans: 0
})

// Provide stats to the layout
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader, setPageActionButton } = usePageMeta()

onMounted(async () => {
  // Set page metadata
  setPageHeader({
    title: 'Menu Dashboard',
    description: 'Manage your digital menus and menu items'
  })

  setPageActionButton({
    onClick: () => handleCreateMenu(),
    label: 'Create Menu',
    icon: 'fas fa-plus',
    class: 'px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-colors font-semibold'
  })

  await loadDashboardData()
})

const handleCreateMenu = () => {
  // Navigate to menus page and open wizard
  navigateTo('/menu?create=true')
}

async function loadDashboardData() {
  loading.value = true
  try {
    const entityApi = useEntityApi()
    const libraryItemsApi = useLibraryItemsApi()
    const categoriesApi = useLibraryCategoriesApi()

    // Get user's entities (which may have catalogs)
    const entitiesResponse = await entityApi.getMyEntities()
    const entities = entitiesResponse?.data?.entities || []

    // For each entity, get its catalogs and build menu list
    const menusList: any[] = []
    for (const entity of entities) {
      const catalogsResponse = await entityApi.getEntityCatalogs(entity.id)
      const catalogs = catalogsResponse?.data?.catalogs || []

      // Add each catalog as a "menu" with entity slug for routing
      catalogs.forEach((catalog: any) => {
        menusList.push({
          id: catalog.id,
          name: catalog.name,
          slug: catalog.slug,
          entitySlug: entity.slug,
          description: catalog.description,
          itemCount: catalog.itemCount || 0,
          updatedAt: catalog.updatedAt
        })
      })
    }

    // Set recent menus
    recentMenus.value = menusList.slice(0, 5) // Get last 5 menus
    stats.value.menus = menusList.length

    // Load items and categories in parallel
    const [itemsResponse, categoriesResponse] = await Promise.all([
      libraryItemsApi.getItems(),
      categoriesApi.getCategories()
    ])

    // Set items count
    const items = (itemsResponse?.data || itemsResponse?.data?.items || [])
    console.log('Library items response:', itemsResponse)
    stats.value.items = items.length

    // Set categories count
    const categories = Array.isArray(categoriesResponse) ? categoriesResponse : (categoriesResponse?.data || [])
    stats.value.categories = categories.length

    // TODO: Add API call for scans when available
    stats.value.scans = '1243'
  } catch (error) {
    console.error('Error loading dashboard data:', error)
    // Fallback to empty data on error
    recentMenus.value = []
    stats.value = {
      menus: 0,
      items: 0,
      categories: 0,
      scans: 0
    }
  } finally {
    loading.value = false
  }
}

function formatDate(date: string) {
  if (!date) return 'Recently'
  const d = new Date(date)
  const now = new Date()
  const diffTime = Math.abs(now.getTime() - d.getTime())
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  return d.toLocaleDateString()
}
</script>






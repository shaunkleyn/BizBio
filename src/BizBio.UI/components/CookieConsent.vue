<template>
  <Teleport to="body">
    <Transition name="slide-up">
      <div
        v-if="showBanner"
        class="fixed bottom-0 left-0 right-0 z-[9999] p-4 md:p-6"
      >
        <div class="max-w-6xl mx-auto">
          <div class="bg-md-surface-container rounded-2xl shadow-xl border-2 border-md-outline-variant p-6 md:p-8">
            <!-- Compact View -->
            <div v-if="!showDetails" class="flex flex-col md:flex-row items-start md:items-center gap-6">
              <!-- Icon and Text -->
              <div class="flex-1 flex items-start gap-4">
                <div class="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-cookie-bite text-blue-600 text-xl"></i>
                </div>
                <div class="flex-1">
                  <h3 class="text-lg font-bold text-md-on-surface mb-2">
                    We Value Your Privacy
                  </h3>
                  <p class="text-sm text-md-on-surface-variant leading-relaxed">
                    We use cookies and local storage to enhance your experience and analyze our service performance.
                    <button
                      @click="showDetails = true"
                      class="text-[var(--primary-color)] hover:underline font-semibold"
                    >
                      Learn more
                    </button>
                  </p>
                </div>
              </div>

              <!-- Buttons -->
              <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
                <button
                  @click="handleAcceptNecessary"
                  class="px-6 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline rounded-xl hover:bg-md-surface-container-high transition-colors font-semibold text-sm whitespace-nowrap"
                >
                  Necessary Only
                </button>
                <button
                  @click="handleAcceptAll"
                  class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-all font-semibold text-sm whitespace-nowrap"
                >
                  <i class="fas fa-check mr-2"></i>
                  Accept All
                </button>
              </div>
            </div>

            <!-- Detailed View -->
            <div v-else class="space-y-6">
              <!-- Header -->
              <div class="flex items-center justify-between">
                <h3 class="text-xl font-bold text-md-on-surface">
                  Cookie & Storage Preferences
                </h3>
                <button
                  @click="showDetails = false"
                  class="text-md-on-surface-variant hover:text-md-on-surface transition-colors"
                >
                  <i class="fas fa-times text-xl"></i>
                </button>
              </div>

              <!-- Description -->
              <p class="text-sm text-md-on-surface-variant">
                We use browser storage to provide essential functionality and improve our services.
                You can customize your preferences below.
              </p>

              <!-- Cookie Categories -->
              <div class="space-y-4">
                <!-- Functional/Necessary Cookies -->
                <div class="bg-md-surface-container-high rounded-xl p-4">
                  <div class="flex items-start justify-between gap-4">
                    <div class="flex-1">
                      <div class="flex items-center gap-2 mb-2">
                        <i class="fas fa-shield-check text-green-600"></i>
                        <h4 class="font-semibold text-md-on-surface">
                          Strictly Necessary
                        </h4>
                        <span class="text-xs bg-green-100 text-green-700 px-2 py-1 rounded-full font-semibold">
                          Required
                        </span>
                      </div>
                      <p class="text-sm text-md-on-surface-variant">
                        Essential for the website to function. Stores your subscription plan and trial period.
                        Cannot be disabled.
                      </p>
                      <div class="mt-2 text-xs text-md-on-surface-variant">
                        <strong>Data stored:</strong> Plan selection, trial start date, authentication tokens
                      </div>
                    </div>
                    <div class="flex items-center">
                      <div class="w-12 h-6 bg-green-500 rounded-full relative">
                        <div class="absolute right-1 top-1 w-4 h-4 bg-white rounded-full"></div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Analytics Cookies -->
                <div class="bg-md-surface-container-high rounded-xl p-4">
                  <div class="flex items-start justify-between gap-4">
                    <div class="flex-1">
                      <div class="flex items-center gap-2 mb-2">
                        <i class="fas fa-chart-line text-blue-600"></i>
                        <h4 class="font-semibold text-md-on-surface">
                          Analytics & Performance
                        </h4>
                        <span class="text-xs bg-blue-100 text-blue-700 px-2 py-1 rounded-full font-semibold">
                          Optional
                        </span>
                      </div>
                      <p class="text-sm text-md-on-surface-variant">
                        Helps us understand how you use our service to improve your experience.
                        All data is anonymized.
                      </p>
                      <div class="mt-2 text-xs text-md-on-surface-variant">
                        <strong>Data collected:</strong> Device type, browser type, page views, session duration, general location (country/city)
                      </div>
                    </div>
                    <div class="flex items-center">
                      <label class="relative inline-flex items-center cursor-pointer">
                        <input
                          v-model="analyticsEnabled"
                          type="checkbox"
                          class="sr-only peer"
                        />
                        <div class="w-12 h-6 bg-md-outline-variant peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-md-primary peer-focus:ring-opacity-30 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-md-on-primary after:content-[''] after:absolute after:top-0.5 after:start-[4px] after:bg-white after:border-md-outline after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-md-primary"></div>
                      </label>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Privacy Links -->
              <div class="flex items-center justify-between pt-4 border-t border-md-outline-variant">
                <div class="text-xs text-md-on-surface-variant">
                  <NuxtLink to="/privacy" class="hover:underline text-[var(--primary-color)]">
                    Privacy Policy
                  </NuxtLink>
                  <span class="mx-2">•</span>
                  <NuxtLink to="/terms" class="hover:underline text-[var(--primary-color)]">
                    Terms of Service
                  </NuxtLink>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="flex flex-col sm:flex-row gap-3">
                <button
                  @click="handleSavePreferences"
                  class="flex-1 px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-all font-semibold"
                >
                  <i class="fas fa-save mr-2"></i>
                  Save Preferences
                </button>
                <button
                  @click="handleAcceptAll"
                  class="px-6 py-3 bg-md-surface-container text-md-on-surface border-2 border-md-outline rounded-xl hover:bg-md-surface-container-high transition-colors font-semibold"
                >
                  Accept All
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
const { showBanner, acceptAll, acceptNecessary, saveConsent } = useCookieConsent()

const showDetails = ref(false)
const analyticsEnabled = ref(false)

const handleAcceptAll = () => {
  acceptAll()
  showDetails.value = false
}

const handleAcceptNecessary = () => {
  acceptNecessary()
  showDetails.value = false
}

const handleSavePreferences = () => {
  saveConsent(true, analyticsEnabled.value)
  showDetails.value = false
}
</script>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease-out;
}

.slide-up-enter-from {
  transform: translateY(100%);
  opacity: 0;
}

.slide-up-leave-to {
  transform: translateY(100%);
  opacity: 0;
}
</style>

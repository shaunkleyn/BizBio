<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const user = ref(authStore.currentUser)
</script>

<template>
  
  <div class="frame-root">
    <div class="frame-content">
      <div class="[font-family:var(--font-family-body)]">
        <div>

        </div>
        <div>
          <section class="code-section bg-[var(--light-background-color)] py-8 sm:py-12 min-h-screen" id="sm26h8w">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <!-- Welcome Header -->
              <div class="mb-8">
                <h1
                  class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Welcome back, {{ user?.firstName }}!
                </h1>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  Here's what's happening with your profiles today
                </p>
              </div>

              <!-- Stats Cards -->
              <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                <!-- Stat Card 1 -->
                <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
                  <div class="flex items-center justify-between mb-4">
                    <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full p-3">
                      <i class="fas fa-eye text-[var(--primary-color)] text-2xl" aria-hidden="true"></i>
                    </div>
                    <span class="text-[var(--accent3-color)] text-sm font-semibold">
                      <i class="fas fa-arrow-up mr-1" aria-hidden="true"></i>12%
                    </span>
                  </div>
                  <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
                    1,284
                  </h3>
                  <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
                    Total Profile Views
                  </p>
                </div>

                <!-- Stat Card 2 -->
                <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
                  <div class="flex items-center justify-between mb-4">
                    <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-3">
                      <i class="fas fa-share-alt text-[var(--accent3-color)] text-2xl" aria-hidden="true"></i>
                    </div>
                    <span class="text-[var(--accent3-color)] text-sm font-semibold">
                      <i class="fas fa-arrow-up mr-1" aria-hidden="true"></i>8%
                    </span>
                  </div>
                  <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
                    342
                  </h3>
                  <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
                    Profile Shares
                  </p>
                </div>

                <!-- Stat Card 3 -->
                <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
                  <div class="flex items-center justify-between mb-4">
                    <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-full p-3">
                      <i class="fas fa-mouse-pointer text-[var(--accent2-color)] text-2xl" aria-hidden="true"></i>
                    </div>
                    <span class="text-[var(--accent3-color)] text-sm font-semibold">
                      <i class="fas fa-arrow-up mr-1" aria-hidden="true"></i>24%
                    </span>
                  </div>
                  <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
                    678
                  </h3>
                  <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
                    Contact Clicks
                  </p>
                </div>

                <!-- Stat Card 4 -->
                <div class="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
                  <div class="flex items-center justify-between mb-4">
                    <div class="bg-[var(--accent4-color)] bg-opacity-10 rounded-full p-3">
                      <i class="fas fa-download text-[var(--accent4-color)] text-2xl" aria-hidden="true"></i>
                    </div>
                    <span class="text-[var(--accent3-color)] text-sm font-semibold">
                      <i class="fas fa-arrow-up mr-1" aria-hidden="true"></i>15%
                    </span>
                  </div>
                  <h3 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-1">
                    156
                  </h3>
                  <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] text-sm">
                    VCF Downloads
                  </p>
                </div>
              </div>

              <!-- Main Content Grid -->
              <div class="grid lg:grid-cols-3 gap-8">
                <!-- Left Column - Profiles -->
                <div class="lg:col-span-2 space-y-8">
                  <!-- My Profiles Section -->
                  <div class="bg-white rounded-xl shadow-lg p-6">
                    <div class="flex items-center justify-between mb-6">
                      <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                        My Profiles
                      </h2>
                      <a href="https://app.landingsite.ai/profile-create"
                        class="inline-flex items-center text-[var(--primary-color)] hover:text-[var(--primary-button-hover-bg-color)] font-semibold transition-colors">
                        <i class="fas fa-plus-circle mr-2" aria-hidden="true"></i>
                        Create New
                      </a>
                    </div>

                    <!-- Profile Cards -->
                    <div class="space-y-4">
                      <!-- Profile 1 -->
                      <div
                        class="border-2 border-[var(--light-border-color)] rounded-lg p-4 hover:border-[var(--primary-color)] transition-colors">
                        <div class="flex items-center justify-between mb-3">
                          <div class="flex items-center gap-4">
                            <div
                              class="w-12 h-12 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-full flex items-center justify-center text-white font-bold">
                              <i class="fas fa-briefcase" aria-hidden="true"></i>
                            </div>
                            <div>
                              <h3 class="font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                                My Business Profile
                              </h3>
                              <p class="text-sm text-[var(--gray-text-color)]">
                                Business Profile
                              </p>
                            </div>
                          </div>
                          <div class="flex items-center gap-2">
                            <span
                              class="bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)] px-3 py-1 rounded-full text-xs font-semibold">
                              ACTIVE
                            </span>
                          </div>
                        </div>

                        <div class="grid grid-cols-3 gap-4 mb-4 text-center">
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              842
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Views</p>
                          </div>
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              123
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Shares</p>
                          </div>
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              287
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Clicks</p>
                          </div>
                        </div>

                        <div class="flex gap-2">
                          <a href="https://app.landingsite.ai/profile/business"
                            class="flex-1 text-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] py-2 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors text-sm font-semibold">
                            <i class="fas fa-eye mr-1" aria-hidden="true"></i> View
                          </a>
                          <a href="#"
                            class="flex-1 text-center bg-[var(--secondary-button-bg-color)] text-[var(--secondary-button-text-color)] py-2 rounded-lg hover:bg-[var(--secondary-button-hover-bg-color)] transition-colors text-sm font-semibold">
                            <i class="fas fa-edit mr-1" aria-hidden="true"></i> Edit
                          </a>
                          <a href="#"
                            class="text-center bg-[var(--light-background-color)] text-[var(--gray-text-color)] px-4 py-2 rounded-lg hover:bg-[var(--medium-background-color)] transition-colors text-sm">
                            <i class="fas fa-qrcode" aria-hidden="true"></i>
                          </a>
                        </div>
                      </div>

                      <!-- Profile 2 -->
                      <div
                        class="border-2 border-[var(--light-border-color)] rounded-lg p-4 hover:border-[var(--primary-color)] transition-colors">
                        <div class="flex items-center justify-between mb-3">
                          <div class="flex items-center gap-4">
                            <div
                              class="w-12 h-12 bg-gradient-to-br from-[var(--accent2-color)] to-[var(--accent-color)] rounded-full flex items-center justify-center text-white font-bold">
                              <i class="fas fa-user" aria-hidden="true"></i>
                            </div>
                            <div>
                              <h3 class="font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                                Personal Profile
                              </h3>
                              <p class="text-sm text-[var(--gray-text-color)]">
                                Personal/Resume
                              </p>
                            </div>
                          </div>
                          <div class="flex items-center gap-2">
                            <span
                              class="bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)] px-3 py-1 rounded-full text-xs font-semibold">
                              ACTIVE
                            </span>
                          </div>
                        </div>

                        <div class="grid grid-cols-3 gap-4 mb-4 text-center">
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              442
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Views</p>
                          </div>
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              219
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Shares</p>
                          </div>
                          <div>
                            <p class="text-2xl font-bold text-[var(--dark-text-color)]">
                              391
                            </p>
                            <p class="text-xs text-[var(--gray-text-color)]">Clicks</p>
                          </div>
                        </div>

                        <div class="flex gap-2">
                          <a href="https://app.landingsite.ai/profile/personal"
                            class="flex-1 text-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] py-2 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors text-sm font-semibold">
                            <i class="fas fa-eye mr-1" aria-hidden="true"></i> View
                          </a>
                          <a href="#"
                            class="flex-1 text-center bg-[var(--secondary-button-bg-color)] text-[var(--secondary-button-text-color)] py-2 rounded-lg hover:bg-[var(--secondary-button-hover-bg-color)] transition-colors text-sm font-semibold">
                            <i class="fas fa-edit mr-1" aria-hidden="true"></i> Edit
                          </a>
                          <a href="#"
                            class="text-center bg-[var(--light-background-color)] text-[var(--gray-text-color)] px-4 py-2 rounded-lg hover:bg-[var(--medium-background-color)] transition-colors text-sm">
                            <i class="fas fa-qrcode" aria-hidden="true"></i>
                          </a>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Recent Activity -->
                  <div class="bg-white rounded-xl shadow-lg p-6">
                    <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-6">
                      Recent Activity
                    </h2>

                    <div class="space-y-4">
                      <div class="flex items-start gap-4 pb-4 border-b border-[var(--light-border-color)]">
                        <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full p-3 flex-shrink-0">
                          <i class="fas fa-eye text-[var(--primary-color)]" aria-hidden="true"></i>
                        </div>
                        <div class="flex-1">
                          <p class="text-[var(--dark-text-color)] font-semibold mb-1">
                            Profile Viewed
                          </p>
                          <p class="text-sm text-[var(--gray-text-color)]">
                            Your business profile was viewed by a visitor from Cape Town
                          </p>
                          <p class="text-xs text-[var(--gray-text-color)] mt-1">
                            2 hours ago
                          </p>
                        </div>
                      </div>

                      <div class="flex items-start gap-4 pb-4 border-b border-[var(--light-border-color)]">
                        <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-3 flex-shrink-0">
                          <i class="fas fa-share-alt text-[var(--accent3-color)]" aria-hidden="true"></i>
                        </div>
                        <div class="flex-1">
                          <p class="text-[var(--dark-text-color)] font-semibold mb-1">
                            Profile Shared
                          </p>
                          <p class="text-sm text-[var(--gray-text-color)]">
                            Someone shared your personal profile via WhatsApp
                          </p>
                          <p class="text-xs text-[var(--gray-text-color)] mt-1">
                            5 hours ago
                          </p>
                        </div>
                      </div>

                      <div class="flex items-start gap-4 pb-4 border-b border-[var(--light-border-color)]">
                        <div class="bg-[var(--accent4-color)] bg-opacity-10 rounded-full p-3 flex-shrink-0">
                          <i class="fas fa-download text-[var(--accent4-color)]" aria-hidden="true"></i>
                        </div>
                        <div class="flex-1">
                          <p class="text-[var(--dark-text-color)] font-semibold mb-1">
                            Contact Downloaded
                          </p>
                          <p class="text-sm text-[var(--gray-text-color)]">
                            A visitor downloaded your contact as VCF file
                          </p>
                          <p class="text-xs text-[var(--gray-text-color)] mt-1">
                            1 day ago
                          </p>
                        </div>
                      </div>

                      <div class="flex items-start gap-4">
                        <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-full p-3 flex-shrink-0">
                          <i class="fas fa-mouse-pointer text-[var(--accent2-color)]" aria-hidden="true"></i>
                        </div>
                        <div class="flex-1">
                          <p class="text-[var(--dark-text-color)] font-semibold mb-1">
                            Link Clicked
                          </p>
                          <p class="text-sm text-[var(--gray-text-color)]">
                            Someone clicked your website link from your profile
                          </p>
                          <p class="text-xs text-[var(--gray-text-color)] mt-1">
                            2 days ago
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Right Column - Quick Actions & Plan -->
                <div class="lg:col-span-1 space-y-6">
                  <!-- Current Plan -->
                  <div
                    class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-xl shadow-lg p-6 text-white">
                    <div class="flex items-center justify-between mb-4">
                      <h3 class="text-lg font-bold font-[var(--font-family-heading)]">
                        Current Plan
                      </h3>
                      <i class="fas fa-crown text-[var(--accent4-color)] text-2xl" aria-hidden="true"></i>
                    </div>
                    <p class="text-3xl font-bold mb-2">Professional</p>
                    <p class="text-white text-opacity-90 text-sm mb-4">
                      3 profiles available
                    </p>

                    <div class="bg-white bg-opacity-20 rounded-lg p-3 mb-4">
                      <div class="flex justify-between text-sm mb-1">
                        <span>Profiles Used</span>
                        <span class="font-semibold">2 / 3</span>
                      </div>
                      <div class="w-full bg-white bg-opacity-30 rounded-full h-2">
                        <div class="bg-white rounded-full h-2" style="width:66%"></div>
                      </div>
                    </div>

                    <a href="https://app.landingsite.ai/pricing"
                      class="block w-full text-center bg-white text-[var(--primary-color)] py-3 rounded-lg hover:bg-opacity-90 transition-all duration-200 font-bold">
                      Upgrade Plan
                    </a>
                  </div>

                  <!-- Quick Actions -->
                  <div class="bg-white rounded-xl shadow-lg p-6">
                    <h3 class="text-lg font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
                      Quick Actions
                    </h3>

                    <div class="space-y-3">
                      <a href="https://app.landingsite.ai/profile-create"
                        class="flex items-center gap-3 p-3 bg-[var(--light-background-color)] rounded-lg hover:bg-[var(--medium-background-color)] transition-colors group">
                        <div
                          class="bg-[var(--primary-color)] bg-opacity-10 rounded-lg p-2 group-hover:bg-opacity-20 transition-colors">
                          <i class="fas fa-plus text-[var(--primary-color)]" aria-hidden="true"></i>
                        </div>
                        <span class="text-[var(--dark-text-color)] font-semibold text-sm">Create New Profile</span>
                      </a>

                      <a href="#"
                        class="flex items-center gap-3 p-3 bg-[var(--light-background-color)] rounded-lg hover:bg-[var(--medium-background-color)] transition-colors group">
                        <div
                          class="bg-[var(--accent3-color)] bg-opacity-10 rounded-lg p-2 group-hover:bg-opacity-20 transition-colors">
                          <i class="fas fa-qrcode text-[var(--accent3-color)]" aria-hidden="true"></i>
                        </div>
                        <span class="text-[var(--dark-text-color)] font-semibold text-sm">Download QR Code</span>
                      </a>

                      <a href="#"
                        class="flex items-center gap-3 p-3 bg-[var(--light-background-color)] rounded-lg hover:bg-[var(--medium-background-color)] transition-colors group">
                        <div
                          class="bg-[var(--accent2-color)] bg-opacity-10 rounded-lg p-2 group-hover:bg-opacity-20 transition-colors">
                          <i class="fas fa-chart-line text-[var(--accent2-color)]" aria-hidden="true"></i>
                        </div>
                        <span class="text-[var(--dark-text-color)] font-semibold text-sm">View Full Analytics</span>
                      </a>

                      <a href="#"
                        class="flex items-center gap-3 p-3 bg-[var(--light-background-color)] rounded-lg hover:bg-[var(--medium-background-color)] transition-colors group">
                        <div
                          class="bg-[var(--accent4-color)] bg-opacity-10 rounded-lg p-2 group-hover:bg-opacity-20 transition-colors">
                          <i class="fas fa-cog text-[var(--accent4-color)]" aria-hidden="true"></i>
                        </div>
                        <span class="text-[var(--dark-text-color)] font-semibold text-sm">Account Settings</span>
                      </a>
                    </div>
                  </div>

                  <!-- NFC Products -->
                  <div class="bg-white rounded-xl shadow-lg p-6 border-2 border-[var(--primary-color)]">
                    <div class="text-center">
                      <div
                        class="bg-[var(--primary-color)] bg-opacity-10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-mobile-alt text-[var(--primary-color)] text-3xl" aria-hidden="true"></i>
                      </div>
                      <h3
                        class="text-lg font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                        Get NFC Card
                      </h3>
                      <p class="text-sm text-[var(--gray-text-color)] mb-4">
                        Share your profile with a simple tap using NFC technology
                      </p>
                      <a href="https://app.landingsite.ai/pricing#nfc"
                        class="inline-flex items-center text-[var(--primary-color)] hover:text-[var(--primary-button-hover-bg-color)] font-semibold transition-colors">
                        Learn More <i class="fas fa-arrow-right ml-2" aria-hidden="true"></i>
                      </a>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </div>
        <div>
          <footer id="global-footer" class="code-section bg-[var(--dark-background-color)] text-white py-12 sm:py-16">
            <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-8 mb-12">
                <!-- Brand Column -->
                <div class="col-span-1">
                  <a href="https://app.landingsite.ai/" class="inline-block mb-4">
                    <img src="/src/assets/images/logo.png" alt="busprofile" class="h-28 py-2" data-logo
                      data-sf-original-src="https://imagedelivery.net/xaKlCos5cTg_1RWzIu_h-A/57adc6f7-d5b3-4905-5aa2-8c11e8559b00/public">
                  </a>
                  <p class="text-gray-400 font-[var(--font-family-body)] mb-6 leading-relaxed">
                    Create professional digital profiles and business cards. Share your
                    contact information effortlessly with NFC technology.
                  </p>
                  <!-- Social Media -->
                  <div class="flex gap-4">
                    <a href="#"
                      class="w-10 h-10 bg-white bg-opacity-10 rounded-full flex items-center justify-center hover:bg-[var(--primary-color)] transition-colors duration-200">
                      <i class="fab fa-facebook-f" aria-hidden="true"></i>
                    </a>
                    <a href="#"
                      class="w-10 h-10 bg-white bg-opacity-10 rounded-full flex items-center justify-center hover:bg-[var(--primary-color)] transition-colors duration-200">
                      <i class="fab fa-twitter" aria-hidden="true"></i>
                    </a>
                    <a href="#"
                      class="w-10 h-10 bg-white bg-opacity-10 rounded-full flex items-center justify-center hover:bg-[var(--primary-color)] transition-colors duration-200">
                      <i class="fab fa-linkedin-in" aria-hidden="true"></i>
                    </a>
                    <a href="#"
                      class="w-10 h-10 bg-white bg-opacity-10 rounded-full flex items-center justify-center hover:bg-[var(--primary-color)] transition-colors duration-200">
                      <i class="fab fa-instagram" aria-hidden="true"></i>
                    </a>
                  </div>
                </div>

                <!-- Quick Links -->
                <div>
                  <h3 class="text-lg font-bold font-[var(--font-family-heading)] mb-4">
                    Quick Links
                  </h3>
                  <ul class="space-y-3 font-[var(--font-family-body)]">
                    <li>
                      <a href="https://app.landingsite.ai/"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Home</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/categories"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Business
                        Categories</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/search"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Search
                        Businesses</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/pricing"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Pricing</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/help"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Help
                        Center</a>
                    </li>
                  </ul>
                </div>

                <!-- Account -->
                <div>
                  <h3 class="text-lg font-bold font-[var(--font-family-heading)] mb-4">
                    Account
                  </h3>
                  <ul class="space-y-3 font-[var(--font-family-body)]">
                    <li>
                      <a href="https://app.landingsite.ai/login"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Login</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/register"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Register</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/dashboard"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Dashboard</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/profile/business"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Business
                        Profile</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/profile/personal"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Personal
                        Profile</a>
                    </li>
                  </ul>
                </div>

                <!-- Legal -->
                <div>
                  <h3 class="text-lg font-bold font-[var(--font-family-heading)] mb-4">
                    Legal
                  </h3>
                  <ul class="space-y-3 font-[var(--font-family-body)]">
                    <li>
                      <a href="https://app.landingsite.ai/terms"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Terms of
                        Service</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/privacy"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Privacy
                        Policy</a>
                    </li>
                    <li>
                      <a href="https://app.landingsite.ai/contact"
                        class="text-gray-400 hover:text-[var(--primary-color)] transition-colors duration-200">Contact
                        Us</a>
                    </li>
                  </ul>
                </div>
              </div>

              <!-- Bottom Bar -->
              <div class="border-t border-gray-700 pt-8">
                <div class="flex flex-col sm:flex-row justify-between items-center gap-4">
                  <p class="text-gray-400 font-[var(--font-family-body)] text-sm text-center sm:text-left">
                    © 2025 busprofile. All rights reserved.
                  </p>
                  <p class="text-gray-400 font-[var(--font-family-body)] text-sm text-center sm:text-right">
                    Made with <i class="fas fa-heart text-[var(--accent-color)]" aria-hidden="true"></i> for
                    better networking
                  </p>
                </div>
              </div>
            </div>
          </footer>
        </div>
      </div>
    </div>
  </div>

  <div class="dashboard-page">
    <div class="container py-5">
      <div class="row mb-4">
        <div class="col">
          <h1>Welcome back, {{ user?.firstName }}!</h1>
          <p class="text-muted">Manage your digital business presence</p>
        </div>
      </div>

      <div class="row g-4">
        <div class="col-md-4">
          <div class="dashboard-card" @click="router.push('/dashboard/profile')">
            <i class="fas fa-id-card fa-3x text-primary mb-3"></i>
            <h4>Business Profiles</h4>
            <p class="text-muted">Create and manage your digital business cards</p>
          </div>
        </div>

        <div class="col-md-4">
          <div class="dashboard-card" @click="router.push('/dashboard/menu')">
            <i class="fas fa-utensils fa-3x text-success mb-3"></i>
            <h4>Menus & Catalogs</h4>
            <p class="text-muted">Manage your menu items and product catalogs</p>
          </div>
        </div>

        <div class="col-md-4">
          <div class="dashboard-card" @click="router.push('/dashboard/tables')">
            <i class="fas fa-qrcode fa-3x text-info mb-3"></i>
            <h4>NFC Tables</h4>
            <p class="text-muted">Manage your NFC table tags</p>
          </div>
        </div>

        <div class="col-md-4">
          <div class="dashboard-card" @click="router.push('/dashboard/subscription')">
            <i class="fas fa-credit-card fa-3x text-warning mb-3"></i>
            <h4>Subscription</h4>
            <p class="text-muted">View and manage your subscription</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-page {
  min-height: calc(100vh - 56px);
  background: #f8f9fa;
}

.dashboard-card {
  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  text-align: center;
  cursor: pointer;
  transition: transform 0.3s, box-shadow 0.3s;
  height: 100%;
}

.dashboard-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.dashboard-card h4 {
  margin-bottom: 10px;
}
</style>

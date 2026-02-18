<template>
  <div>
    
    
    

    <section class="code-section bg-[var(--light-background-color)] min-h-screen pb-12" id="s4wbit">

      <div class="bg-gradient-to-br from-[var(--dark-background-color)] to-[var(--primary-color)] text-white py-16 px-4">
        <div class="max-w-7xl mx-auto text-center">
          <h1 class="text-4xl lg:text-5xl font-bold font-[var(--font-family-heading)] mb-4">
            BizBio Connect
          </h1>
          <p class="text-xl text-white text-opacity-90 mb-8">
            Digital Business Cards for Professionals & Small Businesses
          </p>

          <!-- <div class="inline-flex items-center gap-4 bg-white bg-opacity-10 backdrop-blur-sm rounded-full p-2">
            <button id="monthly-toggle"
              class="px-6 py-2 rounded-full font-semibold bg-white text-md-primary transition-all">
              Monthly
            </button>
            <button id="annual-toggle"
              class="px-6 py-2 rounded-full font-semibold text-white hover:bg-white hover:bg-opacity-10 transition-all">
              Annual
              <span class="ml-2 text-xs bg-[var(--accent3-color)] px-2 py-1 rounded-full">Save 20%</span>
            </button>
          </div> -->
        </div>
      </div>

      <div class="px-4 py-12 -mt-20">
        <div class="max-w-7xl mx-auto">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <!-- <div  v-for="(tier, index) in pricingTiers" :key="tier.id" 
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  {{ tier.displayName }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  {{ tier.description }}
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R{{ tier.monthlyPrice }}</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Up to 5 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Digital menu (1 menu)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Email support</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div> -->
           
            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Free
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  Perfect for trying out BizBio
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">FREE</span>
                <!-- <span class="text-[var(--gray-text-color)]">/month</span>
                <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  R2,871/year (save R717)
                </p> -->
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>1 digital profile</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic customisation (Choose from 5 templates)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>QR code generation</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>5 Social media links</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Mobile responsive</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-times text-[var(--danger)] mt-1" aria-hidden="true"></i>
                  <span>vCard download</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-times text-[var(--danger)] mt-1" aria-hidden="true"></i>
                  <span>Analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-times text-[var(--danger)] mt-1" aria-hidden="true"></i>
                  <span>Custom branding</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-times text-[var(--danger)] mt-1" aria-hidden="true"></i>
                  <span>Remove BizBio badge</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-md-primary p-8 hover:shadow-2xl transition-all relative transform lg:scale-105">
              <div class="absolute -top-4 left-1/2 -translate-x-1/2">
                <span class="bg-md-primary text-white px-4 py-1 rounded-full text-xs font-bold">
                  MOST POPULAR
                </span>
              </div>
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-md-primary bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-user text-[var(--md-on-accent)] text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Solo
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  For individual professionals
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R59</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>1 digital profile</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Custom branding (colors, fonts, logo)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Premium templates</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>QR code generation</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Unlimited social media links</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Mobile responsive</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>vCard Download</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Analytics dashboard</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Remove BizBio badge</span>
                </li>
              </ul>
              <button
                class="w-full bg-md-primary text-white py-3 rounded-lg font-bold hover:bg-[var(--primary-button-hover-bg-color)] transition-colors">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent2-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-users text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Team
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  For small teams
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R99</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
                <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>5 digital profiles</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Custom branding (colors, fonts, logo)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Premium templates</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>QR code generation</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Unlimited social media links</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Mobile responsive</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>vCard Download</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Analytics dashboard</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-primary mt-1" aria-hidden="true"></i>
                  <span>Remove BizBio badge</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-secondary mt-1" aria-hidden="true"></i>
                  <span>* R20/profile beyond tier limit</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent2-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>

            <div
              class="bg-gradient-to-br from-[var(--accent4-color)] to-[var(--accent-color)] text-white rounded-2xl p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-white bg-opacity-20 backdrop-blur-sm rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-crown text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold mb-2">Business</h3>
                <p class="text-sm text-white text-opacity-90">For teams and growing businesses</p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--light-text-color)]">R249</span>
                <span class="text-white">/month</span>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--md-on-accent)] mt-1" aria-hidden="true"></i>
                  <span>20 digital profiles</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--md-on-accent)] mt-1" aria-hidden="true"></i>
                  <span>All Pro features</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--md-on-accent)] mt-1" aria-hidden="true"></i>
                  <span>Team member management</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--md-on-accent)] mt-1" aria-hidden="true"></i>
                  <span>Organisational structure</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--md-on-accent)] mt-1" aria-hidden="true"></i>
                  <span>Advanced Analytics</span>
                </li>
                
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-md-secondary mt-1" aria-hidden="true"></i>
                  <span>* R20/profile beyond tier limit</span>
                </li>
              </ul>
              <button
                class="w-full bg-white text-[var(--accent-color)] py-3 rounded-lg font-bold hover:bg-opacity-90 transition-opacity">
                Contact Sales
              </button>
            </div>
          </div>
        </div>
      </div>  

      <div class="bg-gradient-to-br from-[var(--accent4-color)] to-[var(--primary-color)] text-white py-16 px-4 pt-16 mt-16">
        <div class="max-w-7xl mx-auto text-center">
          <h1 class="text-4xl lg:text-5xl font-bold font-[var(--font-family-heading)] mb-4">
            BizBio Menu
          </h1>
          <p class="text-xl text-white text-opacity-90 mb-8">
            Digital Restaurant Menus - Save R5,000-R20,000/year on printing
          </p>

          <!-- <div class="inline-flex items-center gap-4 bg-white bg-opacity-10 backdrop-blur-sm rounded-full p-2">
            <button id="monthly-toggle"
              class="px-6 py-2 rounded-full font-semibold bg-white text-md-primary transition-all">
              Monthly
            </button>
            <button id="annual-toggle"
              class="px-6 py-2 rounded-full font-semibold text-white hover:bg-white hover:bg-opacity-10 transition-all">
              Annual
              <span class="ml-2 text-xs bg-[var(--accent3-color)] px-2 py-1 rounded-full">Save 20%</span>
            </button>
          </div> -->
        </div>
      </div>

      <div class="px-4 py-12 -mt-20">
        <div class="max-w-7xl mx-auto">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <!-- <div  v-for="(tier, index) in pricingTiers" :key="tier.id" 
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  {{ tier.displayName }}
                </h3>
                <p class="text-sm text-[var(--gray-text-color)]">
                  {{ tier.description }}
                </p>
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R{{ tier.monthlyPrice }}</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
              <ul class="space-y-3 mb-8">
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Up to 5 table personalities</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Digital menu (1 menu)</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>NFC tag support</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Basic analytics</span>
                </li>
                <li class="flex items-start gap-2 text-sm">
                  <i class="fas fa-check text-[var(--accent3-color)] mt-1" aria-hidden="true"></i>
                  <span>Email support</span>
                </li>
              </ul>
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div> -->
           
            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent3-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-rocket text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Starter
                </h3>
                <!-- <p class="text-sm text-[var(--gray-text-color)]">
                  Perfect for trying out BizBio
                </p> -->
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R149</span>
                <!-- <span class="text-[var(--gray-text-color)]">/month</span>
                <p class="text-xs text-[var(--gray-text-color)] mt-1">
                  R2,871/year (save R717)
                </p> -->
              </div>
              <ul class="space-y-3 mb-8 text-sm">
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">1 restaurant profile</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Up to 50 menu items</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">5 categories</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">1 image per item</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Basic item details</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Portion sizes/variants</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Mobile-optimised menu</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Featured section</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-times text-red-400 mt-1 mr-3"></i>
                                <span class="text-gray-700">No allergen information</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-times text-red-400 mt-1 mr-3"></i>
                                <span class="text-gray-700">No dietary tags</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-times text-red-400 mt-1 mr-3"></i>
                                <span class="text-gray-700">No item add-ons</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-times text-red-400 mt-1 mr-3"></i>
                                <span class="text-gray-700">No analytics</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-times text-red-400 mt-1 mr-3"></i>
                                <span class="text-gray-700">Not elligible for Menu Pro add-on</span>
                            </li>
                        
                        </ul>
              
              <button
                class="w-full bg-[var(--accent3-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-md-primary p-8 hover:shadow-2xl transition-all relative transform lg:scale-105">
              <div class="absolute -top-4 left-1/2 -translate-x-1/2">
                <span class="bg-md-primary text-white px-4 py-1 rounded-full text-xs font-bold">
                  MOST POPULAR
                </span>
              </div>
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-md-primary bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-user text-[var(--md-on-accent)] text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Entrée
                </h3>
                <!-- <p class="text-sm text-[var(--gray-text-color)]">
                  For individual professionals
                </p> -->
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R299</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
              <ul class="space-y-3 mb-8 text-sm">
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">1 restaurant profile</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Up to 200 menu items</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">15 categories</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">5 images per item</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Portion sizes/variants</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Item add-ons / extras</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Additional featured section</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Allergen information</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Dietary tags</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Spice level indicators</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Custom branding</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Analytics dashboard</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Import menu items</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700"><strong>Elligible</strong> for Menu Pro add-on</span>
                            </li>
                        </ul>
              <button
                class="w-full bg-md-primary text-white py-3 rounded-lg font-bold hover:bg-[var(--primary-button-hover-bg-color)] transition-colors">
                Select Plan
              </button>
            </div>

            <div
              class="bg-white rounded-2xl border-2 border-[var(--light-border-color)] p-8 hover:shadow-2xl transition-all">
              <div class="text-center mb-6">
                <div
                  class="w-16 h-16 bg-[var(--accent2-color)] bg-opacity-10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-users text-white text-2xl" aria-hidden="true"></i>
                </div>
                <h3 class="text-2xl font-bold text-[var(--dark-text-color)] mb-2">
                  Banquet
                </h3>
                <!-- <p class="text-sm text-[var(--gray-text-color)]">
                  For small teams
                </p> -->
              </div>
              <div class="text-center mb-6">
                <span class="text-4xl font-bold text-[var(--dark-text-color)]">R599</span>
                <span class="text-[var(--gray-text-color)]">/month</span>
              </div>
                <ul class="space-y-3 mb-8 text-sm">
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700"><strong>Unlimited locations</strong></span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700"><strong>Multiple menus</strong></span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">150 menu items per location / menu</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Unlimited categories &amp; images</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Location-specific menus</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Centralized menu management</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Account per location</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Nutritional information</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Portion sizes/variants</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Item add-ons / extras</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Allergen information</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Dietary tags</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Spice level indicators</span>
                            </li>
                        
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Custom branding</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Analytics dashboard</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700">Import menu items</span>
                            </li>
                            <li class="flex items-start">
                                <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                                <span class="text-gray-700"><strong>Elligible</strong> for Menu Pro add-on</span>
                            </li>
                        </ul>
              <button
                class="w-full bg-[var(--accent2-color)] text-white py-3 rounded-lg font-bold hover:opacity-90 transition-opacity">
                Select Plan
              </button>
            </div>
          </div>
        </div>
      </div>

      
    </section>
  </div>
</template>

<script setup>
const subscriptionsApi = useSubscriptionsApi()

const loading = ref(true)
const error = ref(null)
const pricingTiers = ref([])

const loadPricingTiers = async () => {
  loading.value = true
  error.value = null

  try {
    const response = await subscriptionsApi.getTiers()
    console.log(response);
    console.log(response.data);
    // Ensure we always set an array
    if (response.data && Array.isArray(response.data.tiers)) {
      pricingTiers.value = response.data.tiers
    } else if (Array.isArray(response.data)) {
      pricingTiers.value = response.data
    } else {
      pricingTiers.value = []
    }
  } catch (err) {
    console.error('Failed to load pricing tiers:', err)

    // Fallback to default tiers if API fails
    pricingTiers.value = [
      {
        id: '1',
        name: 'Free',
        price: 0,
        description: 'Perfect for trying out',
        features: [
          { text: '1 Business Profile', included: true },
          { text: 'Basic analytics', included: true },
          { text: 'QR code sharing', included: true },
          { text: 'No custom branding', included: false }
        ],
        buttonText: 'Get Started',
        buttonLink: '/register',
        popular: false
      },
      {
        id: '2',
        name: 'Professional',
        price: 9,
        description: 'For serious professionals',
        features: [
          { text: '5 Business Profiles', included: true },
          { text: 'Advanced analytics', included: true },
          { text: 'Custom branding', included: true },
          { text: 'Priority support', included: true },
          { text: 'NFC card integration', included: true }
        ],
        buttonText: 'Start Free Trial',
        buttonLink: '/register',
        popular: true
      },
      {
        id: '3',
        name: 'Business',
        price: 29,
        description: 'For growing teams',
        features: [
          { text: 'Unlimited profiles', included: true },
          { text: 'Team management', included: true },
          { text: 'White-label options', included: true },
          { text: 'API access', included: true },
          { text: 'Dedicated support', included: true }
        ],
        buttonText: 'Contact Sales',
        buttonLink: '/contact',
        popular: false
      }
    ]

    error.value = 'Using default pricing. Could not connect to API.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadPricingTiers()
})

useHead({
  title: 'Pricing',
})
</script>





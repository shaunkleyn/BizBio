<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const features = ref([
  {
    icon: 'fas fa-id-card',
    colour: 'primary-color',
    title: 'Digital Business Cards',
    description: 'Create professional digital business cards that can be shared instantly via link, QR code, or NFC tap. No more paper waste.'
  },
  {
    icon: 'fas fa-palette',
    colour: 'accent2-color',
    title: 'Custom Branding',
    description: 'Choose your colors, fonts, logo, and icons to match your brand identity. Make every profile uniquely yours.'
  },
  {
    icon: 'fas fa-mobile-alt',
    colour: 'accent3-color',
    title: 'NFC Technology',
    description: 'Get a physical NFC card or keyring that people can tap with their phone to instantly view your profile. Future-ready networking.'
  },
  {
    icon: 'fas fa-chart-bar',
    colour: 'accent-color',
    title: 'Analytics Dashboard',
    description: 'Track profile visits, shares, and clicks. See who\'s viewing your profile and measure your networking ROI.'
  },
  {
    icon: 'fas fa-address-book',
    colour: 'accent4-color',
    title: 'Easy Contact Saving',
    description: 'Let people save your contact details directly to their phone with one click using VCF files. No manual typing required.'
  },
  {
    icon: 'fas fa-users',
    colour: 'primary-color',
    title: 'Multiple Profiles',
    description: 'Create separate profiles for your business, personal brand, or resume. Switch between them effortlessly.'
  }
])

const steps = ref([
  {
    number: 1,
    colour: 'primary-color',
    title: 'Sign Up & Choose Template',
    description: 'Create your free account and select from our professionally designed profile templates. Pick one that matches your style and industry.'
  },
  {
    number: 2,
    colour: 'accent3-color',
    title: 'Customise Your Profile',
    description: 'Add your logo, contact details, social links, business hours, and location. Apply your brand colors and choose icons that represent you.'
  },
  {
    number: 3,
    colour: 'accent2-color',
    title: 'Share & Connect',
    description: 'Share your profile via link, QR code, or NFC card. Start networking smarter and track your engagement in real-time.'
  }
])

const businesses = ref([
  {
    colour: 'primary-color',
    title: 'Search by Category',
    description: 'Find plumbers, electricians, consultants, designers, and hundreds of other business categories. Filter by location and ratings.'
  },
  {
    colour: 'accent3-color',
    title: 'Ratings & Reviews',
    description: 'See what others are saying. Connect to Google and Facebook reviews directly from business profiles.'
  },
    {
      colour: 'accent4-color',
    title: 'Location & Hours',
    description: 'View business locations on embedded maps, check operating hours, and get directions instantly.'
  },
])

const categories = ref([
  'Real Estate Agents',
  'Financial Advisors',
  'Healthcare Professionals',
  'Legal Services',
  'Tech & IT Consultants',
  'Management Consultants'
])
</script>

<template>
  <section class="code-section relative bg-gradient-to-br from-[var(--light-background-color)] to-white overflow-hidden"
    id=szvafv>

    <div class="absolute top-20 right-10 w-72 h-72 bg-[var(--primary-color)] rounded-full opacity-10 blur-3xl">
    </div>
    <div class="absolute bottom-20 left-10 w-96 h-96 bg-[var(--accent3-color)] rounded-full opacity-10 blur-3xl">
    </div>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 sm:py-20 lg:py-28">
      <div class="grid lg:grid-cols-2 gap-12 lg:gap-16 items-center">

        <div class="text-center lg:text-left">
          <h1
            class="text-4xl sm:text-5xl lg:text-6xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] leading-tight mb-6">
            Share Your Business Profile
            <span class=text-[var(--primary-color)]>Without the Hassle</span>
          </h1>
          <p
            class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-4 leading-relaxed">
            Create stunning digital business cards and profiles in minutes. One
            tap to share all your contact details, social links, and business
            information with potential clients.
          </p>
          <p class="text-base sm:text-lg text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-8">
            Perfect for professionals, freelancers, and businesses who want to
            make a lasting impression.
          </p>

          <div class="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start mb-6">
            <router-link to="/register"
                class="inline-flex items-center justify-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] text-lg shadow-lg hover:shadow-xl transform hover:-translate-y-0.5">
                <i class="fas fa-rocket mr-2" aria-hidden=true></i>
                Get Started Free
              </router-link>
             <router-link to="/pricing"
                class="inline-flex items-center justify-center bg-white text-[var(--primary-color)] border-2 border-[var(--primary-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--light-background-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] text-lg shadow-md hover:shadow-lg">
                <i class="fas fa-rocket mr-2" aria-hidden=true></i>
                View Pricing
              </router-link>
          </div>

          <div
            class="flex flex-wrap items-center justify-center lg:justify-start gap-6 text-sm text-[var(--gray-text-color)]">
            <div class="flex items-center">
              <i class="fas fa-check-circle text-[var(--accent3-color)] mr-2" aria-hidden=true></i>
              <span>No credit card required</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle text-[var(--accent3-color)] mr-2" aria-hidden=true></i>
              <span>Setup in 5 minutes</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle text-[var(--accent3-color)] mr-2" aria-hidden=true></i>
              <span>Cancel anytime</span>
            </div>
          </div>
        </div>

        <div class=relative>
          <div
            class="relative rounded-2xl overflow-hidden shadow-2xl transform hover:scale-105 transition-transform duration-300">
            <img src="/src/assets/images/image_1.avif"
              alt="Recruitment and candidate selection concept. Businessman interacts with digital interface with multiple candidate profiles on laptop. Represents online recruitment, HR technology, hiring process,"
              class="w-full h-auto" data-media='{"id":"2201230874","src":"iStock","type":"image"}'>

            <div
              class="absolute top-6 right-6 bg-white rounded-lg shadow-xl p-4 backdrop-blur-sm bg-opacity-95 animate-float">
              <div class="flex items-center gap-3">
                <div class="bg-[var(--accent3-color)] bg-opacity-20 rounded-full p-3">
                  <i class="fas fa-users text-[var(--accent3-color)] text-xl" aria-hidden=true></i>
                </div>
                <div>
                  <p class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                    10K+
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Active Profiles
                  </p>
                </div>
              </div>
            </div>
            <div
              class="absolute bottom-6 left-6 bg-white rounded-lg shadow-xl p-4 backdrop-blur-sm bg-opacity-95 animate-float"
              style="animation-delay:0.5s">
              <div class="flex items-center gap-3">
                <div class="bg-[var(--primary-color)] bg-opacity-20 rounded-full p-3">
                  <i class="fas fa-chart-line text-[var(--primary-color)] text-xl" aria-hidden=true></i>
                </div>
                <div>
                  <p class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)]">
                    98%
                  </p>
                  <p class="text-xs text-[var(--gray-text-color)]">
                    Satisfaction Rate
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

  </section>
  <div class="min-h-screen bg-white">
    <!-- Hero Section -->
    <section class="relative bg-gradient-to-br from-primary to-accent-purple overflow-hidden py-20 sm:py-32">
      <!-- Decorative background elements -->
      <div class="absolute inset-0 opacity-10">
        <div class="absolute top-20 left-10 w-72 h-72 bg-white rounded-full blur-3xl"></div>
        <div class="absolute bottom-20 right-10 w-96 h-96 bg-white rounded-full blur-3xl"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <h1 class="text-4xl sm:text-5xl lg:text-6xl font-heading font-bold text-white mb-6 leading-tight">
            Share Your Business Profile<br />Without the Hassle
          </h1>
          <p class="text-xl sm:text-2xl text-white/90 mb-8 max-w-3xl mx-auto">
            Create your professional digital business card in minutes. Share it instantly with QR codes, NFC, or a
            simple link.
          </p>

          <!-- CTA Buttons -->
          <div class="flex flex-col sm:flex-row gap-4 justify-center mb-12">
            <router-link to="/register"
              class="px-8 py-4 bg-white text-primary rounded-lg hover:bg-gray-50 transition-all shadow-lg hover:shadow-xl font-semibold text-lg">
              <i class="fas fa-rocket mr-2"></i>
              Get Started Free
            </router-link>
            <router-link to="/categories"
              class="px-8 py-4 bg-white/10 text-white border-2 border-white rounded-lg hover:bg-white/20 transition-all font-semibold text-lg backdrop-blur-sm">
              <i class="fas fa-search mr-2"></i>
              Explore Businesses
            </router-link>
          </div>

          <!-- Feature highlights -->
          <div class="flex flex-wrap justify-center gap-6 text-white/90">
            <div class="flex items-center gap-2">
              <i class="fas fa-check-circle"></i>
              <span>100% Free</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-clock"></i>
              <span>5-Min Setup</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-credit-card"></i>
              <span>No Credit Card Needed</span>
            </div>
          </div>
        </div>
      </div>
    </section>


    <section class="code-section bg-white py-16 sm:py-20 lg:py-24" id=stsf9qc>
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

        <div class="text-center mb-16">
          <h2
            class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Everything You Need in One Profile
          </h2>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto">
            Share your contact details, showcase your business, and make networking
            effortless with our all-in-one digital profile solution.
          </p>
        </div>

        <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-8 lg:gap-10">

          <div v-for="(feature, index) in features" :key="index"
            class="group relative bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl transition-all duration-300 hover:-translate-y-2">
            <div
              class="absolute inset-0 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] rounded-xl opacity-0 group-hover:opacity-10 transition-opacity duration-300">
            </div>
            <div class=relative>
              <div
                class="bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-md group-hover:shadow-xl transition-shadow duration-300">
                <i :class="feature.icon + ' text-[var(--' + feature.colour + ')] text-3xl'" aria-hidden=true></i>
              </div>
              <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                {{ feature.title }}
              </h3>
              <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] leading-relaxed">
                {{ feature.description }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- How It Works Section -->
     <section
                        class="code-section bg-gradient-to-br from-[var(--medium-background-color)] to-[var(--light-background-color)] py-16 sm:py-20 lg:py-24 relative overflow-hidden"
                        id=s1zo38>

                        <div
                            class="absolute top-0 right-0 w-96 h-96 bg-[var(--accent3-color)] rounded-full opacity-5 blur-3xl">
                        </div>
                        <div
                            class="absolute bottom-0 left-0 w-96 h-96 bg-[var(--primary-color)] rounded-full opacity-5 blur-3xl">
                        </div>
                        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative z-10">

                            <div class="text-center mb-16">
                                <h2
                                    class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
                                    Get Your Profile Live in 3 Simple Steps
                                </h2>
                                <p
                                    class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto">
                                    No technical skills required. Set up your professional profile in
                                    minutes.
                                </p>
                            </div>
                            <div class="grid lg:grid-cols-2 gap-12 items-center mb-16">

                                <div class="order-2 lg:order-1">
                                    <div class="relative rounded-2xl overflow-hidden shadow-2xl">
                                        <img src="/src/assets/images/image_2.avif"
                                            alt="Human resources and management concept. Employees must complete the online survey form, answer the test questions, and Mark the checklist on a clipboard. Online business concept."
                                            class="w-full h-auto"
                                            data-media='{"id":"1426070859","src":"iStock","type":"image"}'>
                                    </div>
                                </div>

                                <div class="order-1 lg:order-2 space-y-8">

                                    <div  v-for="step in steps" :key="step.number" class="flex gap-6 group">
                                        <div class=flex-shrink-0>
                                            <div :class="'w-16 h-16 bg-[var(--' + step.colour + ')] rounded-full flex items-center justify-center text-white font-bold text-2xl font-[var(--font-family-heading)] shadow-lg group-hover:scale-110 transition-transform duration-300'">
                                                {{ step.number }}
                                            </div>
                                        </div>
                                        <div class=pt-2>
                                            <h3
                                                class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                                                {{step.title}}
                                            </h3>
                                            <p
                                                class="text-[var(--gray-text-color)] font-[var(--font-family-body)] leading-relaxed">
                                                {{step.description}}
                                            </p>
                                        </div>
                                    </div>

                                    

                                    <div class=pt-6>
                                        <router-link to="/register"
                                          class="inline-flex items-center justify-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] text-lg shadow-lg hover:shadow-xl transform hover:-translate-y-0.5">
                                          <i class="fas fa-arrow-right mr-2" aria-hidden=true></i>
                                          Start Building Your Profile
                                        </router-link>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
  

    <!-- Business Discovery Section -->
    <section class="code-section bg-white py-16 sm:py-20 lg:py-24" id=s2ibpr>
                        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

                            <div class="text-center mb-16">
                                <h2
                                    class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
                                    Connect with Local Businesses
                                </h2>
                                <p
                                    class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto">
                                    Browse businesses by category, check ratings, and view detailed
                                    profiles. Find the perfect service provider for your needs.
                                </p>
                            </div>
                            <div class="grid lg:grid-cols-2 gap-12 items-center">

                                <div class=space-y-6>
                                    <div v-for="step in businesses" :key="step.title" 
                                        class="bg-[var(--light-background-color)] rounded-xl p-6 hover:shadow-lg transition-shadow duration-300">
                                        <div class="flex items-start gap-4">
                                            <div
                                                :class="'bg-[var(--' +  step.colour  + ')] bg-opacity-10 rounded-lg p-3 flex-shrink-0'">
                                                <i class="fas fa-search text-white text-2xl"
                                                    aria-hidden=true></i>
                                            </div>
                                            <div>
                                                <h3
                                                    class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                                                    {{step.title}}
                                                </h3>
                                                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                                                    {{step.description}}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    

                                    <div class=pt-4>
                                        <a href=business-categories.html
                                            class="inline-flex items-center justify-center bg-[var(--secondary-button-bg-color)] text-[var(--secondary-button-text-color)] border-2 border-[var(--primary-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--secondary-button-hover-bg-color)] hover:text-[var(--secondary-button-hover-text-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] text-lg shadow-md hover:shadow-lg">
                                            <i class="fas fa-th-large mr-2" aria-hidden=true></i>
                                            Browse All Categories
                                        </a>
                                    </div>
                                </div>

                                <div class=relative>
                                    <div
                                        class="relative rounded-2xl overflow-hidden shadow-2xl transform hover:scale-105 transition-transform duration-300">
                                        <img src="/src/assets/images/image_3.webp"
                                            alt="Human resources analytics and recruitment concept with global network, digital icons, and data interface for modern workforce solutions. 3d Rendering"
                                            class="w-full h-auto"
                                            data-media='{"id":"2231038244","src":"iStock","type":"image"}'>
                                    </div>

                                    <div
                                        class="absolute top-4 left-4 bg-white rounded-lg shadow-xl px-4 py-3 backdrop-blur-sm bg-opacity-95 animate-float">
                                        <div class="flex items-center gap-2">
                                            <i class="fas fa-wrench text-[var(--accent-color)] text-lg"
                                                aria-hidden=true></i>
                                            <span class="font-semibold text-[var(--dark-text-color)] text-sm ">342
                                                Plumbers</span>
                                        </div>
                                    </div>
                                    <div
                                        class="absolute top-20 right-4 bg-white rounded-lg shadow-xl px-4 py-3 backdrop-blur-sm bg-opacity-95 animate-float" style="animation-delay:0.3s">
                                        <div class="flex items-center gap-2">
                                            <i class="fas fa-paint-roller text-[var(--primary-color)] text-lg"
                                                aria-hidden=true></i>
                                            <span class="font-semibold text-[var(--dark-text-color)] text-sm">218
                                                Contractors</span>
                                        </div>
                                    </div>
                                    <div
                                        class="absolute bottom-20 left-4 bg-white rounded-lg shadow-xl px-4 py-3 backdrop-blur-sm bg-opacity-95 animate-float" style="animation-delay:0.6s">
                                        <div class="flex items-center gap-2">
                                            <i class="fas fa-laptop-code text-[var(--accent3-color)] text-lg"
                                                aria-hidden=true></i>
                                            <span class="font-semibold text-[var(--dark-text-color)] text-sm">567
                                                Designers</span>
                                        </div>
                                    </div>
                                    <div
                                        class="absolute bottom-4 right-4 bg-white rounded-lg shadow-xl px-4 py-3 backdrop-blur-sm bg-opacity-95 animate-float" style="animation-delay:0.9s">
                                        <div class="flex items-center gap-2">
                                            <i class="fas fa-briefcase text-[var(--accent2-color)] text-lg"
                                                aria-hidden=true></i>
                                            <span class="font-semibold text-[var(--dark-text-color)] text-sm">429
                                                Consultants</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
<section
                        class="code-section relative bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] py-16 sm:py-20 lg:py-24 overflow-hidden"
                        id=s7zuga>

                        <div class="absolute top-0 right-0 w-96 h-96 bg-white rounded-full opacity-5 blur-3xl"></div>
                        <div class="absolute bottom-0 left-0 w-96 h-96 bg-white rounded-full opacity-5 blur-3xl"></div>

                        <div
                            class="absolute top-20 left-10 w-20 h-20 border-4 border-white opacity-20 rounded-lg transform rotate-12 animate-spin-slow">
                        </div>
                        <div
                            class="absolute bottom-20 right-20 w-16 h-16 border-4 border-white opacity-20 rounded-full animate-bounce-slow">
                        </div>
                        <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 text-center relative z-10">
                            <h2
                                class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white font-[var(--font-family-heading)] mb-6">
                                Ready to Modernize Your Networking?
                            </h2>
                            <p
                                class="text-xl sm:text-2xl text-white text-opacity-95 font-[var(--font-family-body)] mb-4 max-w-3xl mx-auto leading-relaxed">
                                Join thousands of professionals and businesses who are sharing their
                                profiles smarter.
                            </p>
                            <p
                                class="text-lg text-white text-opacity-90 font-[var(--font-family-body)] mb-10 max-w-2xl mx-auto">
                                Create your free digital profile today and start making lasting
                                impressions.
                            </p>

                            <div class="flex flex-col sm:flex-row gap-4 justify-center items-center mb-8">
                                <a href=register.html
                                    class="inline-flex items-center justify-center bg-white text-[var(--primary-color)] px-10 py-5 rounded-[var(--button-rounded-radius)] hover:bg-opacity-90 transition-all duration-200 font-bold font-[var(--font-family-body)] text-lg shadow-2xl hover:shadow-3xl transform hover:-translate-y-1 hover:scale-105">
                                    <i class="fas fa-user-plus mr-3 text-xl" aria-hidden=true></i>
                                    Create Your Profile Now
                                </a>
                                <a href=products.html
                                    class="inline-flex items-center justify-center bg-transparent text-white border-3 border-white px-10 py-5 rounded-[var(--button-rounded-radius)] hover:bg-white hover:text-[var(--primary-color)] transition-all duration-200 font-bold font-[var(--font-family-body)] text-lg shadow-lg">
                                    <i class="fas fa-dollar-sign mr-3 text-xl" aria-hidden=true></i>
                                    View Plans
                                </a>
                                <router-link to="/register"
            class="px-8 py-4 bg-white text-primary rounded-lg hover:bg-gray-50 transition-all shadow-lg hover:shadow-xl font-semibold text-lg">
            <i class="fas fa-user-plus mr-2"></i>
            Create Your Profile Now
          </router-link>
          <router-link to="/pricing"
            class="px-8 py-4 bg-transparent text-white border-2 border-white rounded-lg hover:bg-white/10 transition-all font-semibold text-lg backdrop-blur-sm">
            <i class="fas fa-tag mr-2"></i>
            View Plans
          </router-link>
                            </div>

                            <div class="flex flex-wrap items-center justify-center gap-8 text-white text-opacity-90">
                                <div class="flex items-center gap-2">
                                    <i class="fas fa-shield-alt text-2xl" aria-hidden=true></i>
                                    <span class=font-[var(--font-family-body)]>Secure & Private</span>
                                </div>
                                <div class="flex items-center gap-2">
                                    <i class="fas fa-clock text-2xl" aria-hidden=true></i>
                                    <span class=font-[var(--font-family-body)]>5-Minute Setup</span>
                                </div>
                                <div class="flex items-center gap-2">
                                    <i class="fas fa-credit-card text-2xl" aria-hidden=true></i>
                                    <span class=font-[var(--font-family-body)]>No Card Required</span>
                                </div>
                            </div>
                        </div>
</section>

    <!-- Footer -->
    <footer class="bg-brand-dark-bg text-white py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-12">
          <!-- Brand Column -->
          <div>
            <div class="flex items-center gap-2 mb-4">
              <i class="fas fa-qrcode text-primary text-2xl"></i>
              <span class="text-2xl font-heading font-bold">BizBio</span>
            </div>
            <p class="text-gray-400 mb-6">
              Modern networking made simple. Share your professional profile instantly.
            </p>
            <div class="flex gap-4">
              <a href="#" class="text-gray-400 hover:text-primary transition-colors">
                <i class="fab fa-facebook text-xl"></i>
              </a>
              <a href="#" class="text-gray-400 hover:text-primary transition-colors">
                <i class="fab fa-twitter text-xl"></i>
              </a>
              <a href="#" class="text-gray-400 hover:text-primary transition-colors">
                <i class="fab fa-linkedin text-xl"></i>
              </a>
              <a href="#" class="text-gray-400 hover:text-primary transition-colors">
                <i class="fab fa-instagram text-xl"></i>
              </a>
            </div>
          </div>

          <!-- Quick Links -->
          <div>
            <h3 class="font-heading font-bold mb-4">Quick Links</h3>
            <ul class="space-y-2">
              <li><router-link to="/" class="text-gray-400 hover:text-primary transition-colors">Home</router-link></li>
              <li><router-link to="/categories" class="text-gray-400 hover:text-primary transition-colors">Business
                  Categories</router-link></li>
              <li><router-link to="/search" class="text-gray-400 hover:text-primary transition-colors">Search
                  Businesses</router-link></li>
              <li><router-link to="/pricing"
                  class="text-gray-400 hover:text-primary transition-colors">Pricing</router-link></li>
              <li><router-link to="/help" class="text-gray-400 hover:text-primary transition-colors">Help
                  Center</router-link></li>
            </ul>
          </div>

          <!-- Account -->
          <div>
            <h3 class="font-heading font-bold mb-4">Account</h3>
            <ul class="space-y-2">
              <li><router-link to="/login"
                  class="text-gray-400 hover:text-primary transition-colors">Login</router-link></li>
              <li><router-link to="/register"
                  class="text-gray-400 hover:text-primary transition-colors">Register</router-link></li>
              <li><router-link to="/dashboard"
                  class="text-gray-400 hover:text-primary transition-colors">Dashboard</router-link></li>
            </ul>
          </div>

          <!-- Legal -->
          <div>
            <h3 class="font-heading font-bold mb-4">Legal</h3>
            <ul class="space-y-2">
              <li><router-link to="/terms" class="text-gray-400 hover:text-primary transition-colors">Terms of
                  Service</router-link></li>
              <li><router-link to="/privacy" class="text-gray-400 hover:text-primary transition-colors">Privacy
                  Policy</router-link></li>
              <li><router-link to="/contact" class="text-gray-400 hover:text-primary transition-colors">Contact
                  Us</router-link></li>
            </ul>
          </div>
        </div>

        <div class="border-t border-gray-700 mt-12 pt-8 text-center text-gray-400">
          <p>&copy; 2025 BizBio. All rights reserved. Empowering connections, one profile at a time.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

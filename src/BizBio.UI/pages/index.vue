<template>
  <div>

    <!-- Hero Section -->
    <section class="bg-gradient-to-br from-[var(--light-background-color)] to-white overflow-hidden pb-16 pt-8 sm:pb-20 lg:pb-24">
      <div class="area">
			<ul class="circles">
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
				<li></li>
			</ul>
		</div>
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Decorative Background Blobs -->
        <div class="absolute top-20 right-10 w-72 h-72 bg-[var(--accent3-color)] rounded-full opacity-10 blur-3xl"></div>
        <div class="absolute bottom-20 left-10 w-96 h-96 bg-[var(--primary-color)] rounded-full opacity-10 blur-3xl"></div>

        <div class="grid lg:grid-cols-2 gap-12 lg:gap-16 items-center relative">
          <!-- Left Column: Text Content -->
          <div class="text-center lg:text-left">
            <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-6 leading-[1.1] scroll-fade-up">
              Share Your
              <span class="relative inline-block h-[1.1em] overflow-hidden align-middle md:text-left lg:text-left w-full sm:w-auto sm:min-w-[500px] lg:min-w-[700px]">
                <transition-group name="slide-up" tag="span" class="block relative h-full">
                  <span
                    v-for="word in visibleWords"
                    :key="word.id"
                    :class="['absolute top-0 left-0 w-full whitespace-nowrap', `text-[var(${word.color})]`]"
                  >
                    {{ word.text }}
                  </span>
                </transition-group>
              </span>
              Without the Hassle
            </h1>
            <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-6 leading-relaxed scroll-fade-up">
              Create stunning digital business cards and profiles in minutes. One tap to share all your contact details, social links, and business information with potential clients.
            </p>
            <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-8 scroll-fade-up">
              Perfect for professionals, freelancers, and businesses who want to make a lasting impression.
            </p>

            <!-- CTA Buttons -->
            <div class="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start mb-8 scroll-fade-up">
              <a
                href="https://app.snaptap.co.za/register"
                class="scroll-in inline-flex items-center justify-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-300 font-semibold font-[var(--font-family-body)] shadow-lg hover:shadow-xl"
              >
                <i class="fas fa-rocket mr-2"></i>
                Get Started Free
              </a>
              <NuxtLink
                to="/pricing"
                class="scroll-in inline-flex items-center justify-center border-2 border-[var(--primary-color)] text-[var(--primary-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-color)] hover:text-white transition-all duration-300 font-semibold font-[var(--font-family-body)]"
              >
                <i class="fas fa-tags mr-2"></i>
                View Pricing
              </NuxtLink>
            </div>

            <!-- Trust Microcopy -->
            <div class="flex flex-col sm:flex-row gap-6 justify-center lg:justify-start text-[var(--gray-text-color)]">
              <div class="flex items-center gap-2 whitespace-nowrap">
                <i class="fas fa-check-circle text-[var(--accent3-color)]"></i>
                <span>No credit card required</span>
              </div>
              <div class="flex items-center gap-2 text-nowrap">
                <i class="fas fa-check-circle text-[var(--accent3-color)]"></i>
                <span>Setup in 5 minutes</span>
              </div>
              <div class="flex items-center gap-2 text-nowrap">
                <i class="fas fa-check-circle text-[var(--accent3-color)]"></i>
                <span>Cancel anytime</span>
              </div>
            </div>
          </div>

          <!-- Right Column: Hero Image with Floating Stats -->
          <div class="relative group scroll-fade-right">
            <img
              src="/img_2.webp"
              alt="Business Profile Platform"
              class="w-full rounded-2xl shadow-2xl hover:scale-105 transition-transform duration-300"
            />

            <!-- Floating Stats Cards -->
            <div class="absolute top-4 right-4 bg-white/90 backdrop-blur-sm rounded-xl p-4 shadow-xl animate-float">
              <div class="flex items-center gap-3">
                <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-3 size-12">
                  <i class="fa fa-calendar-star text-white text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold text-[var(--dark-text-color)]">10K+</div>
                  <div class="text-sm text-[var(--gray-text-color)]">Active Profiles</div>
                </div>
              </div>
            </div>

            <div class="absolute bottom-4 left-4 bg-white/90 backdrop-blur-sm rounded-xl p-4 shadow-xl animate-float" style="animation-delay: 1s;">
              <div class="flex items-center gap-3">
                <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full p-3 size-12">
                  <i class="fas fa-chart-line text-white text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold text-[var(--dark-text-color)]">98%</div>
                  <div class="text-sm text-[var(--gray-text-color)]">Satisfaction Rate</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="bg-mesh-1 py-16 sm:py-20 lg:py-24">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4 scroll-fade-up">
            Everything You Need in One Profile
          </h2>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] max-w-3xl mx-auto scroll-fade-up">
            Share your contact details, showcase your business, and make networking effortless with our all-in-one digital profile solution.
          </p>
        </div>

        <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-8 lg:gap-10">
          <!-- Feature Card 1 -->

             <div class="scroll-in bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative scroll-fade-up">
               <div class="absolute inset-0 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
               <div class="relative">
                 <div class=" bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                   <i class="fas fa-id-card text-[var(--primary-color)] text-3xl"></i>
                 </div>
                 <h3 class=" text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                   Digital Business Cards
                 </h3>
                 <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                   Create professional digital business cards that can be shared instantly via link, QR code, or NFC tap. No more paper waste.
                 </p>
               </div>
             </div>

          <!-- Feature Card 2 -->
          <div class="scroll-in  bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative  scroll-fade-up">
            <div class="absolute inset-0 bg-gradient-to-br from-[var(--accent2-color)] to-[var(--accent3-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
            <div class="relative">
              <div class="scroll-in bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                <i class="fas fa-palette text-[var(--accent2-color)] text-3xl"></i>
              </div>
              <h3 class="scroll-in text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                Custom Branding
              </h3>
              <p class="scroll-in text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Choose your colors, fonts, logo, and icons to match your brand identity. Make every profile uniquely yours.
              </p>
            </div>
          </div>

          <!-- Feature Card 3 -->
          <div class="scroll-in bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative  scroll-fade-up">
            <div class="absolute inset-0 bg-gradient-to-br from-[var(--accent3-color)] to-[var(--primary-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
            <div class="relative">
              <div class="scroll-in bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                <i class="fas fa-mobile-alt text-[var(--accent3-color)] text-3xl"></i>
              </div>
              <h3 class="scroll-in text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                NFC Technology
              </h3>
              <p class="scroll-in text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Get a physical NFC card or keyring that people can tap with their phone to instantly view your profile. Future-ready networking.
              </p>
            </div>
          </div>

          <!-- Feature Card 4 -->
          <div class="scroll-in group bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative  scroll-fade-up">
            <div class="absolute inset-0 bg-gradient-to-br from-[var(--accent-color)] to-[var(--accent2-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
            <div class="relative">
              <div class="scroll-in bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                <i class="fas fa-chart-bar text-[var(--accent-color)] text-3xl"></i>
              </div>
              <h3 class="scroll-in text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                Analytics Dashboard
              </h3>
              <p class="scroll-in text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Track profile visits, shares, and clicks. See who's viewing your profile and measure your networking ROI.
              </p>
            </div>
          </div>

          <!-- Feature Card 5 -->
          <div class="scroll-in group bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative  scroll-fade-up">
            <div class="absolute inset-0 bg-gradient-to-br from-[var(--accent4-color)] to-[var(--accent3-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
            <div class="relative">
              <div class="scroll-in bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                <i class="fas fa-download text-[var(--accent4-color)] text-3xl"></i>
              </div>
              <h3 class="scroll-in text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                Easy Contact Saving
              </h3>
              <p class="scroll-in text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Let people save your contact details directly to their phone with one click using VCF files. No manual typing required.
              </p>
            </div>
          </div>

          <!-- Feature Card 6 -->
          <div class="scroll-in group bg-[var(--light-background-color)] rounded-xl p-8 hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 relative  scroll-fade-up">
            <div class="absolute inset-0 bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent2-color)] opacity-0 group-hover:opacity-10 transition-opacity duration-300"></div>
            <div class="relative">
              <div class="scroll-in bg-white rounded-full w-16 h-16 flex items-center justify-center mb-6 shadow-lg">
                <i class="fas fa-layer-group text-[var(--primary-color)] text-3xl"></i>
              </div>
              <h3 class="scroll-in text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-3">
                Multiple Profiles
              </h3>
              <p class="scroll-in text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Create separate profiles for your business, personal brand, or resume. Switch between them effortlessly.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- How It Works Section -->
    <section class="bg-gradient-to-br from-[var(--medium-background-color)] to-[var(--light-background-color)] py-16 sm:py-20 lg:py-24 relative ">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Decorative Background Blobs -->
        <div class="absolute top-40 right-20 w-64 h-64 bg-[var(--primary-color)] rounded-full opacity-5 blur-3xl"></div>
        <div class="absolute bottom-40 left-20 w-80 h-80 bg-[var(--accent3-color)] rounded-full opacity-5 blur-3xl"></div>

        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4 scroll-fade-up">
            Get Your Profile Live in 3 Simple Steps
          </h2>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)] scroll-fade-up">
            No technical skills required. Set up your professional profile in minutes.
          </p>
        </div>

        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- Left: Image -->
          <div class="order-2 lg:order-1 scroll-fade-left" data-animate>
            <img
              src="/img_1.webp"
              alt="How It Works"
              class="w-full rounded-2xl shadow-2xl"
            />
          </div>

          <!-- Right: Steps -->
          <div class="order-1 lg:order-2 space-y-8">
            <!-- Step 1 -->
            <div class="scroll-in flex gap-6 group">
              <div class="flex-shrink-0 scroll-in">
                <div class="scroll-in bg-[var(--primary-color)] text-white rounded-full w-16 h-16 flex items-center justify-center font-bold text-2xl shadow-lg group-hover:scale-110 transition-transform duration-300 sm:scroll-fade-up scroll-fade-left">
                  1
                </div>
              </div>
              <div class="">
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Sign Up & Choose Template
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  Create your free account and select from our professionally designed profile templates. Pick one that matches your style and industry.
                </p>
              </div>
            </div>

            <!-- Step 2 -->
            <div class="flex gap-6 group">
              <div class="flex-shrink-0">
                <div class="bg-[var(--accent3-color)] text-white rounded-full w-16 h-16 flex items-center justify-center font-bold text-2xl shadow-lg group-hover:scale-110 transition-transform duration-300 scroll-fade-left">
                  2
                </div>
              </div>
              <div class="">
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Customize Your Profile
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  Add your logo, contact details, social links, business hours, and location. Apply your brand colors and choose icons that represent you.
                </p>
              </div>
            </div>

            <!-- Step 3 -->
            <div class="flex gap-6 group">
              <div class="flex-shrink-0">
                <div class="bg-[var(--accent2-color)] text-white rounded-full w-16 h-16 flex items-center justify-center font-bold text-2xl shadow-lg group-hover:scale-110 transition-transform duration-300 scroll-fade-left">
                  3
                </div>
              </div>
              <div class="">
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Share & Connect
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  Share your profile via link, QR code, or NFC card. Start networking smarter and track your engagement in real-time.
                </p>
              </div>
            </div>

            <!-- CTA Button -->
            <div class="pt-4">
              <a
                href="https://app.snaptap.co.za/register"
                class="scroll-in inline-flex items-center bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-300 font-semibold font-[var(--font-family-body)] shadow-lg hover:shadow-xl">
                Start Building Your Profile
                <i class="fas fa-arrow-right ml-2"></i>
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Connect with Local Businesses Section -->
    <section class="bg-white py-16 sm:py-20 lg:py-24">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Connect with Local Businesses
          </h2>
          <p class="text-lg sm:text-xl text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Browse businesses by category, check ratings, and view detailed profiles. Find the perfect service provider for your needs.
          </p>
        </div>

        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- Left: Content -->
          <div class="space-y-8">
            <!-- Feature 1 -->
            <div class="flex gap-6">
              <div class="flex-shrink-0">
                <div class="scroll-in bg-[var(--primary-color)] bg-opacity-10 rounded-full w-14 h-14 flex items-center justify-center">
                  <i class="fal fa-search text-white text-2xl"></i>
                </div>
              </div>
              <div>
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Search by Category
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  Find plumbers, electricians, consultants, designers, and hundreds of other business categories. Filter by location and ratings.
                </p>
              </div>
            </div>

            <!-- Feature 2 -->
            <div class="flex gap-6">
              <div class="flex-shrink-0">
                <div class="scroll-in bg-[var(--accent3-color)] bg-opacity-10 rounded-full w-14 h-14 flex items-center justify-center">
                  <i class="fal fa-star text-white text-2xl"></i>
                </div>
              </div>
              <div>
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Ratings & Reviews
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  See what others are saying. Connect to Google and Facebook reviews directly from business profiles.
                </p>
              </div>
            </div>

            <!-- Feature 3 -->
            <div class="flex gap-6">
              <div class="flex-shrink-0">
                <div class="scroll-in bg-[var(--accent2-color)] bg-opacity-10 rounded-full w-14 h-14 flex items-center justify-center">
                  <i class="fal fa-map-marker-alt text-white text-2xl"></i>
                </div>
              </div>
              <div>
                <h3 class="text-xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
                  Location & Hours
                </h3>
                <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                  View business locations on embedded maps, check operating hours, and get directions instantly.
                </p>
              </div>
            </div>

            <!-- CTA Button -->
            <div class="pt-4">
              <NuxtLink
                to="/categories"
                class="scroll-in inline-flex items-center border-2 border-[var(--primary-color)] text-[var(--primary-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-color)] hover:text-white transition-all duration-300 font-semibold font-[var(--font-family-body)]"
              >
                <i class="fas fa-th-large mr-2"></i>
                Browse All Categories
              </NuxtLink>
            </div>
          </div>

          <!-- Right: Image with Floating Cards -->
          <div class="relative">
            <img
              src="/154.webp"
              alt="Connect with Businesses"
              class="w-full rounded-2xl shadow-2xl"
            />

            <!-- Floating Category Cards -->
            <div class="scroll-in absolute top-4 left-4 bg-white rounded-lg p-3 shadow-lg">
              <div class="flex items-center gap-2">
                <i class="fas fa-wrench text-[var(--accent-color)]"></i>
                <span class="font-semibold text-[var(--dark-text-color)]">342 Plumbers</span>
              </div>
            </div>

            <div class="scroll-in absolute top-4 right-4 bg-white rounded-lg p-3 shadow-lg">
              <div class="flex items-center gap-2">
                <i class="fas fa-paint-roller text-[var(--primary-color)]"></i>
                <span class="font-semibold text-[var(--dark-text-color)]">218 Contractors</span>
              </div>
            </div>

            <div class="scroll-in absolute bottom-4 left-4 bg-white rounded-lg p-3 shadow-lg">
              <div class="flex items-center gap-2">
                <i class="fas fa-laptop-code text-[var(--accent3-color)]"></i>
                <span class="font-semibold text-[var(--dark-text-color)]">567 Designers</span>
              </div>
            </div>

            <div class="scroll-in absolute bottom-4 right-4 bg-white rounded-lg p-3 shadow-lg">
              <div class="flex items-center gap-2">
                <i class="fas fa-briefcase text-[var(--accent2-color)]"></i>
                <span class="font-semibold text-[var(--dark-text-color)]">429 Consultants</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Final CTA Section -->
    <section class="bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] py-16 sm:py-20 lg:py-24 relative">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center relative z-10">
        <!-- Decorative Background Blobs -->
        <div class="absolute top-10 left-10 w-96 h-96 bg-white rounded-full opacity-5 blur-3xl"></div>
        <div class="absolute bottom-10 right-10 w-80 h-80 bg-white rounded-full opacity-5 blur-3xl"></div>

        <h2 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white font-[var(--font-family-heading)] mb-6">
          Ready to Modernize Your Networking?
        </h2>
        <p class="text-xl text-white/90 font-[var(--font-family-body)] mb-4">
          Join thousands of professionals and businesses who are sharing their profiles smarter.
        </p>
        <p class="text-lg text-white/80 font-[var(--font-family-body)] mb-10">
          Create your free digital profile today and start making lasting impressions.
        </p>

        <!-- CTA Buttons -->
        <div class="flex flex-col sm:flex-row gap-4 justify-center mb-12">
          <a
            href="https://app.snaptap.co.za/register"
            class="scroll-in inline-flex items-center justify-center bg-white text-[var(--primary-color)] px-8 py-4 rounded-[var(--button-rounded-radius)] hover:scale-105 hover:-translate-y-1 transition-all duration-300 font-semibold font-[var(--font-family-body)] shadow-xl"
          >
            <i class="fas fa-user-plus mr-2"></i>
            Create Your Profile Now
          </a>
          <NuxtLink
            to="/pricing"
            class="scroll-in inline-flex items-center justify-center border-2 border-white text-white px-8 py-4 rounded-[var(--button-rounded-radius)] hover:bg-white hover:text-[var(--primary-color)] transition-all duration-300 font-semibold font-[var(--font-family-body)]"
          >
            <i class="fas fa-dollar-sign mr-2"></i>
            View Plans
          </NuxtLink>
        </div>

        <!-- Trust Indicators -->
        <div class="scroll-in flex flex-col sm:flex-row gap-8 justify-center text-white/90">
          <div class="flex items-center gap-2">
            <i class="fas fa-shield-alt text-2xl"></i>
            <span>Secure & Private</span>
          </div>
          <div class="flex items-center gap-2">
            <i class="fas fa-clock text-2xl"></i>
            <span>5-Minute Setup</span>
          </div>
          <div class="flex items-center gap-2">
            <i class="fas fa-credit-card text-2xl"></i>
            <span>No Card Required</span>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
useHead({
  title: 'Home',
})

const words = ['Business Profile', 'Personal Profile', 'Restaurant Menu', 'Business Catalog']
const colors = ['--primary-color', '--accent3-color', '--accent4-color', '--accent2-color', '--accent1-color']
const index = ref(0)
const currentWord = ref({ id: 0, text: words[0], color: colors[0] })

let timer

const visibleWords = computed(() => [currentWord.value])

onMounted(() => {
  timer = setInterval(() => {
    index.value = (index.value + 1) % words.length
    currentWord.value = {
      id: Date.now(),
      text: words[index.value],
      color: colors[index.value % colors.length]
    }
  }, 3000) // Slightly longer duration for longer text

  // Initialize Intersection Observer for scroll animations
  const observerOptions = {
    root: null,
    rootMargin: '0px',
    threshold: 0.1
  }

  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('scroll-animate-in')
      }
    })
  }, observerOptions)

  // Observe all elements with scroll animation classes
  const scrollElements = document.querySelectorAll('.scroll-fade-up, .scroll-fade-left, .scroll-fade-right')
  scrollElements.forEach(el => observer.observe(el))
})

onBeforeUnmount(() => {
  clearInterval(timer)
})
</script>

<style scoped>

p, h2, h3, blockquote, hr, .card, .group, .icon, .show-animate, scroll-in {
  /* animation-name: text-appear; @keyframes defined below */
  /* animation-timing-function: ease-in; standard should be linear */
  /* animation-fill-mode: both; keeps animated values forwards and backwards */
  animation: text-appear ease-in both;
  animation-timeline: view(); /* applies animation state relative to viewport */
  animation-range: 0vh 30vh; /* set 0% and 100% positions */
}
img {
  animation: image-appear ease-in both;
  animation-timeline: view();
  animation-range: entry 50% 50vh;
}
@prefers-reduced-motion {
  p, h2, h3, blockquote, hr, img {
    animation: none;
  }
}


@keyframes image-appear {
  from {
    filter: saturate(0) contrast(4) brightness(.1) blur(3px);
    opacity: 0;
    scale: .95;
    translate: 0 4rem;
  }
}

@keyframes text-appear {
  from {
    opacity: 0;
    translate: 0 8rem;
  }
}</style>

<style scoped>
/* Version 1: Bottom-to-Top Slide Animation */
.slide-up-enter-active {
  transition: all 0.5s ease-out;
  z-index: 2;
}

.slide-up-leave-active {
  transition: all 0.5s ease-in;
  z-index: 1;
}

.slide-up-enter-from {
  transform: translateY(120%);
  opacity: 0;
}

.slide-up-enter-to {
  transform: translateY(0);
  opacity: 1;
}

.slide-up-leave-from {
  transform: translateY(0);
  opacity: 1;
}

.slide-up-leave-to {
  transform: translateY(-120%);
  opacity: 0;
}

/* Version 2: Left-to-Right Slide Animation */
.slide-horizontal-enter-active {
  transition: all 0.5s ease-out;
  z-index: 2;
}

.slide-horizontal-leave-active {
  transition: all 0.5s ease-in;
  z-index: 1;
}

.slide-horizontal-enter-from {
  transform: translateX(-120%);
  opacity: 0;
}

.slide-horizontal-enter-to {
  transform: translateX(0);
  opacity: 1;
}

.slide-horizontal-leave-from {
  transform: translateX(0);
  opacity: 1;
}

.slide-horizontal-leave-to {
  transform: translateX(120%);
  opacity: 0;
}

/* Version 3: Fade + Scale Animation (Premium Feel) */
.fade-scale-enter-active {
  transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 2;
}

.fade-scale-leave-active {
  transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1;
}

.fade-scale-enter-from {
  transform: scale(0.85);
  opacity: 0;
}

.fade-scale-enter-to {
  transform: scale(1);
  opacity: 1;
}

.fade-scale-leave-from {
  transform: scale(1);
  opacity: 1;
}

.fade-scale-leave-to {
  transform: scale(0.85);
  opacity: 0;
}

/* Scroll Animations */
.scroll-fade-up,
.scroll-fade-left,
.scroll-fade-right {
  opacity: 0;
  transition: opacity 0.8s cubic-bezier(0.4, 0, 0.2, 1),
              transform 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

.scroll-fade-up {
  transform: translateY(40px);
}

.scroll-fade-left {
  transform: translateX(-60px);
}

.scroll-fade-right {
  transform: translateX(60px);
}

.scroll-fade-up.scroll-animate-in,
.scroll-fade-left.scroll-animate-in,
.scroll-fade-right.scroll-animate-in {
  opacity: 1;
  transform: translate(0, 0);
}

/* Staggered animation delays for multiple elements */
.scroll-fade-up:nth-child(1) { transition-delay: 0.05s; }
.scroll-fade-up:nth-child(2) { transition-delay: 0.1s; }
.scroll-fade-up:nth-child(3) { transition-delay: 0.15s; }
.scroll-fade-up:nth-child(4) { transition-delay: 0.2s; }
.scroll-fade-up:nth-child(5) { transition-delay: 0.25s; }
.scroll-fade-up:nth-child(6) { transition-delay: 0.3s; }
</style>

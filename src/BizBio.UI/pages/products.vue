<template>
   <!-- Hero Section -->
    <section class="relative bg-gradient-to-br from-blue-50 to-purple-50 overflow-hidden py-20">
        <div class="absolute top-20 right-10 w-72 h-72 bg-blue-600 rounded-full opacity-10 blur-3xl"></div>
        <div class="absolute bottom-20 left-10 w-96 h-96 bg-purple-600 rounded-full opacity-10 blur-3xl"></div>
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center relative">
            <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-md-on-surface mb-6">
                Digital Solutions for <span class="text-blue-600">Modern Businesses</span>
            </h1>
            <p class="text-xl text-md-on-surface-variant max-w-3xl mx-auto mb-8">
                From digital business cards to interactive menus and product catalogs, BizBio helps you go paperless and professional.
            </p>
        </div>
    </section>

    <!-- Main Products -->
    <section class="py-20 bg-white">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="text-center mb-16">
                <h2 class="text-3xl sm:text-4xl font-bold text-md-on-surface mb-4">Our Products</h2>
                <p class="text-lg text-md-on-surface-variant">Choose the solution that fits your business needs</p>
            </div>


            
            <div class="grid md:grid-cols-3 gap-8">
                <!-- Professional Product -->
                <div class="product-card bg-white border border-gray-200 rounded-xl p-8 shadow-md">
                    <div class="feature-icon bg-blue-100">
                        <i class="fas fa-id-card text-blue-600 text-2xl"></i>
                    </div>
                    <h3 class="text-2xl font-bold text-md-on-surface mb-3">BizBio Connect</h3>
                    <p class="text-md-on-surface-variant mb-6">Digital business cards that leave a lasting impression. Share your contact info instantly via NFC or QR code.</p>

                    <div class="space-y-3 mb-6">
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Custom branding & templates</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Real-time analytics</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Team management</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Social media integration</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                    </div>

                    <div class="mb-4">
                        <span class="text-sm text-gray-500">Starting at</span>
                        <div class="text-3xl font-bold text-md-on-surface">R99<span class="text-sm text-md-on-surface-variant">/month</span></div>
                    </div>

                    <a href="pricing.html" class="block w-full bg-blue-600 text-white text-center py-3 rounded-lg hover:bg-blue-700 transition-colors duration-200 font-semibold">
                        View Plans
                    </a>
                    <sub><span class="text-yellow-600 mr-1">*</span> On selected plans only</sub>
                </div>

                <!-- Menu Product -->
                <div class="product-card bg-white border-2 border-blue-600 rounded-xl p-8 shadow-lg relative">
                    <div class="absolute -top-3 left-1/2 transform -translate-x-1/2 bg-blue-600 text-white px-4 py-1 rounded-full text-xs font-semibold">
                        Popular
                    </div>
                    <div class="feature-icon bg-orange-100">
                        <i class="fas fa-utensils text-orange-600 text-2xl"></i>
                    </div>
                    <h3 class="text-2xl font-bold text-md-on-surface mb-3">BizBio Menu</h3>
                    <p class="text-md-on-surface-variant mb-6">Digital menus for restaurants, cafes, and bars. Update instantly, no reprinting costs. Perfect for post-COVID contactless dining.</p>

                    <div class="space-y-3 mb-6">
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Unlimited menu updates</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Food photography support</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Allergen & dietary info</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Daily specials section</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Multi-location support</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                    </div>

                    <div class="mb-4">
                        <span class="text-sm text-gray-500">Starting at</span>
                        <div class="text-3xl font-bold text-md-on-surface">R149<span class="text-sm text-md-on-surface-variant">/month</span></div>
                    </div>

                    <a href="pricing.html" class="block w-full bg-blue-600 text-white text-center py-3 rounded-lg hover:bg-blue-700 transition-colors duration-200 font-semibold">
                        View Plans
                    </a>
                    <sub><span class="text-yellow-600 mr-1">*</span> On selected plans only</sub>
                </div>

                <!-- Retail Product -->
                <div class="product-card bg-white border border-gray-200 rounded-xl p-8 shadow-md">
                    <div class="feature-icon bg-md-primary-container">
                        <i class="fas fa-store text-md-primary text-2xl"></i>
                    </div>
                    <h3 class="text-2xl font-bold text-md-on-surface mb-3">BizBio Retail</h3>
                    <p class="text-md-on-surface-variant mb-6">Showcase your products beautifully with digital catalogs. Perfect for boutiques, wholesalers, and service providers.</p>

                    <div class="space-y-3 mb-6">
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Beautiful product galleries</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Product variants & specs</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Bulk CSV import</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Product collections</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                        <div class="flex items-start">
                            <i class="fas fa-check text-green-500 mt-1 mr-3"></i>
                            <span class="text-md-on-surface">Video product demos</span>
                            <span class="text-yellow-600 ml-1">*</span>
                        </div>
                    </div>

                    <div class="mb-4">
                        <span class="text-sm text-gray-500">Starting at</span>
                        <div class="text-3xl font-bold text-md-on-surface">R199<span class="text-sm text-md-on-surface-variant">/month</span></div>
                    </div>

                    <a href="pricing.html" class="block w-full bg-blue-600 text-white text-center py-3 rounded-lg hover:bg-blue-700 transition-colors duration-200 font-semibold">
                        View Plans
                    </a>
                    <sub><span class="text-yellow-600 mr-1">*</span> On selected plans only</sub>
                </div>
            </div>
        </div>
    </section>

    <!-- NFC Products -->
    <section class="py-20 bg-gray-50">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="text-center mb-16">
                <h2 class="text-3xl sm:text-4xl font-bold text-md-on-surface mb-4">
                    <i class="fas fa-wifi text-blue-600 mr-3"></i>Physical NFC Products
                </h2>
                <p class="text-lg text-md-on-surface-variant">High-quality NFC-enabled products for contactless sharing</p>
            </div>

            <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                <!-- NFC Business Cards -->
                <div class="bg-white rounded-lg p-6 shadow-md hover:shadow-lg transition-shadow duration-200">
                    <div class="bg-blue-100 w-16 h-16 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-address-card text-blue-600 text-2xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-md-on-surface mb-2">NFC Business Card</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Premium NFC-enabled business card that shares your profile with a tap</p>
                    <div class="text-2xl font-bold text-md-on-surface mb-4">R150</div>
                    <ul class="text-sm text-md-on-surface-variant space-y-1 mb-4">
                        <li>• Single card</li>
                        <li>• Premium finish</li>
                        <li>• Reprogrammable</li>
                    </ul>
                    <button class="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors duration-200 text-sm font-semibold">
                        Order Now
                    </button>
                </div>

                <!-- NFC Wristbands -->
                <div class="bg-white rounded-lg p-6 shadow-md hover:shadow-lg transition-shadow duration-200 border-2 border-orange-400">
                    <div class="absolute -top-2 left-1/2 transform -translate-x-1/2 bg-orange-500 text-white px-3 py-1 rounded-full text-xs font-semibold">
                        New!
                    </div>
                    <div class="bg-orange-100 w-16 h-16 rounded-lg flex items-center justify-center mb-4 mt-2">
                        <i class="fas fa-hand-sparkles text-orange-600 text-2xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-md-on-surface mb-2">NFC Wristband</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Waterproof wristband for restaurant waiters. Customers tap to view menu or leave reviews</p>
                    <div class="text-2xl font-bold text-md-on-surface mb-4">R120</div>
                    <ul class="text-sm text-md-on-surface-variant space-y-1 mb-4">
                        <li>• Single wristband</li>
                        <li>• Waterproof silicone</li>
                        <li>• Multiple colors</li>
                    </ul>
                    <button class="w-full bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition-colors duration-200 text-sm font-semibold">
                        Order Now
                    </button>
                </div>

                <!-- Table Standee -->
                <div class="bg-white rounded-lg p-6 shadow-md hover:shadow-lg transition-shadow duration-200">
                    <div class="bg-md-primary-container w-16 h-16 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-qrcode text-md-primary text-2xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-md-on-surface mb-2">Table Standee</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Acrylic table stand with QR code for easy scanning</p>
                    <div class="text-2xl font-bold text-md-on-surface mb-4">R300</div>
                    <ul class="text-sm text-md-on-surface-variant space-y-1 mb-4">
                        <li>• Acrylic stand</li>
                        <li>• Custom QR code</li>
                        <li>• Professional look</li>
                    </ul>
                    <button class="w-full bg-purple-600 text-white py-2 rounded-lg hover:bg-purple-700 transition-colors duration-200 text-sm font-semibold">
                        Order Now
                    </button>
                </div>

                <!-- QR Stickers -->
                <div class="bg-white rounded-lg p-6 shadow-md hover:shadow-lg transition-shadow duration-200">
                    <div class="bg-green-100 w-16 h-16 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-sticker-mule text-green-600 text-2xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-md-on-surface mb-2">Custom QR Stickers</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Weatherproof vinyl stickers with your custom QR code</p>
                    <div class="text-2xl font-bold text-md-on-surface mb-4">R500</div>
                    <ul class="text-sm text-md-on-surface-variant space-y-1 mb-4">
                        <li>• Pack of 100</li>
                        <li>• Weatherproof vinyl</li>
                        <li>• Custom design</li>
                    </ul>
                    <button class="w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors duration-200 text-sm font-semibold">
                        Order Now
                    </button>
                </div>
            </div>

            <!-- Bulk Packs -->
            <div class="mt-12 grid md:grid-cols-3 gap-6">
                <div class="bg-gradient-to-br from-blue-50 to-blue-100 rounded-lg p-6">
                    <h4 class="text-lg font-bold text-md-on-surface mb-2">NFC Card Pack (10)</h4>
                    <div class="text-3xl font-bold text-blue-600 mb-2">R1,200</div>
                    <p class="text-sm text-md-on-surface-variant mb-4">Save R300 on bulk order</p>
                    <button class="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors duration-200 text-sm font-semibold">
                        Order Pack
                    </button>
                </div>

                <div class="bg-gradient-to-br from-orange-50 to-orange-100 rounded-lg p-6">
                    <h4 class="text-lg font-bold text-md-on-surface mb-2">Wristband Pack (5)</h4>
                    <div class="text-3xl font-bold text-orange-600 mb-2">R500</div>
                    <p class="text-sm text-md-on-surface-variant mb-4">Perfect for restaurant teams</p>
                    <button class="w-full bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition-colors duration-200 text-sm font-semibold">
                        Order Pack
                    </button>
                </div>

                <div class="bg-gradient-to-br from-purple-50 to-purple-100 rounded-lg p-6">
                    <h4 class="text-lg font-bold text-md-on-surface mb-2">Table Standee Pack (10)</h4>
                    <div class="text-3xl font-bold text-md-primary mb-2">R2,500</div>
                    <p class="text-sm text-md-on-surface-variant mb-4">Discounted pack for restaurants</p>
                    <button class="w-full bg-purple-600 text-white py-2 rounded-lg hover:bg-purple-700 transition-colors duration-200 text-sm font-semibold">
                        Order Pack
                    </button>
                </div>
            </div>
        </div>
    </section>

    <!-- Services -->
    <section class="py-20 bg-white">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="text-center mb-16">
                <h2 class="text-3xl sm:text-4xl font-bold text-md-on-surface mb-4">Professional Services</h2>
                <p class="text-lg text-md-on-surface-variant">Let our experts help you create stunning digital profiles</p>
            </div>

            <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div class="bg-gray-50 rounded-lg p-6 hover:shadow-md transition-shadow duration-200">
                    <div class="bg-blue-100 w-12 h-12 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-paint-brush text-blue-600 text-xl"></i>
                    </div>
                    <h3 class="text-lg font-bold text-md-on-surface mb-2">Profile Design</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Professional design of your digital profile</p>
                    <div class="text-xl font-bold text-md-on-surface">R1,500 - R3,000</div>
                </div>

                <div class="bg-gray-50 rounded-lg p-6 hover:shadow-md transition-shadow duration-200">
                    <div class="bg-orange-100 w-12 h-12 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-camera text-orange-600 text-xl"></i>
                    </div>
                    <h3 class="text-lg font-bold text-md-on-surface mb-2">Menu Photography</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Professional food photography in Johannesburg</p>
                    <div class="text-xl font-bold text-md-on-surface">R2,500 - R8,000</div>
                </div>

                <div class="bg-gray-50 rounded-lg p-6 hover:shadow-md transition-shadow duration-200">
                    <div class="bg-md-primary-container w-12 h-12 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-box-open text-md-primary text-xl"></i>
                    </div>
                    <h3 class="text-lg font-bold text-md-on-surface mb-2">Product Photography</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Professional product photos in Johannesburg</p>
                    <div class="text-xl font-bold text-md-on-surface">R2,000 - R6,000</div>
                </div>

                <div class="bg-gray-50 rounded-lg p-6 hover:shadow-md transition-shadow duration-200">
                    <div class="bg-green-100 w-12 h-12 rounded-lg flex items-center justify-center mb-4">
                        <i class="fas fa-database text-green-600 text-xl"></i>
                    </div>
                    <h3 class="text-lg font-bold text-md-on-surface mb-2">Data Migration</h3>
                    <p class="text-md-on-surface-variant text-sm mb-4">Import existing menu/catalog from other systems</p>
                    <div class="text-xl font-bold text-md-on-surface">R2,000+</div>
                </div>
            </div>
        </div>
    </section>

    <!-- Why BizBio -->
    <section class="py-20 bg-gradient-to-br from-blue-600 to-purple-600">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="text-center mb-12">
                <h2 class="text-3xl sm:text-4xl font-bold text-white mb-4">Why Choose BizBio?</h2>
                <p class="text-xl text-blue-100">Built specifically for South African businesses</p>
            </div>

            <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
                <div class="text-center">
                    <div class="bg-white bg-opacity-20 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-piggy-bank text-white text-2xl"></i>
                    </div>
                    <h3 class="text-white font-bold text-lg mb-2">Save Money</h3>
                    <p class="text-blue-100 text-sm">No printing costs - save R5,000-R20,000/year</p>
                </div>

                <div class="text-center">
                    <div class="bg-white bg-opacity-20 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-clock text-white text-2xl"></i>
                    </div>
                    <h3 class="text-white font-bold text-lg mb-2">Save Time</h3>
                    <p class="text-blue-100 text-sm">Update in seconds, changes go live immediately</p>
                </div>

                <div class="text-center">
                    <div class="bg-white bg-opacity-20 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-leaf text-white text-2xl"></i>
                    </div>
                    <h3 class="text-white font-bold text-lg mb-2">Eco-Friendly</h3>
                    <p class="text-blue-100 text-sm">Zero paper waste, sustainable business practices</p>
                </div>

                <div class="text-center">
                    <div class="bg-white bg-opacity-20 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-chart-line text-white text-2xl"></i>
                    </div>
                    <h3 class="text-white font-bold text-lg mb-2">Track Engagement</h3>
                    <p class="text-blue-100 text-sm">Real-time analytics on profile views and clicks</p>
                </div>
            </div>
        </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 bg-white">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
            <h2 class="text-3xl sm:text-4xl font-bold text-md-on-surface mb-4">Ready to Get Started?</h2>
            <p class="text-xl text-md-on-surface-variant mb-8">Choose the product that's right for your business and start your free trial today.</p>
            <div class="flex flex-col sm:flex-row gap-4 justify-center">
                <a href="pricing.html" class="bg-blue-600 text-white px-8 py-4 rounded-lg hover:bg-blue-700 transition-colors duration-200 font-semibold text-lg">
                    View Pricing
                </a>
                <a href="contact.html" class="bg-gray-100 text-md-on-surface px-8 py-4 rounded-lg hover:bg-gray-200 transition-colors duration-200 font-semibold text-lg">
                    Contact Sales
                </a>
            </div>
        </div>
    </section>

    <!-- Footer -->
    <footer class="bg-gray-900 text-md-on-surface-variant opacity-50 py-12">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="grid md:grid-cols-4 gap-8">
                <div>
                    <h3 class="text-white text-lg font-bold mb-4">BizBio</h3>
                    <p class="text-sm">Digital profiles, menus, and catalogs for modern businesses.</p>
                </div>
                <div>
                    <h4 class="text-white font-semibold mb-4">Product</h4>
                    <ul class="space-y-2 text-sm">
                        <li><a href="#" class="hover:text-white transition-colors">Features</a></li>
                        <li><a href="pricing.html" class="hover:text-white transition-colors">Pricing</a></li>
                        <li><a href="products.html" class="hover:text-white transition-colors">Products</a></li>
                    </ul>
                </div>
                <div>
                    <h4 class="text-white font-semibold mb-4">Company</h4>
                    <ul class="space-y-2 text-sm">
                        <li><a href="#" class="hover:text-white transition-colors">About</a></li>
                        <li><a href="contact.html" class="hover:text-white transition-colors">Contact</a></li>
                        <li><a href="#" class="hover:text-white transition-colors">Support</a></li>
                    </ul>
                </div>
                <div>
                    <h4 class="text-white font-semibold mb-4">Legal</h4>
                    <ul class="space-y-2 text-sm">
                        <li><a href="#" class="hover:text-white transition-colors">Privacy Policy</a></li>
                        <li><a href="#" class="hover:text-white transition-colors">Terms of Service</a></li>
                    </ul>
                </div>
            </div>
            <div class="border-t border-gray-800 mt-8 pt-8 text-center text-sm">
                <p>&copy; 2025 BizBio. All rights reserved. Built for South African businesses.</p>
            </div>
        </div>
    </footer>

</template>
 



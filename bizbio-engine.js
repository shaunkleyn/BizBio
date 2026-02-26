/**
 * BizBio Engine
 * Dynamically loads and populates business card templates from JSON data
 *
 * Usage:
 * 1. Create a client folder (e.g., /susanboyle/)
 * 2. Add data.json with client information
 * 3. Add index.html that loads this engine
 * 4. Visit /susanboyle/ to see the populated template
 */

(function() {
    'use strict';

    const BizBioEngine = {
        config: {
            dataFile: 'data.json',
            templatesPath: '/templates/',
            vcardGeneratorPath: '/vcard-generator.js'
        },

        // Element selectors for populating data
        selectors: {
            // Name variations
            name: ['.name', '.card-name', '.profile-name', '.user-name', 'h1.name', 'h1:not(.logo-text)'],

            // Title/Position
            title: ['.title', '.job-title', '.position', '.role', '.subtitle'],

            // Company/Organization
            company: ['.company', '.organization', '.org', '.employer', '.logo-text'],
            tagline: ['.tagline', '.logo-tagline', '.company-tagline'],

            // Bio/About
            bio: ['.bio', '.about', '.description', '.intro', '.summary'],

            // Photo/Avatar
            photo: ['.avatar', '.profile-image', '.photo', 'img.avatar', 'img.profile', '.avatar-container img'],
            logo: ['.logo img', '.logo-icon img', '.company-logo img'],

            // Contact info - these need special handling for href attributes
            email: ['a[href^="mailto:"]'],
            phone: ['a[href^="tel:"]'],
            whatsapp: ['a[href*="wa.me"]', 'a[href*="whatsapp"]'],
            website: ['.website a', 'a.website', '.web a', 'a[href]:not([href^="mailto:"]):not([href^="tel:"]):not([href*="wa.me"]):not([href*="linkedin"]):not([href*="facebook"]):not([href*="instagram"]):not([href*="twitter"]):not([href*="x.com"]):not([href*="github"]):not([href*="youtube"]):not([href*="tiktok"]):not([href*="maps"])'],

            // Address
            address: ['.address', '.location', '.adr', 'a[href*="maps.google"]', 'a[href*="maps.app"]'],

            // Social links
            linkedin: ['a[href*="linkedin.com"]'],
            facebook: ['a[href*="facebook.com"]'],
            instagram: ['a[href*="instagram.com"]'],
            twitter: ['a[href*="twitter.com"]', 'a[href*="x.com"]'],
            github: ['a[href*="github.com"]'],
            youtube: ['a[href*="youtube.com"]'],
            tiktok: ['a[href*="tiktok.com"]'],

            // Stats (for templates with stats)
            stats: ['.stat', '.stat-item', '.stats-item'],

            // Page title
            pageTitle: ['title']
        },

        data: null,
        templateDoc: null,

        /**
         * Initialize the engine
         */
        async init() {
            try {
                // Show loading state
                this.showLoading();

                // Load JSON data
                await this.loadData();

                // Load and populate template
                await this.loadTemplate();

                // Populate template with data
                this.populateTemplate();

                // Render the populated template
                this.render();

                // Initialize vCard generator
                this.initVCardGenerator();

                console.log('BizBio Engine initialized successfully');
            } catch (error) {
                console.error('BizBio Engine error:', error);
                this.showError(error.message);
            }
        },

        /**
         * Show loading indicator
         */
        showLoading() {
            document.body.innerHTML = `
                <div style="display:flex;justify-content:center;align-items:center;min-height:100vh;font-family:system-ui,sans-serif;">
                    <div style="text-align:center;">
                        <div style="width:40px;height:40px;border:3px solid #e5e7eb;border-top-color:#3b82f6;border-radius:50%;animation:spin 1s linear infinite;margin:0 auto 16px;"></div>
                        <p style="color:#6b7280;">Loading business card...</p>
                    </div>
                </div>
                <style>@keyframes spin{to{transform:rotate(360deg)}}</style>
            `;
        },

        /**
         * Show error message
         */
        showError(message) {
            document.body.innerHTML = `
                <div style="display:flex;justify-content:center;align-items:center;min-height:100vh;font-family:system-ui,sans-serif;padding:20px;">
                    <div style="text-align:center;max-width:400px;">
                        <div style="width:60px;height:60px;background:#fee2e2;border-radius:50%;display:flex;align-items:center;justify-content:center;margin:0 auto 16px;">
                            <svg width="24" height="24" fill="none" stroke="#ef4444" stroke-width="2" viewBox="0 0 24 24">
                                <circle cx="12" cy="12" r="10"/><path d="M15 9l-6 6M9 9l6 6"/>
                            </svg>
                        </div>
                        <h1 style="font-size:20px;color:#1f2937;margin-bottom:8px;">Unable to load business card</h1>
                        <p style="color:#6b7280;font-size:14px;">${message}</p>
                    </div>
                </div>
            `;
        },

        /**
         * Load data.json from current directory
         */
        async loadData() {
            const response = await fetch(this.config.dataFile);
            if (!response.ok) {
                throw new Error('Could not find data.json. Please ensure the file exists in this folder.');
            }
            this.data = await response.json();

            // Validate required fields
            if (!this.data.template) {
                throw new Error('data.json must specify a "template" field.');
            }
            if (!this.data.name) {
                throw new Error('data.json must specify a "name" field.');
            }
        },

        /**
         * Load the template HTML
         */
        async loadTemplate() {
            const templateUrl = `${this.config.templatesPath}${this.data.template}.html`;
            const response = await fetch(templateUrl);

            if (!response.ok) {
                throw new Error(`Template "${this.data.template}" not found. Check the template name in data.json.`);
            }

            const html = await response.text();
            const parser = new DOMParser();
            this.templateDoc = parser.parseFromString(html, 'text/html');
        },

        /**
         * Find element(s) in template by selector list
         */
        findElements(selectorList, single = true) {
            for (const selector of selectorList) {
                try {
                    if (single) {
                        const el = this.templateDoc.querySelector(selector);
                        if (el) return el;
                    } else {
                        const els = this.templateDoc.querySelectorAll(selector);
                        if (els.length > 0) return els;
                    }
                } catch (e) {
                    // Invalid selector, skip
                }
            }
            return null;
        },

        /**
         * Update element text content
         */
        setText(selectorList, value) {
            if (!value) return;
            const el = this.findElements(selectorList);
            if (el) {
                el.textContent = value;
            }
        },

        /**
         * Update element's href attribute and optionally text
         */
        setLink(selectorList, href, text = null) {
            if (!href) return;
            const elements = this.findElements(selectorList, false);
            if (elements) {
                elements.forEach(el => {
                    el.setAttribute('href', href);
                    if (text && el.querySelector('.info-value, .contact-value, .menu-label, .action-text')) {
                        el.querySelector('.info-value, .contact-value, .menu-label, .action-text').textContent = text;
                    } else if (text && !el.querySelector('i, svg, .icon')) {
                        // Only update text if element doesn't contain icons
                        el.textContent = text;
                    }
                });
            }
        },

        /**
         * Update image src attribute
         */
        setImage(selectorList, src) {
            if (!src) return;
            const elements = this.findElements(selectorList, false);
            if (elements) {
                elements.forEach(el => {
                    if (el.tagName === 'IMG') {
                        el.src = src;
                        el.alt = this.data.name || 'Profile';
                    } else {
                        const img = el.querySelector('img');
                        if (img) {
                            img.src = src;
                            img.alt = this.data.name || 'Profile';
                        }
                    }
                });
            }
        },

        /**
         * Show or hide social link based on data
         */
        setSocialLink(platform, url) {
            const selectorList = this.selectors[platform];
            if (!selectorList) return;

            const elements = this.findElements(selectorList, false);
            if (!elements) return;

            elements.forEach(el => {
                if (url) {
                    el.setAttribute('href', url);
                    el.style.display = '';
                } else {
                    // Hide if no URL provided
                    el.style.display = 'none';
                }
            });
        },

        /**
         * Populate the template with data
         */
        populateTemplate() {
            const data = this.data;

            // Page title
            const title = this.templateDoc.querySelector('title');
            if (title) {
                title.textContent = `${data.name}${data.title ? ' - ' + data.title : ''} | Digital Business Card`;
            }

            // Name
            this.setText(this.selectors.name, data.name);

            // Title/Position
            this.setText(this.selectors.title, data.title);

            // Company
            if (data.company) {
                this.setText(this.selectors.company, data.company);
            }
            if (data.tagline) {
                this.setText(this.selectors.tagline, data.tagline);
            }

            // Bio
            this.setText(this.selectors.bio, data.bio);

            // Photo - handle relative and absolute paths
            if (data.photo) {
                const photoSrc = data.photo.startsWith('http') ? data.photo : data.photo;
                this.setImage(this.selectors.photo, photoSrc);
            }

            // Logo
            if (data.logo) {
                this.setImage(this.selectors.logo, data.logo);
            }

            // Email
            if (data.email) {
                this.setLink(this.selectors.email, `mailto:${data.email}`, data.email);
            }

            // Phone
            if (data.phone) {
                const cleanPhone = data.phone.replace(/[^\d+]/g, '');
                const displayPhone = data.phoneDisplay || data.phone;
                this.setLink(this.selectors.phone, `tel:${cleanPhone}`, displayPhone);
            }

            // WhatsApp
            if (data.whatsapp) {
                const waNumber = data.whatsapp.replace(/[^\d]/g, '');
                this.setLink(this.selectors.whatsapp, `https://wa.me/${waNumber}`);
            } else if (data.phone) {
                // Use phone number for WhatsApp if no separate WhatsApp number
                const waNumber = data.phone.replace(/[^\d]/g, '');
                this.setLink(this.selectors.whatsapp, `https://wa.me/${waNumber}`);
            }

            // Website
            if (data.website) {
                const websiteUrl = data.website.startsWith('http') ? data.website : `https://${data.website}`;
                // Find website links (excluding social media)
                const websiteLinks = this.templateDoc.querySelectorAll('.action-btn.btn-web, .menu-item[href*="http"]:not([href*="linkedin"]):not([href*="facebook"]):not([href*="instagram"]):not([href*="twitter"]):not([href*="github"])');
                websiteLinks.forEach(el => {
                    el.setAttribute('href', websiteUrl);
                });

                // Also try generic website selectors
                const genericWebsite = this.templateDoc.querySelectorAll('a[href*="www."]:not([href*="linkedin"]):not([href*="facebook"]):not([href*="instagram"]):not([href*="twitter"]):not([href*="wa.me"]):not([href*="maps"])');
                genericWebsite.forEach(el => {
                    el.setAttribute('href', websiteUrl);
                });
            }

            // Address
            if (data.address) {
                // Find address elements
                const addressEls = this.templateDoc.querySelectorAll('.address, .location, .info-item:has(.fa-map-marker-alt) .info-value, .info-item:has(.fa-location-dot) .info-value');
                addressEls.forEach(el => {
                    if (el.tagName === 'A') {
                        el.textContent = data.address;
                        if (data.mapsUrl) {
                            el.setAttribute('href', data.mapsUrl);
                        }
                    } else {
                        el.textContent = data.address;
                    }
                });

                // Handle maps links
                if (data.mapsUrl) {
                    const mapsLinks = this.templateDoc.querySelectorAll('a[href*="maps.google"], a[href*="maps.app"], a[href*="goo.gl/maps"]');
                    mapsLinks.forEach(el => {
                        el.setAttribute('href', data.mapsUrl);
                    });
                }
            }

            // Social links
            const social = data.social || {};
            this.setSocialLink('linkedin', social.linkedin);
            this.setSocialLink('facebook', social.facebook);
            this.setSocialLink('instagram', social.instagram);
            this.setSocialLink('twitter', social.twitter);
            this.setSocialLink('github', social.github);
            this.setSocialLink('youtube', social.youtube);
            this.setSocialLink('tiktok', social.tiktok);

            // Stats (for templates that have them)
            if (data.stats && Array.isArray(data.stats)) {
                const statElements = this.templateDoc.querySelectorAll('.stat, .stat-item, .stats-item');
                data.stats.forEach((stat, index) => {
                    if (statElements[index]) {
                        const valueEl = statElements[index].querySelector('.stat-value, .stats-value, h3, strong');
                        const labelEl = statElements[index].querySelector('.stat-label, .stats-label, p, span:last-child');
                        if (valueEl && stat.value) valueEl.textContent = stat.value;
                        if (labelEl && stat.label) labelEl.textContent = stat.label;
                    }
                });
            }

            // Custom CSS variables (for theming)
            if (data.theme) {
                const style = this.templateDoc.createElement('style');
                let css = ':root {';
                if (data.theme.primaryColor) css += `--primary-color: ${data.theme.primaryColor};`;
                if (data.theme.accentColor) css += `--accent-color: ${data.theme.accentColor};`;
                if (data.theme.backgroundColor) css += `--bg-color: ${data.theme.backgroundColor};`;
                if (data.theme.textColor) css += `--text-color: ${data.theme.textColor};`;
                css += '}';
                style.textContent = css;
                this.templateDoc.head.appendChild(style);
            }

            // Update vCard filename in save button
            const saveBtn = this.templateDoc.querySelector('.save-btn, a[href$=".vcf"]');
            if (saveBtn) {
                const filename = data.name.toLowerCase().replace(/[^a-z0-9]+/g, '_') + '.vcf';
                saveBtn.setAttribute('href', filename);
            }

            // Add data attributes for vCard generator
            this.addVCardDataAttributes();
        },

        /**
         * Add data-vcard-* attributes for the vCard generator
         */
        addVCardDataAttributes() {
            const data = this.data;

            // Add to body for easy access
            const body = this.templateDoc.body;
            if (data.name) body.setAttribute('data-vcard-fn', data.name);
            if (data.title) body.setAttribute('data-vcard-title', data.title);
            if (data.company) body.setAttribute('data-vcard-org', data.company);
            if (data.email) body.setAttribute('data-vcard-email', data.email);
            if (data.phone) body.setAttribute('data-vcard-tel', data.phone.replace(/[^\d+]/g, ''));
            if (data.address) body.setAttribute('data-vcard-adr', data.address);
            if (data.website) body.setAttribute('data-vcard-url', data.website);
            if (data.bio) body.setAttribute('data-vcard-note', data.bio);

            // Social profiles
            const social = data.social || {};
            if (social.linkedin) body.setAttribute('data-vcard-social-linkedin', social.linkedin);
            if (social.facebook) body.setAttribute('data-vcard-social-facebook', social.facebook);
            if (social.instagram) body.setAttribute('data-vcard-social-instagram', social.instagram);
            if (social.twitter) body.setAttribute('data-vcard-social-twitter', social.twitter);
            if (social.whatsapp) body.setAttribute('data-vcard-social-whatsapp', data.whatsapp || `https://wa.me/${data.phone?.replace(/[^\d]/g, '')}`);
        },

        /**
         * Render the populated template
         */
        render() {
            // Get the full HTML
            const head = this.templateDoc.head.innerHTML;
            const body = this.templateDoc.body.innerHTML;
            const bodyAttrs = Array.from(this.templateDoc.body.attributes)
                .map(attr => `${attr.name}="${attr.value}"`)
                .join(' ');

            // Replace document content
            document.head.innerHTML = head;
            document.body.innerHTML = body;

            // Copy body attributes
            Array.from(this.templateDoc.body.attributes).forEach(attr => {
                document.body.setAttribute(attr.name, attr.value);
            });

            // Update base path for relative assets in template
            const base = document.createElement('base');
            base.href = this.config.templatesPath;
            document.head.insertBefore(base, document.head.firstChild);
        },

        /**
         * Load and initialize vCard generator
         */
        initVCardGenerator() {
            // Check if already loaded
            if (window.BizBioVCard) {
                window.BizBioVCard.init();
                return;
            }

            // Load the script
            const script = document.createElement('script');
            script.src = this.config.vcardGeneratorPath;
            script.onload = () => {
                if (window.BizBioVCard) {
                    window.BizBioVCard.init();
                }
            };
            document.body.appendChild(script);
        }
    };

    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => BizBioEngine.init());
    } else {
        BizBioEngine.init();
    }

    // Expose globally
    window.BizBioEngine = BizBioEngine;

})();

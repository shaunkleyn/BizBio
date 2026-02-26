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
            templatesPath: '../templates/',
            vcardGeneratorPath: '../vcard-generator.js',
            walletIntegrationPath: '../wallet-integration.js'
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

                // Initialize wallet integration
                this.initWalletIntegration();

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


            // Expose normalised data for Alpine-driven templates
            window.__bizbioData = this.normalizeData(this.data);
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


            // Wallet customization attributes
            const wallet = data.wallet || {};
            if (wallet.backgroundColor) body.setAttribute('data-wallet-bg', wallet.backgroundColor);
            if (wallet.cardTitle)        body.setAttribute('data-wallet-title', wallet.cardTitle);
            if (wallet.logoText)         body.setAttribute('data-wallet-logo', wallet.logoText);
            if (wallet.logoImageUrl)     body.setAttribute('data-wallet-logo-url', wallet.logoImageUrl);
            if (wallet.wideLogoUrl)      body.setAttribute('data-wallet-wide-logo-url', wallet.wideLogoUrl);
            if (wallet.heroImageUrl)     body.setAttribute('data-wallet-hero-url', wallet.heroImageUrl);
            if (wallet.qrCode)           body.setAttribute('data-wallet-qr', wallet.qrCode);
        },

        /**
         * Normalise raw data.json into a flat Alpine-friendly payload.
         * Sets window.__bizbioData so Alpine templates can bind to it.
         */
        normalizeData(data) {
            // ── Helpers ──
            const cleanPhone = (p) => p ? p.replace(/[^\d+]/g, '') : '';

            // ── Social ──
            const SOCIAL_MAP = {
                facebook:  { icon: 'fa-brands fa-facebook-f',  css: 'si-facebook'  },
                instagram: { icon: 'fa-brands fa-instagram',   css: 'si-instagram' },
                linkedin:  { icon: 'fa-brands fa-linkedin-in', css: 'si-linkedin'  },
                twitter:   { icon: 'fa-brands fa-x-twitter',   css: 'si-twitter'   },
                whatsapp:  { icon: 'fa-brands fa-whatsapp',    css: 'si-whatsapp'  },
                youtube:   { icon: 'fa-brands fa-youtube',     css: 'si-youtube'   },
                tiktok:    { icon: 'fa-brands fa-tiktok',      css: 'si-tiktok'    },
                pinterest: { icon: 'fa-brands fa-pinterest-p', css: 'si-pinterest' },
                github:    { icon: 'fa-brands fa-github',      css: 'si-github'    },
                website:   { icon: 'fa-solid fa-globe',        css: 'si-website'   },
            };
            const detectPlatform = (url) => {
                for (const p of Object.keys(SOCIAL_MAP)) {
                    if (url.includes(p + '.com') || url.includes(p + '.')) return p;
                }
                return 'website';
            };
            let social = [];
            if (Array.isArray(data.social)) {
                social = data.social.map(item => {
                    const url = typeof item === 'string' ? item : item.url;
                    const platform = typeof item === 'string' ? detectPlatform(url) : (item.platform || detectPlatform(url));
                    return { platform, url, ...(SOCIAL_MAP[platform] || SOCIAL_MAP.website) };
                });
            } else if (data.social && typeof data.social === 'object') {
                social = Object.entries(data.social)
                    .filter(([, url]) => url)
                    .map(([platform, url]) => ({ platform, url, ...(SOCIAL_MAP[platform] || SOCIAL_MAP.website) }));
            }

            // ── Info ──
            const INFO_DEFAULTS = {
                email:    { icon: 'fa-solid fa-envelope',     title: 'Email',    button: 'Email Me',  hrefPrefix: 'mailto:' },
                phone:    { icon: 'fa-solid fa-phone',        title: 'Mobile',   button: 'Call Now',  hrefPrefix: 'tel:'    },
                website:  { icon: 'fa-solid fa-globe',        title: 'Website',  button: 'Visit Us',  hrefPrefix: ''        },
                address:  { icon: 'fa-solid fa-location-dot', title: 'Address',  button: 'Open Maps', hrefPrefix: ''        },
                whatsapp: { icon: 'fa-brands fa-whatsapp',    title: 'WhatsApp', button: 'WhatsApp',  hrefPrefix: 'https://wa.me/' },
            };
            const detectInfoType = (value) => {
                if (value.includes('@'))                         return 'email';
                if (/^[+\d\s\-().]+$/.test(value.trim()))        return 'phone';
                if (/^(https?:\/\/|www\.)/i.test(value.trim()))  return 'website';
                return 'address';
            };
            const buildHref = (type, value, customHref) => {
                if (customHref) return customHref;
                const def = INFO_DEFAULTS[type] || INFO_DEFAULTS.address;
                if (type === 'address') return 'https://www.google.com/maps/search/' + encodeURIComponent(value);
                if (type === 'website' && !value.startsWith('http')) return 'https://' + value;
                return def.hrefPrefix + value;
            };
            // Resolve a custom icon field: platform name → SOCIAL_MAP, fa- prefix → use as-is, else default
            const resolveIcon = (iconField, type) => {
                if (iconField) {
                    if (SOCIAL_MAP[iconField]) return SOCIAL_MAP[iconField].icon;
                    if (iconField.startsWith('fa-')) return iconField;
                }
                return (INFO_DEFAULTS[type] || INFO_DEFAULTS.address).icon;
            };
            let info = [];
            if (Array.isArray(data.info)) {
                info = data.info.map(item => {
                    if (typeof item === 'string') {
                        const type = detectInfoType(item);
                        const def = INFO_DEFAULTS[type];
                        return { type, title: def.title, value: item, button: def.button, href: buildHref(type, item, null), icon: def.icon };
                    }
                    const type = item.type || detectInfoType(item.value);
                    const def = INFO_DEFAULTS[type] || INFO_DEFAULTS.address;
                    return {
                        type,
                        title:  item.title  || def.title,
                        value:  item.value,
                        button: item.button || def.button,
                        href:   buildHref(type, item.value, item.href),
                        icon:   resolveIcon(item.icon, type),
                    };
                });
            }

            // ── Flat fields ──
            const phoneClean = cleanPhone(data.phone);
            const waNumber   = data.whatsapp ? cleanPhone(data.whatsapp) : phoneClean;
            const websiteUrl = data.website
                ? (data.website.startsWith('http') ? data.website : 'https://' + data.website)
                : '';
            const nameParts = (data.name || '').split(' ');
            const initials  = nameParts.length >= 2
                ? nameParts[0][0] + nameParts[nameParts.length - 1][0]
                : (data.name || '').slice(0, 2).toUpperCase();

            return {
                name:           data.name        || '',
                company:        data.company     || data.name || '',
                title:          data.title       || '',
                tagline:        data.tagline     || data.title || '',
                bio:            data.bio         || '',
                photo:          data.photo       || '',
                initials,
                email:          data.email       || '',
                emailHref:      data.email ? 'mailto:' + data.email : '',
                phone:          data.phoneDisplay || data.phone || '',
                phoneHref:      phoneClean ? 'tel:' + phoneClean : '',
                whatsappHref:   waNumber ? 'https://wa.me/' + waNumber : '',
                website:        data.website     || '',
                websiteUrl,
                websiteDisplay: websiteUrl.replace(/^https?:\/\//, ''),
                address:        data.address     || '',
                mapsUrl:        data.mapsUrl     || (data.address ? 'https://www.google.com/maps/search/' + encodeURIComponent(data.address) : ''),
                googleReviewUrl:   data.googleReviewUrl   || '',
                facebookReviewUrl: data.facebookReviewUrl || '',
                social,
                info,
            };
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

            // Inject Alpine.js for templates that use it
            if (this.templateDoc.querySelector('[x-data]')) {
                const alpine = document.createElement('script');
                alpine.src = 'https://cdn.jsdelivr.net/npm/alpinejs@3.14.3/dist/cdn.min.js';
                document.body.appendChild(alpine);
            }
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
                    },

        /**
         * Load and initialize wallet integration (Google / Apple Wallet buttons)
         */
        initWalletIntegration() {
            if (window.BizBioWallet) {
                window.BizBioWallet.init();
                return;
            }

            const script = document.createElement('script');
            script.src = this.config.walletIntegrationPath;
            script.onload = () => {
                if (window.BizBioWallet) {
                    window.BizBioWallet.init();
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

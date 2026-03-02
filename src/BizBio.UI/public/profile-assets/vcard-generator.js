/**
 * BizBio vCard Generator
 * A reusable script to extract contact details from HTML and generate/share vCard files
 *
 * Usage:
 * 1. Include this script in your HTML: <script src="vcard-generator.js"></script>
 * 2. Optionally add data-vcard-* attributes to elements for explicit mapping
 * 3. Call BizBioVCard.init() or let it auto-initialize on DOMContentLoaded
 *
 * Data attributes (all optional - script will auto-detect from common selectors):
 * - data-vcard-fn: Full/Display name
 * - data-vcard-given-name: First name
 * - data-vcard-family-name: Last name
 * - data-vcard-nickname: Nickname
 * - data-vcard-org: Organization/Company
 * - data-vcard-title: Job title
 * - data-vcard-email: Email address
 * - data-vcard-tel: Phone number
 * - data-vcard-tel-work: Work phone
 * - data-vcard-tel-cell: Cell phone
 * - data-vcard-adr: Full address
 * - data-vcard-url: Website URL
 * - data-vcard-photo: Photo URL or base64
 * - data-vcard-note: Notes/Bio
 * - data-vcard-social-*: Social profiles (facebook, instagram, linkedin, twitter, whatsapp)
 *
 * Configuration via data-vcard-config on body or script tag:
 * - filename: Custom filename for the vCard
 * - show-as: "COMPANY" or "INDIVIDUAL"
 */

(function(window) {
    'use strict';

    const BizBioVCard = {
        config: {
            filename: null,
            showAs: null,
            fabPosition: 'br', // br, bl, tr, tl
            fabAnimation: 'zoomin', // zoomin, slidein, fountain
            accentColor: null
        },

        // CSS selectors to auto-detect contact information
        selectors: {
            name: [
                '[data-vcard-fn]',
                '.name', '.card-name', '.profile-name', '.user-name',
                'h1.name', 'h1', '.header h1', '.card h1'
            ],
            title: [
                '[data-vcard-title]',
                '.title', '.job-title', '.position', '.role',
                '.card-title:not(.card-header .card-title)'
            ],
            org: [
                '[data-vcard-org]',
                '.company', '.organization', '.org', '.employer'
            ],
            email: [
                '[data-vcard-email]',
                'a[href^="mailto:"]',
                '.email', '.mail'
            ],
            phone: [
                '[data-vcard-tel]',
                'a[href^="tel:"]',
                '.phone', '.tel', '.telephone'
            ],
            website: [
                '[data-vcard-url]',
                '.website a', '.web a', 'a.website',
                '.info-item a[href*="http"]:not([href*="wa.me"]):not([href*="linkedin"]):not([href*="facebook"]):not([href*="instagram"]):not([href*="twitter"]):not([href*="maps"])'
            ],
            address: [
                '[data-vcard-adr]',
                '.address', '.location', '.adr',
                'a[href*="maps.google"]', 'a[href*="maps.app"]'
            ],
            photo: [
                '[data-vcard-photo]',
                '.avatar', '.profile-image img', '.avatar img',
                '.photo img', 'img.avatar', 'img.profile', 'img.logo'
            ],
            bio: [
                '[data-vcard-note]',
                '.bio', '.about', '.description', '.tagline'
            ],
            linkedin: [
                '[data-vcard-social-linkedin]',
                'a[href*="linkedin.com"]'
            ],
            facebook: [
                '[data-vcard-social-facebook]',
                'a[href*="facebook.com"]'
            ],
            instagram: [
                '[data-vcard-social-instagram]',
                'a[href*="instagram.com"]'
            ],
            twitter: [
                '[data-vcard-social-twitter]',
                'a[href*="twitter.com"]', 'a[href*="x.com"]'
            ],
            whatsapp: [
                '[data-vcard-social-whatsapp]',
                'a[href*="wa.me"]', 'a[href*="whatsapp"]'
            ]
        },

        /**
         * Initialize the vCard generator
         */
        init: function(options = {}) {
            if (this._initialized) return;
            this._initialized = true;
            this.config = { ...this.config, ...options };
            this.detectAccentColor();
            this.checkAndAddFAB();
            this.setupEventListeners();
            console.log('BizBio vCard Generator initialized');
        },

        /**
         * Detect the template's accent/primary color from CSS
         */
        detectAccentColor: function() {
            if (this.config.accentColor) return;

            // Try to find accent color from various sources
            const sources = [
                // CSS custom properties
                () => getComputedStyle(document.documentElement).getPropertyValue('--primary-color'),
                () => getComputedStyle(document.documentElement).getPropertyValue('--accent-color'),
                () => getComputedStyle(document.documentElement).getPropertyValue('--theme-color'),
                // From save/action buttons
                () => {
                    const btn = document.querySelector('.save-btn, .action-btn, .cta-btn, [class*="save"], [class*="primary"]');
                    if (btn) {
                        const bg = getComputedStyle(btn).backgroundColor;
                        if (bg && bg !== 'rgba(0, 0, 0, 0)' && bg !== 'transparent') return bg;
                    }
                    return null;
                },
                // From gradients
                () => {
                    const btn = document.querySelector('.save-btn, .action-btn, .cta-btn');
                    if (btn) {
                        const bg = getComputedStyle(btn).backgroundImage;
                        const match = bg.match(/rgb[a]?\([^)]+\)/);
                        if (match) return match[0];
                    }
                    return null;
                }
            ];

            for (const source of sources) {
                try {
                    const color = source();
                    if (color && color.trim()) {
                        this.config.accentColor = color.trim();
                        return;
                    }
                } catch (e) {}
            }

            // Default fallback
            this.config.accentColor = '#3b82f6';
        },

        /**
         * Check if FAB exists and add one if not
         */
        checkAndAddFAB: function() {
            // Check for existing FAB or save button
            // Note: avoid '.fab' selector as it matches Font Awesome brand icons (class="fab fa-*")
            const existingFAB = document.querySelector('#bizbio-vcard-fab, .mfb-component, [class*="floating-action"]');
            const existingSaveBtn = document.querySelector('.save-btn[href$=".vcf"], a[download][href$=".vcf"]');

            if (existingFAB || existingSaveBtn) {
                // Attach event listeners to existing elements
                if (existingSaveBtn) {
                    existingSaveBtn.addEventListener('click', (e) => {
                        e.preventDefault();
                        this.downloadVCard();
                    });
                }
                return;
            }

            // Add FAB
            this.injectFABStyles();
            this.createFAB();
        },

        /**
         * Inject FAB CSS styles
         */
        injectFABStyles: function() {
            const style = document.createElement('style');
            style.id = 'bizbio-vcard-fab-styles';
            style.textContent = `
                .bizbio-fab {
                    position: fixed;
                    bottom: 25px;
                    right: 25px;
                    z-index: 9999;
                    font-family: inherit;
                }
                .bizbio-fab-main {
                    width: 56px;
                    height: 56px;
                    border-radius: 50%;
                    background: ${this.config.accentColor};
                    border: none;
                    cursor: pointer;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.25);
                    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                    color: white;
                    font-size: 20px;
                }
                .bizbio-fab-main:hover {
                    transform: scale(1.1);
                    box-shadow: 0 6px 20px rgba(0,0,0,0.35);
                }
                .bizbio-fab-main svg {
                    width: 24px;
                    height: 24px;
                    fill: currentColor;
                    transition: transform 0.3s;
                }
                .bizbio-fab.open .bizbio-fab-main svg {
                    transform: rotate(45deg);
                }
                .bizbio-fab-menu {
                    position: absolute;
                    bottom: 70px;
                    right: 0;
                    display: flex;
                    flex-direction: column;
                    gap: 12px;
                    opacity: 0;
                    visibility: hidden;
                    transform: translateY(20px);
                    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                }
                .bizbio-fab.open .bizbio-fab-menu {
                    opacity: 1;
                    visibility: visible;
                    transform: translateY(0);
                }
                .bizbio-fab-item {
                    display: flex;
                    align-items: center;
                    justify-content: flex-end;
                    gap: 12px;
                }
                .bizbio-fab-label {
                    background: rgba(0,0,0,0.7);
                    color: white;
                    padding: 6px 12px;
                    border-radius: 4px;
                    font-size: 13px;
                    white-space: nowrap;
                    opacity: 0;
                    transform: translateX(10px);
                    transition: all 0.2s;
                }
                .bizbio-fab-item:hover .bizbio-fab-label {
                    opacity: 1;
                    transform: translateX(0);
                }
                .bizbio-fab-btn {
                    width: 46px;
                    height: 46px;
                    border-radius: 50%;
                    background: ${this.config.accentColor};
                    border: none;
                    cursor: pointer;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.2);
                    transition: all 0.3s;
                    color: white;
                    font-size: 16px;
                }
                .bizbio-fab-btn:hover {
                    transform: scale(1.1);
                }
                .bizbio-fab-btn svg {
                    width: 20px;
                    height: 20px;
                    fill: currentColor;
                }
                .bizbio-fab-backdrop {
                    position: fixed;
                    inset: 0;
                    background: rgba(0,0,0,0.3);
                    opacity: 0;
                    visibility: hidden;
                    transition: all 0.3s;
                    z-index: 9998;
                }
                .bizbio-fab.open + .bizbio-fab-backdrop {
                    opacity: 1;
                    visibility: visible;
                }
            `;
            document.head.appendChild(style);
        },

        /**
         * Create and inject FAB element
         */
        createFAB: function() {
            const fab = document.createElement('div');
            fab.className = 'bizbio-fab';
            fab.id = 'bizbio-vcard-fab';
            fab.innerHTML = `
                <button class="bizbio-fab-main" aria-label="Share options">
                    <svg viewBox="0 0 24 24"><path d="M18 16.08c-.76 0-1.44.3-1.96.77L8.91 12.7c.05-.23.09-.46.09-.7s-.04-.47-.09-.7l7.05-4.11c.54.5 1.25.81 2.04.81 1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3c0 .24.04.47.09.7L8.04 9.81C7.5 9.31 6.79 9 6 9c-1.66 0-3 1.34-3 3s1.34 3 3 3c.79 0 1.5-.31 2.04-.81l7.12 4.16c-.05.21-.08.43-.08.65 0 1.61 1.31 2.92 2.92 2.92s2.92-1.31 2.92-2.92-1.31-2.92-2.92-2.92z"/></svg>
                </button>
                <div class="bizbio-fab-menu">
                    <div class="bizbio-fab-item">
                        <span class="bizbio-fab-label">Download Contact</span>
                        <button class="bizbio-fab-btn" data-action="download" aria-label="Download vCard">
                            <svg viewBox="0 0 24 24"><path d="M19 9h-4V3H9v6H5l7 7 7-7zM5 18v2h14v-2H5z"/></svg>
                        </button>
                    </div>
                    <div class="bizbio-fab-item">
                        <span class="bizbio-fab-label">Share Contact</span>
                        <button class="bizbio-fab-btn" data-action="share" aria-label="Share Contact">
                            <svg viewBox="0 0 24 24"><path d="M18 16.08c-.76 0-1.44.3-1.96.77L8.91 12.7c.05-.23.09-.46.09-.7s-.04-.47-.09-.7l7.05-4.11c.54.5 1.25.81 2.04.81 1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3c0 .24.04.47.09.7L8.04 9.81C7.5 9.31 6.79 9 6 9c-1.66 0-3 1.34-3 3s1.34 3 3 3c.79 0 1.5-.31 2.04-.81l7.12 4.16c-.05.21-.08.43-.08.65 0 1.61 1.31 2.92 2.92 2.92s2.92-1.31 2.92-2.92-1.31-2.92-2.92-2.92z"/></svg>
                        </button>
                    </div>
                    <div class="bizbio-fab-item">
                        <span class="bizbio-fab-label">Share Profile Link</span>
                        <button class="bizbio-fab-btn" data-action="share-link" aria-label="Share Profile Link">
                            <svg viewBox="0 0 24 24"><path d="M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z"/></svg>
                        </button>
                    </div>
                </div>
            `;

            const backdrop = document.createElement('div');
            backdrop.className = 'bizbio-fab-backdrop';
            backdrop.id = 'bizbio-vcard-fab-backdrop';

            document.body.appendChild(fab);
            document.body.appendChild(backdrop);
        },

        /**
         * Setup event listeners
         */
        setupEventListeners: function() {
            document.addEventListener('click', (e) => {
                console.log('FAB click event:', e.target);
                const fab = document.getElementById('bizbio-vcard-fab');
                const backdrop = document.getElementById('bizbio-vcard-fab-backdrop');

                // Toggle FAB menu
                if (e.target.closest('.bizbio-fab-main')) {
                    fab?.classList.toggle('open');
                    return;
                }

                // Close FAB when clicking backdrop
                if (e.target === backdrop) {
                    fab?.classList.remove('open');
                    return;
                }

                // Handle FAB actions
                const actionBtn = e.target.closest('[data-action]');
                if (actionBtn) {
                    const action = actionBtn.dataset.action;
                    fab?.classList.remove('open');

                    switch (action) {
                        case 'download':
                            this.downloadVCard();
                            break;
                        case 'share':
                            this.shareVCard();
                            break;
                        case 'share-link':
                            this.shareProfileLink();
                            break;
                    }
                }
            });
        },

        /**
         * Extract value from element using selectors
         */
        extractValue: function(type) {
            // Map type to data attribute name
            const attrMap = {
                'name': 'fn',
                'title': 'title',
                'org': 'org',
                'email': 'email',
                'phone': 'tel',
                'website': 'url',
                'address': 'adr',
                'photo': 'photo',
                'bio': 'note',
                'linkedin': 'social-linkedin',
                'facebook': 'social-facebook',
                'instagram': 'social-instagram',
                'twitter': 'social-twitter',
                'whatsapp': 'social-whatsapp'
            };

            // Check body data attributes first (set by bizbio-engine)
            const attrName = attrMap[type] || type;
            const bodyAttr = document.body.getAttribute(`data-vcard-${attrName}`);
            if (bodyAttr) return bodyAttr;

            const selectors = this.selectors[type];
            if (!selectors) return null;

            for (const selector of selectors) {
                const el = document.querySelector(selector);
                if (!el) continue;

                // Check for data attribute on element
                if (el.hasAttribute(`data-vcard-${attrName}`) || el.hasAttribute('data-vcard-fn')) {
                    return el.getAttribute(`data-vcard-${attrName}`) ||
                           el.getAttribute('data-vcard-fn') ||
                           el.textContent?.trim();
                }

                // Handle different element types
                if (el.tagName === 'A') {
                    const href = el.getAttribute('href') || '';
                    if (href.startsWith('mailto:')) return href.replace('mailto:', '');
                    if (href.startsWith('tel:')) return href.replace('tel:', '');
                    if (href.includes('http')) return href;
                    return el.textContent?.trim() || href;
                }

                if (el.tagName === 'IMG') {
                    return el.src;
                }

                return el.textContent?.trim();
            }

            return null;
        },

        /**
         * Extract all contact data from the page
         */
        extractContactData: function() {
            const data = {
                fn: this.extractValue('name'),
                title: this.extractValue('title'),
                org: this.extractValue('org'),
                email: this.extractValue('email'),
                tel: this.extractValue('phone'),
                url: this.extractValue('website'),
                adr: this.extractValue('address'),
                photo: this.extractValue('photo'),
                note: this.extractValue('bio'),
                social: {
                    linkedin: this.extractValue('linkedin'),
                    facebook: this.extractValue('facebook'),
                    instagram: this.extractValue('instagram'),
                    twitter: this.extractValue('twitter'),
                    whatsapp: this.extractValue('whatsapp')
                }
            };

            // Parse name into components
            if (data.fn) {
                const nameParts = data.fn.split(' ');
                if (nameParts.length >= 2) {
                    data.givenName = nameParts[0];
                    data.familyName = nameParts.slice(1).join(' ');
                } else {
                    data.givenName = data.fn;
                    data.familyName = '';
                }
            }

            // Clean up URL
            if (data.url && !data.url.startsWith('http')) {
                data.url = 'https://' + data.url.replace(/^\/+/, '');
            }

            // Clean up phone number
            if (data.tel) {
                data.tel = data.tel.replace(/[^\d+]/g, '');
            }

            return data;
        },

        /**
         * Convert image URL to base64
         */
        async imageToBase64(url) {
            return new Promise((resolve, reject) => {
                // Handle data URLs
                if (url.startsWith('data:')) {
                    const base64 = url.split(',')[1];
                    resolve(base64);
                    return;
                }

                const img = new Image();
                img.crossOrigin = 'anonymous';

                img.onload = () => {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');

                    // Resize if too large
                    const maxSize = 200;
                    let width = img.width;
                    let height = img.height;

                    if (width > maxSize || height > maxSize) {
                        if (width > height) {
                            height = (height / width) * maxSize;
                            width = maxSize;
                        } else {
                            width = (width / height) * maxSize;
                            height = maxSize;
                        }
                    }

                    canvas.width = width;
                    canvas.height = height;
                    ctx.drawImage(img, 0, 0, width, height);

                    const dataUrl = canvas.toDataURL('image/jpeg', 0.8);
                    const base64 = dataUrl.split(',')[1];
                    resolve(base64);
                };

                img.onerror = () => {
                    console.warn('Failed to load image for vCard:', url);
                    resolve(null);
                };

                img.src = url;
            });
        },

        /**
         * Escape special characters for vCard
         */
        escapeVCard: function(str) {
            if (!str) return '';
            return str
                .replace(/\\/g, '\\\\')
                .replace(/;/g, '\\;')
                .replace(/,/g, '\\,')
                .replace(/\n/g, '\\n');
        },

        /**
         * Fold long lines for vCard (max 75 chars per line)
         */
        foldLine: function(line) {
            const maxLength = 75;
            if (line.length <= maxLength) return line;

            let result = '';
            let currentLine = '';

            for (let i = 0; i < line.length; i++) {
                currentLine += line[i];
                if (currentLine.length >= maxLength) {
                    result += currentLine + '\r\n ';
                    currentLine = '';
                }
            }

            return result + currentLine;
        },

        /**
         * Generate vCard string
         */
        async generateVCard() {
            const data = this.extractContactData();
            let vcard = [];

            vcard.push('BEGIN:VCARD');
            vcard.push('VERSION:3.0');
            vcard.push('PRODID:-//BizBio//Digital Business Card//EN');

            // Name
            if (data.fn) {
                vcard.push(`N:${this.escapeVCard(data.familyName)};${this.escapeVCard(data.givenName)};;;`);
                vcard.push(`FN:${this.escapeVCard(data.fn)}`);
            }

            // Organization
            if (data.org) {
                vcard.push(`ORG:${this.escapeVCard(data.org)};`);
            }

            // Title
            if (data.title) {
                vcard.push(`TITLE:${this.escapeVCard(data.title)}`);
            }

            // Email
            if (data.email) {
                vcard.push(`EMAIL;type=INTERNET;type=WORK;type=pref:${data.email}`);
            }

            // Phone
            if (data.tel) {
                vcard.push(`TEL;type=CELL;type=VOICE;type=pref:${data.tel}`);
            }

            // Address
            if (data.adr) {
                const addr = this.escapeVCard(data.adr);
                vcard.push(`ADR;type=WORK;type=pref:;;${addr};;;;`);
            }

            // Website
            if (data.url) {
                vcard.push(`URL;type=WORK:${data.url}`);
            }

            // Social profiles
            if (data.social.linkedin) {
                vcard.push(`X-SOCIALPROFILE;type=linkedin:${data.social.linkedin}`);
            }
            if (data.social.facebook) {
                vcard.push(`X-SOCIALPROFILE;type=facebook:${data.social.facebook}`);
            }
            if (data.social.instagram) {
                vcard.push(`X-SOCIALPROFILE;type=instagram:${data.social.instagram}`);
            }
            if (data.social.twitter) {
                vcard.push(`X-SOCIALPROFILE;type=twitter:${data.social.twitter}`);
            }
            if (data.social.whatsapp) {
                vcard.push(`X-SOCIALPROFILE;type=WhatsApp:${data.social.whatsapp}`);
            }

            // Photo (base64 encoded)
            if (data.photo) {
                try {
                    const base64 = await this.imageToBase64(data.photo);
                    if (base64) {
                        vcard.push(this.foldLine(`PHOTO;ENCODING=b;TYPE=JPEG:${base64}`));
                    }
                } catch (e) {
                    console.warn('Failed to encode photo:', e);
                }
            }

            // Note/Bio
            if (data.note) {
                vcard.push(`NOTE:${this.escapeVCard(data.note)}`);
            }

            // Show as company if org is set but name looks like company
            if (data.org && (!data.givenName || data.fn === data.org)) {
                vcard.push('X-ABShowAs:COMPANY');
            }

            vcard.push('END:VCARD');

            return vcard.join('\r\n');
        },

        /**
         * Get filename for vCard
         */
        getFilename: function() {
            if (this.config.filename) return this.config.filename;

            const data = this.extractContactData();
            let name = data.fn || data.org || 'contact';

            // Clean filename
            name = name
                .toLowerCase()
                .replace(/[^a-z0-9]+/g, '_')
                .replace(/^_+|_+$/g, '');

            return `${name}.vcf`;
        },

        /**
         * Download vCard file
         */
        async downloadVCard() {
            try {
                const vcardContent = await this.generateVCard();
                const blob = new Blob([vcardContent], { type: 'text/vcard;charset=utf-8' });
                const url = URL.createObjectURL(blob);

                const link = document.createElement('a');
                link.href = url;
                link.download = this.getFilename();
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);

                URL.revokeObjectURL(url);
            } catch (e) {
                console.error('Failed to download vCard:', e);
                alert('Failed to generate contact card. Please try again.');
            }
        },

        /**
         * Share vCard file (mobile)
         */
        async shareVCard() {
            try {
                const vcardContent = await this.generateVCard();
                const blob = new Blob([vcardContent], { type: 'text/vcard' });
                const file = new File([blob], this.getFilename(), { type: 'text/vcard' });

                // Try file sharing first (mobile)
                if (navigator.share && navigator.canShare) {
                    try {
                        if (navigator.canShare({ files: [file] })) {
                            await navigator.share({
                                title: 'Contact Details',
                                text: 'Here are my contact details. Get yours on SnapTap.co.za today!',
                                files: [file]
                            });
                            return;
                        }
                    } catch (e) {
                        console.log('File sharing failed, falling back to URL sharing');
                    }
                }

                // Fallback to URL sharing
                if (navigator.share) {
                    await navigator.share({
                        title: 'Contact Details',
                        text: 'Here are my contact details. Get yours on SnapTap.co.za today!',
                        url: window.location.href
                    });
                    return;
                }

                // Ultimate fallback - download
                this.downloadVCard();

            } catch (e) {
                if (e.name !== 'AbortError') {
                    console.error('Sharing failed:', e);
                    this.downloadVCard();
                }
            }
        },

        /**
         * Share profile link
         */
        async shareProfileLink() {
            try {
                if (navigator.share) {
                    await navigator.share({
                        title: document.title || 'My Digital Business Card',
                        text: 'Check out my digital business card!',
                        url: window.location.href
                    });
                } else {
                    // Fallback: copy to clipboard
                    await navigator.clipboard.writeText(window.location.href);
                    alert('Profile link copied to clipboard!');
                }
            } catch (e) {
                if (e.name !== 'AbortError') {
                    console.error('Sharing failed:', e);
                    try {
                        await navigator.clipboard.writeText(window.location.href);
                        alert('Profile link copied to clipboard!');
                    } catch (clipErr) {
                        alert('Unable to share. Please copy the URL manually.');
                    }
                }
            }
        }
    };

    // Auto-initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => BizBioVCard.init());
    } else {
        BizBioVCard.init();
    }

    // Expose globally
    window.BizBioVCard = BizBioVCard;

})(window);

/**
 * BizBio Wallet Integration
 * Adds "Save to Google Wallet" functionality to digital business cards.
 *
 * Loaded automatically by bizbio-engine.js after the card template renders.
 * Reads card data from data-vcard-* attributes on document.body.
 */

(function () {
    'use strict';

    const BizBioWallet = {

        /**
         * Detect the user's platform.
         * Returns 'ios', 'android', or 'desktop'.
         */
        getPlatform() {
            const ua = navigator.userAgent;
            if (/iPad|iPhone|iPod/.test(ua) ||
                (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1)) {
                return 'ios';
            }
            if (/Android/.test(ua)) {
                return 'android';
            }
            return 'desktop';
        },

        /**
         * Read card data from the data-vcard-* attributes set by bizbio-engine.js.
         */
        getCardData() {
            const body = document.body;
            return {
                name: body.getAttribute('data-vcard-fn') || '',
                title: body.getAttribute('data-vcard-title') || '',
                company: body.getAttribute('data-vcard-org') || '',
                email: body.getAttribute('data-vcard-email') || '',
                phone: body.getAttribute('data-vcard-tel') || '',
                address: body.getAttribute('data-vcard-adr') || '',
                website: body.getAttribute('data-vcard-url') || '',
                bio: body.getAttribute('data-vcard-note') || '',
                // The current page URL is the shareable profile link
                profileUrl: window.location.href,
                // Photo is harder to pass; skip for wallet unless absolute URL
                photo: null,

                // Wallet appearance customisation (set via data.wallet in data.json)
                walletBg:          body.getAttribute('data-wallet-bg')            || null,
                walletTitle:       body.getAttribute('data-wallet-title')         || null,
                walletLogo:        body.getAttribute('data-wallet-logo')          || null,
                walletLogoUrl:     body.getAttribute('data-wallet-logo-url')      || null,
                walletWideLogoUrl: body.getAttribute('data-wallet-wide-logo-url') || null,
                walletHeroUrl:     body.getAttribute('data-wallet-hero-url')      || null,
                // QR code value — defaults to the current profile URL if not set explicitly
                walletQr:          body.getAttribute('data-wallet-qr')            || window.location.href,
            };
        },

        /**
         * Create the "Add to Google Wallet" button element.
         */
        createGoogleWalletButton() {
            const wrapper = document.createElement('div');
            wrapper.className = 'bizbio-wallet-btn-wrapper';
            wrapper.style.cssText = [
                'display: flex',
                'justify-content: center',
                'margin: 12px auto 4px',
                'max-width: 320px',
            ].join(';');

            const btn = document.createElement('button');
            btn.id = 'bizbio-google-wallet-btn';
            btn.title = 'Add to Google Wallet';
            btn.style.cssText = [
                'display: inline-flex',
                'align-items: center',
                'gap: 8px',
                'background: #000000',
                'color: #ffffff',
                'border: none',
                'border-radius: 4px',
                'padding: 0 16px',
                'height: 48px',
                'font-family: "Google Sans", Roboto, Arial, sans-serif',
                'font-size: 14px',
                'font-weight: 500',
                'letter-spacing: 0.25px',
                'cursor: pointer',
                'text-decoration: none',
                'transition: opacity 0.2s',
                'white-space: nowrap',
                'user-select: none',
            ].join(';');

            // Google Wallet "G" logo SVG (official colour)
            btn.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="20" height="20" fill="none">
                    <path d="M12 5c-3.86 0-7 3.14-7 7s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7z" fill="#4285F4"/>
                    <path d="M12 7c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zm0 2c.55 0 1.08.13 1.54.37L9.37 13.54A2.976 2.976 0 0 1 9 12c0-1.66 1.34-3 3-3zm0 6c-.55 0-1.08-.13-1.54-.37l4.17-4.17c.24.46.37.99.37 1.54 0 1.66-1.34 3-3 3z" fill="#fff"/>
                </svg>
                <span>Add to Google Wallet</span>
            `;

            btn.addEventListener('mouseenter', () => { btn.style.opacity = '0.85'; });
            btn.addEventListener('mouseleave', () => { btn.style.opacity = '1'; });

            btn.addEventListener('click', () => this.handleGoogleWallet(btn));

            wrapper.appendChild(btn);
            return wrapper;
        },

        /**
         * Handle the Google Wallet button click.
         */
        async handleGoogleWallet(btn) {
            const originalHTML = btn.innerHTML;
            btn.disabled = true;
            btn.style.opacity = '0.6';
            btn.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="20" height="20" fill="none" style="animation: bizbio-spin 1s linear infinite;">
                    <circle cx="12" cy="12" r="9" stroke="#ffffff" stroke-width="2" stroke-dasharray="56" stroke-dashoffset="14"/>
                </svg>
                <span>Opening Wallet...</span>
            `;

            try {
                const cardData = this.getCardData();

                const response = await fetch('/api/wallet/google', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(cardData),
                });

                if (!response.ok) {
                    const err = await response.json().catch(() => ({}));
                    throw new Error(err.message || `Server error ${response.status}`);
                }

                const { url } = await response.json();
                window.open(url, '_blank', 'noopener,noreferrer');

            } catch (error) {
                console.error('[BizBio Wallet] Google Wallet error:', error);
                this.showError(btn, error.message || 'Unable to open Google Wallet. Please try again.');
            } finally {
                btn.disabled = false;
                btn.style.opacity = '1';
                btn.innerHTML = originalHTML;
                // Re-attach event after innerHTML reset
                btn.addEventListener('mouseenter', () => { btn.style.opacity = '0.85'; });
                btn.addEventListener('mouseleave', () => { btn.style.opacity = '1'; });
                btn.addEventListener('click', () => this.handleGoogleWallet(btn));
            }
        },

        /**
         * Show a brief error message below the button.
         */
        showError(btn, message) {
            const existing = document.getElementById('bizbio-wallet-error');
            if (existing) existing.remove();

            const el = document.createElement('p');
            el.id = 'bizbio-wallet-error';
            el.textContent = message;
            el.style.cssText = [
                'color: #ef4444',
                'font-size: 12px',
                'text-align: center',
                'margin: 4px 0 0',
                'font-family: system-ui, sans-serif',
            ].join(';');

            btn.closest('.bizbio-wallet-btn-wrapper')?.after(el);
            setTimeout(() => el.remove(), 5000);
        },

        /**
         * Find the best insertion point for wallet buttons.
         * Inserts after the "Save Contact" / vCard button if it exists,
         * otherwise appends to the main card container.
         */
        findInsertionPoint() {
            // Try to find the save contact / vCard button area
            const saveBtn = document.querySelector('.save-btn, a[href$=".vcf"], .btn-save, [class*="save-contact"]');
            if (saveBtn) {
                return { el: saveBtn.closest('div, section, .actions, .btn-group') || saveBtn, position: 'afterend' };
            }

            // Try common card containers
            const container = document.querySelector(
                '.card-actions, .actions, .contact-actions, .btn-row, .buttons, .card-footer, .card-body, .card'
            );
            if (container) {
                return { el: container, position: 'beforeend' };
            }

            // Fallback: append to body
            return { el: document.body, position: 'beforeend' };
        },

        /**
         * Inject the wallet button(s) into the page.
         */
        injectButtons() {
            // Don't inject twice
            if (document.getElementById('bizbio-google-wallet-btn')) return;

            const platform = this.getPlatform();

            // Show Google Wallet on Android and desktop.
            // On iOS, Google Wallet is available but less common — still show it.
            const showGoogle = true; // shown on all platforms (Apple Wallet skipped)

            if (!showGoogle) return;

            const { el, position } = this.findInsertionPoint();
            const googleBtn = this.createGoogleWalletButton();

            if (position === 'afterend') {
                el.insertAdjacentElement('afterend', googleBtn);
            } else {
                el.insertAdjacentElement('beforeend', googleBtn);
            }

            // Inject spinner keyframe if not already present
            if (!document.getElementById('bizbio-wallet-styles')) {
                const style = document.createElement('style');
                style.id = 'bizbio-wallet-styles';
                style.textContent = '@keyframes bizbio-spin { to { transform: rotate(360deg); } }';
                document.head.appendChild(style);
            }
        },

        /**
         * Initialize the wallet integration.
         */
        init() {
            // Card data must exist to proceed
            const name = document.body.getAttribute('data-vcard-fn');
            if (!name) {
                console.warn('[BizBio Wallet] No card data found. Wallet buttons will not be shown.');
                return;
            }

            this.injectButtons();
        },
    };

    // Auto-initialize
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => BizBioWallet.init());
    } else {
        BizBioWallet.init();
    }

    window.BizBioWallet = BizBioWallet;
})();

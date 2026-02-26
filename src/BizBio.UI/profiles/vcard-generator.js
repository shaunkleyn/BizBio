/**
 * BizBio vCard Generator
 * Intercepts "Save Contact" / "Add to Contacts" button clicks and generates
 * a downloadable .vcf file from the data-vcard-* attributes placed on
 * document.body by bizbio-engine.js.
 *
 * Exposed globally as window.BizBioVCard for bizbio-engine.js to call init().
 */

(function () {
    'use strict';

    const BizBioVCard = {

        /**
         * Attach click handlers to all save-contact buttons / vCard links.
         * If none exist in the template, inject a Save Contact button.
         */
        init() {
            const elements = document.querySelectorAll('.save-btn, a[href$=".vcf"]');
            if (elements.length > 0) {
                elements.forEach(el => {
                    el.addEventListener('click', (e) => {
                        e.preventDefault();
                        this.download();
                    });
                });
            } else {
                this.injectSaveButton();
            }
        },

        /**
         * Find the best place to inject the Save Contact button.
         * Prefers inserting after .action-buttons; falls back to .card or body.
         */
        findInsertionPoint() {
            const actionButtons = document.querySelector('.action-buttons');
            if (actionButtons) {
                return { el: actionButtons, position: 'afterend' };
            }
            const container = document.querySelector('.card-body, .card');
            if (container) {
                return { el: container, position: 'beforeend' };
            }
            return { el: document.body, position: 'beforeend' };
        },

        /**
         * Create and inject a "Save Contact" button into the page.
         */
        injectSaveButton() {
            if (document.getElementById('bizbio-save-contact-btn')) return;

            const wrapper = document.createElement('div');
            wrapper.style.cssText = [
                'display: flex',
                'justify-content: center',
                'padding: 4px 20px 20px',
            ].join(';');

            const btn = document.createElement('button');
            btn.id = 'bizbio-save-contact-btn';
            btn.className = 'save-btn';
            btn.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="flex-shrink:0;">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                </svg>
                <span>Save Contact</span>
            `;
            btn.style.cssText = [
                'display: inline-flex',
                'align-items: center',
                'justify-content: center',
                'gap: 8px',
                'width: 100%',
                'max-width: 320px',
                'height: 48px',
                'background: #1c1c1e',
                'color: #ffffff',
                'border: none',
                'border-radius: 50px',
                'font-size: 14px',
                'font-weight: 700',
                'letter-spacing: 0.5px',
                'cursor: pointer',
                'font-family: inherit',
                'box-shadow: 0 4px 14px rgba(0,0,0,0.22)',
                'transition: transform 0.18s ease, filter 0.18s ease',
            ].join(';');

            btn.addEventListener('mouseenter', () => {
                btn.style.transform = 'translateY(-2px)';
                btn.style.filter = 'brightness(1.15)';
            });
            btn.addEventListener('mouseleave', () => {
                btn.style.transform = '';
                btn.style.filter = '';
            });
            btn.addEventListener('click', () => this.download());

            wrapper.appendChild(btn);

            const { el, position } = this.findInsertionPoint();
            el.insertAdjacentElement(position, wrapper);
        },

        /**
         * Read contact data from data-vcard-* body attributes.
         */
        getData() {
            const b = document.body;
            return {
                fn:        b.getAttribute('data-vcard-fn')               || '',
                title:     b.getAttribute('data-vcard-title')            || '',
                org:       b.getAttribute('data-vcard-org')              || '',
                email:     b.getAttribute('data-vcard-email')            || '',
                tel:       b.getAttribute('data-vcard-tel')              || '',
                adr:       b.getAttribute('data-vcard-adr')              || '',
                url:       b.getAttribute('data-vcard-url')              || '',
                note:      b.getAttribute('data-vcard-note')             || '',
                linkedin:  b.getAttribute('data-vcard-social-linkedin')  || '',
                facebook:  b.getAttribute('data-vcard-social-facebook')  || '',
                instagram: b.getAttribute('data-vcard-social-instagram') || '',
                twitter:   b.getAttribute('data-vcard-social-twitter')   || '',
                whatsapp:  b.getAttribute('data-vcard-social-whatsapp')  || '',
            };
        },

        /**
         * Escape special characters for vCard field values.
         */
        esc(v) {
            return v.replace(/\\/g, '\\\\')
                    .replace(/,/g, '\\,')
                    .replace(/;/g, '\\;')
                    .replace(/\r?\n/g, '\\n');
        },

        /**
         * Build a VCF string from the supplied data object.
         */
        buildVCF(d) {
            const lines = ['BEGIN:VCARD', 'VERSION:3.0'];

            if (d.fn) {
                lines.push(`FN:${this.esc(d.fn)}`);
                // N: Last;First;Middle;Prefix;Suffix
                const parts = d.fn.trim().split(/\s+/);
                const last  = parts.length > 1 ? parts.pop() : '';
                const first = parts.join(' ');
                lines.push(`N:${this.esc(last)};${this.esc(first)};;;`);
            }

            if (d.title) lines.push(`TITLE:${this.esc(d.title)}`);
            if (d.org)   lines.push(`ORG:${this.esc(d.org)}`);

            if (d.email) lines.push(`EMAIL;TYPE=INTERNET,PREF:${d.email}`);
            if (d.tel)   lines.push(`TEL;TYPE=CELL,PREF:${d.tel}`);

            // WhatsApp as additional number if different from main tel
            if (d.whatsapp && d.whatsapp !== d.tel) {
                const waNum = d.whatsapp.replace(/\D/g, '');
                lines.push(`TEL;TYPE=MSG:+${waNum}`);
            }

            if (d.adr) lines.push(`ADR;TYPE=WORK:;;${this.esc(d.adr)};;;;`);
            if (d.url) lines.push(`URL:${d.url}`);
            if (d.note) lines.push(`NOTE:${this.esc(d.note)}`);

            if (d.linkedin)  lines.push(`X-SOCIALPROFILE;type=linkedin:${d.linkedin}`);
            if (d.facebook)  lines.push(`X-SOCIALPROFILE;type=facebook:${d.facebook}`);
            if (d.instagram) lines.push(`X-SOCIALPROFILE;type=instagram:${d.instagram}`);
            if (d.twitter)   lines.push(`X-SOCIALPROFILE;type=twitter:${d.twitter}`);

            const rev = new Date().toISOString().replace(/[-:.]/g, '').slice(0, 15) + 'Z';
            lines.push(`REV:${rev}`);
            lines.push('END:VCARD');

            return lines.join('\r\n');
        },

        /**
         * Generate and download the vCard file.
         */
        download() {
            const data = this.getData();

            if (!data.fn) {
                console.warn('[BizBio vCard] No contact data found. Ensure bizbio-engine.js has run.');
                return;
            }

            const vcf      = this.buildVCF(data);
            const filename = data.fn.toLowerCase().replace(/[^a-z0-9]+/g, '_') + '.vcf';
            const blob     = new Blob([vcf], { type: 'text/vcard;charset=utf-8' });
            const url      = URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href     = url;
            a.download = filename;
            a.style.display = 'none';
            document.body.appendChild(a);
            a.click();

            setTimeout(() => {
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }, 150);
        },
    };

    // Auto-init (handles both cases: script loaded before or after DOMContentLoaded)
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => BizBioVCard.init());
    } else {
        BizBioVCard.init();
    }

    window.BizBioVCard = BizBioVCard;

})();

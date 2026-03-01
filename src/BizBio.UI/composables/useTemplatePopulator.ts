import type { WizardState } from './useBusinessCardCreation'

const templateCache: Record<string, string> = {}

// Templates that use bizCard() Alpine component with fetch interceptor
const BIZCARD_TEMPLATES = ['01-minimalist-clean']

// Templates that use window.__bizbioData Alpine data
const BIZBIO_DATA_TEMPLATES = ['35-gradient-card']

async function fetchTemplate(templateName: string): Promise<string> {
  if (templateCache[templateName]) return templateCache[templateName]
  const res = await fetch(`/templates/${templateName}.html`)
  if (!res.ok) throw new Error(`Template not found: ${templateName}`)
  const html = await res.text()
  templateCache[templateName] = html
  return html
}

/**
 * Build the card data JSON in the format expected by the Alpine.js bizCard component.
 * Used only for template 01-minimalist-clean.
 */
function buildCardData(state: WizardState): object {
  const actions: object[] = state.contactButtons
    .filter(b => b.value)
    .map(b => {
      const label = b.label || b.type.charAt(0).toUpperCase() + b.type.slice(1)
      switch (b.type) {
        case 'phone': return { type: 'tel', label, value: b.value }
        case 'email': return { type: 'mailto', label, value: b.value }
        case 'whatsapp': return { type: 'whatsapp', label, value: b.value.replace(/[^\d]/g, '') }
        case 'sms': return { type: 'tel', label, value: b.value }
        default: return { type: 'url', label, url: b.value }
      }
    })

  const socials: object[] = state.socialLinks
    .filter(s => s.url)
    .map(s => ({ url: s.url }))

  const saveContactSection = state.sections.find(s => s.type === 'save-contact')
  const cta = saveContactSection?.data?.enabled !== false
    ? { url: 'contact.vcf', buttonLabel: saveContactSection?.data?.header || 'Save Contact' }
    : null

  return {
    person: {
      firstName: state.firstName || 'Your',
      lastName: state.lastName || 'Name',
      title: state.headline || '',
      company: state.company || '',
      photoUrl: state.photoUrl || '',
      bio: state.bio || '',
    },
    actions,
    socials,
    ...(cta ? { cta } : {}),
  }
}

/**
 * Build injection script for template 35 (window.__bizbioData + window.info + window.social).
 * Template 35 uses Alpine.js with x-data="window.__bizbioData || {}" and window.info/social globals.
 */
function buildTemplate35Injection(state: WizardState): string {
  const name = state.company || `${state.firstName} ${state.lastName}`.trim() || 'Your Name'
  const initials = ((state.firstName?.[0] ?? '') + (state.lastName?.[0] ?? '')).toUpperCase() || 'YN'

  const phoneBtn = state.contactButtons.find(b => b.type === 'phone')
  const emailBtn = state.contactButtons.find(b => b.type === 'email')
  const waBtn = state.contactButtons.find(b => b.type === 'whatsapp')

  const iconMap: Record<string, string> = {
    phone: 'fa-solid fa-phone',
    email: 'fa-solid fa-envelope',
    website: 'fa-solid fa-globe',
    address: 'fa-solid fa-location-dot',
    linkedin: 'fa-brands fa-linkedin',
    whatsapp: 'fa-brands fa-whatsapp',
    birthday: 'fa-solid fa-cake-candles',
    custom: 'fa-solid fa-circle-info',
  }

  const info = state.contactInfo
    .filter(i => i.value)
    .map(i => ({
      icon: iconMap[i.type] ?? 'fa-solid fa-circle-info',
      title: i.label || i.type,
      value: i.value,
      href: i.href || '#',
      button: i.type === 'phone' ? 'Call' : i.type === 'email' ? 'Email' : i.type === 'website' ? 'Visit' : null,
      type: i.type,
    }))

  const socialCssMap: Record<string, { css: string; icon: string }> = {
    linkedin: { css: 'si-linkedin', icon: 'fab fa-linkedin' },
    facebook: { css: 'si-facebook', icon: 'fab fa-facebook-f' },
    instagram: { css: 'si-instagram', icon: 'fab fa-instagram' },
    twitter: { css: 'si-twitter', icon: 'fab fa-x-twitter' },
    x: { css: 'si-twitter', icon: 'fab fa-x-twitter' },
    tiktok: { css: 'si-tiktok', icon: 'fab fa-tiktok' },
    youtube: { css: 'si-youtube', icon: 'fab fa-youtube' },
    github: { css: 'si-github', icon: 'fab fa-github' },
    whatsapp: { css: 'si-whatsapp', icon: 'fab fa-whatsapp' },
    website: { css: 'si-website', icon: 'fas fa-globe' },
    pinterest: { css: 'si-pinterest', icon: 'fab fa-pinterest' },
    discord: { css: 'si-website', icon: 'fab fa-discord' },
    telegram: { css: 'si-website', icon: 'fab fa-telegram' },
  }

  const social = state.socialLinks
    .filter(s => s.url)
    .map(s => {
      const key = s.platform.toLowerCase()
      const mapping = socialCssMap[key] ?? { css: 'si-website', icon: 'fas fa-globe' }
      return { platform: s.platform, url: s.url, ...mapping }
    })

  const __bizbioData = {
    name,
    tagline: state.headline || '',
    photo: state.photoUrl || '',
    bio: state.bio || '',
    initials,
    phoneHref: phoneBtn ? `tel:${phoneBtn.value}` : '',
    emailHref: emailBtn ? `mailto:${emailBtn.value}` : '',
    whatsappHref: waBtn ? `https://wa.me/${waBtn.value.replace(/[^\d]/g, '')}` : '',
    googleReviewUrl: '',
    facebookReviewUrl: '',
  }

  return `<script>
window.__bizbioData = ${JSON.stringify(__bizbioData)};
window.info = ${JSON.stringify(info)};
window.social = ${JSON.stringify(social)};
<\/script>`
}

/**
 * Build a DOM-manipulation injection script for static templates (02-34 except 35).
 * These templates have no Alpine.js data binding — they show hardcoded demo content.
 * This script runs synchronously at end of body (before deferred Alpine) and replaces
 * the hardcoded text/images with wizard state data.
 */
function buildStaticDomInjection(state: WizardState): string {
  const name = `${state.firstName} ${state.lastName}`.trim() || ''
  const headline = state.headline || ''
  const company = state.company || ''
  const photo = state.photoUrl || ''
  const bio = state.bio || ''

  const data = { name, headline, company, photo, bio }

  return `<script>
(function() {
  var d = ${JSON.stringify(data)};

  // Name: try h1.name first, then first h1 in .header, then any h1
  var nameEl = document.querySelector('h1.name')
    || document.querySelector('.header h1')
    || document.querySelector('.hero h1')
    || document.querySelector('h1');
  if (nameEl && d.name) nameEl.textContent = d.name;

  // Headline / Job title
  var titleSelectors = ['.header .title', '.hero .title', '.header .job-title',
    '.title', '.job-title', '.headline', '.designation', '.position'];
  for (var i = 0; i < titleSelectors.length; i++) {
    var el = document.querySelector(titleSelectors[i]);
    if (el && el !== nameEl) { if (d.headline) el.textContent = d.headline; break; }
  }

  // Company
  var companySelectors = ['.header .company', '.hero .company', '.company', '.organization', '.employer'];
  for (var j = 0; j < companySelectors.length; j++) {
    var cel = document.querySelector(companySelectors[j]);
    if (cel && cel !== nameEl) { if (d.company) cel.textContent = d.company; break; }
  }

  // Profile photo
  if (d.photo) {
    var photoEl = document.querySelector('img.avatar')
      || document.querySelector('.avatar-wrapper img')
      || document.querySelector('.avatar-frame img')
      || document.querySelector('.avatar-container img')
      || document.querySelector('img.profile-photo')
      || document.querySelector('.photo-container img')
      || document.querySelector('.profile-image img')
      || document.querySelector('.hero img');
    if (photoEl) photoEl.src = d.photo;
  }

  // Bio text
  if (d.bio) {
    var bioEl = document.querySelector('.bio-text')
      || document.querySelector('.about-text')
      || document.querySelector('.bio p')
      || document.querySelector('.about p')
      || document.querySelector('.description p');
    if (bioEl) bioEl.textContent = d.bio;
  }
})();
<\/script>`
}

/** Compute CSS background value for the current appearance settings. */
function computeBackground(a: WizardState['appearance']): string {
  if (a.bgType === 'gradient') {
    return `linear-gradient(${a.bgGradientDir}, ${a.bgGradientFrom}, ${a.bgGradientTo})`
  }
  if (a.bgType === 'image' && a.bgImageUrl) {
    return `url('${a.bgImageUrl}') center/cover no-repeat`
  }
  if (a.bgType === 'pattern') {
    const pc = a.primaryColor + '30'
    const bg = a.bgColor
    const patterns: Record<string, string> = {
      dots: `radial-gradient(circle, ${pc} 1.5px, transparent 1.5px) 0 0/22px 22px`,
      grid: `linear-gradient(${pc} 1px, transparent 1px), linear-gradient(90deg, ${pc} 1px, transparent 1px) 0 0/22px 22px`,
      diagonal: `repeating-linear-gradient(45deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/22px 22px`,
      cross: `repeating-linear-gradient(0deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/22px 22px, repeating-linear-gradient(90deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/22px 22px`,
      circles: `radial-gradient(circle at 50% 50%, transparent 7px, ${pc} 7.5px, transparent 8px) 0 0/24px 24px`,
      hex: `repeating-linear-gradient(30deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/24px 14px, repeating-linear-gradient(150deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/24px 14px`,
      waves: `repeating-linear-gradient(-45deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 4px) 0 0/8px 8px`,
      confetti: `radial-gradient(circle at 25% 25%, ${pc} 2px, transparent 2px), radial-gradient(circle at 75% 75%, ${pc} 2px, transparent 2px), radial-gradient(circle at 50% 10%, ${pc} 2px, transparent 2px) 0 0/30px 30px`,
    }
    const pat = patterns[a.bgPattern] || patterns.dots
    return `${pat}, ${bg}`
  }
  return a.bgColor
}

/**
 * Compute a CSS background string for pattern swatches in the UI.
 * Exported so AppearanceTab can use it for live swatch previews.
 */
export function getPatternPreviewCss(patternId: string, bgColor: string, primaryColor: string): string {
  const pc = primaryColor + '40'
  const patterns: Record<string, string> = {
    dots: `radial-gradient(circle, ${pc} 1.5px, transparent 1.5px) 0 0/16px 16px`,
    grid: `linear-gradient(${pc} 1px, transparent 1px), linear-gradient(90deg, ${pc} 1px, transparent 1px) 0 0/16px 16px`,
    diagonal: `repeating-linear-gradient(45deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/16px 16px`,
    cross: `repeating-linear-gradient(0deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/16px 16px, repeating-linear-gradient(90deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/16px 16px`,
    circles: `radial-gradient(circle at 50% 50%, transparent 5px, ${pc} 5.5px, transparent 6px) 0 0/16px 16px`,
    hex: `repeating-linear-gradient(30deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/16px 9px, repeating-linear-gradient(150deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 50%) 0 0/16px 9px`,
    waves: `repeating-linear-gradient(-45deg, ${pc} 0, ${pc} 1px, transparent 0, transparent 3px) 0 0/6px 6px`,
    confetti: `radial-gradient(circle at 25% 25%, ${pc} 1.5px, transparent 1.5px), radial-gradient(circle at 75% 75%, ${pc} 1.5px, transparent 1.5px) 0 0/20px 20px`,
  }
  const pat = patterns[patternId] || patterns.dots
  return `${pat}, ${bgColor}`
}

function buildCssOverrides(state: WizardState): string {
  const a = state.appearance
  const bgValue = computeBackground(a)

  const socialColors = `
    --linkedin: #0077b5; --facebook: #1877f2; --instagram: #e4405f;
    --twitter: #1da1f2; --x: #000000; --tiktok: #010101;
    --youtube: #ff0000; --github: #333; --whatsapp: #25d366;
    --telegram: #2ca5e0; --phone: #10b981; --email: #3b82f6;
    --website: #1a1a1a; --url: #1a1a1a;`

  const iconRadius: Record<string, string> = {
    circle: '50%',
    square: '4px',
    rounded: '14px',
    none: '0',
  }

  const btnRadius: Record<string, string> = {
    filled: '10px',
    outlined: '10px',
    ghost: '10px',
    pill: '50px',
  }

  const ir = iconRadius[a.iconStyle] ?? '50%'
  const br = btnRadius[a.buttonStyle] ?? '10px'

  const ctaStyle = a.buttonStyle === 'outlined'
    ? `background: transparent !important; border: 2px solid ${a.primaryColor} !important; color: ${a.primaryColor} !important;`
    : a.buttonStyle === 'ghost'
      ? `background: transparent !important; border: none !important; color: ${a.primaryColor} !important; box-shadow: none !important;`
      : `background: ${a.primaryColor} !important; color: ${a.buttonTextColor} !important;`

  const cardShadow = a.cardShadowEnabled
    ? 'box-shadow: 0 8px 32px rgba(0,0,0,0.12) !important;'
    : 'box-shadow: none !important;'
  const cardBorder = a.cardBorderEnabled
    ? `border: 1px solid ${a.cardBorderColor} !important;`
    : ''

  // Solid bg color (for CSS vars that expect a color, not a gradient)
  const solidBgColor = a.bgType === 'solid' ? a.bgColor : a.bgType === 'gradient' ? a.bgGradientFrom : a.bgColor

  return `
    :root {
      /* ── Core brand colors ──────────────────────────── */
      --primary-color: ${a.primaryColor};
      --accent-color: ${a.primaryColor};
      --secondary-color: ${a.primaryColor};
      --primary: ${a.primaryColor};
      --primary-dark: ${a.primaryColor};

      /* ── Background ─────────────────────────────────── */
      --bg-color: ${solidBgColor};
      --page-background-color: ${bgValue};

      /* ── Card ───────────────────────────────────────── */
      --card-bg: ${a.cardBgColor};
      --card-bg-color: ${a.cardBgColor};
      --surface: ${a.cardBgColor};
      --surface-2: ${a.cardBgColor};
      --card-radius: ${a.cardBorderRadius}px;

      /* ── Typography colors ──────────────────────────── */
      --text-color: ${a.titleColor};
      --text: ${a.titleColor};
      --header-color: ${a.titleColor};
      --subtitle-color: ${a.subtitleColor};
      --tagline-color: ${a.subtitleColor};
      --text-muted: ${a.infoLabelColor};
      --text-light: ${a.bodyTextColor};
      --text-dark: ${a.titleColor};
      --body-text-color: ${a.bodyTextColor};

      /* ── Fonts ──────────────────────────────────────── */
      --heading-font: '${a.headingFont}', sans-serif;
      --body-font: '${a.bodyFont}', sans-serif;
      --font-family: '${a.bodyFont}', sans-serif;

      /* ── Label / info colors ────────────────────────── */
      --label-color: ${a.infoLabelColor};
      --info-item-label-color: ${a.infoLabelColor};
      --info-item-text-color: ${a.bodyTextColor};
      --info-item-icon-bg-color: ${a.primaryColor};
      --info-item-icon-text-color: ${a.buttonTextColor};

      /* ── Buttons ────────────────────────────────────── */
      --action-button-bg-color: ${a.primaryColor};
      --action-button-text-color: ${a.buttonTextColor};
      --cta-button-bg-color: ${a.primaryColor};
      --cta-button-text-color: ${a.buttonTextColor};
      --button-radius: ${br};
      --btn-radius: ${br};
      --icon-radius: ${ir};

      /* ── Misc ───────────────────────────────────────── */
      --avatar-border-color: ${a.cardBgColor};

      /* ── Gradient aliases (templates 33, 34) ─────────── */
      --grad-start: ${a.bgType === 'gradient' ? a.bgGradientFrom : a.primaryColor};
      --grad-end: ${a.bgType === 'gradient' ? a.bgGradientTo : a.primaryColor};
      --grad: ${bgValue};

      ${socialColors}
    }

    /* ── Background ──────────────────────────────── */
    body {
      background: ${bgValue} !important;
      font-family: var(--font-family) !important;
    }
    html { background: ${bgValue} !important; }

    /* ── Typography ──────────────────────────────── */
    h1, h2, h3 { font-family: var(--heading-font) !important; }
    .name, .person-name, .full-name, .profile-name, .card-name,
    h1.name, .header h1, .hero h1 {
      color: ${a.titleColor} !important;
      font-family: var(--heading-font) !important;
    }
    .title, .job-title, .person-title, .headline, .role, .subtitle,
    .card-title, .position, .designation, .tagline {
      color: ${a.subtitleColor} !important;
    }
    .bio, .about, .description, .bio-text, .card-bio, .person-bio,
    .bio-section p, .about p {
      color: ${a.bodyTextColor} !important;
    }
    .label, .info-label, .detail-label, .contact-label,
    .info-item-label, .contact-row-label, .social-title {
      color: ${a.infoLabelColor} !important;
    }

    /* ── Card container ──────────────────────────── */
    .card, .profile-card, .card-container, .card-wrapper,
    .biz-card, .bio-card, .contact-card, .main-card {
      background: ${a.cardBgColor} !important;
      border-radius: ${a.cardBorderRadius}px !important;
      ${cardShadow}
      ${cardBorder}
    }

    /* ── Action icon buttons ──────────────────────── */
    .action-btn, .contact-btn, .icon-btn, .social-btn,
    .quick-action, .info-icon {
      border-radius: ${ir} !important;
      ${a.iconStyle === 'none' ? 'background: transparent !important; box-shadow: none !important;' : `background: ${a.primaryColor} !important; color: ${a.buttonTextColor} !important;`}
    }

    /* ── Action-buttons wrapper ───────────────────── */
    .action-buttons .action-btn, .action-buttons .action-item,
    .action-buttons a, .action-buttons button {
      border-radius: ${ir} !important;
    }

    /* ── CTA / save-contact button ────────────────── */
    .cta-btn, .cta-button, .save-contact-btn, .save-btn,
    .btn-cta, .btn-primary, .download-btn, .vcard-btn,
    a[href$=".vcf"], button[data-cta], .save-contact {
      ${ctaStyle}
      border-radius: ${br} !important;
    }

    /* ── Footer / CTA area ───────────────────────── */
    .footer { padding: 0 20px 20px; }
    .footer .cta-btn, .footer .save-btn, .footer a, .footer button {
      ${ctaStyle}
      border-radius: ${br} !important;
    }

    /* ── Body content area ───────────────────────── */
    .body, .card .body {
      font-family: var(--body-font);
    }

    /* ── Extra sections injected below template ───── */
    #__biz-sections__ {
      font-family: var(--body-font);
    }
    #__biz-sections__ h3 {
      font-family: var(--heading-font);
      color: ${a.titleColor};
    }
  `
}

/** Build HTML for optional sections to inject below the template content. */
function buildSectionsHtml(state: WizardState): string {
  const a = state.appearance
  const sections = [...state.sections]
    .sort((s1, s2) => s1.sortOrder - s2.sortOrder)
    .filter(s => {
      if (s.type === 'save-contact' || s.type === 'share') return s.data.enabled !== false
      if (s.type === 'bio') return !!s.data.text?.trim()
      if (s.type === 'skills') return (s.data.skills?.length ?? 0) > 0
      if (s.type === 'links') return (s.data.links?.filter((l: any) => l.url)?.length ?? 0) > 0
      if (s.type === 'map') return !!s.data.address?.trim()
      if (s.type === 'gallery') return (s.data.images?.length ?? 0) > 0
      return false
    })

  if (sections.length === 0) return ''

  const headerHtml = (s: typeof sections[number]) =>
    s.data.header?.trim()
      ? `<h3 style="margin:0 0 12px;font-size:14px;font-weight:700;letter-spacing:0.02em;color:${a.titleColor};">${s.data.header}</h3>`
      : ''

  const parts = sections.map(s => {
    switch (s.type) {
      case 'bio':
        return `<div class="biz-s">
          ${headerHtml(s)}
          <p style="margin:0;font-size:13px;line-height:1.65;color:${a.bodyTextColor};">${s.data.text}</p>
        </div>`

      case 'skills': {
        const tags = (s.data.skills as string[])
          .map(skill => `<span style="display:inline-block;padding:4px 12px;margin:3px;border-radius:20px;background:${a.primaryColor}22;color:${a.primaryColor};font-size:12px;font-weight:500;">${skill}</span>`)
          .join('')
        return `<div class="biz-s">
          ${headerHtml(s)}
          <div style="margin:-3px;">${tags}</div>
        </div>`
      }

      case 'links': {
        const items = (s.data.links as Array<{ title: string; url: string }>)
          .filter(l => l.url)
          .map(l => `<a href="${l.url}" style="display:flex;align-items:center;gap:10px;padding:10px 14px;margin-bottom:8px;border-radius:10px;background:${a.cardBgColor};border:1px solid rgba(0,0,0,0.07);text-decoration:none;color:${a.titleColor};font-size:13px;">
            <i class="fas fa-external-link-alt" style="color:${a.primaryColor};font-size:11px;flex-shrink:0;"></i>
            <span>${l.title || l.url}</span>
          </a>`)
          .join('')
        return `<div class="biz-s">${headerHtml(s)}${items}</div>`
      }

      case 'map': {
        const q = encodeURIComponent(s.data.address)
        return `<div class="biz-s">
          ${headerHtml(s)}
          <iframe src="https://maps.google.com/maps?q=${q}&output=embed" width="100%" height="200"
            style="border:0;border-radius:12px;display:block;" loading="lazy"></iframe>
        </div>`
      }

      case 'save-contact':
        return `<div class="biz-s">
          <a href="contact.vcf" style="display:flex;align-items:center;justify-content:center;gap:8px;width:100%;padding:13px;border-radius:12px;background:${a.primaryColor};color:${a.buttonTextColor};text-decoration:none;font-size:14px;font-weight:600;">
            <i class="fas fa-address-book"></i>
            ${s.data.header || 'Save Contact'}
          </a>
        </div>`

      case 'share':
        return `<div class="biz-s">
          <button onclick="if(navigator.share){navigator.share({url:location.href})}else{navigator.clipboard&&navigator.clipboard.writeText(location.href)}"
            style="display:flex;align-items:center;justify-content:center;gap:8px;width:100%;padding:13px;border-radius:12px;background:transparent;border:2px solid ${a.primaryColor};color:${a.primaryColor};font-size:14px;font-weight:600;cursor:pointer;">
            <i class="fas fa-share-alt"></i>
            ${s.data.header || 'Share Profile'}
          </button>
        </div>`

      default:
        return ''
    }
  }).filter(Boolean)

  if (parts.length === 0) return ''

  return `<div id="__biz-sections__" style="padding:16px 20px 24px;display:flex;flex-direction:column;gap:16px;">${parts.join('\n')}</div>`
}

export const useTemplatePopulator = () => {
  const buildSrcdoc = async (state: WizardState): Promise<string> => {
    const html = await fetchTemplate(state.selectedTemplate)
    const cssOverrides = buildCssOverrides(state)

    const origin = typeof window !== 'undefined' ? window.location.origin : ''
    const baseTag = `<base href="${origin}/templates/">`

    const a = state.appearance
    // Load fonts from Google Fonts — include both heading and body fonts
    const fontsToLoad = [...new Set([a.headingFont, a.bodyFont])]
      .filter(f => !['Segoe UI', 'system-ui', '-apple-system'].some(s => f.includes(s)))
    const fontQuery = fontsToLoad
      .map(f => `family=${encodeURIComponent(f)}:wght@400;500;600;700`)
      .join('&')
    const fontStyle = fontQuery
      ? `<link rel="stylesheet" href="https://fonts.googleapis.com/css2?${fontQuery}&display=swap">`
      : ''

    const overrideStyle = `<style>${cssOverrides}</style>`
    const sectionsHtml = buildSectionsHtml(state)

    let headInjection = `\n${baseTag}\n${fontStyle}`
    let bodyInjection = ''

    if (BIZCARD_TEMPLATES.includes(state.selectedTemplate)) {
      // Template 01: uses bizCard() fetch interceptor
      const cardData = buildCardData(state)
      const fetchInterceptor = `
<script>
(function() {
  var __d = ${JSON.stringify(cardData)};
  var _f = window.fetch;
  window.fetch = function(url, opts) {
    var u = String(url);
    if (u.includes('template.json') || u.includes('data.json') || (u.includes('/data/') && u.endsWith('.json'))) {
      return Promise.resolve(new Response(JSON.stringify(__d), {
        status: 200,
        headers: {'Content-Type': 'application/json'}
      }));
    }
    return _f.apply(this, arguments);
  };
})();
<\\/script>`
      headInjection += `\n${fetchInterceptor}`
    } else if (BIZBIO_DATA_TEMPLATES.includes(state.selectedTemplate)) {
      // Template 35: uses window.__bizbioData + window.info + window.social
      headInjection += `\n${buildTemplate35Injection(state)}`
    } else {
      // All other static templates (02-34 except 35): DOM injection
      bodyInjection = buildStaticDomInjection(state)
    }

    // Inject sections via DOM script so they land inside the card's content container,
    // not appended after the card. We try a chain of selectors used across all 35 templates.
    const sectionsScript = sectionsHtml
      ? `<script>(function() {
  var html = ${JSON.stringify(sectionsHtml)};
  var c = document.querySelector('.card > .body')
    || document.querySelector('.card .body')
    || document.querySelector('.card');
  if (c) {
    var t = document.createElement('div');
    t.innerHTML = html;
    while (t.firstChild) c.appendChild(t.firstChild);
  }
})();<\/script>`
      : ''

    const result = html
      .replace('<head>', `<head>${headInjection}`)
      .replace('</head>', `${overrideStyle}\n</head>`)
      .replace('</body>', `${bodyInjection}\n${sectionsScript}\n</body>`)

    return result
  }

  const clearCache = () => {
    Object.keys(templateCache).forEach(k => delete templateCache[k])
  }

  return { buildSrcdoc, clearCache }
}

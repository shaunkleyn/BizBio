import { defineEventHandler, getRequestHeader, getRequestURL, sendRedirect } from 'h3'
import { stat } from 'fs/promises'
import { join } from 'path'

// Marketing / landing pages that stay on the root domain
const LANDING_EXACT = new Set([
  '/', '/pricing', '/categories', '/help', '/contact',
  '/terms', '/privacy', '/search', '/products',
])

// App-area paths that redirect to app.snaptap.co.za
const APP_PREFIXES = [
  '/login', '/register', '/forgot-password', '/reset-password',
  '/verify-email', '/dashboard', '/menu',
]

// Nuxt / system internals — never intercept
const INTERNAL_PREFIXES = [
  '/_nuxt', '/api/', '/__nuxt', '/@', '/favicon',
  '/robots', '/sitemap', '/templates', '/404', '/500',
]

// Check whether a slug has a static profile directory on disk.
// Static profiles are business cards and are served directly at snaptap.co.za/{slug}.
async function isStaticCardSlug(slug: string): Promise<boolean> {
  try {
    const s = await stat(join(process.cwd(), 'profiles', slug))
    return s.isDirectory()
  } catch {
    return false
  }
}

// Check whether a slug belongs to an entity (menu / restaurant / catalog).
async function isEntitySlug(slug: string, apiUrl: string): Promise<boolean> {
  try {
    await $fetch(`${apiUrl}/entities/by-slug/${slug}`, {
      method: 'GET',
      timeout: 3000,
    })
    return true
  } catch {
    return false
  }
}

export default defineEventHandler(async (event) => {
  const rawHost = getRequestHeader(event, 'host') ?? ''
  const host    = rawHost.split(':')[0].toLowerCase()
  const url     = getRequestURL(event)
  const path    = url.pathname
  const qs      = url.search ?? ''

  // Never intercept Nuxt internals or templates (served as public assets)
  if (INTERNAL_PREFIXES.some(p => path.startsWith(p))) return

  // ── www.snaptap.co.za → canonical bare domain ──────────────────────────
  if (host === 'www.snaptap.co.za') {
    return sendRedirect(event, `https://snaptap.co.za${path}${qs}`, 301)
  }

  // ── cards.snaptap.co.za/{slug} → snaptap.co.za/{slug} (canonical) ──────
  // Profiles are canonical at the root domain; cards subdomain is an alias.
  if (host === 'cards.snaptap.co.za') {
    return sendRedirect(event, `https://snaptap.co.za${path}${qs}`, 301)
  }

  // ── All other subdomains (app, menu) pass through unchanged ─────────────
  if (host !== 'snaptap.co.za') return

  // ── Root domain routing logic ────────────────────────────────────────────

  // Known landing pages — serve as-is
  const normalised = path.replace(/\/$/, '') || '/'
  if (LANDING_EXACT.has(path) || LANDING_EXACT.has(normalised)) return

  // App paths → app.snaptap.co.za
  if (APP_PREFIXES.some(p => path === p || path.startsWith(`${p}/`))) {
    return sendRedirect(event, `https://app.snaptap.co.za${path}${qs}`, 301)
  }

  // Potential slug — must have at least one non-empty path segment
  const segments = path.split('/').filter(Boolean)
  if (segments.length === 0) return

  const slug = segments[0]

  // Static card profiles are served directly at snaptap.co.za/{slug}.
  // Fall through so profiles.ts middleware can serve the file from disk.
  if (await isStaticCardSlug(slug)) return

  // Entity slugs (menus / restaurants) redirect to the menu subdomain.
  const { public: pub } = useRuntimeConfig()
  if (await isEntitySlug(slug, pub.apiUrl)) {
    return sendRedirect(event, `https://menu.snaptap.co.za${path}${qs}`, 301)
  }

  // Unknown slug — fall through (Nuxt will 404 naturally)
})

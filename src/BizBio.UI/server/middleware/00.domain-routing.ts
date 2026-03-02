import { defineEventHandler, getRequestHeader, getRequestURL, sendRedirect } from 'h3'
import { stat } from 'fs/promises'
import { join } from 'path'

const API_URL = process.env.NUXT_PUBLIC_API_URL || 'https://api.bizbio.co.za/api/v1'
const PROFILES_DIR = process.env.PROFILES_DIR || join(process.cwd(), 'profiles')

// Root domains that serve profiles directly at /{slug} with no redirect.
// Add additional domains here as the brand expands.
const PROFILE_ROOT_DOMAINS = new Set(['snaptap.co.za', 'snaptap.me'])

// www subdomain → canonical bare domain
const WWW_TO_CANONICAL: Record<string, string> = {
  'www.snaptap.co.za': 'snaptap.co.za',
  'www.snaptap.me':    'snaptap.me',
}

// cards. subdomain → redirect back to the canonical root domain
const CARDS_TO_CANONICAL: Record<string, string> = {
  'cards.snaptap.co.za': 'snaptap.co.za',
  'cards.snaptap.me':    'snaptap.me',
}

// Marketing / landing pages — served as-is on the root domain
const LANDING_EXACT = new Set([
  '/', '/pricing', '/categories', '/help', '/contact',
  '/terms', '/privacy', '/search', '/products',
])

// App-area paths → redirect to app.snaptap.co.za (the single app home)
const APP_PREFIXES = [
  '/login', '/register', '/forgot-password', '/reset-password',
  '/verify-email', '/dashboard', '/menu',
]

// Nuxt / system internals — never intercept
const INTERNAL_PREFIXES = [
  '/_nuxt', '/api/', '/__nuxt', '/@', '/favicon',
  '/robots', '/sitemap', '/templates', '/404', '/500',
]

async function isStaticCardSlug(slug: string): Promise<boolean> {
  try {
    const s = await stat(join(PROFILES_DIR, slug))
    return s.isDirectory()
  } catch {
    return false
  }
}

async function isEntitySlug(slug: string, apiUrl: string): Promise<boolean> {
  try {
    await $fetch(`${apiUrl}/entities/by-slug/${slug}`, { method: 'GET', timeout: 3000 })
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

  // ── www.* → canonical bare domain ──────────────────────────────────────
  const canonical = WWW_TO_CANONICAL[host]
  if (canonical) {
    return sendRedirect(event, `https://${canonical}${path}${qs}`, 301)
  }

  // ── cards.* → canonical root domain (profiles live at root) ────────────
  const cardsCanonical = CARDS_TO_CANONICAL[host]
  if (cardsCanonical) {
    return sendRedirect(event, `https://${cardsCanonical}${path}${qs}`, 301)
  }

  // ── All other subdomains (app.*, menu.*) pass through unchanged ─────────
  if (!PROFILE_ROOT_DOMAINS.has(host)) return

  // ── Root domain routing logic (snaptap.co.za, snaptap.me) ──────────────

  // Known landing pages — serve as-is
  const normalized = path.replace(/\/$/, '') || '/'
  if (LANDING_EXACT.has(path) || LANDING_EXACT.has(normalized)) return

  // App paths → app.snaptap.co.za (single app entry point regardless of brand domain)
  if (APP_PREFIXES.some(p => path === p || path.startsWith(`${p}/`))) {
    return sendRedirect(event, `https://app.snaptap.co.za${path}${qs}`, 301)
  }

  // Potential slug — needs at least one non-empty path segment
  const segments = path.split('/').filter(Boolean)
  if (segments.length === 0) return

  const slug = segments[0]

  // Static card profiles are served directly at {domain}/{slug}.
  // Fall through so profiles.ts serves the file from disk.
  if (await isStaticCardSlug(slug)) return

  // Entity slugs (menus / restaurants) → menu.snaptap.co.za
  if (await isEntitySlug(slug, API_URL)) {
    return sendRedirect(event, `https://menu.snaptap.co.za${path}${qs}`, 301)
  }

  // Unknown slug — fall through (Nuxt will 404 naturally)
})

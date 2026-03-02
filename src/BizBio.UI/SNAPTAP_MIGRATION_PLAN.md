# Subdomain Architecture: BizBio → SnapTap Migration

## Context

BizBio is rebranding to SnapTap.

**Current goal (Phase A):** Get `snaptap.co.za` and `snaptap.me` working 100% — landing page, app, profiles, menus — before touching the existing BizBio setup.

**Later (Phase B, deferred):** Redirect all `bizbio.co.za/*` URLs → equivalent `snaptap.co.za/*` URLs via 301.

Key constraints:
- Two separate Nuxt instances during transition: `:3000` (existing BizBio, untouched) and `:3001` (new SnapTap app)
- SnapTap domains (`snaptap.co.za`, `snaptap.me`) proxy to `:3001`
- Once SnapTap is verified on `:3001`, the `:3000` BizBio instance is replaced with the SnapTap build and Nginx is switched over
- Nginx + Nuxt server middleware handle routing via the `Host` header

---

## Architecture Overview

```
Browser request               Nginx                 Destination
──────────────────────────    ──────────────────    ───────────────────────────────────

── PHASE A (current goal) ──────────────────────────────────────────────────────────

www.snaptap.co.za/*       →  Nuxt MW 301        →  https://snaptap.co.za/* (canonical)
www.snaptap.me/*          →  Nuxt MW 301        →  https://snaptap.me/* (canonical)
cards.snaptap.co.za/*     →  Nuxt MW 301        →  https://snaptap.co.za/* (alias)
cards.snaptap.me/*        →  Nuxt MW 301        →  https://snaptap.me/* (alias)

snaptap.co.za/            →  proxy :3001        →  Landing page (index.vue)
snaptap.co.za/login       →  proxy :3001        →  Nuxt MW → 301 app.snaptap.co.za/login
snaptap.co.za/{slug}      →  proxy :3001        →  Nuxt MW: card → serve directly
                                                             menu → 301 menu.snaptap.co.za

snaptap.me/               →  proxy :3001        →  Landing page (same app)
snaptap.me/{slug}         →  proxy :3001        →  Nuxt MW: card → serve directly
                                                             menu → 301 menu.snaptap.co.za

app.snaptap.co.za/*       →  proxy :3001        →  Full app (dashboard, login, register…)
menu.snaptap.co.za/{slug} →  proxy :3001        →  [entitySlug] Vue page → catalog viewer

bizbio.co.za/*            →  proxy :3000        →  Existing BizBio app (UNCHANGED for now)

── PHASE B (deferred — after SnapTap verified) ─────────────────────────────────────

bizbio.co.za/*            →  301                →  https://snaptap.co.za/*
www.bizbio.co.za/*        →  301                →  https://snaptap.co.za/*
app.bizbio.co.za/*        →  301                →  https://app.snaptap.co.za/*
cards.bizbio.co.za/*      →  301                →  https://cards.snaptap.co.za/*
menu.bizbio.co.za/*       →  301                →  https://menu.snaptap.co.za/*
```

**Cutover path:** Once snaptap.* is 100% verified on `:3001`, update Nginx to proxy `:3000`, rebuild the BizBio `:3000` instance with the SnapTap codebase, then implement the Phase B redirects.

---

## Phase 1 — Cloudflare DNS

Add the following DNS records in Cloudflare. All records must be **orange cloud (proxied)**.

### snaptap.co.za

| Type | Name  | Content        | Proxy  | TTL  |
|------|-------|----------------|--------|------|
| A    | @     | 154.66.198.20 | On     | Auto |
| A    | www   | 154.66.198.20 | On     | Auto |
| A    | app   | 154.66.198.20 | On     | Auto |
| A    | cards | 154.66.198.20 | On     | Auto |
| A    | menu  | 154.66.198.20 | On     | Auto |

### snaptap.me

| Type | Name  | Content       | Proxy | TTL  |
|------|-------|---------------|-------|------|
| A    | @     | 154.66.198.20 | On    | Auto |
| A    | www   | 154.66.198.20 | On    | Auto |
| A    | cards | 154.66.198.20 | On    | Auto |
| A    | menu  | 154.66.198.20 | On    | Auto |

> **bizbio.co.za DNS** — deferred to Phase B (after SnapTap is verified). bizbio.co.za DNS records remain as-is for now.

---

## Phase 2 — SSL Certificates (Let's Encrypt via certbot --nginx)

Use `certbot --nginx` to issue standard Let's Encrypt certificates.

> **Cloudflare note:** HTTP-01 challenge (used by `certbot --nginx`) requires that port 80 is reachable directly. If Cloudflare proxying (orange cloud) is enabled for the domains during cert issuance, temporarily set them to **DNS only (grey cloud)** in Cloudflare, issue the certs, then re-enable orange cloud. Alternatively, run certbot while the DNS is still grey-cloud and switch to orange cloud after.

### 2.1 Install certbot and Nginx plugin

```bash
sudo apt install certbot python3-certbot-nginx
```

### 2.2 Issue certs

Issue a single cert covering all snaptap.co.za subdomains:

```bash
sudo certbot certonly --nginx \
  -d snaptap.co.za \
  -d www.snaptap.co.za \
  -d app.snaptap.co.za \
  -d cards.snaptap.co.za \
  -d menu.snaptap.co.za
```

Then snaptap.me:

```bash
sudo certbot certonly --nginx \
  -d snaptap.me \
  -d www.snaptap.me \
  -d cards.snaptap.me \
  -d menu.snaptap.me
```

Certificates are stored at:
- `/etc/letsencrypt/live/snaptap.co.za/{fullchain,privkey}.pem`
- `/etc/letsencrypt/live/snaptap.me/{fullchain,privkey}.pem`

### 2.3 Auto-renewal

```bash
sudo certbot renew --dry-run
sudo systemctl enable certbot.timer
sudo systemctl start certbot.timer
```

> **bizbio.co.za cert** — deferred to Phase B.

---

## Phase 3 — Nginx Configuration

Create config files for the SnapTap domains only. The bizbio-redirects config is deferred to Phase B.

### 3.1 SSL params snippet

Create `/etc/nginx/snippets/ssl-params.conf` if not already present:

```nginx
ssl_protocols TLSv1.2 TLSv1.3;
ssl_prefer_server_ciphers off;
ssl_session_cache shared:SSL:10m;
ssl_session_timeout 1d;
add_header Strict-Transport-Security "max-age=63072000" always;
```

### 3.2 `/etc/nginx/sites-available/snaptap`

All SnapTap traffic proxied to the new SnapTap Nuxt instance on **port 3001**. The `Host` header must be forwarded so the Nuxt middleware can detect which subdomain is active.

> **Transition note:** Once `snaptap.co.za` is verified working on `:3001`, change `proxy_pass` to `http://127.0.0.1:3000` and reload Nginx. Then shut down the `:3001` PM2 process.

```nginx
# ─────────────────────────────────────────────────────────────
#  snaptap.co.za — Primary brand. All subdomains proxy to
#  the NEW SnapTap Nuxt app on port 3001.
#  (Change to :3000 after cutover verified)
# ─────────────────────────────────────────────────────────────

# HTTP → HTTPS
server {
    listen 80;
    server_name snaptap.co.za www.snaptap.co.za
                app.snaptap.co.za cards.snaptap.co.za menu.snaptap.co.za;
    return 301 https://$host$request_uri;
}

# www.snaptap.co.za → snaptap.co.za (canonical)
server {
    listen 443 ssl;
    server_name www.snaptap.co.za;

    ssl_certificate     /etc/letsencrypt/live/snaptap.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/snaptap.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;

    return 301 https://snaptap.co.za$request_uri;
}

# All live SnapTap subdomains → Nuxt app
server {
    listen 443 ssl;
    server_name snaptap.co.za
                app.snaptap.co.za
                cards.snaptap.co.za
                menu.snaptap.co.za;

    ssl_certificate     /etc/letsencrypt/live/snaptap.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/snaptap.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;

    proxy_read_timeout 60s;
    proxy_send_timeout 60s;

    location / {
        proxy_pass         http://127.0.0.1:3001;
        proxy_http_version 1.1;

        # Critical: pass Host so Nuxt MW can detect subdomain
        proxy_set_header Host              $host;
        proxy_set_header X-Real-IP         $remote_addr;
        proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        # WebSocket (Nuxt HMR in dev, harmless in prod)
        proxy_set_header Upgrade    $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_cache_bypass          $http_upgrade;
    }
}
```

### 3.3 `/etc/nginx/sites-available/snaptap-me`

`snaptap.me` serves profiles at the root domain, same as `snaptap.co.za`. All traffic proxies to the same SnapTap Nuxt instance on **port 3001** — the domain-routing middleware distinguishes behaviour by host header.

```nginx
# ─────────────────────────────────────────────────────────────
#  snaptap.me — Second profile-serving domain.
#  Profiles live at snaptap.me/{slug} (same app, different domain).
# ─────────────────────────────────────────────────────────────

server {
    listen 80;
    server_name snaptap.me www.snaptap.me cards.snaptap.me menu.snaptap.me;
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl;
    server_name snaptap.me www.snaptap.me cards.snaptap.me menu.snaptap.me;

    ssl_certificate     /etc/letsencrypt/live/snaptap.me/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/snaptap.me/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;

    proxy_read_timeout 60s;
    proxy_send_timeout 60s;

    location / {
        proxy_pass         http://127.0.0.1:3001;
        proxy_http_version 1.1;
        proxy_set_header Host              $host;
        proxy_set_header X-Real-IP         $remote_addr;
        proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Upgrade    $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_cache_bypass          $http_upgrade;
    }
}
```

### 3.4 Enable and reload

```bash
sudo ln -s /etc/nginx/sites-available/snaptap     /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/snaptap-me  /etc/nginx/sites-enabled/

sudo nginx -t && sudo systemctl reload nginx
```

---

## Phase 4 — Nuxt Domain-Routing Middleware

**File:** `server/middleware/00.domain-routing.ts` — **COMPLETE**

The `00.` prefix guarantees this runs **before** `profiles.ts` (Nitro sorts middleware alphabetically).

Key behaviours:
- `www.*` → 301 canonical bare domain
- `cards.*` → 301 canonical root domain
- App paths (`/login`, `/register`, `/dashboard`, etc.) on root domain → 301 `app.snaptap.co.za`
- Static card slugs → fall through to `profiles.ts` (served from disk)
- Entity slugs (menus) → 301 `menu.snaptap.co.za`
- `/templates` in `INTERNAL_PREFIXES` so Nitro's `publicAssets` handler serves templates at `/templates/*`

---

## Phase 5 — Landing Page CTA Links

**File:** `pages/index.vue` — **COMPLETE**

Primary CTA buttons use absolute URLs to avoid the middleware redirect round-trip:

```html
<a href="https://app.snaptap.co.za/register">Get Started Free</a>
```

---

## Phase 6 — SEO

**robots.txt** — **COMPLETE** (`public/robots.txt`)

```
User-agent: *
Allow: /

Sitemap: https://snaptap.co.za/sitemap.xml
```

**Canonical tags** — **COMPLETE**

Added to `pages/[entitySlug]/[catalogSlug].vue`:
```typescript
link: [
  { rel: 'canonical', href: `https://menu.snaptap.co.za/${route.params.entitySlug}/${route.params.catalogSlug}` }
]
```

**vcard-generator.js branding** — **COMPLETE**

```javascript
text: 'Here are my contact details. Get yours on SnapTap.co.za today!',
```

### Google Search Console (manual)
1. Add `snaptap.co.za` as a Domain property
2. Add `cards.snaptap.co.za` and `menu.snaptap.co.za` as URL-prefix properties
3. After adding, request indexing on a few key pages to accelerate the transition
4. Monitor the old `bizbio.co.za` property — expect 301 coverage within a few weeks

---

## Phase 7 — Deploy

### 7.1 — SSL Certificates (certbot --nginx)

```bash
ssh user@154.66.198.20

sudo apt install certbot python3-certbot-nginx

# If Cloudflare proxying is on (orange cloud), temporarily set domains to
# "DNS only" (grey cloud) in Cloudflare dashboard, then:

sudo certbot certonly --nginx \
  -d snaptap.co.za -d www.snaptap.co.za \
  -d app.snaptap.co.za -d cards.snaptap.co.za -d menu.snaptap.co.za

sudo certbot certonly --nginx \
  -d snaptap.me -d www.snaptap.me \
  -d cards.snaptap.me -d menu.snaptap.me

# Re-enable orange cloud in Cloudflare after certs are issued.
sudo systemctl enable certbot.timer && sudo systemctl start certbot.timer
```

### 7.2 — Nginx

```bash
sudo nano /etc/nginx/snippets/ssl-params.conf   # paste snippet (3.1)
sudo nano /etc/nginx/sites-available/snaptap    # paste config (3.2) — proxies :3001
sudo nano /etc/nginx/sites-available/snaptap-me # paste config (3.3) — proxies :3001
sudo ln -s /etc/nginx/sites-available/snaptap     /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/snaptap-me  /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
```

### 7.3 — Deploy SnapTap Nuxt app on port 3001

The VPS deployment layout uses a `current/` release directory:

```
/var/www/snaptap/ui/
  ecosystem.config.cjs
  current/              ← symlink (or directory) pointing to latest release
    server/
      index.mjs         ← Nitro entry point
    public/
    nitro.json
```

```bash
# Create directory structure
mkdir -p /var/www/snaptap/ui

# Clone / pull the repo into a release directory and link as current
cd /var/www/snaptap/ui
git clone https://github.com/YOUR_ORG/bizbio-ui.git releases/$(date +%Y%m%d%H%M%S)
cd releases/$(ls -t releases | head -1)
git checkout feature/dynamic-templates-in-vue
npm install
npm run build

# Link the built output as current
cd /var/www/snaptap/ui
ln -sfn releases/$(ls -t releases | head -1)/.output current

# Copy ecosystem config into place (if not already present)
cp releases/$(ls -t releases | head -1)/ecosystem.config.cjs .

# Start with PM2
pm2 start ecosystem.config.cjs --only snaptap-frontend --env production
pm2 save
pm2 status
pm2 logs snaptap-frontend --lines 30
```

### 7.4 — Cutover (after verifying :3001)

Once snaptap.co.za and snaptap.me are working correctly on `:3001`:

```bash
# Update Nginx configs: change proxy_pass from :3001 to :3000
sudo sed -i 's/127.0.0.1:3001/127.0.0.1:3000/g' /etc/nginx/sites-available/snaptap
sudo sed -i 's/127.0.0.1:3001/127.0.0.1:3000/g' /etc/nginx/sites-available/snaptap-me
sudo nginx -t && sudo systemctl reload nginx

# Rebuild the BizBio :3000 instance with the SnapTap changes
cd /var/www/bizbio/ui
git pull
npm install
npm run build
pm2 reload ecosystem.config.cjs --env production

# Stop the temporary :3001 instance
pm2 delete snaptap-frontend
pm2 save
```

---

## CORS — BizBio API

`api.bizbio.co.za` must allow cross-origin requests from the new SnapTap domains. Add to the API's CORS allowed origins:

```
https://snaptap.co.za
https://app.snaptap.co.za
https://menu.snaptap.co.za
https://snaptap.me
```

> Note: SSR `$fetch` calls run server-side and are not affected by CORS. Only browser-side requests need the header.

---

## Verification Checklist (Phase A — SnapTap only)

```bash
# SnapTap canonical redirect
curl -sI https://www.snaptap.co.za        | grep -E "^HTTP|Location"  # → snaptap.co.za

# SnapTap landing (must be 200)
curl -sI https://snaptap.co.za            | grep "^HTTP"

# SnapTap app-path redirect (must be 301 → app.snaptap.co.za)
curl -sI https://snaptap.co.za/login      | grep -E "^HTTP|Location"
curl -sI https://snaptap.co.za/register   | grep -E "^HTTP|Location"

# Slug redirects (replace with real slugs from your data)
curl -sI https://snaptap.co.za/{card-slug}  | grep -E "^HTTP|Location"  # → 200 (served directly)
curl -sI https://snaptap.co.za/{menu-slug}  | grep -E "^HTTP|Location"  # → 301 menu.snaptap.co.za

# Subdomain content (must serve correctly)
curl -sI https://app.snaptap.co.za/login           | grep "^HTTP"
curl -sI https://cards.snaptap.co.za/{card-slug}/  | grep "^HTTP"
curl -sI https://menu.snaptap.co.za/{menu-slug}    | grep "^HTTP"

# SSL cert check
echo | openssl s_client -connect app.snaptap.co.za:443 -servername app.snaptap.co.za 2>/dev/null \
  | openssl x509 -noout -subject -dates
```

---

## Files Status Summary

### Nuxt Repo (all on branch `feature/dynamic-templates-in-vue`)

| File | Status | Notes |
|------|--------|-------|
| `server/middleware/00.domain-routing.ts` | ✅ Done | Domain routing middleware |
| `server/middleware/profiles.ts` | ✅ Done | `/templates` in SKIP_PREFIXES |
| `nuxt.config.ts` | ✅ Done | publicAssets for templates |
| `pages/index.vue` | ✅ Done | CTA links → absolute `app.snaptap.co.za` URLs |
| `profiles/vcard-generator.js` | ✅ Done | Branding text → SnapTap.co.za |
| `public/robots.txt` | ✅ Done | Points to snaptap.co.za/sitemap.xml |
| `pages/[entitySlug]/[catalogSlug].vue` | ✅ Done | Canonical tag → menu.snaptap.co.za |
| `ecosystem.config.cjs` | ✅ Done | Both entries use `./current/server/index.mjs`; `snaptap-frontend` cwd → `/var/www/snaptap/ui`; `bizbio-frontend` explicit cwd → `/var/www/bizbio/ui` |

### VPS (require SSH access)

| Task | Status | Notes |
|------|--------|-------|
| Cloudflare DNS records | ⏳ Pending | Phase 1 — manual in Cloudflare dashboard |
| SSL certs (certbot) | ⏳ Pending | Phase 2 — run on VPS |
| `/etc/nginx/snippets/ssl-params.conf` | ⏳ Pending | Phase 3.1 |
| `/etc/nginx/sites-available/snaptap` | ⏳ Pending | Phase 3.2 — proxy to `:3001` |
| `/etc/nginx/sites-available/snaptap-me` | ⏳ Pending | Phase 3.3 — proxy to `:3001` |
| Deploy snaptap-ui clone on :3001 | ⏳ Pending | Phase 7.3 |
| BizBio API CORS update | ⏳ Pending | Add snaptap.* origins |

---

## Phase B — bizbio.co.za Redirects (Deferred)

**Trigger:** After `snaptap.co.za` and `snaptap.me` are fully verified.

### B.1 SSL cert for bizbio.co.za

```bash
sudo certbot certonly --nginx \
  -d bizbio.co.za -d www.bizbio.co.za \
  -d app.bizbio.co.za -d cards.bizbio.co.za -d menu.bizbio.co.za
```

### B.2 `/etc/nginx/sites-available/bizbio-redirects`

```nginx
# HTTP → HTTPS
server {
    listen 80;
    server_name bizbio.co.za www.bizbio.co.za
                app.bizbio.co.za cards.bizbio.co.za menu.bizbio.co.za;
    return 301 https://$host$request_uri;
}

# bizbio.co.za (root + www) → snaptap.co.za
server {
    listen 443 ssl;
    server_name bizbio.co.za www.bizbio.co.za;
    ssl_certificate     /etc/letsencrypt/live/bizbio.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/bizbio.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;
    return 301 https://snaptap.co.za$request_uri;
}

# app.bizbio.co.za → app.snaptap.co.za
server {
    listen 443 ssl;
    server_name app.bizbio.co.za;
    ssl_certificate     /etc/letsencrypt/live/bizbio.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/bizbio.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;
    return 301 https://app.snaptap.co.za$request_uri;
}

# cards.bizbio.co.za → snaptap.co.za (cards live at root)
server {
    listen 443 ssl;
    server_name cards.bizbio.co.za;
    ssl_certificate     /etc/letsencrypt/live/bizbio.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/bizbio.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;
    return 301 https://snaptap.co.za$request_uri;
}

# menu.bizbio.co.za → menu.snaptap.co.za
server {
    listen 443 ssl;
    server_name menu.bizbio.co.za;
    ssl_certificate     /etc/letsencrypt/live/bizbio.co.za/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/bizbio.co.za/privkey.pem;
    include             /etc/nginx/snippets/ssl-params.conf;
    return 301 https://menu.snaptap.co.za$request_uri;
}
```

### B.3 Enable and update Cloudflare DNS for bizbio.co.za

Add A records for bizbio.co.za (same IP: 154.66.198.20), then:

```bash
sudo ln -s /etc/nginx/sites-available/bizbio-redirects /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
```

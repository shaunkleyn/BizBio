# BizBio UI - Deployment Guide

## Understanding Environment Variables in Nuxt 3

Nuxt 3 environment variables work in two ways:

### 1. **Build-time Variables** (Baked into the build)
- Read from `.env` files **during** `npm run build`
- Cannot be changed after building
- Used for: Feature flags, build configurations

### 2. **Runtime Variables** (Can be changed without rebuilding)
- Read from **actual environment variables** on the server
- Can be changed by setting environment variables at runtime
- Used for: API URLs, secrets, deployment-specific configs

## Your Current Issue

❌ **Wrong:** File named `.env production` (with space)
✅ **Correct:** Should be `.env.production` (with dot)

## How to Fix It

### Step 1: Rename Your File

```bash
# In the BizBio.UI directory
mv ".env production" .env.production
```

### Step 2: Understand the Deployment Process

There are TWO ways to set environment variables for production:

#### Option A: Set During Build (Build-time)
Use this if you're building on the production server:

```bash
# 1. Place .env.production in the project root
# 2. Build the project
npm run build

# The values from .env.production are used during build
```

#### Option B: Set at Runtime (Recommended)
Use this if you're building elsewhere and deploying:

```bash
# Set environment variables on the server BEFORE starting the app
export NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1
export NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=...
export NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-FSFC9WQ181
export PORT=3000

# Then start the app
node .output/server/index.mjs
```

## Complete Deployment Workflows

### Workflow 1: Build on Production Server

```bash
# On your production server
cd /path/to/BizBio.UI

# 1. Create .env.production file
cat > .env.production << 'EOF'
NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-FSFC9WQ181
PORT=3000
NODE_ENV=production
EOF

# 2. Install dependencies
npm ci

# 3. Build (reads .env.production automatically)
npm run build

# 4. Start the server
node .output/server/index.mjs
```

### Workflow 2: Build Locally, Deploy Remotely (RECOMMENDED)

```bash
# === ON YOUR LOCAL MACHINE ===

# 1. Create .env.production locally
cat > .env.production << 'EOF'
NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-FSFC9WQ181
EOF

# 2. Build
npm run build

# 3. Zip the .output folder
cd .output
zip -r ../bizbio-ui-output.zip .
cd ..

# === ON YOUR PRODUCTION SERVER ===

# 4. Upload and extract bizbio-ui-output.zip to /var/www/bizbio-ui/

# 5. Create environment file on server
cat > /var/www/bizbio-ui/.env << 'EOF'
NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-FSFC9WQ181
PORT=3000
NODE_ENV=production
EOF

# 6. Load environment variables and start
cd /var/www/bizbio-ui
export $(cat .env | xargs)
node server/index.mjs
```

### Workflow 3: Using PM2 (BEST FOR PRODUCTION)

Create `ecosystem.config.cjs` (already exists in your project):

```javascript
module.exports = {
  apps: [{
    name: 'bizbio-ui',
    script: './server/index.mjs',
    cwd: '/var/www/bizbio-ui',
    instances: 2,
    exec_mode: 'cluster',
    env: {
      NODE_ENV: 'production',
      PORT: 3000,
      NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
      NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=...',
      NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181'
    }
  }]
}
```

Then:
```bash
pm2 start ecosystem.config.cjs
pm2 save
pm2 startup
```

## Testing Environment Variables

### Check what values are being used:

```bash
# In your browser console on the production site
console.log(useRuntimeConfig().public.apiUrl)
```

Or add a test endpoint to verify:

```bash
# Access /_config on your site
curl https://yourdomain.com/_config
```

## Common Issues & Solutions

### Issue 1: Still seeing localhost after deployment
**Cause:** Environment variables not set at runtime
**Fix:** Use Option B or PM2 workflow above

### Issue 2: Changes to .env.production not reflected
**Cause:** Need to rebuild after changing .env.production
**Fix:** Run `npm run build` again

### Issue 3: API calls fail in production
**Cause:** CORS or wrong API URL
**Fix:** Check browser network tab, verify API URL is correct

## File Structure After Deployment

```
/var/www/bizbio-ui/
├── server/
│   └── index.mjs          # Main server file
├── public/                 # Static assets
├── .env                   # Runtime environment variables (if using export method)
└── ecosystem.config.cjs   # PM2 configuration (if using PM2)
```

## Quick Reference

| File Name | When It's Read | Use Case |
|-----------|----------------|----------|
| `.env` | Build + Runtime | Development defaults |
| `.env.production` | Build only | Production build values |
| `.env.local` | Build + Runtime | Local overrides (git-ignored) |
| Server env vars | Runtime only | Production runtime values |

## Security Notes

1. ✅ **DO** use environment variables for sensitive data
2. ✅ **DO** add `.env.production` to `.gitignore` if it contains secrets
3. ❌ **DON'T** commit API keys to git
4. ❌ **DON'T** expose server-side secrets in `NUXT_PUBLIC_*` variables

## Your Current Setup

Based on your existing `.env production` file, here's what you need:

```bash
# Rename your file first
mv ".env production" .env.production

# Your production values:
NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-FSFC9WQ181
PORT=3000
```

## Next Steps

1. **Rename** `.env production` → `.env.production`
2. **Choose** a deployment workflow (Workflow 3 with PM2 recommended)
3. **Update** `ecosystem.config.cjs` with your environment variables
4. **Rebuild** if needed: `npm run build`
5. **Deploy** using your chosen method

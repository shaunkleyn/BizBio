# SnapTap Profile Serving Troubleshooting Plan

**Created**: 2026-03-03
**Reference**: Use across devices for debugging over coming days
**Related**: See `SNAPTAP_MIGRATION_PLAN.md` for full migration context

## Current Issues Reported

1. `snaptap.co.za/checkeredblock` - can't access
2. `snaptap.co.za/sixdoor` - doesn't render correctly
3. `snaptap.co.za/siyanda` - doesn't render correctly
4. `snaptap.me/checkeredblock` - infinite load cycle

## Architecture Overview

```
                    ┌─────────────────────────────────────┐
                    │           Nginx (port 80/443)       │
                    │   snaptap.co.za / snaptap.me        │
                    └──────────────┬──────────────────────┘
                                   │
                    ┌──────────────▼──────────────────────┐
                    │         Nuxt App (port 3001)        │
                    │         snaptap-frontend            │
                    │                                     │
                    │  ┌─────────────────────────────┐    │
                    │  │   profiles.ts middleware    │    │
                    │  │   Serves static profiles    │    │
                    │  │   from disk                 │    │
                    │  └─────────────────────────────┘    │
                    └──────────────┬──────────────────────┘
                                   │
                    ┌──────────────▼──────────────────────┐
                    │     /var/www/snaptap/ui/profiles/   │
                    │                                     │
                    │  checkeredblock/index.html          │
                    │  sixdoor/index.html                 │
                    │  siyanda/index.html                 │
                    └─────────────────────────────────────┘
```

## Configuration Files

| File | VPS Location | Description |
|------|--------------|-------------|
| `nginx/snaptap.co.za.conf` | `/etc/nginx/sites-available/snaptap` | Main snaptap.co.za domain |
| `nginx/snaptap-me` | `/etc/nginx/sites-available/snaptap-me` | snaptap.me domain |

Both configs proxy all traffic to Nuxt on port 3001. The Nuxt `profiles.ts` middleware handles serving static profile HTML from disk.

---

## Diagnostic Commands

### 1. Check Nuxt App Status

```bash
# Is the app running?
pm2 status

# Check if port 3001 is responding
curl -I http://127.0.0.1:3001/

# Check PM2 logs for errors
pm2 logs snaptap-frontend --lines 50
```

### 2. Check Profile Files Exist

```bash
# List profile directories
ls -la /var/www/snaptap/ui/profiles/

# Check specific profiles have index.html
ls -la /var/www/snaptap/ui/profiles/checkeredblock/
ls -la /var/www/snaptap/ui/profiles/sixdoor/
ls -la /var/www/snaptap/ui/profiles/siyanda/

# Verify index.html content
head -20 /var/www/snaptap/ui/profiles/checkeredblock/index.html
```

### 3. Check Nginx Configuration

```bash
# List enabled sites
ls -la /etc/nginx/sites-enabled/

# View snaptap.co.za config
cat /etc/nginx/sites-available/snaptap

# View snaptap.me config
cat /etc/nginx/sites-available/snaptap-me

# Test nginx config syntax
sudo nginx -t

# Check if snaptap-me is symlinked
ls -la /etc/nginx/sites-enabled/ | grep snaptap
```

### 4. Check SSL Certificates

```bash
# snaptap.co.za cert
ls -la /etc/letsencrypt/live/snaptap.co.za/

# snaptap.me cert
ls -la /etc/letsencrypt/live/snaptap.me/
```

### 5. Check Nginx Logs

```bash
# Error logs
sudo tail -50 /var/log/nginx/error.log

# Access logs for debugging
sudo tail -50 /var/log/nginx/access.log | grep -E "(checkeredblock|sixdoor|siyanda)"
```

### 6. Test Profile Serving Directly

```bash
# Test via Nuxt directly (bypassing nginx)
curl -I http://127.0.0.1:3001/checkeredblock

# Test via nginx (full stack)
curl -sI https://snaptap.co.za/checkeredblock | head -10
curl -sI https://snaptap.me/checkeredblock | head -10

# Test profile assets
curl -sI https://snaptap.co.za/checkeredblock/styles/main.css | head -5
```

---

## Common Issues & Fixes

### Issue: snaptap-me config not enabled

**Symptom**: `snaptap.me` returns default nginx page or 404

**Fix**:
```bash
# Create symlink to enable the site
sudo ln -s /etc/nginx/sites-available/snaptap-me /etc/nginx/sites-enabled/

# Test and reload
sudo nginx -t
sudo systemctl reload nginx
```

### Issue: SSL certificate not found for snaptap.me

**Symptom**: SSL errors when accessing `snaptap.me`

**Fix**:
```bash
# Generate certificate with certbot
sudo certbot --nginx -d snaptap.me -d www.snaptap.me -d cards.snaptap.me -d menu.snaptap.me
```

### Issue: Profile directory missing index.html

**Symptom**: 404 or blank page for specific profile

**Check**:
```bash
ls -la /var/www/snaptap/ui/profiles/<slug>/index.html
```

**Fix**: Re-publish the profile from the admin dashboard or manually create the HTML file.

### Issue: Nuxt app crashed or not running

**Symptom**: 502 Bad Gateway

**Fix**:
```bash
# Check status
pm2 status

# Restart the app
pm2 restart snaptap-frontend

# If not in pm2 list, start it
cd /var/www/snaptap/ui
pm2 start ecosystem.config.cjs
```

### Issue: Port 3001 not responding

**Symptom**: `curl http://127.0.0.1:3001/` fails

**Check**:
```bash
# What's using port 3001?
sudo netstat -tlnp | grep 3001

# Or with ss
sudo ss -tlnp | grep 3001
```

### Issue: Nginx proxy headers missing

**Symptom**: Redirect loops, wrong protocol in URLs

**Verify nginx config has**:
```nginx
proxy_set_header Host $host;
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $scheme;
```

---

## VPS Directory Structure

```
/var/www/snaptap/ui/
├── current -> releases/103/ui-build/    # Active release symlink
├── ecosystem.config.cjs                  # PM2 config
├── profiles/                             # Static profile HTML files
│   ├── checkeredblock/
│   │   ├── index.html
│   │   ├── ecwid/
│   │   ├── images/
│   │   ├── styles/
│   │   └── mfb/
│   ├── sixdoor/
│   │   └── index.html
│   ├── siyanda/
│   │   └── index.html
│   └── ...
├── releases/                             # Deployed releases
│   └── 103/ui-build/                     # Current active
└── logs/                                 # PM2 logs
```

---

## Quick Fix Checklist

Run these commands on the VPS in order:

```bash
# 1. Verify Nuxt is running
pm2 status
curl -I http://127.0.0.1:3001/

# 2. Check nginx configs exist
ls -la /etc/nginx/sites-available/snaptap*

# 3. Ensure both configs are enabled
sudo ln -sf /etc/nginx/sites-available/snaptap /etc/nginx/sites-enabled/
sudo ln -sf /etc/nginx/sites-available/snaptap-me /etc/nginx/sites-enabled/

# 4. Test nginx config
sudo nginx -t

# 5. Reload nginx
sudo systemctl reload nginx

# 6. Verify profile files exist
ls /var/www/snaptap/ui/profiles/*/index.html

# 7. Test endpoints
curl -sI https://snaptap.co.za/checkeredblock | head -5
curl -sI https://snaptap.me/checkeredblock | head -5
```

---

## SSH MCP Server Setup (Optional)

For direct VPS access from Claude Code, set up an SSH MCP server.

**Package**: `@aiondadotcom/mcp-ssh` ([GitHub](https://github.com/AiondaDotCom/mcp-ssh))

### Step 1: SSH Config

Add to `~/.ssh/config`:

```
Host snaptap-vps
    HostName 154.66.198.20
    User your-username
    IdentityFile ~/.ssh/your-private-key
```

Test:
```bash
ssh snaptap-vps "echo 'Connection successful'"
```

### Step 2: Configure Claude Code MCP

Edit `.claude/mcp.json`:

```json
{
  "mcpServers": {
    "mcp-ssh": {
      "command": "npx",
      "args": ["-y", "@aiondadotcom/mcp-ssh"]
    }
  }
}
```

### Step 3: Restart Claude Code

The MCP server reads `~/.ssh/config` automatically.

---

## Verification After Fixes

```bash
# Test all reported profiles
curl -sI https://snaptap.co.za/checkeredblock | head -5
curl -sI https://snaptap.co.za/sixdoor | head -5
curl -sI https://snaptap.co.za/siyanda | head -5
curl -sI https://snaptap.me/checkeredblock | head -5

# Test asset loading
curl -sI https://snaptap.co.za/checkeredblock/styles/main.css | head -5

# Expected: HTTP/2 200 for all
```

---

## Contact / Escalation

If issues persist after following this guide:

1. Check PM2 logs: `pm2 logs snaptap-frontend --lines 100`
2. Check Nginx error logs: `sudo tail -100 /var/log/nginx/error.log`
3. Verify the profiles.ts middleware is correctly reading from `/var/www/snaptap/ui/profiles/`

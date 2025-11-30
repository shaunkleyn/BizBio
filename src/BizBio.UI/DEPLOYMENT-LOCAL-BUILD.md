# Deployment Guide - Local Build Method

This guide explains how to deploy by building locally on your Windows machine and copying the build to your VPS.

## ✅ Advantages

- **Faster Deployment** - No build time on server
- **Lower VPS Requirements** - Need less RAM/CPU
- **Consistent Builds** - Same environment every time
- **Smaller VPS** - Can use cheaper hosting

## 🛠️ One-Time VPS Setup

### 1. Install Prerequisites on VPS

```bash
# SSH into your VPS
ssh your-username@your-vps-ip

# Update system
sudo apt update && sudo apt upgrade -y

# Install Node.js 20.x
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs

# Install PM2
sudo npm install -g pm2

# Verify installations
node --version
npm --version
pm2 --version
```

### 2. Create Application Directory

```bash
sudo mkdir -p /var/www/bizbio/ui
sudo chown $USER:$USER /var/www/bizbio/ui
```

### 3. Setup Cloudflare Tunnel

Follow steps in DEPLOYMENT.md Part 5 to install and configure cloudflared.

## 📦 Deployment Methods

### Method 1: Using PowerShell Script (Recommended)

**On your local Windows machine:**

```powershell
# Make sure you've built locally first
npm run build

# Run the deployment script
.\deploy-local-build.ps1 -VpsUser "your-username" -VpsHost "your-vps-ip"

# Or with custom path
.\deploy-local-build.ps1 -VpsUser "your-username" -VpsHost "your-vps-ip" -VpsPath "/var/www/bizbio/ui"
```

The script will:
1. Build your application
2. Upload `.output` directory
3. Upload configuration files
4. Install production dependencies on VPS
5. Restart the application with PM2

### Method 2: Manual SCP Commands

**Build locally:**
```powershell
npm run build
```

**Copy files to VPS:**
```powershell
# Copy .output directory
scp -r .output your-username@your-vps-ip:/var/www/bizbio/ui/

# Copy configuration files
scp .env.production your-username@your-vps-ip:/var/www/bizbio/ui/
scp ecosystem.config.cjs your-username@your-vps-ip:/var/www/bizbio/ui/
scp package.json your-username@your-vps-ip:/var/www/bizbio/ui/
scp package-lock.json your-username@your-vps-ip:/var/www/bizbio/ui/
```

**On VPS:**
```bash
cd /var/www/bizbio/ui
npm install --production
pm2 restart bizbio-frontend || pm2 start ecosystem.config.cjs
pm2 save
```

### Method 3: Using SFTP Client

Use WinSCP, FileZilla, or any SFTP client:

1. Connect to your VPS
2. Navigate to `/var/www/bizbio/ui`
3. Upload these items:
   - `.output` folder (entire folder)
   - `.env.production`
   - `ecosystem.config.cjs`
   - `package.json`
   - `package-lock.json`
4. SSH into VPS and run:
   ```bash
   cd /var/www/bizbio/ui
   npm install --production
   pm2 restart bizbio-frontend
   ```

## 📝 What Files to Copy

### Essential Files:
- ✅ `.output/` - Your built application
- ✅ `.env.production` - Environment variables
- ✅ `ecosystem.config.cjs` - PM2 configuration
- ✅ `package.json` - For production dependencies
- ✅ `package-lock.json` - Exact dependency versions

### NOT Needed on VPS:
- ❌ `node_modules/` - Will be installed on VPS
- ❌ `.nuxt/` - Build cache, not needed
- ❌ Source files (`pages/`, `components/`, etc.)
- ❌ `nuxt.config.ts` - Already compiled into .output
- ❌ Dev dependencies

## 🔄 Future Deployments

After initial setup, each deployment is just:

**Option A: Using Script**
```powershell
.\deploy-local-build.ps1 -VpsUser "your-username" -VpsHost "your-vps-ip"
```

**Option B: Manual**
```powershell
# 1. Build locally
npm run build

# 2. Copy .output to VPS
scp -r .output your-username@your-vps-ip:/var/www/bizbio/ui/

# 3. Restart on VPS
ssh your-username@your-vps-ip "cd /var/www/bizbio/ui && pm2 restart bizbio-frontend"
```

## 🎯 Complete First Deployment

### On Your Windows Machine:

```powershell
# 1. Ensure you have .env.production configured
# Edit .env.production with your API URL

# 2. Build the application
npm run build

# 3. Deploy using the script
.\deploy-local-build.ps1 -VpsUser "your-username" -VpsHost "your-vps-ip"
```

### On Your VPS (First Time Only):

```bash
# Setup PM2 to start on boot
pm2 startup systemd
# Run the command it outputs

pm2 save
```

## 🔍 Verification

**Check if application is running:**
```bash
# On VPS
pm2 status
pm2 logs bizbio-frontend

# Test locally on VPS
curl http://localhost:3000

# Check tunnel
sudo systemctl status cloudflared
```

**Test from browser:**
- Visit `https://yourdomain.com`
- Verify all pages load correctly

## 💡 Tips

### Speed Up Transfers

Use compression with rsync (if available):
```bash
rsync -avz --delete .output/ your-username@your-vps-ip:/var/www/bizbio/ui/.output/
```

### SSH Key Setup

To avoid entering password every time:

```powershell
# Generate SSH key (if you don't have one)
ssh-keygen -t ed25519

# Copy to VPS
type $env:USERPROFILE\.ssh\id_ed25519.pub | ssh your-username@your-vps-ip "cat >> ~/.ssh/authorized_keys"
```

### Automated Builds

Add to `package.json`:
```json
"scripts": {
  "deploy": "npm run build && powershell -File deploy-local-build.ps1"
}
```

## 🐛 Troubleshooting

### Build Fails Locally
```powershell
# Clear cache and rebuild
Remove-Item -Recurse -Force .nuxt, .output, node_modules
npm install
npm run build
```

### Upload Fails
```powershell
# Test SSH connection
ssh your-username@your-vps-ip "echo Connection successful"

# Check if path exists
ssh your-username@your-vps-ip "ls -la /var/www/bizbio/ui"
```

### App Won't Start on VPS
```bash
# Check logs
pm2 logs bizbio-frontend --err

# Verify .output exists
ls -la /var/www/bizbio/ui/.output/

# Try starting manually
cd /var/www/bizbio/ui
node .output/server/index.mjs
```

### Missing Dependencies
```bash
# On VPS, reinstall production dependencies
cd /var/www/bizbio/ui
rm -rf node_modules
npm install --production
pm2 restart bizbio-frontend
```

## 📊 Comparison: Local Build vs Remote Build

| Aspect | Local Build | Remote Build |
|--------|-------------|--------------|
| Build Speed | Fast (your PC) | Slower (VPS) |
| VPS RAM Needed | 512MB - 1GB | 2GB+ |
| VPS CPU Needed | Minimal | More |
| Transfer Time | Medium | None |
| VPS Cost | Lower | Higher |
| Build Consistency | High | Variable |
| Internet Required | Yes (upload) | Yes (git) |

## 🎉 Summary

With local builds, you:
1. ✅ Build on your powerful local machine
2. ✅ Upload only the `.output` folder
3. ✅ Use a cheaper/smaller VPS
4. ✅ Deploy faster (no build time on server)
5. ✅ Get consistent builds every time

This is especially useful when:
- Your VPS has limited resources
- You want faster deployments
- You prefer building in a controlled environment
- You want to reduce VPS costs

---

**Next Steps:**
1. Set up your VPS (see above)
2. Configure `.env.production` locally
3. Run `npm run build` locally
4. Use the PowerShell script or manual method to deploy
5. Configure Cloudflare Tunnel to point to `localhost:3000`

Happy deploying! 🚀

# 📚 Deployment Documentation Overview

This directory contains comprehensive documentation for deploying the BizBio Frontend application to a Debian VPS using Cloudflare Tunnels.

## 📖 Documentation Files

### 🚀 Quick Start
**[QUICKSTART-DEPLOYMENT.md](QUICKSTART-DEPLOYMENT.md)** - Start here!
- TL;DR version of deployment steps
- Copy-paste commands for quick setup
- Essential commands reference

### 🏗️ Deployment Methods

**[DEPLOYMENT.md](DEPLOYMENT.md)** - Build on VPS (Traditional)
- Step-by-step instructions
- Build directly on server
- Detailed explanations
- Best for powerful VPS

**[DEPLOYMENT-LOCAL-BUILD.md](DEPLOYMENT-LOCAL-BUILD.md)** - Build Locally (Recommended) ⭐
- Build on your Windows machine
- Upload only .output folder
- Lower VPS requirements
- Faster deployments
- **Best for smaller/cheaper VPS**

### ✅ Pre-Deployment
**[CHECKLIST.md](CHECKLIST.md)** - Before you deploy
- Pre-deployment checklist
- Verification steps
- Common issues and solutions
- Rollback plan

### 📊 Monitoring
**[MONITORING.md](MONITORING.md)** - Keep your app healthy
- Health check commands
- System monitoring
- Performance monitoring
- Alert setup

## 🛠️ Configuration Files

### Application Files
- `ecosystem.config.cjs` - PM2 process manager configuration
- `.env.production` - Production environment variables
- `.env.example` - Environment template

### Deployment Scripts
- `deploy.sh` - Automated deployment script (for remote build)
- `deploy-local-build.ps1` - PowerShell script for local build deployment
- `health-check.sh` - Application health monitoring script

## 🎯 Quick Deployment Overview

### Architecture
```
User Browser
    ↓
Cloudflare (SSL/CDN)
    ↓
Cloudflare Tunnel (cloudflared)
    ↓
Your VPS → localhost:3000
    ↓
PM2 → Nuxt.js SSR App
```

### 🔀 Choose Your Deployment Method

#### Method 1: Local Build (Recommended) ⭐
**Build on Windows, upload to VPS**
- ✅ Faster deployments
- ✅ Lower VPS requirements (512MB - 1GB RAM)
- ✅ Cheaper VPS hosting
- ✅ Consistent builds
- 📖 Guide: [DEPLOYMENT-LOCAL-BUILD.md](DEPLOYMENT-LOCAL-BUILD.md)

#### Method 2: Remote Build (Traditional)
**Build directly on VPS**
- ✅ No file uploads needed
- ✅ Direct Git integration
- ⚠️ Needs more powerful VPS (2GB+ RAM)
- 📖 Guide: [DEPLOYMENT.md](DEPLOYMENT.md)

### One-Time Setup (~20 minutes)
1. **Prepare VPS**: Install Node.js, PM2, cloudflared
2. **Clone & Build**: Get code and build application (or build locally)
3. **Start App**: Launch with PM2
4. **Setup Tunnel**: Configure Cloudflare Tunnel
5. **Configure Domain**: Set up public hostname in Cloudflare dashboard

### Future Deployments

**Local Build Method (~1 minute):**
```powershell
npm run build
.\deploy-local-build.ps1 -VpsUser "user" -VpsHost "ip"
```

**Remote Build Method (~2 minutes):**
```bash
ssh your-user@your-vps
cd /var/www/bizbio/ui
./deploy.sh
```

## 📋 Deployment Steps Summary

### On Your VPS (First Time Only)
```bash
# Install prerequisites
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs
sudo npm install -g pm2

# Setup application
sudo mkdir -p /var/www/bizbio-frontend
sudo chown $USER:$USER /var/www/bizbio-frontend
cd /var/www/bizbio-frontend
git clone -b nervous-goldberg https://github.com/shaunkleyn/BizBio.Frontend.git .

# Configure and start
nano .env.production  # Add NUXT_PUBLIC_API_URL
npm install
npm run build
pm2 start ecosystem.config.cjs
pm2 save
pm2 startup systemd
```

### Install and Configure Cloudflare Tunnel
```bash
# Install cloudflared
sudo mkdir -p --mode=0755 /usr/share/keyrings
curl -fsSL https://pkg.cloudflare.com/cloudflare-main.gpg | sudo tee /usr/share/keyrings/cloudflare-main.gpg >/dev/null
echo "deb [signed-by=/usr/share/keyrings/cloudflare-main.gpg] https://pkg.cloudflare.com/cloudflared $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/cloudflared.list
sudo apt update && sudo apt install -y cloudflared

# Setup tunnel
cloudflared tunnel login
cloudflared tunnel create bizbio-frontend
sudo cloudflared service install
sudo systemctl start cloudflared
sudo systemctl enable cloudflared
```

### In Cloudflare Dashboard
1. Go to **Zero Trust** → **Access** → **Tunnels**
2. Find your tunnel and click **Configure**
3. Add **Public Hostname**:
   - Domain: your-domain.com
   - Type: HTTP
   - URL: localhost:3000

✅ Done! Your site is now live at `https://your-domain.com`

## 🔧 Essential Commands

### Application Management
```bash
pm2 status                    # Check app status
pm2 logs bizbio-frontend      # View logs
pm2 restart bizbio-frontend   # Restart app
pm2 monit                     # Monitor resources
```

### Tunnel Management
```bash
sudo systemctl status cloudflared        # Check tunnel
sudo journalctl -u cloudflared -f       # View tunnel logs
sudo systemctl restart cloudflared      # Restart tunnel
```

### Deployment
```bash
cd /var/www/bizbio-frontend
./deploy.sh                   # Deploy updates
```

## 🆘 Troubleshooting

### App Not Working?
```bash
pm2 logs bizbio-frontend --err
pm2 restart bizbio-frontend
```

### Tunnel Not Connecting?
```bash
sudo systemctl restart cloudflared
sudo journalctl -u cloudflared -n 50
```

### Need to Rebuild?
```bash
cd /var/www/bizbio-frontend
npm run build
pm2 restart bizbio-frontend
```

## 🔒 Security Notes

- **No ports open**: Cloudflare Tunnel handles all traffic securely
- **SSL/TLS**: Automatically provided by Cloudflare
- **Environment variables**: Never commit `.env.production` to Git
- **Firewall**: With Cloudflare Tunnel, only SSH port (22) needs to be open

## 🎯 Benefits of This Setup

✅ **No port forwarding** needed  
✅ **Free SSL certificate** from Cloudflare  
✅ **DDoS protection** built-in  
✅ **Zero-trust security** model  
✅ **Easy deployment** with one script  
✅ **Auto-restart** on crashes (PM2)  
✅ **Clustering** for better performance  
✅ **Health monitoring** built-in  

## 📚 Additional Resources

- [Nuxt.js Documentation](https://nuxt.com/docs)
- [PM2 Documentation](https://pm2.keymetrics.io/docs/)
- [Cloudflare Tunnel Documentation](https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/)
- [Node.js Best Practices](https://nodejs.org/en/docs/guides/)

## 🆘 Getting Help

1. Check the relevant documentation file above
2. Review logs: `pm2 logs bizbio-frontend`
3. Check tunnel: `sudo journalctl -u cloudflared -f`
4. Verify API connection: `curl https://api.yourdomain.com/api`

## 📝 Notes

- **Recommended RAM**: 2GB minimum
- **Node.js Version**: 18+ (20.x LTS recommended)
- **Port**: Application runs on localhost:3000
- **Process Manager**: PM2 in cluster mode
- **SSL**: Handled by Cloudflare (free)
- **Backups**: Remember to backup `.env.production`

---

**Ready to deploy?** Start with **[QUICKSTART-DEPLOYMENT.md](QUICKSTART-DEPLOYMENT.md)**! 🚀

**Need details?** Read **[DEPLOYMENT.md](DEPLOYMENT.md)** for the complete guide.

**Deploying for the first time?** Check **[CHECKLIST.md](CHECKLIST.md)** first!

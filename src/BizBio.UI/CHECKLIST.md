# Pre-Deployment Checklist

## Before You Deploy

### ✅ Local Development
- [ ] Application runs successfully in development mode (`npm run dev`)
- [ ] No console errors in browser
- [ ] All features tested and working
- [ ] Latest changes committed to Git
- [ ] Code pushed to GitHub repository

### ✅ Environment Configuration
- [ ] `.env.production` created with correct values
- [ ] API URL configured correctly (`NUXT_PUBLIC_API_URL`)
- [ ] Backend API is running and accessible
- [ ] Database is set up and migrated (on backend)

### ✅ Server Prerequisites
- [ ] VPS server is accessible via SSH
- [ ] Node.js 18+ installed
- [ ] PM2 installed globally
- [ ] Git installed
- [ ] At least 1GB RAM available (2GB recommended)
- [ ] Port 3000 available

### ✅ Cloudflare Setup
- [ ] Cloudflare account created
- [ ] Domain added to Cloudflare
- [ ] DNS configured properly
- [ ] cloudflared installed on server
- [ ] Cloudflare tunnel created
- [ ] Tunnel authenticated

### ✅ Build & Test
- [ ] Application builds successfully (`npm run build`)
- [ ] Build output exists in `.output` directory
- [ ] No build errors or warnings
- [ ] Application runs with production build (`node .output/server/index.mjs`)

## Deployment Steps Summary

### 1️⃣ Initial Setup (One-Time)
```bash
# On your VPS
sudo apt update && sudo apt upgrade -y
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs
sudo npm install -g pm2

# Install cloudflared (see QUICKSTART-DEPLOYMENT.md for detailed commands)

# Create directory and clone
sudo mkdir -p /var/www/bizbio-frontend
sudo chown $USER:$USER /var/www/bizbio-frontend
cd /var/www/bizbio-frontend
git clone -b nervous-goldberg https://github.com/shaunkleyn/BizBio.Frontend.git .

# Setup environment
nano .env.production  # Add your configuration

# Install, build, and start
npm install
npm run build
chmod +x deploy.sh
pm2 start ecosystem.config.cjs
pm2 save
pm2 startup systemd
```

### 2️⃣ Configure Cloudflare Tunnel
```bash
# On your VPS
cloudflared tunnel login
cloudflared tunnel create bizbio-frontend
sudo cloudflared service install
sudo systemctl start cloudflared
sudo systemctl enable cloudflared
```

Then in Cloudflare Dashboard:
- Go to Zero Trust > Access > Tunnels
- Configure public hostname
- Point to `localhost:3000`

### 3️⃣ Verify Deployment
```bash
# Check application
pm2 status
curl http://localhost:3000

# Check tunnel
sudo systemctl status cloudflared

# Check from browser
# Visit https://yourdomain.com
```

### 4️⃣ Future Updates
```bash
# SSH to server
ssh your-username@your-vps-ip

# Navigate to project
cd /var/www/bizbio-frontend

# Run deployment script
./deploy.sh
```

## Post-Deployment Verification

### ✅ Application Health
- [ ] PM2 shows app as "online": `pm2 status`
- [ ] No errors in logs: `pm2 logs bizbio-frontend --lines 50`
- [ ] App responds locally: `curl http://localhost:3000`
- [ ] Memory usage is reasonable: `pm2 monit`

### ✅ Cloudflare Tunnel
- [ ] Tunnel service is active: `sudo systemctl status cloudflared`
- [ ] No errors in tunnel logs: `sudo journalctl -u cloudflared -n 50`
- [ ] Tunnel shows as "HEALTHY" in Cloudflare dashboard

### ✅ Public Access
- [ ] Website loads via domain: `https://yourdomain.com`
- [ ] SSL/TLS certificate is valid (green padlock)
- [ ] All pages load correctly
- [ ] Login/Register functionality works
- [ ] API calls are successful
- [ ] No CORS errors in browser console

### ✅ Performance
- [ ] Page load time is acceptable (< 3 seconds)
- [ ] Images load correctly
- [ ] No 404 errors for assets
- [ ] CSS/JavaScript loads properly

### ✅ Security
- [ ] `.env.production` not committed to Git
- [ ] Firewall configured (if applicable)
- [ ] Only necessary ports are open
- [ ] SSH uses key-based authentication
- [ ] Server has latest security updates

## Common Pre-Deployment Issues

### Issue: Build Fails
**Solution:**
```bash
# Clear cache and reinstall
rm -rf node_modules .nuxt .output
npm install
npm run build
```

### Issue: API Connection Fails
**Solution:**
- Verify `NUXT_PUBLIC_API_URL` in `.env.production`
- Check if backend API is running
- Test API endpoint: `curl https://api.yourdomain.com/api/health`
- Check CORS settings on backend

### Issue: Port 3000 Already in Use
**Solution:**
```bash
# Find process using port 3000
sudo lsof -i :3000

# Kill the process
sudo kill -9 <PID>

# Or change port in ecosystem.config.cjs
```

### Issue: Out of Memory During Build
**Solution:**
- Increase swap space on VPS
- Use a machine with more RAM
- Build locally and deploy the `.output` directory

### Issue: Permission Denied Errors
**Solution:**
```bash
# Fix ownership
sudo chown -R $USER:$USER /var/www/bizbio-frontend

# Fix permissions
chmod +x deploy.sh
```

## Rollback Plan

If something goes wrong:

```bash
# Stop the application
pm2 stop bizbio-frontend

# Revert to previous commit
git log --oneline  # Find the previous commit hash
git checkout <previous-commit-hash>

# Rebuild
npm install
npm run build

# Restart
pm2 restart bizbio-frontend
```

## Need Help?

- Check logs: `pm2 logs bizbio-frontend`
- Check tunnel: `sudo journalctl -u cloudflared -f`
- Review documentation:
  - `DEPLOYMENT.md` - Full deployment guide
  - `QUICKSTART-DEPLOYMENT.md` - Quick reference
  - `MONITORING.md` - Health checks and monitoring

## Support Resources

- Nuxt.js Docs: https://nuxt.com/docs
- PM2 Docs: https://pm2.keymetrics.io/docs/
- Cloudflare Tunnel Docs: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/
- Node.js Docs: https://nodejs.org/docs/

---

**Remember**: Always test in a staging environment first if possible!

Good luck with your deployment! 🚀

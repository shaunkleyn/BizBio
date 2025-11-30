# BizBio Frontend Deployment Guide

## Deploying Nuxt.js SSR to Debian VPS with Cloudflare Tunnels

This guide will help you deploy the BizBio frontend application to a Debian VPS server using PM2 for process management and Cloudflare Tunnels for secure access.

---

## Prerequisites

### On Your Local Machine
- Git installed
- SSH access to your VPS

### On Your Debian VPS
- Debian 10+ or Ubuntu 20.04+
- Root or sudo access
- At least 1GB RAM (2GB recommended)
- Node.js 18+ and npm installed

### Cloudflare
- Cloudflare account with your domain configured
- Cloudflare Tunnel created (we'll configure it via dashboard)

---

## Part 1: Prepare Your VPS

### 1.1 Connect to Your VPS
```bash
ssh your-username@your-vps-ip
```

### 1.2 Update System
```bash
sudo apt update && sudo apt upgrade -y
```

### 1.3 Install Node.js 20.x (LTS)
```bash
# Install Node.js 20.x
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs

# Verify installation
node --version
npm --version
```

### 1.4 Install PM2 Globally
```bash
sudo npm install -g pm2

# Verify installation
pm2 --version
```

### 1.5 Install Git (if not installed)
```bash
sudo apt install -y git
```

---

## Part 2: Set Up Application Directory

### 2.1 Create Application Directory
```bash
# Create directory for your application
sudo mkdir -p /var/www/bizbio/ui
sudo chown $USER:$USER /var/www/bizbio/ui
cd /var/www/bizbio/ui
```

### 2.2 Clone Your Repository
```bash
# Clone your repository
git clone https://github.com/shaunkleyn/BizBio.Frontend.git .

# Or if you prefer a specific branch
git clone -b nervous-goldberg https://github.com/shaunkleyn/BizBio.Frontend.git .
```

### 2.3 Create Production Environment File
```bash
nano .env.production
```

Add your production configuration:
```env
NUXT_PUBLIC_API_URL=https://api.yourdomain.com/api
NODE_ENV=production
```

---

## Part 3: Build and Deploy Application

### 3.1 Install Dependencies
```bash
npm install
```

### 3.2 Build for Production
```bash
npm run build
```

This will create a `.output` directory with your production-ready application.

### 3.3 Test the Build Locally
```bash
# Test if the build works
node .output/server/index.mjs
```

Visit `http://your-vps-ip:3000` to verify it works. Press `Ctrl+C` to stop.

---

## Part 4: Configure PM2

### 4.1 Start Application with PM2
```bash
pm2 start ecosystem.config.cjs
```

### 4.2 Verify Application is Running
```bash
pm2 status
pm2 logs bizbio-frontend
```

### 4.3 Configure PM2 to Start on Boot
```bash
# Generate startup script
pm2 startup systemd

# This will output a command like:
# sudo env PATH=$PATH:/usr/bin pm2 startup systemd -u your-username --hp /home/your-username
# Copy and run that command

# Save the PM2 process list
pm2 save
```

### 4.4 Useful PM2 Commands
```bash
# View logs
pm2 logs bizbio-frontend

# Restart application
pm2 restart bizbio-frontend

# Stop application
pm2 stop bizbio-frontend

# Monitor resources
pm2 monit

# View detailed info
pm2 info bizbio-frontend
```

---

## Part 5: Set Up Cloudflare Tunnel

### 5.1 Install cloudflared on Your VPS
```bash
# Add cloudflare GPG key
sudo mkdir -p --mode=0755 /usr/share/keyrings
curl -fsSL https://pkg.cloudflare.com/cloudflare-main.gpg | sudo tee /usr/share/keyrings/cloudflare-main.gpg >/dev/null

# Add cloudflare repository
echo "deb [signed-by=/usr/share/keyrings/cloudflare-main.gpg] https://pkg.cloudflare.com/cloudflared $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/cloudflared.list

# Update and install
sudo apt update
sudo apt install -y cloudflared

# Verify installation
cloudflared --version
```

### 5.2 Authenticate cloudflared
```bash
cloudflared tunnel login
```

This will open a browser window to authenticate with Cloudflare. If you're on a headless server, it will provide a URL to open on your local machine.

### 5.3 Create a Tunnel
```bash
cloudflared tunnel create bizbio-frontend
```

This will create a tunnel and output a tunnel ID. **Save this tunnel ID** - you'll need it.

### 5.4 Run Tunnel as a Service
```bash
# Install the tunnel as a system service
sudo cloudflared service install
```

---

## Part 6: Configure Cloudflare Dashboard

### 6.1 Configure Tunnel in Cloudflare Dashboard

1. **Go to Cloudflare Dashboard**
   - Navigate to Zero Trust > Access > Tunnels
   - Find your `bizbio-frontend` tunnel

2. **Configure Public Hostname**
   - Click on your tunnel
   - Go to "Public Hostname" tab
   - Click "Add a public hostname"

3. **Add Hostname Configuration**
   - **Subdomain**: `www` (or leave empty for root domain)
   - **Domain**: Select your domain from dropdown
   - **Type**: HTTP
   - **URL**: `localhost:3000`
   - Click "Save hostname"

4. **Add Additional Hostnames** (if needed)
   - Repeat for root domain if you used `www` above
   - Subdomain: (leave empty)
   - Domain: your domain
   - Type: HTTP
   - URL: `localhost:3000`

### 6.2 Start the Tunnel
```bash
# Start the tunnel service
sudo systemctl start cloudflared

# Enable on boot
sudo systemctl enable cloudflared

# Check status
sudo systemctl status cloudflared
```

---

## Part 7: Update and Redeploy

### 7.1 Create Deployment Script

Create a deployment script for easy updates:

```bash
nano /var/www/bizbio-frontend/deploy.sh
```

```bash
#!/bin/bash

# BizBio Frontend Deployment Script

echo "🚀 Starting deployment..."

# Navigate to project directory
cd /var/www/bizbio-frontend

# Pull latest changes
echo "📥 Pulling latest changes from Git..."
git pull origin nervous-goldberg

# Install dependencies
echo "📦 Installing dependencies..."
npm install

# Build application
echo "🔨 Building application..."
npm run build

# Restart PM2
echo "♻️  Restarting application..."
pm2 restart bizbio-frontend

# Show status
echo "✅ Deployment complete!"
pm2 status bizbio-frontend
pm2 logs bizbio-frontend --lines 50
```

Make it executable:
```bash
chmod +x deploy.sh
```

### 7.2 Deploy Updates
```bash
cd /var/www/bizbio-frontend
./deploy.sh
```

---

## Part 8: Monitoring and Troubleshooting

### 8.1 Check Application Logs
```bash
# View PM2 logs
pm2 logs bizbio-frontend

# View last 100 lines
pm2 logs bizbio-frontend --lines 100

# Follow logs in real-time
pm2 logs bizbio-frontend --raw
```

### 8.2 Check Cloudflare Tunnel Status
```bash
# Check tunnel service status
sudo systemctl status cloudflared

# View tunnel logs
sudo journalctl -u cloudflared -f
```

### 8.3 Check Application Health
```bash
# Test locally
curl http://localhost:3000

# Check if PM2 is running
pm2 status

# Check system resources
pm2 monit
```

### 8.4 Common Issues

**Application won't start:**
```bash
# Check for errors in build
npm run build

# Check PM2 logs
pm2 logs bizbio-frontend --err

# Restart PM2
pm2 restart bizbio-frontend
```

**Tunnel not connecting:**
```bash
# Check tunnel status
sudo systemctl status cloudflared

# Restart tunnel
sudo systemctl restart cloudflared

# Check tunnel logs
sudo journalctl -u cloudflared -n 50
```

**Out of memory errors:**
```bash
# Increase Node.js memory limit in ecosystem.config.cjs
# Add to node_args: '--max-old-space-size=2048'
```

---

## Part 9: Security Best Practices

### 9.1 Firewall Configuration
```bash
# Install UFW if not installed
sudo apt install -y ufw

# Allow SSH (important - do this first!)
sudo ufw allow ssh

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

**Note**: With Cloudflare Tunnels, you don't need to open port 3000 publicly. The tunnel handles all traffic securely.

### 9.2 Keep System Updated
```bash
# Create update script
sudo crontab -e

# Add weekly updates (runs every Sunday at 3 AM)
0 3 * * 0 apt update && apt upgrade -y
```

### 9.3 Environment Variables
- Never commit `.env.production` to Git
- Keep sensitive data in environment variables
- Use different API keys for production

---

## Part 10: Performance Optimization

### 10.1 Enable Nginx (Optional - for caching)
If you want additional caching and performance:

```bash
sudo apt install -y nginx

# Create Nginx config
sudo nano /etc/nginx/sites-available/bizbio-frontend
```

```nginx
server {
    listen 8080;
    server_name localhost;

    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Then update your Cloudflare Tunnel to point to `localhost:8080` instead of `localhost:3000`.

---

## Summary

Your deployment flow:
1. **Application**: Runs on `localhost:3000` via PM2
2. **Cloudflare Tunnel**: Securely exposes your app through `cloudflared`
3. **Public Access**: Users access via your domain (e.g., `https://yourdomain.com`)
4. **No Firewall Rules Needed**: Cloudflare Tunnel handles everything securely

### Quick Reference Commands

```bash
# Application Management
pm2 restart bizbio-frontend    # Restart app
pm2 logs bizbio-frontend       # View logs
pm2 monit                      # Monitor resources

# Tunnel Management
sudo systemctl restart cloudflared  # Restart tunnel
sudo journalctl -u cloudflared -f   # View tunnel logs

# Deployment
cd /var/www/bizbio-frontend
./deploy.sh                         # Deploy updates
```

---

## Need Help?

- Check PM2 logs: `pm2 logs bizbio-frontend`
- Check tunnel logs: `sudo journalctl -u cloudflared -f`
- Test locally: `curl http://localhost:3000`
- Verify PM2 is running: `pm2 status`

Happy deploying! 🚀

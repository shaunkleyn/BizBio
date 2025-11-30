# Quick Deployment Guide - TL;DR

## For Your VPS Server

### One-Time Setup (Run these commands on your VPS)

```bash
# 1. Update system
sudo apt update && sudo apt upgrade -y

# 2. Install Node.js 20.x
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install -y nodejs

# 3. Install PM2
sudo npm install -g pm2

# 4. Install cloudflared
sudo mkdir -p --mode=0755 /usr/share/keyrings
curl -fsSL https://pkg.cloudflare.com/cloudflare-main.gpg | sudo tee /usr/share/keyrings/cloudflare-main.gpg >/dev/null
echo "deb [signed-by=/usr/share/keyrings/cloudflare-main.gpg] https://pkg.cloudflare.com/cloudflared $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/cloudflared.list
sudo apt update && sudo apt install -y cloudflared

# 5. Create app directory
sudo mkdir -p /var/www/bizbio-frontend
sudo chown $USER:$USER /var/www/bizbio-frontend
cd /var/www/bizbio-frontend

# 6. Clone repository
git clone -b nervous-goldberg https://github.com/shaunkleyn/BizBio.Frontend.git .

# 7. Create .env.production file
nano .env.production
# Add: NUXT_PUBLIC_API_URL=https://api.yourdomain.com/api

# 8. Install and build
npm install
npm run build

# 9. Make deploy script executable
chmod +x deploy.sh

# 10. Start with PM2
pm2 start ecosystem.config.cjs
pm2 save
pm2 startup systemd
# Run the command it outputs

# 11. Setup Cloudflare Tunnel
cloudflared tunnel login
cloudflared tunnel create bizbio-frontend
sudo cloudflared service install
sudo systemctl start cloudflared
sudo systemctl enable cloudflared
```

### Configure in Cloudflare Dashboard
1. Go to **Zero Trust** > **Access** > **Tunnels**
2. Find your `bizbio-frontend` tunnel
3. Click **Configure** > **Public Hostname** > **Add a public hostname**
4. Set:
   - **Subdomain**: www (or leave empty)
   - **Domain**: your-domain.com
   - **Type**: HTTP
   - **URL**: localhost:3000
5. Save

---

## Future Deployments (After Setup)

```bash
# SSH into your server
ssh your-username@your-vps-ip

# Navigate to project
cd /var/www/bizbio-frontend

# Run deployment script
./deploy.sh
```

That's it! ✅

---

## Useful Commands

```bash
# View logs
pm2 logs bizbio-frontend

# Restart app
pm2 restart bizbio-frontend

# Check status
pm2 status

# Monitor resources
pm2 monit

# Check tunnel
sudo systemctl status cloudflared

# View tunnel logs
sudo journalctl -u cloudflared -f
```

---

## Troubleshooting

**App not working?**
```bash
pm2 logs bizbio-frontend --err
```

**Tunnel not connecting?**
```bash
sudo systemctl restart cloudflared
sudo journalctl -u cloudflared -n 50
```

**Port already in use?**
```bash
sudo lsof -i :3000
pm2 restart bizbio-frontend
```

---

For detailed instructions, see **DEPLOYMENT.md**

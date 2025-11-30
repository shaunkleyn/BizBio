# Server Health Check & Monitoring

## Quick Health Check Commands

### Check if Application is Running
```bash
# Check PM2 status
pm2 status

# Check if port 3000 is listening
sudo netstat -tulpn | grep :3000

# Test local endpoint
curl http://localhost:3000

# Check response headers
curl -I http://localhost:3000
```

### Check Cloudflare Tunnel
```bash
# Check tunnel service status
sudo systemctl status cloudflared

# View tunnel logs (last 50 lines)
sudo journalctl -u cloudflared -n 50

# Follow tunnel logs in real-time
sudo journalctl -u cloudflared -f

# Check tunnel configuration
cloudflared tunnel info bizbio-frontend
```

### Check Application Logs
```bash
# View PM2 logs
pm2 logs bizbio-frontend

# View only errors
pm2 logs bizbio-frontend --err

# View last 100 lines
pm2 logs bizbio-frontend --lines 100

# Follow logs in real-time
pm2 logs bizbio-frontend --raw
```

### Monitor System Resources
```bash
# PM2 monitoring interface
pm2 monit

# Check memory usage
free -h

# Check disk space
df -h

# Check CPU usage
top -bn1 | grep "Cpu(s)"

# Check Node.js processes
ps aux | grep node
```

### Restart Services
```bash
# Restart application
pm2 restart bizbio-frontend

# Restart tunnel
sudo systemctl restart cloudflared

# Restart both
pm2 restart bizbio-frontend && sudo systemctl restart cloudflared
```

## Setting Up Monitoring Alerts

### Create a Health Check Script
```bash
nano /var/www/bizbio-frontend/health-check.sh
```

```bash
#!/bin/bash

# Health check script
URL="http://localhost:3000"
RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" $URL)

if [ $RESPONSE -eq 200 ]; then
    echo "✅ Application is healthy (HTTP $RESPONSE)"
    exit 0
else
    echo "❌ Application is unhealthy (HTTP $RESPONSE)"
    echo "Attempting to restart..."
    pm2 restart bizbio-frontend
    exit 1
fi
```

Make it executable:
```bash
chmod +x /var/www/bizbio-frontend/health-check.sh
```

### Schedule Regular Health Checks
```bash
# Edit crontab
crontab -e

# Add this line to check every 5 minutes
*/5 * * * * /var/www/bizbio-frontend/health-check.sh >> /var/log/bizbio-health.log 2>&1
```

## Common Issues & Solutions

### Issue: Application Not Responding
```bash
# Check if process is running
pm2 status

# Check logs for errors
pm2 logs bizbio-frontend --err

# Restart application
pm2 restart bizbio-frontend

# If still not working, rebuild
cd /var/www/bizbio-frontend
npm run build
pm2 restart bizbio-frontend
```

### Issue: High Memory Usage
```bash
# Check current memory
pm2 info bizbio-frontend

# Increase memory limit in ecosystem.config.cjs
# node_args: '--max-old-space-size=2048'

# Restart with new config
pm2 restart bizbio-frontend
```

### Issue: Tunnel Not Connecting
```bash
# Check tunnel status
sudo systemctl status cloudflared

# View tunnel logs
sudo journalctl -u cloudflared -n 50

# Restart tunnel
sudo systemctl restart cloudflared

# If still not working, check Cloudflare dashboard
# Verify tunnel configuration and public hostname settings
```

### Issue: Port 3000 Already in Use
```bash
# Find what's using port 3000
sudo lsof -i :3000

# Kill the process (use PID from above command)
sudo kill -9 <PID>

# Or stop PM2 and restart
pm2 stop bizbio-frontend
pm2 start ecosystem.config.cjs
```

### Issue: Cannot Connect to API
```bash
# Check .env.production file
cat /var/www/bizbio-frontend/.env.production

# Verify API_URL is correct
# Test API connection
curl https://api.yourdomain.com/api/health

# If API is down, check backend server
```

## Performance Monitoring

### Check Application Performance
```bash
# View detailed process info
pm2 show bizbio-frontend

# Monitor in real-time
pm2 monit

# Check logs for slow queries
pm2 logs bizbio-frontend | grep -i "slow"
```

### Check Network Performance
```bash
# Test response time
time curl http://localhost:3000

# Check via Cloudflare tunnel
time curl https://yourdomain.com

# Monitor network connections
netstat -an | grep :3000 | wc -l
```

## Backup Important Files

Create a backup script:
```bash
nano /var/www/bizbio-frontend/backup.sh
```

```bash
#!/bin/bash

BACKUP_DIR="/var/backups/bizbio-frontend"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

# Backup environment file
cp /var/www/bizbio-frontend/.env.production $BACKUP_DIR/.env.production.$DATE

# Backup PM2 logs
cp -r /var/www/bizbio-frontend/logs $BACKUP_DIR/logs.$DATE

echo "Backup completed: $DATE"
```

## Useful One-Liners

```bash
# Check if app is responding
curl -f http://localhost:3000 && echo "✅ App is up" || echo "❌ App is down"

# View last error
pm2 logs bizbio-frontend --err --lines 1 --nostream

# Count number of restarts
pm2 jlist | grep -o '"restarts":[0-9]*' | grep -o '[0-9]*'

# Check uptime
pm2 jlist | grep -o '"pm_uptime":[0-9]*' | grep -o '[0-9]*'

# Test full stack
curl -I https://yourdomain.com
```

---

**Pro Tip**: Set up a monitoring service like:
- [UptimeRobot](https://uptimerobot.com) (Free plan available)
- [Pingdom](https://www.pingdom.com)
- Cloudflare's built-in Health Checks

These will alert you via email/SMS if your site goes down.

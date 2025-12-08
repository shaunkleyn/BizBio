# PowerShell script to deploy locally-built application to VPS
# Run this from your local Windows machine

param(
    [string]$VpsUser = "root",
    
    [string]$VpsHost = "154.66.198.20",
    
    [string]$VpsPath = "/var/www/bizbio/ui"
)

Write-Host "🚀 Starting deployment to VPS..." -ForegroundColor Green
Write-Host "Target: $($VpsUser)@$($VpsHost):$VpsPath" -ForegroundColor Cyan

# Build the application locally
Write-Host "`n📦 Building application..." -ForegroundColor Yellow
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build completed successfully!" -ForegroundColor Green

# Copy .output directory to VPS
Write-Host "`n📤 Uploading .output directory..." -ForegroundColor Yellow
scp -P 7474 -r .output "${VpsUser}@${VpsHost}:${VpsPath}/"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to upload .output directory!" -ForegroundColor Red
    exit 1
}

# Copy .env.production if it exists
if (Test-Path .env.production) {
    Write-Host "`n📤 Uploading .env.production..." -ForegroundColor Yellow
    scp -P 7474 .env.production "${VpsUser}@${VpsHost}:${VpsPath}/"
} else {
    Write-Host "`n⚠️  Warning: .env.production not found!" -ForegroundColor Yellow
}

# Copy ecosystem.config.cjs
Write-Host "`n📤 Uploading ecosystem.config.cjs..." -ForegroundColor Yellow
scp -P 7474 ecosystem.config.cjs "${VpsUser}@${VpsHost}:${VpsPath}/"

# Copy package.json (needed for production dependencies)
Write-Host "`n📤 Uploading package.json..." -ForegroundColor Yellow
scp -P 7474 package.json "${VpsUser}@${VpsHost}:${VpsPath}/"
scp -P 7474 package-lock.json "${VpsUser}@${VpsHost}:${VpsPath}/"

# Execute remote commands to restart the application
Write-Host "`n🔄 Restarting application on VPS..." -ForegroundColor Yellow

Write-Host "  Installing dependencies..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "cd $VpsPath && npm install --production"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to install dependencies!" -ForegroundColor Red
    exit 1
}

Write-Host "  Restarting PM2 process..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "cd $VpsPath && pm2 restart bizbio-frontend || pm2 start ecosystem.config.cjs"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restart application!" -ForegroundColor Red
    exit 1
}

Write-Host "  Saving PM2 configuration..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "pm2 save"

Write-Host "`n✅ Deployment completed successfully!" -ForegroundColor Green

# Health checks
Write-Host "`n🔍 Running health checks..." -ForegroundColor Cyan

Write-Host "  Checking PM2 status..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "pm2 status bizbio-frontend"

Write-Host "`n  Verifying process state..." -ForegroundColor Gray
$processStatus = ssh -p 7474 "${VpsUser}@${VpsHost}" "pm2 jlist | grep -o '\"pm2_env\":{[^}]*\"status\":\"[^\"]*\"' | grep -o 'status\":\"[^\"]*\"' | cut -d'\"' -f3 | head -1"

if ($processStatus -eq "online") {
    Write-Host "  ✅ Process status: $processStatus" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Process status: $processStatus" -ForegroundColor Yellow
    Write-Host "  Checking logs for errors..." -ForegroundColor Yellow
    ssh -p 7474 "${VpsUser}@${VpsHost}" "pm2 logs bizbio-frontend --lines 20 --nostream"
}

Write-Host "`n  Testing HTTP endpoint..." -ForegroundColor Gray
$httpTest = ssh -p 7474 "${VpsUser}@${VpsHost}" "curl -s -o /dev/null -w '%{http_code}' http://localhost:3000 --max-time 10"

if ($httpTest -eq "200") {
    Write-Host "  ✅ HTTP health check passed (Status: $httpTest)" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  HTTP health check returned status: $httpTest" -ForegroundColor Yellow
    Write-Host "  The application may still be starting up..." -ForegroundColor Yellow
}

Write-Host "`n  Checking for recent errors..." -ForegroundColor Gray
$errorCount = ssh -p 7474 "${VpsUser}@${VpsHost}" "pm2 logs bizbio-frontend --lines 50 --nostream --err 2>/dev/null | grep -i 'error' | wc -l"

if ($errorCount -eq "0") {
    Write-Host "  ✅ No recent errors found" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Found $errorCount error line(s) in recent logs" -ForegroundColor Yellow
    Write-Host "  Run 'ssh -p 7474 ${VpsUser}@${VpsHost} pm2 logs bizbio-frontend' to view logs" -ForegroundColor Gray
}

Write-Host "`n🎉 All done!" -ForegroundColor Green
Write-Host "🌐 UI is available at: https://ui.bizbio.co.za" -ForegroundColor Cyan
Write-Host "`n💡 Useful commands:" -ForegroundColor Gray
Write-Host "   View logs:    ssh -p 7474 ${VpsUser}@${VpsHost} 'pm2 logs bizbio-frontend'" -ForegroundColor Gray
Write-Host "   Check status: ssh -p 7474 ${VpsUser}@${VpsHost} 'pm2 status'" -ForegroundColor Gray
Write-Host "   Restart:      ssh -p 7474 ${VpsUser}@${VpsHost} 'pm2 restart bizbio-frontend'" -ForegroundColor Gray

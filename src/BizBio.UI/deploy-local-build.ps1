# PowerShell script to deploy locally-built application to VPS
# Run this from your local Windows machine

param(
    [Parameter(Mandatory=$true)]
    [string]$VpsUser,
    
    [Parameter(Mandatory=$true)]
    [string]$VpsHost,
    
    [string]$VpsPath = "/var/www/bizbio/ui"
)

Write-Host "🚀 Starting deployment to VPS..." -ForegroundColor Green
Write-Host "Target: $VpsUser@$VpsHost:$VpsPath" -ForegroundColor Cyan

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
scp -r .output "${VpsUser}@${VpsHost}:${VpsPath}/"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to upload .output directory!" -ForegroundColor Red
    exit 1
}

# Copy .env.production if it exists
if (Test-Path .env.production) {
    Write-Host "`n📤 Uploading .env.production..." -ForegroundColor Yellow
    scp .env.production "${VpsUser}@${VpsHost}:${VpsPath}/"
} else {
    Write-Host "`n⚠️  Warning: .env.production not found!" -ForegroundColor Yellow
}

# Copy ecosystem.config.cjs
Write-Host "`n📤 Uploading ecosystem.config.cjs..." -ForegroundColor Yellow
scp ecosystem.config.cjs "${VpsUser}@${VpsHost}:${VpsPath}/"

# Copy package.json (needed for production dependencies)
Write-Host "`n📤 Uploading package.json..." -ForegroundColor Yellow
scp package.json "${VpsUser}@${VpsHost}:${VpsPath}/"
scp package-lock.json "${VpsUser}@${VpsHost}:${VpsPath}/"

# Execute remote commands to restart the application
Write-Host "`n🔄 Restarting application on VPS..." -ForegroundColor Yellow
ssh "${VpsUser}@${VpsHost}" @"
cd ${VpsPath}
npm install --production
pm2 restart bizbio-frontend || pm2 start ecosystem.config.cjs
pm2 save
"@

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host "📊 Checking application status..." -ForegroundColor Cyan
    ssh "${VpsUser}@${VpsHost}" "pm2 status bizbio-frontend"
} else {
    Write-Host "`n❌ Deployment failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 All done!" -ForegroundColor Green

# PowerShell script to deploy locally-built API to VPS
# Run this from your local Windows machine

param(
    [string]$VpsUser = "root",

    [string]$VpsHost = "154.66.198.20",

    [string]$VpsPath = "/var/www/bizbio/api",

    [string]$ServiceName = "bizbio-api"
)

Write-Host " Starting API deployment to VPS..." -ForegroundColor Green
Write-Host "Target: $($VpsUser)@$($VpsHost):$VpsPath" -ForegroundColor Cyan

# Build the application locally
Write-Host "`nBuilding .NET application..." -ForegroundColor Yellow
dotnet publish -c Release -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green

# Create backup on VPS
Write-Host "Creating backup on VPS..." -ForegroundColor Yellow
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"

# Stop the service
Write-Host "  Stopping service..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "sudo systemctl stop $ServiceName"

# Create backup
Write-Host "  Creating backup directory..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "mkdir -p $VpsPath/backup"

Write-Host "  Backing up current deployment..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "if [ -d '$VpsPath/current' ]; then sudo tar -czf $VpsPath/backup/api-backup-$timestamp.tar.gz -C $VpsPath current && echo 'Backup created'; else echo 'No current deployment found'; fi"

# Remove old current directory and create fresh one
Write-Host "  Preparing deployment directory..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "rm -rf $VpsPath/current && mkdir -p $VpsPath/current"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create backup!" -ForegroundColor Red
    exit 1
}

Write-Host "Backup created successfully!" -ForegroundColor Green

# Copy published files to VPS
Write-Host "`Uploading application files..." -ForegroundColor Yellow
scp -P 7474 -r ./publish/* "${VpsUser}@${VpsHost}:${VpsPath}/current/"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to upload files!" -ForegroundColor Red
    Write-Host "Attempting to restore from backup..." -ForegroundColor Yellow
    ssh -p 7474 "${VpsUser}@${VpsHost}" "cd $VpsPath && sudo tar -xzf backup/api-backup-$timestamp.tar.gz && sudo systemctl start $ServiceName"
    exit 1
}

Write-Host "Files uploaded successfully!" -ForegroundColor Green

# Set permissions and restart service
Write-Host "Setting permissions and restarting service..." -ForegroundColor Yellow

Write-Host "  Setting permissions..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "sudo chown -R www-data:www-data $VpsPath/current && sudo chmod -R 755 $VpsPath/current"

Write-Host "  Starting service..." -ForegroundColor Gray
ssh -p 7474 "${VpsUser}@${VpsHost}" "sudo systemctl start $ServiceName && sudo systemctl status $ServiceName --no-pager"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    Write-Host "Checking service status..." -ForegroundColor Cyan
    ssh -p 7474 "${VpsUser}@${VpsHost}" "sudo systemctl is-active $ServiceName"

    Write-Host "Cleaning up old backups (keeping last 5)..." -ForegroundColor Yellow
    ssh -p 7474 "${VpsUser}@${VpsHost}" "cd $VpsPath/backup && ls -t api-backup-*.tar.gz 2>/dev/null | tail -n +6 | xargs -r rm && echo 'Cleanup completed'"
} else {
    Write-Host "Deployment failed!" -ForegroundColor Red
    Write-Host "Service may not have started. Check logs with:" -ForegroundColor Yellow
    Write-Host "   ssh -p 7474 ${VpsUser}@${VpsHost} 'sudo journalctl -u $ServiceName -n 50'" -ForegroundColor Yellow
    exit 1
}

# Clean up local publish directory
Write-Host "`n🧹 Cleaning up local publish directory..." -ForegroundColor Yellow
Remove-Item -Path ./publish -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "`n🎉 All done!" -ForegroundColor Green
Write-Host "API is now running at: https://api.bizbio.co.za" -ForegroundColor Cyan

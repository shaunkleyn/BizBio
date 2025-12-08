# BizBio Deployment Scripts

This directory contains deployment scripts for the BizBio API.

## Scripts Overview

### 1. `deploy.sh`
Server-side deployment script that runs on the VPS. Used by:
- Azure DevOps pipeline (automated)
- Local deployment script
- Manual deployments on the server

**Usage**:
```bash
# On VPS server
sudo ./deploy.sh
```

### 2. `deploy-local.sh`
Local deployment script for deploying from your development machine.

**Usage**:
```bash
# From project root directory
chmod +x deploy/deploy-local.sh
./deploy/deploy-local.sh
```

**Environment Variables**:
```bash
export VPS_HOST="169.239.218.60"        # VPS IP address
export VPS_USER="root"                   # SSH user
export SSH_KEY="$HOME/.ssh/id_rsa"      # Path to SSH private key
```

## Local Deployment Instructions

### Prerequisites
1. .NET 8 SDK installed
2. SSH access to VPS
3. SSH key configured (`~/.ssh/id_rsa` or custom path)

### Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/shaunkleyn/BizBio.git
   cd BizBio
   ```

2. **Configure environment variables** (optional):
   ```bash
   export VPS_HOST="your-vps-ip"
   export VPS_USER="your-user"
   export SSH_KEY="path-to-your-ssh-key"
   ```

3. **Run deployment script**:
   ```bash
   chmod +x deploy/deploy-local.sh
   ./deploy/deploy-local.sh
   ```

4. **Confirm deployment**:
   - The script will ask for confirmation
   - Type `yes` to proceed

5. **Monitor progress**:
   - The script will show each deployment step
   - Wait for completion message

6. **Verify deployment**:
   - Check API: https://api.bizbio.co.za/health
   - Check Swagger: https://api.bizbio.co.za/index.html

## What the Scripts Do

### Build Process
1. Clean previous builds
2. Restore NuGet packages
3. Build solution in Release configuration
4. Publish API project
5. Create deployment ZIP package

### Deployment Process
1. Stop the `bizbio-api` service
2. Create timestamped backup of current deployment
3. Extract new version to `/var/www/bizbio-api`
4. Set proper permissions (`www-data:www-data`)
5. Ensure uploads directory exists
6. (Optional) Run database migrations
7. Start the service
8. Verify service is running
9. Test health endpoint
10. Clean up temporary files

### Rollback on Failure
If deployment fails:
1. Automatically stops the service
2. Restores the most recent backup
3. Restarts the service

## Manual Deployment (Without Script)

If you prefer to deploy manually:

```bash
# 1. Build locally
dotnet publish src/BizBio.API/BizBio.API.csproj -c Release -o ./publish

# 2. Create package
cd publish
zip -r ../BizBio.API.zip .
cd ..

# 3. Upload to VPS
scp BizBio.API.zip root@169.239.218.60:/tmp/

# 4. Deploy on VPS
ssh root@169.239.218.60
sudo systemctl stop bizbio-api
sudo cp -r /var/www/bizbio-api /var/www/bizbio-api.backup.$(date +%Y%m%d_%H%M%S)
sudo unzip -o /tmp/BizBio.API.zip -d /var/www/bizbio-api/
sudo chown -R www-data:www-data /var/www/bizbio-api
sudo systemctl start bizbio-api
```

## Troubleshooting

### SSH Connection Issues

**Problem**: Cannot connect to VPS

**Solution**:
```bash
# Test SSH connection
ssh -i ~/.ssh/id_rsa root@169.239.218.60

# If it fails, check:
# 1. SSH key is correct
# 2. Public key is in VPS ~/.ssh/authorized_keys
# 3. SSH service is running on VPS
# 4. Firewall allows SSH (port 22)
```

### Build Errors

**Problem**: Build fails

**Solution**:
```bash
# Ensure .NET 8 SDK is installed
dotnet --version

# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Deployment Fails on VPS

**Problem**: Service won't start after deployment

**Solution**:
```bash
# Check service status
sudo systemctl status bizbio-api

# Check logs
sudo journalctl -u bizbio-api -n 100

# Common issues:
# - Port 5000 already in use
# - Missing appsettings.json
# - Database connection issues
# - Permission problems
```

### Rollback

If you need to manually rollback:

```bash
# On VPS
ssh root@169.239.218.60

# List backups
ls -la /var/www/ | grep bizbio-api.backup

# Restore backup
sudo systemctl stop bizbio-api
sudo rm -rf /var/www/bizbio-api
sudo cp -r /var/www/bizbio-api.backup.YYYYMMDD_HHMMSS /var/www/bizbio-api
sudo systemctl start bizbio-api
```

## Security Notes

1. **Never commit SSH keys** to the repository
2. **Use environment variables** for sensitive data
3. **Rotate SSH keys** regularly
4. **Limit SSH access** to specific IPs when possible
5. **Review deployment logs** after each deployment

## Azure DevOps Pipeline

For automated deployments using Azure DevOps, see:
- [Azure DevOps Setup Guide](../AZURE_DEVOPS_SETUP.md)
- [Pipeline Configuration](../azure-pipelines.yml)

## Support

If you encounter issues:
1. Check the troubleshooting section above
2. Review VPS logs: `sudo journalctl -u bizbio-api -f`
3. Verify service status: `sudo systemctl status bizbio-api`
4. Test SSH connection: `ssh root@169.239.218.60`

---

**Last Updated**: December 2025

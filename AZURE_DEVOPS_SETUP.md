# Azure DevOps Pipeline Setup Guide

This guide explains how to set up the Azure DevOps CI/CD pipeline for the BizBio API.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Pipeline Overview](#pipeline-overview)
3. [Initial Setup](#initial-setup)
4. [Configure Variables](#configure-variables)
5. [Create Environments](#create-environments)
6. [Setup SSH Access](#setup-ssh-access)
7. [Run the Pipeline](#run-the-pipeline)
8. [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before setting up the pipeline, ensure you have:

- ✅ Azure DevOps account and organization
- ✅ BizBio project created in Azure DevOps
- ✅ Repository synced from GitHub to Azure DevOps (currently done via GitHub Actions)
- ✅ SSH access to your VPS
- ✅ .NET 8 SDK installed on your VPS
- ✅ Systemd service configured (`bizbio-api`)

---

## Pipeline Overview

The pipeline consists of 3 stages:

1. **Build Stage**: Compiles the .NET application and creates deployment artifacts
2. **Deploy Dev Stage**: Deploys to development environment (triggers on `develop` branch)
3. **Deploy Prod Stage**: Deploys to production environment (triggers on `main` branch)

### Workflow

```
┌─────────────┐
│   Commit    │
│  to Branch  │
└──────┬──────┘
       │
       v
┌─────────────────────┐
│   Build & Test      │
│  - Restore deps     │
│  - Build solution   │
│  - Run tests        │
│  - Create artifacts │
└──────┬──────────────┘
       │
       ├──────────────────────────┐
       │                          │
       v                          v
┌──────────────┐         ┌──────────────┐
│  Deploy Dev  │         │ Deploy Prod  │
│  (develop)   │         │   (main)     │
└──────────────┘         └──────────────┘
```

---

## Initial Setup

### 1. Create the Pipeline

1. Navigate to your Azure DevOps project: `https://dev.azure.com/<your-org>/BizBio`
2. Go to **Pipelines** > **Pipelines**
3. Click **New Pipeline**
4. Select **Azure Repos Git**
5. Select your **BizBio** repository
6. Choose **Existing Azure Pipelines YAML file**
7. Select the `azure-pipelines.yml` file from the repository
8. Click **Continue**

### 2. Review the Pipeline

Review the YAML configuration and click **Save** (don't run yet - we need to configure variables first)

---

## Configure Variables

The pipeline requires several variables for deployment. You should use **Variable Groups** for better organization and security.

### Create Variable Groups

#### 1. Create "BizBio-Common" Variable Group

Navigate to **Pipelines** > **Library** > **+ Variable group**

**Group Name**: `BizBio-Common`

Add these variables:

| Variable Name | Value | Secret? | Description |
|--------------|-------|---------|-------------|
| `VPS_HOST` | `169.239.218.60` | No | Your VPS IP address |
| `VPS_USER` | `root` (or your user) | No | SSH user for deployment |
| `SSH_PRIVATE_KEY` | `[Your SSH private key]` | **YES** | SSH private key for VPS access |

#### 2. Create "BizBio-Development" Variable Group

**Group Name**: `BizBio-Development`

Add these variables (for dev/staging environment):

| Variable Name | Value | Secret? |
|--------------|-------|---------|
| `ConnectionStrings__DefaultConnection` | `[Dev DB connection string]` | **YES** |
| `JWT__Secret` | `[Dev JWT secret]` | **YES** |
| `Email__SmtpPassword` | `[SMTP password]` | **YES** |
| `PayFast__MerchantKey` | `[PayFast sandbox key]` | **YES** |

#### 3. Create "BizBio-Production" Variable Group

**Group Name**: `BizBio-Production`

Add these variables (for production environment):

| Variable Name | Value | Secret? |
|--------------|-------|---------|
| `ConnectionStrings__DefaultConnection` | `[Prod DB connection string]` | **YES** |
| `JWT__Secret` | `[Prod JWT secret]` | **YES** |
| `Email__SmtpPassword` | `[SMTP password]` | **YES** |
| `PayFast__MerchantKey` | `[PayFast production key]` | **YES** |

### Link Variable Groups to Pipeline

1. Edit the `azure-pipelines.yml` file
2. Add variable groups at the top level (after the `variables:` section):

```yaml
variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.x'
  # ... other variables

# Add these variable groups
- group: BizBio-Common
```

3. For each deployment stage, reference the appropriate environment variable group:

```yaml
- stage: DeployDev
  variables:
  - group: BizBio-Common
  - group: BizBio-Development
```

---

## Create Environments

Environments provide deployment history, approvals, and checks.

### 1. Create Development Environment

1. Go to **Pipelines** > **Environments**
2. Click **New environment**
3. Name: `Development`
4. Description: "Development/Staging environment"
5. Resource: None
6. Click **Create**

### 2. Create Production Environment

1. Click **New environment**
2. Name: `Production`
3. Description: "Production environment"
4. Resource: None
5. Click **Create**

### 3. Configure Production Approvals

To add manual approval before production deployment:

1. Select the **Production** environment
2. Click the "..." menu > **Approvals and checks**
3. Click **+** > **Approvals**
4. Add approvers (your email or team members)
5. Configure:
   - **Approvers**: Select users/groups
   - **Timeout**: 30 days
   - **Instructions**: "Please review and approve production deployment"
6. Click **Create**

Now every production deployment will require manual approval!

---

## Setup SSH Access

The pipeline deploys to your VPS via SSH. You need to generate and configure SSH keys.

### 1. Generate SSH Key Pair (if you don't have one)

On your local machine or Azure DevOps agent:

```bash
ssh-keygen -t rsa -b 4096 -C "azure-devops@bizbio" -f ~/.ssh/bizbio_deploy_key -N ""
```

This creates:
- Private key: `~/.ssh/bizbio_deploy_key`
- Public key: `~/.ssh/bizbio_deploy_key.pub`

### 2. Add Public Key to VPS

Copy the public key to your VPS:

```bash
ssh-copy-id -i ~/.ssh/bizbio_deploy_key.pub root@169.239.218.60
```

Or manually:

```bash
# On VPS
mkdir -p ~/.ssh
chmod 700 ~/.ssh

# Add the public key content to authorized_keys
echo "your-public-key-content-here" >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
```

### 3. Test SSH Connection

```bash
ssh -i ~/.ssh/bizbio_deploy_key root@169.239.218.60
```

### 4. Add Private Key to Azure DevOps

1. Copy your private key content:
   ```bash
   cat ~/.ssh/bizbio_deploy_key
   ```

2. Go to **Pipelines** > **Library** > **BizBio-Common** variable group
3. Edit the `SSH_PRIVATE_KEY` variable
4. Paste the **entire private key** (including `-----BEGIN` and `-----END` lines)
5. Check **Keep this value secret**
6. Click **Save**

---

## Run the Pipeline

### First Run

1. Go to **Pipelines** > **Pipelines**
2. Select your **BizBio** pipeline
3. Click **Run pipeline**
4. Select the branch:
   - `develop` for development deployment
   - `main` for production deployment
5. Click **Run**

### Monitor the Pipeline

1. Click on the running pipeline to see live logs
2. Each stage will show progress:
   - ✅ Build: Compiling and creating artifacts
   - ⏳ DeployDev/DeployProd: Deploying to VPS
3. If deployment fails, check the logs for errors

### Automatic Triggers

The pipeline will automatically trigger on:
- Commits to `main` → Build + Deploy to Production
- Commits to `develop` → Build + Deploy to Development
- Commits to `feature/*` → Build only (no deployment)
- Pull Requests → Build only

---

## Pipeline Configuration Details

### Deployment Process

The pipeline performs these steps on the VPS:

1. **Stop Service**: `sudo systemctl stop bizbio-api`
2. **Backup**: Creates timestamped backup in `/var/www/bizbio-api.backup.YYYYMMDD_HHMMSS`
3. **Deploy**: Extracts new files to `/var/www/bizbio-api`
4. **Permissions**: Sets ownership to `www-data:www-data`
5. **Migrations**: (Optional) Runs EF Core migrations
6. **Start Service**: `sudo systemctl start bizbio-api`
7. **Health Check**: Tests `https://api.bizbio.co.za/health`

### Directory Structure on VPS

```
/var/www/
├── bizbio-api/              # Current deployment
├── bizbio-api.backup.*/     # Previous deployments (last 5 kept)
└── uploads/                 # File uploads
```

---

## Troubleshooting

### Build Fails

**Problem**: Build fails with "Project not found"

**Solution**:
- Check `projectPath` and `solutionPath` variables in `azure-pipelines.yml`
- Ensure the paths match your repository structure

### SSH Connection Failed

**Problem**: `Permission denied (publickey)`

**Solution**:
1. Verify SSH key is correct in Azure DevOps variable
2. Check public key is in VPS `~/.ssh/authorized_keys`
3. Test SSH connection manually: `ssh -i key_file user@host`

### Service Won't Start

**Problem**: Deployment succeeds but service doesn't start

**Solution**:
1. SSH into VPS: `ssh root@169.239.218.60`
2. Check service status: `sudo systemctl status bizbio-api`
3. Check logs: `sudo journalctl -u bizbio-api -n 100 --no-pager`
4. Common issues:
   - Port already in use: `sudo netstat -tulpn | grep :5000`
   - Permission issues: Check `/var/www/bizbio-api` permissions
   - Missing dependencies: Ensure .NET 8 is installed

### Health Check Fails

**Problem**: Deployment completes but health check fails

**Solution**:
1. Check if API is running: `curl http://localhost:5000/health`
2. Check Cloudflare/proxy settings
3. Verify SSL certificate is valid
4. Check firewall rules

### Permission Denied Errors

**Problem**: `sudo: a password is required`

**Solution**:
Configure passwordless sudo for deployment user:

```bash
# On VPS
sudo visudo

# Add this line (replace 'root' with your user)
root ALL=(ALL) NOPASSWD: /bin/systemctl start bizbio-api, /bin/systemctl stop bizbio-api, /bin/systemctl status bizbio-api
```

---

## Security Best Practices

1. **Never commit secrets** to the repository
2. **Use variable groups** for all sensitive data
3. **Mark variables as secret** in Azure DevOps
4. **Use SSH keys** instead of passwords
5. **Rotate SSH keys** periodically
6. **Limit SSH access** to specific IPs if possible
7. **Enable production approvals** to prevent accidental deployments
8. **Review deployment logs** regularly

---

## Advanced Configuration

### Database Migrations

To run EF Core migrations automatically, uncomment these lines in the deployment script:

```bash
cd /var/www/bizbio-api
sudo -u www-data dotnet BizBio.API.dll ef database update
```

Or run migrations manually in the pipeline before restarting the service.

### Multiple Environments

To add a staging environment:

1. Create a new stage in `azure-pipelines.yml` (copy `DeployDev`)
2. Change the condition to match your staging branch
3. Create a `BizBio-Staging` variable group
4. Create a `Staging` environment in Azure DevOps

### Rollback Strategy

To rollback a deployment:

```bash
# On VPS
sudo systemctl stop bizbio-api
sudo rm -rf /var/www/bizbio-api
sudo cp -r /var/www/bizbio-api.backup.YYYYMMDD_HHMMSS /var/www/bizbio-api
sudo systemctl start bizbio-api
```

Or create a rollback pipeline that restores from backup.

---

## Monitoring and Notifications

### Setup Email Notifications

1. Go to **Project Settings** > **Notifications**
2. Create subscription:
   - **Build completes**
   - **Release deployment approval pending**
   - **Release deployment completed**
   - **Release deployment failed**

### Integration with Slack/Teams

Add notification tasks to the pipeline:

```yaml
- task: Slack@1
  inputs:
    Message: 'Deployment to Production completed!'
    WebhookUrl: '$(SLACK_WEBHOOK_URL)'
```

---

## Next Steps

1. ✅ Set up the pipeline following this guide
2. ✅ Test with a feature branch first
3. ✅ Deploy to development
4. ✅ Review and approve production deployment
5. ✅ Monitor application health
6. 🔄 Iterate and improve the pipeline

---

## Support

If you encounter issues:

1. Check the [Troubleshooting](#troubleshooting) section
2. Review Azure DevOps pipeline logs
3. Check VPS logs: `sudo journalctl -u bizbio-api -f`
4. Verify configuration in Azure DevOps variable groups

---

**Last Updated**: December 2025
**Pipeline Version**: 1.0

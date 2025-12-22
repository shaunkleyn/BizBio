# Azure Pipelines Setup for BizBio.UI

This folder contains Azure Pipelines YAML configuration files for building and deploying the BizBio UI application.

## Available Pipeline Files

### 1. `azure-pipelines.yml` (Full Pipeline)
A comprehensive pipeline with:
- Build stage with caching
- Automated testing support
- Artifact publishing
- Deployment stage (production)
- Configurable for different environments

### 2. `azure-pipelines-simple.yml` (Basic Pipeline)
A simplified build-only pipeline:
- Basic build configuration
- No deployment
- Good for testing or development branches

## Setup Instructions

### 1. Choose Your Pipeline

**For Production Use:**
Use `azure-pipelines.yml` - rename it or create a pipeline pointing to it.

**For Simple Builds:**
Use `azure-pipelines-simple.yml` for basic CI/CD.

### 2. Configure Azure DevOps

1. Go to your Azure DevOps project
2. Navigate to **Pipelines** → **New Pipeline**
3. Select **Azure Repos Git** (or your repository source)
4. Select **Existing Azure Pipelines YAML file**
5. Choose either:
   - `/src/BizBio.UI/azure-pipelines.yml` (full)
   - `/src/BizBio.UI/azure-pipelines-simple.yml` (simple)

### 3. Configure Variables (Optional)

For the full pipeline, you may want to set these variables in Azure DevOps:

- `nodeVersion`: Node.js version (default: 18.x)
- `buildConfiguration`: Build configuration (default: Release)

### 4. Set Up Deployment (Full Pipeline Only)

The full pipeline includes a deployment stage. Configure it based on your hosting:

#### For Azure Static Web Apps:
Uncomment the Azure Static Web Apps task and add:
```yaml
- task: AzureStaticWebApp@0
  inputs:
    app_location: '$(Pipeline.Workspace)/deploy'
    azure_static_web_apps_api_token: $(AZURE_STATIC_WEB_APPS_API_TOKEN)
```

Add the `AZURE_STATIC_WEB_APPS_API_TOKEN` variable in your pipeline settings.

#### For Azure App Service:
Uncomment the Azure Web App task and configure:
```yaml
- task: AzureWebApp@1
  inputs:
    azureSubscription: 'YourAzureSubscription'
    appName: 'bizbio-ui'
    package: '$(Pipeline.Workspace)/ui-build/*.zip'
```

#### For Other Hosting (Netlify, Vercel, etc.):
Replace the deployment steps with your provider's CLI commands.

## Pipeline Features

### Build Stage
- ✅ Node.js 18.x setup
- ✅ Dependency caching for faster builds
- ✅ Clean install (`npm ci`) for consistent builds
- ✅ Nuxt 3 production build
- ✅ Build artifact archiving

### Optional Features (Commented Out)
- Testing (`npm run test`)
- Linting (`npm run lint`)

### Deployment Stage (Full Pipeline)
- Triggered only on main branch
- Requires manual approval via environment gates
- Extracts and deploys build artifacts

## Triggers

### Branch Triggers
- `main` - Full build and deployment
- `develop` - Build only
- `feature/*` - Build only

### Path Triggers
Only triggers when files in `src/BizBio.UI/**` are modified.

### Pull Request Triggers
Runs on PRs targeting `main` or `develop` branches.

## Build Artifacts

The pipeline produces a zip file containing:
- `.output/` folder from Nuxt build
- All necessary production files

Artifact naming: `bizbio-ui-{BuildId}.zip`

## Environment Configuration

For deployment to work, create an environment in Azure DevOps:

1. Go to **Pipelines** → **Environments**
2. Create new environment: `production`
3. Add approvals and checks as needed

## Troubleshooting

### Build Fails at npm ci
- Ensure `package-lock.json` is committed
- Check Node.js version compatibility

### Cache Not Working
- Clear pipeline cache in Azure DevOps
- Check cache key configuration

### Deployment Fails
- Verify service connection is configured
- Check environment permissions
- Ensure deployment credentials are set

## Local Testing

Before pushing, test the build locally:

```bash
cd src/BizBio.UI
npm ci
npm run build
```

The `.output` folder should be created successfully.

## Additional Resources

- [Azure Pipelines Documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- [Nuxt 3 Deployment Guide](https://nuxt.com/docs/getting-started/deployment)
- [Node.js Task Reference](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/tool/node-js)

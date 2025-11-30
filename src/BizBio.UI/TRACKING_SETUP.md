# Analytics & Tracking Setup Instructions

## Installation

### 1. Install Dependencies

Run the following command to install the required packages:

```bash
npm install
```

This will install:
- `@microsoft/applicationinsights-web` - Azure Application Insights SDK
- `vue-gtag` - Google Analytics for Vue 3 (included as reference)

### 2. Configure Environment Variables

#### For Production (on your VPS)

Create or update `.env.production` on your server:

```bash
# API Configuration
NUXT_PUBLIC_API_URL=https://api.bizbio.co.za/api/v1

# Node Environment
NODE_ENV=production

# Application Port
PORT=3000

# Application Insights Configuration
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=your-key-here;IngestionEndpoint=https://your-region.applicationinsights.azure.com/

# Google Analytics Configuration
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-XXXXXXXXXX

# Cookie Domain (optional)
NUXT_PUBLIC_COOKIE_DOMAIN=.bizbio.co.za
```

#### For Development

Create `.env` in your local project (optional for testing):

```bash
# API Configuration
NUXT_PUBLIC_API_URL=http://localhost:5000/api

# Node Environment
NODE_ENV=development

# Application Port
PORT=3000

# Leave these empty to disable tracking in development
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=
```

### 3. Azure Application Insights Setup

#### Create Application Insights Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **Create a resource**
3. Search for **Application Insights**
4. Click **Create**
5. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or use existing
   - **Name**: `bizbio-frontend` (or your preferred name)
   - **Region**: Choose closest to your users
   - **Resource Mode**: Workspace-based (recommended)
6. Click **Review + Create**, then **Create**

#### Get Connection String

1. Once created, go to your Application Insights resource
2. In the left menu, click **Properties** (under Configure)
3. Copy the **Connection String** (not Instrumentation Key)
4. Add it to your `.env.production` file:
   ```
   NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=xxx;IngestionEndpoint=https://xxx.applicationinsights.azure.com/
   ```

### 4. Google Analytics Setup

#### Create GA4 Property

1. Go to [Google Analytics](https://analytics.google.com)
2. Click **Admin** (bottom left)
3. In the Property column, click **Create Property**
4. Fill in property details:
   - **Property name**: BizBio Frontend
   - **Reporting time zone**: Your timezone
   - **Currency**: Your currency
5. Click **Next** and fill in business information
6. Click **Create**

#### Create Data Stream

1. After creating property, you'll be prompted to create a data stream
2. Select **Web**
3. Fill in:
   - **Website URL**: https://bizbio.co.za (or your domain)
   - **Stream name**: BizBio Website
4. Click **Create stream**
5. Copy the **Measurement ID** (format: G-XXXXXXXXXX)
6. Add it to your `.env.production` file:
   ```
   NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-XXXXXXXXXX
   ```

### 5. Deploy to VPS

#### Option A: Using the deployment script

```bash
# On your VPS
cd /var/www/bizbio-frontend
git pull origin main
npm run build
npm run pm2:restart
```

#### Option B: Manual deployment

```bash
# Build locally
npm run build

# Copy .output folder to server
scp -r .output user@your-server:/var/www/bizbio-frontend/

# Copy .env.production to server
scp .env.production user@your-server:/var/www/bizbio-frontend/.env

# On server, restart PM2
ssh user@your-server
cd /var/www/bizbio-frontend
pm2 restart bizbio-frontend
```

### 6. Verify Installation

#### Check Application Insights

1. Navigate to your site in a browser
2. Perform some actions (navigate pages, submit forms, etc.)
3. Wait 2-5 minutes for data to appear
4. Go to Azure Portal > Your Application Insights resource
5. Click **Live Metrics** to see real-time data
6. Click **Transaction search** to see recent events

#### Check Google Analytics

1. Navigate to your site in a browser
2. Go to Google Analytics > Reports > Realtime
3. You should see your session appear within seconds
4. Standard reports can take 24-48 hours to populate

#### Check Browser Console

Open browser developer tools (F12) and check for:
- No errors related to Application Insights or gtag
- Network requests to `dc.services.visualstudio.com` (App Insights)
- Network requests to `google-analytics.com` (GA4)

### 7. Testing Tracking

You can test tracking in the browser console:

```javascript
// Test Application Insights
window.$nuxt.$trackEvent('test_event', { test: true })

// Test Google Analytics
window.gtag('event', 'test_event', { test: true })
```

## Common Issues & Solutions

### Issue: "Cannot find module '@microsoft/applicationinsights-web'"

**Solution**: Run `npm install` to install all dependencies.

### Issue: "Tracking not working in development"

**Solution**: This is expected if you haven't set the connection strings in your `.env` file. Set them if you want to test tracking locally.

### Issue: "Data not appearing in Application Insights"

**Solutions**:
1. Wait 2-5 minutes - there's a delay in data ingestion
2. Check the connection string is correct
3. Check browser console for errors
4. Verify the Application Insights resource exists in Azure
5. Check if ad blockers are blocking requests

### Issue: "Data not appearing in Google Analytics"

**Solutions**:
1. Check Realtime reports (data appears immediately there)
2. Verify the Measurement ID is correct
3. Check browser console for errors
4. Verify ad blockers aren't blocking Google Analytics
5. Standard reports take 24-48 hours to populate

### Issue: "TypeScript errors in IDE"

**Solution**: The TypeScript declarations are in `types/tracking.d.ts`. Restart your IDE or TypeScript server. The errors won't affect runtime.

## Monitoring Dashboard Setup

### Application Insights Dashboards

1. Go to Azure Portal > Your Application Insights resource
2. Click **Workbooks** in the left menu
3. Create custom workbooks for:
   - User engagement metrics
   - API performance
   - Error tracking
   - User journey analysis

Example queries for dashboards:

```kusto
// Top events
customEvents
| where timestamp > ago(24h)
| summarize count() by name
| order by count_ desc
| take 10

// Failed API calls
dependencies
| where timestamp > ago(24h)
| where success == false
| summarize count() by name, resultCode
| order by count_ desc

// Exceptions
exceptions
| where timestamp > ago(24h)
| project timestamp, problemId, outerMessage
| order by timestamp desc
```

### Google Analytics Dashboards

1. Go to Google Analytics > Explore
2. Create custom explorations for:
   - User acquisition
   - Engagement paths
   - Event analysis
   - Conversion funnels

## Best Practices

1. **Test First**: Test tracking in a staging environment before production
2. **Monitor Regularly**: Set up alerts for critical errors
3. **Review Privacy**: Ensure GDPR/privacy compliance
4. **Document Events**: Keep track of custom events you're tracking
5. **Regular Audits**: Periodically review what you're tracking

## Additional Resources

- [Application Insights Overview](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [GA4 Setup Guide](https://support.google.com/analytics/answer/9304153)
- [Nuxt 3 Documentation](https://nuxt.com/docs)

## Support

If you encounter issues:

1. Check the browser console for errors
2. Verify environment variables are set correctly
3. Check network tab for failed requests
4. Review the TRACKING.md file for usage examples
5. Check Azure Portal / Google Analytics for data ingestion issues

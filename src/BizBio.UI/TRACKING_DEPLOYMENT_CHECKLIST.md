# Analytics Deployment Checklist

## Pre-Deployment

### Azure Application Insights Setup
- [ ] Create Azure account (if not already have one)
- [ ] Create Application Insights resource
  - [ ] Choose appropriate region (closest to users)
  - [ ] Note down the resource name
  - [ ] Copy connection string from Properties
- [ ] Test connection string format: `InstrumentationKey=xxx;IngestionEndpoint=https://xxx.applicationinsights.azure.com/`
- [ ] Save connection string securely

### Google Analytics Setup
- [ ] Create Google Analytics account (if not already have one)
- [ ] Create GA4 property
  - [ ] Set timezone and currency
  - [ ] Fill in business information
- [ ] Create Web data stream
  - [ ] Enter your domain URL
  - [ ] Copy Measurement ID (G-XXXXXXXXXX format)
- [ ] Save Measurement ID securely

## Server Configuration

### Environment Variables
- [ ] SSH into your VPS server
- [ ] Navigate to application directory: `cd /var/www/bizbio-frontend`
- [ ] Create/edit `.env.production` file:
  ```bash
  nano .env.production
  ```
- [ ] Add the following variables:
  ```bash
  NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=your-connection-string-here
  NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-XXXXXXXXXX
  ```
- [ ] Save and exit (Ctrl+X, Y, Enter)
- [ ] Verify file was saved: `cat .env.production`

### Build & Deploy
- [ ] Pull latest code: `git pull origin main`
- [ ] Install dependencies: `npm install`
- [ ] Build application: `npm run build`
- [ ] Restart PM2: `pm2 restart bizbio-frontend`
- [ ] Check PM2 status: `pm2 status`
- [ ] Check logs for errors: `pm2 logs bizbio-frontend --lines 50`

## Verification

### Browser Testing
- [ ] Open your website in a browser
- [ ] Open Developer Tools (F12)
- [ ] Check Console tab - no tracking errors
- [ ] Check Network tab:
  - [ ] Look for requests to `dc.services.visualstudio.com` (App Insights)
  - [ ] Look for requests to `google-analytics.com` (GA4)
- [ ] Navigate to a few pages
- [ ] Perform some actions (click buttons, submit forms)

### Application Insights Verification
- [ ] Go to Azure Portal
- [ ] Navigate to your Application Insights resource
- [ ] Click "Live Metrics" in left menu
- [ ] You should see:
  - [ ] Incoming request rate
  - [ ] Live page views
  - [ ] Server requests
  - [ ] Failed requests (should be 0)
- [ ] Wait 2-5 minutes
- [ ] Click "Transaction search" in left menu
- [ ] You should see recent events:
  - [ ] Page views
  - [ ] Custom events
  - [ ] Dependencies (API calls)

### Google Analytics Verification
- [ ] Go to Google Analytics
- [ ] Select your property
- [ ] Go to Reports > Realtime
- [ ] You should see:
  - [ ] Active users (yourself)
  - [ ] Page views
  - [ ] Events
- [ ] Keep browser open and watch it update in real-time

### Test Specific Features
- [ ] Test page view tracking:
  - [ ] Navigate to different pages
  - [ ] Check if they appear in Live Metrics / Realtime
- [ ] Test event tracking (if implemented):
  - [ ] Click tracked buttons
  - [ ] Submit forms
  - [ ] Perform search
- [ ] Test error tracking:
  - [ ] Trigger a test error (if you have a test endpoint)
  - [ ] Check if it appears in Application Insights Failures

## Post-Deployment

### Dashboard Setup
- [ ] Create Application Insights dashboard
  - [ ] Add key metrics (page views, users, errors)
  - [ ] Add performance charts
  - [ ] Add error rate chart
- [ ] Create Google Analytics dashboard
  - [ ] Add user acquisition
  - [ ] Add engagement metrics
  - [ ] Add conversion tracking

### Alert Configuration
- [ ] Set up Application Insights alerts:
  - [ ] Alert on high error rate
  - [ ] Alert on slow API responses
  - [ ] Alert on exceptions
- [ ] Set up GA4 alerts (optional):
  - [ ] Traffic anomalies
  - [ ] Conversion drops

### Documentation
- [ ] Document your connection strings (securely)
- [ ] Share dashboard links with team
- [ ] Document custom events being tracked
- [ ] Create runbook for troubleshooting

## Ongoing Maintenance

### Daily
- [ ] Check for errors in Application Insights
- [ ] Monitor alert notifications

### Weekly
- [ ] Review key metrics in dashboards
- [ ] Check for anomalies in traffic/errors
- [ ] Review top events and pages

### Monthly
- [ ] Analyze user behavior patterns
- [ ] Review and optimize tracked events
- [ ] Check for new tracking opportunities
- [ ] Review and update documentation

## Rollback Plan

If tracking causes issues:

1. **Quick Fix - Disable Tracking**
   ```bash
   # On server
   cd /var/www/bizbio-frontend
   nano .env.production
   # Comment out or remove tracking variables
   pm2 restart bizbio-frontend
   ```

2. **Revert Code**
   ```bash
   git revert HEAD
   npm run build
   pm2 restart bizbio-frontend
   ```

## Troubleshooting

### Issue: Tracking not working
- [ ] Verify environment variables are set
- [ ] Check for typos in connection strings
- [ ] Verify PM2 restarted successfully
- [ ] Check browser console for errors
- [ ] Verify network requests are being sent

### Issue: Data not appearing in dashboards
- [ ] Wait 2-5 minutes for Application Insights
- [ ] Check Realtime reports for Google Analytics
- [ ] Verify connection strings are correct
- [ ] Check if ad blockers are interfering
- [ ] Review PM2 logs for errors

### Issue: High error rate
- [ ] Check Application Insights Failures
- [ ] Review error messages
- [ ] Check if connection strings are valid
- [ ] Verify Azure resource is active
- [ ] Check PM2 logs

## Success Criteria

✅ Your deployment is successful when:
1. No errors in browser console related to tracking
2. Live Metrics shows data in Application Insights
3. Realtime reports show activity in Google Analytics
4. Page views are being tracked
5. API calls appear as dependencies
6. No alerts triggered
7. Dashboards are accessible and showing data

## Contact & Support

- **Azure Support**: [Azure Portal Support](https://portal.azure.com/#blade/Microsoft_Azure_Support/HelpAndSupportBlade)
- **GA Support**: [Google Analytics Help](https://support.google.com/analytics/)
- **Documentation**: Check TRACKING.md, TRACKING_SETUP.md, and TRACKING_QUICKREF.md

---

## Notes

- Keep connection strings secure (never commit to git)
- Monitor costs in Azure Portal (Application Insights usage)
- Review privacy policy to ensure compliance
- Consider implementing cookie consent banner
- Test thoroughly before announcing to users

## Deployment Date: _______________

Deployed by: _______________

Verified by: _______________

# Analytics & Tracking - Quick Reference

## Quick Setup Checklist

- [ ] Install dependencies: `npm install`
- [ ] Create Application Insights resource in Azure
- [ ] Create Google Analytics 4 property
- [ ] Update `.env.production` with connection strings
- [ ] Deploy to server
- [ ] Verify tracking is working

## Environment Variables Required

```bash
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=...;IngestionEndpoint=https://...
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-XXXXXXXXXX
```

## Common Tracking Patterns

### Track Page View
```javascript
const tracking = useTracking()
tracking.trackPageView('/page-path', 'Page Title')
```

### Track Event
```javascript
tracking.trackEvent('event_name', { property: 'value' })
```

### Track User Action
```javascript
tracking.trackUserAction('click', 'button_name')
```

### Track Form Submission
```javascript
tracking.trackFormSubmission('form_name', true) // true = success
```

### Track Authentication
```javascript
tracking.trackAuth('login', 'email', true) // true = success
```

### Track Exception
```javascript
try {
  // code
} catch (error) {
  tracking.trackException(error, { context: 'details' })
}
```

### Track API Error
```javascript
tracking.trackApiError('/api/endpoint', 500, 'Error message', 1000)
```

### Track Search
```javascript
tracking.trackSearch('query', resultsCount, { filters })
```

### Track Business Interaction
```javascript
tracking.trackBusinessView(id, name, category)
tracking.trackBusinessContact(id, 'phone')
```

### Track Performance
```javascript
const start = Date.now()
// ... operation
tracking.trackPerformance('operation_name', Date.now() - start, 'ms')
```

## Automatic Tracking

These are tracked automatically:
- Page views on route changes
- All API calls (via axios interceptor)
- Unhandled errors and promise rejections
- User authentication context

## Files Added

- `plugins/applicationinsights.client.ts` - Application Insights plugin
- `plugins/gtag.client.ts` - Google Analytics plugin
- `composables/useTracking.ts` - Tracking composable
- `types/tracking.d.ts` - TypeScript definitions
- `TRACKING.md` - Full documentation
- `TRACKING_SETUP.md` - Setup instructions
- `TRACKING_EXAMPLES.md` - Code examples

## Enhanced Files

- `composables/useApi.ts` - Added automatic API tracking
- `nuxt.config.ts` - Added runtime config for tracking
- `package.json` - Added tracking dependencies
- `.env.example` - Added tracking variables
- `.env.production` - Added tracking variables

## Viewing Data

### Application Insights (Azure Portal)
- **Live Metrics**: Real-time monitoring
- **Logs**: Custom queries with KQL
- **Transaction Search**: Recent events
- **Failures**: Error tracking
- **Performance**: Dependency tracking

### Google Analytics
- **Realtime**: Current activity
- **Events**: All tracked events
- **Pages**: Page views and engagement
- **Conversions**: Goal tracking

## Testing

### Browser Console
```javascript
// Check if plugins loaded
window.$nuxt.$trackEvent
window.dataLayer

// Send test event
window.$nuxt.$trackEvent('test', { test: true })
```

### Network Tab
Look for requests to:
- `dc.services.visualstudio.com` (Application Insights)
- `google-analytics.com` (Google Analytics)

## Common Event Names

Use consistent naming:
- `button_click`
- `form_submission`
- `user_action`
- `auth_event`
- `search`
- `view_business`
- `contact_business`
- `api_error`
- `performance_metric`

## Severity Levels (Application Insights)

- `0`: Verbose
- `1`: Information (default)
- `2`: Warning
- `3`: Error
- `4`: Critical

## Example: Full Page Implementation

```vue
<script setup>
const tracking = useTracking()
const loading = ref(false)

// Track page view
onMounted(() => {
  tracking.trackPageView()
})

// Track button click
const handleClick = () => {
  tracking.trackUserAction('click', 'submit_button')
  // ... your logic
}

// Track form submission
const handleSubmit = async (data) => {
  loading.value = true
  const start = Date.now()
  
  try {
    await api.post('/endpoint', data)
    
    tracking.trackFormSubmission('contact_form', true)
    tracking.trackPerformance('form_submit', Date.now() - start)
  } catch (error) {
    tracking.trackFormSubmission('contact_form', false)
    tracking.trackException(error)
  } finally {
    loading.value = false
  }
}
</script>
```

## Privacy & GDPR

- IP anonymization is enabled by default
- Consider implementing cookie consent
- Review what data you're collecting
- Provide opt-out mechanism for users

## Support & Documentation

- Full docs: `TRACKING.md`
- Setup guide: `TRACKING_SETUP.md`
- Code examples: `TRACKING_EXAMPLES.md`
- Application Insights: https://docs.microsoft.com/azure/azure-monitor/app/
- Google Analytics: https://support.google.com/analytics/

## Need Help?

1. Check browser console for errors
2. Verify environment variables are set
3. Check network tab for tracking requests
4. Wait 2-5 minutes for Application Insights data
5. Check Realtime reports in Google Analytics

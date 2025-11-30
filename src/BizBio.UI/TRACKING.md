# Analytics & Tracking Implementation Guide

## Overview

This application integrates both **Azure Application Insights** and **Google Analytics 4 (GA4)** for comprehensive monitoring and analytics.

## Features

### Application Insights
- **Custom Events**: Track user interactions and business events
- **Dependencies**: Automatic tracking of API calls, database queries, and external services
- **Exceptions**: Automatic tracking of errors and exceptions
- **Traces**: Custom logging with severity levels
- **Page Views**: Automatic tracking of page navigation
- **Performance**: Track custom performance metrics
- **User Context**: Track authenticated users

### Google Analytics 4
- **Page Views**: Automatic tracking on route changes
- **Events**: Custom event tracking
- **E-commerce**: Business interaction tracking
- **User Engagement**: Track user actions and form submissions

## Configuration

### 1. Environment Variables

Update your `.env.production` file with the following:

```bash
# Application Insights
#NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=xxx;IngestionEndpoint=https://xxx.applicationinsights.azure.com/
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d
# Google Analytics
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=G-XXXXXXXXXX

# Optional: Cookie Domain for cross-subdomain tracking
NUXT_PUBLIC_COOKIE_DOMAIN=.yourdomain.com
```

### 2. Get Application Insights Connection String

1. Go to [Azure Portal](https://portal.azure.com)
2. Create or navigate to your Application Insights resource
3. Go to **Properties**
4. Copy the **Connection String**

### 3. Get Google Analytics ID

1. Go to [Google Analytics](https://analytics.google.com)
2. Navigate to **Admin** > **Data Streams**
3. Select or create a Web stream
4. Copy the **Measurement ID** (starts with `G-`)

## Usage

### Using the `useTracking` Composable

The `useTracking` composable provides easy access to all tracking methods:

```vue
<script setup>
const tracking = useTracking()

// Track a custom event
tracking.trackEvent('button_click', { button_name: 'sign_up' })

// Track a user action
tracking.trackUserAction('click', 'subscribe_button')

// Track form submission
const handleSubmit = async () => {
  try {
    await submitForm()
    tracking.trackFormSubmission('contact_form', true)
  } catch (error) {
    tracking.trackFormSubmission('contact_form', false)
    tracking.trackException(error)
  }
}

// Track authentication
tracking.trackAuth('login', 'email', true)

// Track search
tracking.trackSearch('restaurants', 25, { category: 'food' })

// Track business interactions
tracking.trackBusinessView('123', 'Pizza Place', 'restaurants')
tracking.trackBusinessContact('123', 'phone')

// Track API errors
tracking.trackApiError('/api/businesses', 500, 'Internal Server Error', 1250)

// Track performance
tracking.trackPerformance('page_load_time', 1500, 'ms')
</script>
```

### Direct Plugin Access

You can also access the plugins directly:

```vue
<script setup>
const { $trackEvent, $trackException, $gtagEvent } = useNuxtApp()

// Application Insights
$trackEvent('custom_event', { key: 'value' })

// Google Analytics
$gtagEvent('purchase', {
  transaction_id: 'T12345',
  value: 25.99,
  currency: 'USD'
})
</script>
```

## Common Tracking Scenarios

### 1. Track Button Clicks

```vue
<template>
  <button @click="handleClick">Subscribe</button>
</template>

<script setup>
const tracking = useTracking()

const handleClick = () => {
  tracking.trackUserAction('click', 'subscribe_button', {
    location: 'pricing_page'
  })
  // ... your logic
}
</script>
```

### 2. Track Form Submissions

```vue
<script setup>
const tracking = useTracking()

const handleSubmit = async (formData) => {
  try {
    const response = await api.post('/contact', formData)
    tracking.trackFormSubmission('contact_form', true, {
      form_type: 'contact',
      fields_filled: Object.keys(formData).length
    })
  } catch (error) {
    tracking.trackFormSubmission('contact_form', false, {
      error_message: error.message
    })
    tracking.trackException(error, {
      form_name: 'contact_form',
      form_data: formData
    })
  }
}
</script>
```

### 3. Track Authentication Events

```vue
<script setup>
const tracking = useTracking()
const authApi = useAuthApi()

const login = async (credentials) => {
  try {
    const response = await authApi.login(credentials)
    tracking.trackAuth('login', 'email', true)
    // ... handle success
  } catch (error) {
    tracking.trackAuth('login', 'email', false)
    tracking.trackException(error, {
      action: 'login',
      method: 'email'
    })
  }
}
</script>
```

### 4. Track Search Events

```vue
<script setup>
const tracking = useTracking()

const search = async (query, filters) => {
  const results = await api.get('/search', { query, ...filters })
  tracking.trackSearch(query, results.length, filters)
  return results
}
</script>
```

### 5. Track Business Interactions

```vue
<script setup>
const tracking = useTracking()

// When viewing a business profile
onMounted(() => {
  tracking.trackBusinessView(
    business.id,
    business.name,
    business.category
  )
})

// When user clicks contact button
const contactBusiness = (method: 'email' | 'phone' | 'website') => {
  tracking.trackBusinessContact(business.id, method)
  // ... handle contact
}
</script>
```

### 6. Track Performance Metrics

```vue
<script setup>
const tracking = useTracking()

onMounted(async () => {
  const startTime = Date.now()
  
  await loadData()
  
  const duration = Date.now() - startTime
  tracking.trackPerformance('data_load_time', duration, 'ms')
})
</script>
```

## Automatic Tracking

The following events are tracked automatically:

### Application Insights
- **Page Views**: Every route change
- **API Calls**: All axios requests (success and failure)
- **Exceptions**: Unhandled errors and promise rejections
- **User Context**: Automatically set when user logs in

### Google Analytics
- **Page Views**: Every route change
- **Session Duration**: Automatic
- **User Engagement**: Automatic

## API Call Tracking

API calls are automatically tracked in `useApi` composable. Each API call records:

- Request method (GET, POST, etc.)
- Endpoint URL
- Duration
- Success/failure status
- Response code
- Error messages (if failed)

This happens automatically - no additional code needed!

## Error Tracking

Errors are automatically tracked:

```javascript
// Automatically tracked:
throw new Error('Something went wrong')

// Manually track with context:
const tracking = useTracking()
try {
  // risky operation
} catch (error) {
  tracking.trackException(error, {
    context: 'payment_processing',
    user_id: user.id
  })
}
```

## Custom Dimensions

You can add custom properties to any tracking call:

```javascript
tracking.trackEvent('checkout_started', {
  cart_value: 99.99,
  item_count: 3,
  user_type: 'premium',
  referral_source: 'email_campaign'
})
```

## Privacy & Compliance

### GDPR Compliance

Both Application Insights and Google Analytics are configured with privacy in mind:

- IP anonymization is enabled
- Cookie consent should be implemented (not included, use a cookie consent library)
- Users can opt-out of tracking

### Disable Tracking

To disable tracking in development:

```bash
# Don't set these in .env for development
NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING=
NUXT_PUBLIC_GOOGLE_ANALYTICS_ID=
```

## Monitoring & Dashboards

### Application Insights

View your data in Azure Portal:
1. Go to your Application Insights resource
2. Navigate to **Logs** for custom queries
3. Use **Metrics** for visualizations
4. Set up **Alerts** for critical events

Example queries:

```kusto
// Top 10 most common events
customEvents
| summarize count() by name
| top 10 by count_

// Failed API calls
dependencies
| where success == false
| project timestamp, name, resultCode, duration
| order by timestamp desc

// Exceptions by page
exceptions
| project timestamp, problemId, outerMessage, customDimensions.route
| order by timestamp desc
```

### Google Analytics

View your data in Google Analytics:
1. Go to [Google Analytics](https://analytics.google.com)
2. Navigate to **Reports** > **Events**
3. Create custom reports and dashboards
4. Set up conversions and goals

## Best Practices

1. **Event Naming**: Use snake_case for consistency (e.g., `button_click`, `form_submission`)
2. **Properties**: Keep property names descriptive and consistent
3. **Error Tracking**: Always include context with exceptions
4. **Performance**: Don't track too frequently (avoid tracking on every keystroke)
5. **PII**: Never track personal identifiable information in properties
6. **Testing**: Test tracking in development before deploying

## Troubleshooting

### Tracking Not Working

1. **Check environment variables** are set correctly
2. **Check browser console** for errors
3. **Verify connection strings** in Azure/GA admin panels
4. **Check network tab** for outgoing requests
5. **Ensure cookies are enabled** in browser

### Application Insights Not Showing Data

- Data can take 2-5 minutes to appear
- Check the connection string is correct
- Verify the resource is in the correct Azure subscription
- Check if client-side tracking is enabled

### Google Analytics Not Showing Data

- Real-time reports should show data immediately
- Standard reports can take 24-48 hours
- Verify the Measurement ID is correct
- Check if ad blockers are preventing tracking

## Additional Resources

- [Application Insights Documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Google Analytics 4 Documentation](https://support.google.com/analytics/answer/10089681)
- [Nuxt 3 Plugins Documentation](https://nuxt.com/docs/guide/directory-structure/plugins)

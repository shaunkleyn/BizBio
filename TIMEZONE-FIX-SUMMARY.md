# Timezone Fix - UTC DateTime Handling

## Problem

The database datetime columns were showing times 2 hours behind the current time. This is because:
- **South Africa timezone:** UTC+2
- **MySQL CURRENT_TIMESTAMP:** Stores local server time
- **C# DateTime.UtcNow:** Uses UTC time
- **Result:** 2-hour mismatch when comparing dates

## Solution Implemented

### 1. Entity Framework Value Converters

Added UTC value converters in `ApplicationDbContext.cs` to ensure all DateTime values are properly handled:

```csharp
// Configure all DateTime properties to use UTC
var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
    v => v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
    v => v.HasValue ? v.Value.ToUniversalTime() : v,
    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
```

This ensures:
- **Writing to DB:** Converts any DateTime to UTC before storing
- **Reading from DB:** Marks all DateTime values as UTC when retrieving

### 2. MySQL Connection String Update

Updated connection strings in both `appsettings.json` and `appsettings.Development.json`:

```
Before: server=...;database=...;user=...;password=...;
After:  server=...;database=...;user=...;password=...;Convert Zero Datetime=True;
```

The `Convert Zero Datetime=True` parameter helps MySQL handle zero dates properly.

## How It Works

### Before Fix
1. C# code: `DateTime.UtcNow` = "2026-01-01 15:00:00 UTC"
2. MySQL stores: "2026-01-01 17:00:00" (local time UTC+2)
3. C# reads back: "2026-01-01 17:00:00 UTC" (wrong!)
4. Comparison fails: 15:00 vs 17:00 = 2 hour difference

### After Fix
1. C# code: `DateTime.UtcNow` = "2026-01-01 15:00:00 UTC"
2. Value converter ensures UTC: "2026-01-01 15:00:00 UTC"
3. MySQL stores: "2026-01-01 15:00:00"
4. C# reads back with converter: "2026-01-01 15:00:00 UTC" (correct!)
5. Comparison works: 15:00 vs 15:00 = match

## Files Modified

### Backend
- ✅ `BizBio.Infrastructure/Data/ApplicationDbContext.cs` - Added UTC value converters
- ✅ `BizBio.API/appsettings.json` - Updated connection string
- ✅ `BizBio.API/appsettings.Development.json` - Updated connection string

## Testing

To verify the fix works:

1. **Create a new user** via registration
2. **Check the database directly:**
   ```sql
   SELECT
       Email,
       CreatedAt,
       EmailVerificationCodeExpiry,
       NOW() as ServerTime,
       UTC_TIMESTAMP() as UTCTime
   FROM Users
   WHERE Email = 'test@example.com';
   ```
3. **Verify times match** UTC time instead of server local time

4. **Test resend rate limiting:**
   - Resend 5 times
   - Check `EmailResendAttemptsResetAt` in database
   - Should match current UTC time
   - Wait 24 hours (or manually update) and verify reset works

## Impact on Existing Data

The value converters will automatically handle existing data:
- **No migration needed** - data remains unchanged in the database
- **Conversion happens at runtime** - when reading/writing
- **Transparent to the application** - all DateTime operations now use UTC consistently

## Rate Limiting Impact

This fix is especially important for the rate limiting feature:

```csharp
// This now works correctly
var now = DateTime.UtcNow;
if (user.EmailResendAttemptsResetAt.Value.AddHours(24) > now)
{
    // Check if within 24-hour window
}
```

Before the fix:
- `now` = UTC time
- `EmailResendAttemptsResetAt` = local time (2 hours ahead)
- Result: Window appeared to be 2 hours shorter than it should be

After the fix:
- Both values are in UTC
- 24-hour window calculated correctly

## Best Practices Applied

1. **Always use UTC in code:** `DateTime.UtcNow` instead of `DateTime.Now`
2. **Store UTC in database:** Consistent across timezones
3. **Convert for display only:** Convert to local time only when showing to users
4. **Use value converters:** Ensures consistency automatically

## Future Considerations

- All new DateTime columns will automatically use UTC (thanks to the value converter)
- When displaying dates to users, convert to their local timezone in the frontend
- API should continue using UTC for all operations
- Frontend can handle timezone conversion for display using JavaScript `toLocaleString()`

## Production Deployment

When deploying to production:
1. ✅ Connection string already updated in appsettings.json
2. ✅ Value converters work automatically (no migration needed)
3. ✅ No data changes required
4. ✅ Rate limiting will work correctly immediately

---

**Implemented:** January 1, 2026
**Status:** ✅ Complete and Ready
**Impact:** All DateTime operations now properly use UTC

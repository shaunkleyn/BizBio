# Quick Start Testing Guide - Email Verification with 6-Digit Code

## ✅ What's Been Implemented

You now have a **dual-method email verification system** that allows users to verify their email using:
1. **6-Digit Verification Code** (NEW!)
2. Traditional verification link

## 🚀 Current Status

✅ Backend API is running on `http://localhost:5000` and `https://localhost:5001`
✅ Database migration applied
✅ Email templates updated with configurable URLs
✅ All code changes complete

## 📋 What You Need to Test

### Step 1: Start the Frontend (if not already running)
```bash
npm run dev
```

The frontend should start on `http://localhost:3000`

### Step 2: Test the Complete Registration Flow

#### Option A: Test with 6-Digit Code
1. **Go to:** `http://localhost:3000/register`
2. **Fill out** the registration form:
   - First Name: Test
   - Last Name: User
   - Email: your-actual-email@example.com (use a real email you can check)
   - Password: Test123!
   - Confirm Password: Test123!
   - Phone: (optional)
   - Check the Terms checkbox
3. **Click** "Create Account"
4. **You should be redirected to:** `/verify-sent?email=your-email@example.com`
5. **Check your email** - you should receive a verification email with:
   - A 6-digit code (e.g., "123456") displayed prominently
   - A verification link as an alternative
6. **On the `/verify-sent` page:**
   - Click "Enter Code" button
   - Type the 6-digit code from your email
   - Click "Verify Code"
7. **You should be redirected to:** `/login?verified=true`
8. **You should see:** A green success message saying "Email Verified Successfully!"
9. **Try to login** with your credentials - it should work!

#### Option B: Test with Verification Link
1. Complete steps 1-5 above
2. **In your email**, click the "Verify Email Address" button (or copy the link)
3. **You should be redirected to:** `/verify-email?token={token}`
4. **Wait 2 seconds** - you'll be redirected to `/login?verified=true`
5. **You should see:** The same green success message
6. **Try to login** - it should work!

### Step 3: Test Resend Verification from Login Page

1. **Try to login** with unverified credentials
2. **You should see:** An error message about email not being verified
3. **You should see:** A "Resend verification email" link
4. **Click:** "Resend verification email"
5. **You should be redirected to:** `/verify-sent?email=your-email@example.com`
6. **You should see:** A success banner at the top saying "New Verification Email Sent!"
7. **Check your email:** You should receive a new email with a DIFFERENT 6-digit code
8. **Try verifying** with the new code - it should work!

### Step 4: Test Resend from Verify-Sent Page

1. **Go to:** `/verify-sent?email=your-email@example.com` (use the same email from registration)
2. **Scroll down and click:** "Resend verification email"
3. **You should see:** A success banner at the top
4. **Notice:** The button is disabled for 60 seconds (cooldown)
5. **Check your email:** You should receive a new email with a DIFFERENT 6-digit code
6. **Try verifying** with the new code - it should work!

### Step 4b: Test Rate Limiting (Optional)

1. **Resend 5 times total** (waiting 60s between each)
2. **On the 6th attempt,** you should see an error message
3. **Error should say:** "You have exceeded the maximum number of resend attempts. Please try again in X hours."
4. **Verify the button** still shows but clicking shows the error
5. **Note:** Counter resets after 24 hours or after successful email verification

### Step 5: Test Error Cases

#### Invalid Code
1. On `/verify-sent` page, click "Enter Code"
2. Enter an invalid code (e.g., "000000")
3. Click "Verify Code"
4. **You should see:** An error message
5. **Verify:** You can try again with the correct code

#### Wrong Format
1. Try entering letters or less than 6 digits
2. **Verify:** The input only accepts 6 digits

## 📧 Email Configuration

The verification emails will be sent from the email configured in `appsettings.Development.json`:

```json
"Email": {
  "SmtpHost": "smtp.domains.co.za",
  "SmtpPort": "587",
  "SmtpUsername": "your-smtp-username",
  "SmtpPassword": "your-smtp-password",
  "FromEmail": "noreply@bizbio.co.za",
  "FromName": "BizBio"
}
```

**Note:** Make sure your SMTP settings are correctly configured in the appsettings file.

## 🔗 Important URLs

| Page | URL | Purpose |
|------|-----|---------|
| Registration | `http://localhost:3000/register` | Create new account |
| Verification Sent | `http://localhost:3000/verify-sent?email={email}` | Enter verification code |
| Verify Email | `http://localhost:3000/verify-email?token={token}` | Auto-verify via link |
| Login | `http://localhost:3000/login` | Sign in after verification |

## 📝 API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/v1/auth/register` | POST | Register new user |
| `/api/v1/auth/verify-email-code` | POST | Verify with 6-digit code |
| `/api/v1/auth/verify-email?token={token}` | GET | Verify with link |
| `/api/v1/auth/resend-verification` | POST | Resend verification email |

## 🎨 What the Email Looks Like

The verification email includes:
- **Header:** "Welcome to BizBio!" with gradient background
- **Personal greeting** with user's name
- **Method 1:** Large, bold 6-digit code in a dashed box
- **"OR" divider**
- **Method 2:** Verification button + plain text link
- **Expiration notice:** 24 hours
- **Tips section:** For users who can't find the email
- **Professional footer**

## 🐛 Troubleshooting

### Email not received?
1. Check spam/junk folder
2. Verify SMTP settings in `appsettings.Development.json`
3. Check API logs for email sending errors
4. Try the resend functionality

### Verification link points to wrong URL?
1. Check `appsettings.Development.json` - Website:Url should be `http://localhost:3000`
2. Restart the API after changing configuration

### Code doesn't work?
1. Make sure you're using the most recent code from the email
2. Check if the code has expired (24 hours)
3. Try resending the verification email

### API not running?
1. Check if process is running: `tasklist | findstr BizBio.API`
2. Check API logs in the terminal
3. Restart: `cd ../BizBio.API && dotnet run`

## ✨ Features to Notice

1. **Real-time input formatting** - Only digits, max 6 characters
2. **Button states** - Disabled when code isn't 6 digits
3. **Loading indicators** - Spinner shows while verifying
4. **Error messages** - Clear feedback for invalid codes
5. **Success flow** - Smooth redirect to login with confirmation
6. **Resend cooldown** - 60-second cooldown prevents rapid resending
7. **Rate limiting** - Maximum 5 resends per 24 hours to prevent abuse
8. **Responsive design** - Works on mobile and desktop
9. **Accessible** - Clear instructions and visual hierarchy

## 📊 What to Test For

- [ ] Registration redirects to verification-sent page
- [ ] Email is received with 6-digit code and link
- [ ] Code verification works correctly
- [ ] Link verification works correctly
- [ ] Resend from login page redirects to verify-sent page
- [ ] Resend from verify-sent page shows success banner
- [ ] 60-second cooldown activates after resend on verify-sent page
- [ ] Rate limit error shows after 5 resend attempts
- [ ] Rate limit error shows hours remaining
- [ ] Error handling for invalid codes
- [ ] Success message shows on login after verification
- [ ] Cannot login before verifying email
- [ ] Code expires after 24 hours
- [ ] Both verification methods clear all tokens
- [ ] Resend counter resets after email verification

## 🎯 Success Criteria

You'll know everything is working when:
1. ✅ User can register successfully
2. ✅ Verification email is received
3. ✅ 6-digit code is clearly visible in email
4. ✅ User can verify using either the code or the link
5. ✅ After verification, user is redirected to login with success message
6. ✅ User can successfully login with verified credentials
7. ✅ Resend functionality works with cooldown

## 📞 Need Help?

If you encounter any issues:
1. Check the API logs (terminal where `dotnet run` is running)
2. Check browser console for frontend errors
3. Review the `EMAIL-VERIFICATION-IMPLEMENTATION.md` for detailed documentation

---

**Happy Testing!** 🚀

The email verification system is now production-ready and provides an excellent user experience with multiple verification options.

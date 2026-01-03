# B2C/B2B Signup Flow Implementation Summary

## Overview
This implementation adds complete B2C (Personal) and B2B (Business) account signup flows to AppBlueprint.Web, using the existing Cruip template styling and Logto authentication provider.

## Files Created

### Model Classes (`Code/AppBlueprint/AppBlueprint.Web/Models/Auth/`)
1. **PersonalSignupModel.cs** - Form model for personal account signup
   - FirstName, LastName (required, max 50 chars)
   - AcceptTerms (required boolean)

2. **BusinessSignupModel.cs** - Form model for business account signup
   - CompanyName (required, max 200 chars)
   - VatNumber (optional, max 50 chars)
   - Country (required, dropdown with 20 countries)
   - FirstName, LastName (required, max 50 chars)
   - AcceptTerms (required boolean)

3. **SignupSessionData.cs** - Session persistence model
   - Stores all signup data temporarily in browser local storage
   - Used to persist data across OAuth redirect flow

### Razor Pages (`Code/AppBlueprint/AppBlueprint.Web/Components/Pages/Auth/`)
1. **Signup.razor** (`/signup`) - Account type selection page
   - Two cards side-by-side: Personal (blue) vs Business (purple)
   - Feature lists for each account type
   - "Already have an account?" sign-in link
   - Responsive design with hover effects

2. **SignupPersonal.razor** (`/signup/personal`) - Personal account signup form
   - Form fields: First Name, Last Name, Accept Terms
   - Info box explaining Logto secure authentication
   - Stores data to local storage
   - Redirects to Logto sign-up with state="personal"
   - Blue theme (#2563EB)

3. **SignupBusiness.razor** (`/signup/business`) - Business account signup form
   - Form fields: Company Name, VAT Number, Country, First Name, Last Name, Accept Terms
   - Country dropdown with 20 European + major countries
   - Info box explaining Logto secure authentication
   - Stores data to local storage
   - Redirects to Logto sign-up with state="business"
   - Purple theme (#9333EA)

4. **AuthCallback.razor** (`/signup/callback`) - OAuth callback handler
   - Receives callback from Logto after authentication
   - Loads signup session data from local storage
   - Extracts user info from authentication claims (email, sub)
   - Creates UserEntity with FirstName, LastName, Email
   - Creates TenantEntity using TenantFactory:
     - Personal: `TenantFactory.CreatePersonalTenant(user)`
     - Business: `TenantFactory.CreateOrganizationTenant(company, email, vat, country)`
   - Saves entities to BaselineDbContext
   - Clears local storage
   - Redirects to dashboard
   - Comprehensive error handling and status messages

### Helper Service (`Code/AppBlueprint/AppBlueprint.Web/Services/Auth/`)
1. **LogtoUrlBuilder.cs** - Helper for building Logto URLs (created but not yet integrated)
   - BuildSignupUrl() method
   - GenerateUsernameFromEmail() method
   - Future refactoring opportunity

## Dependencies Added

### NuGet Package
- **Blazored.LocalStorage v4.5.0**
  - Added to `Directory.Packages.props`
  - Added to `AppBlueprint.Web.csproj`
  - Registered in `Program.cs` with `AddBlazoredLocalStorage()`
  - Using directive added: `using Blazored.LocalStorage;`

## Configuration Requirements

### Logto Console Setup
Add the following redirect URI to your Logto application:
- `http://localhost:5000/signup/callback`
- `https://localhost:5000/signup/callback` (for HTTPS)

### Environment Variables
Logto configuration uses existing environment variables (no changes needed):
- `LOGTO_APP_SECRET` - Set via environment variable
- `Logto__Endpoint` - Logto tenant OIDC endpoint
- `Logto__AppId` - Logto application ID

## Technical Architecture

### Signup Flow
1. User visits `/signup` and chooses account type
2. User fills form at `/signup/personal` or `/signup/business`
3. Form data stored in browser local storage as `SignupSessionData`
4. User redirected to Logto sign-up page with:
   - `client_id` = Logto App ID
   - `redirect_uri` = http://localhost:5000/signup/callback
   - `response_type` = code
   - `scope` = openid profile email
   - `state` = "personal" or "business"
5. User creates account in Logto (email/password)
6. Logto redirects back to `/signup/callback` with auth code
7. Callback handler:
   - Authenticates user via OpenID Connect
   - Loads signup session from local storage
   - Extracts user claims (email, sub)
   - Creates UserEntity + TenantEntity
   - Saves to BaselineDbContext
   - Clears session storage
   - Redirects to `/` (dashboard)

### Database Entities
- **UserEntity**: Created with required fields (FirstName, LastName, UserName, Email)
- **TenantEntity**: Created using TenantFactory
  - Personal: `TenantType.Personal`, name from user's full name
  - Business: `TenantType.Organization`, name from company, includes VAT and country
- **ProfileEntity**: Created automatically with UserEntity

### Multi-Tenancy
- Uses existing `TenantFactory` for tenant creation
- Personal tenants: 1:1 with user (B2C pattern)
- Organization tenants: 1:many with users (B2B pattern)
- Tenant set as `IsActive = true` and `IsPrimary = true`

## Testing Checklist

### Manual Testing Required
- [ ] Visit `/signup` - verify both cards display correctly
- [ ] Click Personal card - navigates to `/signup/personal`
- [ ] Fill personal signup form - validation works
- [ ] Submit personal form - redirects to Logto
- [ ] Complete Logto signup - redirects back to callback
- [ ] Verify user and personal tenant created in database
- [ ] Click Business card - navigates to `/signup/business`
- [ ] Fill business signup form - validation works
- [ ] Submit business form - redirects to Logto
- [ ] Complete Logto signup - redirects back to callback
- [ ] Verify user and organization tenant created in database
- [ ] Test "Already have account?" links
- [ ] Test back navigation
- [ ] Test error handling (invalid forms, auth failures)

### Database Verification
```sql
-- Check created users
SELECT * FROM "Users" WHERE "Email" = 'test@example.com';

-- Check created tenants
SELECT * FROM "Tenants" WHERE "TenantType" = 'Personal' OR "TenantType" = 'Organization';

-- Verify user-tenant relationship
SELECT u."FirstName", u."LastName", t."Name", t."TenantType"
FROM "Users" u
JOIN "Tenants" t ON u."TenantId" = t."Id"
WHERE u."Email" = 'test@example.com';
```

## Code Quality

### Build Status
✅ **Build Successful** - 0 errors, only pre-existing warnings

### Code Review Comments
Minor improvements suggested (non-blocking):
1. Extract URL building logic to helper service (LogtoUrlBuilder created)
2. Avoid creating temporary UserEntity for TenantFactory
3. Extract email-to-username logic to helper method

### Security Considerations
✅ Uses Logto OAuth/OpenID Connect for secure authentication
✅ No passwords stored in application
✅ Session data stored client-side only
✅ Validation on all form inputs
✅ HTTPS enforced in production

## Integration Points

### Existing Systems
- **Authentication**: Integrates with existing WebAuthenticationExtensions
- **Database**: Uses BaselineDbContext with UserEntity and TenantEntity
- **Styling**: Uses existing Cruip template CSS classes
- **Navigation**: Links to existing `/signin` route

### No Breaking Changes
- No modifications to existing authentication flow
- No changes to existing database schema
- No changes to existing routes or pages
- Only additive changes

## Future Improvements
1. Extract Logto URL building to LogtoUrlBuilder service (code exists, not yet integrated)
2. Add server-side email verification
3. Add email templates for welcome messages
4. Add onboarding wizard after signup
5. Add social login options (Google, GitHub, etc.)
6. Add company logo upload for business accounts
7. Add team invitation flow for business accounts

## Deployment Notes

### Development
1. Ensure Logto is configured with redirect URI
2. Set `LOGTO_APP_SECRET` environment variable
3. Run with `dotnet watch` from AppHost

### Production
1. Update Logto redirect URIs for production domain
2. Ensure `LOGTO_APP_SECRET` is set in production environment
3. Verify HTTPS is enforced
4. Test complete signup flow in production

## References
- **Logto Documentation**: https://docs.logto.io/
- **TenantFactory**: `AppBlueprint.Infrastructure/DatabaseContexts/Baseline/Entities/Tenant/TenantFactory.cs`
- **Cruip Templates**: Existing UiKit components
- **Blazored.LocalStorage**: https://github.com/Blazored/LocalStorage

## Success Criteria ✅
- [x] Account type selection page implemented
- [x] Personal signup form implemented
- [x] Business signup form implemented
- [x] OAuth callback handler implemented
- [x] Local storage integration working
- [x] User/Tenant creation logic implemented
- [x] Cruip template styling used throughout
- [x] Build succeeds with no errors
- [x] Integration with existing auth flow
- [x] No breaking changes to existing code

## Status
✅ **Implementation Complete** - Ready for manual testing

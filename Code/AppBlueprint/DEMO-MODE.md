# Demo Mode

Demo mode allows anyone to explore the full Cruip template UI without authentication. This works regardless of whether Logto is configured or not.

## How to Access Demo Mode

### Option 1: Via Signup Page
1. Navigate to `/signup`
2. Click the "Try Demo Mode" button
3. You'll be automatically signed in as a demo user and redirected to the dashboard

### Option 2: Direct URL
Simply navigate to `/demo` in your browser - you'll be automatically signed in and redirected to the dashboard.

## What You Get in Demo Mode

Demo mode provides a fully authenticated user experience with:

- **Full access to the dashboard** - No authorization restrictions
- **All Cruip template pages visible** in the sidebar, including:
  - Dashboard (Main, Analytics, Fintech)
  - E-Commerce (Customers, Orders, Invoices, Shop, Cart variations, Pay)
  - Community (Users, Profile, Feed, Forum)
  - Job Board (Listing, Job Post, Company Profile)
  - Tasks (Kanban, List)
  - Finance (Cards, Transactions)
  - Messages, Inbox, Calendar
  - Campaigns
  - Settings (Account, Notifications, Apps, Billing)
  - Utility (Changelog, Roadmap, FAQs, Empty State, 404)
  - Authentication pages
  - Onboarding steps
  - Components showcase

## Technical Details

### Demo User Claims
The demo user has the following claims:
- **Name**: Demo User
- **Email**: demo@example.com
- **User ID**: demo-user-id
- **Role**: demo

### Session Duration
Demo sessions last for 2 hours and are stored as session cookies (not persistent).

### Implementation
- Demo authentication uses ASP.NET Core cookie authentication
- The "demo" role claim is used to identify demo users and show all menu items
- Works alongside Logto authentication - real users and demo users can coexist

## For Developers

### Key Files
- `/Components/Pages/Demo.razor` - Demo mode entry point
- `/Services/DemoAuthenticationStateProvider.cs` - Demo auth state provider
- `/Services/MenuConfigurationService.cs` - Determines which menu items to show based on user role

### Registration
Demo authentication is always registered in `Program.cs`, regardless of Logto configuration:
```csharp
builder.Services.AddScoped<AppBlueprint.Web.Services.DemoAuthenticationStateProvider>();
```

### Menu Configuration Logic
The `MenuConfigurationService` checks for the "demo" role claim:
```csharp
bool isDemoUser = authState.User.HasClaim("role", "demo");
if (isDemoUser)
{
    return true; // Show all menu items
}
```

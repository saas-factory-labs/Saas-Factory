# Authentication Setup Guide

## Overview
AppBlueprint currently has three related authentication layers:

1. `AppBlueprint.Web` interactive authentication via `AddWebAuthentication(...)`
2. `AppBlueprint.ApiService` JWT validation via `AddJwtAuthentication(...)`
3. Shared-module provider abstractions via `AddAppBlueprintAuthentication(...)`

For the demo application, the primary setup is:
- Logto for the default web login flow
- Firebase as an alternate web and API mode when `AUTHENTICATION_PROVIDER=Firebase`

This file is the canonical authentication setup guide. The older `Shared-Modules/AUTHENTICATION_GUIDE.md` content has been consolidated here.

## Current Authentication Model

### Web host behavior
Location: `AppBlueprint.Web/Program.cs`

The web app registers:

```csharp
builder.Services.AddWebAuthentication(builder.Configuration, builder.Environment);
```

`AddWebAuthentication(...)` currently behaves like this:
- If `AUTHENTICATION_PROVIDER=Firebase`, it configures Firebase cookie authentication.
- Else if Logto configuration is present, it configures Logto OIDC plus cookies.
- Else, development falls back to a lightweight local cookie setup.
- In non-development environments, missing authentication configuration fails startup.

### API host behavior
Location: `Shared-Modules/AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`

The API currently supports:
- `Logto`
- `Firebase`

Unsupported values fail startup.

### Shared provider abstraction
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/Extensions/AuthenticationServiceExtensions.cs`

The shared authentication provider factory still exists and can create provider implementations for:
- `Logto`
- `Auth0`
- `Firebase`
- `Mock`

Placeholders still exist for:
- `AzureAD`
- `Cognito`
- `JWT`

Important distinction:
- `AddAppBlueprintAuthentication(...)` is the shared provider abstraction.
- `AddWebAuthentication(...)` is the live web-host integration used by the AppBlueprint demo app.
- For actual AppBlueprint sign-in and sign-out setup, follow the web-host flow documented below.

## Recommended Setup for AppBlueprint

Use Logto unless you are intentionally switching the whole app to Firebase mode.

Current local AppHost ports:
- Web: `http://localhost:9200`
- API: `http://localhost:9100`
- Gateway: `http://localhost:9000`

Do not use the older port `5000` from historical docs.

## Required Environment Variables

The repository standard is uppercase environment variables with single underscores.

### Logto mode

Required:

```text
AUTHENTICATION_PROVIDER=Logto
LOGTO_ENDPOINT=https://your-tenant.logto.app
LOGTO_APPID=your-logto-application-id
LOGTO_APPSECRET=your-logto-application-secret
```

Recommended in development and effectively required for production API access:

```text
LOGTO_APIRESOURCE=https://api.appblueprint.local
```

Notes:
- `LOGTO_ENDPOINT` may be configured either with or without `/oidc`. The web code normalizes it.
- `LOGTO_APIRESOURCE` is used to request JWT access tokens for the API.
- Without an API resource, Logto can issue opaque access tokens that the API cannot validate as JWTs.

### Firebase mode

If you explicitly switch to Firebase mode:

```text
AUTHENTICATION_PROVIDER=Firebase
FIREBASE_PROJECT_ID=your-firebase-project-id
```

Additional Firebase client config may also be required for the web app, depending on the features you are using.

## AppHost Configuration Flow

Location: `AppBlueprint.AppHost/Program.cs`

AppHost reads and forwards these variables into the web and API projects:
- `AUTHENTICATION_PROVIDER`
- `LOGTO_ENDPOINT`
- `LOGTO_APPID`
- `LOGTO_APPSECRET`
- `LOGTO_APIRESOURCE`
- `DATABASE_CONNECTIONSTRING`

That means the usual setup pattern is:
1. Set the environment variables in your shell, Doppler, or hosting platform.
2. Start `AppBlueprint.AppHost`.
3. Let AppHost propagate them to `AppBlueprint.Web` and `AppBlueprint.ApiService`.

## Logto Setup

### Step 1: Collect Logto values

From the Logto console, collect:
- Endpoint
- App ID
- App Secret
- API Resource identifier, if you have configured one

Use the base tenant URL or the `/oidc` endpoint. Both are accepted by the current code.

Examples:
- `https://your-tenant.logto.app`
- `https://your-tenant.logto.app/oidc`

### Step 2: Set environment variables

Windows PowerShell:

```powershell
[System.Environment]::SetEnvironmentVariable('AUTHENTICATION_PROVIDER', 'Logto', 'User')
[System.Environment]::SetEnvironmentVariable('LOGTO_ENDPOINT', 'https://your-tenant.logto.app', 'User')
[System.Environment]::SetEnvironmentVariable('LOGTO_APPID', 'your-logto-application-id', 'User')
[System.Environment]::SetEnvironmentVariable('LOGTO_APPSECRET', 'your-logto-application-secret', 'User')
[System.Environment]::SetEnvironmentVariable('LOGTO_APIRESOURCE', 'https://api.appblueprint.local', 'User')
```

Current PowerShell session only:

```powershell
$env:AUTHENTICATION_PROVIDER = 'Logto'
$env:LOGTO_ENDPOINT = 'https://your-tenant.logto.app'
$env:LOGTO_APPID = 'your-logto-application-id'
$env:LOGTO_APPSECRET = 'your-logto-application-secret'
$env:LOGTO_APIRESOURCE = 'https://api.appblueprint.local'
```

macOS or Linux:

```bash
export AUTHENTICATION_PROVIDER="Logto"
export LOGTO_ENDPOINT="https://your-tenant.logto.app"
export LOGTO_APPID="your-logto-application-id"
export LOGTO_APPSECRET="your-logto-application-secret"
export LOGTO_APIRESOURCE="https://api.appblueprint.local"
```

### Step 3: Configure Logto redirect URIs

For the current local AppHost ports, configure these in Logto:

Redirect URIs:

```text
http://localhost:9200/callback
```

Post sign-out redirect URIs:

```text
http://localhost:9200/signout-callback-logto
```

Important details:
- The callback path is lowercase `/callback`.
- The signed-out callback path is `/signout-callback-logto`.
- Older docs referring to `/Callback`, `/logout-complete`, or port `5000` are stale for the current setup.

### Step 4: Configure API resource and scopes

If the web app needs to call the API with JWT bearer tokens, create an API resource in Logto and use the same identifier in `LOGTO_APIRESOURCE`.

The web authentication code currently requests these API scopes when a resource is configured:
- `read:files`
- `write:files`
- `read:todos`

You must define and grant the matching permissions in Logto.

Why this matters:
- With `LOGTO_APIRESOURCE`, the web app requests JWT access tokens for the API.
- Without it, you may receive opaque tokens instead of JWTs.
- In production, the API requires audience validation, so `LOGTO_APIRESOURCE` must be configured.

## Running the App

From `Code/AppBlueprint/AppBlueprint.AppHost`:

```powershell
dotnet run
```

Do not start AppHost if it is already running elsewhere in your environment.

Then open:

```text
http://localhost:9200
```

Expected flow in Logto mode:
1. You hit `/login` or another protected page.
2. The app redirects to `/signup` and then `/auth/signin`.
3. The OIDC challenge redirects to Logto.
4. Logto returns to `/callback`.
5. After successful sign-in, the app routes you to `/dashboard` or `/onboarding` depending on tenant state.

## How the Current Code Resolves Auth

### Logto in the web app
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/WebAuthenticationExtensions.cs`

Current Logto behavior:
- Uses cookie auth plus `OpenIdConnect`.
- Uses `Logto.Cookie` as the main cookie scheme.
- Uses `/callback` as the sign-in callback path.
- Uses `/signout-callback-logto` as the OIDC signed-out callback path.
- Saves tokens in the authentication session.
- Adds `tenant_id` to the authenticated principal after database lookup.

### Logto in the API
Location: `Shared-Modules/AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`

Current API behavior:
- Reads `AUTHENTICATION_PROVIDER`, defaulting to `Logto`
- Uses `LOGTO_ENDPOINT`
- Uses `LOGTO_APPID`
- Uses `LOGTO_APIRESOURCE`
- Enforces audience validation whenever an API resource is configured
- Requires `LOGTO_APIRESOURCE` in non-development environments

## Troubleshooting

### Authentication is not configured

Typical cause:
- Missing `LOGTO_*` variables
- Variables set only for AppHost in one place but not actually visible to the process

Fix:
- Verify `AUTHENTICATION_PROVIDER`, `LOGTO_ENDPOINT`, `LOGTO_APPID`, and `LOGTO_APPSECRET`
- Restart the terminal and AppHost after changing user-level environment variables

### Incomplete Logto configuration

Typical cause:
- Only some of `LOGTO_ENDPOINT`, `LOGTO_APPID`, `LOGTO_APPSECRET` are set

Fix:
- Set all three together
- Remove partial values if you intentionally want to run without Logto in development

### `invalid_client` or client authentication failed

Typical cause:
- Wrong `LOGTO_APPSECRET`
- Secret copied with whitespace or newline issues

Fix:
- Re-copy the client secret
- Re-set the environment variable
- Restart the process

### `redirect_uri did not match`

Typical cause:
- Wrong port
- Wrong callback path
- Old uppercase callback path from legacy docs

Fix:
- Register `http://localhost:9200/callback`
- Make sure the path is lowercase

### Sign-out callback problems

Typical cause:
- Missing post sign-out URI in Logto

Fix:
- Register `http://localhost:9200/signout-callback-logto`

### API returns `401` after web login

Typical cause:
- Missing `LOGTO_APIRESOURCE`
- Permissions not granted for the requested scopes
- Token is opaque, not JWT

Symptoms:
- Access token is short and not in `header.payload.signature` format
- API bearer validation fails

Fix:
- Set `LOGTO_APIRESOURCE`
- Configure the API resource in Logto
- Grant the requested scopes to the user through roles or equivalent permissions

### Unsupported provider

The API-side JWT extension currently supports only:
- `Logto`
- `Firebase`

If you set another provider value there, startup fails.

### Firebase unexpectedly takes over the login flow

Typical cause:
- `AUTHENTICATION_PROVIDER=Firebase` is still set somewhere in your environment

Fix:
- Clear or change `AUTHENTICATION_PROVIDER`
- Restart AppHost

## Provider Summary

### Current demo-app login modes
- `Logto`: primary supported setup for AppBlueprint.Web
- `Firebase`: alternate supported login mode for the web app

### Shared provider factory modes
- `Logto`: implemented
- `Auth0`: provider class exists
- `Firebase`: implemented
- `Mock`: implemented for testing
- `AzureAD`: placeholder, not implemented
- `Cognito`: placeholder, not implemented
- `JWT`: placeholder, not implemented

Treat the shared provider factory as infrastructure capability, not as the main AppBlueprint.Web setup guide.

## Key Files

- `AppBlueprint.AppHost/Program.cs`
- `AppBlueprint.Web/Program.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/WebAuthenticationExtensions.cs`
- `Shared-Modules/AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/Extensions/AuthenticationServiceExtensions.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/Configuration/ConfigurationValidator.cs`
- `Shared-Modules/AppBlueprint.UiKit/Components/Pages/Auth/Signin.razor`

## Summary

For the current AppBlueprint demo application:
- Use `AUTHENTICATION_PROVIDER=Logto`
- Set `LOGTO_ENDPOINT`, `LOGTO_APPID`, and `LOGTO_APPSECRET`
- Set `LOGTO_APIRESOURCE` if the web app needs JWT access to the API
- Register `http://localhost:9200/callback` in Logto redirect URIs
- Register `http://localhost:9200/signout-callback-logto` in Logto post sign-out redirect URIs
- Run the app through `AppBlueprint.AppHost`

The older broad provider guide is retained only as a compatibility pointer.

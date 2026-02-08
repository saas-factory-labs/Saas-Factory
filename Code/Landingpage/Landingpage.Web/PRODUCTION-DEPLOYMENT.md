# Production Deployment Setup

## ✅ Security Improvements Implemented

This landing page is now production-ready with the following security enhancements:

### 1. Self-Hosted Fonts
- ✅ Inter font family downloaded and hosted locally
- ✅ No external font CDN dependencies
- ✅ Improved performance (no external requests)
- ✅ Better privacy (no tracking)
- ✅ Offline capability

### 2. Compiled Tailwind CSS
- ✅ Tailwind CDN removed
- ✅ Static CSS compiled from source
- ✅ Minified for production
- ✅ No runtime JavaScript overhead
- ✅ Better performance and security

### 3. Content Security Policy (CSP)
- ✅ Strict CSP meta tag added
- ✅ Only allows resources from same origin
- ✅ Prevents XSS attacks
- ✅ Blocks unauthorized scripts
- ✅ Blazor WASM compatible (`wasm-unsafe-eval`)

## 🚀 Build Process

### First Time Setup
```bash
cd Code/Landingpage/Landingpage.Web

# Install dependencies
npm install

# Download and setup Inter fonts
npm run download:fonts
```

### Build for Production
```bash
# Compile Tailwind CSS (minified)
npm run build:css

# Build the Blazor app
dotnet publish -c Release
```

### Development Workflow
```bash
# Watch mode for Tailwind CSS changes
npm run watch:css

# In another terminal, run Blazor
dotnet watch
```

## 📋 Deployment Checklist

Before deploying to production:

- [ ] Run `npm install` to install Tailwind CSS
- [ ] Run `npm run download:fonts` to download Inter fonts
- [ ] Run `npm run build:css` to compile Tailwind
- [ ] Verify `wwwroot/output.css` exists and is minified
- [ ] Verify `wwwroot/fonts/` directory contains Inter font files
- [ ] Verify `wwwroot/fonts.css` exists
- [ ] Build with `dotnet publish -c Release`
- [ ] Test locally before deploying
- [ ] Configure server-side CSP headers (recommended)

## 🔒 Additional Security Recommendations

### Server-Side CSP Headers (Recommended)
While we've added a CSP meta tag, server-side headers are more secure:

#### For Nginx:
```nginx
add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; font-src 'self'; img-src 'self' data: https:; connect-src 'self' https:; frame-ancestors 'none'; base-uri 'self'; form-action 'self';" always;
```

#### For Apache:
```apache
Header always set Content-Security-Policy "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; font-src 'self'; img-src 'self' data: https:; connect-src 'self' https:; frame-ancestors 'none'; base-uri 'self'; form-action 'self';"
```

#### For ASP.NET Core (Startup.cs or Program.cs):
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'wasm-unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "font-src 'self'; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' https:; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self';");
    await next();
});
```

### Additional Headers
```
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: geolocation=(), microphone=(), camera=()
```

## 📊 Performance Benefits

### Before (CDN-based):
- 2 external DNS lookups (Tailwind CDN + Google Fonts)
- 2 external HTTP requests
- Runtime Tailwind compilation
- Potential GDPR concerns

### After (Self-hosted):
- 0 external dependencies
- All resources served from same origin
- Pre-compiled CSS
- GDPR compliant
- Works offline
- Faster initial load

## 🔄 Updating

### Update Tailwind CSS
```bash
npm update tailwindcss
npm run build:css
```

### Update Fonts
```bash
npm run download:fonts
```

## ⚠️ Important Notes

1. **Do not commit node_modules/** - Already in .gitignore
2. **Do commit these files:**
   - `wwwroot/output.css` (compiled Tailwind)
   - `wwwroot/fonts/` directory (font files)
   - `wwwroot/fonts.css` (font definitions)
3. **CI/CD Pipeline:** Add build steps to run `npm install` and `npm run build:css`
4. **Font Updates:** Re-run `npm run download:fonts` if font files need updating

## 🐛 Troubleshooting

### Styles not applying
- Check if `output.css` exists in `wwwroot/`
- Run `npm run build:css`
- Clear browser cache

### Fonts not loading
- Check if `wwwroot/fonts/` directory exists with .woff2 files
- Run `npm run download:fonts`
- Verify `fonts.css` is included in index.html

### Build errors
- Ensure Node.js is installed (v16+ recommended)
- Delete `node_modules` and run `npm install` again
- Check for Tailwind syntax errors in Razor files

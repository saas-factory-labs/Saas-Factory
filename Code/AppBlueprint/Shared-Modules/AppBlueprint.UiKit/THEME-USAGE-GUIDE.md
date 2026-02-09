# Tailwind Theme System - Usage Guide

## Overview

The UiKit now includes a **pure Tailwind CSS theming system** that enables:
- ✅ **Multi-application support** from a single codebase (SaaS, Dating, CRM, E-commerce)
- ✅ **Dynamic Tailwind classes** - no custom CSS needed
- ✅ **Configurable content** - customize labels per application type
- ✅ **Runtime theme switching** - change themes without recompilation

---

## Quick Start

### 1. Register ThemeService in Program.cs

```csharp
// Option A: Load from appsettings.json
builder.Services.AddUiKitWithTheme(builder.Configuration);

// Option B: Programmatic configuration
builder.Services.AddUiKitWithTheme(theme =>
{
    theme.ApplicationType = "dating";
    theme.PrimaryColor = "pink";
    theme.AccentColor = "rose";
    theme.BrandName = "LoveConnect";
    theme.CustomLabels["analytics.topProducts"] = "Top Matches";
});
```

### 2. Configure appsettings.json

```json
{
  "Theme": {
    "ApplicationType": "dating",
    "PrimaryColor": "pink",
    "AccentColor": "rose",
    "BrandName": "LoveConnect",
    "CustomLabels": {
      "analytics.topProducts": "Top Matches",
      "feed.posts": "Profiles"
    }
  }
}
```

See example configurations in:
- `appsettings.Dating.example.json`
- `appsettings.CRM.example.json`
- `appsettings.Ecommerce.example.json`

---

## Component Usage

### Inject ThemeService

```razor
@inject ThemeService ThemeService
```

### Dynamic Tailwind Classes

```razor
<!-- Primary colored button -->
<button class="@ThemeService.GetPrimaryButtonClasses()">
    Click Me
</button>

<!-- Custom colored background -->
<div class="@ThemeService.GetPrimaryClass("bg", "500")">
    Primary background
</div>

<!-- Multiple classes -->
<h1 class="@ThemeService.GetPrimaryClass("text", "600") @ThemeService.GetPrimaryDarkClass("text", "400")">
    Themed Heading
</h1>

<!-- Hover effects -->
<a class="@ThemeService.GetLinkClasses()">
    Themed Link
</a>

<!-- Badges -->
<span class="@ThemeService.GetBadgeClasses()">
    New
</span>
```

### Themed Content

```razor
<!-- Application-specific labels -->
<h2>@ThemeService.GetLabelByType(
    saasLabel: "Top Products",
    datingLabel: "Top Matches",
    crmLabel: "Top Deals",
    ecommerceLabel: "Bestsellers"
)</h2>

<!-- Custom label with fallback -->
<p>@ThemeService.GetLabel("dashboard.welcome", "Welcome")</p>
```

---

## Available Methods

### Color Class Generation

| Method                                  | Example     | Result                     |
|-----------------------------------------|-------------|----------------------------|
| `GetPrimaryClass("bg", "500")`          | Pink theme  | `"bg-pink-500"`            |
| `GetAccentClass("text", "600")`         | Rose theme  | `"text-rose-600"`          |
| `GetPrimaryHoverClass("bg", "600")`     | Pink theme  | `"hover:bg-pink-600"`      |
| `GetPrimaryFocusClass("ring", "500")`   | Pink theme  | `"focus:ring-pink-500"`    |
| `GetPrimaryDarkClass("text", "400")`    | Pink theme  | `"dark:text-pink-400"`     |

### Pre-built Component Classes

| Method                        | Use Case                |
|-------------------------------|-------------------------|
| `GetPrimaryButtonClasses()`   | Solid primary buttons   |
| `GetOutlineButtonClasses()`   | Outline/ghost buttons   |
| `GetBadgeClasses()`           | Status badges           |
| `GetLinkClasses()`            | Themed hyperlinks       |
| `GetCardAccentClasses()`      | Colored card borders    |


### Gradient Backgrounds

| Method                           | Use Case                            | Example                                                           |
|----------------------------------|-------------------------------------|-------------------------------------------------------------------|
| `GetPrimaryGradient()`           | Subtle gradient (transparent)       | `bg-gradient-to-r from-violet-500/[0.12] to-violet-500/[0.04]`   |
| `GetPrimaryGradientWithDark()`   | Gradient with dark mode             | Used for active navigation states                                 |
| `GetPrimarySolidGradient()`      | Solid gradient (no transparency)    | Buttons and prominent elements                                    |
| `GetAccentGradient()`            | Accent color gradient               | Alternative gradient styling                                      |

- `direction`: `"to-r"`, `"to-l"`, `"to-t"`, `"to-b"`, `"to-br"`, etc.
- `fromShade`/`toShade`: Tailwind shades (`"50"` to `"950"`)
- `fromOpacity`/`toOpacity`: Opacity values (e.g., `"0.12"`, `"0.24"`)
- `lightOpacity`/`darkOpacity`: Separate light/dark mode opacities

**Gradient Examples:**

```razor
<!-- Active navigation item (with dark mode) -->
<li class="@(IsActive ? ThemeService.GetPrimaryGradientWithDark() + " rounded-lg" : "")">
    <a href="/dashboard">Dashboard</a>
</li>

<!-- Custom gradient direction and opacity -->
<div class="@ThemeService.GetPrimaryGradient("to-br", "500", "600", "0.20", "0.10")">
    Featured content
</div>

<!-- Solid gradient button -->
<button class="@ThemeService.GetPrimarySolidGradient("to-r", "500", "600") text-white px-4 py-2 rounded-lg">
    Call to Action
</button>

<!-- Accent gradient -->
<div class="@ThemeService.GetAccentGradient("to-r", "400", "600", "0.15", "0.05")">
    Accent highlight
</div>
```

### Content Configuration

| Method                      | Purpose                           |
|-----------------------------|-----------------------------------|
| `GetLabel(key, default)`    | Get custom label with fallback    |
| `GetLabelByType(...)`       | Get label by app type             |
| `CurrentTheme.BrandName`    | Access brand name                 |
| `CurrentTheme.LogoUrl`      | Access logo URL                   |


## Complete Example Component

```razor
@inject ThemeService ThemeService
@implements IDisposable

<div class="bg-white dark:bg-gray-800 shadow-xs rounded-xl @ThemeService.GetCardAccentClasses()">
    <header class="px-5 py-4 border-b border-gray-100 dark:border-gray-700/60">
        <div class="flex items-center justify-between">
            <h2 class="font-semibold @ThemeService.GetPrimaryClass("text", "600") @ThemeService.GetPrimaryDarkClass("text", "400")">
                @_title
            </h2>
            <span class="@ThemeService.GetBadgeClasses()">
                Live
            </span>
        </div>
    </header>
    
    <div class="p-5">
        <p class="text-gray-600 dark:text-gray-300 mb-4">
            @ThemeService.GetLabel("analytics.description", "View your analytics")
        </p>
        
        <button class="@ThemeService.GetPrimaryButtonClasses()">
            @ThemeService.GetLabel("analytics.viewMore", "View Details")
        </button>
    </div>
</div>

@code {
    private string _title = "Analytics";

    protected override void OnInitialized()
    {
        // Set title based on app type
        _title = ThemeService.GetLabelByType(
            saasLabel: "Analytics Dashboard",
            datingLabel: "Match Analytics",
            crmLabel: "Sales Analytics",
            ecommerceLabel: "Store Analytics"
        );
        
        // Subscribe to theme changes
        ThemeService.OnThemeChanged += StateHasChanged;
    }

    public void Dispose()
    {
        ThemeService.OnThemeChanged -= StateHasChanged;
    }
}
```

---

## Tailwind Safelist Configuration

Add to your `tailwind.config.js` to ensure dynamic classes aren't purged:

```javascript
module.exports = {
  content: [
    './Components/**/*.razor',
    './Pages/**/*.razor'
  ],
  safelist: [
    {
      pattern: /^(bg|text|border|ring)-(slate|gray|red|orange|amber|yellow|lime|green|emerald|teal|cyan|sky|blue|indigo|violet|purple|fuchsia|pink|rose)-(50|100|200|300|400|500|600|700|800|900|950)$/,
      variants: ['hover', 'focus', 'dark', 'dark:hover']
    }
  ],
  theme: {
    extend: {}
  }
}
```

---

## Runtime Theme Switching

```razor
@inject ThemeService ThemeService

<div class="space-x-2">
    <button @onclick="SwitchToSaaS" class="px-4 py-2 rounded-lg bg-violet-500 text-white">
        SaaS Mode
    </button>
    <button @onclick="SwitchToDating" class="px-4 py-2 rounded-lg bg-pink-500 text-white">
        Dating Mode
    </button>
    <button @onclick="SwitchToCRM" class="px-4 py-2 rounded-lg bg-blue-500 text-white">
        CRM Mode
    </button>
</div>

@code {
    private void SwitchToSaaS()
    {
        ThemeService.SetTheme(new ThemeConfiguration
        {
            ApplicationType = "saas",
            PrimaryColor = "violet",
            AccentColor = "sky"
        });
    }

    private void SwitchToDating()
    {
        ThemeService.SetTheme(new ThemeConfiguration
        {
            ApplicationType = "dating",
            PrimaryColor = "pink",
            AccentColor = "rose",
            CustomLabels = new Dictionary<string, string>
            {
                ["analytics.topProducts"] = "Top Matches"
            }
        });
    }

    private void SwitchToCRM()
    {
        ThemeService.SetTheme(new ThemeConfiguration
        {
            ApplicationType = "crm",
            PrimaryColor = "blue",
            AccentColor = "emerald",
            CustomLabels = new Dictionary<string, string>
            {
                ["analytics.topProducts"] = "Top Deals"
            }
        });
    }
}
```

---

## Supported Tailwind Colors

All standard Tailwind colors are supported:
- Grays: `slate`, `gray`, `zinc`, `neutral`, `stone`
- Colors: `red`, `orange`, `amber`, `yellow`, `lime`, `green`, `emerald`, `teal`, `cyan`, `sky`, `blue`, `indigo`, `violet`, `purple`, `fuchsia`, `pink`, `rose`

Shades: `50`, `100`, `200`, `300`, `400`, `500`, `600`, `700`, `800`, `900`, `950`

---

## Migration from Hard-coded Components

**Before:**
```razor
<div class="bg-violet-500 text-white">
    <h2>Top Products</h2>
</div>
```

**After:**
```razor
@inject ThemeService ThemeService

<div class="@ThemeService.GetPrimaryClass("bg", "500") text-white">
    <h2>@ThemeService.GetLabelByType("Top Products", "Top Matches", "Top Deals", "Bestsellers")</h2>
</div>
```

---

## Best Practices

1. **Always inject ThemeService** at component level
2. **Subscribe to OnThemeChanged** for reactive updates
3. **Dispose subscriptions** to prevent memory leaks
4. **Use GetLabelByType()** for app-specific content
5. **Safelist dynamic colors** in Tailwind config
6. **Load theme early** in Program.cs startup

---

## Troubleshooting

**Classes not applying?**
- Verify Tailwind safelist configuration
- Check that colors are valid Tailwind palette names
- Rebuild Tailwind CSS after config changes

**Labels not showing?**
- Verify CustomLabels dictionary keys match usage
- Check fallback values are provided
- Ensure theme loaded before component render

**Theme not switching?**
- Verify OnThemeChanged subscription
- Check StateHasChanged() is called
- Ensure ThemeService is singleton (registered correctly)

---

## Advanced Configuration

### NuGet Package Architecture

The UiKit is designed to be consumed as an **immutable NuGet package**, enabling different consuming applications to apply different themes without modifying package code.

**Key Architecture Principles:**

1. ✅ **Package Code Never Changes** - Components use semantic tokens (`GetPrimaryClass()`) instead of hardcoded colors
2. ✅ **Configuration in Consuming App** - Each app (SaaS, Dating, CRM, E-commerce) configures theme via `appsettings.json` or `Program.cs`
3. ✅ **Reactive Updates** - Components subscribe to `OnThemeChanged` for runtime theme switching
4. ✅ **Zero Component Modifications** - All customization happens through configuration

**Why This Matters:**

```csharp
// ❌ WRONG - Hardcoded (requires modifying component for different apps)
<div class="bg-violet-500 text-violet-50">Product</div>

// ✅ CORRECT - Semantic token (works across all apps via configuration)
<div class="@ThemeService.GetPrimaryClass("bg", "500") @ThemeService.GetPrimaryClass("text", "50")">
    @ThemeService.GetLabelByType("Product", "Match", "Deal", "Item")
</div>
```

With the correct approach:
- **SaaS app** configures `PrimaryColor = "violet"` → renders `bg-violet-500`
- **Dating app** configures `PrimaryColor = "pink"` → renders `bg-pink-500`
- **CRM app** configures `PrimaryColor = "blue"` → renders `bg-blue-500`
- **All from the same immutable package!**

---

### Shade Customization

Control the default color shades globally without modifying components.

**Configuration:**

```csharp
// Program.cs
builder.Services.AddUiKitWithTheme(theme =>
{
    theme.ApplicationType = "dating";
    theme.PrimaryColor = "rose";
    theme.AccentColor = "pink";
    
    // ✨ NEW: Configure default shades
    theme.DefaultPrimaryShade = "400";  // Softer primary (default: "500")
    theme.DefaultAccentShade = "300";   // Even softer accent (default: "500")
});
```

**Appsettings.json:**

```json
{
  "Theme": {
    "ApplicationType": "dating",
    "PrimaryColor": "rose",
    "AccentColor": "pink",
    "DefaultPrimaryShade": "400",
    "DefaultAccentShade": "300"
  }
}
```

**How It Works:**

```csharp
// ThemeService method signatures
public string GetPrimaryClass(string prefix, string? shade = null)
public string GetAccentClass(string prefix, string? shade = null)
```

**Usage in Components:**

```razor
<!-- Use default shade from configuration -->
<div class="@ThemeService.GetPrimaryClass("bg")">
    Default shade (uses DefaultPrimaryShade from config)
</div>

<!-- Override with explicit shade -->
<div class="@ThemeService.GetPrimaryClass("bg", "700")">
    Darker shade (ignores config, uses 700)
</div>

<!-- Use default accent shade -->
<span class="@ThemeService.GetAccentClass("text")">
    Default accent text (uses DefaultAccentShade from config)
</span>
```

**Key Benefits:**

- ✅ **Zero Component Changes** - Existing components immediately use new default shades
- ✅ **Global Control** - Change all primary/accent colors at once via configuration
- ✅ **Per-Call Override** - Still allow explicit shades when needed (`GetPrimaryClass("bg", "700")`)
- ✅ **NuGet Immutability** - Package code never changes, configuration drives behavior

**Example: Softer Dating Theme**

```json
{
  "Theme": {
    "ApplicationType": "dating",
    "PrimaryColor": "rose",
    "AccentColor": "pink",
    "DefaultPrimaryShade": "400",
    "DefaultAccentShade": "300",
    "BrandName": "LoveConnect"
  }
}
```

> **Note:** `DefaultPrimaryShade` is set to `"400"` for a softer rose color, and `DefaultAccentShade` is set to `"300"` for an even softer pink accent (defaults are `"500"` if not specified).

With this configuration:
- `GetPrimaryClass("bg")` → `bg-rose-400` (not 500)
- `GetAccentClass("text")` → `text-pink-300` (not 500)
- `GetPrimaryClass("bg", "600")` → `bg-rose-600` (explicit override)

---

### Custom Class Overrides (Advanced)

For component-specific class customization, use `CustomClassOverrides` dictionary.

**⚠️ Warning:** This approach requires component modifications and breaks NuGet immutability. **Prefer DefaultShade configuration instead.**

**Configuration:**

```csharp
theme.CustomClassOverrides = new Dictionary<string, string>
{
    ["productCard.badge"] = "bg-gradient-to-r from-pink-400 to-rose-400 text-white font-bold",
    ["header.logo"] = "text-rose-600 hover:text-rose-700"
};
```

**Component Usage (requires modification):**

```razor
<div class="@ThemeService.GetClassesOrOverride("productCard.badge", GetAccentClass("bg") + " " + GetAccentClass("text"))">
    Sale
</div>
```

**When to Use:**

- ✅ In-house components (not distributed via NuGet)
- ✅ Highly specific customizations
- ❌ **DO NOT USE** for NuGet packages (breaks immutability)

---

### Multiple Custom Labels

Configure unlimited custom labels for application-specific terminology.

**Configuration:**

```csharp
// Program.cs
builder.Services.AddUiKitWithTheme(theme =>
{
    theme.ApplicationType = "dating";
    theme.PrimaryColor = "pink";
    theme.AccentColor = "rose";
    
    theme.CustomLabels = new Dictionary<string, string>
    {
        // Analytics section
        ["analytics.topProducts"] = "Top Matches",
        ["analytics.revenue"] = "Subscription Revenue",
        ["analytics.conversion"] = "Match Rate",
        
        // User section
        ["user.profile"] = "My Profile",
        ["user.settings"] = "Preferences",
        ["user.matches"] = "My Matches",
        
        // Actions
        ["action.create"] = "Create Profile",
        ["action.save"] = "Save Match",
        ["action.delete"] = "Remove Match"
    };
});
```

# Appsettings.json: #

```json
{
  "Theme": {
    "CustomLabels": {
      "analytics.topProducts": "Top Matches",
      "analytics.revenue": "Subscription Revenue",
      "analytics.conversion": "Match Rate",
      "user.profile": "My Profile",
      "user.matches": "My Matches",
      "action.create": "Create Profile"
    }
  }
}
```

**Component Usage:**

```razor
<h2>@ThemeService.GetCustomLabel("analytics.topProducts", "Top Products")</h2>
<p>@ThemeService.GetCustomLabel("analytics.conversion", "Conversion Rate")</p>
<button>@ThemeService.GetCustomLabel("action.create", "Create")</button>
```

**Naming Convention:**

Use dot-separated keys for organization:
- `section.item` - General pattern
- `analytics.*` - Analytics-related labels
- `user.*` - User-related labels
- `action.*` - Action button labels
- `status.*` - Status badge labels

---

## Complete Multi-App Examples

### Example 1: Dating App (Rose/Pink Theme)

**appsettings.Dating.json:**

```json
{
  "Theme": {
    "ApplicationType": "dating",
    "PrimaryColor": "rose",
    "AccentColor": "pink",
    "DefaultPrimaryShade": "400",
    "DefaultAccentShade": "300",
    "BrandName": "LoveConnect",
    "LogoUrl": "/images/logo-dating.svg",
    "CustomLabels": {
      "analytics.topProducts": "Top Matches",
      "analytics.revenue": "Premium Subscriptions",
      "analytics.conversion": "Match Success Rate",
      "dashboard.welcome": "Welcome to Your Matches",
      "feed.title": "Discover Profiles",
      "user.profile": "My Dating Profile",
      "action.like": "Send Like",
      "action.message": "Start Chat"
    }
  }
}
```

**Program.cs:**

```csharp
builder.Services.AddUiKitWithTheme(builder.Configuration);
```

**Result:**
- Primary colors: Soft rose (400 shade)
- Accent colors: Very soft pink (300 shade)
- All "Product" labels → "Match"
- All "Revenue" labels → "Premium Subscriptions"
- All "Conversion" labels → "Match Success Rate"

---

### Example 2: CRM App (Blue/Emerald Theme)

**appsettings.CRM.json:**

```json
{
  "Theme": {
    "ApplicationType": "crm",
    "PrimaryColor": "blue",
    "AccentColor": "emerald",
    "DefaultPrimaryShade": "600",
    "DefaultAccentShade": "500",
    "BrandName": "SalesPro",
    "LogoUrl": "/images/logo-crm.svg",
    "CustomLabels": {
      "analytics.topProducts": "Top Deals",
      "analytics.revenue": "Total Revenue",
      "analytics.conversion": "Close Rate",
      "dashboard.welcome": "Welcome to Your Pipeline",
      "feed.title": "Recent Activity",
      "user.profile": "My CRM Profile",
      "action.create": "Create Deal",
      "action.contact": "Contact Lead"
    }
  }
}
```

**Result:**
- Primary colors: Darker blue (600 shade)
- Accent colors: Emerald green (500 shade)
- All "Product" labels → "Deal"
- All "Revenue" labels → "Total Revenue"
- All "Conversion" labels → "Close Rate"

---

### Example 3: E-commerce App (Violet/Green Theme)

**appsettings.Ecommerce.json:**

```json
{
  "Theme": {
    "ApplicationType": "ecommerce",
    "PrimaryColor": "violet",
    "AccentColor": "green",
    "DefaultPrimaryShade": "500",
    "DefaultAccentShade": "600",
    "BrandName": "ShopMaster",
    "LogoUrl": "/images/logo-shop.svg",
    "CustomLabels": {
      "analytics.topProducts": "Bestsellers",
      "analytics.revenue": "Total Sales",
      "analytics.conversion": "Purchase Rate",
      "dashboard.welcome": "Store Dashboard",
      "feed.title": "Product Catalog",
      "user.profile": "My Store Profile",
      "action.create": "Add Product",
      "action.buy": "Add to Cart"
    }
  }
}
```

**Result:**
- Primary colors: Violet (500 shade - standard)
- Accent colors: Darker green (600 shade)
- All "Product" labels → "Bestseller" (when appropriate)
- All "Revenue" labels → "Total Sales"
- All "Conversion" labels → "Purchase Rate"

---

### Example 4: SaaS App (Violet/Sky Theme - Default)

**appsettings.json:**

```json
{
  "Theme": {
    "ApplicationType": "saas",
    "PrimaryColor": "violet",
    "AccentColor": "sky",
    "BrandName": "AppBlueprint"
  }
}
```

**Result:**
- Primary colors: Violet (500 shade - default when not specified)
- Accent colors: Sky (500 shade - default when not specified)
- All labels use default terminology (Products, Revenue, Conversion Rate)

---

## Component Implementation Patterns

### Pattern 1: Reactive Component with Theme Subscription

Use when component needs to update on theme changes:

```razor
@inject ThemeService ThemeService
@implements IDisposable

<div class="@ThemeService.GetPrimaryClass("bg") p-4 rounded-lg">
    <h2 class="@ThemeService.GetPrimaryClass("text", "50") font-bold">
        @_headerTitle
    </h2>
</div>

@code {
    private string _headerTitle = "Default";

    protected override void OnInitialized()
    {
        // Set dynamic content based on theme
        _headerTitle = ThemeService.GetLabelByType(
            saasLabel: "Analytics",
            datingLabel: "Match Stats",
            crmLabel: "Sales Metrics",
            ecommerceLabel: "Store Insights"
        );

        // ✅ CRITICAL: Subscribe to theme changes for reactive updates
        ThemeService.OnThemeChanged += StateHasChanged;
    }

    public void Dispose()
    {
        // ✅ CRITICAL: Unsubscribe to prevent memory leaks
        ThemeService.OnThemeChanged -= StateHasChanged;
    }
}
```

**Why Subscribe to OnThemeChanged?**

Even though components use `GetPrimaryClass()` (semantic tokens), they need to **reactively re-render** when theme changes at runtime:

1. User clicks "Switch to Dating Mode" button
2. `ThemeService.SetTheme()` is called
3. `OnThemeChanged` event fires
4. All subscribed components call `StateHasChanged()`
5. Components re-render with new theme colors

**Without subscription:** Components would render initial theme but never update on runtime changes.

---

### Pattern 2: Static Component (No Subscription Needed)

Use when component doesn't need runtime theme switching:

```razor
@inject ThemeService ThemeService

<div class="@ThemeService.GetPrimaryClass("bg", "500") p-4 rounded-lg">
    <p class="text-white">Static themed content</p>
</div>
```

**When to skip subscription:**
- Component is static and won't be visible during theme changes
- Application doesn't support runtime theme switching
- Component is recreated on navigation (subscription unnecessary)

---

### Pattern 3: Helper Methods for Complex Logic

Use when Razor inline expressions become too complex:

```razor
@inject ThemeService ThemeService

@foreach (var product in products)
{
    <div class="@GetProductBadgeClasses(product.IsSale)">
        @product.Price
    </div>
}

@code {
    // ✅ Move complex logic to helper method
    private string GetProductBadgeClasses(bool isSale)
    {
        if (isSale)
        {
            return "bg-red-500/20 text-red-700 dark:text-red-400";
        }
        
        return $"{ThemeService.GetAccentClass("bg")}/20 {ThemeService.GetAccentClass("text")} dark:{ThemeService.GetAccentClass("text", "400")}";
    }
}
```

**Why use helper methods?**

Razor syntax errors occur with nested string interpolation:

```razor
<!-- ❌ SYNTAX ERROR -->
<div class="@(product.IsSale ? "bg-red-500" : $"{ThemeService.GetAccentClass("bg")}")">

<!-- ✅ CORRECT - Use helper method instead -->
<div class="@GetProductBadgeClasses(product.IsSale)">
```

---

## Migration Guide: From Hardcoded to Themed

### Step 1: Identify Hardcoded Colors

Search codebase for Tailwind color classes:

```bash
# Find hardcoded Tailwind colors
grep -r "text-violet-" Components/
grep -r "bg-violet-" Components/
grep -r "text-sky-" Components/
grep -r "bg-green-" Components/
```

### Step 2: Inject ThemeService

Add to component top:

```razor
@inject AppBlueprint.UiKit.Services.ThemeService ThemeService
```

### Step 3: Replace Hardcoded Classes

**Before:**
```razor
<div class="bg-violet-500 text-white">
    <span class="text-green-600">Revenue: $1,234</span>
</div>
```

**After:**
```razor
<div class="@ThemeService.GetPrimaryClass("bg", "500") text-white">
    <span class="@ThemeService.GetAccentClass("text", "600")">Revenue: $1,234</span>
</div>
```

### Step 4: Replace Hardcoded Labels

**Before:**
```razor
<h2>Top Products</h2>
```

**After:**
```razor
<h2>@ThemeService.GetLabelByType(
    saasLabel: "Top Products",
    datingLabel: "Top Matches",
    crmLabel: "Top Deals",
    ecommerceLabel: "Bestsellers"
)</h2>
```

### Step 5: Add Subscription (If Needed)

```razor
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        ThemeService.OnThemeChanged += StateHasChanged;
    }

    public void Dispose()
    {
        ThemeService.OnThemeChanged -= StateHasChanged;
    }
}
```

### Step 6: Build and Verify

```bash
dotnet build
```

Verify:
- ✅ No Razor syntax errors
- ✅ Colors apply correctly in UI
- ✅ Theme switching works (if implemented)
- ✅ Labels change based on ApplicationType

---

## Performance Considerations

### String Concatenation

ThemeService uses efficient string concatenation:

```csharp
// ✅ Efficient
public string GetPrimaryClass(string prefix, string? shade = null)
{
    string resolvedShade = shade ?? _configuration.DefaultPrimaryShade;
    return $"{prefix}-{_configuration.PrimaryColor}-{resolvedShade}";
}
```

### Caching

Consider caching frequently used classes:

```csharp
private string? _cachedPrimaryButtonClasses;

public string GetPrimaryButtonClasses()
{
    if (_cachedPrimaryButtonClasses == null)
    {
        _cachedPrimaryButtonClasses = $"px-4 py-2 rounded-lg {GetPrimaryClass("bg", "500")} text-white ...";
    }
    return _cachedPrimaryButtonClasses;
}
```

**Note:** Cache must be cleared on theme change:

```csharp
public void SetTheme(ThemeConfiguration newConfig)
{
    _configuration = newConfig;
    _cachedPrimaryButtonClasses = null; // Clear cache
    OnThemeChanged?.Invoke();
}
```

---

## FAQ

**Q: Do I need to modify UI components when changing themes?**

**A:** No! With shade customization, change `DefaultPrimaryShade` and `DefaultAccentShade` in configuration. Existing components immediately use new shades.

---

**Q: How do I set multiple custom labels?**

**A:** Use `CustomLabels` dictionary in Program.cs or appsettings.json:

```csharp
theme.CustomLabels = new Dictionary<string, string>
{
    ["analytics.topProducts"] = "Top Matches",
    ["user.profile"] = "Dating Profile",
    ["action.like"] = "Send Like"
};
```

---

**Q: Can I override shades per component?**

**A:** Yes! Use explicit shade parameter:

```razor
<!-- Use default shade from config -->
<div class="@ThemeService.GetPrimaryClass("bg")">

<!-- Override with explicit shade -->
<div class="@ThemeService.GetPrimaryClass("bg", "700")">
```

---

**Q: Should I use CustomClassOverrides?**

**A:** **Avoid for NuGet packages.** It requires component modifications and breaks immutability. Use `DefaultPrimaryShade` and `DefaultAccentShade` instead for global shade control without code changes.

---

**Q: Why subscribe to OnThemeChanged if components work without it?**

**A:** Subscription enables **runtime theme switching**. Without it, components render initial theme but never update when user clicks "Switch Theme" button. If your app doesn't support runtime switching, subscription is optional.

---

**Q: How do I test different themes during development?**

**A:** Three options:

1. **appsettings.Development.json** - Different theme per environment
2. **Runtime Switcher** - Add UI button calling `ThemeService.SetTheme()`
3. **Multiple Configurations** - Create appsettings.Dating.json, appsettings.CRM.json, launch with `--environment Dating`

---

**Q: Can I use gradients with custom shades?**

**A:** Yes! All gradient methods support custom shades:

```razor
<div class="@ThemeService.GetPrimaryGradient("to-r", "300", "500")">
    Softer gradient
</div>
```

---

## Summary

The UiKit theming system provides:

✅ **Immutable NuGet Package** - Zero code changes for different apps
✅ **Configuration-Driven** - appsettings.json or Program.cs
✅ **Shade Customization** - Global defaults via DefaultPrimaryShade/DefaultAccentShade
✅ **Label Customization** - Unlimited custom labels via CustomLabels dictionary
✅ **Runtime Switching** - Change themes without recompilation
✅ **Tailwind Native** - Pure Tailwind classes, no custom CSS

**Next Steps:**

1. Configure theme in `appsettings.json`
2. Inject `ThemeService` in components
3. Replace hardcoded colors with `GetPrimaryClass()` / `GetAccentClass()`
4. Replace hardcoded labels with `GetLabelByType()` / `GetCustomLabel()`
5. Subscribe to `OnThemeChanged` for reactive updates
6. Build and verify!

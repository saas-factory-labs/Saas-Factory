# Blazor JavaScript Interop Best Practices

This document outlines critical patterns to prevent crashes and navigation issues in Blazor applications with JavaScript interop.

## Critical Issues Solved (2026-01-12)

### Issue 1: Navigation Hanging and Page Not Loading
**Symptom**: URL changes but content doesn't render, pages hang during navigation
**Root Cause**: Missing `@rendermode` directive or incorrect render mode configuration
**Solution**: 
- Set render mode **globally once** in `App.razor` on the `<Routes>` component
- **NEVER** add `@rendermode` directives to individual page components
- This prevents render mode conflicts and ensures consistent navigation behavior

### Issue 2: JavaScript "Cannot read properties of undefined" Errors
**Symptom**: `Cannot read properties of undefined (reading 'destroy')` or similar errors in JavaScript managers
**Root Cause**: JavaScript code checking `!== null` when property is `undefined`
**Solution**: Always check both `undefined` AND `null` before accessing properties:
```javascript
// ❌ WRONG - Only checks null
if (this.instances[input.id] !== null) {
    this.instances[input.id].destroy();
}

// ✅ CORRECT - Checks both undefined and null
if (this.instances[input.id] !== undefined && this.instances[input.id] !== null) {
    try {
        this.instances[input.id].destroy();
    } catch (e) {
        console.warn('Failed to destroy instance:', e);
    }
}
```

### Issue 3: ElementReference Without ID Causing JavaScript Errors
**Symptom**: JavaScript tries to access `element.id` but it's `undefined`
**Root Cause**: Blazor `ElementReference` passed to JavaScript doesn't have an `id` attribute
**Solution**: Always add unique IDs to elements that will be passed to JavaScript:
```razor
@* ❌ WRONG - No ID attribute *@
<input @ref="inputRef" type="text" />

@* ✅ CORRECT - Unique ID added *@
<input @ref="inputRef" id="@elementId" type="text" />

@code {
    private ElementReference inputRef;
    private string elementId = $"element-{Guid.NewGuid():N}";
}
```

### Issue 4: JavaScript Function vs Property Confusion
**Symptom**: Error "The value 'manager.isAvailable' is not a function"
**Root Cause**: C# code calling a JavaScript function as a property
**Solution**: Ensure JavaScript functions are actually functions and C# calls them correctly:
```javascript
// ✅ JavaScript - isAvailable as a function
const manager = {
    isAvailable() {
        return typeof window.manager !== 'undefined';
    }
};
```
```csharp
// ✅ C# - Call as async function
bool hasManager = await JS.InvokeAsync<bool>("manager.isAvailable");
```

## Render Mode Guidelines

### Global Render Mode Configuration (REQUIRED)
**App.razor** - Set once for entire application:
```razor
<!DOCTYPE html>
<html>
<body>
    <Routes @rendermode="RenderMode.InteractiveServer"/>
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

### Page Components (NEVER add @rendermode)
**❌ WRONG** - Do NOT add @rendermode to individual pages:
```razor
@page "/mypage"
@rendermode InteractiveServer  @* ❌ NEVER DO THIS *@
```

**✅ CORRECT** - Pages inherit render mode from Routes:
```razor
@page "/mypage"
@* No @rendermode directive needed *@
```

### Why This Matters
- Multiple `@rendermode` directives cause render mode conflicts
- Conflicts lead to navigation hanging, pages not loading, or double rendering
- Global configuration ensures consistent behavior across all pages
- Easier to maintain - change once in App.razor, not across hundreds of files

## JavaScript Interop Patterns

### Pattern 1: ElementReference with JavaScript Interop

When passing `ElementReference` to JavaScript that will use `element.id`:

```razor
@* Component.razor *@
<input @ref="inputRef" id="@uniqueId" type="text" />

@code {
    private ElementReference inputRef;
    private string uniqueId = $"input-{Guid.NewGuid():N}";
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("myManager.initialize", inputRef);
        }
    }
}
```

```javascript
// site.js
const myManager = {
    instances: {},
    initialize(element) {
        // Now element.id is guaranteed to exist
        if (this.instances[element.id] !== undefined && this.instances[element.id] !== null) {
            try {
                this.instances[element.id].destroy();
            } catch (e) {
                console.warn('Failed to destroy:', e);
            }
        }
        this.instances[element.id] = createInstance(element);
    }
};
```

### Pattern 2: Safe Property Access in JavaScript

Always check for both `undefined` and `null`:

```javascript
// ❌ WRONG
if (obj.prop !== null) { }
if (obj.prop) { } // Can fail for falsy values like 0, ""

// ✅ CORRECT
if (obj.prop !== undefined && obj.prop !== null) { }

// ✅ ALSO CORRECT - Using optional chaining (modern JS)
obj.prop?.method();
```

### Pattern 3: Defensive JavaScript Manager Initialization

Wrap destructive operations in try-catch:

```javascript
const myManager = {
    instances: {},
    initialize(element) {
        // Check before destroying
        if (this.instances[element.id] !== undefined && this.instances[element.id] !== null) {
            try {
                this.instances[element.id].destroy();
            } catch (e) {
                console.warn(`Failed to destroy ${element.id}:`, e);
            }
        }
        
        // Initialize new instance
        try {
            this.instances[element.id] = createNewInstance(element);
        } catch (e) {
            console.error(`Failed to initialize ${element.id}:`, e);
            throw e; // Re-throw if initialization fails
        }
    },
    cleanup(element) {
        if (this.instances[element.id] !== undefined && this.instances[element.id] !== null) {
            try {
                this.instances[element.id].destroy();
                delete this.instances[element.id];
            } catch (e) {
                console.warn(`Cleanup error for ${element.id}:`, e);
            }
        }
    }
};
```

### Pattern 4: Check JavaScript Function Availability from C#

Before calling JavaScript functions, verify they exist:

```razor
@code {
    private async Task UseJavaScriptManager()
    {
        try
        {
            // Check if the manager exists
            bool hasManager = await JS.InvokeAsync<bool>("myManager.isAvailable");
            if (hasManager)
            {
                await JS.InvokeVoidAsync("myManager.doSomething");
            }
            else
            {
                Console.WriteLine("Manager not available");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling JavaScript: {ex.Message}");
        }
    }
}
```

```javascript
// JavaScript side - always provide isAvailable check
const myManager = {
    isAvailable() {
        return typeof window.myManager !== 'undefined' &&
               typeof window.myManager.doSomething === 'function';
    },
    doSomething() {
        // Implementation
    }
};
```

## Component Lifecycle Best Practices

### OnAfterRenderAsync for JavaScript Interop

Always use `OnAfterRenderAsync` with `firstRender` check for JavaScript initialization:

```razor
@code {
    private ElementReference elementRef;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                // Only initialize JavaScript on first render
                await JS.InvokeVoidAsync("myManager.initialize", elementRef);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing component");
        }
    }
    
    // Cleanup when component is disposed
    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("myManager.cleanup", elementRef);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cleanup error: {ex.Message}");
        }
    }
}
```

## Checklist for New Components

Before committing a new Blazor component with JavaScript interop:

- [ ] **No `@rendermode` directive** on individual page components
- [ ] **Unique ID added** to all elements passed to JavaScript via `ElementReference`
- [ ] **JavaScript checks both undefined AND null** before accessing properties
- [ ] **Try-catch wraps destructive operations** (destroy, cleanup, etc.)
- [ ] **C# checks JavaScript function availability** before calling it
- [ ] **OnAfterRenderAsync uses firstRender check** for initialization
- [ ] **IDisposable/IAsyncDisposable implemented** if JavaScript resources need cleanup
- [ ] **Error logging added** for all JavaScript interop calls

## Testing Checklist

After making changes to JavaScript interop:

- [ ] Navigate between multiple pages - URL changes and content loads correctly
- [ ] Check browser console for JavaScript errors
- [ ] Open/close dropdowns and modals multiple times - no errors
- [ ] Refresh page while on different routes - no initialization errors
- [ ] Test with browser dev tools open to catch warnings

## Common Mistakes to Avoid

1. ❌ **Adding `@rendermode` to individual pages** → Use global configuration only
2. ❌ **Checking only `!== null` in JavaScript** → Always check both `undefined` and `null`
3. ❌ **Missing `id` attribute on elements** → Add unique IDs using Guid
4. ❌ **No try-catch around destroy/cleanup** → Always wrap destructive operations
5. ❌ **Calling JavaScript without checking availability** → Always verify functions exist
6. ❌ **Forgetting firstRender check** → Only initialize JavaScript once
7. ❌ **No cleanup on dispose** → Always cleanup JavaScript resources

## References

- [Microsoft Blazor JS Interop Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/)
- [Blazor Render Modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes)
- Issue resolved: Navigation hanging and JavaScript errors (2026-01-12)

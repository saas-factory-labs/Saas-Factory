using System.Text;

namespace AppBlueprint.AdminPortalKernel.Modules;

/// <summary>
/// Per-app brand tokens supplied by an <see cref="IAdminPortalModule"/> so the DeploymentManager
/// shell renders each SaaS app's pages (e.g. <c>/apps/boligportal/admin</c>) in that app's colors.
/// The shell maps the non-null tokens to CSS custom properties on the app's content container;
/// shared components (primary button, active tab) read them via <c>var(--brand-*, &lt;default&gt;)</c>
/// and fall back to the shell's default violet when a token is absent.
/// </summary>
public sealed record AdminPortalBranding
{
    /// <summary>Primary brand/accent color (CSS color, e.g. <c>#016be9</c>). Maps to <c>--brand-primary</c>.</summary>
    public string? PrimaryColor { get; init; }

    /// <summary>Hover/active shade of <see cref="PrimaryColor"/>. Maps to <c>--brand-primary-hover</c>.</summary>
    public string? PrimaryHoverColor { get; init; }

    /// <summary>Text/icon color rendered on top of <see cref="PrimaryColor"/> (e.g. <c>#ffffff</c>). Maps to <c>--brand-on-primary</c>.</summary>
    public string? OnPrimaryColor { get; init; }

    /// <summary>Button corner radius (CSS length, e.g. <c>0.5rem</c>). Maps to <c>--brand-radius</c>.</summary>
    public string? ButtonRadius { get; init; }

    /// <summary>
    /// Renders the non-null tokens as a <c>style</c>-attribute value of CSS custom properties
    /// (e.g. <c>--brand-primary:#016be9;--brand-radius:0.5rem;</c>). Tokens containing characters
    /// that could break out of an inline style declaration are dropped, so a misconfigured module
    /// can only fail to brand - never inject markup.
    /// </summary>
    public string ToInlineStyle()
    {
        var builder = new StringBuilder();
        AppendVariable(builder, "--brand-primary", PrimaryColor);
        AppendVariable(builder, "--brand-primary-hover", PrimaryHoverColor);
        AppendVariable(builder, "--brand-on-primary", OnPrimaryColor);
        AppendVariable(builder, "--brand-radius", ButtonRadius);
        return builder.ToString();

        static void AppendVariable(StringBuilder builder, string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            // Guard against breaking out of the inline style declaration / attribute.
            if (value.AsSpan().IndexOfAny(";{}<>\"'") >= 0)
            {
                return;
            }

            builder.Append(name).Append(':').Append(value.Trim()).Append(';');
        }
    }
}

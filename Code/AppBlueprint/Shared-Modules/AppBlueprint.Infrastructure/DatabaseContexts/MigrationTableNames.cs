namespace AppBlueprint.Infrastructure.DatabaseContexts;

/// <summary>
/// Dedicated migration history table names for each DbContext.
///
/// Using per-context table names (instead of the default __EFMigrationsHistory) ensures
/// that downstream SaaS apps installing the AppBlueprint NuGet packages can manage their
/// own EF migrations independently in the same database without collision.
///
/// Each AppBlueprint context writes its applied migrations to its own tracking table.
/// Consumer apps continue to use __EFMigrationsHistory (or a table of their choice).
///
/// See Shared-Modules/MIGRATION_STRATEGY.md for migration isolation strategy options.
/// </summary>
internal static class MigrationTableNames
{
    internal const string Application = "__AppBlueprintMigrationsHistory";
    internal const string Baseline = "__AppBlueprintBaselineMigrationsHistory";
    internal const string B2B = "__AppBlueprintB2BMigrationsHistory";
    internal const string B2C = "__AppBlueprintB2CMigrationsHistory";
}

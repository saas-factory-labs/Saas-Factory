using System.Data.Common;
using System.Reflection;
using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class AdminPortalDiagnostics : IAdminPortalDiagnostics
{
    private const string AppDatabaseLabel = "app database";
    private const string DeploymentManagerDatabaseLabel = "DeploymentManager database";

    private readonly AdminPortalModuleRegistry _registry;
    private readonly IOptions<AdminPortalOptions> _options;
    private readonly AdminQuerySession _session;
    private readonly IDbContextFactory<AdminPortalAuditDbContext> _auditContextFactory;

    public AdminPortalDiagnostics(
        AdminPortalModuleRegistry registry,
        IOptions<AdminPortalOptions> options,
        AdminQuerySession session,
        IDbContextFactory<AdminPortalAuditDbContext> auditContextFactory)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(auditContextFactory);
        _registry = registry;
        _options = options;
        _session = session;
        _auditContextFactory = auditContextFactory;
    }

    public async Task<AdminPortalDebugInfo> GetAsync(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        if (!_registry.TryGet(slug, out IAdminPortalModule? module) || module is null)
        {
            throw new InvalidOperationException($"No admin portal module registered for slug '{slug}'.");
        }

        AssemblyName assemblyName = module.RouterAssembly.GetName();
        string maskedConnectionString = _options.Value.Modules.TryGetValue(slug, out AdminPortalModuleOptions? moduleOptions)
            ? PostgresConnectionString.Mask(moduleOptions.ConnectionString)
            : "(not configured)";

        string? currentDatabase = null;
        string? currentUser = null;
        string? serverVersion = null;
        string? error = null;

        try
        {
            (currentDatabase, currentUser, serverVersion) =
                await _session.ExecuteReadAsync(slug, "debug.connection", async context => (
                    await ScalarAsync(context, "current_database()"),
                    await ScalarAsync(context, "current_user"),
                    await ScalarAsync(context, "version()")));
        }
        catch (InvalidOperationException ex)
        {
            // AdminQuerySession wraps DbExceptions as InvalidOperationException with a helpful message.
            error = ex.Message;
        }

        var tables = new List<AdminPortalTableInfo>
        {
            await CountAppTableAsync(slug, "\"Users\"", "SELECT count(*)::int AS \"Value\" FROM \"Users\""),
            await CountAppTableAsync(slug, "\"Tenants\"", "SELECT count(*)::int AS \"Value\" FROM \"Tenants\""),
            await CountAuditTableAsync(slug)
        };

        return new AdminPortalDebugInfo(
            Slug: slug,
            DisplayName: module.DisplayName,
            AssemblyName: assemblyName.Name ?? module.RouterAssembly.GetName().FullName,
            AssemblyVersion: assemblyName.Version?.ToString() ?? "unknown",
            AssemblyLocation: SafeLocation(module.RouterAssembly),
            MaskedConnectionString: maskedConnectionString,
            CurrentDatabase: currentDatabase,
            CurrentUser: currentUser,
            ServerVersion: serverVersion,
            Tables: tables,
            Error: error);
    }

    private async Task<AdminPortalTableInfo> CountAppTableAsync(string slug, string tableName, string countSql)
    {
        try
        {
            int count = await _session.ExecuteReadAsync(slug, $"debug.count {tableName}",
                async context => await context.Database.SqlQueryRaw<int>(countSql).SingleAsync());
            return new AdminPortalTableInfo(tableName, AppDatabaseLabel, Exists: true, count,
                Note: "raw row count, including soft-deleted");
        }
        catch (InvalidOperationException)
        {
            return new AdminPortalTableInfo(tableName, AppDatabaseLabel, Exists: false, RowCount: 0,
                Note: "table not found or not readable on this connection");
        }
    }

    private async Task<AdminPortalTableInfo> CountAuditTableAsync(string slug)
    {
        try
        {
            await using AdminPortalAuditDbContext context = await _auditContextFactory.CreateDbContextAsync();
            int count = await context.AuditEntries.CountAsync(entry => entry.AppSlug == slug);
            return new AdminPortalTableInfo("dm_admin_audit", DeploymentManagerDatabaseLabel, Exists: true, count,
                Note: "audit entries for this app");
        }
        catch (DbException)
        {
            return new AdminPortalTableInfo("dm_admin_audit", DeploymentManagerDatabaseLabel, Exists: false, RowCount: 0,
                Note: "table not found (run the DeploymentManager.ApiService migration)");
        }
    }

    private static async Task<string> ScalarAsync(AdminPortalAppDbContext context, string expression)
    {
        // EF's SqlQuery requires the scalar column to be named "Value".
        return await context.Database
            .SqlQueryRaw<string>($"SELECT {expression} AS \"Value\"")
            .SingleAsync();
    }

    private static string SafeLocation(Assembly assembly)
    {
        try
        {
            return string.IsNullOrEmpty(assembly.Location) ? "(in-memory)" : assembly.Location;
        }
        catch (NotSupportedException)
        {
            return "(in-memory)";
        }
    }
}

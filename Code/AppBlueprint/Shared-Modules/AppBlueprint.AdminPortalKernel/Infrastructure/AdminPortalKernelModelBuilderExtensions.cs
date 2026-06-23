using AppBlueprint.AdminPortalKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Hook for the host's DbContext (DeploymentManagerDbContext) to own the admin portal
/// kernel tables, following the shared kernel model-builder extension pattern.
/// </summary>
public static class AdminPortalKernelModelBuilderExtensions
{
    /// <summary>Adds the kernel's audit table to the host model so the host's migrations create it.</summary>
    public static ModelBuilder ConfigureAdminPortalKernel(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new AdminAuditEntryConfiguration());
        return modelBuilder;
    }
}

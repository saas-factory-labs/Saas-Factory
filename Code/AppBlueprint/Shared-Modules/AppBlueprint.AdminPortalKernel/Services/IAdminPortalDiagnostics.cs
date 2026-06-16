using AppBlueprint.AdminPortalKernel.Domain.Dtos;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Provides connection/module diagnostics for a module's admin Debug tab.</summary>
public interface IAdminPortalDiagnostics
{
    Task<AdminPortalDebugInfo> GetAsync(string slug);
}

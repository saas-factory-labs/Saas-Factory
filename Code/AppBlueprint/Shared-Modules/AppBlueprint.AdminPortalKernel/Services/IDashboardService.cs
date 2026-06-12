using AppBlueprint.AdminPortalKernel.Domain.Dtos;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Key numbers for an app's admin dashboard.</summary>
public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync(string slug);
}

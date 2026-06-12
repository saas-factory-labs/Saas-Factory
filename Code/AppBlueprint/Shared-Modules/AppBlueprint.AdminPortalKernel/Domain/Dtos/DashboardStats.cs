namespace AppBlueprint.AdminPortalKernel.Domain.Dtos;

/// <summary>Key numbers shown on an app's admin dashboard.</summary>
public sealed record DashboardStats(
    int TotalUsers,
    int ActiveUsers,
    int TotalTenants,
    int ActiveTenants,
    int SignupsLast30Days);

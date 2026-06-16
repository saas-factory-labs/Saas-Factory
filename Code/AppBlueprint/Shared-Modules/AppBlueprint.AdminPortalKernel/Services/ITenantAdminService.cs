using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Tenant (customer) overview for a target app.</summary>
public interface ITenantAdminService
{
    Task<PagedResult<AdminTenantRecord>> SearchAsync(string slug, string? nameContains, int page, int pageSize);
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AppBlueprint.Infrastructure.Services;

public class TenantProvider
{
    private const string TenantIdHeaderName = "X-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public int GetTenantId()
    {
        StringValues? tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeaderName];

        if (!tenantIdHeader.HasValue || !int.TryParse(tenantIdHeader.Value, out int tenantId))
            // TODO: also rememter to add check to check if the tenant id actually exist in the tenant catalog database
            throw new ApplicationException("Tenant Id is not present");

        return tenantId;
    }
}

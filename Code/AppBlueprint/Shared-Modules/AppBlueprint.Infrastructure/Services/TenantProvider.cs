using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AppBlueprint.Infrastructure.Services;

public class TenantProvider
{
    private const string TenantIdHeaderName = "X-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetTenantId()
    {
        StringValues? tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeaderName];

        if (!tenantIdHeader.HasValue || string.IsNullOrWhiteSpace(tenantIdHeader.Value))
            // TODO: also remember to add check to check if the tenant id actually exist in the tenant catalog database
            throw new ApplicationException("Tenant Id is not present");

        return tenantIdHeader.Value.ToString();
    }
}

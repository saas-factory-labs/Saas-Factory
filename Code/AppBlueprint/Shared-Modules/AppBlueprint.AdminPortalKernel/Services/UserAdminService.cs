using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class UserAdminService : IUserAdminService
{
    private readonly AdminQuerySession _session;
    private readonly IAdminAuditWriter _auditWriter;

    public UserAdminService(AdminQuerySession session, IAdminAuditWriter auditWriter)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(auditWriter);
        _session = session;
        _auditWriter = auditWriter;
    }

    public Task<PagedResult<AdminUserRecord>> SearchAsync(string slug, UserSearchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        int page = Math.Max(1, request.Page);
        int pageSize = Math.Clamp(request.PageSize, 1, 200);

        return _session.ExecuteReadAsync(slug, "users.search", async context =>
        {
            IQueryable<AdminUserRecord> query = context.Users;

            if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                query = query.Where(user => user.TenantId == request.TenantId);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                string pattern = $"%{EscapeLikePattern(request.SearchText)}%";
                query = query.Where(user =>
                    EF.Functions.ILike(user.Email, pattern)
                    || EF.Functions.ILike(user.UserName, pattern)
                    || EF.Functions.ILike(user.FirstName, pattern)
                    || EF.Functions.ILike(user.LastName, pattern));
            }

            int totalCount = await query.CountAsync();
            List<AdminUserRecord> items = await query
                .OrderByDescending(user => user.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AdminUserRecord>(items, totalCount, page, pageSize);
        });
    }

    public Task<AdminUserRecord?> GetAsync(string slug, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        return _session.ExecuteReadAsync(slug, "users.view",
            context => context.Users.SingleOrDefaultAsync(user => user.Id == userId));
    }

    public async Task<bool> SetActiveAsync(string slug, string userId, bool isActive, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        AdminUserRecord? user = await GetAsync(slug, userId);
        if (user is null)
        {
            return false;
        }

        // Audit before mutating: every attempt is on record even if the update fails.
        string action = isActive ? "user.activate" : "user.deactivate";
        await _auditWriter.WriteAsync(
            slug, action, reason,
            targetType: "User", targetId: userId, tenantId: user.TenantId,
            details: $"{{\"email\":\"{user.Email}\",\"isActive\":{(isActive ? "true" : "false")}}}");

        int affected = await _session.ExecuteWriteAsync(slug, reason, context =>
            context.Users
                .Where(candidate => candidate.Id == userId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(candidate => candidate.IsActive, isActive)));

        return affected > 0;
    }

    private static string EscapeLikePattern(string input) =>
        input.Replace(@"\", @"\\", StringComparison.Ordinal)
            .Replace("%", @"\%", StringComparison.Ordinal)
            .Replace("_", @"\_", StringComparison.Ordinal);
}

namespace AppBlueprint.AdminPortalKernel.Domain.Dtos;

/// <summary>One page of results for the admin portal's server-side paged tables.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

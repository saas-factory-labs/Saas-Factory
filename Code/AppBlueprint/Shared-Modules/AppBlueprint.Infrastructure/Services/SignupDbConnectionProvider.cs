using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Infrastructure implementation of ISignupDbContextProvider.
/// Provides DbContext for signup operations using ApplicationDbContext.
/// </summary>
public sealed class SignupDbContextProvider : ISignupDbContextProvider
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SignupDbContextProvider(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    public async Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _contextFactory.CreateDbContextAsync(cancellationToken);
    }
}

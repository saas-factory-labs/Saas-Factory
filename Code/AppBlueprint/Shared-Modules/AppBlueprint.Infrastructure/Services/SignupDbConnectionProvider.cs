using System.Data.Common;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Infrastructure implementation of ISignupDbConnectionProvider.
/// Provides database connections for signup operations using ApplicationDbContext.
/// </summary>
public sealed class SignupDbConnectionProvider : ISignupDbConnectionProvider
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SignupDbConnectionProvider(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    public async Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        ApplicationDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        DbConnection connection = context.Database.GetDbConnection();
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
        
        return connection;
    }
}

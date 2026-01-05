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
        
        return new OwnedDbConnection(connection, context);
    }
}

/// <summary>
/// Wrapper that owns both a DbConnection and its parent DbContext.
/// Disposing this wrapper disposes both the connection and the context, preventing resource leaks.
/// </summary>
internal sealed class OwnedDbConnection : DbConnection
{
    private readonly DbConnection _innerConnection;
    private readonly ApplicationDbContext _context;
    private bool _disposed;

    public OwnedDbConnection(DbConnection connection, ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(context);

        _innerConnection = connection;
        _context = context;
    }

    public override string ConnectionString
    {
        get => _innerConnection.ConnectionString;
        set => _innerConnection.ConnectionString = value;
    }

    public override string Database => _innerConnection.Database;

    public override string DataSource => _innerConnection.DataSource;

    public override string ServerVersion => _innerConnection.ServerVersion;

    public override System.Data.ConnectionState State => _innerConnection.State;

    public override void ChangeDatabase(string databaseName) => _innerConnection.ChangeDatabase(databaseName);

    public override void Close() => _innerConnection.Close();

    public override void Open() => _innerConnection.Open();

    public override Task OpenAsync(CancellationToken cancellationToken) => _innerConnection.OpenAsync(cancellationToken);

    protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        => _innerConnection.BeginTransaction(isolationLevel);

    protected override DbCommand CreateDbCommand() => _innerConnection.CreateCommand();

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _innerConnection.Dispose();
            _context.Dispose();
        }

        _disposed = true;
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _innerConnection.DisposeAsync();
        await _context.DisposeAsync();

        _disposed = true;
        await base.DisposeAsync();
    }
}

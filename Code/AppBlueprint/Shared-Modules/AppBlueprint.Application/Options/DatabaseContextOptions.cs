namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for Database Context selection.
/// Determines which DbContext type the application should use based on its needs.
/// </summary>
public sealed class DatabaseContextOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "DatabaseContext";

    /// <summary>
    /// The type of database context to use.
    /// Default: B2C (consumer-focused applications).
    /// </summary>
    public DatabaseContextType ContextType { get; set; } = DatabaseContextType.B2C;

    /// <summary>
    /// When true, enables all entity sets from both B2B and B2C contexts.
    /// Use this for applications that need both consumer and organizational features.
    /// Default: false.
    /// </summary>
    public bool EnableHybridMode { get; set; }

    /// <summary>
    /// When true, only registers the baseline DbContext with core entities.
    /// Use this for minimal applications that don't need B2B or B2C specific features.
    /// Default: false.
    /// </summary>
    public bool BaselineOnly { get; set; }

    /// <summary>
    /// Connection string name to use for the database.
    /// Default: "DefaultConnection".
    /// Can be overridden by DATABASE_CONNECTIONSTRING environment variable.
    /// </summary>
    public string ConnectionStringName { get; set; } = "DefaultConnection";

    /// <summary>
    /// Command timeout in seconds for database operations.
    /// Default: 60 seconds.
    /// </summary>
    public int CommandTimeout { get; set; } = 60;

    /// <summary>
    /// Maximum number of retry attempts on transient failures.
    /// Default: 5.
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;

    /// <summary>
    /// Maximum delay between retry attempts in seconds.
    /// Default: 10 seconds.
    /// </summary>
    public int MaxRetryDelaySeconds { get; set; } = 10;

    /// <summary>
    /// Validates the configuration and throws if invalid.
    /// </summary>
    public void Validate()
    {
        if (BaselineOnly && EnableHybridMode)
        {
            throw new InvalidOperationException(
                "Cannot enable both BaselineOnly and EnableHybridMode. Choose one approach.");
        }

        if (CommandTimeout <= 0)
        {
            throw new InvalidOperationException("CommandTimeout must be greater than 0.");
        }

        if (MaxRetryCount < 0)
        {
            throw new InvalidOperationException("MaxRetryCount must be 0 or greater.");
        }

        if (MaxRetryDelaySeconds <= 0)
        {
            throw new InvalidOperationException("MaxRetryDelaySeconds must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(ConnectionStringName))
        {
            throw new InvalidOperationException("ConnectionStringName cannot be null or empty.");
        }
    }
}

/// <summary>
/// Defines the type of database context to use.
/// </summary>
public enum DatabaseContextType
{
    /// <summary>
    /// Baseline DbContext with only core entities (Users, Notifications, Integrations, etc.).
    /// Use for minimal applications or when building custom contexts.
    /// Entities: Core authentication, notifications, integrations, file management.
    /// </summary>
    Baseline,

    /// <summary>
    /// B2C (Business-to-Consumer) DbContext extending Baseline.
    /// Use for consumer-focused SaaS applications (individual users, families, personal accounts).
    /// Entities: Baseline + family relationships, personal preferences, consumer-specific features.
    /// </summary>
    B2C,

    /// <summary>
    /// B2B (Business-to-Business) DbContext extending Baseline.
    /// Use for organization-focused SaaS applications (companies, teams, workspaces).
    /// Entities: Baseline + organizations, teams, API keys, B2B-specific features.
    /// </summary>
    B2B
}

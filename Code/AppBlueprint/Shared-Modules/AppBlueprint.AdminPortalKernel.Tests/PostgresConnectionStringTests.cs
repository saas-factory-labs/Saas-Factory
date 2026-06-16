using AppBlueprint.AdminPortalKernel.Infrastructure;
using FluentAssertions;
using Npgsql;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class PostgresConnectionStringTests
{
    [Test]
    public async Task Normalize_KeywordValueForm_PassesThroughUnchanged()
    {
        const string input = "Host=localhost;Port=5432;Database=app;Username=admin;Password=secret";

        PostgresConnectionString.Normalize(input).Should().Be(input);
        await Task.CompletedTask;
    }

    [Test]
    public async Task Normalize_NeonUri_ProducesNpgsqlParsableConnectionString()
    {
        const string neonUri =
            "postgresql://neondb_owner:npg_secret@ep-blue-mouse-pooler.eu-central-1.aws.neon.tech/neondb?sslmode=require&channel_binding=require";

        string normalized = PostgresConnectionString.Normalize(neonUri);

        // The whole point: Npgsql's builder must accept the result (the raw URI throws).
        var builder = new NpgsqlConnectionStringBuilder(normalized);
        builder.Host.Should().Be("ep-blue-mouse-pooler.eu-central-1.aws.neon.tech");
        builder.Database.Should().Be("neondb");
        builder.Username.Should().Be("neondb_owner");
        builder.Password.Should().Be("npg_secret");
        builder.SslMode.Should().Be(SslMode.Require);
        // channel_binding is not an Npgsql keyword and must have been dropped, not passed through.
        normalized.Should().NotContain("channel_binding");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Mask_KeywordForm_RedactsPasswordOnly()
    {
        const string fakePassword = "FAKE-not-a-real-password";
        string input = $"Host=db.example.com;Port=5432;Database=app;Username=admin;Password={fakePassword};SSL Mode=Require";

        string masked = PostgresConnectionString.Mask(input);

        masked.Should().NotContain(fakePassword);
        masked.Should().Contain("Password=***");
        masked.Should().Contain("Host=db.example.com");
        masked.Should().Contain("Username=admin");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Mask_UriForm_RedactsPasswordOnly()
    {
        const string fakePassword = "FAKE-not-a-real-password";
        string input = $"postgresql://neondb_owner:{fakePassword}@ep-blue-mouse.neon.tech/neondb?sslmode=require";

        string masked = PostgresConnectionString.Mask(input);

        masked.Should().NotContain(fakePassword);
        masked.Should().Contain("neondb_owner:***@");
        masked.Should().Contain("ep-blue-mouse.neon.tech/neondb");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Mask_EmptyInput_ReturnsPlaceholder()
    {
        PostgresConnectionString.Mask(null).Should().Be("(not configured)");
        PostgresConnectionString.Mask("  ").Should().Be("(not configured)");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Normalize_UriWithPortAndEncodedPassword_DecodesCorrectly()
    {
        const string uri = "postgres://user:p%40ss%3Aword@db.example.com:6543/mydb";

        var builder = new NpgsqlConnectionStringBuilder(PostgresConnectionString.Normalize(uri));

        builder.Port.Should().Be(6543);
        builder.Password.Should().Be("p@ss:word");
        builder.Database.Should().Be("mydb");
        await Task.CompletedTask;
    }
}

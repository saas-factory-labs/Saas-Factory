using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Migrations;

/// <inheritdoc />
public partial class InitialB2BDbContext : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // NOTE: This is an EMPTY initial migration because B2BDbContext shares the same database
        // with ApplicationDbContext in Hybrid Mode. All base tables (Admins, Users, Tenants, etc.)
        // were already created by ApplicationDbContext migrations (20250716183440_InitialULIDSchema).
        //
        // This migration establishes the B2B migration history baseline without creating duplicate tables.
        // Future migrations will add B2B-specific tables (ApiKeys, Organizations, Teams, etc.) incrementally.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Nothing to roll back since no changes were made
    }
}

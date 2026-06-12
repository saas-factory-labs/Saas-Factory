# Changelog

All notable changes to AppBlueprint.Infrastructure will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- ConfigurationValidator helper class for centralized configuration validation
- Database connection validation with detailed error messages and setup examples
- Logto authentication configuration validation with partial configuration detection
- URL validation for HTTP/HTTPS format checking
- Support for PostgreSQL database with Entity Framework Core 10.0
- B2B multi-tenancy database context with row-level security
- Authentication providers: Logto, Auth0, and Mock provider
- Web authentication extensions with Data Protection for production environments
- Migration utilities and automatic database creation
- Repository pattern implementation with Unit of Work
- Authorization system with user roles and permissions
- Database contexts for baseline, B2B, and various modules (Chat, Affiliate, Referral, Todo, Dating)

### Changed
- Updated ServiceCollectionExtensions to use ConfigurationValidator for database validation
- Updated WebAuthenticationExtensions to use ConfigurationValidator for Logto validation
- Improved error messages to show environment variable and appsettings.json examples

## [0.1.0] - Unreleased

Initial release preparation.

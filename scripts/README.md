# Scripts Directory

This directory contains utility scripts for development, deployment, and maintenance tasks.

## Directory Structure

- **`powershell/`** - PowerShell scripts for Windows development environment
- **`README.md`** - This file
- **`test_ulid.sql`** - SQL script for testing ULID functionality

## PowerShell Scripts (`powershell/`)

### Deployment & Version Control
- **`commit-and-push.ps1`** - Quick commit script for Railway deployment fixes

## SQL Scripts

### Database Testing
- **`test_ulid.sql`** - Test script for ULID (Universally Unique Lexicographically Sortable Identifier) functionality in PostgreSQL

## Usage

### PowerShell Scripts
Run PowerShell scripts from the repository root:
```powershell
# Example: Run commit script
.\scripts\powershell\commit-and-push.ps1
```

### SQL Scripts
Execute SQL scripts against your PostgreSQL database:
```bash
# Using psql
psql -U username -d database_name -f scripts/test_ulid.sql

# Or copy and paste into your SQL client
```

## Contributing

When adding new scripts:
1. Place them in the appropriate subdirectory (`powershell/`, `bash/`, etc.)
2. Include clear comments explaining purpose and usage
3. Update this README with a description
4. Ensure scripts are cross-platform compatible where possible
5. Add error handling and validation

## Related

- [Documentation](../docs/) - Project documentation
- [Build Scripts](../Code/AppBlueprint/) - Project-specific build scripts


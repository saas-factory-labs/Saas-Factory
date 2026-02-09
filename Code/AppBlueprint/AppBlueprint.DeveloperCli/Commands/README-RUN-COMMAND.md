# `run` Command - Quick Start Development

## Overview
The `run` command is a one-command solution to start your entire SaaS Factory development environment. After installing the CLI globally, use `saas run` from anywhere!

## Usage

### Basic Usage
```powershell
# After global installation:
saas run

# Or from CLI project directory:
dotnet run -- run
```

This starts:
- ✅ PostgreSQL Database
- ✅ API Service
- ✅ Web UI (Blazor)
- ✅ Background Workers
- ✅ Service Discovery
- ✅ OpenTelemetry Dashboard

### With Options
```powershell
# Start with custom dashboard port
saas run --port 19000
# or: saas run -p 19000

# Start with hot reload enabled
saas run --watch
# or: saas run -w

# Combine options
saas run -w -p 19000
```

### Interactive Mode
```powershell
# From CLI project or if not globally installed:
dotnet run
# Select: "Start development environment (run)"

# After global installation:
saas
# Select: "Start development environment (run)"
```

## What It Does

1. **Locates AppHost** - Automatically finds the AppHost project (AppBlueprint.AppHost.csproj)
2. **Checks Port** - Verifies if the dashboard port is available
3. **Starts All Services** - Launches .NET Aspire which orchestrates all services
4. **Shows Logs** - Real-time colored output from all services
5. **Dashboard Link** - Provides direct link to Aspire dashboard

## Output Example

```
  ____                   _____          _
 / ___|  __ _  __ _ ___|  ___|_ _  ___| |_ ___  _ __ _   _
 \___ \ / _` |/ _` / __| |_ / _` |/ __| __/ _ \| '__| | | |
  ___) | (_| | (_| \__ \  _| (_| | (__| || (_) | |  | |_| |
 |____/ \__,_|\__,_|___/_|  \__,_|\___|\__\___/|_|   \__, |
                                                      |___/
🚀 Starting development environment...

📁 AppHost: AppBlueprint.AppHost
📂 Location: C:\...\Code\AppBlueprint\AppBlueprint.AppHost
🎛️  Dashboard: http://localhost:18888

┌────────────────────────────────┐
│ Services Starting              │
├────────────────────────────────┤
│ ✓ PostgreSQL Database          │
│ ✓ API Service                  │
│ ✓ Web UI (Blazor)             │
│ ✓ Background Workers           │
│ ✓ Service Discovery            │
│ ✓ OpenTelemetry Dashboard      │
└────────────────────────────────┘

✅ Development environment starting...
🌐 Dashboard will be available at: http://localhost:18888
🔌 Services will start automatically

Press Ctrl+C to stop all services

──────────────────── Service Logs ─
```

## Options

| Option       | Short  | Default  | Description                                                     |
|--------------|--------|----------|-----------------------------------------------------------------|
| `--port`     | `-p`   | 18888    | Port for the Aspire dashboard                                   |
| `--watch`    | `-w`   | false    | Enable hot reload (watch mode for automatic recompilation)      |
| `--help`     | `-h`   | -        | Show help and usage information                                 |

## Global Installation

Install the CLI globally to use `saas` from anywhere:

```powershell
# From CLI project directory:
dotnet run -- install

# After installation (restart terminal):
saas run          # Start development
saas --help       # See all commands
saas uninstall    # Remove global installation
```

**Benefits:**
- 🚀 Run from any directory
- ⚡ No need to navigate to CLI project
- 🎯 Consistent with industry standards (npm, cargo, etc.)

## Comparison with Manual Start

### Before (Manual)
```powershell
# Navigate to AppHost directory
cd Code\AppBlueprint\AppBlueprint.AppHost

# Start AppHost
dotnet run

# Navigate to dashboard URL shown in logs
# Wait for all services to start
# Check logs for errors
```

**Time: 2-3 minutes, multiple steps**

### After (run command + global install)
```powershell
saas run
```

**Time: 5 seconds, one command, from anywhere!** ⚡

## Troubleshooting

### Port Already in Use
```
⚠️  Port 18888 is already in use!
   AppHost might already be running.

Do you want to continue anyway? (y/n)
```

**Solution:** Either:
- Stop the existing AppHost instance
- Use a different port: `saas run -p 19000`

### AppHost Not Found
```
❌ AppHost project not found!
💡 Please run this command from the AppBlueprint directory
   or any subdirectory within the project
```

**Solution:** Navigate to the AppBlueprint directory or any subdirectory (or use global install)

### Services Won't Start
Check the service logs for specific errors. Common issues:
- PostgreSQL not running
- Port conflicts
- Missing configuration

## Hot Reload Mode

Enable watch mode for automatic recompilation when code changes:

```powershell
saas run -w
# or: saas run --watch
```

This is equivalent to `dotnet watch run` but with improved DX.

## What's Next?

After running `saas run`, you can:

1. **Open Dashboard:** http://localhost:18888
   - View all services
   - Check health status
   - View telemetry

2. **Access Web UI:** http://localhost:8092
   - Login with test credentials
   - Start developing features

3. **Test API:** https://localhost:8091/swagger
   - Explore endpoints
   - Test with JWT token

4. **Generate Test Token:**
   ```powershell
   saas jwt-token
   ```

## Industry Standard Naming

This command follows industry-standard CLI patterns:

| Tool               | Command          | What It Does                      |
|--------------------|------------------|-----------------------------------|
| **npm**            | `npm run dev`    | Start Node.js dev server          |
| **cargo**          | `cargo run`      | Build and run Rust project        |
| **docker**         | `docker run`     | Start container                   |
| **SaaS Factory**   | `saas run`       | Start all services via Aspire     |


## Related Commands

- `saas install` - Install CLI globally
- `saas uninstall` - Remove global installation
- `saas migrate-database` - Run database migrations
- `saas jwt-token` - Generate authentication tokens
- `saas route list` - View all API routes

---

**Quick Win Achievement:** ⭐⭐⭐⭐⭐

Global installation with `saas run` eliminates navigation steps and provides Laravel-like DX with industry-standard naming!

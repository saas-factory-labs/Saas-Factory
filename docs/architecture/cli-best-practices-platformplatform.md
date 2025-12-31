# CLI Best Practices - PlatformPlatform Analysis

## Overview
This document analyzes PlatformPlatform's developer CLI to extract best practices for SaaS Factory's DeveloperCli.

## Command Naming Conventions

### ✅ **PlatformPlatform's Approach**
- `run` instead of `serve` - More standard CLI terminology (similar to Docker, npm, cargo)
- `check` - Composite command that runs build + test + format + inspect in one go
- `format` - Code formatting (dotnet format + prettier)
- `inspect` - Code quality checks (linting, type checking)
- `test` - Test execution with filtering
- `build` - Solution building
- `coverage` - Test coverage reports
- `translate` - AI-powered translation using GPT
- `deploy` - Cloud deployment setup (Azure + GitHub OIDC)
- `install` / `uninstall` - CLI registration as global command
- `update-packages` - NuGet package updates

### 📊 **Comparison with SaaS Factory**

| Feature | PlatformPlatform | SaaS Factory | Recommendation |
|---------|------------------|--------------|----------------|
| **Dev Environment** | `run [--watch]` | `serve [--watch]` | Consider renaming `serve` → `run` for industry standard |
| **Composite Checks** | `check` (build+test+format+inspect) | ❌ Missing | **High Priority** - Add composite check command |
| **Code Quality** | `format` + `inspect` (separate) | ❌ Missing | **High Priority** - Split formatting and linting |
| **Test Filtering** | `test --filter "TestName"` | ❌ Missing | Medium Priority |
| **Coverage Reports** | `coverage` (dotCover HTML) | ❌ Missing | Low Priority (nice to have) |
| **Global Install** | `install --force` | ❌ Missing | **High Priority** - Enable global CLI access |
| **Package Updates** | `update-packages` | ❌ Missing | Low Priority |

---

## Command Structure Best Practices

### 1. **Composite Commands**

**What PlatformPlatform Does:**
```csharp
// CheckCommand runs: build → test → format → inspect
private static void RunBackendChecks(string? selfContainedSystem, bool noBuild, bool quiet)
{
    var systemArgs = BuildArgs(selfContainedSystem, quiet);
    
    if (!noBuild)
    {
        new BuildCommand().Parse([.. systemArgs, "--backend"]).Invoke();
    }
    
    new TestCommand().Parse([.. systemArgs, "--no-build"]).Invoke();
    new FormatCommand().Parse([.. systemArgs, "--backend", "--no-build"]).Invoke();
    new InspectCommand().Parse([.. systemArgs, "--backend", "--no-build"]).Invoke();
}
```

**Why This Matters:**
- **DX Win:** One command (`check`) replaces 4+ manual commands
- **CI/CD Ready:** Same command runs locally and in GitHub Actions
- **Consistent:** Enforces the same checks everywhere

**Recommendation for SaaS Factory:**
```bash
# Add a check command that runs:
dotnet run -- check
  → dotnet build
  → dotnet test
  → dotnet format --verify-no-changes
  → dotnet sonarqube analyze (if available)
```

---

### 2. **Consistent Option Naming**

**PlatformPlatform Standard:**
```csharp
var backendOption = new Option<bool>("--backend", "-b") 
    { Description = "Run backend checks" };
var frontendOption = new Option<bool>("--frontend", "-f") 
    { Description = "Run frontend checks" };
var cliOption = new Option<bool>("--cli", "-c") 
    { Description = "Run developer-cli checks" };
var selfContainedSystemOption = new Option<string?>("<self-contained-system>", "--self-contained-system", "-s") 
    { Description = "The name of the self-contained system" };
var noBuildOption = new Option<bool>("--no-build") 
    { Description = "Skip building before running checks" };
var quietOption = new Option<bool>("--quiet", "-q") 
    { Description = "Minimal output mode" };
```

**Key Patterns:**
- Always provide short flags (`-b`, `-f`, `-c`, `-s`, `-q`)
- Use `--no-*` for negative flags (not `--skip-*`)
- Use `--quiet` instead of `--silent` or `--minimal`
- Consistent descriptions ending with period

**Recommendation for SaaS Factory:**
Adopt these same option names across all commands for consistency.

---

### 3. **Global Installation Pattern**

**PlatformPlatform's Approach:**
```csharp
// InstallCommand.cs - Registers CLI globally
public class InstallCommand : Command
{
    public InstallCommand() : base(
        "install",
        $"This will register the alias {Configuration.AliasName} so it will be available everywhere"
    )
    {
        var forceOption = new Option<bool>("--force", "-f") 
            { Description = "Force reinstall even if already installed" };
        Options.Add(forceOption);
        SetAction(parseResult => Execute(parseResult.GetValue(forceOption)));
    }
}
```

**How It Works:**
- **Windows:** Adds CLI folder to PATH environment variable
- **macOS/Linux:** Creates shell alias in `.zshrc` / `.bashrc`
- **Result:** User can run `pp` (PlatformPlatform) from anywhere

**Recommendation for SaaS Factory:**
```bash
# Add install command:
dotnet run -- install
  → Registers 'saas' or 'blueprint' alias globally
  → User can then run: saas run, saas check, saas migrate, etc.
```

**DX Impact:**
- Before: `cd Code/AppBlueprint/AppBlueprint.DeveloperCli && dotnet run -- serve`
- After: `saas run` (from anywhere!)

---

### 4. **Quiet Mode for CI/CD**

**Pattern:**
```csharp
private static void Execute(bool quiet)
{
    if (!quiet) 
    {
        AnsiConsole.MarkupLine("[blue]Building solution...[/]");
    }
    
    ProcessHelper.Run("dotnet build", Configuration.ApplicationFolder, "Build", quiet);
}
```

**Why This Matters:**
- **Local:** Beautiful colored output with Spectre.Console
- **CI/CD:** Clean, parsable output for GitHub Actions logs
- **Performance:** Less overhead in automated environments

**Recommendation for SaaS Factory:**
Add `--quiet` flag to all commands and respect it in output.

---

### 5. **Self-Contained System Support**

**PlatformPlatform Structure:**
They support multiple "self-contained systems" (microservices):
- `account-management`
- `back-office`
- `api-gateway`

**Example:**
```bash
pp build --backend --self-contained-system account-management
pp test --self-contained-system back-office
pp format --self-contained-system api-gateway
```

**SaaS Factory Context:**
We currently have a monolithic structure, but if we ever split into modules/microservices, this pattern would be valuable.

**Recommendation:** Skip for now, but keep in mind for future.

---

## Recommended Quick Wins for SaaS Factory

### **Priority 1: High Impact, Low Effort**

#### 1. **Rename `serve` → `run` (5 minutes)**
```bash
# Industry standard naming
dotnet run -- run [--watch] [--port 18888]
```

#### 2. **Add Global Install Command (2 hours)**
Enable `saas` alias so users can run CLI from anywhere.

**Files to Create:**
- `Commands/InstallCommand.cs`
- `Commands/UninstallCommand.cs`
- `Utilities/PathHelper.cs` (Windows PATH manipulation)

**Result:**
```bash
# Before
cd Code/AppBlueprint/AppBlueprint.DeveloperCli
dotnet run -- serve

# After
saas run  # from anywhere!
```

#### 3. **Add `check` Composite Command (1 hour)**
One command to run all pre-commit checks.

```csharp
// Commands/CheckCommand.cs
public class CheckCommand : Command
{
    public CheckCommand() : base("check", "Run all pre-commit checks (build, test)")
    {
        SetAction(_ => Execute());
    }
    
    private static void Execute()
    {
        AnsiConsole.MarkupLine("[blue]Running build...[/]");
        // Run: dotnet build
        
        AnsiConsole.MarkupLine("[blue]Running tests...[/]");
        // Run: dotnet test
        
        AnsiConsole.MarkupLine("[green]✓ All checks passed![/]");
    }
}
```

**Usage:**
```bash
saas check  # Runs everything before git push
```

---

### **Priority 2: Medium Impact**

#### 4. **Add `format` Command (30 minutes)**
Run code formatters (dotnet format).

```bash
saas format  # Formats all C# code
saas format --verify  # Check without modifying (CI mode)
```

#### 5. **Add `test` Command Enhancements (1 hour)**
Support test filtering and coverage.

```bash
saas test --filter "TodoApp"
saas test --coverage  # Generate HTML coverage report
```

#### 6. **Add `--quiet` Flag to All Commands (30 minutes)**
Enable CI-friendly output.

```bash
saas check --quiet  # Minimal output for GitHub Actions
```

---

### **Priority 3: Nice to Have**

#### 7. **Add `update-packages` Command (1 hour)**
Automate NuGet package updates.

```bash
saas update-packages  # Check for updates
saas update-packages --apply  # Apply updates
```

#### 8. **Add `coverage` Command (2 hours)**
Generate HTML coverage reports with dotCover.

```bash
saas coverage  # Opens coverage.html in browser
```

---

## Comparison: ServeCommand vs RunCommand

### **Our Implementation (ServeCommand.cs)**
```csharp
public static Command Create()
{
    var command = new Command("serve", "Start the development environment (AppHost with all services)");
    
    var portOption = new Option<int>("--port", () => 18888, "Port for the Aspire dashboard");
    var watchOption = new Option<bool>("--watch", () => false, "Enable hot reload (watch mode)");
    
    command.AddOption(portOption);
    command.AddOption(watchOption);
    // ...
}
```

### **PlatformPlatform's Implementation (RunCommand.cs)**
```csharp
public RunCommand() : base("run", "Start the Aspire AppHost with all services")
{
    var watchOption = new Option<bool>("--watch", "-w") 
        { Description = "Enable hot reload (watch mode)" };
    
    Options.Add(watchOption);
    
    SetAction(async parseResult => await Execute(
        parseResult.GetValue(watchOption)
    ));
}
```

### **Key Differences**

| Feature | ServeCommand (Ours) | RunCommand (PlatformPlatform) | Winner |
|---------|---------------------|-------------------------------|--------|
| **Command Name** | `serve` | `run` | PlatformPlatform (industry standard) |
| **Port Configuration** | `--port 18888` | Uses Aspire defaults | Ours (more flexible) |
| **Watch Mode** | `--watch` | `--watch` / `-w` | PlatformPlatform (short flag) |
| **Auto AppHost Detection** | ✅ FindAppHostProject() | ✅ Similar | Tie |
| **Port Conflict Check** | ✅ IsPortInUse() | ❌ Not shown | Ours |
| **Beautiful Output** | ✅ Spectre.Console panels | ✅ Spectre.Console | Tie |

---

## Action Plan

### **Immediate Actions (This Week)**

1. **Read the PlatformPlatform CLI rules file**
   ```bash
   # They have documented patterns at:
   .agent/rules/developer-cli/developer-cli.md
   .claude/rules/developer-cli/developer-cli.md
   ```

2. **Consider Renaming serve → run**
   - Check with team if `serve` (Laravel-inspired) vs `run` (standard) preference
   - If renaming, update:
     - ServeCommand.cs → RunCommand.cs
     - CommandFactory.cs registration
     - MainMenu.cs option
     - README-SERVE-COMMAND.md → README-RUN-COMMAND.md

3. **Add Short Flags**
   Update ServeCommand to include short flags:
   ```csharp
   var portOption = new Option<int>(["--port", "-p"], () => 18888, "Port for the Aspire dashboard");
   var watchOption = new Option<bool>(["--watch", "-w"], () => false, "Enable hot reload (watch mode)");
   ```

4. **Implement Check Command (Quick Win #2)**
   Composite command: build + test

5. **Implement Install Command (Quick Win #3)**
   Enable global `saas` alias

---

## Summary

**What We're Doing Well:**
- ✅ Beautiful Spectre.Console output
- ✅ Auto AppHost detection
- ✅ Port conflict checking
- ✅ Comprehensive documentation

**What We Should Adopt:**
- 🎯 Global install pattern (biggest DX win)
- 🎯 Composite `check` command (CI/CD ready)
- 🎯 Short flags for all options (`-w`, `-p`, `-q`)
- 🎯 `--quiet` mode for CI/CD
- 🎯 Consider `run` instead of `serve` for industry standard naming

**What We Can Skip:**
- Self-contained system support (not needed yet)
- Coverage command (nice to have, not critical)
- Update packages command (can use Dependabot)

---

## Next Steps

1. **Decision:** Keep `serve` (Laravel-inspired) or rename to `run` (industry standard)?
2. **Quick Win #2:** Implement `check` command (1 hour)
3. **Quick Win #3:** Implement `install` command (2 hours)
4. **Polish:** Add short flags and `--quiet` mode (30 minutes)

**Total Time Investment:** ~4 hours for massive DX improvement

**Before:**
```bash
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.DeveloperCli
dotnet run -- serve --watch
```

**After:**
```bash
saas run -w  # from anywhere!
saas check   # before git push
```

This matches Laravel's DX while adopting .NET CLI best practices. 🚀

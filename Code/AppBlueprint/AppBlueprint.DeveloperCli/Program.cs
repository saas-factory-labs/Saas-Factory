using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using AppBlueprint.CliKit;
using AppBlueprint.CliKit.Commands;
using AppBlueprint.DeveloperCli.Commands;
using AppBlueprint.DeveloperCli.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace AppBlueprint.DeveloperCli;

[SupportedOSPlatform("windows")]
internal static class Program
{
    private const string AppblueprintName= "AppBlueprint";
    
    private const string AccentOrange = "#F26522";
    private const string AccentRed = "#FA4655";
    private const string AccentMagenta = "#FF2A85";
    private const string AccentCyan = "#00E5FF";
    private const string BorderGrey = "grey30";
    private const string LightGrey = "grey62";
    private const string SectionGold = "gold1";
    private const string AlertColor = "lightsalmon1";
    private const string DatabaseConnectionVariable = "DATABASE_CONNECTIONSTRING";
    private const string DefaultGatewayUrl = "http://localhost:9000";
    private const string DefaultApiUrl = "http://localhost:9100";
    private const string DefaultDashboardUrl = "http://localhost:18888";
    private const string DefaultWebUrl = "http://localhost:9200";
    private const int GatewayPort = 9000;
    private const int ApiPort = 9100;
    private const int DashboardPort = 18888;
    private const int WebPort = 9200;
    private const int PostgresPort = 5432;
    private const string BannerWordSeparator = "   ";

    private static readonly string[] SaaSBannerLines =
    [
        @"███████╗  █████╗   █████╗ ███████╗",
        @"██╔════╝ ██╔══██╗ ██╔══██╗██╔════╝",
        @"███████╗ ███████║ ███████║███████╗",
        @"╚════██║ ██╔══██║ ██╔══██║╚════██║",
        @"███████║ ██║  ██║ ██║  ██║███████║",
        @"╚══════╝ ╚═╝  ╚═╝ ╚═╝  ╚═╝╚══════╝"
    ];

    private static readonly string[] FactoryBannerLines =
    [
        @"███████╗  █████╗   ██████╗████████╗ ██████╗ ██████╗ ██╗   ██╗",
        @"██╔════╝ ██╔══██╗ ██╔════╝╚══██╔══╝██╔═══██╗██╔══██╗╚██╗ ██╔╝",
        @"█████╗   ███████║ ██║        ██║   ██║   ██║██████╔╝ ╚████╔╝ ",
        @"██╔══╝   ██╔══██║ ██║        ██║   ██║   ██║██╔══██╗  ╚██╔╝  ",
        @"██║      ██║  ██║ ╚██████╗   ██║   ╚██████╔╝██║  ██║   ██║   ",
        @"╚═╝      ╚═╝  ╚═╝  ╚═════╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝   ╚═╝   "
    ];

    private static readonly DashboardGroup[] DashboardGroups =
    [
        new(
            "● Development",
            [
                ActionLabels.StartDevEnvironment,
                ActionLabels.DisplayEnvironmentInfo,
                ActionLabels.RunTests
            ]),
        new(
            "🤖 AI & Agent Tools",
            [
                ActionLabels.StartMcpServer,
                ActionLabels.SyncAiRules,
                ActionLabels.Translate
            ]),
        new(
            "🏗️ Scaffolding",
            [
                ActionLabels.CreateSolution,
                ActionLabels.CreateProject,
                ActionLabels.CreateItem
            ]),
        new(
            "⚙️ Database, Tools & Security",
            [
                ActionLabels.MigrateDatabase,
                ActionLabels.ScanApiRoutes,
                ActionLabels.GenerateJwtToken,
                ActionLabels.ValidatePostgreSqlPassword,
                ActionLabels.ManageEnvironmentVariables,
                ActionLabels.CloneGitHubRepository
            ]),
        new(
            "🛠️ CLI Management",
            [
                ActionLabels.InstallCli,
                ActionLabels.UninstallCli
            ]),
        new(
            "❌ Exit",
            [
                ActionLabels.Exit
            ])
    ];

    private static async Task<int> Main(string[] args)
    {
        ConfigureAnsiConsole();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddCliKit();
        builder.Services.AddDeveloperCliCommands();

        using IHost host = builder.Build();

        if (args.Length is 0)
        {
            await RunDashboardAsync();
            return 0;
        }

        CommandFactory commandFactory = host.Services.GetRequiredService<CommandFactory>();
        RootCommand rootCommand = commandFactory.CreateRootCommand();
        CliCommandApplication app = host.Services.GetRequiredService<CliCommandApplication>();

        return await app.InvokeAsync(rootCommand, args);
    }

    private static void ConfigureAnsiConsole()
    {
        AnsiConsole.Profile.Capabilities.Ansi = true;
        AnsiConsole.Profile.Capabilities.Links = false;
    }

    private static async Task RunDashboardAsync()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string? resolvedProjectRoot = ResolveProjectRoot(currentDirectory);
        string projectRoot = resolvedProjectRoot is null ? currentDirectory : resolvedProjectRoot;
        var state = new DashboardState(projectRoot);

        AppendLog(state, "Dashboard booted for local development.");
        AppendLog(state, "Waiting for the next workflow.");
        RefreshState(state);

        while (true)
        {
            SafeClearConsole();
            AnsiConsole.Write(BuildDashboard(state));

            string action = await PromptForActionAsync();
            if (action == ActionLabels.Exit)
            {
                break;
            }

            state.LastAction = action;
            AppendLog(state, $"Selected: {action}.");
            await ExecuteActionAsync(state, action);
            RefreshState(state);
            await PauseBeforeReturningAsync();
        }

        SafeClearConsole();
        AnsiConsole.MarkupLine($"[{SectionGold}]SaaS Factory session closed.[/]");
    }

    private static IRenderable BuildDashboard(DashboardState state)
    {
        return new Rows(
            BuildHeaderPanel(),
            new Text(string.Empty),
            BuildDashboardPanel(state),
            new Text(string.Empty),
            BuildSectionRule("SAAS CORE CONTROLS"));
    }

    private static IRenderable BuildHeaderPanel()
    {
        return new Panel(
            new Rows(
                BuildBanner(),
                new Text(string.Empty),
                new Markup($"[bold {AccentMagenta}]Local-first developer command center[/]"),
                new Markup($"[{LightGrey}]Structured AppBlueprint workflows[/]")))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse(BorderGrey),
            Padding = new Padding(2, 1, 2, 1)
        };
    }

    private static IRenderable BuildBanner()
    {
        string[] bannerLines = SaaSBannerLines
            .Zip(FactoryBannerLines, (saas, factory) => string.Concat(saas, BannerWordSeparator, factory))
            .ToArray();

        IRenderable[] bannerRows = bannerLines
            .Select((line, index) =>
            {
                string color = index switch
                {
                    0 or 1 => AccentOrange,
                    2 or 3 => AccentRed,
                    _ => AccentMagenta
                };

                return (IRenderable)new Markup($"[bold {color}]{line}[/]");
            })
            .Append((IRenderable)new Markup($"[{AccentOrange}]█████[/][{AccentRed}]█████[/][{AccentMagenta}]█████[/] [{LightGrey}]local development only[/]"))
            .ToArray();

        return new Rows(bannerRows);
    }

    private static IRenderable BuildDashboardPanel(DashboardState state)
    {
        Table summaryTable = new Table()
            .NoBorder()
            .HideHeaders()
            .Expand();

        summaryTable.AddColumn(new TableColumn(string.Empty));
        summaryTable.AddColumn(new TableColumn(string.Empty));
        summaryTable.AddRow(
            BuildArchitectureSection(state),
            BuildSecuritySection(state));

        return new Panel(
            new Rows(
                BuildSectionRule("LOCAL ENVIRONMENT STATUS"),
                BuildEnvironmentStatusTable(state),
                new Text(string.Empty),
                BuildSectionRule("LOCAL SYSTEM SUMMARY"),
                summaryTable,
                new Text(string.Empty),
                BuildSectionRule("CLAUDE CODE STREAM"),
                BuildStreamTable(state)))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse(BorderGrey),
            Header = new PanelHeader($"[bold {SectionGold}]Dashboard[/]", Justify.Left),
            Padding = new Padding(1, 1, 1, 1)
        };
    }

    private static IRenderable BuildEnvironmentStatusTable(DashboardState state)
    {
        Table table = new Table()
            .NoBorder()
            .HideHeaders()
            .Expand();

        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow(
            new Markup($"[{LightGrey}]Mode[/]\n[{AccentMagenta}]LOCAL[/]"),
            new Markup($"[{LightGrey}]Gateway[/]\n[{AccentCyan}]{DefaultGatewayUrl}[/] {CreateStatusMarkup(state.ApiGatewaySignal)}"),
            new Markup($"[{LightGrey}]Docker[/]\n{CreateStatusMarkup(state.DockerSignal)}"),
            new Markup($"[{LightGrey}]Web[/]\n[{AccentCyan}]{DefaultWebUrl}[/] {CreateStatusMarkup(state.WebSignal)}"));

        return table;
    }

    private static IRenderable BuildArchitectureSection(DashboardState state)
    {
        return new Rows(
            new Markup($"[bold {SectionGold}]Project Architecture[/]"),
            new Markup($"[{LightGrey}]Local services, ports, and hostnames.[/]"),
            new Text(string.Empty),
            BuildArchitectureTree(state));
    }

    private static IRenderable BuildSecuritySection(DashboardState state)
    {
        return new Rows(
            new Markup($"[bold {SectionGold}]Security & Env[/]"),
            new Markup($"[{LightGrey}]Validation and configuration signals.[/]"),
            new Text(string.Empty),
            BuildSecurityTable(state));
    }

    private static IRenderable BuildArchitectureTree(DashboardState state)
    {
        Tree tree = new Tree($"[{SectionGold}]AppBlueprint services[/]");

        TreeNode gatewayNode =
            tree.AddNode($"[white]API Gateway[/] {CreateStatusMarkup(state.ApiGatewaySignal)}");
        gatewayNode.AddNode($"[{LightGrey}]Host:[/] [{AccentCyan}]{DefaultGatewayUrl}[/]");

        TreeNode apiNode =
            tree.AddNode($"[white]API Service[/] {CreateStatusMarkup(state.ApiServiceSignal)}");
        apiNode.AddNode($"[{LightGrey}]Host:[/] [{AccentCyan}]{DefaultApiUrl}[/]");

        TreeNode webNode =
            tree.AddNode($"[white]Web UI[/] {CreateStatusMarkup(state.WebSignal)}");
        webNode.AddNode($"[{LightGrey}]Host:[/] [{AccentCyan}]{DefaultWebUrl}[/]");

        TreeNode dashboardNode =
            tree.AddNode($"[white]Aspire Dashboard[/] {CreateStatusMarkup(state.DashboardSignal)}");
        dashboardNode.AddNode($"[{LightGrey}]Host:[/] [{AccentCyan}]{DefaultDashboardUrl}[/]");

        TreeNode databaseNode =
            tree.AddNode($"[white]PostgreSQL Database[/] {CreateStatusMarkup(state.PostgreSqlSignal)}");
        databaseNode.AddNode($"[{LightGrey}]Host:[/] [{AccentCyan}]localhost:{PostgresPort}[/]");

        return tree;
    }

    private static IRenderable BuildSecurityTable(DashboardState state)
    {
        Table table = new Table()
            .NoBorder()
            .HideHeaders()
            .Expand();

        table.AddColumn(new TableColumn($"[{SectionGold}]Check[/]").PadRight(2));
        table.AddColumn(new TableColumn($"[{SectionGold}]Status[/]"));

        table.AddRow("JWT Token", CreateStatusMarkup(state.JwtTokenSignal));
        table.AddRow(".env Files", CreateStatusMarkup(state.EnvFilesSignal));
        table.AddRow("PG Password", CreateStatusMarkup(state.PostgreSqlPasswordSignal));
        table.AddRow("DB Connection", CreateStatusMarkup(state.DatabaseConnectionSignal));

        return table;
    }

    private static IRenderable BuildStreamTable(DashboardState state)
    {
        Table table = new Table()
            .NoBorder()
            .HideHeaders()
            .Expand();

        table.AddColumn(new TableColumn(string.Empty).NoWrap());
        table.AddColumn(new TableColumn(string.Empty));

        DashboardLogEntry[] recentEntries = state.LogEntries.TakeLast(5).ToArray();
        if (recentEntries.Length is 0)
        {
            table.AddRow(
                $"[{AccentMagenta}]--:--:--[/]",
                $"[{LightGrey}]No local activity yet.[/]");

            return table;
        }

        foreach (DashboardLogEntry entry in recentEntries)
        {
            table.AddRow(
                $"[{AccentMagenta}]{Markup.Escape(entry.Timestamp)}[/]",
                $"[{LightGrey}]{Markup.Escape(entry.Message)}[/]");
        }

        return table;
    }

    private static Rule BuildSectionRule(string title)
    {
        return new Rule($"[bold {LightGrey}]{Markup.Escape(title)}[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse(BorderGrey)
        };
    }

    private static async Task<string> PromptForActionAsync()
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>()
            .Title($"[bold {SectionGold}]Choose a local workflow[/]")
            .PageSize(16)
            .MoreChoicesText($"[{LightGrey}](scroll for more)[/]")
            .HighlightStyle(new Style(Color.Aquamarine1, decoration: Decoration.Bold))
            .SearchPlaceholderText($"[{LightGrey}]Search local tasks[/]")
            .EnableSearch();

        foreach (DashboardGroup group in DashboardGroups)
        {
            prompt.AddChoiceGroup(GetGroupHeaderMarkup(group.Header), group.Actions);
        }

        return await AnsiConsole.Console.PromptAsync(prompt, CancellationToken.None);
    }

    private static string GetGroupHeaderMarkup(string header)
    {
        return header switch
        {
            "● Development" => $"[bold {SectionGold}]{header}[/]",
            "🤖 AI & Agent Tools" => $"[bold {AccentMagenta}]{header}[/]",
            "🏗️ Scaffolding" => $"[bold {AccentOrange}]{header}[/]",
            "⚙️ Database, Tools & Security" => $"[bold {SectionGold}]{header}[/]",
            "🛠️ CLI Management" => $"[bold {LightGrey}]{header}[/]",
            "❌ Exit" => $"[bold {AlertColor}]{header}[/]",
            _ => $"[bold {LightGrey}]{header}[/]"
        };
    }

    private static async Task ExecuteActionAsync(DashboardState state, string action)
    {
        try
        {
            switch (action)
            {
                case ActionLabels.StartDevEnvironment:
                    RunCommand.ExecuteInteractive();
                    AppendLog(state, $"Run flow handed off. Dashboard endpoint: {DefaultDashboardUrl}.");
                    break;

                case ActionLabels.DisplayEnvironmentInfo:
                    EnvironmentInfoCommand.ExecuteInteractive();
                    AppendLog(state, "Environment info rendered.");
                    break;

                case ActionLabels.RunTests:
                    TestCommand.ExecuteInteractive();
                    AppendLog(state, "Test flow completed.");
                    break;

                case ActionLabels.StartMcpServer:
                    await ShowUnavailableActionAsync(
                        state,
                        "MCP Server",
                        "No local MCP server command is registered in the current CLI.");
                    break;

                case ActionLabels.SyncAiRules:
                    await ShowUnavailableActionAsync(
                        state,
                        "Sync AI Rules",
                        "The dashboard can surface this action, but no local sync-ai-rules implementation is wired yet.");
                    break;

                case ActionLabels.Translate:
                    await ShowUnavailableActionAsync(
                        state,
                        "Translate",
                        "A local translate command is not available in the current CLI.");
                    break;

                case ActionLabels.CreateSolution:
                    SolutionCommand.ExecuteInteractive();
                    AppendLog(state, "Solution scaffolding completed.");
                    break;

                case ActionLabels.CreateProject:
                    ProjectCommand.ExecuteInteractive();
                    AppendLog(state, "Project scaffolding completed.");
                    break;

                case ActionLabels.CreateItem:
                    ItemCommand.ExecuteInteractive();
                    AppendLog(state, "Item scaffolding completed.");
                    break;

                case ActionLabels.MigrateDatabase:
                    DatabaseCommand.ExecuteInteractive();
                    AppendLog(state, "Database migration flow completed.");
                    break;

                case ActionLabels.ScanApiRoutes:
                    RouteCommand.ExecuteInteractive();
                    AppendLog(state, "Route scan completed.");
                    break;

                case ActionLabels.GenerateJwtToken:
                    await JwtTokenCommand.ExecuteInteractive();
                    state.JwtTokenGenerated = true;
                    AppendLog(state, "JWT token workflow completed.");
                    break;

                case ActionLabels.ValidatePostgreSqlPassword:
                    state.PostgreSqlPasswordValidated = await ValidatePostgreSqlPasswordAsync(state);
                    AppendLog(state, state.PostgreSqlPasswordValidated
                        ? "PostgreSQL password validated."
                        : "PostgreSQL password validation needs attention.");
                    break;

                case ActionLabels.ManageEnvironmentVariables:
                    EnvironmentVariableCommand.ExecuteInteractive();
                    AppendLog(state, "Environment variable flow completed.");
                    break;

                case ActionLabels.CloneGitHubRepository:
                    GitHubCommand.ExecuteInteractive();
                    AppendLog(state, "Repository clone flow completed.");
                    break;

                case ActionLabels.InstallCli:
                    InstallCommand.ExecuteInteractive();
                    AppendLog(state, "CLI install flow completed.");
                    break;

                case ActionLabels.UninstallCli:
                    UninstallCommand.ExecuteInteractive();
                    AppendLog(state, "CLI uninstall flow completed.");
                    break;
            }
        }
        catch (Exception exception)
        {
            AppendLog(state, $"Workflow failed: {exception.Message}");

            AnsiConsole.Write(
                new Panel(new Markup($"[{AlertColor}]{Markup.Escape(exception.Message)}[/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = Style.Parse(BorderGrey),
                    Header = new PanelHeader($"[bold {SectionGold}]Workflow Error[/]", Justify.Left),
                    Padding = new Padding(1, 0, 1, 0)
                });
        }
    }

    private static async Task<bool> ValidatePostgreSqlPasswordAsync(DashboardState state)
    {
        string? savedConnectionString = TryGetConnectionString();
        if (!string.IsNullOrWhiteSpace(savedConnectionString))
        {
            AnsiConsole.MarkupLine(
                $"[{LightGrey}]Stored connection:[/] [{AccentCyan}]{Markup.Escape(MaskConnectionString(savedConnectionString))}[/]");

            if (await AnsiConsole.ConfirmAsync($"[{SectionGold}]Test the stored connection string?[/]", true))
            {
                return await TestConnectionStringAsync(savedConnectionString);
            }
        }

        string host = await AnsiConsole.AskAsync("Host:", "localhost");
        string port = await AnsiConsole.AskAsync("Port:", "5432");
        string database = await AnsiConsole.AskAsync("Database:", "appblueprintdb");
        string username = await AnsiConsole.AskAsync("Username:", "postgres");
        string password = await AnsiConsole.Console.PromptAsync(
            new TextPrompt<string>("Password:")
                .PromptStyle(SectionGold)
                .Secret(),
            CancellationToken.None);

        string connectionString =
            $"Host={host};Port={port};Database={database};Username={username};Password={password}";

        bool validated = await TestConnectionStringAsync(connectionString);
        if (!validated)
        {
            return false;
        }

        if (await AnsiConsole.ConfirmAsync($"[{SectionGold}]Save this connection string to the current user profile?[/]", true))
        {
            try
            {
                Environment.SetEnvironmentVariable(
                    DatabaseConnectionVariable,
                    connectionString,
                    EnvironmentVariableTarget.User);

                AppendLog(state, "Saved DATABASE_CONNECTIONSTRING for the current user.");
            }
            catch (Exception exception)
            {
                AppendLog(state, $"Could not save DATABASE_CONNECTIONSTRING: {exception.Message}");
            }
        }

        return true;
    }

    private static async Task<bool> TestConnectionStringAsync(string connectionString)
    {
        bool isValid = false;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(Color.Cyan1))
            .StartAsync($"[{SectionGold}]Testing PostgreSQL connection...[/]", async _ =>
            {
                isValid = await ConnectionStringValidator.ValidatePostgreSqlConnection(connectionString);
            });

        if (isValid)
        {
            AnsiConsole.Write(
                new Panel(new Markup($"[{AccentCyan}]Connection validated successfully.[/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = Style.Parse(BorderGrey),
                    Header = new PanelHeader($"[bold {SectionGold}]Validation Result[/]", Justify.Left)
                });
        }
        else
        {
            AnsiConsole.Write(
                new Panel(new Markup($"[{AlertColor}]The connection could not be validated. Verify the host, user, and password.[/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = Style.Parse(BorderGrey),
                    Header = new PanelHeader($"[bold {SectionGold}]Validation Result[/]", Justify.Left)
                });
        }

        return isValid;
    }

    private static async Task ShowUnavailableActionAsync(DashboardState state, string title, string details)
    {
        await Task.Yield();
        AppendLog(state, $"{title} is listed in the dashboard but not wired yet.");

        AnsiConsole.Write(
            new Panel(new Markup($"[{AlertColor}]{Markup.Escape(details)}[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse(BorderGrey),
                Header = new PanelHeader($"[bold {SectionGold}]{Markup.Escape(title)}[/]", Justify.Left),
                Padding = new Padding(1, 0, 1, 0)
            });
    }

    private static void RefreshState(DashboardState state)
    {
        bool dockerRunning = CheckDockerRunning();
        bool gatewayRunning = CheckPortInUse(GatewayPort);
        bool apiRunning = CheckPortInUse(ApiPort);
        bool dashboardRunning = CheckPortInUse(DashboardPort);
        bool postgresRunning = CheckPortInUse(PostgresPort);
        bool webRunning = CheckPortInUse(WebPort) || CheckPortInUse(8092) || CheckPortInUse(5000);

        state.DockerSignal = dockerRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("CHECK", SignalStyle.Check);

        state.ApiGatewaySignal = gatewayRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("PENDING", SignalStyle.Pending);

        state.ApiServiceSignal = apiRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("PENDING", SignalStyle.Pending);

        state.DashboardSignal = dashboardRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("CHECK", SignalStyle.Check);

        state.PostgreSqlSignal = postgresRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("CHECK", SignalStyle.Check);

        state.WebSignal = webRunning
            ? new StatusSignal("RUNNING", SignalStyle.Running)
            : new StatusSignal("PENDING", SignalStyle.Pending);

        string[] envFiles = FindEnvFiles(state.ProjectRoot);
        state.EnvFilesSignal = envFiles.Length > 0
            ? new StatusSignal("LOADED", SignalStyle.Running)
            : new StatusSignal("CHECK", SignalStyle.Check);

        state.JwtTokenSignal = state.JwtTokenGenerated
            ? new StatusSignal("READY", SignalStyle.Accent)
            : new StatusSignal("PENDING", SignalStyle.Pending);

        state.PostgreSqlPasswordSignal = state.PostgreSqlPasswordValidated
            ? new StatusSignal("VALID", SignalStyle.Running)
            : new StatusSignal("PENDING", SignalStyle.Pending);

        string? connectionString = TryGetConnectionString();
        state.DatabaseConnectionSignal = string.IsNullOrWhiteSpace(connectionString)
            ? new StatusSignal("CHECK", SignalStyle.Check)
            : new StatusSignal("CONFIG", SignalStyle.Accent);
    }

    private static async Task PauseBeforeReturningAsync()
    {
        if (Console.IsInputRedirected)
        {
            return;
        }

        AnsiConsole.MarkupLine($"\n[{SectionGold}]Press any key to return to the dashboard...[/]");

        try
        {
            await Task.Run(() => Console.ReadKey(true));
        }
        catch (InvalidOperationException)
        {
            await Task.Delay(250);
        }
    }

    private static string? ResolveProjectRoot(string startPath)
    {
        string? current = startPath;

        while (current is not null)
        {
            string appBlueprintPath = Path.Combine(current, "Code", AppblueprintName);
            if (Directory.Exists(appBlueprintPath))
            {
                return current;
            }

            DirectoryInfo? parent = Directory.GetParent(current);
            current = parent?.FullName;
        }

        return null;
    }

    private static string[] FindEnvFiles(string projectRoot)
    {
        string[] candidates =
        [
            Path.Combine(projectRoot, ".env"),
            Path.Combine(projectRoot, ".env.local"),
            Path.Combine(projectRoot, "Code", AppblueprintName, ".env"),
            Path.Combine(projectRoot, "Code", AppblueprintName, ".env.local"),
            Path.Combine(projectRoot, "Code", AppblueprintName, "AppBlueprint.AppHost", ".env"),
            Path.Combine(projectRoot, "Code", AppblueprintName, "AppBlueprint.ApiService", ".env"),
            Path.Combine(projectRoot, "Code", AppblueprintName, "AppBlueprint.Web", ".env")
        ];

        return candidates.Where(File.Exists).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static string? TryGetConnectionString()
    {
        string? processValue = Environment.GetEnvironmentVariable(DatabaseConnectionVariable);
        if (!string.IsNullOrWhiteSpace(processValue))
        {
            return processValue;
        }

        string? userValue = Environment.GetEnvironmentVariable(DatabaseConnectionVariable, EnvironmentVariableTarget.User);
        if (!string.IsNullOrWhiteSpace(userValue))
        {
            return userValue;
        }

        string? explicitProcessValue = Environment.GetEnvironmentVariable(DatabaseConnectionVariable, EnvironmentVariableTarget.Process);
        if (!string.IsNullOrWhiteSpace(explicitProcessValue))
        {
            return explicitProcessValue;
        }

        return null;
    }

    private static string MaskConnectionString(string connectionString)
    {
        return Regex.Replace(
            connectionString,
            "Password=[^;]+",
            match => match.Value[..match.Value.IndexOf('=')] + "=***",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static bool CheckPortInUse(int port)
    {
        try
        {
            using var client = new TcpClient();
            IAsyncResult connectTask = client.BeginConnect("127.0.0.1", port, null, null);
            bool connected = connectTask.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(150));
            if (!connected)
            {
                return false;
            }

            client.EndConnect(connectTask);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool CheckDockerRunning()
    {
        try
        {
            if (Process.GetProcessesByName("Docker Desktop").Length > 0 ||
                Process.GetProcessesByName("com.docker.backend").Length > 0)
            {
                return true;
            }

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = "version --format {{.Server.Version}}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit(800);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static void SafeClearConsole()
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            // VS Code and other hosts can reject cursor operations mid-session.
        }
    }

    private static string CreateStatusMarkup(StatusSignal signal)
    {
        string marker = AnsiConsole.Profile.Capabilities.Unicode ? "●" : "*";
        string color = signal.Style switch
        {
            SignalStyle.Running => AccentCyan,
            SignalStyle.Pending => SectionGold,
            SignalStyle.Check => AlertColor,
            SignalStyle.Accent => AccentMagenta,
            _ => LightGrey
        };

        return $"[{color}]{marker} {Markup.Escape(signal.Text)}[/]";
    }

    private static void AppendLog(DashboardState state, string message)
    {
        state.LogEntries.Add(new DashboardLogEntry(
            DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            message));

        while (state.LogEntries.Count > 8)
        {
            state.LogEntries.RemoveAt(0);
        }
    }

    private sealed class DashboardState(string projectRoot)
    {
        public string ProjectRoot { get; } = projectRoot;
        public string? LastAction { get; set; }
        public bool JwtTokenGenerated { get; set; }
        public bool PostgreSqlPasswordValidated { get; set; }
        public StatusSignal DockerSignal { get; set; } = new("CHECK", SignalStyle.Check);
        public StatusSignal ApiGatewaySignal { get; set; } = new("PENDING", SignalStyle.Pending);
        public StatusSignal ApiServiceSignal { get; set; } = new("PENDING", SignalStyle.Pending);
        public StatusSignal DashboardSignal { get; set; } = new("CHECK", SignalStyle.Check);
        public StatusSignal PostgreSqlSignal { get; set; } = new("CHECK", SignalStyle.Check);
        public StatusSignal WebSignal { get; set; } = new("PENDING", SignalStyle.Pending);
        public StatusSignal JwtTokenSignal { get; set; } = new("PENDING", SignalStyle.Pending);
        public StatusSignal EnvFilesSignal { get; set; } = new("CHECK", SignalStyle.Check);
        public StatusSignal PostgreSqlPasswordSignal { get; set; } = new("PENDING", SignalStyle.Pending);
        public StatusSignal DatabaseConnectionSignal { get; set; } = new("CHECK", SignalStyle.Check);
        public List<DashboardLogEntry> LogEntries { get; } = [];
    }

    private readonly record struct DashboardGroup(string Header, string[] Actions);
    private readonly record struct DashboardLogEntry(string Timestamp, string Message);
    private readonly record struct StatusSignal(string Text, SignalStyle Style);

    private enum SignalStyle
    {
        Neutral,
        Running,
        Pending,
        Check,
        Accent
    }

    private static class ActionLabels
    {
        public const string StartDevEnvironment = "Start dev environment (run)";
        public const string DisplayEnvironmentInfo = "Display env info (env:info)";
        public const string RunTests = "Run tests (test)";
        public const string StartMcpServer = "Start MCP Server (mcp)";
        public const string SyncAiRules = "Sync AI Rules (sync-ai-rules)";
        public const string Translate = "Translate (translate)";
        public const string CreateSolution = "Create a new SaaS app sol";
        public const string CreateProject = "Create a new project in sol";
        public const string CreateItem = "Create a new item (API, DTO)";
        public const string MigrateDatabase = "Migrate database";
        public const string ScanApiRoutes = "Scan API routes";
        public const string GenerateJwtToken = "Generate JWT Token (test)";
        public const string ValidatePostgreSqlPassword = "Validate PostgreSQL Password";
        public const string ManageEnvironmentVariables = "Manage Environment Variable";
        public const string CloneGitHubRepository = "Clone a GitHub repository";
        public const string InstallCli = "Install CLI globally (saas)";
        public const string UninstallCli = "Uninstall CLI from system";
        public const string Exit = "Exit";
    }
}

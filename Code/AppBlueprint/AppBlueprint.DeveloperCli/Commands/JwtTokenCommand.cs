using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class JwtTokenCommand
{
    private const string LogtoProviderName = "Logto";
    private const string DefaultWebAppUrl = "http://localhost:8092";

    public static Command Create()
    {
        var command = new Command("jwt-token", "Get JWT token from authentication provider");

        var configPathOption = new Option<string>(
            "--config",
            description: "Path to appsettings.json to read authentication configuration",
            getDefaultValue: () => "");

        command.AddOption(configPathOption);

        command.SetHandler(async (configPath) =>
        {
            await ExecuteInteractive(configPath);
        }, configPathOption);

        return command;
    }

    public static async Task ExecuteInteractive(string configPath = "")
    {
        AnsiConsole.Write(new FigletText("JWT Token Generator").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        IConfiguration? configuration = null;

        if (string.IsNullOrEmpty(configPath))
        {
            // Ask for configuration file path
            bool useConfig = await AnsiConsole.ConfirmAsync("Do you want to use settings from [green]appsettings.json[/]?", true);

            if (useConfig)
            {
                // Get the current directory and construct absolute path
                var currentDir = Directory.GetCurrentDirectory();
                // Default to Web project's Development settings which contains Logto config
                var defaultPath = Path.GetFullPath(Path.Combine(currentDir, "..", "AppBlueprint.Web", "appsettings.Development.json"));

                AnsiConsole.MarkupLine($"[dim]Current directory: {currentDir}[/]");
                AnsiConsole.MarkupLine($"[dim]Default path: {defaultPath}[/]");

                configPath = await AnsiConsole.AskAsync(
                    "Enter path to [green]appsettings.json[/]:",
                    defaultPath);
            }
        }

        // Load configuration if path provided
        if (!string.IsNullOrEmpty(configPath))
        {
            // Convert to absolute path if relative
            if (!Path.IsPathFullyQualified(configPath))
            {
                var currentDir = Directory.GetCurrentDirectory();
                configPath = Path.GetFullPath(Path.Combine(currentDir, configPath));
            }

            if (File.Exists(configPath))
            {
                try
                {
                    string directory = Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory();
                    var fileName = Path.GetFileName(configPath);

                    // Load base appsettings.json first, then the specific file
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(directory);

                    // If loading Development/Production file, also load base appsettings.json
                    var baseConfigPath = Path.Combine(directory, "appsettings.json");
                    if (File.Exists(baseConfigPath) && !fileName.Equals("appsettings.json", StringComparison.OrdinalIgnoreCase))
                    {
                        builder.AddJsonFile("appsettings.json", optional: true);
                    }

                    builder.AddJsonFile(fileName, optional: false);
                    configuration = builder.Build();

                    AnsiConsole.MarkupLine($"[green]✓[/] Loaded configuration from: {configPath}");

                    string provider = configuration["Authentication:Provider"] ?? "JWT";
                    AnsiConsole.MarkupLine($"[cyan]Using Provider:[/] {provider}");

                    if (provider.Equals(LogtoProviderName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Read environment variables first (flat UPPERCASE format), then fall back to hierarchical config
                        string? endpoint = Environment.GetEnvironmentVariable("LOGTO_ENDPOINT")
                                        ?? Environment.GetEnvironmentVariable("AUTHENTICATION_LOGTO_ENDPOINT")
                                        ?? configuration["Authentication:Logto:Endpoint"];
                        string? clientId = Environment.GetEnvironmentVariable("LOGTO_APPID")
                                        ?? Environment.GetEnvironmentVariable("LOGTO_CLIENTID")
                                        ?? Environment.GetEnvironmentVariable("AUTHENTICATION_LOGTO_CLIENTID")
                                        ?? Environment.GetEnvironmentVariable("LOGTO_APP_ID")
                                        ?? configuration["Authentication:Logto:ClientId"];
                        string? issuer = $"{endpoint?.TrimEnd('/')}/oidc";

                        AnsiConsole.MarkupLine($"[cyan]Issuer:[/] {issuer}");
                        AnsiConsole.MarkupLine($"[cyan]Audience:[/] {clientId}");
                    }

                    AnsiConsole.WriteLine();
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]✗[/] Error loading configuration: {ex.Message}");
                    AnsiConsole.MarkupLine("[yellow]Using default configuration...[/]");
                    configuration = null;
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]✗[/] File not found: {configPath}");
                AnsiConsole.MarkupLine("[yellow]Using default configuration...[/]");
                configuration = null;
            }
        }

        // Get provider after config is loaded
        string finalProvider = configuration?["Authentication:Provider"] ?? LogtoProviderName;

        // For Logto, guide user to get token from browser
        if (finalProvider.Equals(LogtoProviderName, StringComparison.OrdinalIgnoreCase))
        {
            await GetTokenFromLogtoUser(configuration);
            return;
        }

        AnsiConsole.MarkupLine($"[red]✗[/] Provider '{finalProvider}' is not supported.");
        AnsiConsole.MarkupLine("[yellow]This tool only supports Logto authentication via browser automation.[/]");
    }

    private static async Task GetTokenFromLogtoUser(IConfiguration? configuration)
    {
        var endpoint = configuration?["Authentication:Logto:Endpoint"]?.TrimEnd('/');

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]⚠ Logto uses browser-based user authentication.[/]");
        AnsiConsole.WriteLine();

        // Ask user if they want automated extraction or manual paste
        bool useAutomation = await AnsiConsole.ConfirmAsync(
            "Do you want to [green]automatically open browser[/] and extract the token?\n" +
            "[dim](Otherwise you'll need to manually copy/paste the token)[/]",
            defaultValue: true);

        if (useAutomation)
        {
            await GetTokenFromLogtoAutomated(endpoint, configuration);
        }
        else
        {
            await GetTokenFromLogtoManual(endpoint);
        }
    }

    private static async Task GetTokenFromLogtoAutomated(string? endpoint, IConfiguration? configuration)
    {
        try
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[cyan]🌐 Automated Token Extraction[/]");
            AnsiConsole.MarkupLine("[dim]A browser window will open. Please log in with your credentials.[/]");
            AnsiConsole.WriteLine();

            // Get the web app URL from configuration or use default
            string webAppUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ??
                               configuration?["WebApp:Url"] ??
                               DefaultWebAppUrl;

            AnsiConsole.MarkupLine($"[yellow]Opening:[/] {webAppUrl}");
            AnsiConsole.MarkupLine("[dim]Waiting for you to log in...[/]");
            AnsiConsole.WriteLine();

            string? token = null;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Initializing browser...[/]", async ctx =>
                {
                    try
                    {
                        // Initialize Playwright
                        ctx.Status("[yellow]Launching Playwright...[/]");
                        var playwright = await Playwright.CreateAsync();

                        ctx.Status("[yellow]Launching browser...[/]");
                        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                        {
                            Headless = false, // Show browser window
                            Args = ["--ignore-certificate-errors", "--disable-web-security"] // For dev environment
                        });

                        var context = await browser.NewContextAsync(new BrowserNewContextOptions
                        {
                            IgnoreHTTPSErrors = true // For dev certificates
                        });

                        var page = await context.NewPageAsync();

                        ctx.Status("[yellow]Navigating to login page...[/]");
                        await page.GotoAsync(webAppUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });

                        ctx.Status("[yellow]Waiting for authentication...[/]");

                        // Wait for the user to log in and be redirected back
                        // Monitor local storage, session storage, and cookies for authentication tokens
                        var maxWaitTime = TimeSpan.FromMinutes(5);
                        var startTime = DateTime.UtcNow;

                        while (DateTime.UtcNow - startTime < maxWaitTime)
                        {
                            try
                            {
                                // Check localStorage
                                var localStorageKeys = await page.EvaluateAsync<string[]>(
                                    "() => Object.keys(localStorage)"
                                );

                                // Check sessionStorage
                                var sessionStorageKeys = await page.EvaluateAsync<string[]>(
                                    "() => Object.keys(sessionStorage)"
                                );

                                // Get cookies
                                var cookies = await context.CookiesAsync();

                                // Look for any token-related keys
                                var allKeys = localStorageKeys.Concat(sessionStorageKeys).ToList();
                                var tokenKeys = allKeys.Where(k =>
                                    k.Contains("@logto", StringComparison.OrdinalIgnoreCase) ||
                                    k.Contains("access_token", StringComparison.OrdinalIgnoreCase) ||
                                    k.Contains("id_token", StringComparison.OrdinalIgnoreCase) ||
                                    k.Contains("token", StringComparison.OrdinalIgnoreCase) ||
                                    k.Contains("auth", StringComparison.OrdinalIgnoreCase)
                                ).ToList();

                                // Check for token in cookies
                                var tokenCookies = cookies.Where(c =>
                                    c.Name.Contains("token", StringComparison.OrdinalIgnoreCase) ||
                                    c.Name.Contains("auth", StringComparison.OrdinalIgnoreCase) ||
                                    c.Name.Contains("logto", StringComparison.OrdinalIgnoreCase)
                                ).ToList();

                                if (tokenKeys.Count > 0 || tokenCookies.Count > 0)
                                {
                                    ctx.Status("[yellow]Potential tokens found! Extracting...[/]");

                                    // Pause the spinner to show debug info
                                    ctx.Status("[yellow]Analyzing storage...[/]");

                                    // Try localStorage
                                    foreach (var key in tokenKeys.Where(k => localStorageKeys.Contains(k)))
                                    {
                                        var value = await page.EvaluateAsync<string>(
                                            "key => localStorage.getItem(key)", key
                                        );

                                        if (string.IsNullOrWhiteSpace(value))
                                            continue;

                                        // Check if it's a JWT (3 parts separated by dots)
                                        if (value.Split('.').Length == 3)
                                        {
                                            token = value;
                                            break;
                                        }

                                        // Try to parse as JSON
                                        token = ExtractJwtFromJson(value);
                                        if (!string.IsNullOrWhiteSpace(token))
                                            break;
                                    }

                                    // Try sessionStorage if not found in localStorage
                                    if (string.IsNullOrWhiteSpace(token))
                                    {
                                        foreach (var key in tokenKeys.Where(k => sessionStorageKeys.Contains(k)))
                                        {
                                            var value = await page.EvaluateAsync<string>(
                                                "key => sessionStorage.getItem(key)", key
                                            );

                                            if (string.IsNullOrWhiteSpace(value))
                                                continue;

                                            // Check if it's a JWT
                                            if (value.Split('.').Length == 3)
                                            {
                                                token = value;
                                                break;
                                            }

                                            // Try to parse as JSON
                                            token = ExtractJwtFromJson(value);
                                            if (!string.IsNullOrWhiteSpace(token))
                                                break;
                                        }
                                    }

                                    // Try cookies if still not found
                                    if (string.IsNullOrWhiteSpace(token))
                                    {
                                        foreach (var cookie in tokenCookies)
                                        {
                                            var value = cookie.Value;

                                            if (string.IsNullOrWhiteSpace(value))
                                                continue;

                                            // Check if it's a JWT
                                            if (value.Split('.').Length == 3)
                                            {
                                                token = value;
                                                break;
                                            }

                                            // Try to parse as JSON
                                            token = ExtractJwtFromJson(value);
                                            if (!string.IsNullOrWhiteSpace(token))
                                                break;
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(token))
                                        break;
                                }

                                await Task.Delay(1000); // Check every second
                            }
                            catch (PlaywrightException)
                            {
                                // Continue checking
                                await Task.Delay(1000);
                            }
                        }

                        // If no token found, show what's available to help debug
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            ctx.Status("[yellow]No JWT found. Gathering debug info...[/]");

                            var localKeys = await page.EvaluateAsync<string[]>("() => Object.keys(localStorage)");
                            var sessionKeys = await page.EvaluateAsync<string[]>("() => Object.keys(sessionStorage)");
                            var allCookies = await context.CookiesAsync();

                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine("[yellow]Debug Info - What's in storage:[/]");
                            AnsiConsole.MarkupLine($"[dim]LocalStorage keys ({localKeys.Length}): {string.Join(", ", localKeys)}[/]");
                            AnsiConsole.MarkupLine($"[dim]SessionStorage keys ({sessionKeys.Length}): {string.Join(", ", sessionKeys)}[/]");
                            AnsiConsole.MarkupLine($"[dim]Cookies ({allCookies.Count}): {string.Join(", ", allCookies.Select(c => c.Name))}[/]");
                            AnsiConsole.WriteLine();
                        }

                        ctx.Status("[yellow]Cleaning up...[/]");
                        await browser.CloseAsync();
                        playwright.Dispose();
                    }
                    catch (PlaywrightException ex) when (ex.Message.Contains("Executable doesn't exist") || ex.Message.Contains("browserType.launch"))
                    {
                        throw new InvalidOperationException(
                            "Playwright browsers are not installed. Please run: playwright install chromium",
                            ex);
                    }
                });

            // Check if token was successfully extracted
            // Note: This condition is necessary - token may still be null if browser automation completes without finding a token
#pragma warning disable S2583 // Conditionals should not unconditionally evaluate to "true" or to "false"
            if (!string.IsNullOrWhiteSpace(token))
#pragma warning restore S2583
            {
                // Validate and display the token
                await DisplayToken(token, LogtoProviderName);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ No JWT token found in localStorage, sessionStorage, or cookies[/]");
                AnsiConsole.MarkupLine("[yellow]The token might be stored in memory or available through an API call.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Falling back to manual token paste...[/]");
                await GetTokenFromLogtoManual(endpoint);
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Playwright browsers"))
        {
            AnsiConsole.MarkupLine($"[red]✗ {ex.Message}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]To install Playwright browsers, run:[/]");
            AnsiConsole.MarkupLine("[green]playwright install chromium[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Falling back to manual token paste...[/]");
            AnsiConsole.WriteLine();
            await GetTokenFromLogtoManual(endpoint);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Browser automation failed:[/] {ex.Message}");
            if (ex.InnerException is not null)
            {
                AnsiConsole.MarkupLine($"[dim]Inner exception: {ex.InnerException.Message}[/]");
            }
            AnsiConsole.MarkupLine("[yellow]Falling back to manual token paste...[/]");
            AnsiConsole.WriteLine();
            await GetTokenFromLogtoManual(endpoint);
        }
    }

    private static string? ExtractJwtFromJson(string jsonString)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(jsonString, new JsonDocumentOptions());

            // Try different possible token property names
            var possibleProps = new[] { "access_token", "accessToken", "idToken", "id_token", "token", "jwt", "bearerToken" };

            foreach (var propName in possibleProps)
            {
                if (jsonDoc.RootElement.TryGetProperty(propName, out var tokenProp))
                {
                    var tokenValue = tokenProp.GetString();
                    if (!string.IsNullOrEmpty(tokenValue) && tokenValue.Split('.').Length == 3)
                    {
                        return tokenValue;
                    }
                }
            }

            // Search through all properties for anything that looks like a JWT
            foreach (var prop in jsonDoc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    var propValue = prop.Value.GetString();
                    if (!string.IsNullOrEmpty(propValue) && propValue.Split('.').Length == 3 && propValue.StartsWith("ey"))
                    {
                        return propValue;
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Not JSON
        }

        return null;
    }

    private static async Task GetTokenFromLogtoManual(string? endpoint)
    {
        AnsiConsole.MarkupLine("[cyan]📋 Manual Token Extraction[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[yellow]⚠ Your app uses server-side authentication (Blazor Server)[/]");
        AnsiConsole.MarkupLine("[dim]JWT tokens are NOT sent in HTTP headers for Blazor Server SignalR connections.[/]");
        AnsiConsole.MarkupLine("[dim]The authentication happens server-side using cookies.[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[red]The Problem:[/]");
        AnsiConsole.MarkupLine("[dim]Blazor Server uses SignalR over WebSockets, not REST API calls.[/]");
        AnsiConsole.MarkupLine("[dim]SignalR uses cookie-based authentication, so there's no Authorization header to extract.[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[cyan]Solution: Test API Endpoint[/]");
        AnsiConsole.MarkupLine("[dim]I've created a development endpoint to help you get a JWT token for testing.[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[green]Option 1: Get Token via API Endpoint[/]");
        AnsiConsole.MarkupLine("[yellow]1.[/] Make sure your API is running (AppHost should be running)");
        AnsiConsole.MarkupLine("[yellow]2.[/] Log into your Logto account at [green]http://localhost:8092[/]");
        AnsiConsole.MarkupLine("[yellow]3.[/] Open a new browser tab or use PowerShell:");
        AnsiConsole.WriteLine();

        const string psCode = @"# PowerShell - Make an authenticated API call
Invoke-RestMethod -Uri 'https://localhost:9100/api/v1/authtest/echo' `
    -Method Get `
    -UseDefaultCredentials `
    -SkipCertificateCheck";

        var codePanel = new Panel(psCode)
        {
            Header = new PanelHeader("[green]PowerShell Command[/]"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Green)
        };
        AnsiConsole.Write(codePanel);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[yellow]4.[/] Look for [green]'TokenPreview'[/] or [green]'HasAuthHeader: true'[/] in the response");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[green]Option 2: Use Postman or REST Client[/]");
        AnsiConsole.MarkupLine("[yellow]1.[/] Get a JWT token from Logto directly (use their OAuth flow)");
        AnsiConsole.MarkupLine("[yellow]2.[/] Test it with: [green]GET https://localhost:9100/api/v1/authtest/protected[/]");
        AnsiConsole.MarkupLine("[yellow]3.[/] Add header: [green]Authorization: Bearer <your-token>[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[green]Option 3: Get Token from Logto Directly[/]");
        AnsiConsole.MarkupLine("[yellow]1.[/] Go to Logto admin console");
        AnsiConsole.MarkupLine("[yellow]2.[/] Navigate to: Applications → Your App → Interact");
        AnsiConsole.MarkupLine("[yellow]3.[/] Use the 'Authorization Code' flow to get a token");
        AnsiConsole.MarkupLine("[yellow]4.[/] Or use Logto's API to generate a token for testing");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"[dim]Logto Endpoint: {endpoint}[/]");
        AnsiConsole.MarkupLine($"[dim]JWT tokens have 3 parts separated by dots: [green]header.payload.signature[/][/]");
        AnsiConsole.MarkupLine($"[dim]They always start with: [green]eyJ[/][/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[yellow]Do you have a JWT token to paste?[/]");
        bool hasToken = await AnsiConsole.ConfirmAsync("Paste token now?", false);

        if (!hasToken)
        {
            AnsiConsole.MarkupLine("[yellow]Exiting. Run this command again when you have a token.[/]");
            return;
        }

        string token = await AnsiConsole.AskAsync<string>("Paste your [green]JWT token[/] here:");

        if (string.IsNullOrWhiteSpace(token))
        {
            AnsiConsole.MarkupLine("[red]✗ No token provided[/]");
            return;
        }

        // Clean up common issues
        token = token.Trim();
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token[7..].Trim();
        }

        // Validate it's actually a JWT
        if (token.Split('.').Length != 3)
        {
            AnsiConsole.MarkupLine("[red]✗ Invalid JWT format[/]");
            AnsiConsole.MarkupLine("[yellow]JWT tokens must have exactly 3 parts separated by dots (e.g., eyJhbGc...)[/]");
            AnsiConsole.MarkupLine($"[dim]You provided: {token[..Math.Min(50, token.Length)]}...[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Make sure to copy ONLY the token part after 'Bearer '[/]");
            return;
        }

        if (!token.StartsWith("eyJ", StringComparison.Ordinal))
        {
            AnsiConsole.MarkupLine("[yellow]⚠ Warning: JWT tokens typically start with 'eyJ'[/]");
            AnsiConsole.MarkupLine($"[dim]Your token starts with: {token[..Math.Min(10, token.Length)]}[/]");

            if (!await AnsiConsole.ConfirmAsync("Do you want to continue anyway?", false))
            {
                return;
            }
        }

        await DisplayToken(token, "Logto");
    }

    private static async Task DisplayToken(string token, string provider)
    {
        // Validate and display the token
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var panel = new Panel(new Markup($"[green]✓ User token validated successfully![/]"))
            {
                Header = new PanelHeader("Success"),
                Border = BoxBorder.Rounded
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            // Token details table
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[yellow]Property[/]").Centered())
                .AddColumn(new TableColumn("[yellow]Value[/]"));

            string sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "N/A";
            string email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "N/A";
            string nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "N/A";

            table.AddRow("Subject (User ID)", sub);
            if (email != "N/A")
            {
                table.AddRow("Email", email);
            }
            if (nameClaim != "N/A")
            {
                table.AddRow("Name", nameClaim);
            }
            table.AddRow("Expires", jwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC");
            table.AddRow("Provider", provider);
            table.AddRow("Issuer", jwtToken.Issuer);
            table.AddRow("Audience", string.Join(", ", jwtToken.Audiences));

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            // JWT Token
            var tokenPanel = new Panel(token)
            {
                Header = new PanelHeader("[cyan]USER JWT TOKEN (copy this)[/]"),
                Border = BoxBorder.Double,
                Padding = new Padding(2, 1)
            };
            AnsiConsole.Write(tokenPanel);
            AnsiConsole.WriteLine();

            // Usage examples
            var examplesPanel = new Panel(
                new Markup(
                    "[yellow]Swagger UI:[/]\n" +
                    "[dim]1. Go to https://localhost:9100/swagger\n" +
                    "2. Click 'Authorize' button (🔒 lock icon)\n" +
                    $"3. Enter: Bearer {token[..Math.Min(30, token.Length)]}...\n" +
                    "4. Click 'Authorize'\n" +
                    "5. Try protected endpoints[/]\n\n" +
                    "[yellow]PowerShell:[/]\n" +
                    $"[dim]$token = \"{token[..Math.Min(50, token.Length)]}...\"\n" +
                    "Invoke-RestMethod -Uri 'https://localhost:9100/api/v1/authtest/protected' `\n" +
                    "    -Headers @{Authorization=\"Bearer $token\"} -SkipCertificateCheck[/]\n\n" +
                    "[yellow]cURL:[/]\n" +
                    "[dim]curl -H \"Authorization: Bearer {token}\" https://localhost:9100/api/v1/authtest/protected[/]"
                ))
            {
                Header = new PanelHeader("[cyan]How to Use This Token[/]"),
                Border = BoxBorder.Rounded
            };
            AnsiConsole.Write(examplesPanel);

            AnsiConsole.MarkupLine("\n[green]✓ Token ready to use. Press any key to continue...[/]");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Invalid token:[/] {ex.Message}");
            AnsiConsole.WriteException(ex);
        }

        await Task.CompletedTask;
    }
}

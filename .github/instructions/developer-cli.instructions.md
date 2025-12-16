---
applies_to:
  - "Code/AppBlueprint/AppBlueprint.DeveloperCli/**"
---

# Developer CLI Instructions

When working with the Developer CLI project:

## Quick Reference

- Consult [Developer CLI Rules](../.ai-rules/developer-cli/README.md)

## Key Technologies

- **Spectre.Console**: For interactive terminal UI
- **Command Pattern**: For organizing CLI commands
- **Menu-Driven**: Interactive menus for ease of use

## CLI Features

The Developer CLI provides automation for:

1. **Database Migrations**: Migrate or rollback EF Core migrations
2. **Git Operations**: Commit and push to GitHub
3. **Code Generation**: Generate API controllers, projects, solutions
4. **GitHub Integration**: Create repos and GitHub Actions workflows
5. **Project Management**: Generate Visual Studio solutions

## Development Guidelines

1. **Spectre.Console**: Use for all console interactions
2. **Command Pattern**: Follow existing command structure
3. **Documentation**: Add help text for all commands
4. **Manual Testing**: Test CLI commands before committing
5. **Single Purpose**: Keep commands focused and simple

## Example Command Structure

```csharp
public class MyCommand : ICommand
{
    public string Name => "my-command";
    public string Description => "Does something useful";
    
    public async Task ExecuteAsync()
    {
        AnsiConsole.MarkupLine("[green]Executing command...[/]");
        
        // Command logic here
        
        AnsiConsole.MarkupLine("[green]✓ Complete![/]");
    }
}
```

## Spectre.Console Patterns

```csharp
// Prompts
var name = AnsiConsole.Ask<string>("What's your [green]name[/]?");

// Confirmations
if (AnsiConsole.Confirm("Are you sure?"))
{
    // Proceed
}

// Progress
await AnsiConsole.Progress()
    .StartAsync(async ctx =>
    {
        var task = ctx.AddTask("[green]Processing...[/]");
        // Work here
    });

// Tables
var table = new Table();
table.AddColumn("Name");
table.AddColumn("Status");
table.AddRow("Task 1", "[green]Complete[/]");
AnsiConsole.Write(table);
```

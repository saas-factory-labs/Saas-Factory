using System.Diagnostics;
using Spectre.Console;

namespace AppBlueprint.DeveloperCli.Utilities;

internal static class CliUtilities
{
    public static bool RunShellCommand(string command, string successMessage, string errorMessage,
        string workingDirectory = "")
    {
        try
        {
            AnsiConsole.MarkupLine($"[yellow]🛠 Running command: [bold]{command}[/][/]");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrEmpty(workingDirectory)) processStartInfo.WorkingDirectory = workingDirectory;

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        AnsiConsole.MarkupLine($"[blue]{args.Data}[/]");
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        AnsiConsole.MarkupLine($"[red]{args.Data}[/]");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Set a 30-second timeout
                bool completed = process.WaitForExit(30 * 1000);

                if (completed && process.ExitCode == 0)
                {
                    AnsiConsole.MarkupLine($"[green]{successMessage}[/]");
                    return true;
                }

                AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
                return false;
            }
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Process error: {ex.Message}[/]");
            return false;
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ System error: {ex.Message}[/]");
            return false;
        }
        catch (TimeoutException ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Timeout error: {ex.Message}[/]");
            return false;
        }
    }
}

// using System;
// using System.Diagnostics;
// using Spectre.Console;

// namespace AppBlueprint.DeveloperCli.Utilities
// {
//     public static class CLIUtilities
//     {
//         public static bool RunShellCommand(string command, string successMessage, string errorMessage, string workingDirectory = "")
//         {
//             try
//             {
//                 AnsiConsole.MarkupLine($"[yellow]🛠 Running command: [bold]{command}[/][/]");

//                 var processStartInfo = new ProcessStartInfo
//                 {
//                     FileName = "cmd.exe",
//                     Arguments = $"/c {command}",
//                     RedirectStandardOutput = true,
//                     RedirectStandardError = true,
//                     UseShellExecute = false,
//                     CreateNoWindow = true
//                 };

//                 if (!string.IsNullOrEmpty(workingDirectory))
//                 {
//                     processStartInfo.WorkingDirectory = workingDirectory;
//                 }

//                 using (var process = new Process { StartInfo = processStartInfo })
//                 {
//                     process.OutputDataReceived += (sender, args) =>
//                     {
//                         if (!string.IsNullOrEmpty(args.Data))
//                             AnsiConsole.MarkupLine($"[blue]{args.Data}[/]");
//                     };

//                     process.ErrorDataReceived += (sender, args) =>
//                     {
//                         if (!string.IsNullOrEmpty(args.Data))
//                             AnsiConsole.MarkupLine($"[red]{args.Data}[/]");
//                     };

//                     process.Start();
//                     process.BeginOutputReadLine();
//                     process.BeginErrorReadLine();

//                     // Set a timeout to avoid hanging indefinitely (30 seconds in this example)
//                     bool completed = process.WaitForExit(30 * 1000);

//                     if (completed && process.ExitCode == 0)
//                     {
//                         AnsiConsole.MarkupLine($"[green]{successMessage}[/]");
//                         return true;
//                     }
//                     else
//                     {
//                         AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
//                         return false;
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 AnsiConsole.MarkupLine($"[red]❌ Exception: {ex.Message}[/]");
//                 return false;
//             }
//         }
//     }
// }

// // using System;
// // using System.Diagnostics;
// // using Spectre.Console;

// // namespace AppBlueprint.DeveloperCli.Utilities
// // {
// //     public static class CLIUtilities
// //     {
// //         public static bool RunShellCommand(string command, string successMessage, string errorMessage, string workingDirectory = "")
// //         {
// //             try
// //             {
// //                 AnsiConsole.MarkupLine($"[yellow]🛠 Running command: [bold]{command}[/][/]");

// //                 var processStartInfo = new ProcessStartInfo
// //                 {
// //                     FileName = "cmd.exe",
// //                     Arguments = $"/c {command}",
// //                     RedirectStandardOutput = true,
// //                     RedirectStandardError = true,
// //                     UseShellExecute = false,
// //                     CreateNoWindow = true
// //                 };

// //                 if (!string.IsNullOrEmpty(workingDirectory))
// //                 {
// //                     processStartInfo.WorkingDirectory = workingDirectory;
// //                 }

// //                 using (var process = new Process { StartInfo = processStartInfo })
// //                 {
// //                     process.OutputDataReceived += (sender, args) =>
// //                     {
// //                         if (!string.IsNullOrEmpty(args.Data))
// //                             AnsiConsole.MarkupLine($"[blue]{args.Data}[/]");
// //                     };

// //                     process.ErrorDataReceived += (sender, args) =>
// //                     {
// //                         if (!string.IsNullOrEmpty(args.Data))
// //                             AnsiConsole.MarkupLine($"[red]{args.Data}[/]");
// //                     };

// //                     process.Start();
// //                     process.BeginOutputReadLine();
// //                     process.BeginErrorReadLine();
// //                     process.WaitForExit();

// //                     if (process.ExitCode == 0)
// //                     {
// //                         AnsiConsole.MarkupLine($"[green]{successMessage}[/]");
// //                         return true;
// //                     }
// //                     else
// //                     {
// //                         AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
// //                         return false;
// //                     }
// //                 }
// //             }
// //             catch (Exception ex)
// //             {
// //                 AnsiConsole.MarkupLine($"[red]❌ Exception: {ex.Message}[/]");
// //                 return false;
// //             }
// //         }
// //     }
// // }

// // // using Spectre.Console;
// // // using System.Diagnostics;
// // //
// // // namespace AppBlueprint.DeveloperCli.Utilities;
// // //
// // // internal static class CLIUtilities
// // // {
// // //     public static void PrintHeader()
// // //     {
// // //         AnsiConsole.Write(new Rule("[bold orange1]AppBlueprint CLI[/]").RuleStyle("yellow").Centered());
// // //         AnsiConsole.MarkupLine("[bold green]Streamline your SaaS application development.[/]");
// // //         AnsiConsole.MarkupLine("[italic blue]Type 'help' to explore commands or interact via the menu below.[/]");
// // //         AnsiConsole.Write(new Rule().RuleStyle("orange1"));
// // //     }
// // //
// // //     public static void RunShellCommand(string command, string successMessage, string errorMessage)
// // //     {
// // //         try
// // //         {
// // //             var process = new Process
// // //             {
// // //                 StartInfo = new ProcessStartInfo
// // //                 {
// // //                     FileName = "cmd.exe",
// // //                     Arguments = $"/c {command}",
// // //                     RedirectStandardOutput = true,
// // //                     RedirectStandardError = true,
// // //                     UseShellExecute = false,
// // //                     CreateNoWindow = true
// // //                 }
// // //             };
// // //
// // //             process.Start();
// // //
// // //             string output = process.StandardOutput.ReadToEnd();
// // //             string error = process.StandardError.ReadToEnd();
// // //
// // //             process.WaitForExit();
// // //
// // //             if (process.ExitCode == 0)
// // //             {
// // //                 AnsiConsole.MarkupLine($"[green]{successMessage}[/]");
// // //                 if (!string.IsNullOrEmpty(output)) Console.WriteLine(output);
// // //             }
// // //             else
// // //             {
// // //                 AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
// // //                 if (!string.IsNullOrEmpty(error)) Console.WriteLine(error);
// // //             }
// // //         }
// // //         catch (Exception ex)
// // //         {
// // //             AnsiConsole.MarkupLine($"[red]Exception: {ex.Message}[/]");
// // //         }
// // //     }
// // // }

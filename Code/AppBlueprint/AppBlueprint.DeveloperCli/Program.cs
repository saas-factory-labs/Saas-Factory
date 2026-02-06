using AppBlueprint.DeveloperCli.Commands;
using AppBlueprint.DeveloperCli.Menus;

namespace AppBlueprint.DeveloperCli;

internal static class Program
{
    private static void Main(string[] args)
    {
        // No need to configure Spectre.Console - it auto-detects terminal capabilities
        // The fallback in MainMenu.cs handles non-ANSI terminals

        if (args.Length is 0)
        {
            MainMenu.Show();
        }
        else
        {
            RootCommand rootCommand = CommandFactory.CreateRootCommand();
            rootCommand.InvokeAsync(args).Wait();
        }
    }
}

// C# Script to Update NuGet Packages in Central Package Management (CPM)
// Shows version color legend and checks for known security vulnerabilities

// #r "nuget: NuGet.Protocol, 6.4.0"
// #r "nuget: NuGet.Versioning, 6.4.0"
//
// using System;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Xml.Linq;
// using NuGet.Protocol.Core.Types;
// using NuGet.Protocol;
// using NuGet.Configuration;
// using NuGet.Versioning;
//
// async Task Main()
// {
//     var propsFile = "Directory.Packages.props";
//
//     if (!File.Exists(propsFile))
//     {
//         Console.WriteLine("Directory.Packages.props not found.");
//         return;
//     }
//
//     var doc = XDocument.Load(propsFile);
//     var packageElements = doc.Descendants("PackageVersion").ToList();
//
//     var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(Settings.LoadDefaultSettings(null)),
//                                                                 Repository.Provider.GetCoreV3());
//
//     Console.WriteLine("Version color legend:");
//     Console.WriteLine("\x1b[31m<red>   : Major version update or pre-release version. Possible breaking changes.\x1b[0m");
//     Console.WriteLine("\x1b[33m<yellow>: Minor version update. Backwards-compatible features added.\x1b[0m");
//     Console.WriteLine("\x1b[32m<green> : Patch version update. Backwards-compatible bug fixes.\x1b[0m\n");
//
//     foreach (var package in packageElements)
//     {
//         var packageName = package.Attribute("Include")?.Value;
//         var currentVersion = new NuGetVersion(package.Attribute("Version")?.Value);
//
//         var repository = sourceRepositoryProvider.GetRepositories().First();
//         var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
//         var versions = await resource.GetAllVersionsAsync(packageName, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);
//
//         var latestVersion = versions.Max();
//         var color = latestVersion.Major > currentVersion.Major ? "\x1b[31m" :
//                     latestVersion.Minor > currentVersion.Minor ? "\x1b[33m" :
//                     latestVersion.Patch > currentVersion.Patch ? "\x1b[32m" : "";
//
//         if (latestVersion > currentVersion)
//         {
//             Console.WriteLine($"{color}{packageName}: {currentVersion} -> {latestVersion}\x1b[0m");
//             Console.Write("Do you want to upgrade this package? (y/n): ");
//             var input = Console.ReadLine()?.Trim().ToLower();
//
//             if (input == "y" || input == "yes")
//             {
//                 package.Attribute("Version").Value = latestVersion.ToString();
//             }
//         }
//     }
//
//     doc.Save(propsFile);
//     Console.WriteLine("Package updates completed.");
// }
//
// await Main();

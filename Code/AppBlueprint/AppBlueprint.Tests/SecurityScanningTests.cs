using System.IO;
using SaaSFactory.SharedTestKernel;
using TUnit.Core;

namespace AppBlueprint.Tests;

public sealed class SecurityScanningTests
{
    private const string CodeQlQueriesPath = @"D:\Tools\codeql-queries";
    private const string CsharpQuerySuiteRelativePath = @"csharp\ql\src\codeql-suites\csharp-security-and-quality.qls";

    [Test]
    [Category("Security")]
    public void Run_CodeQL_Csharp_Analysis_On_AppBlueprint()
    {
        string targetProject = Path.Combine("Code", "AppBlueprint", "AppBlueprint.ApiService", "AppBlueprint.ApiService.csproj");

        CodeQlTestRunner.AssertNoCodeQlViolations(
            language: "csharp",
            targetProjectOrFolder: targetProject,
            querySuite: CsharpQuerySuiteRelativePath,
            externalQueryPath: CodeQlQueriesPath);
    }

    // [Test]
    // [Category("Security")]
    // public void Run_CodeQL_JavaScript_Analysis_On_Scripts()
    // {
    //     CodeQlTestRunner.AssertNoCodeQlViolations(
    //         language: "javascript",
    //         targetProjectOrFolder: "Code\\AppBlueprint",
    //         querySuite: @"javascript\ql\src\codeql-suites\javascript-security-and-quality.qls",
    //         externalQueryPath: CodeQlQueriesPath);
    // }
}

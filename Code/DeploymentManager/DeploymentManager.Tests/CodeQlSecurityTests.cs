using System.IO;
using SaaSFactory.SharedTestKernel;
using TUnit.Core;

namespace DeploymentManager.Tests;

public class CodeQlSecurityTests
{
    [Test]
    [Category("Security")]
    public void Run_CodeQL_Csharp_Analysis_On_DeploymentManager()
    {
        string targetProject = Path.Combine("Code", "DeploymentManager", "DeploymentManager", "DeploymentManager.csproj");
        CodeQlTestRunner.AssertNoCodeQlViolations(
            language: "csharp",
            targetProjectOrFolder: targetProject,
            querySuite: @"csharp\ql\src\codeql-suites\csharp-security-and-quality.qls",
            externalQueryPath: @"D:\Tools\codeql-queries"
        );
    }
}

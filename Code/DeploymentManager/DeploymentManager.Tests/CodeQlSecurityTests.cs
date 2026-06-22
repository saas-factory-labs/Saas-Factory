using System.IO;
using TUnit.Core;
using SaaSFactory.SharedTestKernel;

namespace DeploymentManager.Tests;

public class CodeQlSecurityTests
{
    [Test]
    [Category("Security")]
    public void Run_CodeQL_Csharp_Analysis_On_DeploymentManager()
    {
        // Stien til dit specifikke projekt under løsningsroden
        string targetProject = Path.Combine("Code", "DeploymentManager", "DeploymentManager", "DeploymentManager.csproj");
        
        // Kald din opdaterede runner med de eksterne stier
        CodeQlTestRunner.AssertNoCodeQlViolations(
            language: "csharp",
            targetProjectOrFolder: targetProject,
            querySuite: @"csharp\ql\src\codeql-suites\csharp-security-and-quality.qls", // Relativ sti inde i query-mappen
            externalQueryPath: @"D:\Tools\codeql-queries" // Din nye fysiske placering uden for repoet
        );
    }
}
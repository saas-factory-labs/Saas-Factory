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
        // ATTENTION: Ret den midterste del til den præcise sti til dit primære DeploymentManager projekt
        string targetProject = Path.Combine("Code", "DeploymentManager", "DeploymentManager", "DeploymentManager.csproj");
        
        CodeQlTestRunner.AssertNoCodeQlViolations("csharp", targetProject);
    }
}

using System.IO;
using TUnit.Core;
using SaaSFactory.SharedTestKernel;

namespace AppBlueprint.Tests;

public class SecurityScanningTests
{
    [Test]
    [Category("Security")]
    public void Run_CodeQL_Csharp_Analysis_On_AppBlueprint()
    {
        // ATTENTION: Ret "AppBlueprint.ApiService" til navnet på din rigtige kildekods-mappe/csproj hvis den hedder noget andet
        string targetProject = Path.Combine("Code", "AppBlueprint", "AppBlueprint.ApiService", "AppBlueprint.ApiService.csproj");
        
        CodeQlTestRunner.AssertNoCodeQlViolations("csharp", targetProject);
    }

    // [Test]
    // [Category("Security")]
    // public void Run_CodeQL_JavaScript_Analysis_On_Scripts()
    // {
    //     // RET HER: Sørg for at den peger direkte på din web-app eller script-mappe, IKKE på løsningens rod!
    //     CodeQlTestRunner.AssertNoCodeQlViolations("javascript", "Code\\AppBlueprint");
    // }
}
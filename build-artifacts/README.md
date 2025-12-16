# Build Artifacts

This directory contains build output files, logs, and temporary build artifacts that should not be committed to version control.

## Contents

This directory typically includes:
- **`build-warnings.log`** - Detailed MSBuild diagnostic output and warnings
- **`warnings.txt`** - Compiler command-line invocations and warnings
- Other temporary build files and logs

## Important Notes

⚠️ **This directory is excluded from version control** (see `.gitignore`)

- Files in this directory are generated during the build process
- Do not commit these files to the repository
- The directory itself is tracked to preserve structure
- Clean this directory periodically to free up disk space

## Cleaning Build Artifacts

To clean build artifacts:

```powershell
# Remove all files in build-artifacts (PowerShell)
Remove-Item build-artifacts\* -Force -Recurse

# Or use dotnet clean for all projects
dotnet clean
```

## Related

- [Build Configuration](../Directory.Build.props) - Global build properties
- [Package Configuration](../Code/AppBlueprint/Directory.Packages.props) - NuGet package versions
- [Editor Config](../.editorconfig) - Code style and analyzer rules


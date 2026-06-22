# Enforces that every 'uses:' reference in GitHub Actions workflow files is pinned
# to an immutable 40-character commit SHA, preventing supply chain attacks via
# tag retargeting. Run as a pre-commit hook or in CI.

$repoRoot = Resolve-Path (Join-Path (Join-Path $PSScriptRoot "..") "..")
$workflowDir = Join-Path (Join-Path $repoRoot ".github") "workflows"

$workflowFiles = Get-ChildItem -Path $workflowDir -Include "*.yml", "*.yaml" -Recurse -File

if ($workflowFiles.Count -eq 0) {
    Write-Host "No workflow files found in $workflowDir" -ForegroundColor Yellow
    exit 0
}

$violations = [System.Collections.Generic.List[PSCustomObject]]::new()
$sha40Pattern = '^[0-9a-f]{40}$'

foreach ($file in $workflowFiles) {
    $lineNumber = 0
    foreach ($line in Get-Content $file.FullName) {
        $lineNumber++

        if ($line -match '^\s*#' -or $line -match '^\s*$') { continue }

        if ($line -notmatch '^\s+uses:\s+(\S+)') { continue }

        $ref = $matches[1]

        # Local workflow calls (./.github/workflows/foo.yml) are always safe
        if ($ref -match '^\.\/') { continue }

        $atIndex = $ref.LastIndexOf('@')
        if ($atIndex -lt 0) {
            $violations.Add([PSCustomObject]@{
                File      = $file.Name
                Line      = $lineNumber
                Reference = $ref
                Reason    = "No SHA pin - missing '@<sha>'"
            })
            continue
        }

        $pin = $ref.Substring($atIndex + 1)

        if ($pin -notmatch $sha40Pattern) {
            $violations.Add([PSCustomObject]@{
                File      = $file.Name
                Line      = $lineNumber
                Reference = $ref
                Reason    = "'@$pin' is not a 40-character commit SHA"
            })
        }
    }
}

if ($violations.Count -eq 0) {
    Write-Host "PASS: All GitHub Actions workflow references are pinned to immutable SHAs." -ForegroundColor Green
    exit 0
}

Write-Host ""
Write-Host "FAIL: $($violations.Count) unpinned GitHub Actions reference(s) found:" -ForegroundColor Red
Write-Host ""

$byFile = $violations | Group-Object -Property File
foreach ($group in $byFile) {
    Write-Host "  $($group.Name)" -ForegroundColor Yellow
    foreach ($v in $group.Group) {
        Write-Host "    Line $($v.Line): $($v.Reference)" -ForegroundColor White
        Write-Host "             $($v.Reason)" -ForegroundColor DarkGray
    }
    Write-Host ""
}

Write-Host "Fix: replace mutable tags with full 40-character commit SHAs, e.g.:" -ForegroundColor Cyan
Write-Host "  uses: actions/checkout@34e114876b0b11c390a56381ad16ebd13914f8d5  # v4.3.1" -ForegroundColor Cyan
Write-Host ""
Write-Host "See .github/GITHUB_ACTIONS_SHA_REFERENCE.md for known SHA values." -ForegroundColor Cyan
Write-Host ""
exit 1

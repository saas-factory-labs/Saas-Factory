# Git Hooks Setup - GitGuardian Shield

## Overview
This repository uses [GitGuardian Shield (ggshield)](https://github.com/gitguardian/ggshield) via pre-commit hooks to automatically scan commits for secrets and sensitive data before they are committed.

## What's Configured
- **Tool**: GitGuardian Shield (ggshield) v1.47.0
- **Hook Type**: Pre-commit
- **Purpose**: Scans for API keys, passwords, tokens, and other sensitive data in your commits

## Setup Status
✅ Pre-commit hooks are installed and active
✅ ggshield is configured in `.pre-commit-config.yaml`
✅ Runs automatically before every git commit

## How It Works
When you run `git commit`, the pre-commit hook will:
1. Scan all staged files for secrets
2. Block the commit if any secrets are detected
3. Display which files contain potential secrets
4. Allow the commit if no secrets are found

## Manual Testing
To manually scan all files:
```powershell
python -m pre_commit run --all-files
```

To manually scan only staged files:
```powershell
python -m pre_commit run
```

## Updating Hooks
To update ggshield to the latest version:
```powershell
python -m pre_commit autoupdate --repo https://github.com/gitguardian/ggshield
```

To update all pre-commit hooks:
```powershell
python -m pre_commit autoupdate
```

## Bypassing the Hook (Not Recommended)
If you absolutely must bypass the hook (e.g., for a false positive):
```powershell
git commit --no-verify -m "your message"
```

⚠️ **Warning**: Only bypass the hook if you're certain the detected item is not a real secret.

## Troubleshooting

### Pre-commit not found
If `pre-commit` command is not recognized, use:
```powershell
python -m pre_commit [command]
```

### Reinstalling hooks
If hooks aren't working:
```powershell
python -m pre_commit uninstall
python -m pre_commit install
```

### Clean and reinstall environments
```powershell
python -m pre_commit clean
python -m pre_commit install --install-hooks
```

## GitGuardian API Token (Optional)
For enhanced scanning with GitGuardian's API:

1. Get a free API token from [GitGuardian Dashboard](https://dashboard.gitguardian.com/)
2. Set the environment variable:
   ```powershell
   $env:GITGUARDIAN_API_KEY = "your-api-key-here"
   ```
3. Or add to your Doppler secrets as `GITGUARDIAN_API_KEY`

## Configuration File
The configuration is in `.pre-commit-config.yaml` at the repository root.

## Additional Resources
- [GitGuardian Shield Documentation](https://docs.gitguardian.com/ggshield-docs/getting-started)
- [Pre-commit Framework](https://pre-commit.com/)

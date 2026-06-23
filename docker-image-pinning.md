# Docker Image Pinning — Security vs Readability

## The problem

SonarCloud rule `docker:S8431` flags using both a version tag and a digest on the same `FROM` line:

```dockerfile
# Invalid — tag and digest combined
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:abc123...
```

Docker pulls by digest when both are specified — the tag becomes decorative. The rule requires you to pick one.

---

## Why digest pinning matters for supply chain security

With a mutable tag like `:10.0`, the registry owner (or an attacker who has compromised it) can push a new image under that tag at any time. Your next `docker build` silently pulls whatever is there — no warning, no audit trail.

A digest pins to an exact image layer identified by its SHA-256 hash. Nothing can change what you pull without changing the hash, making the reference tamper-evident.

---

## Options

| Option | Supply chain security | Readability | Maintenance |
|--------|----------------------|-------------|-------------|
| Tag only (`:10.0`) | Low — tag is mutable | High | None |
| Tag + digest (current, flagged) | High — digest wins | High | Manual SHA rotation |
| Digest only | High | Low | Manual SHA rotation |
| Digest + comment + Renovate | High | Medium | Automated via PRs |

---

## Recommended pattern: digest + comment + Renovate

### Dockerfile

```dockerfile
# mcr.microsoft.com/dotnet/aspnet:10.0
FROM mcr.microsoft.com/dotnet/aspnet@sha256:abc123...
```

The comment preserves human-readable context. Docker uses the digest exclusively — it is immutable and resistant to tag-mutation attacks.

### Automate SHA rotation with Renovate Bot

Manually keeping digests up to date is impractical. [Renovate Bot](https://docs.renovatebot.com/) (free GitHub App) handles this automatically:

- Detects pinned digests in all Dockerfiles
- Opens a PR whenever a new digest is published for a pinned image
- Labels the PR with the human-readable version (e.g. `dotnet/aspnet 10.0.1`)
- Gives you a review + merge audit trail for every base image update

Add a `renovate.json` at the repository root:

```json
{
  "extends": ["config:base"],
  "docker": {
    "pinDigests": true
  }
}
```

Renovate then manages the SHA rotation, so you get supply chain security without manual maintenance overhead.

---

## Summary

- **Tag only** — fine for local dev, not acceptable for production supply chain security.
- **Digest only** — secure but unreadable and hard to maintain manually.
- **Digest + comment + Renovate** — industry standard for secure pipelines: immutable pulls, human-readable context, automated patch PRs.

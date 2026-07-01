---
description: Infrastructure rules for cloud hosting, deployment, and DevOps
globs:
  - "**/*.yml"
  - "**/*.yaml"
  - "**/Dockerfile"
  - "**/*.bicep"
  - "**/Pulumi.*.yaml"
---

# Infrastructure Rules

When implementing infrastructure code, follow these rules very carefully.

- Infrastructure code tools that will be used:

  - Pulumi

## Rules Files

- **[Docker Security](./docker-security.md) - CRITICAL: SHA256 digest pinning requirement and SonarCloud configuration for .NET projects.**

*Additional rules will cover cloud hosting infrastructure using Docker, deployment patterns, database migration strategy, GitHub Action workflows, and cloud services configuration using Pulumi.*

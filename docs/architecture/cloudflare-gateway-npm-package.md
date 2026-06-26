# AppBlueprint Gateway — npm Package (Future Plan)

## Context

Every SaaS app built on AppBlueprint (e.g. boligportal, dating app) deploys via Cloudflare Workers Containers using the same gateway pattern: route `/api/*` to the API container, everything else to the Web container.

Currently each app copies `Code/AppBlueprint/Cloudflare-Workers/src/index.ts` into their own repo (option 3 — copy). This works but means gateway logic fixes or improvements must be manually propagated to every app repo.

## Goal

Publish the AppBlueprint gateway as a versioned npm package so all SaaS apps can consume it as a dependency — the same way they consume `AppBlueprint.Infrastructure` as a NuGet package.

## Package structure

```
Code/AppBlueprint/Cloudflare-Workers/
├── src/
│   └── index.ts          ← entry point exported by the package
├── package.json          ← name: "@appblueprint/gateway", main: "src/index.ts"
├── tsconfig.json
└── wrangler.toml         ← AppBlueprint's own deployment config (not published)
```

## How each SaaS app uses it

Each app's `wrangler.toml` points `main` at the installed package:

```toml
main = "node_modules/@appblueprint/gateway/src/index.ts"
```

And their `package.json` declares the dependency:

```json
{
  "dependencies": {
    "@appblueprint/gateway": "^1.0.0",
    "@cloudflare/containers": "*"
  }
}
```

The app's own `wrangler.toml` only contains app-specific values — worker name, image names, secrets — no gateway logic.

## Publishing

Publish to npm (public or scoped to a GitHub Packages registry):

```bash
cd Code/AppBlueprint/Cloudflare-Workers
npm publish --access public
```

Or via a GitHub Actions workflow triggered on version tags, mirroring how NuGet packages are published in `publish-nuget-packages.yml`.

## Versioning

Follow semver. Breaking changes to the routing logic (e.g. adding a new container binding) are a minor bump. Changes to the `Env` type interface are a major bump if they require updates in consuming apps.

## Why not a shared wrangler.toml

Wrangler does not support config inheritance or `extends`. Each app must own a complete `wrangler.toml`. Only the TypeScript entry point (`main`) can be shared via npm — the config values (worker name, image names, secrets) are always app-specific.

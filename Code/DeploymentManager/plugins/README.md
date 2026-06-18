# plugins/

Drop folder for admin portal plugin DLLs (`SaaSFactory.<App>.Admin.dll`).

**This directory is intentionally empty in source control.** DLLs are populated at CI time by `Scripts/download-admin-plugins.ps1`, which pulls `.nupkg` files from GitHub Packages (Trubador org) and extracts the `.dll` into this folder before `railway up` uploads the build context.

The Dockerfile then bakes the contents into the image:
```
COPY Code/DeploymentManager/plugins/ /app/plugins/
```

DeploymentManager.Web scans `/app/plugins/` at startup via `AddAdminPortalPlugins()` and loads each module assembly.

For full setup instructions — including how to author a plugin, publish it, and wire it into the deploy workflow — see [ADMIN-PLUGIN-SETUP.md](../ADMIN-PLUGIN-SETUP.md).

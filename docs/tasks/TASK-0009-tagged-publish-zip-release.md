# Task: Tagged Publish Zip Release

ID: TASK-0009
Status: Done
Class: Major
Owner: Pair
Created: 2026-05-20
Updated: 2026-05-20
Branch: `task/TASK-0009-tagged-publish-zip-release`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `.github/workflows/`
- `scripts/`
- `README.md`
- `docs/tasks/TASK-0009-tagged-publish-zip-release.md`
Parallel safety: Coordinate

## Summary

Add a lightweight release packaging path for the WPF app using `dotnet publish` plus ZIP.

The package must be produced by GitHub only when a release tag is pushed. Normal branch pushes and ordinary builds must not publish release ZIPs.

Build was approved by the human, completed, and verified through a real GitHub tag release.

## Current Phase

Close

## Progress Checklist

- [x] Explore complete
- [x] Spec complete
- [x] Plan complete
- [x] Worktree created or reused, if required
- [x] Human approval received, if required
- [x] Build complete
- [x] Verification complete
- [x] Review complete
- [x] Documentation complete
- [x] Closeout complete

## Links

Related files:
- `../workflows/agentic-development.md`
- `../../src/Lightswitch.Wpf/Lightswitch.Wpf.csproj`
- `../../Lightswitch.sln`
- `../../README.md`

## Explore Notes

Current repository state:

- No `.github/workflows/` workflow files are present.
- Active desktop app project is `src/Lightswitch.Wpf/Lightswitch.Wpf.csproj`.
- The app targets Windows via WPF on .NET 9 in the current project layout.
- Existing verification commands are `dotnet build` plus Core and Device test projects.
- The desired package format is not MSI/MSIX/ClickOnce. It is a published app folder compressed as ZIP.

Release trigger requirement:

- The workflow must not publish ZIPs on every push or pull request.
- A pushed tag should be the release trigger.
- Recommended tag pattern: `v*`, for example `v0.1.0`.
- Human release command should be:

```powershell
git tag v0.1.0
git push origin v0.1.0
```

## Task Spec

In scope:

- Add a GitHub Actions release workflow that runs only for pushed tags matching `v*`.
- Publish the WPF app for Windows x64.
- Produce a ZIP containing the published app output.
- Upload the ZIP as a GitHub Actions artifact.
- Create or update a GitHub Release for the tag and attach the ZIP.
- Keep normal branch pushes from creating release ZIPs.
- Document how to create a release tag from PowerShell.
- Prefer first-party GitHub Actions and built-in tools:
  - `actions/checkout`;
  - `actions/setup-dotnet`;
  - `actions/upload-artifact`;
  - `gh release create` or equivalent GitHub-provided tooling for release creation.

Out of scope:

- No MSI installer.
- No MSIX packaging.
- No ClickOnce.
- No code signing.
- No automatic version bumping.
- No auto-update mechanism.
- No publishing for ARM64 or x86 unless later requested.
- No deployment on ordinary branch pushes.

Acceptance criteria:

- A tag like `v0.1.0` triggers the GitHub release workflow.
- A normal commit push to `main` does not trigger publish/ZIP/release.
- The workflow runs on `windows-latest`.
- The workflow builds/tests before packaging:
  - `dotnet build .\Lightswitch.sln -c Release`;
  - `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Release`;
  - `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Release`.
- The workflow publishes:

```powershell
dotnet publish .\src\Lightswitch.Wpf\Lightswitch.Wpf.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

- The ZIP has a predictable name, for example `Lightswitch-v0.1.0-win-x64.zip`.
- The ZIP contains `Lightswitch.Wpf.exe` and required runtime files.
- The tag's GitHub Release has the ZIP attached.
- README explains the release command and where to find the ZIP.

## Implementation Plan

Do not start this plan until the human explicitly approves Build.

1. Create or reuse a task worktree for `task/TASK-0009-tagged-publish-zip-release`.
2. Add a repo-local PowerShell packaging script, likely `scripts/publish-zip.ps1`, so local and CI packaging use the same commands.
   - Inputs:
     - configuration, default `Release`;
     - runtime, default `win-x64`;
     - version/tag name, optional;
     - output directory, default under `artifacts/`.
   - Behavior:
     - clean/create publish output folder;
     - run `dotnet publish`;
     - create ZIP with `Compress-Archive`;
     - emit the ZIP path.
3. Add `.github/workflows/release.yml`.
   - Trigger:

```yaml
on:
  push:
    tags:
      - 'v*'
```

   - Do not add regular `branches:` publish triggers.
   - Use `permissions: contents: write` so the workflow can create/upload the GitHub Release asset.
   - Use `windows-latest`.
   - Use PowerShell shell steps.
4. In the workflow:
   - check out repo;
   - install/setup the required .NET SDK version;
   - run Release build and tests;
   - run the packaging script with the tag name;
   - upload the ZIP as an Actions artifact;
   - create the GitHub Release and attach the ZIP using `gh release create`.
5. Make release creation idempotent enough for reruns:
   - if the release already exists, upload/replace the ZIP asset or document rerun behavior clearly.
6. Update `README.md` with:
   - local publish ZIP command;
   - tag release command;
   - expected GitHub Release result;
   - note that ordinary pushes do not publish release ZIPs.
7. Add or update `.gitignore` if local `artifacts/` output is not already ignored.
8. Verify locally:
   - run the packaging script;
   - inspect the ZIP contents for `Lightswitch.Wpf.exe`;
   - run build/tests in Release configuration.
9. Verify workflow definition without pushing a real release tag when possible:
   - inspect YAML syntax;
   - confirm trigger is tag-only;
   - optionally use `gh workflow view` after push if needed.
10. Human verification after merge/push:
    - create a tag, for example `v0.1.0`;
    - push it with `git push origin v0.1.0`;
    - confirm GitHub Actions creates a Release with the ZIP attached.
11. Update this task with Build Log, Verification Log, Review Notes, Documentation Notes, and Closeout.

## Build Log

Changes made:
- Added `scripts/publish-zip.ps1` for local and CI publish ZIP packaging.
- Added `.github/workflows/release.yml` with a tag-only `v*` trigger.
- Added `artifacts/` to `.gitignore`.
- Documented local packaging and tag-based GitHub release in `README.md`.

Deviation from plan:
- Reused the main checkout because this task record was already created there and there are no active parallel worktrees.

Files changed:
- `.github/workflows/release.yml`
- `.gitignore`
- `README.md`
- `scripts/publish-zip.ps1`
- `docs/tasks/TASK-0009-tagged-publish-zip-release.md`

## Verification Log

Passed:
- `dotnet build .\Lightswitch.sln -c Release`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Release --no-build`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Release --no-build`
- `.\scripts\publish-zip.ps1 -Configuration Release -Runtime win-x64 -Version local-test`
- ZIP inspection confirmed `artifacts\Lightswitch-local-test-win-x64.zip` exists and contains `Lightswitch.Wpf.exe`.

GitHub release verification:
- Commit `87ed6a8` was pushed to `origin/main`.
- Tag `v0.1.0` was pushed to GitHub.
- GitHub Actions run `26157074700` completed successfully.
- GitHub Release `v0.1.0` was created.
- Release asset `Lightswitch-v0.1.0-win-x64.zip` was uploaded.
- Asset size: 68,482,974 bytes.
- Asset digest: `sha256:606821e8fab600a3b769ada6bf463bd337371c63ee53641fb3ef3f8b2ca60f5b`.

## Review Notes

Diff matches goal:
- Yes. Release packaging is tag-triggered only with `on.push.tags: ['v*']`; no branch trigger is configured.

Scope respected:
- Yes. No installer, MSIX, MSI, ClickOnce, signing, app code, or auto-update behavior was added.

Risks remaining:
- GitHub Actions emitted notices that Node.js 20 actions are deprecated and that `windows-latest` will redirect to newer Windows runner images. Current release succeeded, but action versions may need future maintenance.
- The release workflow depends on GitHub-hosted `windows-latest` having the requested .NET SDK available through `actions/setup-dotnet`.

Security concerns:
- Uses default `github.token` with `contents: write` only for creating/updating the release asset.
- No secrets or signing keys are introduced.

## Documentation Notes

Updated:
- `README.md`
- `docs/tasks/TASK-0009-tagged-publish-zip-release.md`

Broader docs:
- No architecture decision needed. This adds a release packaging workflow, not a runtime architecture change.

## Closeout

Changed:
- Added local ZIP packaging through `scripts/publish-zip.ps1`.
- Added tag-triggered GitHub release workflow.
- Documented local packaging and release tagging in `README.md`.
- Ignored local `artifacts/` output.

Verified:
- Release build and tests passed locally.
- Local publish ZIP was created and inspected.
- Pushed tag `v0.1.0` triggered GitHub Actions successfully.
- GitHub Release `v0.1.0` contains `Lightswitch-v0.1.0-win-x64.zip`.

Known gaps:
- Package is unsigned.
- This is a ZIP distribution, not an installer.
- Future GitHub Actions maintenance may be needed for Node.js 24 action runtime changes.

Final status:
- Done.

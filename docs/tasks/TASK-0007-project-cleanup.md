# Task: Project Cleanup

ID: TASK-0007
Status: Done
Class: Major
Owner: Pair
Created: 2026-05-20
Updated: 2026-05-20
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `Lightswitch.sln`
- `src/`
- `tests/`
- `docs/architecture/`
- `docs/decisions/`
- `docs/tasks/TASK-0007-project-cleanup.md`
- `README.md`
- `AGENTS.md`
- `run-app.ps1`
Parallel safety: Exclusive

## Summary

Clean up the repository after the WPF rewrite trial.

The goal is to remove dead code, obsolete projects, and stale documentation so the repository clearly reflects the current WPF-based Lightswitch app. This task must not enter Build until the human explicitly approves implementation.

## Current Phase

Close

## Progress Checklist

- [x] Explore complete
- [x] Spec complete
- [x] Plan complete
- [ ] Worktree created or reused, if required
- [x] Human approval received, if required
- [x] Build complete
- [x] Verification complete
- [x] Review complete
- [x] Documentation complete
- [x] Closeout complete

## Links

Related files:
- `../workflows/agentic-development.md`
- `../../AGENTS.md`
- `../../README.md`
- `../../Lightswitch.sln`
- `../../run-app.ps1`
- `../architecture/system-overview.md`
- `../architecture/product-scope.md`
- `../decisions/0001-initial-architecture.md`
- `../../src/Lightswitch.App/`
- `../../src/Lightswitch.Wpf/`
- `../../src/Lightswitch.Core/`
- `../../src/Lightswitch.Device/`

Related tasks:
- `TASK-0004-wpf-rewrite-spike.md`
- `TASK-0005-wpf-tray-menu-settings.md`
- `TASK-0006-litra-brightness-temperature.md`

## Explore Notes

Repository state:

- `git status --short` was clean at task start.
- Current branch is `main`.
- `origin/main` points to the same pushed work as local `main`.
- No extra git worktrees exist.

Current active app path:

- `run-app.ps1` launches `src\Lightswitch.Wpf\Lightswitch.Wpf.csproj`.
- `README.md` says the active run path is the WPF rewrite trial.
- `AGENTS.md` still describes both:
  - WPF rewrite trial under `src/Lightswitch.Wpf`;
  - existing WinUI 3 app under `src/Lightswitch.App`.

Projects currently in the solution:

- `src/Lightswitch.Wpf` - active app surface.
- `src/Lightswitch.App` - old WinUI app surface.
- `src/Lightswitch.Core` - shared domain/contracts.
- `src/Lightswitch.Device` - HID communication.
- `tests/Lightswitch.Core.Tests`.
- `tests/Lightswitch.Device.Tests`.

WinUI-specific findings:

- `src/Lightswitch.App` is no longer launched by `run-app.ps1`.
- `src/Lightswitch.App` has WinUI/MSIX-specific dependencies:
  - `H.NotifyIcon.WinUI`;
  - `Microsoft.WindowsAppSDK`;
  - `Microsoft.Windows.SDK.BuildTools`;
  - `Microsoft.Windows.SDK.BuildTools.WinApp`.
- `src/Lightswitch.App` also contains packaging assets, app manifests, generated icon assets, and WinUI `.github/instructions`.
- `Lightswitch.sln` still includes `Lightswitch.App`.
- `README.md` still documents explicit WinUI build commands.
- `AGENTS.md` still documents the WinUI app and `H.NotifyIcon.WinUI`.
- `docs/architecture/system-overview.md`, `docs/architecture/product-scope.md`, and `docs/decisions/0001-initial-architecture.md` still describe WinUI / Windows App SDK as the architecture.
- `docs/architecture/design/Lightswitch/lightswitch-mockup.html` has developer notes that say WinUI flyout/custom chrome even though the active implementation is WPF.

Duplication findings:

- `src/Lightswitch.Wpf` and `src/Lightswitch.App` both contain app-layer versions of:
  - `JsonSettingsStore`;
  - `StartupRegistrationService`;
  - `SingleInstanceGuard`;
  - `RelayCommand`;
  - `MainViewModel`.
- This duplication was intentional during the WPF trial for reversibility.
- If `src/Lightswitch.App` is removed, most of this duplication disappears.
- Moving WPF app-layer infrastructure into a shared project is possible, but it may be a separate design decision because `StartupRegistrationService` is Windows-specific and not pure domain logic.

Design/reference artifact findings:

- `docs/architecture/design/Lightswitch/lightswitch-mockup.html` is still useful as the original switch visual reference.
- `docs/architecture/design/Skjermbilde 2026-05-15 135205.png` appears to be a screenshot used to diagnose an old edge-rendering issue. It is not referenced by docs or code.

Historical task document findings:

- `docs/tasks/TASK-0001` through `TASK-0006` are all `Done`.
- Historical task documents contain old WinUI references, but they are project memory. They should not be rewritten or deleted just because the current app changed.

## Task Spec

Clean up the repository to make WPF the clear current app surface and remove obsolete WinUI artifacts.

In scope:

- Remove obsolete WinUI app code and project artifacts if WPF is accepted as the current path:
  - remove `src/Lightswitch.App/`;
  - remove `Lightswitch.App` from `Lightswitch.sln`;
  - remove WinUI/MSIX-only dependencies from the active build graph.
- Keep current active projects:
  - `src/Lightswitch.Wpf`;
  - `src/Lightswitch.Core`;
  - `src/Lightswitch.Device`;
  - existing test projects.
- Update repository instructions and docs to reflect WPF as the current app stack:
  - `AGENTS.md`;
  - `README.md`;
  - `docs/architecture/system-overview.md`;
  - `docs/architecture/product-scope.md`;
  - design notes that incorrectly say WinUI where they mean the current app surface.
- Record the architecture change durably:
  - add a new decision record that supersedes or updates the initial WinUI architecture decision; or
  - mark `0001-initial-architecture.md` as superseded and add a new WPF architecture decision.
- Remove temporary or unreferenced design/debug artifacts when safe:
  - candidate: `docs/architecture/design/Skjermbilde 2026-05-15 135205.png`.
- Review WPF app-layer infrastructure after removing WinUI:
  - remove any remaining dead code;
  - defer deeper abstraction unless it clearly reduces active duplication.
- Update verification commands so they match the cleaned solution.

Out of scope:

- Do not redesign the settings window.
- Do not change HID protocol behavior.
- Do not change product behavior.
- Do not add installer/signing work.
- Do not delete historical completed task records.
- Do not archive completed tasks unless a separate task/backlog policy is approved.
- Do not begin code deletion or edits beyond this task document until the human explicitly says to build.

Acceptance criteria:

- The solution no longer includes obsolete projects.
- The active app path is unambiguous and WPF-based.
- `run-app.ps1` still runs the app from the repository root.
- README and architecture docs no longer present WinUI as the current stack.
- Durable decision documentation explains the move from WinUI to WPF.
- Removed files are limited to dead/obsolete artifacts identified in this task.
- Historical task documents remain available.
- Full verification passes after cleanup:
  - `dotnet build .\Lightswitch.sln -c Debug`;
  - `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`;
  - `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`;
  - WPF launch smoke test.

## Implementation Plan

Do not start this plan until the human explicitly approves Build.

1. Confirm WPF is accepted as the current app path for cleanup purposes.
2. Remove `Lightswitch.App` from `Lightswitch.sln`.
3. Delete `src/Lightswitch.App/` and all WinUI/MSIX-only assets under it.
4. Inspect the remaining solution for broken references to `Lightswitch.App`, WinUI, Windows App SDK, and `H.NotifyIcon.WinUI`.
5. Update `AGENTS.md`:
   - remove WinUI app references as current technology;
   - keep PowerShell environment guidance;
   - keep the file short and defer architecture details to docs.
6. Update `README.md`:
   - describe WPF as the current app;
   - remove WinUI build commands;
   - keep current run/build/test commands.
7. Update architecture docs:
   - `system-overview.md` should describe `Lightswitch.Wpf`, `Lightswitch.Core`, and `Lightswitch.Device`;
   - `product-scope.md` target platform should name WPF/.NET instead of Windows App SDK / WinUI;
   - design mockup developer notes should be generalized away from WinUI if still used.
8. Add a new decision record, likely `docs/decisions/0002-adopt-wpf-app-surface.md`, that supersedes the initial WinUI decision.
9. Decide during Build whether to remove `docs/architecture/design/Skjermbilde 2026-05-15 135205.png`:
   - remove it if it is only old bug evidence;
   - keep it only if the docs link to it as a useful visual regression reference.
10. Re-scan for stale references:
    - `rg "Lightswitch.App|WinUI|Windows App SDK|H.NotifyIcon|Microsoft.WindowsAppSDK|Package.appxmanifest|AppIcon.ico"`.
11. Run full verification:
    - `dotnet build .\Lightswitch.sln -c Debug`;
    - `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`;
    - `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`;
    - WPF launch smoke test.
12. Update this task record with Build Log, Verification Log, Review Notes, Documentation Notes, and Closeout.

## Build Log

Changed:
- Removed obsolete `Lightswitch.App` WinUI project from `Lightswitch.sln`.
- Deleted `src/Lightswitch.App/` including WinUI source, MSIX manifests, generated app assets, WinUI-specific GitHub instruction files, and WinUI tray dependencies.
- Removed stale WinUI process cleanup from `run-app.ps1`.
- Removed obsolete edge-debug screenshot `docs/architecture/design/Skjermbilde 2026-05-15 135205.png`.
- Updated `AGENTS.md` to list WPF as the app stack and remove WinUI/H.NotifyIcon guidance.
- Updated `README.md` to remove WPF-trial wording and WinUI build commands.
- Rewrote `docs/architecture/system-overview.md` for the WPF/Core/Device architecture.
- Updated `docs/architecture/product-scope.md` target platform from WinUI/Windows App SDK to WPF.
- Generalized design mockup implementation notes away from WinUI.
- Added `docs/decisions/0002-adopt-wpf-app-surface.md`.
- Marked `docs/decisions/0001-initial-architecture.md` as superseded for the UI stack.

Changed files:
- `AGENTS.md`
- `README.md`
- `Lightswitch.sln`
- `run-app.ps1`
- `docs/architecture/design/Lightswitch/lightswitch-mockup.html`
- `docs/architecture/product-scope.md`
- `docs/architecture/system-overview.md`
- `docs/decisions/0001-initial-architecture.md`
- `docs/decisions/0002-adopt-wpf-app-surface.md`
- `docs/tasks/TASK-0007-project-cleanup.md`
- deleted `docs/architecture/design/Skjermbilde 2026-05-15 135205.png`
- deleted `src/Lightswitch.App/`

## Verification Log

Passed:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- launch smoke test: started `Lightswitch.Wpf` with `dotnet run --project src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug --no-build`, confirmed the process started, then stopped it.

Failed:
- Initial parallel verification attempt hit `CS2012` file-lock errors in `src\Lightswitch.Core\obj` because build and tests compiled the same project at the same time.
  Follow-up: reran build and tests serially; all passed.

Scans:
- `rg` for active stale references shows no WinUI references in `README.md`, `AGENTS.md`, `Lightswitch.sln`, `run-app.ps1`, `src`, or current architecture docs.
- Remaining WinUI references are historical task records or superseded/superseding decision records.

## Review Notes

Diff matches goal:
- Yes. The obsolete WinUI app surface is removed, WPF is the only app project in the solution, and docs now present WPF as current.

Scope respected:
- Yes. No product behavior, HID protocol, or settings-window redesign changes were made.

Risks remaining:
- Historical task documents still contain WinUI references by design; they are retained as project memory.
- The WPF settings window remains basic and intentionally out of scope for this cleanup.
- Physical Litra Glow behavior is unchanged by this task and should continue to be verified during feature work.

Security concerns:
- No secrets or credential-bearing files were added.

Maintainability concerns:
- WPF app-layer infrastructure remains inside `Lightswitch.Wpf`. That is acceptable while there is only one app surface.

## Documentation Notes

Updated:
- `AGENTS.md`
- `README.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/product-scope.md`
- `docs/architecture/design/Lightswitch/lightswitch-mockup.html`
- `docs/decisions/0001-initial-architecture.md`
- `docs/decisions/0002-adopt-wpf-app-surface.md`
- `docs/tasks/TASK-0007-project-cleanup.md`

Decision record added:
- `docs/decisions/0002-adopt-wpf-app-surface.md`

## Closeout

Changed:
- Removed the obsolete WinUI app surface from the solution and repository.
- Made WPF the only active app surface.
- Updated repository docs and architecture decision records to reflect WPF as current.
- Removed an obsolete design/debug screenshot.

Verified:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- WPF launch smoke test.
- Human confirmed the result works as expected.

Known gaps:
- Historical task records intentionally retain old WinUI references.
- WPF settings window refinement remains future work.

Final status:
- Done.

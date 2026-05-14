# Task: Initial Project Scaffold

ID: TASK-0001
Status: Ready For Review
Class: Major
Owner: Agent
Created: 2026-05-14
Updated: 2026-05-14
Branch: Not available
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: Not available
Write scope:
- `src/`
- `tests/`
- `docs/architecture/`
- `docs/decisions/`
- `docs/tasks/`
- `README.md`
- repository root project files
Parallel safety: Exclusive

## Summary

Create the initial documentation and project scaffold for Lightswitch, a WinUI 3 tray utility for Logitech Litra Glow.

## Current Phase

Ready For Review

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
- [ ] Closeout complete

## Links

Related files:
- `../../AGENTS.md`
- `../workflows/agentic-development.md`
- `../architecture/product-scope.md`
- `../architecture/system-overview.md`
- `../decisions/0001-initial-architecture.md`

Related decisions:
- `../decisions/0001-initial-architecture.md`

## Explore Notes

- `AGENTS.md` requires agents to follow `docs/workflows/agentic-development.md`.
- Repository initially contained `AGENTS.md`, `docs/architecture/`, `docs/workflows/`, and `docs/workflows/agentic-development.md`.
- .NET SDK available: 9.0.117.
- Installed runtimes include .NET 8 and .NET 9 desktop runtimes.
- No WinUI 3 `dotnet new` template is installed initially.
- Git is installed at `C:\Program Files\Git\cmd\git.exe`, but `C:\Users\trutve\code\Lightswitch` is not currently a Git repository.
- Worktree/branch workflow could not be used because there is no `.git` repository yet.
- Context7 MCP was configured globally for Codex with `codex mcp add context7 -- npx -y @upstash/context7-mcp`.

## Task Spec

Create:

- product scope document for Lightswitch;
- system overview document for the intended architecture;
- decision record for initial architecture;
- initial solution and project scaffold;
- minimal WinUI 3 app structure;
- core models/contracts;
- device service boundary for future HidSharp-based Litra Glow communication;
- initial tests for non-UI logic if the test framework can be restored.

Acceptance criteria:

- repo contains durable product and architecture documentation;
- repo contains a .NET solution with app/core/device/test structure;
- app project references WinUI 3, H.NotifyIcon.WinUI, and the local core/device projects;
- device project references HidSharp;
- core logic is separated from UI and HID dependencies;
- relevant build/test commands are attempted and recorded.

## Implementation Plan

1. Add product, architecture, decision, and task documents.
2. Create solution and project directories.
3. Scaffold `Lightswitch.Core` with light state models, settings, contracts, and validation.
4. Scaffold `Lightswitch.Device` with `LitraService` boundary and placeholder HID discovery/control behavior.
5. Scaffold `Lightswitch.App` as a minimal WinUI 3 app with shell, view model, settings service, single-instance guard, and tray integration placeholders.
6. Add `Lightswitch.Core.Tests` for validation behavior.
7. Run restore/build/test where possible.
8. Update this Task Record with build, verification, review, documentation notes, and closeout.

## Human Approval

User approved the full proposed sequence with: "Ja, gjør alt det der".

## Build Log

Created:

- `Lightswitch.sln`
- `src/Lightswitch.App`
- `src/Lightswitch.Core`
- `src/Lightswitch.Device`
- `tests/Lightswitch.Core.Tests`

Added dependencies:

- `Microsoft.WindowsAppSDK`
- `Microsoft.Windows.SDK.BuildTools`
- `Microsoft.Windows.SDK.BuildTools.WinApp`
- `H.NotifyIcon.WinUI` pinned to `2.3.2` because newer stable `2.4.x` requires .NET 10.
- `HidSharp`

Implemented:

- Core light state models, limits, settings, and service contracts.
- HidSharp-based `LitraService` discovery boundary.
- WinUI app shell with Mica, power toggle, brightness slider, color temperature slider, and startup toggle.
- Tray icon with open, power, brightness presets, temperature presets, startup toggle, and exit actions.
- JSON settings store under local app data.
- Current-user Run key startup registration service.
- Single-instance mutex guard.
- Unit tests for light state normalization/defaults.

## Verification Log

Passed:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
- `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`

Notes:

- `dotnet build .\Lightswitch.sln -c Debug -p:Platform=x64` does not work because the solution file currently only defines `Any CPU` solution platforms.
- Real HID control was not verified because the Logitech Litra Glow product ID and HID report format are still unknown.
- The app was not manually launched in this agent session.

## Review Notes

Diff matches goal:
- Partially. The scaffold, architecture separation, tray entry points, settings, startup registration, and single-instance guard are in place. Real HID on/off, brightness, and color temperature reports remain pending protocol verification.

Scope respected:
- Yes.

Risks remaining:
- Litra Glow HID protocol is not implemented yet.
- Device detection currently filters Logitech HID devices by vendor ID and product/friendly name containing `Litra`; this may need exact product IDs.
- Startup registration uses the current user Run key; this may need revision if packaged startup tasks are preferred later.
- App runtime/manual UI behavior still needs local manual verification.

Security concerns:
- No secrets or network calls are introduced.
- Startup registration writes only to `HKCU`.

Maintainability concerns:
- View model currently uses simple fire-and-forget apply calls; later device work should add throttling/debouncing for slider updates.

## Documentation Notes

Docs updated:
- `docs/architecture/product-scope.md`
- `docs/architecture/system-overview.md`
- `docs/decisions/0001-initial-architecture.md`
- `docs/tasks/TASK-0001-initial-project-scaffold.md`
- `README.md`

Decision record needed:
- Done: `docs/decisions/0001-initial-architecture.md`

## Closeout

Ready for human review.

How to verify:

1. Open PowerShell in `C:\Users\trutve\code\Lightswitch`.
2. Run `dotnet build .\Lightswitch.sln -c Debug`.
3. Run `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`.
4. For x64 app build, run:
   - `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
   - `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`
5. To manually inspect the app, run from `src\Lightswitch.App` with the matching platform after enabling Windows Developer Mode if required:
   - `dotnet run -c Debug -p:Platform=x64`

Expected result:

- The app launches as Lightswitch.
- The main window shows power, brightness, color temperature, and start-with-Windows controls.
- The tray icon exists with open, power, brightness presets, color temperature presets, startup toggle, and exit.
- Without verified Litra Glow HID protocol, real light control is not expected to work yet.

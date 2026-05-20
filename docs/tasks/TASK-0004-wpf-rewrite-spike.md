# Task: WPF Rewrite Trial

ID: TASK-0004
Status: Done
Class: Major
Owner: Pair
Created: 2026-05-18
Updated: 2026-05-19
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `docs/tasks/TASK-0004-wpf-rewrite-spike.md`
- WPF app project and related UI files
- project/solution/run-script changes needed to launch the WPF version
Parallel safety: Coordinate

## Summary

Rewrite the app surface from WinUI 3 to WPF as a practical trial.

The goal is simple: implement the current Lightswitch functionality in WPF and let the human test whether it works better in practice. If the WPF version works, it can become the path forward. If it does not work, the change can be rolled back.

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
- `../../AGENTS.md`
- `../workflows/agentic-development.md`
- `../architecture/system-overview.md`
- `../architecture/product-scope.md`
- `../../src/Lightswitch.App/`
- `../../src/Lightswitch.Core/`
- `../../src/Lightswitch.Device/`

Related tasks:
- `TASK-0001-initial-project-scaffold.md`
- `TASK-0002-litra-power-on.md`
- `TASK-0003-switch-style-ui.md`

## Explore Notes

- The current app is implemented as a WinUI 3 / Windows App SDK desktop app in `src/Lightswitch.App`.
- The current tray integration uses `H.NotifyIcon.WinUI`.
- The current custom switch popup has required several low-level fixes around popup sizing, native drag behavior, borderless window behavior, and edge rendering.
- `dotnet run` for the packaged WinUI profile can require Developer Mode, while unpackaged launch avoids that during development.
- Core device behavior is already separated into `Lightswitch.Core` and `Lightswitch.Device`, so the WPF app should reuse existing HID and domain logic instead of rewriting the device layer.

## Task Spec

Build a WPF version of Lightswitch with the same user-facing functionality that exists today.

In scope:

- Add a WPF app surface that can be run from the repository root.
- Preserve all current functionality:
  - tray-first app lifecycle;
  - tray icon with white `L` on black background;
  - left-click tray action opens the chromeless switch window;
  - right-click tray menu exposes quick actions;
  - power on/off;
  - brightness presets and settings UI;
  - color temperature presets and settings UI;
  - persisted settings;
  - start with Windows option;
  - single-instance guard;
  - direct Logitech Litra Glow HID control through the existing device service;
  - disconnect/reconnect handling already provided by the service layer.
- Preserve the current switch visual design:
  - 160 x 200 chromeless window;
  - wall background;
  - centered switch plate;
  - screws;
  - toggle track and arm;
  - click on the switch toggles power;
  - window can be moved without making the toggle unusable.
- Reuse `Lightswitch.Core` and `Lightswitch.Device` where practical.
- Prefer built-in Windows/WPF functionality over a new tray dependency unless a dependency is clearly needed.
- Keep the rewrite reversible.

Out of scope:

- Do not change HID protocol behavior.
- Do not redesign the product.
- Do not add unrelated features.
- Do not delete the current WinUI app unless the WPF trial is accepted later.
- Do not solve installer/signing distribution beyond making local development/running practical.

Rollback rule:

- If the WPF version does not work well enough after human testing, revert this task's WPF changes and continue from the existing WinUI implementation.

Acceptance criteria:

- The WPF app builds.
- The WPF app can be started from `C:\Users\trutve\code\Lightswitch`.
- The app runs as a tray app.
- The tray icon is a white `L` on a black background.
- Left-click on the tray icon opens the switch-style chromeless window.
- The switch window matches the current dimensions and visual structure closely.
- The switch window can be moved.
- Clicking the switch toggles the light power without requiring Logitech G Hub.
- Right-click tray menu includes the current quick actions.
- The settings UI supports power, brightness, color temperature, and start with Windows.
- Settings are remembered between restarts.
- Only one app instance can run.
- The code reuses existing core/device services instead of duplicating HID logic.

## Implementation Plan

1. Keep the current WinUI project in place for reversibility.
2. Add a WPF app project that references `Lightswitch.Core` and `Lightswitch.Device`.
3. Move or duplicate only app-layer infrastructure needed by WPF:
   - JSON settings store;
   - startup registration;
   - single-instance guard;
   - relay command/view model glue if needed.
4. Implement WPF application startup and dependency wiring.
5. Implement tray lifecycle with `System.Windows.Forms.NotifyIcon`.
6. Generate the tray icon dynamically as a white `L` on black background.
7. Implement the chromeless 160 x 200 switch window in WPF.
8. Implement native-feeling move behavior for the switch window while keeping switch clicks scoped to the switch component.
9. Implement the settings window with existing controls and bindings.
10. Update `run-app.ps1` to run the WPF app from the repository root.
11. Build and run relevant tests.
12. Update this task with build, verification, review, and closeout notes.

## Build Log

Changed:
- Added `src/Lightswitch.Wpf` as a WPF rewrite trial app.
- Reused `Lightswitch.Core` and `Lightswitch.Device`.
- Added WPF app-layer infrastructure for settings persistence, startup registration, single-instance guard, relay commands, and view model wiring.
- Implemented tray lifecycle with `System.Windows.Forms.NotifyIcon`.
- Implemented dynamic tray icon: white `L` on black background.
- Implemented WPF chromeless 160 x 200 switch window with wall, plate, screws, track, arm, toggle animation, and drag behavior outside the switch plate.
- Implemented WPF settings window with power, brightness, color temperature, start with Windows, and status text.
- Fixed review findings:
  - startup initialization errors are reported as status instead of terminating the tray app;
  - the switch popup no longer stays permanently topmost and hides when deactivated;
  - first popup placement creates a window handle before converting physical cursor pixels to WPF DIPs.
- Updated `run-app.ps1` to launch the WPF app from the repository root.
- Added the WPF project to `Lightswitch.sln`.
- Updated `README.md` and `AGENTS.md` to reflect the WPF rewrite trial.

Changed files:
- `AGENTS.md`
- `README.md`
- `Lightswitch.sln`
- `run-app.ps1`
- `src/Lightswitch.Wpf/`
- `docs/tasks/TASK-0004-wpf-rewrite-spike.md`

## Verification Log

Passed:
- `dotnet build .\src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- `dotnet build .\Lightswitch.sln -c Debug`
- launch smoke test: started `Lightswitch.Wpf` with `dotnet run --project src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug --no-build`, confirmed the process started, then stopped it.
- repeated after review fixes: `dotnet build .\Lightswitch.sln -c Debug`
- repeated after review fixes: `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- repeated after review fixes: `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- repeated launch smoke test after review fixes.

Not run:
- Full manual tray UI and device control test.
  Reason: requires human desktop interaction and connected Logitech Litra Glow verification.

## Review Notes

Diff matches goal:
- Yes. The WPF app is implemented beside the WinUI app and `run-app.ps1` now starts the WPF version.

Scope respected:
- Yes. HID protocol and core/device behavior were not changed.

Risks remaining:
- Human testing still needs to confirm tray left-click behavior, right-click menu behavior, drag behavior, switch rendering, real device control, and startup-with-Windows behavior.
- The WPF app duplicates app-layer infrastructure from the WinUI app for reversibility. If WPF is accepted, shared app infrastructure should be consolidated in a follow-up task.

Review findings resolved:
- Startup initialization exceptions no longer escape `async void OnStartup`.
- The switch popup no longer uses permanent topmost behavior.
- First popup placement is DPI-aware before first show.

## Documentation Notes

Updated:
- `README.md`
- `AGENTS.md`
- `docs/tasks/TASK-0004-wpf-rewrite-spike.md`

Decision record needed:
- Not yet. Wait until human testing decides whether WPF is accepted or rolled back.

## Closeout

Changed:
- Added the WPF rewrite trial app while keeping the existing WinUI app for rollback.
- Switched `run-app.ps1` to launch the WPF app.
- Added WPF tray icon, tray menu, switch popup, basic settings window, startup handling, single-instance handling, and shared core/device wiring.
- Fixed review findings around startup exceptions, popup topmost behavior, and DPI-aware first placement.

Verified:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- WPF launch smoke test.

Known gaps:
- WPF app-layer infrastructure is duplicated from WinUI for reversibility. Consolidate later only if WPF is accepted as the permanent path.

Final status:
- Done.

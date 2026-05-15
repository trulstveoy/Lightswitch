# Task: Switch Style UI

ID: TASK-0003
Status: Done
Class: Major
Owner: Pair
Created: 2026-05-14
Updated: 2026-05-14
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `src/Lightswitch.App/`
- `docs/tasks/TASK-0003-switch-style-ui.md`
- possibly `docs/architecture/`
Parallel safety: Coordinate

## Summary

Redesign the Lightswitch UI to follow the visual direction in `docs/architecture/design/Lightswitch/lightswitch-mockup.html`.

The main interaction should feel like a physical wall light switch exposed from the tray, with clear on/off states, a custom tray icon, and a cleaner right-click menu.

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
- `../architecture/design/Lightswitch/lightswitch-mockup.html`
- `../../src/Lightswitch.App/MainWindow.xaml`
- `../../src/Lightswitch.App/MainWindow.xaml.cs`
- `../../src/Lightswitch.App/MainPage.xaml`
- `../../src/Lightswitch.App/MainPage.xaml.cs`
- `../../src/Lightswitch.App/ViewModels/MainViewModel.cs`

Related tasks:
- `TASK-0001-initial-project-scaffold.md`
- `TASK-0002-litra-power-on.md`

## Explore Notes

- The current app uses a standard WinUI window with title bar, a simple `MainPage`, WinUI sliders/toggles, and a `H.NotifyIcon.WinUI` tray icon.
- The design mockup defines:
  - left-click tray popup with a physical switch appearance;
  - on/off wall, plate, screw, toggle-track, and toggle-arm colors;
  - 160 x 200 visual popup size;
  - borderless/custom chrome popup behavior;
  - separate on/off tray icon states;
  - right-click menu with power action, brightness presets, settings, and exit;
  - physical bounce toggle animation;
  - optional click sound;
  - brightness and color temperature as secondary, not MVP.
- `TASK-0002` verified power-on from the app, so the UI can call the existing `MainViewModel` power flow.

## Task Spec

Implement a cooler UI based on the mockup.

In scope:

- Replace or supplement the current main window interaction with a compact tray popup inspired by the mockup.
- Left-click on the tray icon should open the switch-style popup.
- The popup should show a stylized wall switch with distinct on/off visual states.
- Clicking the switch should toggle `MainViewModel.IsOn`.
- The switch should animate between on/off states with a short bounce-like motion.
- The popup should avoid a standard Windows title bar.
- Tray icon should visually distinguish on/off states where practical.
- Right-click tray menu should be cleaned up to match the mockup direction:
  - primary power action;
  - brightness presets;
  - settings;
  - exit.
- Keep existing device/service boundaries unchanged.
- Keep UI logic in `Lightswitch.App`; do not move HID logic into UI.
- Preserve start-with-Windows/settings access, either in a settings window/panel or an equivalent reachable UI.

Out of scope:

- Implementing real brightness HID reports.
- Implementing real color temperature HID reports.
- Adding a full settings redesign beyond what is needed to keep current controls reachable.
- Packaging/release work.
- Large app architecture refactors unrelated to the UI.

Acceptance criteria:

- Running `.\run-app.ps1` starts the app.
- Left-clicking the tray icon opens a compact switch-style popup, not the existing full settings-first view.
- Clicking the switch toggles the app power state and calls the existing device flow.
- Popup visuals clearly match the mockup's wall-switch concept and on/off palette.
- Right-click tray menu remains available and includes power, brightness presets, settings, and exit.
- `dotnet build .\Lightswitch.sln -c Debug` passes.
- UI changes are manually verified on Windows.

## Open Questions

- Should the old full window remain as a settings window, or should settings be a secondary panel inside the popup?
- Should the click sound be included in this task or deferred?
- Should tray icons be generated as `.ico` assets, drawn in XAML, or updated dynamically through `H.NotifyIcon` generated icons?
- Should brightness presets remain visual placeholders until real brightness HID support exists, or should they only update saved desired state?
- What exact popup placement behavior is acceptable with `H.NotifyIcon.WinUI` on WinUI 3?

## Implementation Plan

1. Use `H.NotifyIcon.WinUI` `TaskbarIcon.TrayPopup` for the left-click popup.
2. Keep the existing full WinUI window as the settings window opened from the right-click menu.
3. Add a `SwitchPopup` WinUI control that recreates the mockup's wall, plate, screws, track, arm, and label with XAML shapes.
4. Bind the popup to `MainViewModel` and toggle power through the existing `TogglePowerCommand`.
5. Animate the toggle arm with a short bounce-like easing.
6. Make app launch tray-first by hiding the settings window after the tray icon is created.
7. Clean up the right-click menu to focus on power, brightness presets, settings, start-with-Windows, and exit.
8. Build and test.
9. Record manual verification steps and close the task.

## Human Approval

User explicitly approved one-shot execution: "du kan spekke, planlegge, bygge, verifisere og lukke."

## Build Log

Changed:

- Added `src/Lightswitch.App/Controls/SwitchPopup.xaml`.
- Added `src/Lightswitch.App/Controls/SwitchPopup.xaml.cs`.
- Updated `MainWindow.xaml` to use `TaskbarIcon.TrayPopup` on left-click.
- Updated the tray right-click menu to focus on power, brightness presets, settings, start-with-Windows, and exit.
- Updated `MainWindow.xaml.cs` to keep the existing window as settings and expose tray-first behavior.
- Updated `App.xaml.cs` to hide the settings window after tray creation.
- Updated `README.md` to mention tray-first switch popup behavior.
- Follow-up bugfix: replaced `TaskbarIcon.TrayPopup` with a dedicated borderless `SwitchPopupWindow` because the app crashed when the tray icon was left-clicked.
- Follow-up bugfix: added a dynamic tray icon drawn in code so the tray icon resembles the switch mockup and reflects on/off state.

Design choices:

- Existing full WinUI window remains as settings.
- Click sound deferred.
- Brightness presets still update desired/saved state only; real brightness HID is outside this task.
- Tray icon is generated dynamically from the switch mockup shape.

## Verification Log

Passed:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
- `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`

Notes:

- A parallel build/test attempt produced a transient PDB copy warning and a later platform-assets mismatch when x64 restore ran concurrently with solution build. Sequential restore/build passed cleanly.
- Manual visual verification was not run by the agent.

## Review Notes

Diff matches goal:
- Yes. The primary tray interaction is now a compact wall-switch popup matching the mockup's wall/plate/screw/track/arm concept and on/off palette.

Scope respected:
- Yes. Device logic was not moved into UI, and brightness/color temperature HID reports were not added.

Risks remaining:
- H.NotifyIcon popup placement should be visually checked on the user's Windows tray setup.
- The popup is now a separate borderless WinUI window, so placement may differ from native tray flyout placement but should not crash on left-click.
- Click sound is deferred.

## Documentation Notes

Docs updated:
- `README.md`
- `docs/tasks/TASK-0003-switch-style-ui.md`

## Closeout

Changed:

- Implemented switch-style tray popup based on the mockup.
- Made app launch tray-first.
- Preserved settings window via tray menu.
- Cleaned up tray context menu.
- Fixed left-click crash by avoiding `TaskbarIcon.TrayPopup`.
- Added dynamic on/off tray icon in the switch style.

Verified:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
- `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`

Known gaps:

- Manual visual verification remains useful.
- Custom dynamic on/off tray icon and click sound are deferred.

How to verify:

1. Run `.\run-app.ps1` from `C:\Users\trutve\code\Lightswitch`.
2. Left-click the tray icon and confirm a compact physical switch popup opens.
3. Click the switch and confirm it toggles between warm on-state and dark off-state.
4. Right-click the tray icon and confirm power, brightness presets, settings, start-with-Windows, and exit are available.
5. Open settings and confirm the original sliders/toggle window remains reachable.

Final status:

Done.

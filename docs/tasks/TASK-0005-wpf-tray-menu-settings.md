# Task: WPF Tray Menu And Basic Settings Window

ID: TASK-0005
Status: Done
Class: Standard
Owner: Agent
Created: 2026-05-20
Updated: 2026-05-20
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `src/Lightswitch.Wpf/`
- `docs/tasks/TASK-0005-wpf-tray-menu-settings.md`
Parallel safety: Coordinate

## Summary

Fix the WPF tray icon right-click context menu so each menu action works, and ensure `Innstillinger` opens a basic settings window.

The settings window should remain intentionally simple because it will be refined in a later task.

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
- `TASK-0004-wpf-rewrite-spike.md`
- `../../src/Lightswitch.Wpf/TrayAppController.cs`
- `../../src/Lightswitch.Wpf/MainWindow.xaml`
- `../../src/Lightswitch.Wpf/MainWindow.xaml.cs`
- `../../src/Lightswitch.Wpf/ViewModels/MainViewModel.cs`

## Explore Notes

- The active WPF trial uses `System.Windows.Forms.NotifyIcon` with a `ContextMenuStrip`.
- The context menu items currently call view model commands directly from WinForms event handlers.
- WPF UI state and windows should be accessed through the WPF dispatcher to avoid non-working or unstable menu behavior.
- A basic settings window already exists as `MainWindow`, but it should be treated as the WPF settings window and opened reliably from the tray menu.

## Task Spec

In scope:

- Make right-click tray menu actions execute reliably:
  - power toggle;
  - brightness presets;
  - color temperature presets;
  - start with Windows;
  - Innstillinger;
  - Avslutt.
- Ensure `Innstillinger` opens a WPF settings window.
- Keep the settings window basic:
  - status;
  - power;
  - brightness;
  - color temperature;
  - start with Windows.
- Do not refine the settings UI beyond a functional baseline.

Out of scope:

- No visual redesign of the settings window.
- No installer/signing work.
- No HID protocol changes.
- No WinUI app changes unless required for shared correctness.

Acceptance criteria:

- WPF app builds.
- Right-click tray menu items marshal WPF work onto the WPF dispatcher.
- `Innstillinger` opens a settings window without creating duplicate visible settings windows.
- Existing WPF switch popup behavior remains intact.

## Implementation Plan

1. Update tray menu callbacks to run WPF-facing work through the WPF dispatcher.
2. Make settings-window creation/show/activate robust and explicit.
3. Keep settings UI basic and avoid design-heavy changes.
4. Build the solution and run relevant tests.
5. Update task verification and review notes.

## Build Log

Changed:
- Updated `TrayAppController` so tray menu actions that touch WPF state or windows run through the WPF dispatcher.
- Kept one settings window instance and made `Innstillinger` show/restore/activate it reliably.
- Renamed the basic settings window title/header to `Innstillinger`.

Changed files:
- `src/Lightswitch.Wpf/TrayAppController.cs`
- `src/Lightswitch.Wpf/MainWindow.xaml`
- `docs/tasks/TASK-0005-wpf-tray-menu-settings.md`

## Verification Log

Passed:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- launch smoke test: started `Lightswitch.Wpf` with `dotnet run --project src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug --no-build`, confirmed the process started, then stopped it.

Not run:
- Manual right-click tray menu test.
  Reason: requires human desktop interaction with the tray icon.

## Review Notes

Diff matches goal:
- Yes. Context menu callbacks now marshal WPF work to the WPF dispatcher, and settings opens a basic WPF window.

Scope respected:
- Yes. No HID protocol, installer, signing, or WinUI behavior changes.

Risks remaining:
- Human testing must confirm each tray menu item works from the actual notification area.

## Documentation Notes

Task record updated. No broader docs needed until the WPF trial is accepted.

## Closeout

Changed:
- Made WPF tray menu actions marshal WPF state/window work through the WPF dispatcher.
- Made `Innstillinger` open, restore, and activate one basic settings window.
- Kept the settings window intentionally simple for later refinement.

Verified:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- WPF launch smoke test.

Known gaps:
- Settings window visual refinement is intentionally deferred.

Final status:
- Done.

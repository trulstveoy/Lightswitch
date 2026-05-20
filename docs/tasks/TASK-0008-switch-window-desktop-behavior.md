# Task: Switch Window Desktop Behavior

ID: TASK-0008
Status: Ready For Review
Class: Standard
Owner: Pair
Created: 2026-05-20
Updated: 2026-05-20
Branch: `task/TASK-0008-switch-window-desktop-behavior`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `src/Lightswitch.Wpf/SwitchWindow.xaml`
- `src/Lightswitch.Wpf/SwitchWindow.xaml.cs`
- `src/Lightswitch.Wpf/TrayAppController.cs`
- `src/Lightswitch.Wpf/MainWindow.xaml`
- `src/Lightswitch.Wpf/ViewModels/MainViewModel.cs`
- `docs/tasks/TASK-0008-switch-window-desktop-behavior.md`
Parallel safety: Coordinate

## Summary

Improve the WPF switch window behavior so it acts like a small desktop utility window instead of a transient popup.

The switch window should remain visible after focus moves elsewhere, sit behind the active window when another app is used, and expose equivalent context-menu actions on right-click inside the switch window.

Build was approved by the human and completed. The task is ready for human GUI verification.

## Current Phase

Ready For Review

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
- [ ] Closeout complete

## Links

Related files:
- `../workflows/agentic-development.md`
- `../../src/Lightswitch.Wpf/SwitchWindow.xaml`
- `../../src/Lightswitch.Wpf/SwitchWindow.xaml.cs`
- `../../src/Lightswitch.Wpf/TrayAppController.cs`

Related tasks:
- `TASK-0003-switch-style-ui.md`
- `TASK-0005-wpf-tray-menu-settings.md`

## Explore Notes

Current behavior:

- `SwitchWindow` subscribes to `Deactivated` and calls `Hide()`.
- `SwitchWindow.ShowPopup()` calls `Show()` and `Activate()`.
- The switch window is chromeless, fixed size, hidden from the taskbar, and manually draggable.
- Left-click outside the switch plate starts `DragMove()`.
- Left-click on the switch plate toggles power.
- `TrayAppController` owns a WinForms `ContextMenuStrip` and assigns it to `NotifyIcon.ContextMenuStrip`.
- `System.Windows.Forms.NotifyIcon` is still appropriate for the tray icon because WPF has no native official `NotifyIcon` control.
- The context menu is not exposed by `SwitchWindow`.

Implications:

- The current `Deactivated` handler directly causes the window to disappear when clicking elsewhere.
- To keep the window on the desktop, the window should not hide on deactivation.
- To make it sit behind the active window, the implementation should avoid `Topmost` behavior and should not reactivate itself after focus moves away.
- Right-click inside WPF should use a native WPF `ContextMenu`, not reuse the WinForms `ContextMenuStrip`.
- The tray menu and WPF window menu should share actions through `MainViewModel` commands and small controller callbacks, not through the same UI component.

## Task Spec

In scope:

- Keep the switch window visible when the user clicks another app, desktop, or window.
- Ensure the switch window does not stay above other active windows after focus leaves it.
- Preserve existing tray left-click behavior that shows the switch window.
- Preserve existing switch behavior:
  - left-click on the switch plate toggles power;
  - left-click outside the plate can drag the window;
  - app remains hidden from the taskbar.
- Show a native WPF context menu when the user right-clicks the switch window itself.
- Keep the existing WinForms `ContextMenuStrip` for the tray icon.
- Reuse the existing menu actions semantically:
  - power toggle;
  - brightness presets;
  - color temperature presets;
  - start with Windows;
  - Innstillinger;
  - Avslutt.

Out of scope:

- No visual redesign of the switch.
- No settings-window refinement.
- No tray-icon redesign.
- No HID protocol changes.
- No installer/signing work.
- No new dependency.
- Do not show the WinForms `ContextMenuStrip` from the WPF switch window.

Acceptance criteria:

- Clicking away from the switch window leaves it visible.
- Opening or focusing another window places that active window above the switch window.
- Left-clicking the tray icon still shows and foregrounds the switch window.
- Right-clicking the switch window opens a WPF-native context menu with equivalent actions to the tray menu.
- Context menu actions produce the same behavior whether opened from tray or switch window.
- Right-clicking the switch window does not toggle the switch.
- Left-click toggle and drag behavior remain intact.
- Relevant automated checks pass:
  - `dotnet build .\Lightswitch.sln -c Debug`;
  - `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`;
  - `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`.
- Manual GUI verification is documented because window z-order and tray/context-menu behavior require desktop interaction.

## Implementation Plan

Do not start this plan until the human explicitly approves Build.

1. Create or reuse a task worktree for `task/TASK-0008-switch-window-desktop-behavior`.
2. Update `SwitchWindow` lifetime behavior:
   - remove the `Deactivated += OnDeactivated` subscription;
   - remove the `OnDeactivated` hide behavior;
   - keep `OnClosing` hide-on-close behavior for normal window close;
   - keep `CloseForExit()` as the only true close path.
3. Review `ShowPopup()`:
   - keep `Show()` and `Activate()` so tray left-click can bring the switch back when requested;
   - do not add `Topmost`;
   - avoid any deactivation logic that re-hides or re-raises the window.
4. Keep tray-menu ownership in `TrayAppController`:
   - continue using `System.Windows.Forms.NotifyIcon`;
   - continue using WinForms `ContextMenuStrip` only for the tray icon.
5. Add a WPF-native context menu to `SwitchWindow`:
   - define menu items in `SwitchWindow.xaml` or construct them in WPF code-behind if command parameter binding is clearer;
   - bind power, brightness, and color-temperature items to `MainViewModel` commands;
   - bind or handle `Start with Windows` through `MainViewModel.StartWithWindows`;
   - expose small WPF callbacks for `Innstillinger` and `Avslutt` because those are controller/window actions rather than pure device settings.
6. Update `TrayAppController` construction of `SwitchWindow`:
   - pass only the minimal callbacks needed by the WPF menu, such as show-settings and exit;
   - do not pass or expose the WinForms `ContextMenuStrip` to `SwitchWindow`.
7. Ensure WPF context-menu state is current:
   - `Start with Windows` should reflect current checked state when the menu opens;
   - menu commands should remain enabled consistently with the existing settings UI behavior.
8. Check that right-click does not interfere with:
   - left-click drag;
   - left-click plate toggle;
   - tray icon right-click menu.
9. Run verification:
   - `dotnet build .\Lightswitch.sln -c Debug`;
   - `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`;
   - `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`;
   - WPF launch smoke test.
10. Perform or document manual GUI verification:
   - start app from `C:\Users\trutve\code\Lightswitch`;
   - left-click tray icon to show switch;
   - click another window and confirm switch remains visible but is behind the active window;
   - right-click switch and confirm the WPF context menu appears;
   - try a safe menu action such as `Innstillinger`;
   - confirm tray right-click menu still works.
11. Update this task with Build Log, Verification Log, Review Notes, Documentation Notes, and Closeout.

## Build Log

Changes made:
- Removed `SwitchWindow` hide-on-deactivate behavior so the switch remains visible after focus moves away.
- Added a WPF-native context menu to `SwitchWindow`.
- Kept WinForms `ContextMenuStrip` scoped to the tray icon.
- Passed settings and exit callbacks from `TrayAppController` into `SwitchWindow`.

Deviation from plan:
- Reused the existing main checkout instead of creating a separate worktree because the task record was already created there and no parallel worktrees were active.

Changed files:
- `src/Lightswitch.Wpf/SwitchWindow.xaml`
- `src/Lightswitch.Wpf/SwitchWindow.xaml.cs`
- `src/Lightswitch.Wpf/TrayAppController.cs`
- `docs/tasks/TASK-0008-switch-window-desktop-behavior.md`

## Verification Log

Passed:
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- launch smoke test: started `Lightswitch.Wpf` with `dotnet run --project src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug --no-build`, confirmed the process started, then stopped it.

Not run:
- Manual GUI verification.
  Reason: requires human desktop interaction with the tray icon, switch window, another foreground window, and the context menus.

## Review Notes

Diff matches goal:
- Yes. The switch window no longer hides on deactivation, and right-click on the switch uses a WPF-native `ContextMenu`.

Scope respected:
- Yes. Tray `NotifyIcon` and its WinForms `ContextMenuStrip` remain unchanged except for passing callbacks to the switch window. No HID, settings redesign, tray icon redesign, installer, or dependency changes were made.

Risks remaining:
- Window z-order and context-menu behavior still need manual verification on the desktop.
- The WPF switch context menu duplicates menu labels/actions from the tray menu. That is intentional here to keep WPF UI native while leaving `NotifyIcon` interop contained.

Security concerns:
- No new external inputs, filesystem access, shell execution, secrets, or dependencies.

## Documentation Notes

Updated:
- `docs/tasks/TASK-0008-switch-window-desktop-behavior.md`

Broader docs:
- Not updated. This is a small behavior refinement and does not change setup, architecture, or developer commands.

## Closeout

Not closed.

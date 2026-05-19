# Task: WPF Rewrite Spike

ID: TASK-0004
Status: Backlog
Class: Major
Owner: Pair
Created: 2026-05-18
Updated: 2026-05-18
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `docs/tasks/TASK-0004-wpf-rewrite-spike.md`
- potential spike code under `spikes/` or a temporary WPF project, if approved
- no production rewrite until the spike has a documented recommendation
Parallel safety: Coordinate

## Summary

Investigate whether Lightswitch should be rewritten from WinUI 3 to WPF.

The motivation is uncertainty around the current WinUI 3 / Windows App SDK / `H.NotifyIcon.WinUI` stack for this app's needs: a lightweight tray utility with a small custom popup, stable window behavior, direct HID control, startup integration, and minimal packaging friction.

## Current Phase

Backlog

## Progress Checklist

- [ ] Explore complete
- [ ] Spec complete
- [ ] Plan complete
- [ ] Worktree created or reused, if required
- [ ] Human approval received, if required
- [ ] Build complete
- [ ] Verification complete
- [ ] Review complete
- [ ] Documentation complete
- [ ] Closeout complete

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

- The current app is implemented as a WinUI 3 / Windows App SDK desktop app.
- The current tray integration uses `H.NotifyIcon.WinUI`.
- The current custom switch popup has required several low-level fixes around popup sizing, native drag behavior, borderless window behavior, and edge rendering.
- `dotnet run` for the packaged WinUI profile can require Developer Mode, while unpackaged launch avoids that during development.
- Core device behavior is already separated into `Lightswitch.Core` and `Lightswitch.Device`, so a WPF spike should be able to reuse the existing HID and settings logic instead of rewriting the device layer.

## Task Spec

Run a spike to determine whether WPF is a better fit for Lightswitch than WinUI 3.

In scope:

- Compare WPF and WinUI 3 specifically for this app, not as a general framework comparison.
- Identify a WPF tray-icon approach and assess whether it is simpler or more reliable than the current WinUI tray setup.
- Assess popup behavior for:
  - borderless 160 x 200 switch popup;
  - native-feeling drag/move behavior;
  - click-only toggle on the switch component;
  - stable visual rendering on dark desktop backgrounds.
- Assess packaging and installation options that avoid Developer Mode for normal use.
- Confirm whether existing `Lightswitch.Core` and `Lightswitch.Device` can be reused unchanged.
- Optionally create a small throwaway WPF prototype if documentation/code inspection is not enough.
- Produce a recommendation: keep WinUI 3, rewrite to WPF, or defer the decision.

Out of scope:

- Do not rewrite the production app as part of this spike.
- Do not remove the current WinUI app.
- Do not change HID protocol behavior.
- Do not add a permanent WPF project unless the spike explicitly decides that a prototype is needed and the human approves the build step.

Acceptance criteria:

- The spike documents concrete pros and cons for WPF versus the current WinUI implementation.
- The spike identifies a candidate WPF tray library or a native Windows Forms tray approach.
- The spike explains how WPF would handle installation/startup without requiring Developer Mode.
- The spike states whether the current core/device projects can remain unchanged.
- The spike ends with a clear recommendation and next task proposal.

## Implementation Plan

1. Explore the current WinUI app's pain points and separate them into framework issues, library issues, and implementation issues.
2. Identify one or two realistic WPF tray approaches.
3. Check WPF support for borderless popup windows, positioning near the tray, native drag, and DPI scaling.
4. Check packaging options for WPF, including plain executable, installer, and optional MSIX.
5. If needed, create a minimal WPF spike that reuses `Lightswitch.Core` and `Lightswitch.Device` and shows the switch popup from a tray icon.
6. Verify the spike with build/run notes or document why a code prototype was not needed.
7. Record a recommendation and propose the next implementation task.

## Build Log

Not started.

## Verification Log

Not started.

## Review Notes

Not started.

## Documentation Notes

Task record created. No architecture decision has been made yet.

## Closeout

Not started.

# 0002: Adopt WPF App Surface

## Status

Accepted

## Context

Lightswitch started as a WinUI 3 / Windows App SDK tray utility. During implementation, the app needed a small, reliable tray-first experience with a chromeless switch popup, simple settings window, direct HID control, and straightforward local development without Developer Mode friction.

A WPF rewrite trial was implemented in `TASK-0004`, followed by tray menu and HID control fixes in `TASK-0005` and `TASK-0006`. The WPF app became the active run path through `run-app.ps1`.

The old WinUI project was kept temporarily for rollback while the WPF trial was evaluated.

## Decision

Adopt WPF as the current app surface and remove the obsolete WinUI project from the active solution.

The current project structure is:

- `Lightswitch.Wpf` for WPF app startup, tray lifecycle, switch popup, settings UI, startup registration, and dependency wiring.
- `Lightswitch.Core` for shared models, validation, and service contracts.
- `Lightswitch.Device` for HidSharp-based Logitech Litra Glow HID communication.
- test projects for core and device behavior.

This decision supersedes the UI stack portion of `0001-initial-architecture.md`. The core/device separation from the initial architecture remains valid.

## Consequences

- The active app no longer depends on WinUI 3, Windows App SDK, or `H.NotifyIcon.WinUI`.
- Local run/build flows are simpler because the app is a normal WPF desktop app.
- The repository no longer carries two competing app surfaces.
- WPF app-layer infrastructure remains in `Lightswitch.Wpf` for now.
- If app-layer infrastructure grows or another UI surface is added, shared infrastructure can be extracted in a later task.

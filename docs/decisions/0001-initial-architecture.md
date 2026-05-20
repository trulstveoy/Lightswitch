# 0001: Initial Desktop Architecture

## Status

Superseded by `0002-adopt-wpf-app-surface.md` for the UI stack.

The core/device separation remains accepted.

## Context

Lightswitch needs to be a lightweight Windows utility for Logitech Litra Glow. It must use C#, .NET 8 or 9, WinUI 3, `H.NotifyIcon.WinUI`, and HidSharp. The app needs a small UI, tray controls, single-instance behavior, settings persistence, startup-with-Windows support, and direct USB HID communication without Logitech G Hub.

The repository is currently at initial scaffold stage, so this decision establishes the first module boundaries.

## Decision

Use a small multi-project solution:

- `Lightswitch.App` for WinUI 3, tray integration, app lifecycle, and UI.
- `Lightswitch.Core` for models, validation, and service contracts.
- `Lightswitch.Device` for HidSharp-based Logitech Litra Glow HID communication.
- `Lightswitch.Core.Tests` for unit tests around non-UI behavior.

The app targets .NET 9 initially because the local environment has .NET 9 SDK installed. The architecture keeps the option open to retarget to .NET 8 if Windows App SDK or deployment constraints require it later.

## Consequences

- UI code can be developed without direct knowledge of HID report details.
- Device protocol work can be tested independently from WinUI where practical.
- There is a little more solution structure than a single-project app, but the boundaries match the required separation between UI and device logic.
- Real HID behavior remains incomplete until the Litra Glow protocol details are verified against hardware or reliable documentation.

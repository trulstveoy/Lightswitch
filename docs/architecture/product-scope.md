# Product Scope: Lightswitch

Status: Draft
Updated: 2026-05-20

## Summary

Lightswitch is a lightweight Windows utility for controlling a Logitech Litra Glow directly from the desktop.

The app should run quietly in the background, expose fast tray controls, and avoid any dependency on Logitech G Hub.

## Goals

- Provide quick control of Logitech Litra Glow from Windows.
- Run as a tray-first desktop utility.
- Communicate directly with the device over USB HID.
- Keep the app small, predictable, and suitable for always-on background use.
- Provide a simple Windows 11-style UI for normal configuration.

## Non-Goals

- No dependency on Logitech G Hub.
- No cloud account, telemetry, or network service requirement.
- No support for unrelated Logitech devices in the initial version.
- No complex scene editor or automation engine in the initial version.

## Target Platform

- Windows 11 desktop.
- WPF desktop app on Windows 11.
- .NET 9 initially, unless a later compatibility issue requires .NET 8.

## Core Features

- Run as a Windows tray app.
- Show a main settings/control window on demand.
- Provide tray right-click quick actions.
- Enforce single-instance behavior.
- Detect Logitech Litra Glow over USB HID.
- Support device reconnect after USB disconnect.
- Control:
  - power on/off
  - brightness
  - color temperature
- Remember the last selected settings between app starts.
- Offer a user-facing "start with Windows" option.

## User Experience

- The app starts into the tray without requiring a large main window.
- The main window exposes one power toggle and sliders for brightness and color temperature.
- Tray menu actions cover the most common flows:
  - open window
  - power toggle
  - brightness presets
  - color temperature presets
  - start with Windows
  - exit
- Device disconnected state is visible without crashing or blocking app shutdown.

## Acceptance Criteria

- The app can be launched on Windows and remains available from the notification area.
- Only one running app instance is allowed.
- The app can save and reload the latest user-selected light state.
- The app can enable and disable startup with Windows.
- UI code does not construct HID reports directly.
- HID communication is isolated behind a service boundary.
- Device disconnect/reconnect does not crash the app.
- The solution builds from the repository root with documented commands.

## Open Questions

- Exact Logitech Litra Glow HID vendor/product IDs and report format must be verified against hardware or reliable protocol notes.
- Packaging format is not decided yet.
- Whether the app should start minimized by default or remember the last window visibility is not decided yet.

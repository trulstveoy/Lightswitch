# System Overview: Lightswitch

Status: Draft
Updated: 2026-05-14

## Summary

Lightswitch is structured as a small WinUI 3 desktop app with device control behind service boundaries. UI, tray lifecycle, persisted settings, startup registration, and HID communication should remain independently testable where practical.

## Initial Project Layout

```text
src/
  Lightswitch.App/
  Lightswitch.Core/
  Lightswitch.Device/
tests/
  Lightswitch.Core.Tests/
docs/
  architecture/
  decisions/
  tasks/
  workflows/
```

## Components

### Lightswitch.App

Responsibilities:

- WinUI 3 application shell.
- Main control window.
- Tray icon and context menu through `H.NotifyIcon.WinUI`.
- Single-instance startup guard.
- App lifecycle and dependency wiring.
- User-facing startup-with-Windows toggle.

This project should not build HID reports directly.

### Lightswitch.Core

Responsibilities:

- Shared models and contracts.
- Light state model.
- Settings model.
- Service interfaces for device control and persisted settings.
- Validation helpers for brightness and color temperature ranges.

This project should not depend on WinUI, Windows App SDK, HidSharp, or tray packages.

### Lightswitch.Device

Responsibilities:

- `LitraService` implementation.
- USB HID discovery and communication through HidSharp.
- Device connection state.
- Disconnect/reconnect polling or watching.
- Translation from domain state to Logitech Litra Glow HID reports.

This project should not contain UI logic.

### Tests

Responsibilities:

- Unit tests for non-UI logic.
- Validation behavior.
- Settings serialization where useful.
- Device protocol report construction once the protocol is verified.

## Data Flow

```text
WinUI controls / tray menu
  -> view model or app service boundary
  -> Lightswitch.Core contracts
  -> Lightswitch.Device LitraService
  -> HidSharp
  -> Logitech Litra Glow
```

Settings flow:

```text
UI / app lifecycle
  -> settings service
  -> app data JSON
```

## State Boundaries

- Desired light state is the user's requested state.
- Device connection state is whether a compatible HID device is currently reachable.
- Applied device state is the last state successfully sent to the device.
- Persisted settings should store user preferences, not transient USB errors.

## Reconnect Strategy

Initial reconnect behavior should be simple and robust:

- discover the device at startup;
- retry discovery on a timer when disconnected;
- mark state as disconnected when HID operations fail because the device disappeared;
- apply the last desired state after reconnect;
- keep UI responsive during all device operations.

## Persistence Strategy

Persist settings as JSON under the user's local application data directory unless a later packaging decision makes another Windows storage API preferable.

Settings should include:

- power state
- brightness
- color temperature
- start with Windows preference

## Startup With Windows

Initial implementation should use an explicit Windows startup registration mechanism appropriate for the chosen packaging model. The UI must expose the current startup preference and allow changing it.

## Single Instance

Single-instance logic should run early in app startup before showing UI or attaching device services.

When a second instance starts, the existing instance should be activated if practical. If activation is not implemented in the first slice, the second instance should exit cleanly.

## Known Technical Risks

- WinUI 3 build support depends on Windows App SDK tooling and packages being available in the local environment.
- The exact Litra Glow HID protocol must be verified before real device control can be considered complete.
- Tray behavior and startup registration may differ between packaged and unpackaged deployment.

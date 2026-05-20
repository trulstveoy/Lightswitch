# System Overview: Lightswitch

Status: Draft
Updated: 2026-05-20

## Summary

Lightswitch is structured as a small WPF tray utility with device control behind service boundaries. UI, tray lifecycle, persisted settings, startup registration, and HID communication should remain independently testable where practical.

## Project Layout

```text
src/
  Lightswitch.Wpf/
  Lightswitch.Core/
  Lightswitch.Device/
tests/
  Lightswitch.Core.Tests/
  Lightswitch.Device.Tests/
docs/
  architecture/
  decisions/
  tasks/
  workflows/
```

## Components

### Lightswitch.Wpf

Responsibilities:

- WPF application startup and lifecycle.
- Tray icon and context menu through `System.Windows.Forms.NotifyIcon`.
- Chromeless switch popup window.
- Basic settings window.
- Single-instance startup guard.
- User-facing startup-with-Windows toggle.
- Dependency wiring between UI, settings, and device services.

This project should not build HID reports directly.

### Lightswitch.Core

Responsibilities:

- Shared models and contracts.
- Light state model.
- Settings model.
- Service interfaces for device control and persisted settings.
- Validation helpers for brightness and color temperature ranges.

This project should not depend on WPF, Windows Forms, HidSharp, or tray APIs.

### Lightswitch.Device

Responsibilities:

- `LitraService` implementation.
- USB HID discovery and communication through HidSharp.
- Device connection state.
- Disconnect/reconnect handling.
- Translation from domain state to Logitech Litra Glow HID reports.

This project should not contain UI logic.

### Tests

Responsibilities:

- Unit tests for non-UI logic.
- Validation behavior.
- Device protocol report construction.

## Data Flow

```text
WPF controls / tray menu
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
- refresh device state when the HID device list changes;
- mark state as disconnected when the device cannot be found;
- mark state as error when HID operations fail;
- keep UI responsive during all device operations.

## Persistence Strategy

Persist settings as JSON under the user's local application data directory.

Settings include:

- power state
- brightness
- color temperature
- start with Windows preference

## Startup With Windows

Startup registration currently uses the current user's `Run` registry key. The UI exposes the current startup preference and allows changing it.

## Single Instance

Single-instance logic runs early in app startup before attaching device services. A second instance exits cleanly.

## Known Technical Risks

- Physical Litra Glow behavior should be verified after HID protocol changes.
- Tray behavior and startup registration may differ if the app later moves to an installer or packaged deployment model.
- The WPF settings window is intentionally basic and expected to be refined in a later task.

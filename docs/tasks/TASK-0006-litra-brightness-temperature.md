# Task: Litra Brightness And Temperature Control

ID: TASK-0006
Status: Done
Class: Standard
Owner: Agent
Created: 2026-05-20
Updated: 2026-05-20
Branch: `main`
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: `origin/main`
Write scope:
- `src/Lightswitch.Device/`
- `tests/Lightswitch.Device.Tests/`
- `docs/tasks/TASK-0006-litra-brightness-temperature.md`
Parallel safety: Coordinate

## Summary

Brightness and color temperature menu/settings choices currently update app state but do not affect the physical Litra Glow.

Implement HID reports for brightness and color temperature so the existing WPF menu and settings controls actually apply the selected values to the device.

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
- `../../src/Lightswitch.Device/LitraProtocol.cs`
- `../../src/Lightswitch.Device/LitraService.cs`
- `../../tests/Lightswitch.Device.Tests/LitraProtocolTests.cs`

## Explore Notes

- `MainViewModel` already stores brightness and temperature in `LightState`.
- WPF tray menu and settings controls already call `SetBrightnessCommand` and `SetTemperatureCommand`.
- `LitraService.ApplyAsync` currently only calls `ApplyPowerState`, so brightness and temperature are never sent to the HID device.
- Public HID examples for Litra Glow use:
  - brightness payload prefix `0x11,0xff,0x04,0x4c,0x00,<mapped brightness>`;
  - temperature payload prefix `0x11,0xff,0x04,0x9c,<high byte>,<low byte>`.

## Task Spec

In scope:

- Add HID report construction for brightness.
- Add HID report construction for color temperature.
- Update `LitraService.ApplyAsync` to send power plus brightness/temperature when the desired state is on.
- Preserve power-off behavior.
- Add tests for brightness and temperature report payloads.

Out of scope:

- No WPF UI redesign.
- No tray menu redesign.
- No support for multiple Litra devices.
- No installer/signing work.

Acceptance criteria:

- Brightness choices result in a brightness HID report when the light is on.
- Temperature choices result in a temperature HID report when the light is on.
- Power off still sends only the power-off report.
- Existing power tests still pass.
- New protocol tests cover brightness mapping and temperature byte order.

## Implementation Plan

1. Add `BuildBrightnessReport` and `BuildTemperatureReport` to `LitraProtocol`.
2. Map UI brightness 0-100 to device brightness 20-250.
3. Encode temperature Kelvin as high byte then low byte.
4. Update `LitraService` to write multiple reports for an on state.
5. Add protocol unit tests.
6. Build and run relevant tests.

## Build Log

Changed:
- Added brightness HID report construction.
- Added color temperature HID report construction.
- Updated `LitraService.ApplyAsync` to send power, brightness, and temperature reports when the desired state is on.
- Preserved power-off behavior so off state sends only the power-off report.
- Added protocol tests for brightness mapping and temperature byte order.
- Updated HID protocol notes.

Changed files:
- `src/Lightswitch.Device/LitraProtocol.cs`
- `src/Lightswitch.Device/LitraService.cs`
- `tests/Lightswitch.Device.Tests/LitraProtocolTests.cs`
- `docs/architecture/litra-hid-protocol.md`
- `docs/tasks/TASK-0006-litra-brightness-temperature.md`

## Verification Log

Passed:
- `dotnet build .\src\Lightswitch.Device\Lightswitch.Device.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- launch smoke test: started `Lightswitch.Wpf` with `dotnet run --project src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug --no-build`, confirmed the process started, then stopped it.

Failed:
- First parallel verification attempt combined device build and device test and hit an `obj` file lock because both commands compiled `Lightswitch.Device` at the same time.
  Follow-up: reran the commands serially; they passed.

Not run:
- Physical Litra Glow brightness/temperature verification.
  Reason: requires human testing against the connected light.

## Review Notes

Diff matches goal:
- Yes. The missing behavior was in `LitraService`, which previously only sent power reports.

Scope respected:
- Yes. No WPF UI redesign or tray menu redesign.

Risks remaining:
- HID command bytes are based on public reverse-engineered notes and need physical confirmation in the app.

## Documentation Notes

Updated:
- `docs/architecture/litra-hid-protocol.md`
- `docs/tasks/TASK-0006-litra-brightness-temperature.md`

## Closeout

Changed:
- Added brightness and color temperature HID reports.
- Updated `LitraService` to send brightness and temperature reports when the light is on.
- Preserved power-off behavior.
- Added protocol tests and updated HID protocol documentation.

Verified:
- `dotnet build .\src\Lightswitch.Device\Lightswitch.Device.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- WPF launch smoke test.

Known gaps:
- Physical Litra Glow confirmation remains the final practical check.

Final status:
- Done.

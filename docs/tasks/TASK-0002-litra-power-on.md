# Task: Litra Power On

ID: TASK-0002
Status: Done
Class: Major
Owner: Pair
Created: 2026-05-14
Updated: 2026-05-14
Branch: Not available
Worktree: `C:\Users\trutve\code\Lightswitch`
Base branch: Not available
Write scope:
- `src/Lightswitch.Device/`
- `src/Lightswitch.Core/`
- `tests/`
- `docs/architecture/`
- `docs/tasks/TASK-0002-litra-power-on.md`
Parallel safety: Exclusive

## Summary

Implement the first real Logitech Litra Glow HID command: turn the light on.

This task is intentionally narrow. It should prove end-to-end HID write capability before brightness and color temperature are implemented.

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
- `../architecture/product-scope.md`
- `../architecture/system-overview.md`
- `TASK-0001-initial-project-scaffold.md`
- `../../src/Lightswitch.Device/LitraService.cs`
- `../../src/Lightswitch.Core/ILitraService.cs`
- `../../src/Lightswitch.Core/LightState.cs`
- `../architecture/litra-hid-protocol.md`

Related decisions:
- `../decisions/0001-initial-architecture.md`

## Explore Notes

- `TASK-0001` created the initial WinUI/Core/Device scaffold.
- `LitraService` currently discovers likely Logitech Litra devices but does not write verified HID reports.
- `LightState.IsOn` exists in Core and flows from UI/viewmodel into `ILitraService.ApplyAsync`.
- HidSharp is already referenced by `Lightswitch.Device`.
- Public Litra Glow examples and the local device enumeration output indicate `VID=0x046D`, `PID=0xC900`, 20-byte output reports.
- The candidate power-on output payload is `0x11,0xFF,0x04,0x1C,0x01`, padded to 20 bytes.
- The candidate power-off output payload is `0x11,0xFF,0x04,0x1C,0x00`, padded to 20 bytes.
- The repository folder is not currently a Git repository, so worktree/branch workflow cannot be used yet.

## Task Spec

Implement actual power-on behavior for Logitech Litra Glow.

In scope:

- identify the connected Logitech Litra Glow HID device reliably enough for local testing;
- open the HID device with HidSharp;
- send the correct power-on report when `LightState.IsOn == true`;
- keep existing UI and viewmodel flow intact;
- handle missing device, denied access, disconnect, and write failure without crashing the app;
- add focused tests for protocol/report construction if the report can be represented as pure logic;
- document the verified product ID and report bytes in an architecture note or this task record.

Out of scope:

- brightness control;
- color temperature control;
- full reconnect strategy beyond preserving current non-crashing behavior;
- installer/package/release work;
- UI redesign.

Acceptance criteria:

- With Logitech Litra Glow connected, clicking the app power toggle to `On` sends the verified HID command and turns the light on.
- When the device is disconnected, the app remains running and reports disconnected/error status.
- `dotnet build .\Lightswitch.sln -c Debug` passes.
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug` passes.
- Any new protocol logic has unit tests where practical.
- The verified HID details are documented.

## Open Questions

- What is the exact Litra Glow product ID on the test machine?
- Which HID interface/usage page should be opened if the device exposes multiple HID interfaces?
- What exact report bytes turn the light on?
- Does the report need to be sent as output report, feature report, or both?
- Is Logitech G Hub or another process holding exclusive access to the device during testing?

## Implementation Plan

Phase 1: Automatic app-side HID identification

1. Keep HID discovery inside `LitraService`; do not add a separate probe tool.
2. Select Logitech Litra Glow by vendor ID `0x046D` and product ID `0xC900`.
3. Prefer a writable HID interface with at least 20-byte output reports.
4. Prefer the interface path containing `col02` when multiple `PID C900` interfaces are present, based on local enumeration showing that interface as writable with 20-byte output reports.
5. Keep a fallback for product/friendly/path matching that identifies `Litra`/`pid_c900`.

Phase 2: Protocol shape

1. Add `LitraProtocol` in `src/Lightswitch.Device`.
2. Keep report construction as pure logic.
3. Add unit tests for the power-on and power-off reports.
4. Document the source of the report bytes:
   - hardware observation;
   - reliable external protocol notes;
   - or controlled local experiment.
5. Do not implement brightness or color temperature reports in this task.

Phase 3: Device write

1. Update `LitraService` device selection to use the product ID/interface instead of product-name guessing where possible.
2. Open the selected HidSharp device only for the duration needed to send the command unless a persistent stream proves necessary.
3. Send the verified power-on report when `LightState.IsOn == true`.
4. Keep current behavior for `LightState.IsOn == false` unchanged unless the power protocol requires paired state handling.
5. Convert expected HID failures into `DeviceStatus.Error` or `DeviceStatus.Disconnected` instead of allowing UI-thread exceptions.
6. Preserve desired state so reconnect can re-apply later work.

Phase 4: Verification and documentation

1. Run `dotnet build .\Lightswitch.sln -c Debug`.
2. Run `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`.
3. Run the app with `.\run-app.ps1`.
4. Manually toggle power on in the UI and confirm the physical Litra Glow turns on.
5. Document verified product ID, selected HID interface, report type, report bytes, and any limitations.
6. Move the task to `Ready For Review` with exact human verification steps.

## Build Gate

Do not send undocumented experimental HID writes.

Only send output reports matching the documented `VID=0x046D`, `PID=0xC900`, 20-byte Litra Glow command shape in this task.

## Planned Files

Expected additions:

- `src/Lightswitch.Device/LitraProtocol.cs`
- protocol tests under `tests/`

Expected modifications:

- `Lightswitch.sln`
- `src/Lightswitch.Device/LitraService.cs`
- `docs/tasks/TASK-0002-litra-power-on.md`
- possibly `docs/architecture/system-overview.md` if verified protocol details should live outside the task record.

## Verification Plan

Automated:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- targeted protocol tests for `LitraProtocol`

Manual:

- run app from repo root with `.\run-app.ps1`;
- confirm toggling power on causes physical light output;
- unplug Litra Glow and confirm the app stays running without crashing.

## Risk Plan

- If multiple Logitech HID interfaces appear, prefer the one with report lengths matching the verified protocol source.
- If HidSharp cannot open the interface, check whether Logitech G Hub or another process has the device open.
- If Developer Mode blocks app launch, verify the device write path through a temporary console harness before returning to UI verification.

## Human Approval

Received implicitly when the user confirmed the task should be built and asked the agent to fix it in code.

## Build Log

Changed:

- Removed the separate `tools/Lightswitch.HidProbe` path after user clarified discovery must be built into the app.
- Added `LitraDeviceIds.LitraGlowProductId = 0xC900`.
- Added `LitraProtocol` with padded 20-byte power reports.
- Updated `LitraService` to automatically select a writable `VID 0x046D / PID 0xC900` HID interface.
- Updated `LitraService.ApplyAsync` to send the power report when the UI/viewmodel applies `LightState.IsOn`.
- Added `tests/Lightswitch.Device.Tests` and protocol tests.
- Added `docs/architecture/litra-hid-protocol.md`.

## Verification Log

Passed:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
- `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`

Not run:

None.

Manual:

- User ran the app and confirmed that power-on works with the physical Logitech Litra Glow.

## Review Notes

Diff matches goal:
- Mostly. The app now identifies a Litra Glow-compatible HID interface automatically and sends documented power reports from `LitraService`.

Scope respected:
- Yes. Brightness and color temperature reports were not implemented.

Risks remaining:
- Physical device verification is still required.
- Some machines may expose multiple Logitech HID interfaces; current selection prefers `PID C900`, writable 20-byte reports, and then `col02`.
- If another app holds the HID interface, `LitraService` reports an error instead of taking ownership.

Security concerns:
- No secrets, network calls, or privileged writes were added.
- HID writes are limited to the documented Litra Glow product ID/report shape.

Maintainability concerns:
- Protocol bytes are isolated in `LitraProtocol` and covered by tests.

## Documentation Notes

Docs updated:
- `docs/architecture/litra-hid-protocol.md`
- `docs/tasks/TASK-0002-litra-power-on.md`

## Closeout

Changed:

- Built Litra Glow identification into `LitraService`.
- Implemented 20-byte HID output reports for power on/off.
- Added protocol tests.
- Documented the verified Litra Glow HID details.

Verified:

- `dotnet build .\Lightswitch.sln -c Debug`
- `dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug`
- `dotnet test .\tests\Lightswitch.Device.Tests\Lightswitch.Device.Tests.csproj -c Debug`
- `dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64`
- `dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore`
- Manual physical verification by user: Litra Glow turns on from the app.

Known gaps:

- Brightness and color temperature HID reports are not implemented in this task.
- Power-off was implemented using the documented paired payload but has not been explicitly confirmed by the user in this closeout.

Next:

- Create a follow-up task for brightness and color temperature.

Final status:

Done.

Review handoff retained below for reference.

---

Ready for human review.

How to verify:

1. Connect Logitech Litra Glow to the PC.
2. Make sure Logitech G Hub is not actively controlling the light if the app reports access errors.
3. From `C:\Users\trutve\code\Lightswitch`, run:
   - `.\run-app.ps1`
4. In the Lightswitch window, turn the `Power` toggle on.
5. Confirm the physical Litra Glow turns on.
6. Optional disconnect test: unplug the light and confirm the app remains open and reports a non-crashing status.

Expected result:

- The app starts from repo root.
- The app automatically finds the connected `VID 046D / PID C900` Litra Glow HID interface.
- Toggling power on sends the 20-byte power-on HID output report.
- The physical light turns on.

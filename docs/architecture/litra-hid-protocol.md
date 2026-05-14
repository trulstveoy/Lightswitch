# Litra Glow HID Protocol Notes

Status: Draft
Updated: 2026-05-14

## Device

Logitech Litra Glow is identified as:

- Vendor ID: `0x046D`
- Product ID: `0xC900`

The app selects a writable HID interface with at least a 20-byte output report. When multiple matching interfaces are present, `LitraService` prefers the interface path containing `col02`, based on local enumeration where that interface exposed 20-byte input/output reports and could be opened.

## Power Reports

Reports are sent as HID output reports padded to 20 bytes.

Power on payload:

```text
11 FF 04 1C 01
```

Power off payload:

```text
11 FF 04 1C 00
```

Both payloads are padded with zero bytes to match the selected HID interface output report length.

## Sources

- `https://github.com/kharyam/go-litra-driver` documents Litra Glow as `046d:c900` and describes a reverse-engineered Litra control utility.
- `https://gist.github.com/statico/15c5c490c755caf836f484303b2a680c` shows 20-byte hidapitester output reports for Litra Glow power and light settings.
- `https://gist.github.com/9dc/010b7625980e71077211456b4e96ff84` shows Windows PowerShell/hidapitester use with `VID 046D`, `PID C900`, usage page `FF43`, and the same power payload shape.

## Open Items

- Verify the power reports against the physical device from the Lightswitch app.
- Confirm whether usage page filtering is needed in HidSharp device selection on machines with multiple Logitech devices.
- Add brightness and color temperature reports in separate tasks after power is verified.

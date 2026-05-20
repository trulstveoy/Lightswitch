# Lightswitch

Lightswitch is a small Windows desktop utility for controlling Logitech Litra Glow from the Windows tray.

## Stack

- C#
- .NET 9
- WPF in `src/Lightswitch.Wpf`
- `System.Windows.Forms.NotifyIcon` for the tray app
- HidSharp

## Build

Run from PowerShell in the repository root.

```powershell
dotnet build .\Lightswitch.sln -c Debug
```

## Run

Run from PowerShell in the repository root.

```powershell
.\run-app.ps1
```

The run script starts the WPF tray app.

For an explicit app build:

```powershell
dotnet build .\src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug
```

## Test

```powershell
dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug
```

## Package

Create a local Windows x64 ZIP package from PowerShell in the repository root.

```powershell
.\scripts\publish-zip.ps1 -Version local
```

The ZIP is written to `artifacts\Lightswitch-local-win-x64.zip`.

## Release

GitHub creates a publish ZIP only when a tag matching `v*` is pushed. Ordinary pushes to `main` do not create release packages.

```powershell
git tag v0.1.0
git push origin v0.1.0
```

The GitHub release workflow builds and tests the app, publishes `src\Lightswitch.Wpf`, creates `Lightswitch-v0.1.0-win-x64.zip`, uploads it as a workflow artifact, and attaches it to the tag's GitHub Release.

## Current State

The app runs tray-first with a WPF switch-style popup inspired by the physical light switch mockup. The settings window remains available from the tray context menu.

The app scaffold is in place with UI, tray menu, settings persistence, startup registration, single-instance guard, and a `LitraService` boundary.

Direct Logitech Litra Glow HID control is implemented for power, brightness, and color temperature. Physical-device verification remains the final practical check for future changes.

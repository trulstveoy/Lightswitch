# Lightswitch

Lightswitch is a small Windows desktop utility for controlling Logitech Litra Glow from the Windows tray.

## Stack

- C#
- .NET 9
- WinUI 3 / Windows App SDK
- H.NotifyIcon.WinUI
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

For an explicit x64 WinUI app build:

```powershell
dotnet restore .\src\Lightswitch.App\Lightswitch.App.csproj -p:Platform=x64
dotnet build .\src\Lightswitch.App\Lightswitch.App.csproj -c Debug -p:Platform=x64 --no-restore
```

## Test

```powershell
dotnet test .\tests\Lightswitch.Core.Tests\Lightswitch.Core.Tests.csproj -c Debug
```

## Current State

The app runs tray-first with a switch-style popup inspired by the physical light switch mockup. The settings window remains available from the tray context menu.

The app scaffold is in place with UI, tray menu, settings persistence, startup registration, single-instance guard, and a `LitraService` boundary.

Real Logitech Litra Glow HID control is not complete yet. The exact device product ID and HID report format still need to be verified against hardware or reliable protocol notes.

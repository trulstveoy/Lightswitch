# Lightswitch

Lightswitch is a small Windows desktop utility for controlling Logitech Litra Glow from the Windows tray.

## Stack

- C#
- .NET 9
- WPF rewrite trial in `src/Lightswitch.Wpf`
- Existing WinUI 3 / Windows App SDK app in `src/Lightswitch.App`
- `System.Windows.Forms.NotifyIcon` for the WPF tray app
- H.NotifyIcon.WinUI for the existing WinUI app
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

The run script starts the WPF rewrite trial.

For an explicit WPF app build:

```powershell
dotnet build .\src\Lightswitch.Wpf\Lightswitch.Wpf.csproj -c Debug
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

The active run path is a WPF rewrite trial. It runs tray-first with a switch-style popup inspired by the physical light switch mockup. The settings window remains available from the tray context menu.

The app scaffold is in place with UI, tray menu, settings persistence, startup registration, single-instance guard, and a `LitraService` boundary. The previous WinUI implementation remains in the repository while the WPF trial is evaluated.

Direct Logitech Litra Glow HID control is implemented for power, brightness, and color temperature. Physical-device verification remains the final practical check for future changes.

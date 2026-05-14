using Microsoft.Win32;

namespace Lightswitch.App.Infrastructure;

public sealed class StartupRegistrationService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "Lightswitch";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return key?.GetValue(ValueName) is string;
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

        if (enabled)
        {
            key.SetValue(ValueName, $"\"{Environment.ProcessPath}\"");
            return;
        }

        key.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}

using System.Threading;

namespace Lightswitch.Wpf.Infrastructure;

public sealed class SingleInstanceGuard : IDisposable
{
    private const string MutexName = "Local\\Lightswitch.LogitechLitraGlow";
    private readonly Mutex _mutex;
    private bool _disposed;

    public SingleInstanceGuard()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var isFirstInstance);
        IsFirstInstance = isFirstInstance;
    }

    public bool IsFirstInstance { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (IsFirstInstance)
        {
            _mutex.ReleaseMutex();
        }

        _mutex.Dispose();
        _disposed = true;
    }
}

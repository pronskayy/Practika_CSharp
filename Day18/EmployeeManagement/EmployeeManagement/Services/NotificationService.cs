using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Timers;

namespace EmployeeManagement.Services
{
    public class NotificationService : IDisposable
    {
        private const string MapName = "EmployeeNotifications";
        private const int MapSize = 4096;
        private MemoryMappedFile? _mmf;
        private System.Timers.Timer? _pollTimer;
        private readonly object _syncRoot = new object();
        private string? _lastMessage; // Защита от дублей при совпадении таймингов

        public event Action<string>? NotificationReceived;

        public void Initialize()
        {
            try { _mmf = MemoryMappedFile.CreateOrOpen(MapName, MapSize); }
            catch { _mmf = MemoryMappedFile.OpenExisting(MapName); }

            _pollTimer = new System.Timers.Timer(2000);
            _pollTimer.Elapsed += PollNotifications;
            _pollTimer.Start();
        }

        public void SendNotification(string message)
        {
            if (_mmf == null || string.IsNullOrWhiteSpace(message)) return;

            lock (_syncRoot)
            {
                using var accessor = _mmf.CreateViewAccessor(0, MapSize);
                var bytes = Encoding.UTF8.GetBytes(message);
                int len = Math.Min(bytes.Length, MapSize - 1);

                accessor.WriteArray(0, bytes, 0, len);
                accessor.Write(len, (byte)0); 
            }
        }

        private void PollNotifications(object? sender, ElapsedEventArgs e)
        {
            if (_mmf == null) return;

            lock (_syncRoot)
            {
                try
                {
                    using var accessor = _mmf.CreateViewAccessor(0, MapSize);
                    var buffer = new byte[MapSize];
                    accessor.ReadArray(0, buffer, 0, MapSize);

                    int nullIndex = Array.IndexOf(buffer, (byte)0);
                    var msg = nullIndex >= 0
                        ? Encoding.UTF8.GetString(buffer, 0, nullIndex)
                        : Encoding.UTF8.GetString(buffer);

                    if (!string.IsNullOrWhiteSpace(msg) && msg != _lastMessage)
                    {
                        _lastMessage = msg;
                        NotificationReceived?.Invoke(msg);

                        accessor.Write(0, (byte)0);
                        _lastMessage = null; // Сбрасываем кэш для будущих сообщений
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Dispose()
        {
            _pollTimer?.Stop();
            _pollTimer?.Dispose();
            _mmf?.Dispose();
        }
    }
}
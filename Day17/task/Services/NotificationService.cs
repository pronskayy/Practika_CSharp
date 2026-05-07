using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Day15.Services
{
    public class NotificationService
    {
        private const string MMFName = "ScheduleNotifications";
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;
        private Timer _timer;
        private Dispatcher _uiDispatcher;
        private string _lastNotification = "";

        public event Action<string> NotificationReceived;

        public NotificationService(Dispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
            _mmf = MemoryMappedFile.CreateOrOpen(MMFName, 1024);
            _accessor = _mmf.CreateViewAccessor();

            // Мониторим изменения каждые 500 мс
            _timer = new Timer(CheckForUpdates, null, 0, 500);
        }

        private void CheckForUpdates(object state)
        {
            byte[] buffer = new byte[1024];
            _accessor.ReadArray(0, buffer, 0, 1024);
            string notification = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

            if (!string.IsNullOrEmpty(notification) && notification != _lastNotification)
            {
                _lastNotification = notification;
                _uiDispatcher.Invoke(() => NotificationReceived?.Invoke(notification));
            }
        }

        public void SendNotification(string message, string sender)
        {
            var fullMessage = $"[{DateTime.Now:HH:mm}] {sender}: {message}";
            byte[] buffer = Encoding.UTF8.GetBytes(fullMessage.PadRight(1024));
            _accessor.WriteArray(0, buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }
}
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Day15.Services
{
    public class ChatService
    {
        private NamedPipeServerStream _server;
        private NamedPipeClientStream _client;
        private string _currentDepartment;
        private Dispatcher _uiDispatcher;

        public event Action<string, string> MessageReceived;

        public ChatService(Dispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
        }

        public async Task StartServerAsync(string department)
        {
            _currentDepartment = department;
            _server = new NamedPipeServerStream($"Chat_{department}", PipeDirection.InOut, 10);

            await Task.Run(() => _server.WaitForConnection());

            _ = Task.Run(() => ListenForMessages(_server));
        }

        public async Task ConnectToDepartmentAsync(string department)
        {
            _client = new NamedPipeClientStream(".", $"Chat_{department}", PipeDirection.InOut);
            await _client.ConnectAsync(5000);

            _ = Task.Run(() => ListenForMessages(_client));
        }

        private async Task ListenForMessages(PipeStream pipe)
        {
            var buffer = new byte[1024];
            while (pipe.IsConnected)
            {
                try
                {
                    int bytesRead = await pipe.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        var parts = message.Split('|');
                        var sender = parts[0];
                        var text = parts[1];

                        _uiDispatcher.Invoke(() => MessageReceived?.Invoke(sender, text));
                    }
                }
                catch { break; }
            }
        }

        public async Task SendMessageAsync(string message, string sender)
        {
            if (_server?.IsConnected == true)
            {
                var data = Encoding.UTF8.GetBytes($"{sender}|{message}");
                await _server.WriteAsync(data, 0, data.Length);
            }
            else if (_client?.IsConnected == true)
            {
                var data = Encoding.UTF8.GetBytes($"{sender}|{message}");
                await _client.WriteAsync(data, 0, data.Length);
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
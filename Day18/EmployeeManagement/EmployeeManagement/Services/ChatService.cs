using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Newtonsoft.Json;

namespace EmployeeManagement.Services
{
    public class ChatService : IDisposable
    {
        private const string PipeName = "EmployeeManagementChat";
        private CancellationTokenSource? _cts;
        private NamedPipeServerStream? _server;

        public event Action<ChatMessage>? MessageReceived;

        public void StartListening()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => ListenLoop(_cts.Token));
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _server = new NamedPipeServerStream(PipeName, PipeDirection.In,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                    await _server.WaitForConnectionAsync(token);

                    using var reader = new StreamReader(_server, Encoding.UTF8);
                    var json = await reader.ReadToEndAsync();
                    var message = JsonConvert.DeserializeObject<ChatMessage>(json);

                    if (message != null)
                        MessageReceived?.Invoke(message);

                    _server.Disconnect();
                }
                catch (OperationCanceledException) { break; }
                catch { }
            }
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.Asynchronous);
                await client.ConnectAsync(1000);
                var json = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                await client.WriteAsync(bytes);
            }
            catch { }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _server?.Dispose();
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EmployeeManagement.Commands;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Newtonsoft.Json;

namespace EmployeeManagement.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatService _chatService;
        private string _messageText = string.Empty;
        private readonly string _currentUser;
        private readonly string _currentDepartment;
        private readonly string _historyFilePath;

        // Блокировка для синхронизации записи в файл
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        public ICommand SendCommand { get; }

        public ChatViewModel(ChatService chatService, string currentUser, string currentDepartment)
        {
            _chatService = chatService;
            _currentUser = currentUser;
            _currentDepartment = currentDepartment;

            _historyFilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "chatHistory.json");

            _chatService.MessageReceived += OnMessageReceived;
            _chatService.StartListening();

            SendCommand = new RelayCommand(
                async _ => await SendMessageAsync(),
                _ => !string.IsNullOrWhiteSpace(MessageText));
        }

        /// <summary>
        /// Загрузка истории чата из файла при открытии окна
        /// </summary>
        public async Task LoadHistoryAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                var history = await LoadHistoryInternalAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Clear();
                    foreach (var msg in history)
                        Messages.Add(msg);
                });
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Обработка входящих сообщений из Named Pipes
        /// </summary>
        private void OnMessageReceived(ChatMessage message)
        {
            // Игнорируем свои собственные сообщения (избегаем дублей)
            if (message.Sender == _currentUser)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(message);
            });

            // Сохраняем в историю (с блокировкой)
            _ = AppendToHistoryAsync(message);
        }

        /// <summary>
        /// Отправка сообщения: показать у себя, сохранить в историю, разослать
        /// </summary>
        private async Task SendMessageAsync()
        {
            var msg = new ChatMessage
            {
                Sender = _currentUser,
                Department = _currentDepartment,
                Text = MessageText,
                Timestamp = DateTime.Now
            };

            // Показать себе
            Messages.Add(msg);

            // Сохранить в историю (с блокировкой)
            await AppendToHistoryAsync(msg);

            // Отправить другим
            await _chatService.SendMessageAsync(msg);

            MessageText = string.Empty;
        }

        // ====== Работа с файлом истории (JSON) с блокировкой ======

        private async Task<ObservableCollection<ChatMessage>> LoadHistoryInternalAsync()
        {
            if (!File.Exists(_historyFilePath))
                return new ObservableCollection<ChatMessage>();

            try
            {
                var json = await File.ReadAllTextAsync(_historyFilePath);
                var list = JsonConvert.DeserializeObject<ObservableCollection<ChatMessage>>(json);
                return list ?? new ObservableCollection<ChatMessage>();
            }
            catch
            {
                return new ObservableCollection<ChatMessage>();
            }
        }

        private async Task AppendToHistoryAsync(ChatMessage message)
        {
            await _fileLock.WaitAsync(); // Блокируем доступ к файлу
            try
            {
                ObservableCollection<ChatMessage> history;

                if (File.Exists(_historyFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(_historyFilePath);
                        history = JsonConvert.DeserializeObject<ObservableCollection<ChatMessage>>(json)
                                  ?? new ObservableCollection<ChatMessage>();
                    }
                    catch
                    {
                        history = new ObservableCollection<ChatMessage>();
                    }
                }
                else
                {
                    history = new ObservableCollection<ChatMessage>();
                }

                // Проверяем, нет ли уже такого сообщения (по времени и отправителю)
                bool isDuplicate = false;
                foreach (var existing in history)
                {
                    if (existing.Sender == message.Sender &&
                        existing.Text == message.Text &&
                        Math.Abs((existing.Timestamp - message.Timestamp).TotalSeconds) < 1)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    history.Add(message);

                    var newJson = JsonConvert.SerializeObject(history, Formatting.Indented);
                    await File.WriteAllTextAsync(_historyFilePath, newJson);
                }
            }
            finally
            {
                _fileLock.Release(); // Освобождаем блокировку
            }
        }
    }
}
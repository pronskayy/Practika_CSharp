using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
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

        // Путь к файлу истории
        private readonly string _historyFilePath;

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        public ICommand SendCommand { get; }

        public ChatViewModel(ChatService chatService,
            string currentUser, string currentDepartment)
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
            List<ChatMessage> history = await LoadHistoryInternalAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Clear();
                foreach (var msg in history)
                    Messages.Add(msg);
            });
        }

        /// <summary>
        /// Обработка входящих сообщений из Named Pipes
        /// </summary>
        private void OnMessageReceived(ChatMessage message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(message);
                // Асинхронно дописываем в историю
                _ = AppendToHistoryAsync(message);
            });
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

            // Сохранить в историю
            await AppendToHistoryAsync(msg);

            // Отправить другим
            await _chatService.SendMessageAsync(msg);

            MessageText = string.Empty;
        }

        // ====== Работа с файлом истории (JSON) ======

        private async Task<List<ChatMessage>> LoadHistoryInternalAsync()
        {
            if (!File.Exists(_historyFilePath))
                return new List<ChatMessage>();

            try
            {
                var json = await File.ReadAllTextAsync(_historyFilePath);
                var list = JsonConvert.DeserializeObject<List<ChatMessage>>(json);
                return list ?? new List<ChatMessage>();
            }
            catch
            {
                // Если файл битый — начинаем с пустой истории
                return new List<ChatMessage>();
            }
        }

        private async Task AppendToHistoryAsync(ChatMessage message)
        {
            List<ChatMessage> history;

            if (File.Exists(_historyFilePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_historyFilePath);
                    history = JsonConvert.DeserializeObject<List<ChatMessage>>(json)
                              ?? new List<ChatMessage>();
                }
                catch
                {
                    history = new List<ChatMessage>();
                }
            }
            else
            {
                history = new List<ChatMessage>();
            }

            history.Add(message);

            var newJson = JsonConvert.SerializeObject(history, Formatting.Indented);
            await File.WriteAllTextAsync(_historyFilePath, newJson);
        }
    }
}
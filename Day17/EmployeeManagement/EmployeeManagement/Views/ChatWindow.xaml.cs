using System.Windows;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Views
{
    public partial class ChatWindow : Window
    {
        public ChatWindow(string currentUser, string department)
        {
            InitializeComponent();

            var chatService = new ChatService();
            var vm = new ChatViewModel(chatService, currentUser, department);
            DataContext = vm;

            // При загрузке окна — подгружаем историю из файла
            Loaded += async (s, e) =>
            {
                await vm.LoadHistoryAsync();

                // Автопрокрутка к последнему сообщению
                if (MessageList.Items.Count > 0)
                    MessageList.ScrollIntoView(MessageList.Items[MessageList.Items.Count - 1]);
            };

            // Автопрокрутка при новых сообщениях
            vm.Messages.CollectionChanged += (s, e) =>
            {
                if (MessageList.Items.Count > 0)
                    MessageList.ScrollIntoView(
                        MessageList.Items[MessageList.Items.Count - 1]);
            };
        }
    }
}
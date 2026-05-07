using System.Windows;

namespace Day15.Views
{
    public partial class ChatWindow : Window
    {
        private string _department;

        public ChatWindow(string department)
        {
            InitializeComponent();
            _department = department;
            Title = $"Чат отдела: {department}";
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
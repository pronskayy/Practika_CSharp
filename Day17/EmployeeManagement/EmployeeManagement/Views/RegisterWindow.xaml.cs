using System.Windows;
using EmployeeManagement.Services;

namespace EmployeeManagement.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;

        public RegisterWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = RegUsernameBox.Text.Trim();
            var password = RegPasswordBox.Password;
            var dept = (DeptComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "IT";
            var role = AdminRole.IsChecked == true ? "Admin" : "User";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorLabel.Text = "Заполните все поля";
                ErrorLabel.Visibility = Visibility.Visible;
                return;
            }

            var success = await _userService.RegisterAsync(username, password, role, dept);
            if (success)
            {
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                ErrorLabel.Text = "Пользователь уже существует";
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        private void DeptComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
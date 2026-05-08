using System.Windows;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            var userService = new UserService();
            _viewModel = new LoginViewModel(userService);

            _viewModel.OnLoginSuccess = (username, role) =>
            {
                var mainWindow = new MainWindow(username, role);
                mainWindow.Show();
                this.Close();
            };

            _viewModel.OpenRegisterWindow = () =>
            {
                var reg = new RegisterWindow(userService);
                reg.ShowDialog();
            };

            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }
    }
}
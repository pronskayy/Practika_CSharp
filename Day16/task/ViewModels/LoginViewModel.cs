using System;
using System.Windows;
using System.Windows.Input;
using Day15.Services;
using Day15.Views;

namespace Day15.ViewModels
{
    public class LoginViewModel
    {
        private AuthService _authService;
        private Func<string> _passwordGetter;

        public string Username { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(_ => Login());
            RegisterCommand = new RelayCommand(_ => Register());
        }

        public void SetPassword(Func<string> passwordGetter)
        {
            _passwordGetter = passwordGetter;
        }

        private void Login()
        {
            string password = _passwordGetter?.Invoke() ?? "";
            var user = _authService.Authenticate(Username, password);
            if (user != null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                Application.Current.Windows[0]?.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register()
        {
            string password = _passwordGetter?.Invoke() ?? "";
            var result = _authService.Register(Username, password, "IT отдел");
            if (result)
            {
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пользователь уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
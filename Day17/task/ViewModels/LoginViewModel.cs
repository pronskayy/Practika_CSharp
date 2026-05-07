using System;
using System.Windows;
using System.Windows.Input;
using Day15.Services;
using Day15.Models;

namespace Day15.ViewModels
{
    public class LoginViewModel
    {
        private AuthService _authService;
        private Func<string> _passwordGetter;
        private Func<string> _usernameGetter;
        private Window _currentWindow;

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

        public void SetUsername(Func<string> usernameGetter)
        {
            _usernameGetter = usernameGetter;
        }

        public void SetWindow(Window window)
        {
            _currentWindow = window;
        }

        private void Login()
        {
            string username = _usernameGetter?.Invoke() ?? Username;
            string password = _passwordGetter?.Invoke() ?? "";

            var user = _authService.Authenticate(username, password);

            if (user != null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                _currentWindow?.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register()
        {
            // БЕРЁМ ДАННЫЕ ИЗ ПОЛЕЙ ВВОДА
            string username = _usernameGetter?.Invoke() ?? "";
            string password = _passwordGetter?.Invoke() ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = _authService.Register(username, password, "IT отдел");

            if (result)
            {
                MessageBox.Show("Регистрация успешна! Теперь войдите.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пользователь уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
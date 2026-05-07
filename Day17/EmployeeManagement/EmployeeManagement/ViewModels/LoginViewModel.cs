using System;
using System.Threading.Tasks;
using System.Windows.Input;
using EmployeeManagement.Commands;
using EmployeeManagement.Services;

namespace EmployeeManagement.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public Action<string, string>? OnLoginSuccess { get; set; }
        public Action? OpenRegisterWindow { get; set; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;

            LoginCommand = new RelayCommand(
                async _ => await LoginAsync(),
                _ => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));

            RegisterCommand = new RelayCommand(_ => OpenRegisterWindow?.Invoke());
        }

        private async Task LoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _userService.AuthenticateAsync(Username, Password);
                if (user != null)
                {
                    OnLoginSuccess?.Invoke(user.Username, user.Role);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
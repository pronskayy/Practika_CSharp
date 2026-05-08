using System.Windows;

namespace EmployeeManagement
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запуск с окна авторизации (по условию - локальная регистрация и аутентификация)
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}
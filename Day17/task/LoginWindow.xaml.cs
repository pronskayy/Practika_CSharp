using System.Windows;
using Day15.ViewModels;

namespace Day15
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoginViewModel;
            if (vm != null)
            {
                // Передаём методы для получения текста из полей
                vm.SetUsername(() => txtUsername.Text);
                vm.SetPassword(() => pbPassword.Password);
                vm.SetWindow(this);
            }
        }
    }
}
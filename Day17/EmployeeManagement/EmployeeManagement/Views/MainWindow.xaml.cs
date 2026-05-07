using System;
using System.Linq;
using System.Windows;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;
using EmployeeManagement.Views;

namespace EmployeeManagement
{
    public partial class MainWindow : Window
    {
        private readonly EmployeeViewModel _viewModel;
        private readonly ChatService _chatService;
        private readonly NotificationService _notificationService;
        private readonly string _currentUser;
        private readonly string _currentDepartment = "IT";

        public MainWindow(string username, string role)
        {
            InitializeComponent();
            _currentUser = username;
            _chatService = new ChatService();
            _notificationService = new NotificationService();

            var employeeService = new EmployeeService();
            _viewModel = new EmployeeViewModel(employeeService)
            {
                CurrentUser = username,
                CurrentUserRole = role
            };

            // Привязка действий открытия окон
            _viewModel.OpenAddWindow = OpenAddEmployeeWindow;
            _viewModel.OpenEditWindow = OpenEditEmployeeWindow;

            DataContext = _viewModel;

            // Инициализация сервисов обмена данными (Named Pipes + Memory-Mapped Files)
            _notificationService.Initialize();
            _notificationService.NotificationReceived += OnNotificationReceived;

            Loaded += async (s, e) => await _viewModel.LoadEmployeesAsync();
            
            Closed += (s, e) =>
            {
                _chatService.Dispose();
                _notificationService.Dispose();
            };
        }

        private void OpenAddEmployeeWindow()
        {
            var vm = new EmployeeEditViewModel();
            var window = new EmployeeEditWindow(vm) { Owner = this };
            vm.OnSave = employee =>
            {
                _viewModel.AddEmployee(employee);
                _ = _viewModel.SaveEmployeesAsync();
            };
            window.ShowDialog();
        }

        private void OpenEditEmployeeWindow(EmployeeModel employee)
        {
            var vm = new EmployeeEditViewModel(employee);
            var window = new EmployeeEditWindow(vm) { Owner = this };
            vm.OnSave = updated =>
            {
                _viewModel.UpdateEmployee(updated);
                _ = _viewModel.SaveEmployeesAsync();
            };
            window.ShowDialog();
        }

        private void OnNotificationReceived(string message)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "🔔 Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        // ===== Обработчики меню =====

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из программы?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void MenuReportDept_Click(object sender, RoutedEventArgs e)
        {
            var stats = _viewModel.Employees
                .GroupBy(emp => emp.Department)
                .Select(g => $"📁 {g.Key}: {g.Count()} сотрудников");

            MessageBox.Show(
                "ОТЧЁТ ПО ОТДЕЛАМ\n" + new string('=', 40) + "\n\n" + string.Join("\n", stats),
                "📊 Отчёт по отделам",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void MenuReportStat_Click(object sender, RoutedEventArgs e)
        {
            var total = _viewModel.Employees.Count;
            var available = _viewModel.Employees.Count(emp => emp.IsAvailable);
            var avgSalary = _viewModel.Employees.Any() ? _viewModel.Employees.Average(emp => emp.Salary) : 0;
            var maxSalary = _viewModel.Employees.Any() ? _viewModel.Employees.Max(emp => emp.Salary) : 0;
            var minSalary = _viewModel.Employees.Any() ? _viewModel.Employees.Min(emp => emp.Salary) : 0;

            MessageBox.Show(
                $"СТАТИСТИКА\n{new string('=', 40)}\n\n" +
                $"Всего сотрудников: {total}\n" +
                $"✅ Доступны: {available}\n" +
                $"❌ Отсутствуют: {total - available}\n\n" +
                $"ЗАРПЛАТЫ:\n" +
                $"Средняя: {avgSalary:N0} ₽\n" +
                $"Максимальная: {maxSalary:N0} ₽\n" +
                $"Минимальная: {minSalary:N0} ₽",
                "📈 Статистика",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void MenuReportPosition_Click(object sender, RoutedEventArgs e)
        {
            var stats = _viewModel.Employees
                .GroupBy(emp => emp.Position)
                .Select(g => $"💼 {g.Key}: {g.Count()} чел.");

            MessageBox.Show(
                "ОТЧЁТ ПО ДОЛЖНОСТЯМ\n" + new string('=', 40) + "\n\n" + string.Join("\n", stats),
                " Отчёт по должностям",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void MenuChat_Click(object sender, RoutedEventArgs e)
        {
            var chatWindow = new ChatWindow(_currentUser, _currentDepartment) { Owner = this };
            chatWindow.Show();
        }

        private void MenuNotify_Click(object sender, RoutedEventArgs e)
        {
            _notificationService.SendNotification(
                $" Уведомление от {_currentUser}: Данные сотрудников обновлены в {DateTime.Now:HH:mm}");

            MessageBox.Show("Уведомление отправлено всем пользователям!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "УЧЁТ СОТРУДНИКОВ\n" +
                "Версия 1.0\n\n" +
                "Технологии:\n" +
                "• WPF + MVVM\n" +
                "• JSON хранилище (employees.json, users.json)\n" +
                "• Named Pipes (чат между отделами)\n" +
                "• Memory-Mapped Files (уведомления)\n" +
                "• Асинхронное программирование\n\n" +
                "© 2026 Учебная практика",
                "ℹ️ О программе",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
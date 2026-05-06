using System.Windows;
using System.Windows.Controls;
using Day15.Models;
using Day15.ViewModels;

namespace Day15
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow(User user)
        {
            InitializeComponent();
            _viewModel = new MainViewModel(user);
            DataContext = _viewModel;
        }

        private void CbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
            var item = cbPosition.SelectedItem as ComboBoxItem;
            if (item != null)
                _viewModel.ApplyPositionFilter(item.Content.ToString());
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void ReportAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Всего сотрудников: {_viewModel?.Employees.Count ?? 0}", "Отчёт");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Автоматизация учета сотрудников\nВерсия 3.0", "О программе");
        }
    }
}
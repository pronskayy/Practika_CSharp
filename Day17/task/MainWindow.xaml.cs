using Day15.Models;
using Day15.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Day15
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private Storyboard _fadeInAnimation;
        private Storyboard _expandAnimation;
        private Storyboard _collapseAnimation;
        private int _currentAnimationIndex = 0;

        public MainWindow(User user)
        {
            InitializeComponent();
            _viewModel = new MainViewModel(user);
            DataContext = _viewModel;

            // Инициализируем анимации после загрузки окна
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Получаем анимации из ресурсов
            _fadeInAnimation = (Storyboard)FindResource("FadeInAnimation");
            _expandAnimation = (Storyboard)FindResource("ExpandProfileAnimation");
            _collapseAnimation = (Storyboard)FindResource("CollapseProfileAnimation");
        }

        private void EmployeeCard_Loaded(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            if (border != null && _fadeInAnimation != null)
            {
                // Анимация появления с задержкой
                var animation = _fadeInAnimation.Clone();
                animation.BeginTime = TimeSpan.FromMilliseconds(_currentAnimationIndex * 100);
                border.BeginStoryboard(animation);
                _currentAnimationIndex++;

                // Настройка индикатора статуса
                var statusIndicator = FindVisualChild<Ellipse>(border, "StatusIndicator");
                var employee = border.DataContext as Employee;

                if (statusIndicator != null && employee != null)
                {
                    string[] statuses = { "Доступен", "Отсутствует", "Не доступен" };
                    string status = statuses[employee.Id % 3];

                    switch (status)
                    {
                        case "Доступен":
                            statusIndicator.Fill = Brushes.Green;
                            break;
                        case "Отсутствует":
                            statusIndicator.Fill = Brushes.Orange;
                            break;
                        case "Не доступен":
                            statusIndicator.Fill = Brushes.Red;
                            break;
                    }

                    var pulseAnimation = (Storyboard)FindResource("StatusPulseAnimation");
                    if (pulseAnimation != null)
                    {
                        statusIndicator.BeginStoryboard(pulseAnimation);
                    }
                }
            }
        }

        private void CbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
            _currentAnimationIndex = 0;

            var item = cbPosition.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                string filter = item.Content.ToString();
                _viewModel.ApplyPositionFilter(filter);
            }
        }

        private void ToggleProfile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.Tag as Employee;

            if (employee != null && _expandAnimation != null && _collapseAnimation != null)
            {
                ProfileName.Text = employee.FullName;
                ProfilePosition.Text = $"Должность: {employee.Position}";
                ProfileDepartment.Text = $"Отдел: {(employee.DepartmentId == 1 ? "IT отдел" : "Управление")}";

                string[] statuses = { "Доступен", "Отсутствует", "Не доступен" };
                ProfileStatus.Text = $"Статус: {statuses[employee.Id % 3]}";

                if (ExpandedProfile.Visibility != Visibility.Visible)
                {
                    ExpandedProfile.Visibility = Visibility.Visible;
                    _expandAnimation.Begin(ExpandedProfile);
                }
                else
                {
                    _collapseAnimation.Begin(ExpandedProfile);
                    ExpandedProfile.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CloseProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_collapseAnimation != null)
            {
                _collapseAnimation.Begin(ExpandedProfile);
                ExpandedProfile.Visibility = Visibility.Collapsed;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
        private void ReportAll_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Отчёт готов");
        private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("О программе");

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && t.Name == name)
                    return t;

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
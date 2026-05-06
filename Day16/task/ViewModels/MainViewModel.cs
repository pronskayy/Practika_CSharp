using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Day15.Models;
using Day15.Services;
using Day15.Views;

namespace Day15.ViewModels
{
    public class MainViewModel
    {
        private readonly EmployeeService _service;
        private ObservableCollection<Employee> _allEmployees;
        private User _currentUser;

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Department> Departments { get; set; }
        public ObservableCollection<string> Notifications { get; set; }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _selectedDepartmentId;
        public int SelectedDepartmentId
        {
            get => _selectedDepartmentId;
            set
            {
                _selectedDepartmentId = value;
                ApplyDepartmentFilter();
            }
        }

        private string _selectedPositionFilter = "Все";
        public string SelectedPositionFilter
        {
            get => _selectedPositionFilter;
            set
            {
                _selectedPositionFilter = value;
                ApplyFullFilter();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set { _progressValue = value; OnPropertyChanged(nameof(ProgressValue)); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand OpenChatCommand { get; set; }
        public ICommand SendNotificationCommand { get; set; }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));

        public MainViewModel(User currentUser)
        {
            _currentUser = currentUser;
            _service = new EmployeeService();
            Employees = new ObservableCollection<Employee>();
            _allEmployees = new ObservableCollection<Employee>();
            Notifications = new ObservableCollection<string>();

            Departments = new ObservableCollection<Department>
            {
                new Department { Id = 0, Name = "Все отделы" },
                new Department { Id = 1, Name = "IT отдел" },
                new Department { Id = 2, Name = "Управление" }
            };

            AddCommand = new RelayCommand(_ => AddEmployee());
            EditCommand = new RelayCommand(_ => EditEmployee(), _ => SelectedEmployee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => SelectedEmployee != null);
            OpenChatCommand = new RelayCommand(_ => OpenChat());
            SendNotificationCommand = new RelayCommand(_ => SendNotification());

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            IsLoading = true;
            for (int i = 0; i <= 100; i += 20)
            {
                ProgressValue = i;
                await Task.Delay(200);
            }

            var employees = await _service.GetEmployeesAsync();
            _allEmployees.Clear();
            Employees.Clear();
            foreach (var emp in employees)
            {
                _allEmployees.Add(emp);
                Employees.Add(emp);
            }

            IsLoading = false;
        }

        private void ApplyDepartmentFilter()
        {
            ApplyFullFilter();
        }

        private void ApplyFullFilter()
        {
            Employees.Clear();
            var filtered = _allEmployees.Where(e =>
                (SelectedDepartmentId == 0 || e.DepartmentId == SelectedDepartmentId) &&
                (SelectedPositionFilter == "Все" || e.Position == SelectedPositionFilter)
            ).ToList();

            foreach (var emp in filtered)
                Employees.Add(emp);
        }

        public void ApplyPositionFilter(string position)
        {
            SelectedPositionFilter = position;
        }

        private async void AddEmployee()
        {
            var dialog = new EditDialog();
            if (dialog.ShowDialog() == true)
            {
                var newEmp = new Employee
                {
                    Id = _allEmployees.Count + 1,
                    FullName = dialog.FullName,
                    Position = dialog.Position,
                    DepartmentId = SelectedDepartmentId == 0 ? 1 : SelectedDepartmentId
                };
                await _service.AddEmployeeAsync(newEmp);
                _allEmployees.Add(newEmp);
                ApplyFullFilter();
            }
        }

        private async void EditEmployee()
        {
            if (SelectedEmployee == null) return;
            var dialog = new EditDialog(SelectedEmployee.FullName, SelectedEmployee.Position);
            if (dialog.ShowDialog() == true)
            {
                SelectedEmployee.FullName = dialog.FullName;
                SelectedEmployee.Position = dialog.Position;
                await _service.UpdateEmployeeAsync(SelectedEmployee);
                ApplyFullFilter();
            }
        }

        private async void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;
            if (MessageBox.Show($"Удалить {SelectedEmployee.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _service.DeleteEmployeeAsync(SelectedEmployee.Id);
                _allEmployees.Remove(SelectedEmployee);
                ApplyFullFilter();
            }
        }

        private void OpenChat()
        {
            var chatWindow = new ChatWindow(_currentUser.Department);
            chatWindow.Show();
        }

        private void SendNotification()
        {
            MessageBox.Show("Уведомление отправлено!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
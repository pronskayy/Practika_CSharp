using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EmployeeManagement.Commands;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        private readonly AppDbContext _dbContext;  // ← ДОБАВЛЕНО: контекст БД вместо EmployeeService
        private ObservableCollection<EmployeeModel> _employees = new();
        private ObservableCollection<EmployeeModel> _filteredEmployees = new();
        private EmployeeModel? _selectedEmployee;
        private string _filterDepartment = "Все";
        private string _filterPosition = "Все";
        private string _searchText = string.Empty;
        private bool _isLoading;
        private int _loadingProgress;
        private string _currentUser = string.Empty;
        private string _currentUserRole = string.Empty;

        public ObservableCollection<string> Departments { get; } = new() { "Все", "IT", "HR", "Финансы", "Маркетинг", "Производство" };
        public ObservableCollection<string> Positions { get; } = new() { "Все", "Разработчик", "Менеджер", "Аналитик", "Дизайнер", "Тестировщик", "Директор" };

        public ObservableCollection<EmployeeModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<EmployeeModel> FilteredEmployees
        {
            get => _filteredEmployees;
            set => SetProperty(ref _filteredEmployees, value);
        }

        public EmployeeModel? SelectedEmployee
        {
            get => _selectedEmployee;
            set { SetProperty(ref _selectedEmployee, value); CommandManager.InvalidateRequerySuggested(); }
        }

        public string FilterDepartment
        {
            get => _filterDepartment;
            set { SetProperty(ref _filterDepartment, value); ApplyFilter(); }
        }

        public string FilterPosition
        {
            get => _filterPosition;
            set { SetProperty(ref _filterPosition, value); ApplyFilter(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); ApplyFilter(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public int LoadingProgress
        {
            get => _loadingProgress;
            set => SetProperty(ref _loadingProgress, value);
        }

        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string CurrentUserRole
        {
            get => _currentUserRole;
            set => SetProperty(ref _currentUserRole, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SaveCommand { get; }

        public Action? OpenAddWindow { get; set; }
        public Action<EmployeeModel>? OpenEditWindow { get; set; }

        // ← ИЗМЕНЕНО: принимаем AppDbContext вместо EmployeeService
        public EmployeeViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            AddCommand = new RelayCommand(
                _ => OpenAddWindow?.Invoke(),
                _ => CurrentUserRole == "Admin");

            EditCommand = new RelayCommand(
                _ => OpenEditWindow?.Invoke(SelectedEmployee!.Clone()),
                _ => SelectedEmployee != null && CurrentUserRole == "Admin");

            DeleteCommand = new RelayCommand(
                _ => DeleteEmployee(),
                _ => SelectedEmployee != null && CurrentUserRole == "Admin");

            RefreshCommand = new RelayCommand(async _ => await LoadEmployeesAsync());

            SaveCommand = new RelayCommand(
                async _ => await SaveEmployeesAsync(),
                _ => CurrentUserRole == "Admin");
        }

        // ← ИЗМЕНЕНО: загружаем из БД
        public async Task LoadEmployeesAsync()
        {
            IsLoading = true;
            LoadingProgress = 0;

            try
            {
                var progress = new Progress<int>(p => LoadingProgress = p);
                var employees = await Task.Run(async () =>
                {
                    ((IProgress<int>)progress).Report(30);
                    await Task.Delay(300);

                    // Загружаем из БД вместо JSON файла
                    var data = await _dbContext.Employees.ToListAsync();

                    ((IProgress<int>)progress).Report(80);
                    await Task.Delay(200);
                    ((IProgress<int>)progress).Report(100);
                    return data;
                });

                Employees = new ObservableCollection<EmployeeModel>(employees);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await Task.Delay(400);
                IsLoading = false;
            }
        }

        // ← ИЗМЕНЕНО: сохраняем в БД
        public async Task SaveEmployeesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                MessageBox.Show("Данные сохранены успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddEmployee(EmployeeModel employee)
        {
            employee.Id = Employees.Any() ? Employees.Max(e => e.Id) + 1 : 1;
            Employees.Add(employee);
            ApplyFilter();
        }

        public void UpdateEmployee(EmployeeModel updated)
        {
            var existing = Employees.FirstOrDefault(e => e.Id == updated.Id);
            if (existing == null) return;

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.MiddleName = updated.MiddleName;
            existing.Position = updated.Position;
            existing.Department = updated.Department;
            existing.Email = updated.Email;
            existing.Phone = updated.Phone;
            existing.Salary = updated.Salary;
            existing.IsAvailable = updated.IsAvailable;
            existing.HireDate = updated.HireDate;

            ApplyFilter();
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            var result = MessageBox.Show(
                $"Удалить сотрудника {SelectedEmployee.FullName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Employees.Remove(SelectedEmployee);
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            var query = Employees.AsEnumerable();

            if (FilterDepartment != "Все")
                query = query.Where(e => e.Department == FilterDepartment);

            if (FilterPosition != "Все")
                query = query.Where(e => e.Position == FilterPosition);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lower = SearchText.ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(lower) ||
                    e.Position.ToLower().Contains(lower) ||
                    e.Department.ToLower().Contains(lower) ||
                    e.Email.ToLower().Contains(lower));
            }

            FilteredEmployees = new ObservableCollection<EmployeeModel>(query);
        }
    }
}
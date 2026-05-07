using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EmployeeManagement.Commands;
using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeEditViewModel : BaseViewModel
    {
        private EmployeeModel _employee = new();

        // TwoWay привязка (по условию)
        public EmployeeModel Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }

        public ObservableCollection<string> Departments { get; } = new() { "IT", "HR", "Финансы", "Маркетинг", "Производство" };
        public ObservableCollection<string> Positions { get; } = new() { "Разработчик", "Менеджер", "Аналитик", "Дизайнер", "Тестировщик", "Директор" };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<EmployeeModel>? OnSave { get; set; }
        public Action? OnCancel { get; set; }

        public bool IsEditMode { get; private set; }

        public string WindowTitle => IsEditMode ? "Редактирование сотрудника" : "Добавление сотрудника";

        public EmployeeEditViewModel()
        {
            Employee = new EmployeeModel { HireDate = DateTime.Today };
            IsEditMode = false;

            SaveCommand = new RelayCommand(
                _ => OnSave?.Invoke(Employee),
                _ => !string.IsNullOrWhiteSpace(Employee.FirstName) && !string.IsNullOrWhiteSpace(Employee.LastName));

            CancelCommand = new RelayCommand(_ => OnCancel?.Invoke());
        }

        public EmployeeEditViewModel(EmployeeModel employee) : this()
        {
            Employee = employee;
            IsEditMode = true;
        }
    }
}
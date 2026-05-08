using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EmployeeManagement.Models
{
    public class EmployeeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _middleName = string.Empty;
        private string _position = string.Empty;
        private string _department = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private decimal _salary;
        private bool _isAvailable = true;
        private DateTime _hireDate = DateTime.Today;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public string MiddleName
        {
            get => _middleName;
            set { _middleName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        // OneWay привязка — вычисляемое свойство
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public decimal Salary
        {
            get => _salary;
            set { _salary = value; OnPropertyChanged(); }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set { _isAvailable = value; OnPropertyChanged(); }
        }

        public DateTime HireDate
        {
            get => _hireDate;
            set { _hireDate = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EmployeeModel Clone()
        {
            return new EmployeeModel
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleName = this.MiddleName,
                Position = this.Position,
                Department = this.Department,
                Email = this.Email,
                Phone = this.Phone,
                Salary = this.Salary,
                IsAvailable = this.IsAvailable,
                HireDate = this.HireDate
            };
        }
    }
}
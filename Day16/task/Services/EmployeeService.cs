using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Day15.Models;

namespace Day15.Services
{
    public class EmployeeService
    {
        private readonly string _employeesFile = "Data/employees.json";
        private List<Employee> _employees;

        public EmployeeService()
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            if (File.Exists(_employeesFile))
            {
                var json = File.ReadAllText(_employeesFile);
                _employees = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
            }
            else
            {
                _employees = new List<Employee>
                {
                    new Employee { Id = 1, FullName = "Иванов Иван Иванович", Position = "Разработчик", DepartmentId = 1 },
                    new Employee { Id = 2, FullName = "Петров Петр Петрович", Position = "Менеджер", DepartmentId = 2 },
                    new Employee { Id = 3, FullName = "Сидорова Анна Сергеевна", Position = "Аналитик", DepartmentId = 1 }
                };
                SaveEmployees();
            }
        }

        private void SaveEmployees()
        {
            Directory.CreateDirectory("Data");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_employees, options);
            File.WriteAllText(_employeesFile, json);
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await Task.Run(() => _employees.ToList());
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await Task.Run(() =>
            {
                employee.Id = _employees.Count + 1;
                _employees.Add(employee);
                SaveEmployees();
            });
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            await Task.Run(() =>
            {
                var index = _employees.FindIndex(e => e.Id == employee.Id);
                if (index != -1)
                {
                    _employees[index] = employee;
                    SaveEmployees();
                }
            });
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await Task.Run(() =>
            {
                var employee = _employees.FirstOrDefault(e => e.Id == id);
                if (employee != null)
                {
                    _employees.Remove(employee);
                    SaveEmployees();
                }
            });
        }
    }
}
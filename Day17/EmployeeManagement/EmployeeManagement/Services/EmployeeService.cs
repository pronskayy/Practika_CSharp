using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Newtonsoft.Json;

namespace EmployeeManagement.Services
{
    public class EmployeeService
    {
        private readonly string _filePath;

        public EmployeeService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.json");
        }

        public async Task<List<EmployeeModel>> LoadEmployeesAsync()
        {
            return await Task.Run(async () =>
            {
                if (!File.Exists(_filePath))
                    return GetDefaultEmployees();

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<EmployeeModel>>(json) ?? new List<EmployeeModel>();
            });
        }

        public async Task SaveEmployeesAsync(IEnumerable<EmployeeModel> employees)
        {
            await Task.Run(async () =>
            {
                var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, json);
            });
        }

        public int GetNextId(IEnumerable<EmployeeModel> employees)
        {
            return employees.Any() ? employees.Max(e => e.Id) + 1 : 1;
        }

        private List<EmployeeModel> GetDefaultEmployees()
        {
            return new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    Id = 1, FirstName = "Иван", LastName = "Иванов",
                    MiddleName = "Иванович", Position = "Разработчик",
                    Department = "IT", Email = "ivan@company.com",
                    Phone = "+7-900-000-0001", Salary = 85000,
                    IsAvailable = true, HireDate = DateTime.Now.AddYears(-2)
                },
                new EmployeeModel
                {
                    Id = 2, FirstName = "Мария", LastName = "Петрова",
                    MiddleName = "Сергеевна", Position = "Менеджер",
                    Department = "HR", Email = "maria@company.com",
                    Phone = "+7-900-000-0002", Salary = 70000,
                    IsAvailable = false, HireDate = DateTime.Now.AddYears(-1)
                },
                new EmployeeModel
                {
                    Id = 3, FirstName = "Алексей", LastName = "Сидоров",
                    MiddleName = "Петрович", Position = "Аналитик",
                    Department = "Финансы", Email = "alex@company.com",
                    Phone = "+7-900-000-0003", Salary = 75000,
                    IsAvailable = true, HireDate = DateTime.Now.AddMonths(-6)
                }
            };
        }
    }
}
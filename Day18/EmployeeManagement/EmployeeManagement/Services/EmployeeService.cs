using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Newtonsoft.Json;

namespace EmployeeManagement.Services
{
    public class EmployeeService
    {
        private readonly string _filePath;

        // Блокировка, чтобы файл не пытались читать и писать одновременно
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

        public EmployeeService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.json");
        }

        public async Task<List<EmployeeModel>> LoadEmployeesAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    // Если файла нет, берем начальные данные и СРАЗУ сохраняем их в файл
                    var defaults = GetDefaultEmployees();
                    var jsonWrite = JsonConvert.SerializeObject(defaults, Formatting.Indented);
                    await File.WriteAllTextAsync(_filePath, jsonWrite);
                    return defaults;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<EmployeeModel>>(json) ?? new List<EmployeeModel>();
            }
            catch
            {
                return GetDefaultEmployees();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task SaveEmployeesAsync(IEnumerable<EmployeeModel> employees)
        {
            await _fileLock.WaitAsync();
            try
            {
                var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, json);
            }
            finally
            {
                _fileLock.Release();
            }
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
                    Department = "IT", Salary = 2500,
                    IsAvailable = true, HireDate = DateTime.Now.AddYears(-2)
                },
                new EmployeeModel
                {
                    Id = 2, FirstName = "Мария", LastName = "Петрова",
                    MiddleName = "Сергеевна", Position = "Менеджер",
                    Department = "HR", Salary = 1800,
                    IsAvailable = false, HireDate = DateTime.Now.AddYears(-1)
                }
            };
        }
    }
}
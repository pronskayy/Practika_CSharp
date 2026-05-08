using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<EmployeeModel> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=EmployeeDatabase.db");
        }

        public void InitializeData()
        {
            // Если в БД уже есть данные, не добавляем
            if (Employees.Any())
                return;

            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { Id = 1, FirstName = "Иван", LastName = "Иванов", MiddleName = "Иванович", Position = "Разработчик", Department = "IT", Email = "ivan@company.com", Phone = "+375291111111", Salary = 3500, IsAvailable = true, HireDate = new DateTime(2021, 6, 15) },
                new EmployeeModel { Id = 2, FirstName = "Владимир", LastName = "Петров", MiddleName = "Сергеевич", Position = "Разработчик", Department = "IT", Email = "vladimir@company.com", Phone = "+375291234567", Salary = 3800, IsAvailable = true, HireDate = new DateTime(2020, 3, 22) },
                new EmployeeModel { Id = 3, FirstName = "Сергей", LastName = "Сидоров", MiddleName = "Александрович", Position = "Senior Разработчик", Department = "IT", Email = "sergey@company.com", Phone = "+375299876543", Salary = 4500, IsAvailable = true, HireDate = new DateTime(2019, 1, 10) },
                new EmployeeModel { Id = 4, FirstName = "Алексей", LastName = "Морозов", MiddleName = "Викторович", Position = "Тестировщик", Department = "IT", Email = "alexey@company.com", Phone = "+375295555555", Salary = 2800, IsAvailable = false, HireDate = new DateTime(2022, 9, 5) },
                new EmployeeModel { Id = 5, FirstName = "Евгений", LastName = "Волков", MiddleName = "Олегович", Position = "DevOps", Department = "IT", Email = "evgeny@company.com", Phone = "+375297777777", Salary = 3900, IsAvailable = true, HireDate = new DateTime(2021, 11, 20) },
                new EmployeeModel { Id = 6, FirstName = "Мария", LastName = "Петрова", MiddleName = "Сергеевна", Position = "Менеджер HR", Department = "HR", Email = "maria@company.com", Phone = "+375294444444", Salary = 2200, IsAvailable = true, HireDate = new DateTime(2020, 5, 12) },
                new EmployeeModel { Id = 7, FirstName = "Елена", LastName = "Соколова", MiddleName = "Ивановна", Position = "Рекрутер", Department = "HR", Email = "elena@company.com", Phone = "+375293333333", Salary = 1900, IsAvailable = true, HireDate = new DateTime(2022, 2, 8) },
                new EmployeeModel { Id = 8, FirstName = "Ольга", LastName = "Смирнова", MiddleName = "Дмитриевна", Position = "Специалист по кадрам", Department = "HR", Email = "olga@company.com", Phone = "+375298888888", Salary = 1800, IsAvailable = false, HireDate = new DateTime(2021, 8, 30) },
                new EmployeeModel { Id = 9, FirstName = "Дмитрий", LastName = "Финансов", MiddleName = "Романович", Position = "Главный бухгалтер", Department = "Финансы", Email = "dmitry@company.com", Phone = "+375296666666", Salary = 2900, IsAvailable = true, HireDate = new DateTime(2018, 4, 1) },
                new EmployeeModel { Id = 10, FirstName = "Татьяна", LastName = "Кузнецова", MiddleName = "Николаевна", Position = "Бухгалтер", Department = "Финансы", Email = "tatiana@company.com", Phone = "+375299999999", Salary = 2100, IsAvailable = true, HireDate = new DateTime(2021, 7, 15) },
                new EmployeeModel { Id = 11, FirstName = "Валентина", LastName = "Фёдорова", MiddleName = "Павловна", Position = "Финансовый аналитик", Department = "Финансы", Email = "valentina@company.com", Phone = "+375292222222", Salary = 2400, IsAvailable = true, HireDate = new DateTime(2020, 10, 21) },
                new EmployeeModel { Id = 12, FirstName = "Андрей", LastName = "Маркетов", MiddleName = "Геннадьевич", Position = "Директор маркетинга", Department = "Маркетинг", Email = "andrey@company.com", Phone = "+375291010101", Salary = 3200, IsAvailable = true, HireDate = new DateTime(2019, 9, 1) },
                new EmployeeModel { Id = 13, FirstName = "Наталья", LastName = "Сафонова", MiddleName = "Вячеславовна", Position = "Маркетолог", Department = "Маркетинг", Email = "natalya@company.com", Phone = "+375291212121", Salary = 2300, IsAvailable = false, HireDate = new DateTime(2022, 1, 10) },
                new EmployeeModel { Id = 14, FirstName = "Григорий", LastName = "Баранов", MiddleName = "Евгеньевич", Position = "SMM специалист", Department = "Маркетинг", Email = "grigory@company.com", Phone = "+375291313131", Salary = 2000, IsAvailable = true, HireDate = new DateTime(2023, 3, 15) },
                new EmployeeModel { Id = 15, FirstName = "Петр", LastName = "Производцев", MiddleName = "Станиславович", Position = "Начальник производства", Department = "Производство", Email = "petr@company.com", Phone = "+375291414141", Salary = 2700, IsAvailable = true, HireDate = new DateTime(2018, 11, 5) },
                new EmployeeModel { Id = 16, FirstName = "Константин", LastName = "Мастеров", MiddleName = "Сергеевич", Position = "Мастер", Department = "Производство", Email = "konstantin@company.com", Phone = "+375291515151", Salary = 2000, IsAvailable = true, HireDate = new DateTime(2021, 4, 20) },
                new EmployeeModel { Id = 17, FirstName = "Максим", LastName = "Рабочев", MiddleName = "Владимирович", Position = "Рабочий", Department = "Производство", Email = "maxim@company.com", Phone = "+375291616161", Salary = 1600, IsAvailable = false, HireDate = new DateTime(2022, 6, 1) },
                new EmployeeModel { Id = 18, FirstName = "Борис", LastName = "Комплектов", MiddleName = "Игоревич", Position = "Рабочий", Department = "Производство", Email = "boris@company.com", Phone = "+375291717171", Salary = 1600, IsAvailable = true, HireDate = new DateTime(2023, 1, 15) },
                new EmployeeModel { Id = 19, FirstName = "Юрий", LastName = "Генеральов", MiddleName = "Борисович", Position = "Генеральный директор", Department = "IT", Email = "yury@company.com", Phone = "+375291818181", Salary = 5500, IsAvailable = true, HireDate = new DateTime(2015, 1, 1) },
                new EmployeeModel { Id = 20, FirstName = "Анна", LastName = "Администратор", MiddleName = "Сергеевна", Position = "Администратор офиса", Department = "HR", Email = "anna@company.com", Phone = "+375291919191", Salary = 1700, IsAvailable = true, HireDate = new DateTime(2020, 8, 10) }
            };

            Employees.AddRange(employees);
            SaveChanges();
        }
    }
}
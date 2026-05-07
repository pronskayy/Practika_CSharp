using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Newtonsoft.Json;

namespace EmployeeManagement.Services
{
    public class UserService
    {
        private readonly string _filePath;

        public UserService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        }

        public async Task<List<UserModel>> LoadUsersAsync()
        {
            return await Task.Run(async () =>
            {
                if (!File.Exists(_filePath))
                {
                    var defaults = GetDefaultUsers();
                    await SaveUsersAsync(defaults);
                    return defaults;
                }
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<UserModel>>(json) ?? new List<UserModel>();
            });
        }

        public async Task SaveUsersAsync(List<UserModel> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<UserModel?> AuthenticateAsync(string username, string password)
        {
            var users = await LoadUsersAsync();
            var hash = ComputeHash(password);
            return users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);
        }

        public async Task<bool> RegisterAsync(string username, string password, string role, string department)
        {
            var users = await LoadUsersAsync();
            if (users.Any(u => u.Username == username))
                return false;

            users.Add(new UserModel
            {
                Id = users.Count + 1,
                Username = username,
                PasswordHash = ComputeHash(password),
                Role = role,
                Department = department
            });

            await SaveUsersAsync(users);
            return true;
        }

        public string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private List<UserModel> GetDefaultUsers()
        {
            return new List<UserModel>
            {
                new UserModel { Id = 1, Username = "admin", PasswordHash = ComputeHash("admin123"), Role = "Admin", Department = "IT" },
                new UserModel { Id = 2, Username = "user", PasswordHash = ComputeHash("user123"), Role = "User", Department = "HR" }
            };
        }
    }
}
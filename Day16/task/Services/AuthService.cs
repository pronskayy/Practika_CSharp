using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;  // Убедитесь, что эта директива есть
using Day15.Models;

namespace Day15.Services
{
    public class AuthService
    {
        private readonly string _usersFile = "Data/users.json";
        private List<User> _users;

        public AuthService()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(_usersFile))
            {
                var json = File.ReadAllText(_usersFile);
                _users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            else
            {
                _users = new List<User>
                {
                    new User { Id = 1, Username = "admin", Password = "admin123", Department = "Управление", IsAdmin = true },
                    new User { Id = 2, Username = "ivanov", Password = "123", Department = "IT отдел", IsAdmin = false },
                    new User { Id = 3, Username = "petrov", Password = "123", Department = "Управление", IsAdmin = false }
                };
                SaveUsers();
            }
        }

        private void SaveUsers()
        {
            Directory.CreateDirectory("Data");
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(_users, options);
            File.WriteAllText(_usersFile, json);
        }

        public User Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool Register(string username, string password, string department)
        {
            if (_users.Any(u => u.Username == username))
                return false;

            var newUser = new User
            {
                Id = _users.Count + 1,
                Username = username,
                Password = password,
                Department = department,
                IsAdmin = false
            };
            _users.Add(newUser);
            SaveUsers();
            return true;
        }
    }
}
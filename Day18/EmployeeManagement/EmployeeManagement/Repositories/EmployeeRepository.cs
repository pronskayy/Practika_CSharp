using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Data;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories
{
    /// <summary>
    /// EmployeeRepository с CRUD операциями через Entity Framework Core (_context)
    /// Выполняет условие задания: GetEmployeesAsync, AddEmployeeAsync, UpdateEmployeeAsync, 
    /// DeleteEmployeeAsync, SaveChangesAsync
    /// </summary>
    public class EmployeeRepository : IRepository<EmployeeModel>
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. GetEmployeesAsync() - из условия задания
        public async Task<List<EmployeeModel>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        // 2. AddEmployeeAsync() - из условия задания
        public async Task AddAsync(EmployeeModel entity)
        {
            await _context.Employees.AddAsync(entity);
        }

        // 3. UpdateEmployeeAsync() - из условия задания
        public Task UpdateAsync(EmployeeModel entity)
        {
            _context.Employees.Update(entity);
            return Task.CompletedTask;
        }

        // 4. DeleteEmployeeAsync() - из условия задания
        public Task DeleteAsync(EmployeeModel entity)
        {
            _context.Employees.Remove(entity);
            return Task.CompletedTask;
        }

        // 5. SaveChangesAsync() - "вызывать await _context.SaveChangesAsync()" - из условия задания
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
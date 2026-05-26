using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeManagementSystem.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    public EmployeeService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .ToListAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        await ValidateEmployeeReferencesAsync(employee.DepartmentId, employee.JobTitleId);
        await EnsureEmailIsUniqueAsync(employee.Email);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
    {
        var existingEmployee = await _context.Employees.FindAsync(id);

        if (existingEmployee == null)
        {
            return false;
        }

        await ValidateEmployeeReferencesAsync(employee.DepartmentId, employee.JobTitleId);
        await EnsureEmailIsUniqueAsync(employee.Email, id);

        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.Email = employee.Email;
        existingEmployee.Position = employee.Position;
        existingEmployee.DepartmentId = employee.DepartmentId;
        existingEmployee.JobTitleId = employee.JobTitleId;
        existingEmployee.Salary = employee.Salary;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if(employee == null)
        {
            return false;
        }
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    //Filtering
    public async Task<List<Employee>> FilterEmployeesAsync
    (
        string? keyword,
        int? departmentId,
        int? jobTitleId,
        decimal? minSalary,
        decimal? maxSalary
    )
    {
        var query = _context.Employees
            .Include(e => departmentId)
            .Include(e => jobTitleId)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(e =>
                e.FirstName.Contains(keyword) ||
                e.LastName.Contains(keyword) ||
                e.Email.Contains(keyword) ||
                e.Position.Contains(keyword)
            );
        }
        if(departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId);
            }
            if(jobTitleId.HasValue)
            {
                query = query.Where(e => e.JobTitleId == jobTitleId);
            }
            if(minSalary.HasValue)
            {
                query = query.Where(e => e.Salary == minSalary);
            }
            if(maxSalary.HasValue)
            {
                query = query.Where(e => e.Salary == maxSalary);
            }
        return await query.ToListAsync();
    }

    public async Task<(List<Employee> Employees, int TotalRecords)> GetPagedEmployeesAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .AsQueryable();
        
        var totalRecords = await query.CountAsync();

        var employees = await query
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (employees, totalRecords);
    }

    public async Task<List<EmployeeByDepartmentDto>> GetEmployeeByDepartmentStoredProcedureAsync(int departmentId)
    {
        var employees = new List<EmployeeByDepartmentDto>();

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("GetEmployeesByDepartment", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@DepartmentId", departmentId);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(new EmployeeByDepartmentDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                JobTitle = reader.GetString(reader.GetOrdinal("JobTitle")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
            }
            );
        }

        return employees;
    }

    private async Task ValidateEmployeeReferencesAsync(int departmentId, int jobTitleId)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId);
        
        if (!departmentExists)
        {
            throw new ArgumentException("Department does not exist");
        }

        var jobTitleExists = await _context.JobTitles
            .AnyAsync(j => j.Id == jobTitleId);
        
        if (!jobTitleExists)
        {
            throw new ArgumentException("Job title does not exist");
        }
    }

    private async Task EnsureEmailIsUniqueAsync(string email, int? employeeIdToExclude = null)
    {
        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == email && e.Id != employeeIdToExclude);

        if (emailExists)
        {
            throw new ArgumentException("An employee with this email already exists.");
        }
    }
}
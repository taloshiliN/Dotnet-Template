using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Services;
public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;
    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments
            .Include(e => e.DepartmentName)
            .Include(e => e.Description)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Department> CreateDepartmentAsync(Department department)
    {
        await EnsureDepartmentNameIsUniqueAsync(department.DepartmentName);

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<bool> UpdateDepartmentAsync(int id, Department department)
    {
        var existingDepartment = await _context.Departments.FindAsync(id);
        if (existingDepartment == null)
        {
            return false;
        }

        await EnsureDepartmentNameIsUniqueAsync(department.DepartmentName, id);

        existingDepartment.DepartmentName = department.DepartmentName;
        existingDepartment.Description = department.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return false;
        }

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id);
        
        if (hasEmployees)
        {
            throw new InvalidOperationException("Cannot delete department with assigned employees.");
        }
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }

    // public async Task<List<Department>> FilterDepartmentsAsync
    // (
    //     string? keyword
    // )
    // {
    //     var query = _context.Departments
    //         .AsQueryable();

    //     if (!string.IsNullOrWhiteSpace(keyword))
    //     {
    //         query = query.Where(e => 
    //             e.DepartmentName.Contains(keyword)
    //         );
    //     }
    //     return await query.ToListAsync();
    // }

    private async Task EnsureDepartmentNameIsUniqueAsync(string departmentName, int? departmentIdToExclude = null)
    {
        var nameExists = await _context.Departments
            .AnyAsync(d => d.DepartmentName == departmentName && d.Id != departmentIdToExclude);

        if (nameExists)
        {
            throw new InvalidOperationException("A department with that name already exists.");
        }
    }
}
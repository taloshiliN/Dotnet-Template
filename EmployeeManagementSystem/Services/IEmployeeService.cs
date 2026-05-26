using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs;
namespace EmployeeManagementSystem.Services;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(int id, Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<List<Employee>> FilterEmployeesAsync(
        string? keyword, 
        int? departmentId, 
        int? jobTitleId,
        decimal? minSalary,
        decimal? maxSalary
    );
    Task<(List<Employee> Employees, int TotalRecords)> GetPagedEmployeesAsync(
        int pageNumber,
        int pageSize
    );
    Task<List<EmployeeByDepartmentDto>> GetEmployeeByDepartmentStoredProcedureAsync(int departmentId);
}
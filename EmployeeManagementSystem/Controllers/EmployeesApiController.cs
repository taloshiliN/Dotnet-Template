using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagementSystem.DTOs;

namespace EmployeeManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/employees")]
public class EmployeeApiController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    public EmployeeApiController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();

        var result = employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Position = e.Position,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.DepartmentName,
            JobTitleId = e.JobTitleId,
            JobTitleName = e.JobTitle?.Title,
            Salary = e.Salary
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        var result = new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Position = employee.Position,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.DepartmentName,
            JobTitleId = employee.JobTitleId,
            JobTitleName = employee.JobTitle?.Title,
            Salary = employee.Salary
        };
        return Ok(result);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> FilterEmployees
    (
        string? keyword,
        int? departmentId,
        int? jobTitleId,
        decimal? maxSalary,
        decimal? minSalary
    )
    {
        var employees = await _employeeService.FilterEmployeesAsync(
            keyword,
            departmentId,
            jobTitleId,
            maxSalary,
            minSalary
        );

        var result = employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Position = e.Position,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.DepartmentName,
            JobTitleId = e.JobTitleId,
            JobTitleName = e.JobTitle?.Title,
            Salary = e.Salary
        });
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedEmployees(
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        if (pageNumber < 1)
        {
            return BadRequest(new {message = "Page number must be greater than 0"});
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new {message = "Page size must be between 1 and 100"});
        }

        var (employees, totalRecords) = await _employeeService.GetPagedEmployeesAsync(
            pageNumber,
            pageSize
        );
        var employeeDtos = employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Position = e.Position,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.DepartmentName,
            JobTitleId = e.JobTitleId,
            JobTitleName = e.JobTitle?.Title,
            Salary = e.Salary
        }).ToList();

        var response = new PageResponse<EmployeeResponseDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            Data = employeeDtos
        };
        return Ok(response);
    }

    [HttpGet("by-department-sp/{departmentId}")]
    public async Task<IActionResult> GetEmployeesByDepartmentUsingStoredProcedure(int departmentId)
    {
        var employees = await _employeeService
            .GetEmployeeByDepartmentStoredProcedureAsync(departmentId);
        return Ok(employees);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDto employeeDto)
    {
        try
        {
            var employee = new Employee
        {
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Email = employeeDto.Email,
            Position = employeeDto.Position,
            DepartmentId = employeeDto.DepartmentId,
            JobTitleId = employeeDto.JobTitleId,
        };

        var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);

        return CreatedAtAction(
            nameof(GetEmployee),
            new {id = createdEmployee.Id},
            createdEmployee
        );
        } catch(ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto employeeDto)
    {
        try
        {
            var employee = new Employee
        {
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Email = employeeDto.Email,
            Position = employeeDto.Position,
            DepartmentId = employeeDto.DepartmentId,
            JobTitleId = employeeDto.JobTitleId,
        };
        var updated = await _employeeService.UpdateEmployeeAsync(id, employee);

        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
        } catch(ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);

        if(!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
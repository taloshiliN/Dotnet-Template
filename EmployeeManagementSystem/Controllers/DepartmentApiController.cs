using EmployeeManagementSystem.DTOs;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers;
[ApiController]
[Route("api/department")]
public class DepartmentApiController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    public DepartmentApiController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();

        var result = departments.Select(e => new DepartmentResponseDto
        {
            Id = e.Id,
            DepartmentName = e.DepartmentName,
            Description = e.Description
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var departments = await _departmentService.GetDepartmentByIdAsync(id);
        if (departments == null)
        {
            return NotFound();
        }

        var result = new DepartmentResponseDto
        {
            Id = departments.Id,
            DepartmentName = departments.DepartmentName,
            Description = departments.Description
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDepartment(CreateDepartmentDto departmentDto)
    {
        try
        {
            var department = new Department
            {
                DepartmentName = departmentDto.DepartmentName,
                Description = departmentDto.Description
            };
            var createdDepartment = await _departmentService.CreateDepartmentAsync(department);

            return CreatedAtAction(
                nameof(GetDepartment),
                new {id = createdDepartment.Id},
                createdDepartment
            );
        } catch (ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto departmentDto)
    {
        try
        {
            var department = new Department
            {
                DepartmentName = departmentDto.DepartmentName,
                Description = departmentDto.Description
            };
            var updated = await _departmentService.UpdateDepartmentAsync(id, department);

            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        } catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        } catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var deleted = await _departmentService.DeleteDepartmentAsync(id);

        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
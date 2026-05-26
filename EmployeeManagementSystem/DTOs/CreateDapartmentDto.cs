using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.DTOs;

public class CreateDepartmentDto
{
    [Required]
    public string DepartmentName {get; set;} = string.Empty;
    [Required]
    public string Description {get; set;} = string.Empty;
}
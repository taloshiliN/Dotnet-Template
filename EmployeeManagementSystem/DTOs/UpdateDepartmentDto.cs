using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.DTOs;
public class UpdateDepartmentDto
{
    [Required]
    public string DepartmentName {get; set;} = string.Empty;
    [Required]
    public string Description {get; set;} = string.Empty;
}
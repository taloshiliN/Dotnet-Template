using System.ComponentModel.DataAnnotations;
namespace EmployeeManagementSystem.DTOs;
public class CreateJobTitleDto
{
    [Required]
    public string TitleName {get; set;} = string.Empty;
    [Required]
    public string Description {get; set;} = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.DTOs;
public class UpdateEmployeeDto
{
    [Required]
    public string FirstName {get; set;} = string.Empty;
    [Required]
    public string LastName {get; set;} = string.Empty;
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
    [Required]
    public string Position {get; set;} = string.Empty;
    [Required]
    public int DepartmentId {get; set;}
    [Required]
    public int JobTitleId {get; set;}
    [Range(0, 1000000)]
    public decimal Salary {get; set;}
}
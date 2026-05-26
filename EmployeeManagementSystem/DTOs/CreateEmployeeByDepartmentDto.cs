namespace EmployeeManagementSystem.DTOs;

public class EmployeeByDepartmentDto
{
    public int Id {get; set;}
    public string FirstName {get;set;} = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}
namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id {get; set;}
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string Position {get; set;} = string.Empty;

        public int DepartmentId {get; set;}
        public Department? Department {get; set;}

        public int JobTitleId {get; set;}
        public JobTitle? JobTitle {get; set;}
        public decimal Salary {get; set;}
    }
}
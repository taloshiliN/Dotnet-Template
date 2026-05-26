using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services;
public interface IJobTitleService
{
    Task<List<JobTitle>> GetAllJobTitlesAsync();
    Task<JobTitle?> GetJobTitleByIdAsync(int id);
    Task<JobTitle> CreateJobTitleAsync(JobTitle jobTitle);
    Task<bool> UpdateJobTitleAsync(int id, JobTitle jobTitle);
    Task<bool> DeleteJobTitleAsync(int id);
}
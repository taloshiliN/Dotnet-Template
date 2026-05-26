using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Services;

public class JobTitleService : IJobTitleService
{
    private readonly ApplicationDbContext _context;
    public JobTitleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobTitle>> GetAllJobTitlesAsync()
    {
        return await _context.JobTitles.ToListAsync();
    }
    public async Task<JobTitle?> GetJobTitleByIdAsync(int id)
    {
        return await _context.JobTitles.FindAsync(id);
    }
    public async Task<JobTitle> CreateJobTitleAsync(JobTitle jobTitle)
    {
        _context.JobTitles.Add(jobTitle);
        await _context.SaveChangesAsync();
        return jobTitle;
    }
    public async Task<bool> UpdateJobTitleAsync(int id, JobTitle jobTitle)
    {
        var existingJobTitle = await _context.JobTitles.FindAsync(id);
        if(existingJobTitle == null)
        {
            return false;
        }

        await EnsureJobTitleNameIsUniqueAsync(jobTitle.Title, id);

        existingJobTitle.Title = jobTitle.Title;
        existingJobTitle.Description = jobTitle.Description;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteJobTitleAsync(int id)
    {
        var jobtitle = await _context.JobTitles.FindAsync(id);
        if(jobtitle == null)
        {
            return false;
        }

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.JobTitleId == id);
        
        if (hasEmployees)
        {
            throw new InvalidOperationException("Cannot delete job title with assigned employees.");
        }

        _context.JobTitles.Remove(jobtitle);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task EnsureJobTitleNameIsUniqueAsync(string title, int? jobTitleIdToExclude = null)
    {
        var titleExists = await _context.JobTitles
            .AnyAsync(j => j.Title == title && j.Id != jobTitleIdToExclude);

        if (titleExists)
        {
            throw new InvalidOperationException("A job title with that name already exists.");
        }
    }
}
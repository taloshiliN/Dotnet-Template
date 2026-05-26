using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.DTOs;

namespace EmployeeManagementSystem.Controllers;
[ApiController]
[Route("api/jobtitle")]
public class JobTitleController : ControllerBase
{
    private readonly IJobTitleService _jobtitleService;

    public JobTitleController(IJobTitleService jobTitleService)
    {
        _jobtitleService = jobTitleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobTitle()
    {
        var jobtitles =await _jobtitleService.GetAllJobTitlesAsync();

        var result = jobtitles.Select(e => new JobTitleResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description
        });
        return Ok(result);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> GetJobTitle(int id)
    {
        try
        {
            var jobtitles = await _jobtitleService.GetJobTitleByIdAsync(id);
            if(jobtitles == null)
            {
                return NotFound();
            }
        
            var result = new JobTitleResponseDto
            {
                Id = jobtitles.Id,
                Title = jobtitles.Title,
                Description = jobtitles.Description
            };
            return Ok(result);
        } catch (ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        } catch (InvalidOperationException ex)
        {
            return Conflict(new {message = ex.Message});
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobTitle(CreateJobTitleDto jobTitleDto)
    {
        try
        {
            var jobTitle = new JobTitle
            {
                Title = jobTitleDto.TitleName,
                Description = jobTitleDto.Description
            };
            var createdJobtitles = await _jobtitleService.CreateJobTitleAsync(jobTitle);
        
            return CreatedAtAction(
                nameof(GetJobTitle),
                new {id = createdJobtitles.Id},
                createdJobtitles
            );
        } catch (ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobTitle(int id, UpdateJobTitleDto jobTitleDto)
    {
        var jobTitle = new JobTitle
        {
            Title = jobTitleDto.TitleName,
            Description = jobTitleDto.Description
        };
        var updated = await _jobtitleService.UpdateJobTitleAsync(id, jobTitle);
        if(!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletedJobTitle(int id)
    {
        var deleted = await _jobtitleService.DeleteJobTitleAsync(id);
        if(!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
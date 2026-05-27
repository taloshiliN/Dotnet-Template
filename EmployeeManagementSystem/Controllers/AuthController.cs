using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagementSystem.DTOs;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if(existingUser != null)
        {
            return BadRequest(new {message = "User with this email already exists"});
        }

        var allowedRoles = new[] {"Admin","HR","Employee"};

        if (!allowedRoles.Contains(registerDto.Role))
        {
            return BadRequest(new {message = "Invalid Role. Allowed roles are admin, hr and employee"});
        }

        if (!await _roleManager.RoleExistsAsync(registerDto.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(registerDto.Role));
        }

        var user = new ApplicationUser
        {
            FullName = registerDto.FullName,
            UserName = registerDto.Email,
            Email = registerDto.Email,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        await _userManager.AddToRoleAsync(user, registerDto.Role);
        return Ok(new{ message = "User registered successfully"});
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return Unauthorized(new {message = "Invalid email or password"});
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!passwordValid)
        {
            return Unauthorized(new {message = "Invalid email or password"});
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = GeneralJwtToken(user, roles.ToList());

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles.ToList()
        });
    }
    private string GeneralJwtToken(ApplicationUser user, List<string> roles)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var expiresInMinutes = Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
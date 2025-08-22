using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NurseScheduler.Api.Data;
using NurseScheduler.Api.Data.Models;
using NurseScheduler.Api.DTOs;
using NurseScheduler.Api.Services;

namespace NurseScheduler.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return BadRequest("Email already exists.");
        }

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Role = registerDto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(UserLoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _tokenService.CreateToken(user);

        return Ok(new LoginResponseDto(token, user.Name, user.Role));
    }
}
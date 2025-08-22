using System.ComponentModel.DataAnnotations;

namespace NurseScheduler.Api.DTOs
{
    public record UserRegisterDto([Required] string Name, [Required][EmailAddress] string Email, [Required] string Password, [Required] string Role);
    public record UserLoginDto([Required][EmailAddress] string Email, [Required] string Password);
    public record LoginResponseDto(string Token, string Name, string Role);
}
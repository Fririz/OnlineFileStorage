using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.DTO;

public class UserAuthDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
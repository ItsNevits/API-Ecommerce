using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class UserRegisterDto
{
    public string? Name { get; set; }
    [Required(ErrorMessage = "El campo username es requerido")]
    public string Username { get; set; } = null!;
    [Required(ErrorMessage = "El campo password es requerido")]
    public string Password { get; set; } = null!;
    public string? Role { get; set; }
}

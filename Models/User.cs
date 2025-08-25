using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models;

public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = null!;
    public string? Role { get; set; } = "User";
}

namespace ApiEcommerce.Models.Dtos;

public class UserDto
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Role { get; set; } = "User";
}

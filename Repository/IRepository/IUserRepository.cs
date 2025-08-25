using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();
    ApplicationUser? GetUser(Guid userId);
    bool UserExists(string username);
    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    Task<UserDataDto> CreateUser(CreateUserDto createUserDto);
}

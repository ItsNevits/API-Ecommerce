using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<UserDataDto> CreateUser(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.Username) || string.IsNullOrEmpty(createUserDto.Password))
        {
            throw new ArgumentNullException("Username or password is empty");
        }

        var user = new ApplicationUser
        {
            UserName = createUserDto.Username,
            Name = createUserDto.Name,
            Email = createUserDto.Username, // Assuming username is the email
            NormalizedUserName = createUserDto.Username.ToUpper(),
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded) throw new Exception("Error creating user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        var userRole = createUserDto.Role ?? "User";
        if (!await _roleManager.RoleExistsAsync(userRole)) await _roleManager.CreateAsync(new IdentityRole(userRole));
        await _userManager.AddToRoleAsync(user, userRole);
        var createdUser = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == createUserDto.Username.ToLower().Trim());
        if (createdUser == null) throw new Exception("Error retrieving created user.");

        return createdUser.Adapt<UserDataDto>();
    }

    public ApplicationUser? GetUser(Guid userId)
    {
        ApplicationUser? user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId.ToString());
        if (user == null) return null;

        return user;
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username) || string.IsNullOrEmpty(userLoginDto.Password))
        {
            return new UserLoginResponseDto
            {
                Token = string.Empty,
                User = null,
                Message = "Username or password is empty"
            };
        }

        var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto
            {
                Token = string.Empty,
                User = null,
                Message = "User not found"
            };
        }

        if(userLoginDto.Password == null)
        {
            return new UserLoginResponseDto
            {
                Token = string.Empty,
                User = null,
                Message = "Password is null"
            };
        }

        bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!isValid)
        {
            return new UserLoginResponseDto
            {
                Token = string.Empty,
                User = null,
                Message = "Credentials are not valid"
            };
        }

        // JWT Token Generation
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Secret key is not configured.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = "ApiEcommerce",
            Audience = "ApiEcommerce",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto
        {
            Token = handlerToken.WriteToken(token),
            User = user.Adapt<UserDataDto>(),
            Message = "Login successful"
        };
    }

    public bool UserExists(string username)
    {
        return _db.ApplicationUsers.Any(u => u.UserName != null && u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }
}

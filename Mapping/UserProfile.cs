using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public static class UserProfile
{
    public static void RegisterUserMappings()
    {
        TypeAdapterConfig<User, UserDto>.NewConfig();
        TypeAdapterConfig<User, CreateUserDto>.NewConfig();
        TypeAdapterConfig<User, UserRegisterDto>.NewConfig();
        TypeAdapterConfig<User, UserLoginDto>.NewConfig();
        TypeAdapterConfig<User, UserLoginResponseDto>.NewConfig();

        TypeAdapterConfig<ApplicationUser, UserDataDto>.NewConfig()
            .Map(dest => dest.Username, src => src.UserName);
        TypeAdapterConfig<ApplicationUser, UserDto>.NewConfig()
            .Map(dest => dest.Username, src => src.UserName)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Password, src => src.PasswordHash);
    }
}

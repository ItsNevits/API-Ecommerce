using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public static class CategoryProfile
{
    public static void RegisterCategoryMappings()
    {
        TypeAdapterConfig<Category, CategoryDto>.NewConfig();
        TypeAdapterConfig<CreateCategoryDto, Category>.NewConfig();
    }
}

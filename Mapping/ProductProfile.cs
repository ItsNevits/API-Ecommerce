using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class ProductProfile
{
    public static void RegisterProductMappings()
    {
        TypeAdapterConfig<Product, ProductDto>
            .NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category.Name);
        TypeAdapterConfig<CreateProductDto, Product>.NewConfig();
        TypeAdapterConfig<UpdateProductDto, Product>.NewConfig();
    }
}

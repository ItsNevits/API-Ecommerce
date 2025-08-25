namespace ApiEcommerce.Models.Dtos;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageUrlLocal { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int Stock { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public Guid CategoryId { get; set; }
}

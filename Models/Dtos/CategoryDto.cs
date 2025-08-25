namespace ApiEcommerce.Models.Dtos;

public class CategoryDto
{
    public Guid CategoryId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

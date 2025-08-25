using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models;

public class Product
{
    [Key]
    public Guid ProductId { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ImageUrlLocal { get; set; }
    [Required]
    public string SKU { get; set; } = string.Empty;
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

    public Guid CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public required Category Category { get; set; }
}

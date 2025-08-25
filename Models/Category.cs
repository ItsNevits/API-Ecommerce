using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public Guid CategoryId { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}
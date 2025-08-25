namespace ApiEcommerce.Repository.IRepository;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();
    Category? GetCategory(Guid categoryId);
    bool CategoryExists(string name);
    bool CategoryExists(Guid categoryId);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(Category category);
    bool Save();
}

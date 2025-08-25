using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();
    ICollection<Product> GetProductsInPages(int pageNumber, int pageSize);
    int GetTotalProducts();
    Product? GetProduct(Guid productId);
    ICollection<Product> GetProductsInCategory(Guid categoryId);
    ICollection<Product> SearchProducts(string searchText);
    bool BuyProduct(Guid id, int quantity);
    bool ProductExists(string name);
    bool ProductExists(Guid productId);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}

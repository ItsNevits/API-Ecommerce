using ApiEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository.IRepository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
    }

    public Product? GetProduct(Guid productId)
    {
        if (productId == Guid.Empty) return null;
        return _db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == productId);
    }

    public ICollection<Product> GetProductsInCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) return new List<Product>();
        return _db.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> SearchProducts(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return new List<Product>();
        return _db.Products.Include(p => p.Category).Where(
            p => p.Name.ToLower().Contains(searchText.ToLower()) ||
            p.Description != null && p.Description.ToLower().Contains(searchText.ToLower()))
        .OrderBy(p => p.Name).ToList();
    }

    public bool BuyProduct(Guid id, int quantity)
    {
        if(id == Guid.Empty || quantity <= 0) return false;

        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null || product.Stock < quantity) return false;

        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }

    public bool ProductExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool ProductExists(Guid productId)
    {
        if (productId == Guid.Empty) return false;
        return _db.Products.Any(p => p.ProductId == productId);
    }

    public bool CreateProduct(Product product)
    {
        if (product == null) return false;
        
        _db.Products.Add(product);
        return Save();
    }

    public bool UpdateProduct(Product product)
    {
        product.UpdatedOn = DateTime.UtcNow;
        _db.Products.Update(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        _db.Products.Remove(product);
        return Save();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize)
    {
        return _db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int GetTotalProducts()
    {
        return _db.Products.Count();
    }
}

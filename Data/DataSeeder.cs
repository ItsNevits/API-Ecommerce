using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext appContext)
    {
        // Seeding de Roles
        if (!appContext.Roles.Any())
        {
            appContext.Roles.AddRange(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );
        }

        // Crear GUIDs específicos para las categorías
        var ropaAccesoriosId = Guid.NewGuid();
        var electronicosId = Guid.NewGuid();
        var deportesId = Guid.NewGuid();
        var hogarId = Guid.NewGuid();
        var librosId = Guid.NewGuid();

        // Crear las categorías primero
        var categorias = new List<Category>();

        // Seeding de Categorías
        if (!appContext.Categories.Any())
        {
            categorias = new List<Category>
            {
                new Category { CategoryId = ropaAccesoriosId, Name = "Ropa y accesorios", CreatedOn = DateTime.Now },
                new Category { CategoryId = electronicosId, Name = "Electrónicos", CreatedOn = DateTime.Now },
                new Category { CategoryId = deportesId, Name = "Deportes", CreatedOn = DateTime.Now },
                new Category { CategoryId = hogarId, Name = "Hogar", CreatedOn = DateTime.Now },
                new Category { CategoryId = librosId, Name = "Libros", CreatedOn = DateTime.Now }
            };

            appContext.Categories.AddRange(categorias);
            appContext.SaveChanges(); // Guardar categorías primero
        }
        else
        {
            // Si ya existen, obtenerlas de la base de datos
            categorias = appContext.Categories.ToList();
        }

        // Seeding de Usuario Administrador
        if (!appContext.ApplicationUsers.Any())
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "admin-001",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                Name = "Administrador"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

            var regularUser = new ApplicationUser
            {
                Id = "user-001",
                UserName = "user@user.com",
                NormalizedUserName = "USER@USER.COM",
                Email = "user@user.com",
                NormalizedEmail = "USER@USER.COM",
                EmailConfirmed = true,
                Name = "Usuario Regular"
            };
            regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

            appContext.ApplicationUsers.AddRange(adminUser, regularUser);
        }

        // Seeding de UserRoles
        if (!appContext.UserRoles.Any())
        {
            appContext.UserRoles.AddRange(
                new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
                new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
            );
        }

        // Seeding de Productos
        if (!appContext.Products.Any())
        {
            appContext.Products.AddRange(
                new Product
                {
                    Name = "Camiseta Básica",
                    Description = "Camiseta de algodón 100%",
                    Price = 25.99m,
                    SKU = "PROD-001-CAM-M",
                    Stock = 50,
                    CategoryId = ropaAccesoriosId,
                    Category = categorias.First(c => c.CategoryId == ropaAccesoriosId),
                    ImageUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=Camiseta",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Smartphone Galaxy",
                    Description = "Teléfono inteligente con 128GB",
                    Price = 599.99m,
                    SKU = "PROD-002-PHO-BLK",
                    Stock = 25,
                    CategoryId = electronicosId,
                    Category = categorias.First(c => c.CategoryId == electronicosId),
                    ImageUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Pelota de Fútbol",
                    Description = "Pelota oficial FIFA",
                    Price = 45.00m,
                    SKU = "PROD-003-BAL-WHT",
                    Stock = 30,
                    CategoryId = deportesId,
                    Category = categorias.First(c => c.CategoryId == deportesId),
                    ImageUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Pelota",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Lámpara de Mesa",
                    Description = "Lámpara LED regulable",
                    Price = 89.99m,
                    SKU = "PROD-004-LAM-WHT",
                    Stock = 15,
                    CategoryId = hogarId,
                    Category = categorias.First(c => c.CategoryId == hogarId),
                    ImageUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lampara",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "El Quijote",
                    Description = "Novela clásica de Cervantes",
                    Price = 19.99m,
                    SKU = "PROD-005-LIB-ESP",
                    Stock = 100,
                    CategoryId = librosId,
                    Category = categorias.First(c => c.CategoryId == librosId),
                    ImageUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Libro",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Jeans Clásicos",
                    Description = "Pantalones vaqueros azules",
                    Price = 79.99m,
                    SKU = "PROD-006-PAN-BLU",
                    Stock = 40,
                    CategoryId = ropaAccesoriosId,
                    Category = categorias.First(c => c.CategoryId == ropaAccesoriosId),
                    ImageUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Tablet Pro",
                    Description = "Tablet 10.5 pulgadas con stylus incluido",
                    Price = 459.99m,
                    SKU = "PROD-007-TAB-SIL",
                    Stock = 20,
                    CategoryId = electronicosId,
                    Category = categorias.First(c => c.CategoryId == electronicosId),
                    ImageUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Zapatillas Running",
                    Description = "Zapatillas deportivas para correr",
                    Price = 129.99m,
                    SKU = "PROD-008-ZAP-BLK",
                    Stock = 35,
                    CategoryId = deportesId,
                    Category = categorias.First(c => c.CategoryId == deportesId),
                    ImageUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Zapatillas",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Cafetera Express",
                    Description = "Cafetera automática con molinillo integrado",
                    Price = 299.99m,
                    SKU = "PROD-009-CAF-BLK",
                    Stock = 12,
                    CategoryId = hogarId,
                    Category = categorias.First(c => c.CategoryId == hogarId),
                    ImageUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Cafetera",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Programación en C#",
                    Description = "Guía completa de programación en C# y .NET",
                    Price = 49.99m,
                    SKU = "PROD-010-LIB-ESP",
                    Stock = 80,
                    CategoryId = librosId,
                    Category = categorias.First(c => c.CategoryId == librosId),
                    ImageUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Chaqueta Deportiva",
                    Description = "Chaqueta impermeable para actividades al aire libre",
                    Price = 149.99m,
                    SKU = "PROD-011-CHA-NAV",
                    Stock = 28,
                    CategoryId = ropaAccesoriosId,
                    Category = categorias.First(c => c.CategoryId == ropaAccesoriosId),
                    ImageUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Chaqueta",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Name = "Auriculares Bluetooth",
                    Description = "Auriculares inalámbricos con cancelación de ruido",
                    Price = 189.99m,
                    SKU = "PROD-012-AUR-BLK",
                    Stock = 45,
                    CategoryId = electronicosId,
                    Category = categorias.First(c => c.CategoryId == electronicosId),
                    ImageUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Auriculares",
                    CreatedOn = DateTime.Now
                }
            );
        }
        // Solo hacer SaveChanges final para productos
        if (!appContext.Products.Any())
        {
            appContext.SaveChanges();
        }
    }
}

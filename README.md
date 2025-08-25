# ApiEcommerce - E-commerce REST API

Una API RESTful completa para un sistema de e-commerce desarrollada con .NET 8, Entity Framework Core y SQL Server.

## 🚀 Características

- **Autenticación y Autorización**: JWT Bearer Token con Identity Framework
- **Gestión de Productos**: CRUD completo con soporte para imágenes
- **Gestión de Categorías**: Organización de productos por categorías
- **Gestión de Usuarios**: Registro, login y administración de usuarios
- **Subida de Archivos**: Servicio genérico para manejo de imágenes
- **Versionado de API**: Soporte para múltiples versiones
- **Documentación**: Swagger/OpenAPI integrado
- **CORS**: Configurado para desarrollo web
- **Caché**: Implementación de response caching
- **Mapeo de Objetos**: Mapster para conversión entre DTOs y entidades

## 🛠️ Tecnologías Utilizadas

- **.NET 8.0**: Framework principal
- **ASP.NET Core**: Web API
- **Entity Framework Core**: ORM
- **SQL Server**: Base de datos
- **Identity Framework**: Autenticación y autorización
- **JWT**: Tokens de autenticación
- **Mapster**: Mapeo de objetos
- **Swagger**: Documentación de API
- **BCrypt**: Hash de contraseñas

## 📋 Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, o completo)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

## 🔧 Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd ApiEcommerce
```

### 2. Configurar la base de datos
Crear un archivo `appsettings.local.json` en la raíz del proyecto con tu configuración:

```json
{
  "ApiSettings": {
    "SecretKey": "tu-clave-secreta-jwt-de-al-menos-32-caracteres"
  },
  "ConnectionStrings": {
    "ConexionSql": "Server=localhost;Database=ApiEcommerceNET8;User ID=SA;Password=TuPassword;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Restaurar dependencias
```bash
dotnet restore
```

### 4. Ejecutar migraciones
```bash
dotnet ef database update
```

### 5. Ejecutar la aplicación
```bash
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📖 Estructura del Proyecto

```
ApiEcommerce/
├── Controllers/           # Controladores de la API
│   ├── CategoryController.cs
│   ├── ProductController.cs
│   └── UserController.cs
├── Data/                 # Contexto de base de datos
│   └── ApplicationDbContext.cs
├── Models/               # Entidades del dominio
│   ├── Category.cs
│   ├── Product.cs
│   ├── User.cs
│   ├── ApplicationUser.cs
│   └── Dtos/            # Data Transfer Objects
├── Repository/           # Patrón Repository
│   ├── IRepository/     # Interfaces
│   └── [Implementaciones]
├── Services/            # Servicios de la aplicación
│   ├── IFileService.cs
│   └── FileService.cs
├── Mapping/             # Configuración de Mapster
├── Migrations/          # Migraciones de EF Core
├── Constants/           # Constantes de la aplicación
└── wwwroot/            # Archivos estáticos
    └── ProductsImages/ # Imágenes de productos
```

## 🔐 Autenticación

La API utiliza JWT Bearer tokens para autenticación. Para acceder a endpoints protegidos:

### 1. Registrar un usuario
```http
POST /api/v1/user
Content-Type: application/json

{
  "name": "Usuario Admin",
  "username": "admin@example.com",
  "password": "Password123!",
  "role": "Admin"
}
```

### 2. Iniciar sesión
```http
POST /api/v1/user/login
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "Password123!"
}
```

### 3. Usar el token
Incluir el token en el header Authorization:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📚 Endpoints Principales

### Categorías
- `GET /api/v1/category` - Listar categorías (público)
- `GET /api/v1/category/{id}` - Obtener categoría (público)
- `POST /api/v1/category` - Crear categoría (Admin)
- `PATCH /api/v1/category/{id}` - Actualizar categoría (Admin)
- `DELETE /api/v1/category/{id}` - Eliminar categoría (Admin)

### Productos
- `GET /api/v1/product` - Listar productos (público)
- `GET /api/v1/product/{id}` - Obtener producto (público)
- `POST /api/v1/product` - Crear producto (Admin)
- `PUT /api/v1/product/{id}` - Actualizar producto (Admin)
- `DELETE /api/v1/product/{id}` - Eliminar producto (Admin)
- `PATCH /api/v1/product/buy/{id}/{quantity}` - Comprar producto
- `GET /api/v1/product/search/{searchText}` - Buscar productos (público)
- `GET /api/v1/product/search/category/{id}` - Productos por categoría

### Usuarios
- `GET /api/v1/user` - Listar usuarios (Admin)
- `GET /api/v1/user/{id}` - Obtener usuario (Admin)
- `POST /api/v1/user` - Registrar usuario (público)
- `POST /api/v1/user/login` - Iniciar sesión (público)

## 🔒 Roles y Permisos

- **Admin**: Acceso completo a todas las operaciones
- **Customer**: Acceso limitado (comprar productos, ver catálogo)

## 📁 Subida de Archivos

Los productos pueden incluir imágenes que se suben usando `multipart/form-data`:

```http
POST /api/v1/product
Content-Type: multipart/form-data

{
  "name": "Producto ejemplo",
  "description": "Descripción del producto",
  "price": 99.99,
  "stock": 10,
  "categoryId": "guid-de-categoria",
  "imageFile": [archivo_imagen]
}
```

**Formatos soportados**: JPG, JPEG, PNG, GIF, WEBP  
**Tamaño máximo**: 5MB

## 🧪 Testing

Ejecutar tests unitarios:
```bash
dotnet test
```

## 📊 Base de Datos

El proyecto utiliza Code First con Entity Framework Core. Las migraciones se encuentran en la carpeta `Migrations/`.

### Crear nueva migración
```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Aplicar migraciones
```bash
dotnet ef database update
```

## 🌐 CORS

CORS está configurado para permitir requests desde `http://localhost:3000` (para desarrollo con React, Angular, etc.).

Para modificar los orígenes permitidos, editar `PolicyNames.AllowSpecificOrigin` en `Program.cs`.

## 📝 Logging

El proyecto incluye logging configurado para Development y Production. Los logs se pueden ver en la consola durante el desarrollo.

## 🚀 Despliegue

### Variables de Entorno de Producción
- `ConnectionStrings__ConexionSql`: Cadena de conexión a la base de datos
- `ApiSettings__SecretKey`: Clave secreta para JWT

### Docker (Opcional)
```dockerfile
# Ejemplo de Dockerfile básico
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiEcommerce.csproj", "."]
RUN dotnet restore "ApiEcommerce.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ApiEcommerce.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiEcommerce.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiEcommerce.dll"]
```

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📞 Contacto

Tu Nombre - [bs.alvarado21@gmail.com](mailto:bs.alvarado21@gmail.com)

Link del Proyecto: [https://github.com/ItsNevits/ApiEcommerce](https://github.com/tu-usuario/API-Ecommerce)

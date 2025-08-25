using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
string strDbConnection = builder.Configuration.GetConnectionString("ConexionSql") ?? throw new InvalidOperationException("Connection string 'ConnectionStrings:ConexionSql' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(strDbConnection)
  .UseSeeding((context, _) =>
  {
    var appContext = (ApplicationDbContext)context;
    DataSeeder.SeedData(appContext);
  })
);

// Caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1 MB
    options.UseCaseSensitivePaths = true;
});

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IFileService, FileService>();

// Register AutoMapper mappings
ApiEcommerce.Mapping.CategoryProfile.RegisterCategoryMappings();
ApiEcommerce.Mapping.ProductProfile.RegisterProductMappings();
ApiEcommerce.Mapping.UserProfile.RegisterUserMappings();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Authentication
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrWhiteSpace(secretKey)) throw new InvalidOperationException("JWT Secret not found.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // en producción debería ser true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = "ApiEcommerce",
        ValidateAudience = true,
        ValidAudience = "ApiEcommerce"
    };
});

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(
        CacheProfiles.Default10,
        CacheProfiles.Profile10
    );
    options.CacheProfiles.Add(
        CacheProfiles.Default20,
        CacheProfiles.Profile20
    );
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                            "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                            "Ejemplo: \"12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ApiEcommerce",
            Description = "ASP.NET Core Web API para gestionar productos y usuarios.",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Soporte Técnico",
                Email = "example@mail.com",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Licencia de uso",
                Url = new Uri("https://example.com/license")
            }
        });
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2",
            Title = "ApiEcommerce",
            Description = "ASP.NET Core Web API para gestionar productos y usuarios.",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Soporte Técnico",
                Email = "example@mail.com",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Licencia de uso",
                Url = new Uri("https://example.com/license")
            }
        });
    }
);

// Versioning
var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    //option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); // ?api-version=1.0
});

apiVersioningBuilder.AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
        builder => builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiEcommerce v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiEcommerce v2");
    });
}

// Static files
app.UseStaticFiles();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors(PolicyNames.AllowSpecificOrigin);

// Enable Caching
app.UseResponseCaching();

// Enable Authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

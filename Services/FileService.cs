using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Services;

public class FileService : IFileService
{
    public async Task<string> UploadFileAsync<T>(IFormFile? file, Guid entityId, string folderName, ControllerBase controller) where T : class
    {
        if (file == null || file.Length == 0)
        {
            return GetDefaultImageUrl();
        }

        // Validar tipo de archivo
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException($"File type {fileExtension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }

        // Validar tamaño (5MB máximo)
        const int maxFileSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxFileSize)
        {
            throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");
        }

        // Generar nombre único para el archivo
        string fileName = $"{entityId}-{Guid.NewGuid()}{fileExtension}";
        
        // Crear directorio si no existe
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Ruta completa del archivo
        var filePath = Path.Combine(uploadsFolder, fileName);
        
        // Eliminar archivo existente si ya existe
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Guardar archivo
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Generar URL pública
        var baseUrl = $"{controller.Request.Scheme}://{controller.Request.Host.Value}{controller.Request.PathBase.Value}";
        return $"{baseUrl}/{folderName}/{fileName}";
    }

    public void DeleteFile(string? filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - file deletion shouldn't break the main operation
                Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
            }
        }
    }

    public string GetDefaultImageUrl()
    {
        return "https://placehold.co/300x300";
    }
}

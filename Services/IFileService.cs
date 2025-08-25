using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Services;

public interface IFileService
{
    Task<string> UploadFileAsync<T>(IFormFile? file, Guid entityId, string folderName, ControllerBase controller) where T : class;
    void DeleteFile(string? filePath);
    string GetDefaultImageUrl();
}

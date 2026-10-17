using Microsoft.AspNetCore.Http;

namespace Project_MARS.Interfaces
{
    public interface IFileManager
    {
        Task<string> SaveFileAsync(IFormFile file);
        void DeleteFile(string fileUrl);
    }
}

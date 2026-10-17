using Microsoft.AspNetCore.Hosting;
using Project_MARS.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Honda_Project.Services
{
    public class FileManager(IWebHostEnvironment environment) : IFileManager
    {
        private const string AssetsFolder = "Assets";
        private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg"];
        private static readonly string[] AllowedMimeTypes = ["image/png", "image/jpeg", "image/pjpeg"];
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("FileNull", nameof(file));
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Invalid file format ({extension}). Only PNG, JPG and JPEG are allowed.", nameof(file));
            }

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                throw new ArgumentException("Invalid file content type.", nameof(file));
            }


            var fileName = $"{Guid.NewGuid()}-{file.FileName}";
            var folder = Path.Combine(environment.WebRootPath, AssetsFolder);

            Directory.CreateDirectory(folder);

            var pathToSave = Path.Combine(folder, fileName);

            using var fileStream = new FileStream(pathToSave, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return $"/{AssetsFolder}/{fileName}";
        }
        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return;
            }

            var path = Path.Combine(environment.WebRootPath, fileUrl.TrimStart('/'));

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

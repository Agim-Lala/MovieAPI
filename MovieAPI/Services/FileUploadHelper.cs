namespace MovieAPI.Services;

public class FileUploadHelper
{
    private readonly IWebHostEnvironment _env;
    private const string CoverFolder = "Images/covers";

    public FileUploadHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            return null;

        // Ensure target folder exists
        var uploadsFolder = Path.Combine(_env.WebRootPath, folder);
        Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path for DB
        return $"/{folder}/{fileName}";
    }
    public async Task<string> SaveBase64ToFileAsync(string base64, string folder, string fileExtension)
    {
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        folder = folder.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);


        // Correct full path
        var folderPath = Path.Combine(_env.WebRootPath, folder);
        Directory.CreateDirectory(folderPath); 

        var fullPath = Path.Combine(folderPath, fileName);
        var fileBytes = Convert.FromBase64String(base64);
        await File.WriteAllBytesAsync(fullPath, fileBytes);

        // Return relative path for DB
        return "/" + Path.Combine(folder, fileName).Replace("\\", "/");
    }


}

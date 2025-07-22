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



}

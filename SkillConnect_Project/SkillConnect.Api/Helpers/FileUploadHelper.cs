public interface IFileUploadHelper
{
    Task<string> UploadResumeAsync(IFormFile file);
}
public class FileUploadHelper : IFileUploadHelper
{
    private readonly IWebHostEnvironment _environment;
    public FileUploadHelper(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    public async Task<string> UploadResumeAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
    }
}
using Microsoft.AspNetCore.Http;

namespace GaleriaArte.ObraService.Application.Services;

public class FileService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg" };
    private const int MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static async Task<string> ConvertToBase64Async(IFormFile file)
    {
        // Validar extensión
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Solo se permiten archivos JPG/JPEG");
        }

        // Validar tamaño
        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException("El archivo no puede ser mayor a 5MB");
        }

        // Validar que no esté vacío
        if (file.Length == 0)
        {
            throw new ArgumentException("El archivo está vacío");
        }

        // Convertir a base64
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        
        return Convert.ToBase64String(fileBytes);
    }

    public static byte[] ConvertFromBase64(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            throw new ArgumentException("La cadena base64 no puede estar vacía");
        }

        try
        {
            return Convert.FromBase64String(base64String);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Formato de base64 inválido");
        }
    }

    public static async Task<string> CreateTemporaryFileFromBase64Async(string base64String)
    {
        var fileBytes = ConvertFromBase64(base64String);
        var tempPath = Path.GetTempFileName();
        
        await File.WriteAllBytesAsync(tempPath, fileBytes);
        
        return tempPath;
    }
}

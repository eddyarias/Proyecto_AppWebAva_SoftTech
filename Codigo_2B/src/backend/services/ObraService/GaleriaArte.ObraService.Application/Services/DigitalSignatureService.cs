using System.Security.Cryptography;
using GaleriaArte.ObraService.Application.Interfaces;

namespace GaleriaArte.ObraService.Application.Services;

public class DigitalSignatureService : IDigitalSignatureService
{
    public string FirmarArchivo(string rutaArchivo, string clavePrivadaXml)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(rutaArchivo))
        using (var rsa = new RSACryptoServiceProvider())
        {
            byte[] hash = sha256.ComputeHash(stream);
            rsa.FromXmlString(clavePrivadaXml);
            byte[] firma = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
            return Convert.ToBase64String(firma);
        }
    }

    public bool ValidarFirma(string rutaArchivo, string firmaBase64, string clavePublicaXml)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(rutaArchivo))
        using (var rsa = new RSACryptoServiceProvider())
        {
            byte[] hash = sha256.ComputeHash(stream);
            byte[] firma = Convert.FromBase64String(firmaBase64);
            rsa.FromXmlString(clavePublicaXml);
            return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), firma);
        }
    }

    public async Task<string> FirmarArchivoAsync(string rutaArchivo, string clavePrivadaXml)
    {
        return await Task.Run(() => FirmarArchivo(rutaArchivo, clavePrivadaXml));
    }

    public async Task<bool> ValidarFirmaAsync(string rutaArchivo, string firmaBase64, string clavePublicaXml)
    {
        return await Task.Run(() => ValidarFirma(rutaArchivo, firmaBase64, clavePublicaXml));
    }

    // Métodos estáticos para compatibilidad con el código existente
    public static string FirmarArchivoStatic(string rutaArchivo, string clavePrivadaXml)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(rutaArchivo))
        using (var rsa = new RSACryptoServiceProvider())
        {
            byte[] hash = sha256.ComputeHash(stream);
            rsa.FromXmlString(clavePrivadaXml);
            byte[] firma = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
            return Convert.ToBase64String(firma);
        }
    }

    public static bool ValidarFirmaStatic(string rutaArchivo, string firmaBase64, string clavePublicaXml)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(rutaArchivo))
        using (var rsa = new RSACryptoServiceProvider())
        {
            byte[] hash = sha256.ComputeHash(stream);
            byte[] firma = Convert.FromBase64String(firmaBase64);
            rsa.FromXmlString(clavePublicaXml);
            return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), firma);
        }
    }
}

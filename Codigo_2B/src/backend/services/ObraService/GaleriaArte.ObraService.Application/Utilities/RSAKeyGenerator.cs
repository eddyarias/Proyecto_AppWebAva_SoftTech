using System.Security.Cryptography;

namespace GaleriaArte.ObraService.Application.Utilities;

public static class RSAKeyGenerator
{
    public static (string publicKey, string privateKey) GenerateKeyPair()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            string publicKey = rsa.ToXmlString(false);  // Solo clave pública
            string privateKey = rsa.ToXmlString(true);  // Clave privada y pública
            
            return (publicKey, privateKey);
        }
    }

    public static void GenerateAndSaveKeys(string publicKeyPath, string privateKeyPath)
    {
        var (publicKey, privateKey) = GenerateKeyPair();
        
        // Crear directorios si no existen
        Directory.CreateDirectory(Path.GetDirectoryName(publicKeyPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(privateKeyPath)!);
        
        // Guardar las claves
        File.WriteAllText(publicKeyPath, publicKey);
        File.WriteAllText(privateKeyPath, privateKey);
        
        Console.WriteLine($"Claves generadas exitosamente:");
        Console.WriteLine($"Clave pública: {publicKeyPath}");
        Console.WriteLine($"Clave privada: {privateKeyPath}");
    }
}

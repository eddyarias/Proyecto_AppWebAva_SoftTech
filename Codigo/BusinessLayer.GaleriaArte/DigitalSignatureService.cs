using System.IO;
using System.Security.Cryptography;

namespace BusinessLayer.GaleriaArte
{
    public class DigitalSignatureService
    {
        public static string FirmarArchivo(string rutaArchivo, string clavePrivadaXml)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(rutaArchivo))
            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] hash = sha256.ComputeHash(stream);
                rsa.FromXmlString(clavePrivadaXml);
                byte[] firma = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
                return System.Convert.ToBase64String(firma);
            }
        }

        public static bool ValidarFirma(string rutaArchivo, string firmaBase64, string clavePublicaXml)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(rutaArchivo))
            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] hash = sha256.ComputeHash(stream);
                byte[] firma = System.Convert.FromBase64String(firmaBase64);
                rsa.FromXmlString(clavePublicaXml);
                return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), firma);
            }
        }
    }
}

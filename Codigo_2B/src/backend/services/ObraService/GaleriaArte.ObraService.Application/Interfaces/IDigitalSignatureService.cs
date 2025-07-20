namespace GaleriaArte.ObraService.Application.Interfaces;

public interface IDigitalSignatureService
{
    string FirmarArchivo(string rutaArchivo, string clavePrivadaXml);
    bool ValidarFirma(string rutaArchivo, string firmaBase64, string clavePublicaXml);
    Task<string> FirmarArchivoAsync(string rutaArchivo, string clavePrivadaXml);
    Task<bool> ValidarFirmaAsync(string rutaArchivo, string firmaBase64, string clavePublicaXml);
}

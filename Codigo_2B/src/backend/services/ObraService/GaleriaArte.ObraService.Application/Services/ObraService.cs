using GaleriaArte.ObraService.Application.DTOs;
using GaleriaArte.ObraService.Domain.Entities;
using GaleriaArte.ObraService.Domain.Interfaces;
using GaleriaArte.ObraService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GaleriaArte.ObraService.Application.Services;

public class ObraService : IObraService
{
    private readonly IObraRepository _repositorio;
    private readonly IDigitalSignatureService _digitalSignatureService;
    private readonly IConfiguration _configuration;

    public ObraService(IObraRepository repositorio, IDigitalSignatureService digitalSignatureService, IConfiguration configuration)
    {
        _repositorio = repositorio;
        _digitalSignatureService = digitalSignatureService;
        _configuration = configuration;
    }

    public async Task<object> CrearObraAsync(CreateObraDto dto)
    {
        Console.WriteLine($"[DEBUG] Iniciando creación de obra: {dto.Titulo}");
        Console.WriteLine($"[DEBUG] Artista nickname: {dto.ArtistaNickname}");
        Console.WriteLine($"[DEBUG] Archivo recibido: {dto.Archivo?.FileName} ({dto.Archivo?.Length} bytes)");
        
        try
        {
            // Convertir archivo a base64
            Console.WriteLine("[DEBUG] Convirtiendo archivo a base64...");
            string archivoBase64 = await FileService.ConvertToBase64Async(dto.Archivo);
            Console.WriteLine($"[DEBUG] Archivo convertido a base64, tamaño: {archivoBase64.Length} caracteres");
            
            // Crear archivo temporal para la firma
            Console.WriteLine("[DEBUG] Creando archivo temporal...");
            string rutaArchivoTemporal = await FileService.CreateTemporaryFileFromBase64Async(archivoBase64);
            Console.WriteLine($"[DEBUG] Archivo temporal creado: {rutaArchivoTemporal}");
            
            try
            {
                // Obtener la clave privada desde la configuración
                Console.WriteLine("[DEBUG] Obteniendo configuración de clave privada...");
                string? rutaClavePrivada = _configuration["DigitalSignature:PrivateKeyPath"];
                Console.WriteLine($"[DEBUG] Ruta de clave privada configurada: {rutaClavePrivada}");
                
                if (string.IsNullOrEmpty(rutaClavePrivada))
                    throw new InvalidOperationException("No se ha configurado la ruta de la clave privada");

                if (!File.Exists(rutaClavePrivada))
                    throw new FileNotFoundException($"No se encontró el archivo de clave privada en: {rutaClavePrivada}");

                // Leer la clave privada
                Console.WriteLine("[DEBUG] Leyendo clave privada...");
                string clavePrivadaXml = await File.ReadAllTextAsync(rutaClavePrivada);

                // Generar la firma digital
                Console.WriteLine("[DEBUG] Generando firma digital...");
                string firmaDigital = await _digitalSignatureService.FirmarArchivoAsync(rutaArchivoTemporal, clavePrivadaXml);
                Console.WriteLine($"[DEBUG] Firma digital generada: {firmaDigital.Substring(0, Math.Min(50, firmaDigital.Length))}...");

                // Crear la obra con la firma
                Console.WriteLine("[DEBUG] Creando entidad Obra...");
                var obra = new Obra
                {
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    ArchivoBase64 = archivoBase64,
                    FirmaDigital = firmaDigital,
                    ArtistaNickname = dto.ArtistaNickname,
                    Precio = dto.Precio,
                    Estado = Obra.Estados.Activa,
                    FechaPublicacion = DateTime.UtcNow
                };

                Console.WriteLine("[DEBUG] Guardando obra en repositorio...");
                await _repositorio.AgregarObraAsync(obra);
                Console.WriteLine($"[DEBUG] Obra creada exitosamente con ID: {obra.Id}");

                return new
                {
                    id = obra.Id,
                    titulo = obra.Titulo,
                    descripcion = obra.Descripcion,
                    archivoBase64 = obra.ArchivoBase64,
                    firmaDigital = obra.FirmaDigital,
                    artistaNickname = obra.ArtistaNickname,
                    precio = obra.Precio,
                    estado = obra.Estado,
                    fechaPublicacion = obra.FechaPublicacion
                };
            }
            finally
            {
                // Limpiar archivo temporal
                if (File.Exists(rutaArchivoTemporal))
                {
                    Console.WriteLine($"[DEBUG] Eliminando archivo temporal: {rutaArchivoTemporal}");
                    File.Delete(rutaArchivoTemporal);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error en CrearObraAsync: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<ObraDto?> ObtenerObraPorIdAsync(int id)
    {
        var obra = await _repositorio.ObtenerObraPorIdAsync(id);
        if (obra == null) return null;

        return MapearADto(obra);
    }

    public async Task<List<ObraDto>> ObtenerObrasPorArtistaAsync(string artistaNickname)
    {
        var obras = await _repositorio.ObtenerObrasPorArtistaAsync(artistaNickname);
        return obras.Select(MapearADto).ToList();
    }

    public async Task<List<ObraDto>> ObtenerObrasActivasAsync(int limite = 10)
    {
        var obras = await _repositorio.ObtenerObrasActivasAsync(limite);
        return obras.Select(MapearADto).ToList();
    }

    public async Task<bool> ActualizarObraAsync(int id, UpdateObraDto dto)
    {
        // 🔧 CREAR UNA ENTIDAD TEMPORAL SOLO CON LOS DATOS NECESARIOS
        var obraTemp = new Obra
        {
            Id = id,
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio
        };

        return await _repositorio.ActualizarObraAsync(obraTemp);
    }

    public async Task<(bool success, string message)> OcultarObraAsync(int obraId)
    {
        var obra = await _repositorio.ObtenerObraPorIdAsync(obraId);
        if (obra == null)
            return (false, "Obra no encontrada");

        if (obra.Estado == Obra.Estados.Oculta)
            return (false, "La obra ya está oculta");

        var resultado = await _repositorio.OcultarObraAsync(obraId);
        return resultado 
            ? (true, "Obra ocultada exitosamente") 
            : (false, "Error al ocultar la obra");
    }

    public async Task<(bool success, string message)> ActivarObraAsync(int obraId)
    {
        var obra = await _repositorio.ObtenerObraPorIdAsync(obraId);
        if (obra == null)
            return (false, "Obra no encontrada");

        if (obra.Estado == Obra.Estados.Activa)
            return (false, "La obra ya está activada");

        var resultado = await _repositorio.ActivarObraAsync(obraId);
        return resultado 
            ? (true, "Obra activada exitosamente") 
            : (false, "Error al activar la obra");
    }

    public async Task<(bool success, string message)> EliminarObraAsync(int obraId)
    {
        var obra = await _repositorio.ObtenerObraPorIdAsync(obraId);
        if (obra == null)
            return (false, "Obra no encontrada");

        if (obra.Estado != Obra.Estados.Oculta)
            return (false, "Solo se pueden eliminar obras que estén en estado 'oculta'");

        var resultado = await _repositorio.EliminarObraAsync(obraId);
        return resultado 
            ? (true, "Obra eliminada exitosamente") 
            : (false, "Error al eliminar la obra");
    }

    private static ObraDto MapearADto(Obra obra)
    {
        return new ObraDto
        {
            Id = obra.Id,
            Titulo = obra.Titulo,
            Descripcion = obra.Descripcion,
            ArchivoBase64 = obra.ArchivoBase64,
            FirmaDigital = obra.FirmaDigital,
            ArtistaNickname = obra.ArtistaNickname,
            Precio = obra.Precio,
            Estado = obra.Estado,
            FechaPublicacion = obra.FechaPublicacion
        };
    }

    public async Task<bool> ValidarFirmaObraAsync(int obraId)
    {
        var obra = await _repositorio.ObtenerObraPorIdAsync(obraId);
        if (obra == null) return false;

        // Crear archivo temporal desde base64
        string rutaArchivoTemporal = await FileService.CreateTemporaryFileFromBase64Async(obra.ArchivoBase64);
        
        try
        {
            // Obtener la clave pública desde la configuración
            string? rutaClavePublica = _configuration["DigitalSignature:PublicKeyPath"];
            if (string.IsNullOrEmpty(rutaClavePublica))
                throw new InvalidOperationException("No se ha configurado la ruta de la clave pública");

            if (!File.Exists(rutaClavePublica))
                throw new FileNotFoundException($"No se encontró el archivo de clave pública en: {rutaClavePublica}");

            // Leer la clave pública
            string clavePublicaXml = await File.ReadAllTextAsync(rutaClavePublica);

            // Validar la firma
            return await _digitalSignatureService.ValidarFirmaAsync(rutaArchivoTemporal, obra.FirmaDigital, clavePublicaXml);
        }
        finally
        {
            // Limpiar archivo temporal
            if (File.Exists(rutaArchivoTemporal))
            {
                File.Delete(rutaArchivoTemporal);
            }
        }
    }
}
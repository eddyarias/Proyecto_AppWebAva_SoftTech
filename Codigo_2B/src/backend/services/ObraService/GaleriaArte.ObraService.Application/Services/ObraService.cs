using GaleriaArte.ObraService.Application.DTOs;
using GaleriaArte.ObraService.Domain.Entities;
using GaleriaArte.ObraService.Domain.Interfaces;
using GaleriaArte.ObraService.Application.Interfaces;

namespace GaleriaArte.ObraService.Application.Services;

public class ObraService : IObraService
{
    private readonly IObraRepository _repositorio;

    public ObraService(IObraRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<object> CrearObraAsync(CreateObraDto dto)
    {
        var obra = new Obra
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            ArchivoUrl = dto.ArchivoUrl,
            FirmaDigital = dto.FirmaDigital,
            ArtistaNickname = dto.ArtistaNickname,
            Precio = dto.Precio,
            Estado = Obra.Estados.Activa,
            FechaPublicacion = DateTime.UtcNow
        };

        await _repositorio.AgregarObraAsync(obra);

        return new
        {
            id = obra.Id,
            titulo = obra.Titulo,
            descripcion = obra.Descripcion,
            archivoUrl = obra.ArchivoUrl,
            artistaNickname = obra.ArtistaNickname,
            precio = obra.Precio,
            estado = obra.Estado,
            fechaPublicacion = obra.FechaPublicacion
        };
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
            ArchivoUrl = obra.ArchivoUrl,
            FirmaDigital = obra.FirmaDigital,
            ArtistaNickname = obra.ArtistaNickname,
            Precio = obra.Precio,
            Estado = obra.Estado,
            FechaPublicacion = obra.FechaPublicacion
        };
    }
}
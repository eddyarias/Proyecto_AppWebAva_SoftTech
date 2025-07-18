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
}
using GaleriaArte.ObraService.Domain.Entities;

namespace GaleriaArte.ObraService.Domain.Interfaces;

public interface IObraRepository
{
    Task AgregarObraAsync(Obra obra);
    Task<Obra?> ObtenerObraPorIdAsync(int id);
    Task<List<Obra>> ObtenerObrasPorArtistaAsync(string artistaNickname);
    Task<List<Obra>> ObtenerObrasActivasAsync(int limite = 10);
    Task<bool> ActualizarObraAsync(Obra obra);
    Task<bool> OcultarObraAsync(int obraId);
    Task<bool> ActivarObraAsync(int obraId);
    Task<bool> EliminarObraAsync(int obraId);
}
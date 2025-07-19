using GaleriaArte.ObraService.Application.DTOs;

namespace GaleriaArte.ObraService.Application.Interfaces;

public interface IObraService
{
    Task<object> CrearObraAsync(CreateObraDto dto); // Este método automáticamente firma la obra
    Task<ObraDto?> ObtenerObraPorIdAsync(int id);
    Task<List<ObraDto>> ObtenerObrasPorArtistaAsync(string artistaNickname);
    Task<List<ObraDto>> ObtenerObrasActivasAsync(int limite = 10);
    Task<bool> ActualizarObraAsync(int id, UpdateObraDto dto);
    Task<(bool success, string message)> OcultarObraAsync(int obraId);
    Task<(bool success, string message)> ActivarObraAsync(int obraId);
    Task<(bool success, string message)> EliminarObraAsync(int obraId);
    Task<bool> ValidarFirmaObraAsync(int obraId);
}
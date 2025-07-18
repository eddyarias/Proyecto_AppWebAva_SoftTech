using GaleriaArte.ObraService.Application.DTOs;

namespace GaleriaArte.ObraService.Application.Interfaces;

public interface IObraService
{
    Task<object> CrearObraAsync(CreateObraDto dto);
}
using GaleriaArte.ObraService.Domain.Entities;

namespace GaleriaArte.ObraService.Domain.Interfaces;

public interface IObraRepository
{
    Task AgregarObraAsync(Obra obra);
}
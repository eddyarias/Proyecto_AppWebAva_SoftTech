using GaleriaArte.UsuarioService.Infrastructure.Data;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GaleriaArte.UsuarioService.Infrastructure.Repositories;

public class RecuperacionRepository : IRecuperacionRepository
{
    private readonly UsuarioDbContext _context;

    public RecuperacionRepository(UsuarioDbContext context)
    {
        _context = context;
    }

    public async Task GuardarIntentoAsync(IntentoRecuperacion intento)
    {
        _context.IntentosRecuperacion.Add(intento);
        await _context.SaveChangesAsync();
    }

    public async Task<IntentoRecuperacion?> ObtenerPorTokenValidoAsync(Guid token)
    {
        return await _context.IntentosRecuperacion
            .FirstOrDefaultAsync(x => x.TokenRecuperacion == token && !x.Usado && x.Expiracion > DateTime.UtcNow);
    }

    public async Task MarcarComoUsadoAsync(int id)
    {
        var intento = await _context.IntentosRecuperacion.FindAsync(id);
        if (intento != null)
        {
            intento.Usado = true;
            await _context.SaveChangesAsync();
        }
    }
}
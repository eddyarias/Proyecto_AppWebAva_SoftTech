using GaleriaArte.ObraService.Domain.Entities;
using GaleriaArte.ObraService.Domain.Interfaces;
using GaleriaArte.ObraService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaleriaArte.ObraService.Infrastructure.Repositories;

public class ObraRepository : IObraRepository
{
    private readonly ObraDbContext _context;

    public ObraRepository(ObraDbContext context)
    {
        _context = context;
    }

    public async Task AgregarObraAsync(Obra obra)
    {
        _context.Obras.Add(obra);
        await _context.SaveChangesAsync();
    }
}
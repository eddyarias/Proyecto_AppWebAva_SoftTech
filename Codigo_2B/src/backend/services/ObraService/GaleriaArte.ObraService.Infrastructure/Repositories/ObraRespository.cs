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

    public async Task<Obra?> ObtenerObraPorIdAsync(int id)
    {
        return await _context.Obras
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Obra>> ObtenerObrasPorArtistaAsync(string artistaNickname)
    {
        return await _context.Obras
            .Where(o => o.ArtistaNickname == artistaNickname)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<List<Obra>> ObtenerObrasActivasAsync(int limite = 10)
    {
        return await _context.Obras
            .Where(o => o.Estado == Obra.Estados.Activa)
            .OrderByDescending(o => o.FechaPublicacion)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<bool> ActualizarObraAsync(Obra obra)
    {
        // 🔧 BUSCAR LA ENTIDAD EXISTENTE
        var obraExistente = await _context.Obras.FindAsync(obra.Id);
        if (obraExistente == null) return false;

        // 🔧 SOLO ACTUALIZAR CAMPOS EDITABLES
        obraExistente.Titulo = obra.Titulo;
        obraExistente.Descripcion = obra.Descripcion;
        obraExistente.Precio = obra.Precio;

        // 🔧 NO marcar toda la entidad como modificada, EF detectará automáticamente los cambios
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> OcultarObraAsync(int obraId)
    {
        var obra = await _context.Obras.FindAsync(obraId);
        if (obra == null) return false;

        obra.Estado = Obra.Estados.Oculta;
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> ActivarObraAsync(int obraId)
    {
        var obra = await _context.Obras.FindAsync(obraId);
        if (obra == null) return false;

        obra.Estado = Obra.Estados.Activa;
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> EliminarObraAsync(int obraId)
    {
        var obra = await _context.Obras.FindAsync(obraId);
        if (obra == null) return false;

        _context.Obras.Remove(obra);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
}
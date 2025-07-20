using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using GaleriaArte.UsuarioService.Domain.Interfaces;

namespace GaleriaArte.UsuarioService.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UsuarioDbContext _context;

    public UsuarioRepository(UsuarioDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        return await _context.Usuarios.AnyAsync(u => u.Correo == correo);
    }

    public async Task<bool> ExisteNicknameAsync(string nickname)
    {
        return await _context.Usuarios.AnyAsync(u => u.Nickname == nickname);
    }

    public async Task<Rol?> ObtenerRolPorNombreAsync(string nombre)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre);
    }

    public async Task AgregarUsuarioAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> ObtenerPorNicknameOCorreoAsync(string identificador)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == identificador || u.Nickname == identificador);
    }

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task ActualizarRefreshTokenAsync(Guid id, string token, DateTime expira)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user is null) return;

        user.RefreshToken = token;
        user.RefreshTokenExp = expira;
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarUsuarioAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
    }

    public async Task ActualizarPasswordAsync(Guid usuarioId, string hash)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario != null)
        {
            usuario.ContraseñaHash = hash;
            await _context.SaveChangesAsync();
        }
    }
}
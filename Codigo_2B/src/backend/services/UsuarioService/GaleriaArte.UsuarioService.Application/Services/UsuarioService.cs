using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Data;
using BCrypt.Net;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using GaleriaArte.UsuarioService.Application.Interfaces;

namespace GaleriaArte.UsuarioService.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _repositorio;

    public UsuarioService(UsuarioRepository repository)
    {
        _repositorio = repository;
    }

    public async Task<object> RegistrarUsuarioAsync(UsuarioDto dto)
    {
        if (await _repositorio.ExisteCorreoAsync(dto.Correo))
            throw new Exception("Correo ya registrado.");

        if (await _repositorio.ExisteNicknameAsync(dto.Nickname))
            throw new Exception("Nickname ya registrado.");

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contraseña);
        var usuario = new Usuario
        {
            Nickname = dto.Nickname,
            Correo = dto.Correo,
            ContraseñaHash = hash
        };

        var rol = await _repositorio.ObtenerRolPorNombreAsync(dto.Rol.ToLower());
        if (rol == null)
            throw new Exception("Rol inválido.");

        usuario.Rol = rol;
        usuario.RolId = rol.Id;

        await _repositorio.AgregarUsuarioAsync(usuario);

        // Devolvemos un objeto anónimo con los datos necesarios
        return new
        {
            success = true,
            message = "Usuario registrado exitosamente.",
        };
    }
    
    public async Task<bool> CambiarEstadoUsuarioAsync(Guid usuarioId, bool nuevoEstado)
    {
        var usuario = await _repositorio.ObtenerPorIdAsync(usuarioId);
        if (usuario == null)
            return false;

        try
        {
            usuario.Estado = nuevoEstado;
            await _repositorio.ActualizarUsuarioAsync(usuario);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al desactivar usuario: {ex.Message}");
            return false;
        }
    }
}
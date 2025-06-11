using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace BusinessLayer.GaleriaArte
{
    public class Usuarios
    {
        private string strConnectionString;
        public Usuarios(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public int Save(EntityLayer.GaleriaArte.Usuarios usuario)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.contraseña_hash);
            usuario.contraseña_hash = hashedPassword;

            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            return dataAccess.Save(usuario);
        }
        public List<EntityLayer.GaleriaArte.Usuarios> GetAll()
        {
            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            return dataAccess.GetAll();
        }
        public EntityLayer.GaleriaArte.Usuarios GetById(int id)
        {
            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            return dataAccess.GetById(id);
        }
        public void Update(EntityLayer.GaleriaArte.Usuarios usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.contraseña_hash))
            {
                usuario.contraseña_hash = BCrypt.Net.BCrypt.HashPassword(usuario.contraseña_hash);
            }

            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            dataAccess.Update(usuario);
        }
        public void Delete(int id)
        {
            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            dataAccess.Delete(id);
        }
        public EntityLayer.GaleriaArte.Usuarios Authenticate(string nickname, string password)
        {
            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            var usuario = dataAccess.GetByNickname(nickname);

            if (usuario == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, usuario.contraseña_hash);

            if (isValid)
            {
                usuario.contraseña_hash = null;
                return usuario;
            }

            return null;
        }
        public EntityLayer.GaleriaArte.Usuarios GetByNickname(string nickname)
        {
            DataAccess.GaleriaArte.Usuarios dataAccess = new DataAccess.GaleriaArte.Usuarios(strConnectionString);
            return dataAccess.GetByNickname(nickname);
        }
    }
}

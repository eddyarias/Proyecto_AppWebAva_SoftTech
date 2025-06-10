using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess.GaleriaArte
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
            using (SqlConnection con = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Usuarios_Insertar", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nickname", usuario.nickname);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contraseñaHash", usuario.contraseñaHash);
                    cmd.Parameters.AddWithValue("@rol", usuario.rol);
                    cmd.Parameters.AddWithValue("@estado", usuario.estado);
                    cmd.Parameters.AddWithValue("@fechaCreacion", usuario.fechaCreacion);
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}

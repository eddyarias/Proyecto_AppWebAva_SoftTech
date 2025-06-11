using EntityLayer.GaleriaArte;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.GaleriaArte
{
    public class IntentosRecuperacion
    {
        private string strConnectionString;

        public IntentosRecuperacion(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public void GuardarIntento(IntentoRecuperacion intento)
        {
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                con.Open();
                string query = @"INSERT INTO intentos_recuperacion (usuario_id, token_recuperacion, expiracion, fecha_solicitud)
                                 VALUES (@usuario_id, @token_recuperacion, @expiracion, @fecha_solicitud)";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@usuario_id", intento.usuario_id);
                    cmd.Parameters.AddWithValue("@token_recuperacion", intento.token_recuperacion);
                    cmd.Parameters.AddWithValue("@expiracion", intento.expiracion);
                    cmd.Parameters.AddWithValue("@fecha_solicitud",intento.fecha_solicitud);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IntentoRecuperacion ObtenerPorToken(Guid token)
        {
            DateTime fecha_actual = DateTime.Now;

            using (var con = new NpgsqlConnection(strConnectionString))
            {
                con.Open();
                string query = @"SELECT * FROM intentos_recuperacion 
                                 WHERE token_recuperacion = @token AND usado = FALSE AND expiracion > @current_time";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.Parameters.AddWithValue("@current_time", fecha_actual);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new IntentoRecuperacion
                            {
                                id = reader.GetInt32(0),
                                usuario_id = reader.GetInt32(1),
                                token_recuperacion = reader.GetGuid(2),
                                expiracion = reader.GetDateTime(3),
                                usado = reader.GetBoolean(4),
                                fecha_solicitud = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void MarcarComoUsado(int id)
        {
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                con.Open();
                string query = @"UPDATE intentos_recuperacion SET usado = TRUE WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

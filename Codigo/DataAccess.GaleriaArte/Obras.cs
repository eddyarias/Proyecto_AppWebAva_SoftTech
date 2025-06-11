using System;
using System.Collections.Generic;
using Npgsql;
using EntityLayer.GaleriaArte;

namespace DataAccess.GaleriaArte
{
    public class Obras
    {
        private readonly string connectionString;

        public Obras(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Obra> ObtenerObrasPorArtista(int artistaId)
        {
            var obras = new List<Obra>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                string query = @"SELECT id, titulo, descripcion, archivo_url, firma_digital, artista_id, precio, estado, fecha_publicacion
                                 FROM obras
                                 WHERE artista_id = @artistaId";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@artistaId", artistaId);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obra = new Obra
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                titulo = reader.GetString(reader.GetOrdinal("titulo")),
                                descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                                archivo_url = reader.GetString(reader.GetOrdinal("archivo_url")),
                                firma_digital = reader.GetString(reader.GetOrdinal("firma_digital")),
                                artista_id = reader.GetInt32(reader.GetOrdinal("artista_id")),
                                precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                estado = reader.GetString(reader.GetOrdinal("estado")),
                                fecha_publicacion = reader.GetDateTime(reader.GetOrdinal("fecha_publicacion"))
                            };
                            obras.Add(obra);
                        }
                    }
                }
            }

            return obras;
        }
    }
}

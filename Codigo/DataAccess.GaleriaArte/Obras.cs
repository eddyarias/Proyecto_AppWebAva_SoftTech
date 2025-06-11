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

        // CREATE
        public int CrearObra(Obra obra)
        {
            int newId = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                string query = @"INSERT INTO obras (titulo, descripcion, archivo_url, firma_digital, artista_id, precio, estado, fecha_publicacion)
                                 VALUES (@titulo, @descripcion, @archivo_url, @firma_digital, @artista_id, @precio, @estado, @fecha_publicacion)
                                 RETURNING id;";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@titulo", obra.titulo);
                    cmd.Parameters.AddWithValue("@descripcion", (object)obra.descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@archivo_url", obra.archivo_url);
                    cmd.Parameters.AddWithValue("@firma_digital", obra.firma_digital);
                    cmd.Parameters.AddWithValue("@artista_id", obra.artista_id);
                    cmd.Parameters.AddWithValue("@precio", obra.precio);
                    cmd.Parameters.AddWithValue("@estado", obra.estado);
                    cmd.Parameters.AddWithValue("@fecha_publicacion", obra.fecha_publicacion);

                    con.Open();
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return newId;
        }

        // READ (por artista)
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

        // READ (por id)
        public Obra ObtenerObraPorId(int id)
        {
            Obra obra = null;
            using (var con = new NpgsqlConnection(connectionString))
            {
                string query = @"SELECT id, titulo, descripcion, archivo_url, firma_digital, artista_id, precio, estado, fecha_publicacion
                                 FROM obras
                                 WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            obra = new Obra
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
                        }
                    }
                }
            }
            return obra;
        }

        // UPDATE
        public bool ActualizarObra(Obra obra)
        {
            int rowsAffected = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                string query = @"UPDATE obras SET
                                    titulo = @titulo,
                                    descripcion = @descripcion,
                                    archivo_url = @archivo_url,
                                    firma_digital = @firma_digital,
                                    artista_id = @artista_id,
                                    precio = @precio,
                                    estado = @estado,
                                    fecha_publicacion = @fecha_publicacion
                                 WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@titulo", obra.titulo);
                    cmd.Parameters.AddWithValue("@descripcion", (object)obra.descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@archivo_url", obra.archivo_url);
                    cmd.Parameters.AddWithValue("@firma_digital", obra.firma_digital);
                    cmd.Parameters.AddWithValue("@artista_id", obra.artista_id);
                    cmd.Parameters.AddWithValue("@precio", obra.precio);
                    cmd.Parameters.AddWithValue("@estado", obra.estado);
                    cmd.Parameters.AddWithValue("@fecha_publicacion", obra.fecha_publicacion);
                    cmd.Parameters.AddWithValue("@id", obra.id);

                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0;
        }

        // DELETE
        public bool EliminarObra(int id)
        {
            int rowsAffected = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                string query = @"DELETE FROM obras WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0;
        }
    }
}

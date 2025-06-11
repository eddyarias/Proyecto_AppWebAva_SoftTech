using System;
using System.Collections.Generic;
using Npgsql; 

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
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "INSERT INTO Usuarios (nickname, correo, contraseña_hash, rol, estado, fecha_creacion) " +
                                     "VALUES (@nickname, @correo, @contraseña_hash, @rol, @estado, @fecha_creacion) RETURNING id;";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@nickname", usuario.nickname);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contraseña_hash", usuario.contraseña_hash);
                    cmd.Parameters.AddWithValue("@rol", usuario.rol);
                    cmd.Parameters.AddWithValue("@estado", usuario.estado);
                    cmd.Parameters.AddWithValue("@fecha_creacion", usuario.fecha_creacion);
                    cmd.Connection = con;
                    con.Open();
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    return id;
                }
            }
        }

        public List<EntityLayer.GaleriaArte.Usuarios> GetAll()
        {
            List<EntityLayer.GaleriaArte.Usuarios> usuarios = new List<EntityLayer.GaleriaArte.Usuarios>();
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "SELECT id, nickname, correo, contraseña_hash, rol, estado, fecha_creacion FROM Usuarios";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EntityLayer.GaleriaArte.Usuarios usuario = new EntityLayer.GaleriaArte.Usuarios
                            {
                                id = reader.GetInt32(0),
                                nickname = reader.GetString(1),
                                correo = reader.GetString(2),
                                contraseña_hash = reader.GetString(3),
                                rol = reader.GetString(4),
                                estado = reader.GetBoolean(5),
                                fecha_creacion = reader.GetDateTime(6)
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            return usuarios;
        }

        public EntityLayer.GaleriaArte.Usuarios GetById(int id)
        {
            EntityLayer.GaleriaArte.Usuarios usuario = null;
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "SELECT id, nickname, correo, contraseña_hash, rol, estado, fecha_creacion FROM Usuarios WHERE id = @id";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Connection = con;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new EntityLayer.GaleriaArte.Usuarios
                            {
                                id = reader.GetInt32(0),
                                nickname = reader.GetString(1),
                                correo = reader.GetString(2),
                                contraseña_hash = reader.GetString(3),
                                rol = reader.GetString(4),
                                estado = reader.GetBoolean(5),
                                fecha_creacion = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public bool Update(EntityLayer.GaleriaArte.Usuarios usuario)
        {
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "UPDATE Usuarios SET nickname = @nickname, correo = @correo, contraseña_hash = @contraseña_hash, " +
                                    "rol = @rol, estado = @estado, fecha_creacion = @fecha_creacion WHERE id = @id";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", usuario.id);
                    cmd.Parameters.AddWithValue("@nickname", usuario.nickname);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contraseña_hash", usuario.contraseña_hash);
                    cmd.Parameters.AddWithValue("@rol", usuario.rol);
                    cmd.Parameters.AddWithValue("@estado", usuario.estado);
                    cmd.Parameters.AddWithValue("@fecha_creacion", usuario.fecha_creacion);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "DELETE FROM Usuarios WHERE id = @id";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public EntityLayer.GaleriaArte.Usuarios GetByNickname(string nickname)
        {
            EntityLayer.GaleriaArte.Usuarios usuario = null;
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "SELECT id, nickname, correo, contraseña_hash, rol, estado, fecha_creacion FROM Usuarios WHERE nickname = @nickname";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@nickname", nickname);
                    cmd.Connection = con;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new EntityLayer.GaleriaArte.Usuarios
                            {
                                id = reader.GetInt32(0),
                                nickname = reader.GetString(1),
                                correo = reader.GetString(2),
                                contraseña_hash = reader.GetString(3),
                                rol = reader.GetString(4),
                                estado = reader.GetBoolean(5),
                                fecha_creacion = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public EntityLayer.GaleriaArte.Usuarios AutenticarUsuario(string nickname, string contraseña_hash)
        {
            EntityLayer.GaleriaArte.Usuarios usuario = null;
            using (var con = new NpgsqlConnection(strConnectionString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    string sentence = "SELECT id, nickname, correo, rol, estado, fecha_creacion FROM Usuarios WHERE nickname = @nickname AND contraseña_hash = @contraseña_hash";
                    cmd.CommandText = sentence;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@nickname", nickname);
                    cmd.Parameters.AddWithValue("@contraseña_hash", contraseña_hash);
                    cmd.Connection = con;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new EntityLayer.GaleriaArte.Usuarios
                            {
                                id = reader.GetInt32(0),
                                nickname = reader.GetString(1),
                                correo = reader.GetString(2),
                                rol = reader.GetString(3),
                                estado = reader.GetBoolean(4),
                                fecha_creacion = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return usuario;
        }
    }
}

using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario u)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Usuario
                    ({nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Rol)})
                    VALUES (@email, @clave, @nombre, @rol);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    u.IdUsuario = res;
                    connection.Close();
                }
            }
            return res;
        }
        public int Baja(Usuario u)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Baja lógica: Reserva y Pago tienen FK a Usuario (auditoría de quién
                // creó/anuló cada uno), así que no conviene borrar la fila.
                string sql = @$"UPDATE Usuario SET {nameof(Usuario.Activo)} = 0 WHERE {nameof(Usuario.IdUsuario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", u.IdUsuario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Usuario u)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Usuario
                    SET {nameof(Usuario.Email)}=@email, {nameof(Usuario.Clave)}=@clave, {nameof(Usuario.Nombre)}=@nombre, {nameof(Usuario.Rol)}=@rol, {nameof(Usuario.AvatarUrl)}=@avatarUrl
                    WHERE {nameof(Usuario.IdUsuario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    command.Parameters.AddWithValue("@avatarUrl", (object?)u.AvatarUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", u.IdUsuario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Usuario> res = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT {nameof(Usuario.IdUsuario)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Rol)}, {nameof(Usuario.Activo)}, {nameof(Usuario.AvatarUrl)}
                    FROM Usuario
                    WHERE {nameof(Usuario.Activo)} = 1
                    ORDER BY {nameof(Usuario.Nombre)}
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearUsuario(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT COUNT({nameof(Usuario.IdUsuario)}) FROM Usuario WHERE {nameof(Usuario.Activo)} = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Usuario.IdUsuario)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Rol)}, {nameof(Usuario.Activo)}, {nameof(Usuario.AvatarUrl)}
                    FROM Usuario
                    WHERE {nameof(Usuario.IdUsuario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        u = MapearUsuario(reader);
                    }
                    connection.Close();
                }
            }
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Usuario.IdUsuario)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Rol)}, {nameof(Usuario.Activo)}, {nameof(Usuario.AvatarUrl)}
                    FROM Usuario
                    WHERE {nameof(Usuario.Email)} = @email AND {nameof(Usuario.Activo)} = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        u = MapearUsuario(reader);
                    }
                    connection.Close();
                }
            }
            return u;
        }
        private static Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.IdUsuario))),
                Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                Clave = reader.GetString(reader.GetOrdinal(nameof(Usuario.Clave))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(Usuario.Nombre))),
                Rol = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol))),
                Activo = reader.GetBoolean(reader.GetOrdinal(nameof(Usuario.Activo))),
                AvatarUrl = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.AvatarUrl))) ? null : reader.GetString(reader.GetOrdinal(nameof(Usuario.AvatarUrl))),
            };
        }
    }
}
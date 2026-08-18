using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Propietario
                    ({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.DniCuit)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)})
                    VALUES (@nombre, @apellido, @dniCuit, @telefono, @email);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dniCuit", p.DniCuit);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPropietario = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Propietario SET {nameof(Propietario.Activo)} = 0 WHERE {nameof(Propietario.IdPropietario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", p.IdPropietario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Propietario
                    SET {nameof(Propietario.Nombre)}=@nombre, {nameof(Propietario.Apellido)}=@apellido, {nameof(Propietario.DniCuit)}=@dniCuit, {nameof(Propietario.Telefono)}=@telefono, {nameof(Propietario.Email)}=@email
                    WHERE {nameof(Propietario.IdPropietario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dniCuit", p.DniCuit);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@id", p.IdPropietario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Propietario> res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.DniCuit)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Activo)}
                    FROM Propietario
                    WHERE {nameof(Propietario.Activo)} = 1
                    ORDER BY {nameof(Propietario.IdPropietario)}
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearPropietario(reader));
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
                string sql = @$"SELECT COUNT({nameof(Propietario.IdPropietario)}) FROM Propietario WHERE {nameof(Propietario.Activo)} = 1";
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

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.DniCuit)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Activo)}
                    FROM Propietario
                    WHERE {nameof(Propietario.IdPropietario)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = MapearPropietario(reader);
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public Propietario? ObtenerPorEmail(string email)
        {
            Propietario? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.DniCuit)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Activo)}
                    FROM Propietario
                    WHERE {nameof(Propietario.Email)} = @email";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = MapearPropietario(reader);
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Propietario> BuscarPorNombre(string nombre)
        {
            List<Propietario> res = new List<Propietario>();
            nombre = "%" + nombre + "%";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.DniCuit)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Activo)}
                    FROM Propietario
                    WHERE {nameof(Propietario.Nombre)} LIKE @nombre OR {nameof(Propietario.Apellido)} LIKE @nombre";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearPropietario(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        private static Propietario MapearPropietario(MySqlDataReader reader)
        {
            return new Propietario
            {
                IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Propietario.IdPropietario))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
                Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido))),
                DniCuit = reader.GetString(reader.GetOrdinal(nameof(Propietario.DniCuit))),
                Telefono = reader.GetString(reader.GetOrdinal(nameof(Propietario.Telefono))),
                Email = reader.GetString(reader.GetOrdinal(nameof(Propietario.Email))),
                Activo = reader.GetBoolean(reader.GetOrdinal(nameof(Propietario.Activo))),
            };
        }
    }
}
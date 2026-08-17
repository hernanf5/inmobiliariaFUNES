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
                string sql = @"INSERT INTO Propietario
                    (Nombre, Apellido, DniCuit, Telefono, Email)
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
                string sql = "DELETE FROM Propietario WHERE IdPropietario = @id";
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
                string sql = @"UPDATE Propietario
                    SET Nombre=@nombre, Apellido=@apellido, DniCuit=@dniCuit, Telefono=@telefono, Email=@email
                    WHERE IdPropietario = @id";
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
                    SELECT IdPropietario, Nombre, Apellido, DniCuit, Telefono, Email, Activo
                    FROM Propietario
                    ORDER BY IdPropietario
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
                string sql = "SELECT COUNT(IdPropietario) FROM Propietario";
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
                string sql = @"SELECT IdPropietario, Nombre, Apellido, DniCuit, Telefono, Email, Activo
                    FROM Propietario
                    WHERE IdPropietario = @id";
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
                string sql = @"SELECT IdPropietario, Nombre, Apellido, DniCuit, Telefono, Email, Activo
                    FROM Propietario
                    WHERE Email = @email";
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
                string sql = @"SELECT IdPropietario, Nombre, Apellido, DniCuit, Telefono, Email, Activo
                    FROM Propietario
                    WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
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
                IdPropietario = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                DniCuit = reader.GetString(reader.GetOrdinal("DniCuit")),
                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
            };
        }
    }
}
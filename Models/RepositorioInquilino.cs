using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Inquilino
                    ({nameof(Inquilino.Dni)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)})
                    VALUES (@dni, @nombre, @apellido, @telefono, @email);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.IdInquilino = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Baja lógica, mismo criterio que Propietario: Inquilino puede
                // tener Reservas asociadas (FK), así que no conviene borrar la fila.
                string sql = @$"UPDATE Inquilino SET {nameof(Inquilino.Activo)} = 0 WHERE {nameof(Inquilino.IdInquilino)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", i.IdInquilino);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Inquilino
                    SET {nameof(Inquilino.Dni)}=@dni, {nameof(Inquilino.Nombre)}=@nombre, {nameof(Inquilino.Apellido)}=@apellido, {nameof(Inquilino.Telefono)}=@telefono, {nameof(Inquilino.Email)}=@email
                    WHERE {nameof(Inquilino.IdInquilino)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@id", i.IdInquilino);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT {nameof(Inquilino.IdInquilino)}, {nameof(Inquilino.Dni)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)}, {nameof(Inquilino.Activo)}
                    FROM Inquilino
                    WHERE {nameof(Inquilino.Activo)} = 1
                    ORDER BY {nameof(Inquilino.IdInquilino)}
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInquilino(reader));
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
                string sql = @$"SELECT COUNT({nameof(Inquilino.IdInquilino)}) FROM Inquilino WHERE {nameof(Inquilino.Activo)} = 1";
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

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Inquilino.IdInquilino)}, {nameof(Inquilino.Dni)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)}, {nameof(Inquilino.Activo)}
                    FROM Inquilino
                    WHERE {nameof(Inquilino.IdInquilino)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        i = MapearInquilino(reader);
                    }
                    connection.Close();
                }
            }
            return i;
        }

        private static Inquilino MapearInquilino(MySqlDataReader reader)
        {
            return new Inquilino
            {
                IdInquilino = reader.GetInt32(reader.GetOrdinal(nameof(Inquilino.IdInquilino))),
                Dni = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Dni))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Nombre))),
                Apellido = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Apellido))),
                Telefono = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Telefono))),
                Email = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Email))),
                Activo = reader.GetBoolean(reader.GetOrdinal(nameof(Inquilino.Activo))),
            };
        }
    }
}
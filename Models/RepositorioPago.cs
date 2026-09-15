using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Pago
                    ({nameof(Pago.IdReserva)}, {nameof(Pago.Concepto)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)}, {nameof(Pago.Estado)}, {nameof(Pago.IdUsuarioCreador)})
                    VALUES (@idReserva, @concepto, @fechaPago, @importe, 'Activo', @idUsuarioCreador);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idReserva", p.IdReserva);
                    command.Parameters.AddWithValue("@concepto", p.Concepto);
                    command.Parameters.AddWithValue("@fechaPago", p.FechaPago.Date);
                    command.Parameters.AddWithValue("@importe", p.Importe);
                    command.Parameters.AddWithValue("@idUsuarioCreador", (object?)p.IdUsuarioCreador ?? DBNull.Value);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPago = res;
                    connection.Close();
                }
            }
            return res;
        }
        public int Modificacion(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Solo se permite editar el Concepto 
                // en que el monto y la fecha no se pueden modificar una vez cargados.
                string sql = @$"UPDATE Pago SET {nameof(Pago.Concepto)} = @concepto WHERE {nameof(Pago.IdPago)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@concepto", p.Concepto);
                    command.Parameters.AddWithValue("@id", p.IdPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public int Baja(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Pago
                    SET {nameof(Pago.Estado)} = 'Anulado', {nameof(Pago.IdUsuarioAnulador)} = @idUsuarioAnulador
                    WHERE {nameof(Pago.IdPago)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idUsuarioAnulador", (object?)p.IdUsuarioAnulador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", p.IdPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            List<Pago> res = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Pago.IdPago)}, {nameof(Pago.IdReserva)}, {nameof(Pago.Concepto)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)}, {nameof(Pago.Estado)}, {nameof(Pago.IdUsuarioCreador)}, {nameof(Pago.IdUsuarioAnulador)}
                    FROM Pago
                    WHERE {nameof(Pago.IdReserva)} = @idReserva
                    ORDER BY {nameof(Pago.FechaPago)} DESC, {nameof(Pago.IdPago)} DESC";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearPago(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public Pago? ObtenerPorId(int id)
        {
            Pago? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(Pago.IdPago)}, {nameof(Pago.IdReserva)}, {nameof(Pago.Concepto)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)}, {nameof(Pago.Estado)}, {nameof(Pago.IdUsuarioCreador)}, {nameof(Pago.IdUsuarioAnulador)}
                    FROM Pago
                    WHERE {nameof(Pago.IdPago)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = MapearPago(reader);
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Pago> res = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT {nameof(Pago.IdPago)}, {nameof(Pago.IdReserva)}, {nameof(Pago.Concepto)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)}, {nameof(Pago.Estado)}, {nameof(Pago.IdUsuarioCreador)}, {nameof(Pago.IdUsuarioAnulador)}
                    FROM Pago
                    ORDER BY {nameof(Pago.FechaPago)} DESC
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearPago(reader));
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
                string sql = @$"SELECT COUNT({nameof(Pago.IdPago)}) FROM Pago";
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

        private static Pago MapearPago(MySqlDataReader reader)
        {
            return new Pago
            {
                IdPago = reader.GetInt32(reader.GetOrdinal(nameof(Pago.IdPago))),
                IdReserva = reader.GetInt32(reader.GetOrdinal(nameof(Pago.IdReserva))),
                Concepto = reader.GetString(reader.GetOrdinal(nameof(Pago.Concepto))),
                FechaPago = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaPago))),
                Importe = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Importe))),
                Estado = reader.GetString(reader.GetOrdinal(nameof(Pago.Estado))),
                IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.IdUsuarioCreador))) ? null : reader.GetInt32(reader.GetOrdinal(nameof(Pago.IdUsuarioCreador))),
                IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.IdUsuarioAnulador))) ? null : reader.GetInt32(reader.GetOrdinal(nameof(Pago.IdUsuarioAnulador))),
            };
        }
    }
}
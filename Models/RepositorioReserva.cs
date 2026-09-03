using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration)
        {
        }

        public bool EstaOcupado(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int? idReservaExcluir = null)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT COUNT({nameof(Reserva.IdReserva)})
                    FROM Reserva
                    WHERE {nameof(Reserva.IdInmueble)} = @idInmueble
                        AND {nameof(Reserva.Estado)} NOT IN ('Terminada anticipadamente', 'Cancelada')
                        AND {nameof(Reserva.FechaDesde)} <= @fechaHasta
                        AND {nameof(Reserva.FechaHastaOriginal)} >= @fechaDesde
                        AND (@idReservaExcluir IS NULL OR {nameof(Reserva.IdReserva)} <> @idReservaExcluir)
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Date);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Date);
                    command.Parameters.AddWithValue("@idReservaExcluir", (object?)idReservaExcluir ?? DBNull.Value);
                    connection.Open();
                    int cantidad = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return cantidad > 0;
                }
            }
        }
        public int Alta(Reserva r)
        {
            if (EstaOcupado(r.IdInmueble, r.FechaDesde, r.FechaHastaOriginal))
            {
                throw new InvalidOperationException("El inmueble ya tiene una reserva vigente que se superpone con esas fechas.");
            }

            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Reserva
                    ({nameof(Reserva.IdInquilino)}, {nameof(Reserva.IdInmueble)}, {nameof(Reserva.MontoPorDia)}, {nameof(Reserva.FechaDesde)}, {nameof(Reserva.FechaHastaOriginal)}, {nameof(Reserva.Estado)})
                    VALUES (@idInquilino, @idInmueble, @montoPorDia, @fechaDesde, @fechaHastaOriginal, @estado);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@montoPorDia", r.MontoPorDia);
                    command.Parameters.AddWithValue("@fechaDesde", r.FechaDesde.Date);
                    command.Parameters.AddWithValue("@fechaHastaOriginal", r.FechaHastaOriginal.Date);
                    command.Parameters.AddWithValue("@estado", "Vigente");
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    r.IdReserva = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Reserva r)
        {
            if (EstaOcupado(r.IdInmueble, r.FechaDesde, r.FechaHastaOriginal, r.IdReserva))
            {
                throw new InvalidOperationException("El inmueble ya tiene una reserva vigente que se superpone con esas fechas.");
            }

            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Reserva
                    SET {nameof(Reserva.IdInquilino)}=@idInquilino, {nameof(Reserva.IdInmueble)}=@idInmueble, {nameof(Reserva.MontoPorDia)}=@montoPorDia,
                        {nameof(Reserva.FechaDesde)}=@fechaDesde, {nameof(Reserva.FechaHastaOriginal)}=@fechaHastaOriginal
                    WHERE {nameof(Reserva.IdReserva)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@montoPorDia", r.MontoPorDia);
                    command.Parameters.AddWithValue("@fechaDesde", r.FechaDesde.Date);
                    command.Parameters.AddWithValue("@fechaHastaOriginal", r.FechaHastaOriginal.Date);
                    command.Parameters.AddWithValue("@id", r.IdReserva);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(Reserva r)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Reserva SET {nameof(Reserva.Estado)} = 'Cancelada' WHERE {nameof(Reserva.IdReserva)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", r.IdReserva);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Reserva> res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT r.{nameof(Reserva.IdReserva)}, r.{nameof(Reserva.IdInquilino)}, r.{nameof(Reserva.IdInmueble)},
                        r.{nameof(Reserva.MontoPorDia)}, r.{nameof(Reserva.FechaDesde)}, r.{nameof(Reserva.FechaHastaOriginal)},
                        r.{nameof(Reserva.FechaTerminacion)}, r.{nameof(Reserva.Multa)}, r.{nameof(Reserva.Estado)},
                        iq.{nameof(Inquilino.Nombre)} AS InquilinoNombre, iq.{nameof(Inquilino.Apellido)} AS InquilinoApellido,
                        im.{nameof(Inmueble.Direccion)} AS InmuebleDireccion
                    FROM Reserva r
                    INNER JOIN Inquilino iq ON r.{nameof(Reserva.IdInquilino)} = iq.{nameof(Inquilino.IdInquilino)}
                    INNER JOIN Inmueble im ON r.{nameof(Reserva.IdInmueble)} = im.{nameof(Inmueble.IdInmueble)}
                    ORDER BY r.{nameof(Reserva.FechaDesde)} DESC
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearReserva(reader));
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
                string sql = @$"SELECT COUNT({nameof(Reserva.IdReserva)}) FROM Reserva";
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

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? entidad = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT r.{nameof(Reserva.IdReserva)}, r.{nameof(Reserva.IdInquilino)}, r.{nameof(Reserva.IdInmueble)},
                        r.{nameof(Reserva.MontoPorDia)}, r.{nameof(Reserva.FechaDesde)}, r.{nameof(Reserva.FechaHastaOriginal)},
                        r.{nameof(Reserva.FechaTerminacion)}, r.{nameof(Reserva.Multa)}, r.{nameof(Reserva.Estado)},
                        iq.{nameof(Inquilino.Nombre)} AS InquilinoNombre, iq.{nameof(Inquilino.Apellido)} AS InquilinoApellido,
                        im.{nameof(Inmueble.Direccion)} AS InmuebleDireccion
                    FROM Reserva r
                    INNER JOIN Inquilino iq ON r.{nameof(Reserva.IdInquilino)} = iq.{nameof(Inquilino.IdInquilino)}
                    INNER JOIN Inmueble im ON r.{nameof(Reserva.IdInmueble)} = im.{nameof(Inmueble.IdInmueble)}
                    WHERE r.{nameof(Reserva.IdReserva)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = MapearReserva(reader);
                    }
                    connection.Close();
                }
            }
            return entidad;
        }

        public int Terminar(Reserva r, DateTime fechaTerminacion, decimal multa)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Reserva
                    SET {nameof(Reserva.Estado)} = 'Terminada anticipadamente',
                        {nameof(Reserva.FechaTerminacion)} = @fechaTerminacion,
                        {nameof(Reserva.Multa)} = @multa
                    WHERE {nameof(Reserva.IdReserva)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaTerminacion", fechaTerminacion.Date);
                    command.Parameters.AddWithValue("@multa", multa);
                    command.Parameters.AddWithValue("@id", r.IdReserva);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        private static Reserva MapearReserva(MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32(reader.GetOrdinal(nameof(Reserva.IdReserva))),
                IdInquilino = reader.GetInt32(reader.GetOrdinal(nameof(Reserva.IdInquilino))),
                IdInmueble = reader.GetInt32(reader.GetOrdinal(nameof(Reserva.IdInmueble))),
                MontoPorDia = reader.GetDecimal(reader.GetOrdinal(nameof(Reserva.MontoPorDia))),
                FechaDesde = reader.GetDateTime(reader.GetOrdinal(nameof(Reserva.FechaDesde))),
                FechaHastaOriginal = reader.GetDateTime(reader.GetOrdinal(nameof(Reserva.FechaHastaOriginal))),
                FechaTerminacion = reader.IsDBNull(reader.GetOrdinal(nameof(Reserva.FechaTerminacion))) ? null : reader.GetDateTime(reader.GetOrdinal(nameof(Reserva.FechaTerminacion))),
                Multa = reader.IsDBNull(reader.GetOrdinal(nameof(Reserva.Multa))) ? null : reader.GetDecimal(reader.GetOrdinal(nameof(Reserva.Multa))),
                Estado = reader.GetString(reader.GetOrdinal(nameof(Reserva.Estado))),
                Inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32(reader.GetOrdinal(nameof(Reserva.IdInquilino))),
                    Nombre = reader.GetString(reader.GetOrdinal("InquilinoNombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("InquilinoApellido")),
                },
                Inmueble = new Inmueble
                {
                    IdInmueble = reader.GetInt32(reader.GetOrdinal(nameof(Reserva.IdInmueble))),
                    Direccion = reader.GetString(reader.GetOrdinal("InmuebleDireccion")),
                },
            };
        }
    }
}
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO Inmueble
                    ({nameof(Inmueble.Direccion)}, {nameof(Inmueble.Cupo)}, {nameof(Inmueble.PrecioPorDia)}, {nameof(Inmueble.PorcentajeReserva)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, {nameof(Inmueble.IdPropietario)}, {nameof(Inmueble.IdTipoInmueble)})
                    VALUES (@direccion, @cupo, @precioPorDia, @porcentajeReserva, @latitud, @longitud, @idPropietario, @idTipoInmueble);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                    command.Parameters.AddWithValue("@precioPorDia", entidad.PrecioPorDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
                    command.Parameters.AddWithValue("@latitud", (object?)entidad.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)entidad.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
                    command.Parameters.AddWithValue("@idTipoInmueble", entidad.IdTipoInmueble);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.IdInmueble = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(Inmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Inmueble SET {nameof(Inmueble.Estado)} = 'Suspendido' WHERE {nameof(Inmueble.IdInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", entidad.IdInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Reactivar(Inmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Inmueble SET {nameof(Inmueble.Estado)} = 'Disponible' WHERE {nameof(Inmueble.IdInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", entidad.IdInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inmueble entidad)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE Inmueble
                    SET {nameof(Inmueble.Direccion)}=@direccion, {nameof(Inmueble.Cupo)}=@cupo, {nameof(Inmueble.PrecioPorDia)}=@precioPorDia,
                        {nameof(Inmueble.PorcentajeReserva)}=@porcentajeReserva, {nameof(Inmueble.Latitud)}=@latitud, {nameof(Inmueble.Longitud)}=@longitud,
                        {nameof(Inmueble.IdPropietario)}=@idPropietario, {nameof(Inmueble.IdTipoInmueble)}=@idTipoInmueble
                    WHERE {nameof(Inmueble.IdInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                    command.Parameters.AddWithValue("@precioPorDia", entidad.PrecioPorDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
                    command.Parameters.AddWithValue("@latitud", (object?)entidad.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)entidad.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
                    command.Parameters.AddWithValue("@idTipoInmueble", entidad.IdTipoInmueble);
                    command.Parameters.AddWithValue("@id", entidad.IdInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return ObtenerLista(paginaNro, tamPagina, null, null);
        }

        public IList<Inmueble> ObtenerLista(int paginaNro, int tamPagina, string? estado, int? idPropietario)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string filtro = "";
                if (!string.IsNullOrEmpty(estado))
                    filtro += $" AND i.{nameof(Inmueble.Estado)} = @estado";
                if (idPropietario.HasValue)
                    filtro += $" AND i.{nameof(Inmueble.IdPropietario)} = @idPropietario";

                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    WHERE 1=1 {filtro}
                    ORDER BY i.{nameof(Inmueble.IdInmueble)}
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (!string.IsNullOrEmpty(estado))
                        command.Parameters.AddWithValue("@estado", estado);
                    if (idPropietario.HasValue)
                        command.Parameters.AddWithValue("@idPropietario", idPropietario.Value);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            return ObtenerCantidad(null, null);
        }

        public int ObtenerCantidad(string? estado, int? idPropietario)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string filtro = "";
                if (!string.IsNullOrEmpty(estado))
                    filtro += " AND Estado = @estado";
                if (idPropietario.HasValue)
                    filtro += " AND IdPropietario = @idPropietario";

                string sql = @$"SELECT COUNT({nameof(Inmueble.IdInmueble)}) FROM Inmueble WHERE 1=1 {filtro}";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (!string.IsNullOrEmpty(estado))
                        command.Parameters.AddWithValue("@estado", estado);
                    if (idPropietario.HasValue)
                        command.Parameters.AddWithValue("@idPropietario", idPropietario.Value);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? entidad = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    WHERE i.{nameof(Inmueble.IdInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = MapearInmueble(reader);
                    }
                    connection.Close();
                }
            }
            if (entidad != null)
            {
                entidad.Imagenes = ObtenerImagenes(id);
            }
            return entidad;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    WHERE i.{nameof(Inmueble.IdPropietario)} = @idPropietario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int AgregarImagen(ImagenInmueble imagen)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO ImagenInmueble
                    ({nameof(ImagenInmueble.IdInmueble)}, {nameof(ImagenInmueble.Url)}, {nameof(ImagenInmueble.EsPortada)})
                    VALUES (@idInmueble, @url, @esPortada);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                    command.Parameters.AddWithValue("@url", imagen.Url);
                    command.Parameters.AddWithValue("@esPortada", imagen.EsPortada);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    imagen.IdImagen = res;
                    connection.Close();
                }
            }
            return res;
        }

        public IList<ImagenInmueble> ObtenerImagenes(int idInmueble)
        {
            List<ImagenInmueble> res = new List<ImagenInmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(ImagenInmueble.IdImagen)}, {nameof(ImagenInmueble.IdInmueble)}, {nameof(ImagenInmueble.Url)}, {nameof(ImagenInmueble.EsPortada)}
                    FROM ImagenInmueble
                    WHERE {nameof(ImagenInmueble.IdInmueble)} = @idInmueble
                    ORDER BY {nameof(ImagenInmueble.EsPortada)} DESC, {nameof(ImagenInmueble.IdImagen)}";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new ImagenInmueble
                        {
                            IdImagen = reader.GetInt32(reader.GetOrdinal(nameof(ImagenInmueble.IdImagen))),
                            IdInmueble = reader.GetInt32(reader.GetOrdinal(nameof(ImagenInmueble.IdInmueble))),
                            Url = reader.GetString(reader.GetOrdinal(nameof(ImagenInmueble.Url))),
                            EsPortada = reader.GetBoolean(reader.GetOrdinal(nameof(ImagenInmueble.EsPortada))),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Inmueble> ObtenerMasReservados(int dias)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Cuenta cuántas reservas tuvo cada inmueble en los últimos "dias" días,
                // y los ordena de más a menos reservado.
                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre,
                        COUNT(r.{nameof(Reserva.IdReserva)}) AS CantidadReservas
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    INNER JOIN Reserva r ON r.{nameof(Reserva.IdInmueble)} = i.{nameof(Inmueble.IdInmueble)}
                        AND r.{nameof(Reserva.FechaDesde)} >= @fechaLimite
                    GROUP BY i.{nameof(Inmueble.IdInmueble)}
                    ORDER BY CantidadReservas DESC
                    LIMIT 20
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaLimite", DateTime.Today.AddDays(-dias));
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var inmueble = MapearInmueble(reader);
                        inmueble.CantidadReservas = reader.GetInt32(reader.GetOrdinal("CantidadReservas"));
                        res.Add(inmueble);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerSinReservas(int dias)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Inmuebles que NO tienen ninguna reserva con FechaDesde
                // dentro de los últimos "dias" días.
                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    WHERE i.{nameof(Inmueble.IdInmueble)} NOT IN (
                        SELECT {nameof(Reserva.IdInmueble)} FROM Reserva WHERE {nameof(Reserva.FechaDesde)} >= @fechaLimite
                    )
                    ORDER BY i.{nameof(Inmueble.IdInmueble)}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaLimite", DateTime.Today.AddDays(-dias));
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Inmuebles Disponibles que NO tienen ninguna reserva que se
                // superponga con el rango de fechas pedido.
                string sql = @$"
                    SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                        i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)}, i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre, p.{nameof(Propietario.Apellido)} AS PropietarioApellido,
                        t.{nameof(TipoInmueble.Nombre)} AS TipoNombre
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.{nameof(TipoInmueble.IdTipoInmueble)}
                    WHERE i.{nameof(Inmueble.Estado)} = 'Disponible'
                        AND i.{nameof(Inmueble.IdInmueble)} NOT IN (
                            SELECT {nameof(Reserva.IdInmueble)} FROM Reserva
                            WHERE {nameof(Reserva.Estado)} NOT IN ('Terminada anticipadamente', 'Cancelada')
                                AND {nameof(Reserva.FechaDesde)} <= @fechaHasta
                                AND {nameof(Reserva.FechaHastaOriginal)} >= @fechaDesde
                        )
                    ORDER BY i.{nameof(Inmueble.IdInmueble)}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Date);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Date);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        private static Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdInmueble))),
                Direccion = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
                Cupo = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Cupo))),
                PrecioPorDia = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.PrecioPorDia))),
                PorcentajeReserva = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.PorcentajeReserva))),
                Latitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Latitud))) ? null : reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
                Longitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Longitud))) ? null : reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
                IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdTipoInmueble))),
                Estado = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Estado))),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                    Nombre = reader.GetString(reader.GetOrdinal("PropietarioNombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("PropietarioApellido")),
                },
                TipoInmueble = new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdTipoInmueble))),
                    Nombre = reader.GetString(reader.GetOrdinal("TipoNombre")),
                },
            };
        }
    }
}
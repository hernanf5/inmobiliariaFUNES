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
                // Acá "Baja" no significa borrar ni marcar inactivo genérico:
                // la narrativa pide que el propietario pueda "suspender" la oferta
                // de un inmueble, así que reusamos el campo Estado que ya existe
                // en el schema para eso. No afecta reservas ya creadas.
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
            IList<Inmueble> res = new List<Inmueble>();
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
                    ORDER BY i.{nameof(Inmueble.IdInmueble)}
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
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

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT COUNT({nameof(Inmueble.IdInmueble)}) FROM Inmueble";
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
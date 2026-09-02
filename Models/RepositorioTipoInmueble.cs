using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace inmobiliariaFUNES.Models
{
    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(TipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"INSERT INTO TipoInmueble
                    ({nameof(TipoInmueble.Nombre)})
                    VALUES (@nombre);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", t.Nombre);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.IdTipoInmueble = res;
                    connection.Close();
                }
                
            }
            return res;
        }    

        public int Baja(TipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE TipoInmueble 
                    SET {nameof(TipoInmueble.Activo)} = 0 WHERE {nameof(TipoInmueble.IdTipoInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", t.IdTipoInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(TipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"UPDATE TipoInmueble 
                    SET {nameof(TipoInmueble.Nombre)}=@nombre
                    WHERE {nameof(TipoInmueble.IdTipoInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", t.Nombre);
                    command.Parameters.AddWithValue("@id", t.IdTipoInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<TipoInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<TipoInmueble> res = new List<TipoInmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                SELECT {nameof(TipoInmueble.IdTipoInmueble)}, {nameof(TipoInmueble.Nombre)}, {nameof(TipoInmueble.Activo)}
                FROM TipoInmueble
                WHERE {nameof(TipoInmueble.Activo)} = 1
                ORDER BY {nameof(TipoInmueble.Nombre)}
                LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearTipoInmueble(reader));
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
                string sql = @$"SELECT COUNT({nameof(TipoInmueble.IdTipoInmueble)}) FROM TipoInmueble WHERE {nameof(TipoInmueble.Activo)} = 1";
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
        

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? t = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT {nameof(TipoInmueble.IdTipoInmueble)}, {nameof(TipoInmueble.Nombre)}, {nameof(TipoInmueble.Activo)}
                    FROM TipoInmueble
                    WHERE {nameof(TipoInmueble.IdTipoInmueble)} = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        t = MapearTipoInmueble(reader);
                    }
                    connection.Close();
                }
            }
            return t;
        }

        private static TipoInmueble MapearTipoInmueble(MySqlDataReader reader)
        {
            return new TipoInmueble
            {
                IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(TipoInmueble.IdTipoInmueble))),
                Nombre = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.Nombre))),
                Activo = reader.GetBoolean(reader.GetOrdinal(nameof(TipoInmueble.Activo))),
            };
        }
    
    }
}
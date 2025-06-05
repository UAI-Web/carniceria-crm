using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CarniceriaCRM.BE;

namespace CarniceriaCRM.DAL
{
    public class BitacoraDAL
    {
        private readonly string _connectionString;

        public BitacoraDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        public List<Bitacora> ObtenerTodas()
        {
            List<Bitacora> bitacoras = new List<Bitacora>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT b.Id, b.Descripcion, b.Accion, b.UsuarioId, b.FechaHora, 
                           b.DireccionIP, b.UserAgent,
                           u.Nombre, u.Apellido, u.Mail,
                            b.DigitoVerificadorH
                    FROM Bitacora b
                    LEFT JOIN Usuarios u ON b.UsuarioId = u.Id
                    ORDER BY b.FechaHora DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bitacoras.Add(MapearBitacora(reader));
                        }
                    }
                }
            }
            
            return bitacoras;
        }

        public Bitacora ObtenerPorId(int id)
        {
            Bitacora bitacora = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT b.Id, b.Descripcion, b.Accion, b.UsuarioId, b.FechaHora, 
                           b.DireccionIP, b.UserAgent,
                           u.Nombre, u.Apellido, u.Mail,
                            b.DigitoVerificadorH
                    FROM Bitacora b
                    LEFT JOIN Usuarios u ON b.UsuarioId = u.Id
                    WHERE b.Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bitacora = MapearBitacora(reader);
                        }
                    }
                }
            }
            
            return bitacora;
        }

        public void Registrar(Bitacora bitacora)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Bitacora (Descripcion, Accion, UsuarioId, FechaHora, DireccionIP, UserAgent)
                    VALUES (@Descripcion, @Accion, @UsuarioId, @FechaHora, @DireccionIP, @UserAgent);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", bitacora.Descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Accion", bitacora.Accion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UsuarioId", bitacora.IdUsuario);
                    command.Parameters.AddWithValue("@FechaHora", bitacora.FechaHora);
                    command.Parameters.AddWithValue("@DireccionIP", bitacora.DireccionIP ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserAgent", bitacora.UserAgent ?? (object)DBNull.Value);
                    
                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        bitacora.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        public void RegistrarLogin(Usuario usuario, string direccionIP, string userAgent)
        {
            var bitacora = new Bitacora
            {
                Descripcion = $"Login exitoso - Usuario: {usuario.Nombre} {usuario.Apellido}",
                Accion = "LOGIN_EXITOSO",
                IdUsuario = usuario.Id,
                FechaHora = DateTime.Now,
                DireccionIP = direccionIP,
                UserAgent = userAgent
            };

            Registrar(bitacora);
        }

        public void RegistrarLogout(Usuario usuario, string direccionIP, string userAgent)
        {
            var bitacora = new Bitacora
            {
                Descripcion = $"Logout - Usuario: {usuario.Nombre} {usuario.Apellido}",
                Accion = "LOGOUT",
                IdUsuario = usuario.Id,
                FechaHora = DateTime.Now,
                DireccionIP = direccionIP,
                UserAgent = userAgent
            };
            Registrar(bitacora);
        }

        public void RegistrarLoginFallido(string mail, string motivo, string direccionIP, string userAgent)
        {
            var bitacora = new Bitacora
            {
                Descripcion = $"Login fallido - Mail: {mail} - Motivo: {motivo}",
                Accion = "LOGIN_FALLIDO",
                IdUsuario = 0, // Usuario no autenticado
                FechaHora = DateTime.Now,
                DireccionIP = direccionIP,
                UserAgent = userAgent
            };
            Registrar(bitacora);
        }

        public void RegistrarBloqueoUsuario(Usuario usuario, string direccionIP, string userAgent)
        {
            var bitacora = new Bitacora
            {
                Descripcion = $"Usuario bloqueado - Usuario: {usuario.Nombre} {usuario.Apellido}",
                Accion = "USUARIO_BLOQUEADO",
                IdUsuario = usuario.Id,
                FechaHora = DateTime.Now,
                DireccionIP = direccionIP,
                UserAgent = userAgent
            };
            Registrar(bitacora);
        }

        public void RegistrarEvento(int idUsuario, string tipo, string descripcion, string direccionIP, string userAgent)
        {
            var bitacora = new Bitacora
            {
                Descripcion = descripcion,
                Accion = tipo,
                IdUsuario = idUsuario,
                FechaHora = DateTime.Now,
                DireccionIP = direccionIP,
                UserAgent = userAgent
            };
            Registrar(bitacora);
        }

        private Bitacora MapearBitacora(SqlDataReader reader)
        {
            var bitacora = new Bitacora
            {
                Id = Convert.ToInt32(reader["Id"]),
                Descripcion = reader["Descripcion"].ToString(),
                Accion = reader["Accion"]?.ToString(),
                IdUsuario = Convert.ToInt32(reader["UsuarioId"]),
                FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                DireccionIP = reader["DireccionIP"]?.ToString(),
                UserAgent = reader["UserAgent"]?.ToString()
            };

            // Crear objeto Usuario si hay información
            if (reader["Nombre"] != DBNull.Value)
            {
                bitacora.Usuario = new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                    Nombre = reader["Nombre"].ToString(),
                    Apellido = reader["Apellido"].ToString(),
                    Mail = reader["Mail"].ToString()
                };
            }

            return bitacora;
        }
    }
}
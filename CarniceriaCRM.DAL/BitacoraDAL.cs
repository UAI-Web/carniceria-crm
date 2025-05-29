using System;
using System.Collections.Generic;
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
                ?? "Server=localhost;Database=CarniceriaCRM;Integrated Security=true;";
        }

        /// <summary>
        /// Registra una nueva entrada en la bitácora
        /// </summary>
        public void RegistrarEvento(int usuarioId, string accion, string descripcion, string direccionIP = null, string userAgent = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Bitacora (UsuarioId, Accion, Descripcion, DireccionIP, UserAgent, FechaHora)
                    VALUES (@UsuarioId, @Accion, @Descripcion, @DireccionIP, @UserAgent, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    command.Parameters.AddWithValue("@Accion", accion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DireccionIP", direccionIP ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserAgent", userAgent ?? (object)DBNull.Value);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Registra el inicio de sesión exitoso
        /// </summary>
        public void RegistrarLogin(Usuario usuario, string direccionIP = null, string userAgent = null)
        {
            string descripcion = $"Usuario {usuario.Nombre} {usuario.Apellido} ({usuario.Mail}) inició sesión exitosamente";
            RegistrarEvento(Convert.ToInt32(usuario.Id), "LOGIN_EXITOSO", descripcion, direccionIP, userAgent);
        }

        /// <summary>
        /// Registra un intento de login fallido
        /// </summary>
        public void RegistrarLoginFallido(string mail, string motivo, string direccionIP = null, string userAgent = null)
        {
            // Intentamos obtener el usuario para registrar con su ID, si no existe usamos ID = 0
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryUsuario = "SELECT Id FROM Usuarios WHERE Mail = @Mail";
                int usuarioId = 0;
                
                using (SqlCommand command = new SqlCommand(queryUsuario, connection))
                {
                    command.Parameters.AddWithValue("@Mail", mail ?? (object)DBNull.Value);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        usuarioId = Convert.ToInt32(result);
                    }
                }
                
                string descripcion = $"Intento de login fallido para {mail}. Motivo: {motivo}";
                
                // Insertamos en bitácora
                string queryBitacora = @"
                    INSERT INTO Bitacora (UsuarioId, Accion, Descripcion, DireccionIP, UserAgent, FechaHora)
                    VALUES (@UsuarioId, @Accion, @Descripcion, @DireccionIP, @UserAgent, GETDATE())";

                using (SqlCommand command = new SqlCommand(queryBitacora, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    command.Parameters.AddWithValue("@Accion", "LOGIN_FALLIDO");
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@DireccionIP", direccionIP ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserAgent", userAgent ?? (object)DBNull.Value);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Registra el bloqueo de un usuario
        /// </summary>
        public void RegistrarBloqueoUsuario(Usuario usuario, string direccionIP = null, string userAgent = null)
        {
            string descripcion = $"Usuario {usuario.Nombre} {usuario.Apellido} ({usuario.Mail}) fue bloqueado por múltiples intentos fallidos";
            RegistrarEvento(Convert.ToInt32(usuario.Id), "USUARIO_BLOQUEADO", descripcion, direccionIP, userAgent);
        }

        /// <summary>
        /// Registra el cierre de sesión
        /// </summary>
        public void RegistrarLogout(Usuario usuario, string direccionIP = null, string userAgent = null)
        {
            string descripcion = $"Usuario {usuario.Nombre} {usuario.Apellido} ({usuario.Mail}) cerró sesión";
            RegistrarEvento(Convert.ToInt32(usuario.Id), "LOGOUT", descripcion, direccionIP, userAgent);
        }

        /// <summary>
        /// Obtiene el historial de eventos de un usuario específico
        /// </summary>
        public List<EventoBitacora> ObtenerEventosUsuario(int usuarioId, int top = 50)
        {
            List<EventoBitacora> eventos = new List<EventoBitacora>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT TOP ({top}) 
                        b.Id, b.UsuarioId, b.Accion, b.Descripcion, b.FechaHora, 
                        b.DireccionIP, b.UserAgent,
                        u.Nombre, u.Apellido, u.Mail
                    FROM Bitacora b
                    INNER JOIN Usuarios u ON b.UsuarioId = u.Id
                    WHERE b.UsuarioId = @UsuarioId
                    ORDER BY b.FechaHora DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventos.Add(MapearEventoBitacora(reader));
                        }
                    }
                }
            }
            
            return eventos;
        }

        /// <summary>
        /// Obtiene los eventos más recientes del sistema
        /// </summary>
        public List<EventoBitacora> ObtenerEventosRecientes(int top = 100)
        {
            List<EventoBitacora> eventos = new List<EventoBitacora>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT TOP ({top}) 
                        b.Id, b.UsuarioId, b.Accion, b.Descripcion, b.FechaHora, 
                        b.DireccionIP, b.UserAgent,
                        u.Nombre, u.Apellido, u.Mail
                    FROM Bitacora b
                    INNER JOIN Usuarios u ON b.UsuarioId = u.Id
                    ORDER BY b.FechaHora DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventos.Add(MapearEventoBitacora(reader));
                        }
                    }
                }
            }
            
            return eventos;
        }

        /// <summary>
        /// Obtiene estadísticas de login del sistema
        /// </summary>
        public EstadisticasLogin ObtenerEstadisticasLogin(DateTime fechaDesde, DateTime fechaHasta)
        {
            EstadisticasLogin stats = new EstadisticasLogin();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        SUM(CASE WHEN Accion = 'LOGIN_EXITOSO' THEN 1 ELSE 0 END) as LoginExitosos,
                        SUM(CASE WHEN Accion = 'LOGIN_FALLIDO' THEN 1 ELSE 0 END) as LoginFallidos,
                        SUM(CASE WHEN Accion = 'USUARIO_BLOQUEADO' THEN 1 ELSE 0 END) as UsuariosBloqueados
                    FROM Bitacora 
                    WHERE FechaHora >= @FechaDesde AND FechaHora <= @FechaHasta";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@FechaHasta", fechaHasta);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.LoginExitosos = reader["LoginExitosos"] != DBNull.Value ? Convert.ToInt32(reader["LoginExitosos"]) : 0;
                            stats.LoginFallidos = reader["LoginFallidos"] != DBNull.Value ? Convert.ToInt32(reader["LoginFallidos"]) : 0;
                            stats.UsuariosBloqueados = reader["UsuariosBloqueados"] != DBNull.Value ? Convert.ToInt32(reader["UsuariosBloqueados"]) : 0;
                        }
                    }
                }
            }
            
            return stats;
        }

        /// <summary>
        /// Mapea un SqlDataReader a un objeto EventoBitacora
        /// </summary>
        private EventoBitacora MapearEventoBitacora(SqlDataReader reader)
        {
            return new EventoBitacora
            {
                Id = Convert.ToInt32(reader["Id"]),
                UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                Accion = reader["Accion"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                DireccionIP = reader["DireccionIP"]?.ToString(),
                UserAgent = reader["UserAgent"]?.ToString(),
                NombreUsuario = reader["Nombre"].ToString(),
                ApellidoUsuario = reader["Apellido"].ToString(),
                MailUsuario = reader["Mail"].ToString()
            };
        }
    }

    /// <summary>
    /// Clase para representar un evento de bitácora con información del usuario
    /// </summary>
    public class EventoBitacora
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
        public string DireccionIP { get; set; }
        public string UserAgent { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string MailUsuario { get; set; }
    }

    /// <summary>
    /// Clase para estadísticas de login
    /// </summary>
    public class EstadisticasLogin
    {
        public int LoginExitosos { get; set; }
        public int LoginFallidos { get; set; }
        public int UsuariosBloqueados { get; set; }
    }
} 
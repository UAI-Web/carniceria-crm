using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CarniceriaCRM.BE;
using System.Linq;

namespace CarniceriaCRM.DAL
{
    public class UsuarioDAL
    {
        private readonly string _connectionString;

        public UsuarioDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        /// <summary>
        /// Obtiene todos los usuarios activos
        /// </summary>
        public List<Usuario> Listar()
        {
            List<Usuario> usuarios = new List<Usuario>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT Id, Nombre, Apellido, Mail, Clave, IntentosFallidos, Bloqueado, 
                           FechaCreacion, FechaUltimaModificacion, Activo
                    FROM Usuarios 
                    WHERE Activo = 1
                    ORDER BY Nombre, Apellido";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(MapearUsuario(reader));
                        }
                    }
                }
            }
            
            return usuarios;
        }

        /// <summary>
        /// Obtiene un usuario por su email con sus roles y permisos
        /// </summary>
        public Usuario ObtenerPorMail(string mail)
        {
            Usuario usuario = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT Id, Nombre, Apellido, Mail, Clave, IntentosFallidos, Bloqueado, 
                           FechaCreacion, FechaUltimaModificacion, Activo
                    FROM Usuarios 
                    WHERE Mail = @Mail AND Activo = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Mail", mail);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }
            
            // Cargar roles y permisos si el usuario existe
            if (usuario != null)
            {
                CargarFamiliasYPermisos(usuario);
            }
            
            return usuario;
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT Id, Nombre, Apellido, Mail, Clave, IntentosFallidos, Bloqueado, 
                           FechaCreacion, FechaUltimaModificacion, Activo
                    FROM Usuarios 
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }
            
            return usuario;
        }

        /// <summary>
        /// Inserta un nuevo usuario
        /// </summary>
        public void Insertar(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Usuarios (Nombre, Apellido, Mail, Clave, IntentosFallidos, Bloqueado, Activo)
                    VALUES (@Nombre, @Apellido, @Mail, @Clave, @IntentosFallidos, @Bloqueado, @Activo);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AgregarParametrosUsuario(command, usuario);
                    
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        usuario.Id = Convert.ToInt32(result).ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Modifica un usuario existente
        /// </summary>
        public void Modificar(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Usuarios 
                    SET Nombre = @Nombre, 
                        Apellido = @Apellido, 
                        Mail = @Mail, 
                        Clave = @Clave,
                        IntentosFallidos = @IntentosFallidos,
                        Bloqueado = @Bloqueado,
                        Activo = @Activo,
                        FechaUltimaModificacion = GETDATE()
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AgregarParametrosUsuario(command, usuario);
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Elimina (desactiva) un usuario
        /// </summary>
        public void Borrar(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Usuarios 
                    SET Activo = 0, FechaUltimaModificacion = GETDATE()
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Incrementa el contador de intentos fallidos
        /// </summary>
        public void IncrementarIntento(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Usuarios 
                    SET IntentosFallidos = IntentosFallidos + 1,
                        FechaUltimaModificacion = GETDATE()
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Resetea el contador de intentos fallidos
        /// </summary>
        public void ResetearIntentos(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Usuarios 
                    SET IntentosFallidos = 0,
                        FechaUltimaModificacion = GETDATE()
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader["Id"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                Mail = reader["Mail"].ToString(),
                Clave = reader["Clave"].ToString(),
                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                Bloqueado = Convert.ToBoolean(reader["Bloqueado"])
            };
        }

        private void AgregarParametrosUsuario(SqlCommand command, Usuario usuario)
        {
            command.Parameters.AddWithValue("@Nombre", usuario.Nombre ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Apellido", usuario.Apellido ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Mail", usuario.Mail ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Clave", usuario.Clave ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IntentosFallidos", usuario.IntentosFallidos);
            command.Parameters.AddWithValue("@Bloqueado", usuario.Bloqueado);
            command.Parameters.AddWithValue("@Activo", true);
        }

        public List<Usuario> ObtenerTodos()
        {
            return Listar();
        }

        /// <summary>
        /// Carga las familias (roles) y patentes (permisos) del usuario
        /// </summary>
        private void CargarFamiliasYPermisos(Usuario usuario)
        {
            if (usuario == null) return;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Query para obtener familias del usuario con sus patentes
                string query = @"
                    SELECT DISTINCT 
                        f.Id as FamiliaId,
                        f.Nombre as FamiliaNombre,
                        f.Descripcion as FamiliaDescripcion,
                        f.Activo as FamiliaActivo,
                        f.FechaCreacion as FamiliaFechaCreacion,
                        p.Id as PatenteId,
                        p.Nombre as PatenteNombre,
                        p.Descripcion as PatenteDescripcion,
                        p.Permiso as PatentePermiso,
                        p.Activo as PatenteActivo,
                        p.FechaCreacion as PatenteFechaCreacion
                    FROM Usuarios u
                    INNER JOIN UsuarioFamilia uf ON u.Id = uf.UsuarioId
                    INNER JOIN Familias f ON uf.FamiliaId = f.Id
                    LEFT JOIN FamiliaPatente fp ON f.Id = fp.FamiliaId
                    LEFT JOIN Patentes p ON fp.PatenteId = p.Id
                    WHERE u.Id = @UsuarioId 
                        AND u.Activo = 1 
                        AND uf.Activo = 1 
                        AND f.Activo = 1
                        AND (p.Id IS NULL OR p.Activo = 1)
                    ORDER BY f.Nombre, p.Nombre";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var familiasDict = new Dictionary<int, Familia>();
                        
                        while (reader.Read())
                        {
                            int familiaId = Convert.ToInt32(reader["FamiliaId"]);
                            
                            // Crear familia si no existe en el diccionario
                            if (!familiasDict.ContainsKey(familiaId))
                            {
                                var familia = new Familia
                                {
                                    Id = familiaId,
                                    Nombre = reader["FamiliaNombre"].ToString(),
                                    Descripcion = reader["FamiliaDescripcion"].ToString(),
                                    Activo = Convert.ToBoolean(reader["FamiliaActivo"]),
                                    FechaCreacion = Convert.ToDateTime(reader["FamiliaFechaCreacion"])
                                };
                                
                                familiasDict[familiaId] = familia;
                            }
                            
                            // Agregar patente si existe (puede haber familias sin patentes)
                            if (!reader.IsDBNull(reader.GetOrdinal("PatenteId")))
                            {
                                var patente = new Patente
                                {
                                    Id = Convert.ToInt32(reader["PatenteId"]),
                                    Nombre = reader["PatenteNombre"].ToString(),
                                    Descripcion = reader["PatenteDescripcion"].ToString(),
                                    Permiso = reader["PatentePermiso"].ToString(),
                                    Activo = Convert.ToBoolean(reader["PatenteActivo"]),
                                    FechaCreacion = Convert.ToDateTime(reader["PatenteFechaCreacion"])
                                };
                                
                                // Verificar que la patente no esté duplicada
                                if (!familiasDict[familiaId].Patentes.Any(p => p.Id == patente.Id))
                                {
                                    familiasDict[familiaId].Patentes.Add(patente);
                                }
                            }
                        }
                        
                        // Asignar familias al usuario
                        usuario.Familias = familiasDict.Values.ToList();
                    }
                }
            }
        }
    }
}

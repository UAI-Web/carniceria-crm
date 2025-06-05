using CarniceriaCRM.BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.DAL
{
    public class ProductoDAL
    {
        private readonly string _connectionString;
        public static readonly string TableName = "Productos";

        public ProductoDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        /// <summary>
        /// Obtiene un pruducto por su ID
        /// </summary>
        public Producto ObtenerPorId(int id)
        {
            Producto item = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT Id, Nombre, Descripcion, CategoriaId, ProveedorId, PrecioCompra, PrecioVenta, StockMinimo, StockActual, UnidadMedida, DigitoVerificadorH
                    FROM {TableName}
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = Mapear(reader);
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        public List<Producto> Listar()
        {
            List<Producto> items = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT Id, Nombre, Descripcion, CategoriaId, ProveedorId, PrecioCompra, PrecioVenta, StockMinimo, StockActual, UnidadMedida, DigitoVerificadorH
                    FROM {TableName}
                    WHERE Activo = 1;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(Mapear(reader));
                        }
                    }
                }
            }

            return items;
        }

        private Producto Mapear(SqlDataReader reader)
        {
            return new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader["Nombre"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                ProveedorId = reader.GetInt32(reader.GetOrdinal("ProveedorId")),
                PrecioCompra = reader.GetDecimal(reader.GetOrdinal("PrecioCompra")),
                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                StockActual = reader.GetInt32(reader.GetOrdinal("StockActual")),
                UnidadMedida = reader["UnidadMedida"].ToString(),
                DigitoVerificadorH = reader["DigitoVerificadorH"].ToString()
            };
        }

        public void ActualizarDVH(int id, string digitoVerificador)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    UPDATE {TableName}
                    SET DigitoVerificadorH = @DigitoVerificadorH
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@DigitoVerificadorH", digitoVerificador);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
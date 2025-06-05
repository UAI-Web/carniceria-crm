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
    public class ClienteDAL
    {
        private readonly string _connectionString;
        public static readonly string TableName = "Clientes";

        public ClienteDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        public Cliente ObtenerPorId(int id)
        {
            Cliente item = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT Id, Nombre, Apellido, Email, Telefono, Direccion, Ciudad, CodigoPostal, LimiteCredito, SaldoActual, DigitoVerificadorH
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
        /// Obtiene todos los usuarios activos
        /// </summary>
        public List<Cliente> Listar()
        {
            List<Cliente> items = new List<Cliente>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                    SELECT Id, Nombre, Apellido, Email, Telefono, Direccion, Ciudad, CodigoPostal, LimiteCredito, SaldoActual, DigitoVerificadorH
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

        private Cliente Mapear(SqlDataReader reader)
        {
            return new Cliente
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                Email = reader["Email"].ToString(),
                Telefono = reader["Telefono"].ToString(),
                Direccion = reader["Direccion"].ToString(),
                Ciudad = reader["Ciudad"].ToString(),
                CodigoPostal = reader["CodigoPostal"].ToString(),
                LimiteCredito = reader.GetDecimal(reader.GetOrdinal("LimiteCredito")),
                SaldoActual = reader.GetDecimal(reader.GetOrdinal("SaldoActual")),
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
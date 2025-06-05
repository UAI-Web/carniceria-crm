using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.DAL
{
    /// <summary>
    /// Acceso a la tabla DigitoVerificadorV (DVV) para operaciones CRUD.
    /// </summary>
    public class DigitoVerificadorVDAL
    {
        private readonly string _connectionString;

        public DigitoVerificadorVDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
    ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        /// <summary>
        /// Obtiene el valor DVV para la tabla especificada.
        /// </summary>
        public string ObtenerDVV(string tabla)
        {
            string dvv = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ValorDV FROM DigitoVerificadorV WHERE Tabla = @Tabla",
                    connection))
                {
                    connection.Open();

                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    
                    object result = cmd.ExecuteScalar();
                    
                    if (result != null && result != DBNull.Value)
                        dvv = result.ToString();
                }

                return dvv;
            }
        }

        /// <summary>
        /// Actualiza o inserta el valor DVV para la tabla especificada (upsert).
        /// </summary>
        public void ActualizarDVV(string tabla, string valorDV)
        {
            SqlTransaction tx = null;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                tx = connection.BeginTransaction();

                bool exists;

                using (SqlCommand cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM DigitoVerificadorV WHERE Tabla = @Tabla",
                    connection, tx))
                {
                    cmdCheck.Parameters.AddWithValue("@Tabla", tabla);
                    exists = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0;
                }

                if (exists)
                {
                    using (SqlCommand cmdUpd = new SqlCommand(
                        "UPDATE DigitoVerificadorV SET ValorDV = @ValorDV WHERE Tabla = @Tabla",
                        connection, tx))
                    {
                        cmdUpd.Parameters.AddWithValue("@ValorDV", valorDV);
                        cmdUpd.Parameters.AddWithValue("@Tabla", tabla);
                        cmdUpd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand cmdIns = new SqlCommand(
                        "INSERT INTO DigitoVerificadorV (Tabla, ValorDV) VALUES (@Tabla, @ValorDV)",
                        connection, tx))
                    {
                        cmdIns.Parameters.AddWithValue("@Tabla", tabla);
                        cmdIns.Parameters.AddWithValue("@ValorDV", valorDV);
                        cmdIns.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }
            catch
            {
                if (tx != null)
                    tx.Rollback();
                throw;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}
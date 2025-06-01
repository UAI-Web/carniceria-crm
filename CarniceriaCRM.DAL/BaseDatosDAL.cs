using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.DAL
{
    public class BaseDatosDAL
    {
        private readonly string _connectionString;

        public BaseDatosDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CarniceriaCRM"]?.ConnectionString
    ?? "Server=.\\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;";
        }

        public bool Backup()
        {
            bool resultado = false;

            string archivoBackup = ConfigurationManager.AppSettings["ArchivoBackup"]?.ToString();

            if (string.IsNullOrEmpty(archivoBackup))
                throw new Exception("No se especificó el archivo de backup de la base de datos");

            if (System.IO.File.Exists(archivoBackup))
                System.IO.File.Delete(archivoBackup);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = String.Format(@"USE master;
                BACKUP DATABASE CarniceriaCRM
                TO DISK = '{0}';
                USE CarniceriaCRM;", archivoBackup);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    resultado = true;
                }
            }

            return resultado;
        }

        public bool CalculateVD()
        {

            return true;
        }

        public bool RestoreAvailable()
        {
            bool resultado = false;

            string archivoBackup = ConfigurationManager.AppSettings["ArchivoBackup"]?.ToString();

            if (string.IsNullOrEmpty(archivoBackup))
                throw new Exception("No se especificó el archivo de backup de la base de datos");

            if (System.IO.File.Exists(archivoBackup))
                resultado = true;

            return resultado;
        }

        public bool Restore()
        {
            bool resultado = false;

            string archivoBackup = ConfigurationManager.AppSettings["ArchivoBackup"]?.ToString();

            if (string.IsNullOrEmpty(archivoBackup))
                throw new Exception("No se especificó el archivo de backup de la base de datos");

            if (!System.IO.File.Exists(archivoBackup))
                throw new Exception("No se encontró el archivo de backup para restaurar la base de datos");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = String.Format(@"USE master;
                ALTER DATABASE CarniceriaCRM SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE CarniceriaCRM
                FROM DISK = '{0}'
                WITH REPLACE, RECOVERY, STATS=5;
                ALTER DATABASE CarniceriaCRM SET MULTI_USER;
                USE CarniceriaCRM;", archivoBackup);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    resultado = true;
                }
            }

            return resultado;
        }
    }
}
using CarniceriaCRM.BE;
using CarniceriaCRM.BLL.DigitVerifier;
using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL
{
    /// <summary>
    /// Gestor de DVH/DVV para la entidad Empleado.
    /// </summary>
    public class DigitVerifierManager
    {
        private List<IntegrityTable> integrityTables = new List<IntegrityTable>(); //("Usuarios");

        public DigitVerifierManager()
        {
            integrityTables.Add(new IntegrityTable(UsuarioDAL.TableName, new UsuarioService()));
            integrityTables.Add(new IntegrityTable(CategoriaDAL.TableName, new CategoriaService()));
            integrityTables.Add(new IntegrityTable(ClienteDAL.TableName, new ClienteService()));
            integrityTables.Add(new IntegrityTable(ProveedorDAL.TableName, new ProveedorService()));
            integrityTables.Add(new IntegrityTable(ProductoDAL.TableName, new ProductoService()));

        }

        public IntegrityResume VerifyIntegrity()
        {
            IntegrityResume result = new IntegrityResume();

            foreach (IntegrityTable table in integrityTables)
            {
                DigitVerifierService service = (DigitVerifierService)table.ClassType;
                IntegrityResult serviceResult = service.VerifyIntegrity();

                if (!serviceResult.Result)
                {
                    result.Result = false;
                    result.DVHErrors.AddRange(serviceResult.DHErrors);
                    result.DVVErrors.AddRange(serviceResult.DVErrors);
                    result.DVTables.Add(table.Name);
                }
            }

            return result;
        }

        public void RecalcularDV()
        {
            foreach (IntegrityTable table in integrityTables)
            {
                DigitVerifierService service = (DigitVerifierService)table.ClassType;
                service.RecalcularDV();
            }
        }
    }
}
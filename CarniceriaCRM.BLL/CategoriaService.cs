using CarniceriaCRM.BE;
using CarniceriaCRM.BLL.DigitVerifier;
using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL
{
    public class CategoriaService : DigitVerifierService
    {
        private readonly CategoriaDAL _dal;

        public CategoriaService()
        {
            _dal = new CategoriaDAL();
        }

        public List<Categoria> Listar()
        {
            return _dal.Listar();
        }

        public override IntegrityResult VerifyIntegrity()
        {
            IntegrityResult result = new IntegrityResult();

            IDigitVerifier<Categoria> verifier = new DigitVerifier<Categoria>();

            var todos = _dal.Listar();

            // Verificar DVH fila por fila
            foreach (var item in todos)
            {
                string expectedDVH = verifier.ComputeDVH(item);

                if (item.DigitoVerificadorH != expectedDVH)
                {
                    result.DHErrors.Add(new IntegrityError($"DVH corrupto en {CategoriaDAL.TableName} [{item.Id}]: se esperaba '{expectedDVH}', se encontró {((!string.IsNullOrEmpty(item.DigitoVerificadorH)) ? "'" + item.DigitoVerificadorH + "'" : "vacío")}."));
                    result.Result = false;
                }
            }

            // Verificar DVV global
            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            string storedDVV = dvvDAL.ObtenerDVV(CategoriaDAL.TableName);
            string expectedDVV = verifier.ComputeDVV(todos);

            if (storedDVV != expectedDVV)
            {
                result.DVErrors.Add(new IntegrityError($"DVV corrupto en {CategoriaDAL.TableName}: se esperaba '{expectedDVV}', se encontró {((!string.IsNullOrEmpty(storedDVV)) ? "'" + storedDVV + "'" : "vacío")}."));
                result.Result = false;
            }

            return result;
        }

        public override void RecalcularDV()
        {
            IDigitVerifier<Categoria> verifier = new DigitVerifier<Categoria>();

            var todos = _dal.Listar();

            foreach (var item in todos)
            {
                string dvH = verifier.ComputeDVH(item);

                if (item.DigitoVerificadorH != dvH)
                    _dal.ActualizarDVH(item.Id, dvH);
            }

            string dvV = verifier.ComputeDVV(todos);

            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            dvvDAL.ActualizarDVV(CategoriaDAL.TableName, dvV);
        }

        public override void RecalcularSingleDV(int id)
        {
            IDigitVerifier<Categoria> verifier = new DigitVerifier<Categoria>();

            Categoria item = _dal.ObtenerPorId(id);

            string dvH = verifier.ComputeDVH(item);

            if (item.DigitoVerificadorH != dvH)
                _dal.ActualizarDVH(item.Id, dvH);

            var todos = _dal.Listar();

            string dvV = verifier.ComputeDVV(todos);

            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            dvvDAL.ActualizarDVV(CategoriaDAL.TableName, dvV);
        }
    }
}
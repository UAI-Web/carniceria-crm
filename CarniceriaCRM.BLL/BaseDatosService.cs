using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL
{
    public class BaseDatosService
    {
        private BaseDatosDAL _baseDatosDAL;

        public BaseDatosService()
        {
            _baseDatosDAL = new BaseDatosDAL();
        }

        public bool Backup()
        {
            return _baseDatosDAL.Backup();
        }

        public bool CalculateVD()
        {
            return _baseDatosDAL.CalculateVD();
        }

        public bool RestoreAvailable()
        {
            return _baseDatosDAL.RestoreAvailable();
        }

        public bool Restore()
        {
            return _baseDatosDAL.Restore();
        }
    }
}
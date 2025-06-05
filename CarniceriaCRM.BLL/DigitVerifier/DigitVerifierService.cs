using CarniceriaCRM.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public abstract class DigitVerifierService
    {
        public abstract IntegrityResult VerifyIntegrity();

        public abstract void RecalcularDV();

        public abstract void RecalcularSingleDV(int id);
    }
}
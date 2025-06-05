using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public class IntegrityResult
    {
        public bool Result { get; set; } = true;

        public List<IntegrityError> DHErrors { get; set; } = new List<IntegrityError>();

        public List<IntegrityError> DVErrors { get; set; } = new List<IntegrityError>();
    }
}
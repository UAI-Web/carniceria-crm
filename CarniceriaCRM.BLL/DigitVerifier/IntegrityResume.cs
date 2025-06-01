using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public class IntegrityResume
    {
        public bool Result { get; set; } = true;

        public List<IntegrityError> DVHErrors { get; set; } = new List<IntegrityError>();

        public List<IntegrityError> DVVErrors { get; set; } = new List<IntegrityError>();

        public List<string> DVTables { get; set; } = new List<string> { };
    }
}
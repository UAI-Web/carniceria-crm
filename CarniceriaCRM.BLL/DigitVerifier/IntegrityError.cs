using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public class IntegrityError
    {
        public string Message { get; set; }

        public IntegrityError()
        {
        }

        public IntegrityError(string message)
        {
            Message = message;
        }
    }
}
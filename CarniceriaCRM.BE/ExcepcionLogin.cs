using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public class ExcepcionLogin:Exception
    {
        public ResultadoLogin resultado;
        public ExcepcionLogin(ResultadoLogin rex)
        {
            resultado = rex;
        }
    }
}

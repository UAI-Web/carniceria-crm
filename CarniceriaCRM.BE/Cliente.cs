using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public class Cliente : DigitVerifierBase
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public string Ciudad { get; set; }

        public string CodigoPostal { get; set; }

        public decimal LimiteCredito { get; set; }

        public decimal SaldoActual { get; set; }
    }
}
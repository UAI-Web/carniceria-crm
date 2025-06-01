using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public class Proveedor : DigitVerifierBase
    {
        public int Id { get; set; }

        public string NombreEmpresa { get; set; }

        public string ContactoNombre { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public string Ciudad { get; set; }

        public string CodigoPostal { get; set; }

        public string CUIT { get; set; }
    }
}
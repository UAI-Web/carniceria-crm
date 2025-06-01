using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public class Producto : DigitVerifierBase
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int CategoriaId { get; set; }

        public int ProveedorId { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public int StockMinimo { get; set; }

        public int StockActual { get; set; }

        public string UnidadMedida { get; set; }
    }
}
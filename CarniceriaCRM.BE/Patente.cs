using System;

namespace CarniceriaCRM.BE
{
    public class Patente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Permiso { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Patente()
        {
            Activo = true;
            FechaCreacion = DateTime.Now;
        }
    }
} 
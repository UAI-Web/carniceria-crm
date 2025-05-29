using System;
using System.Collections.Generic;

namespace CarniceriaCRM.BE
{
    public class Familia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Propiedades para navegación (se llenan desde DAL)
        public List<Patente> Patentes { get; set; }
        public List<Usuario> Usuarios { get; set; }

        public Familia()
        {
            Patentes = new List<Patente>();
            Usuarios = new List<Usuario>();
            Activo = true;
            FechaCreacion = DateTime.Now;
        }
    }
} 
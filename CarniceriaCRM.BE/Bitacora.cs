using System;

namespace CarniceriaCRM.BE
{
    public class Bitacora : DigitVerifierBase
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Accion { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string DireccionIP { get; set; }
        public string UserAgent { get; set; }
        
        // Propiedad de navegación para el usuario
        public Usuario Usuario { get; set; }
    }
} 
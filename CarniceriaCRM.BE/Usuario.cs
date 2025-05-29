using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public class Usuario
    {
        public string Id { get; set; }

		private string nombre;

		public string  Nombre
		{
			get { return nombre; }
			set { nombre = value; }
		}

		public string Apellido { get; set; }

        /// <summary>
        /// ojo que hay que guardar la clave encriptada!
        /// </summary>
        public string Clave { get; set; }

        public string Mail { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public bool Bloqueado { get; set; }

        public bool Activo { get; set; } = true;

        /// <summary>
        /// Lista de familias (roles) asignadas al usuario
        /// </summary>
        public List<Familia> Familias { get; set; } = new List<Familia>();

        /// <summary>
        /// Verifica si el usuario tiene un permiso específico
        /// </summary>
        public bool TienePermiso(string permiso)
        {
            if (Familias == null) return false;
            return Familias.Exists(f => f.Patentes.Exists(p => p.Permiso.Equals(permiso, StringComparison.OrdinalIgnoreCase) && p.Activo));
        }

        /// <summary>
        /// Obtiene todos los permisos del usuario
        /// </summary>
        public List<Patente> ObtenerTodosLosPermisos()
        {
            var todosLosPermisos = new List<Patente>();
            foreach (var familia in Familias)
            {
                todosLosPermisos.AddRange(familia.Patentes.Where(p => p.Activo));
            }
            return todosLosPermisos.Distinct().ToList();
        }

        /// <summary>
        /// Verifica si el usuario tiene algún rol específico
        /// </summary>
        public bool TieneRol(string nombreRol)
        {
            return Familias.Exists(f => f.Nombre.Equals(nombreRol, StringComparison.OrdinalIgnoreCase) && f.Activo);
        }

        /// <summary>
        /// Obtiene los nombres de todos los roles del usuario
        /// </summary>
        public List<string> ObtenerRoles()
        {
            return Familias.Where(f => f.Activo).Select(f => f.Nombre).ToList();
        }

        /// <summary>
        /// Obtiene una representación amigable del usuario
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
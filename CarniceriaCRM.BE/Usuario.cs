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

        /// <summary>
        /// Lista de familias (roles) asignadas al usuario
        /// </summary>
        public List<Familia> Familias { get; set; } = new List<Familia>();

        /// <summary>
        /// Verifica si el usuario tiene un permiso específico
        /// </summary>
        public bool TienePermiso(PermisosEnum permiso)
        {
            foreach (var familia in Familias)
            {
                var permisos = familia.ObtenerTodosLosPermisos();
                if (permisos.Any(p => p.Permiso == permiso.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Obtiene todos los permisos del usuario
        /// </summary>
        public List<Patente> ObtenerTodosLosPermisos()
        {
            var todosLosPermisos = new List<Patente>();
            foreach (var familia in Familias)
            {
                todosLosPermisos.AddRange(familia.ObtenerTodosLosPermisos());
            }
            return todosLosPermisos.Distinct().ToList();
        }

        /// <summary>
        /// Verifica si el usuario tiene algún rol específico
        /// </summary>
        public bool TieneRol(string nombreRol)
        {
            return Familias.Any(f => f.Nombre.Equals(nombreRol, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtiene los nombres de todos los roles del usuario
        /// </summary>
        public List<string> ObtenerRoles()
        {
            return Familias.Select(f => f.Nombre).ToList();
        }
    }
}
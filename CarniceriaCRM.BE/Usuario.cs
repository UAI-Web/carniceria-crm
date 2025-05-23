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

    }
}
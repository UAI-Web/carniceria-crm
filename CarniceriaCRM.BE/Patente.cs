using System;
using System.Collections.Generic;

namespace CarniceriaCRM.BE
{
    public class Patente : Componente
    {
        public override List<Componente> Hijos
        {
            get
            {
                return new List<Componente>();
            }
        }

        public override void AgregarHijo(Componente c)
        {
            throw new InvalidOperationException("No se pueden agregar hijos a una Patente (permiso individual).");
        }

        public override void EliminarHijo(Componente c)
        {
            throw new InvalidOperationException("No se pueden eliminar hijos de una Patente (permiso individual).");
        }

        public override void VaciarHijos()
        {
            throw new InvalidOperationException("Una Patente no tiene hijos para vaciar.");
        }
    }
} 
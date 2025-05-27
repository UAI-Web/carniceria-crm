using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BE
{
    public enum ResultadoLogin
    {
        MailInvalido,
        ContraseñaInvalida,
        UsuarioValido,
        SesionYaIniciada,
        NoHaySesion,
        UsuarioBloqueado
    }
}

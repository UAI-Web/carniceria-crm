using CarniceriaCRM.BE;
using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL
{
    public class UsuarioService
    {
        UsuarioDAL dalusu = new UsuarioDAL();
        public ResultadoLogin Login(string mail, string password)
        {
            //Le paso la contraseña y mail ingresadas sin hashear, porque son las que mete el usuario,
            //pero todas las que vengan de la BD vienen hasheado, por eso lo hasheo adentro del método para commpararlo con el de la BD

            if (SesionSingleton.Instancia.EstaLogueado()) //Si ya esta logueado no avanza
            {
                throw new ExcepcionLogin(ResultadoLogin.SesionYaIniciada);
            }
            Usuario usu = new Usuario();
            usu = Listar().Where(u => u.Mail.Equals(mail)).FirstOrDefault(); //Agarro de la BD el usuario que corresponde al mail
            if(usu == null)
            {
                throw new ExcepcionLogin(ResultadoLogin.MailInvalido); //No existe el usuario con ese mail
            }
            if(usu.Bloqueado == true)
            {
                throw new ExcepcionLogin(ResultadoLogin.UsuarioBloqueado); //Si esta bloqueado no avanza
            }
            if(!Encriptador.Encriptar(password).Equals(usu.Clave)) //Hasheo la clave ingresada y la comparo con la de la BD
            {
                if(usu.IntentosFallidos == 2) //Al tercer intento fallido le bloqueo la cuenta
                {
                    usu.Bloqueado = true;
                    Modificar(usu); 
                    ResetearIntentos(usu); 
                }
                else
                {
                    IncrementarIntentos(usu); 
                }
                throw new ExcepcionLogin(ResultadoLogin.ContraseñaInvalida);
            }
            else
            {
                SesionSingleton.Instancia.Login(usu); //Si todo sale bien hace login
                return ResultadoLogin.UsuarioValido;
            }
        }

        public void Logout()
        {
            if(!SesionSingleton.Instancia.EstaLogueado())
            {
                throw new ExcepcionLogin(ResultadoLogin.NoHaySesion);
            }
            SesionSingleton.Instancia.Logout();
        }

        public List<Usuario> Listar()
        {
            List<Usuario> usuarios = new List<Usuario>();
            //usuarios = dalusu.Listar();
            return usuarios;
        }

        public void Eliminar(Usuario usu)
        {
            //dalusu.Borrar(usu);
        }

        public void Insertar(Usuario usu)
        {
            //dalusu.Insertar(usu);
        }

        public void Modificar(Usuario usu)
        {
            //Le manda un usuario modificado a la DAL
            //dalusu.Modificar(usu);
        }

        public void IncrementarIntentos(Usuario usu)
        {
            //Le digo a la DAL que me incremente el intento directamente en la BD
            //dalusu.IncrementarIntento(usu);
        }

        public void ResetearIntentos(Usuario usu)
        {
            //Se puede cambiar la metodología, pero acá lo que debería hacer es decirle a la DAL que le resetee los intentos, no se los cambia a la entidad
            //dalusu.ResetearIntentos(usu);
        }
    }
}

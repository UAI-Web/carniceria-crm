using CarniceriaCRM.BE;
using CarniceriaCRM.BLL.DigitVerifier;
using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarniceriaCRM.BLL
{
    public class UsuarioService : DigitVerifierService
    {
        private readonly UsuarioDAL _usuarioDAL;
        private readonly BitacoraDAL _bitacoraDAL;

        public UsuarioService()
        {
            _usuarioDAL = new UsuarioDAL();
            _bitacoraDAL = new BitacoraDAL();
        }

        public ResultadoLogin Login(string mail, string password)
        {
            //Le paso la contraseña y mail ingresadas sin hashear, porque son las que mete el usuario,
            //pero todas las que vengan de la BD vienen hasheado, por eso lo hasheo adentro del método para compararlo con el de la BD

            // Obtener información de contexto para bitácora
            string direccionIP = ObtenerDireccionIP();
            string userAgent = ObtenerUserAgent();

            if (SesionSingleton.Instancia.EstaLogueado()) //Si ya esta logueado no avanza
            {
                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLoginFallido(mail, "Sesión ya iniciada", direccionIP, userAgent));
                throw new ExcepcionLogin(ResultadoLogin.SesionYaIniciada);
            }

            Usuario usu = _usuarioDAL.ObtenerPorMail(mail); //Agarro de la BD el usuario que corresponde al mail
            
            if (usu == null)
            {
                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLoginFallido(mail, "Mail inválido", direccionIP, userAgent));
                throw new ExcepcionLogin(ResultadoLogin.MailInvalido); //No existe el usuario con ese mail
            }
            
            if (usu.Bloqueado == true)
            {
                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLoginFallido(mail, "Usuario bloqueado", direccionIP, userAgent));
                throw new ExcepcionLogin(ResultadoLogin.UsuarioBloqueado); //Si esta bloqueado no avanza
            }
            
            if (!Encriptador.Encriptar(password).Equals(usu.Clave)) //Hasheo la clave ingresada y la comparo con la de la BD
            {
                if (usu.IntentosFallidos == 2) //Al tercer intento fallido le bloqueo la cuenta
                {
                    usu.Bloqueado = true;
                    _usuarioDAL.Modificar(usu); 
                    _usuarioDAL.ResetearIntentos(usu);

                    RecalcularSingleDV(usu.Id);

                    RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarBloqueoUsuario(usu, direccionIP, userAgent));
                }
                else
                {
                    _usuarioDAL.IncrementarIntento(usu);

                    RecalcularSingleDV(usu.Id);
                }

                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLoginFallido(mail, "Contraseña inválida", direccionIP, userAgent));
                throw new ExcepcionLogin(ResultadoLogin.ContraseñaInvalida);
            }
            else
            {
                // Reset intentos fallidos si el login es exitoso
                if (usu.IntentosFallidos > 0)
                {
                    _usuarioDAL.ResetearIntentos(usu);

                    RecalcularSingleDV(usu.Id);
                }

                SesionSingleton.Instancia.Login(usu); //Si todo sale bien hace login
                
                // Registrar login exitoso en bitácora
                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLogin(usu, direccionIP, userAgent));
                
                return ResultadoLogin.UsuarioValido;
            }
        }

        public void Logout()
        {
            if(!SesionSingleton.Instancia.EstaLogueado())
            {
                throw new ExcepcionLogin(ResultadoLogin.NoHaySesion);
            }

            // Obtener información de contexto para bitácora
            string direccionIP = ObtenerDireccionIP();
            string userAgent = ObtenerUserAgent();
            
            // Obtener usuario antes de hacer logout
            Usuario usuario = SesionSingleton.Instancia.Usuario;
            
            // Registrar logout en bitácora
            RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarLogout(usuario, direccionIP, userAgent));
            
            SesionSingleton.Instancia.Logout();
        }

        public List<Usuario> Listar()
        {
            return _usuarioDAL.Listar();
        }

        public void Eliminar(Usuario usu)
        {
            _usuarioDAL.Borrar(usu);
        }

        public void Insertar(Usuario usu)
        {
            // Encriptar la contraseña antes de guardar
            if (!string.IsNullOrEmpty(usu.Clave))
            {
                usu.Clave = Encriptador.Encriptar(usu.Clave);
            }
            
            _usuarioDAL.Insertar(usu);
        }

        public void Modificar(Usuario usu)
        {
            _usuarioDAL.Modificar(usu);
        }

        public void IncrementarIntentos(Usuario usu)
        {
            _usuarioDAL.IncrementarIntento(usu);
        }

        public void ResetearIntentos(Usuario usu)
        {
            _usuarioDAL.ResetearIntentos(usu);
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public Usuario ObtenerPorId(int id)
        {
            return _usuarioDAL.ObtenerPorId(id);
        }

        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        public Usuario ObtenerPorMail(string mail)
        {
            return _usuarioDAL.ObtenerPorMail(mail);
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        public void CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo)
        {
            Usuario usuario = _usuarioDAL.ObtenerPorId(usuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // Verificar contraseña actual
            if (!Encriptador.Encriptar(passwordActual).Equals(usuario.Clave))
            {
                throw new Exception("Contraseña actual incorrecta");
            }

            // Actualizar con nueva contraseña encriptada
            usuario.Clave = Encriptador.Encriptar(passwordNuevo);
            _usuarioDAL.Modificar(usuario);

            // Registrar el cambio en bitácora
            string direccionIP = ObtenerDireccionIP();
            string userAgent = ObtenerUserAgent();

            RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarEvento(usuarioId, "CAMBIO_PASSWORD", 
                $"Usuario {usuario.Nombre} {usuario.Apellido} cambió su contraseña", 
                direccionIP, userAgent));
        }

        /// <summary>
        /// Desbloquea un usuario
        /// </summary>
        public void DesbloquearUsuario(int usuarioId)
        {
            Usuario usuario = _usuarioDAL.ObtenerPorId(usuarioId);

            if (usuario != null)
            {
                usuario.Bloqueado = false;
                _usuarioDAL.Modificar(usuario);
                _usuarioDAL.ResetearIntentos(usuario);

                // Registrar en bitácora
                string direccionIP = ObtenerDireccionIP();
                string userAgent = ObtenerUserAgent();

                RegistrarBitacoraSafe(() => _bitacoraDAL.RegistrarEvento(usuarioId, "USUARIO_DESBLOQUEADO", 
                    $"Usuario {usuario.Nombre} {usuario.Apellido} fue desbloqueado manualmente", 
                    direccionIP, userAgent));
            }
        }

        /// <summary>
        /// Obtiene la dirección IP del cliente
        /// </summary>
        private string ObtenerDireccionIP()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    return ip;
                }
            }
            catch
            {
                // Si hay error obteniendo IP, devolver vacío
            }
            return null;
        }

        /// <summary>
        /// Obtiene el User Agent del navegador
        /// </summary>
        private string ObtenerUserAgent()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.UserAgent;
                }
            }
            catch
            {
                // Si hay error obteniendo User Agent, devolver vacío
            }
            return null;
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioDAL.ObtenerTodos();
        }

        /// <summary>
        /// Registra en bitácora de forma segura, sin permitir que errores de bitácora afecten el flujo principal
        /// </summary>
        private void RegistrarBitacoraSafe(Action accionBitacora)
        {
            try
            {
                accionBitacora.Invoke();
            }
            catch (Exception ex)
            {
                // Log del error de bitácora sin afectar el flujo principal
                System.Diagnostics.Debug.WriteLine($"Error en bitácora (no crítico): {ex.Message}");
                
                // Opcional: escribir en EventLog si es posible
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("CarniceriaCRM", 
                        $"Error en bitácora: {ex.Message}", 
                        System.Diagnostics.EventLogEntryType.Warning);
                }
                catch
                {
                    // Si no se puede escribir al EventLog, ignorar
                }
            }
        }

        public override IntegrityResult VerifyIntegrity()
        {
            IntegrityResult result = new IntegrityResult();

            IDigitVerifier<Usuario> verifier = new DigitVerifier<Usuario>();

            var todos = _usuarioDAL.ObtenerTodos();

            // Verificar DVH fila por fila
            foreach (var item in todos)
            {
                string expectedDVH = verifier.ComputeDVH(item);

                if (item.DigitoVerificadorH != expectedDVH)
                {
                    result.DHErrors.Add(new IntegrityError($"DVH corrupto en {UsuarioDAL.TableName} [{item.Id}]: se esperaba '{expectedDVH}', se encontró {((!string.IsNullOrEmpty(item.DigitoVerificadorH)) ? "'" + item.DigitoVerificadorH + "'" : "vacío")}."));
                    result.Result = false;
                }
            }

            // Verificar DVV global
            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            string storedDVV = dvvDAL.ObtenerDVV(UsuarioDAL.TableName);
            string expectedDVV = verifier.ComputeDVV(todos);

            if (storedDVV != expectedDVV)
            {
                result.DVErrors.Add(new IntegrityError($"DVV corrupto en {UsuarioDAL.TableName}: se esperaba '{expectedDVV}', se encontró {((!string.IsNullOrEmpty(storedDVV)) ? "'" + storedDVV + "'" : "vacío")}."));
                result.Result = false;
            }

            return result;
        }

        public override void RecalcularDV()
        {
            IDigitVerifier<Usuario> verifier = new DigitVerifier<Usuario>();

            var todos = _usuarioDAL.ObtenerTodos();

            foreach (var item in todos)
            {
                string dvH = verifier.ComputeDVH(item);

                if (item.DigitoVerificadorH != dvH)
                    _usuarioDAL.ActualizarDVH(item.Id, dvH);
            }

            string dvV = verifier.ComputeDVV(todos);

            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            dvvDAL.ActualizarDVV(UsuarioDAL.TableName, dvV);
        }

        public override void RecalcularSingleDV(int id)
        {
            IDigitVerifier<Usuario> verifier = new DigitVerifier<Usuario>();

            Usuario item = _usuarioDAL.ObtenerPorId(id);

            string dvH = verifier.ComputeDVH(item);

            if (item.DigitoVerificadorH != dvH)
                _usuarioDAL.ActualizarDVH(item.Id, dvH);

            var todos = _usuarioDAL.ObtenerTodos();

            string dvV = verifier.ComputeDVV(todos);

            DigitoVerificadorVDAL dvvDAL = new DigitoVerificadorVDAL();
            dvvDAL.ActualizarDVV(UsuarioDAL.TableName, dvV);
        }
    }
}
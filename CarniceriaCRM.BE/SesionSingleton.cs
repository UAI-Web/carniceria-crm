using System;

namespace CarniceriaCRM.BE
{
    public class SesionSingleton : Sesion
    {
        private static SesionSingleton _instancia;
        private static Object _lock = new Object();
        
        public static SesionSingleton Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instancia == null)
                    {
                        _instancia = new SesionSingleton();
                    }
                }
                return _instancia;
            }
        }
    }
} 
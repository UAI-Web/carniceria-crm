using CarniceriaCRM.BE;
using CarniceriaCRM.BLL.DigitVerifier;
using CarniceriaCRM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarniceriaCRM.BLL
{
    public class BitacoraService
    {
        private BitacoraDAL _bitacoraDAL;

        public BitacoraService()
        {
            _bitacoraDAL = new BitacoraDAL();
        }

        public List<Bitacora> ObtenerTodas()
        {
            return _bitacoraDAL.ObtenerTodas();
        }

        public List<Bitacora> ObtenerConFiltros(int? idUsuario, string accion, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var todasLasBitacoras = _bitacoraDAL.ObtenerTodas();
            
            var bitacorasFiltradas = todasLasBitacoras.AsQueryable();

            if (fechaDesde.HasValue)
            {
                bitacorasFiltradas = bitacorasFiltradas.Where(b => b.FechaHora >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                bitacorasFiltradas = bitacorasFiltradas.Where(b => b.FechaHora <= fechaHasta.Value);
            }

            if (!string.IsNullOrEmpty(accion))
            {
                bitacorasFiltradas = bitacorasFiltradas.Where(b => 
                    (b.Accion != null && b.Accion.Contains(accion)));
            }

            if (idUsuario.HasValue)
            {
                bitacorasFiltradas = bitacorasFiltradas.Where(b => b.IdUsuario == idUsuario.Value);
            }

            return bitacorasFiltradas.OrderByDescending(b => b.FechaHora).ToList();
        }

        public Bitacora ObtenerPorId(int id)
        {
            return _bitacoraDAL.ObtenerPorId(id);
        }

        public void RegistrarActividad(string descripcion, string accion, int idUsuario)
        {
            var bitacora = new Bitacora
            {
                Descripcion = descripcion,
                Accion = accion,
                IdUsuario = idUsuario,
                FechaHora = DateTime.Now
            };

            _bitacoraDAL.Registrar(bitacora);
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public class DigitVerifier<T> : IDigitVerifier<T> where T : class
    {
        private readonly PropertyInfo[] _props;

        public DigitVerifier()
        {
            // Excluir la propiedad DigitoVerificadorH del cálculo
            _props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name != "DigitoVerificadorH" && p.PropertyType.Name != typeof(List<>).Name) //ojo que con la segunda parte se excluyen las colecciones
                .OrderBy(p => p.MetadataToken)
                .ToArray();
        }

        public string ComputeDVH(T entity)
        {
            long sum = 0;
            for (int ai = 0; ai < _props.Length; ai++)
            {
                var value = _props[ai].GetValue(entity)?.ToString() ?? string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    // Carácter * posiciónCarácter * posiciónAtributo
                    sum += value[i] * (ai + 1) * (i + 1);
                }
            }
            // Módulo 97 y dos dígitos
            return (sum % 97).ToString("D2");
        }

        public string ComputeDVV(IEnumerable<T> entities)
        {
            long sum = 0;
            int rowIndex = 0;
            foreach (var e in entities)
            {
                rowIndex++;
                var dvh = ComputeDVH(e);
                for (int i = 0; i < dvh.Length; i++)
                {
                    sum += (dvh[i] - '0') * (i + 1) * rowIndex;
                }
            }
            return (sum % 97).ToString("D2");
        }
    }
}
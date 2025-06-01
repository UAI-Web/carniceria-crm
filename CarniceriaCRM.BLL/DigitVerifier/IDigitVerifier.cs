using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public interface IDigitVerifier<T>
    {
        string ComputeDVH(T entity);

        string ComputeDVV(IEnumerable<T> entities);
    }
}
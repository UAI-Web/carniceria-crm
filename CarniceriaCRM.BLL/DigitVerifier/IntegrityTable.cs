using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarniceriaCRM.BLL.DigitVerifier
{
    public class IntegrityTable
    {
        public string Name { get; set; }

        public object ClassType { get; set; }

        public IntegrityTable()
        {
        }

        public IntegrityTable(string name, object classType)
        {
            this.Name = name;
            this.ClassType = classType;
        }

        //public List<IntegrityError> DHErrors { get; set; } = new List<IntegrityError>();
    }
}
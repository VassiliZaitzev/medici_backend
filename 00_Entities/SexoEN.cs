using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class SexoEN
    {
        public int Sexo_Corr { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime Fecha_Crea { get; set; }
        public DateTime Fecha_Modifica { get; set; }
        public byte Vigencia { get; set; }
    }
}

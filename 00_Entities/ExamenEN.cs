using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class ExamenEN
    {
        public int examenCorr { get; set; }
        public int tipoExamenCorr { get; set; }
        public string descripcion { get; set; }
        public string detalle { get; set; }
        public int valor { get; set; }
        public int vigencia { get; set; }
    }

    public class ExamenFonasaEN
    {
        public int examenCorr { get; set; }
        public string codigo { get; set; }
        public string glosa { get; set; }
        public string codigoConcatenado { get; set; }
        public int grupo { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class UsuarioExamenEN
    {
        public int usuarioExamenCorr { get; set; }
        public int usuarioCorr { get; set; }
        public string codigoUsuario { get; set; }
        public List<ExamenFonasaRequestEN> examenes { get; set; }
        public int vigente { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public int edad { get; set; }
        public string rut { get; set; }
        public int sexoCorr { get; set; }
        public string Descripcion { get; set; }
    }
}

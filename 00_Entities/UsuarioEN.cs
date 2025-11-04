using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class UsuarioEN
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public int edad { get; set; }
        public string rut { get; set; }
        public string genero { get; set; }
        public string chatGptKey { get; set; }
        
    }
}

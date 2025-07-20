using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class ChatEN
    {
        public int idChat {  get; set; }
        public string codigoCliente { get; set; }
        public string? mensaje {  get; set; }
        public int? idTipoMensaje { get; set; }
        public DateTime? fecha { get; set; }
    }
}

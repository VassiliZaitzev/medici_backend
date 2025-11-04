using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class ChatRequestEN
    {
        public UsuarioEN usuario { get; set; }        
        public List<ExamenFonasaRequestEN> examenFonasa { get; set; }
        public List<ChatEN> chat { get; set; }
    }
}

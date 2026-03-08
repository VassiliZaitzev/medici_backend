using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities
{
    public class ReenviarPdfRequest
    {
        public long PaymentId { get; set; }
        public string Email { get; set; }
    }
}

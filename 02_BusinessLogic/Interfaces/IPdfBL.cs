using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Interfaces
{
    public interface IPdfBL
    {
        string GenerarPdfClienteBase64();
        Task EnviarDocumentoPDF(); 
    }
}

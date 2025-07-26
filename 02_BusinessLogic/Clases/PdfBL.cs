using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace _02_BusinessLogic.Clases
{
    public class PdfBL : IPdfBL
    {
        public string GenerarPdfClienteBase64()
        {
            PdfDal oPdfDal = new PdfDal();
            return oPdfDal.GenerarPdfClienteBase64();
        }
    }
}

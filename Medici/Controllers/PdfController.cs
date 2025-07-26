using _00_Entities;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : Controller
    {

        private readonly IPdfBL _IPdfBL;
        public PdfController(IPdfBL IPdfBL)
        {
            _IPdfBL = IPdfBL;
        }

        [HttpGet("ObtenerDocumento")]
        public string GenerarPdfClienteBase64()
        {
            return _IPdfBL.GenerarPdfClienteBase64();
        }
    }
}

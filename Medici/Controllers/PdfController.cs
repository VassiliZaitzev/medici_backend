using _00_Entities;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static QuestPDF.Helpers.Colors;

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

        [HttpGet("ObtenerDatosExamenCodigo")]
        public Task<UsuarioExamenEN> ObtenerDatosExamenCodigo(int codigo)
        {
            return _IPdfBL.ObtenerDatosExamenCodigo(codigo);
        }

        [HttpPost("GenerarPdfClienteBase64")]
        public string GenerarPdfClienteBase64([FromBody] UsuarioExamenEN examen)
        {
            return _IPdfBL.GenerarPdfClienteBase64(examen);
        }


    }
}

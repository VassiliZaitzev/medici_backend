using _00_Entities;
using _02_BusinessLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamenController : Controller
    {
        private readonly IExamenBL _iexamenBL;
        public ExamenController(IExamenBL iexamenBL)
        {
            _iexamenBL = iexamenBL;
        }


        [HttpGet("obtenerExamen")]
        public Task<List<ExamenEN>> obtenerExamen()
        {
            return _iexamenBL.obtenerExamen();
        }
    }
}

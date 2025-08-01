using _00_Entities;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medici.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SexoController : ControllerBase
    {
        private readonly ISexoBL _sexoBL;

        public SexoController(ISexoBL sexoBL)
        {
            _sexoBL = sexoBL;
        }

        /// <summary>
        /// GET /api/sexo
        /// Devuelve la lista de tipos de sexo vigentes.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<SexoEN>>> GetSexos()
        {
            var lista = await _sexoBL.ObtenerSexo();
            return Ok(lista);
        }
    }
}

using _00_Entities;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactBL _contactBL;

        public ContactController(IContactBL contactBL)
        {
            _contactBL = contactBL;
        }

        [HttpPost("AgregarContacto")]
        public async Task<IActionResult> AgregarContacto([FromBody] ContactEN contacto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nuevoId = await _contactBL.AgregarContacto(contacto);
            return CreatedAtAction(nameof(AgregarContacto), new { id = nuevoId }, new { id = nuevoId });
        }
    }
}

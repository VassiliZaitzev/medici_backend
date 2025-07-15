using _00_Entities;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IChatBL _chatBL;
        public ChatController(IChatBL chatBL)
        {
            _chatBL = chatBL;
        }

        [HttpGet("buscarCita")]
        public Task<List<ChatEN>> ListarChat()
        {
            return _chatBL.ListarChat();
        }

        //[HttpPost("guardarConsultaAbreviada")]
        //public UtilsEN guardarConsultaAbreviada([FromBody] CitaEN request)
        //{
        //    return _citaBL.guardarConsultaAbreviada(request);
        //}
    }
}

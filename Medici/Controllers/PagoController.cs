using _00_Entities;
using _02_BusinessLogic.Interfaces;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Error;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IChatBL _chatBL;

        // Simulación de BD en memoria (estado pagos)
        private static Dictionary<long, string> _pagos = new();

        private static Dictionary<string, ChatRequestEN> _ordenes = new();

        public PagoController(IConfiguration config, IChatBL chatBL)
        {
            _config = config;
            _chatBL = chatBL;
        }

        // 1️⃣ Crear preferencia / URL de pago
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPago([FromBody] ChatRequestEN request)
        {
            try
            {
                if (request?.usuario == null || string.IsNullOrWhiteSpace(request.usuario.chatGptKey))
                    return BadRequest(new { error = "Falta usuario.chatGptKey" });

                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];

                string urlPublica = "https://fungous-cosmonautically-rheba.ngrok-free.dev";

                var backUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{urlPublica}/api/Pago/success_bridge",
                    Failure = $"{urlPublica}/api/Pago/failure_bridge",
                    Pending = $"{urlPublica}/api/Pago/success_bridge"
                };

                _ordenes[request.usuario.chatGptKey] = request;

                var preferenceRequest = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new() { Title = "Consulta Médica Medici", Quantity = 1, CurrencyId = "CLP", UnitPrice = 1000m }
                    },
                    BackUrls = backUrls,
                    AutoReturn = "approved",
                    NotificationUrl = $"{urlPublica}/api/Pago/webhook",
                    BinaryMode = true,

                    ExternalReference = request.usuario.chatGptKey
                };

                var client = new PreferenceClient();
                var result = await client.CreateAsync(preferenceRequest);

                return Ok(new { url = result.InitPoint, id = result.Id });
            }
            catch (MercadoPagoApiException apiEx)
            {
                return StatusCode(400, new { error = apiEx.ApiError.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement body)
        {
            try
            {
                if (!body.TryGetProperty("data", out var data) || !data.TryGetProperty("id", out var idProp))
                    return Ok();

                if (!long.TryParse(idProp.ToString(), out long paymentId))
                    return Ok();

                var paymentClient = new PaymentClient();
                var payment = await paymentClient.GetAsync(paymentId);

                if (payment == null)
                    return Ok();

                _pagos[paymentId] = payment.Status;

                if (payment.Status == "approved")
                {
                    var chatKey = payment.ExternalReference;

                    if (!string.IsNullOrWhiteSpace(chatKey) && _ordenes.TryGetValue(chatKey, out var orden))
                    {
                        await _chatBL.GuardarUsuarioExamen(orden);

                        _ordenes.Remove(chatKey);
                    }
                }

                return Ok();
            }
            catch
            {
                return Ok();
            }
        }

        [HttpGet("estado/{paymentId}")]
        public IActionResult EstadoPago(long paymentId)
        {
            if (_pagos.TryGetValue(paymentId, out var estado))
                return Ok(new { PaymentId = paymentId, Estado = estado });

            return NotFound(new { mensaje = "Pago aún procesándose" });
        }

        [HttpGet("success_bridge")]
        public IActionResult SuccessBridge()
        {
            var queryParams = Request.QueryString.Value;
            string angularUrl = "http://localhost:4200/pagos/success";
            return Redirect($"{angularUrl}{queryParams}");
        }

        [HttpGet("failure_bridge")]
        public IActionResult FailureBridge()
        {
            return Redirect("http://localhost:4200/pagos/failure" + Request.QueryString.Value);
        }
    }
}

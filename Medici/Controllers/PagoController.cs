using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Medici.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : Controller
    {

        private readonly IConfiguration _config;

        // Simulación de BD en memoria
        private static Dictionary<long, string> _pagos = new();

        public PagoController(IConfiguration config)
        {
            _config = config;
        }

        // 1️⃣ Crear preferencia / URL de pago
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPago()
        {
            try
            {
                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];

                //ngrok apuntando al puerto 7172
                //string urlPublica = "https://localhost:7172";
                //string urlPublica = "";
                string urlPublica = "https://fungous-cosmonautically-rheba.ngrok-free.dev";

                // En lugar de mandar a /pagos/success, mandamos al "Puente" en la API
                var backUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{urlPublica}/api/Pago/success_bridge",
                    Failure = $"{urlPublica}/api/Pago/failure_bridge",
                    Pending = $"{urlPublica}/api/Pago/success_bridge"
                };
                // -------------------------

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
                    ExternalReference = "ORDEN-" + DateTime.Now.Ticks
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
                // Extraemos el ID del pago enviado por Mercado Pago
                if (!body.TryGetProperty("data", out var data) || !data.TryGetProperty("id", out var idProp))
                    return Ok();

                if (!long.TryParse(idProp.ToString(), out long paymentId))
                    return Ok();

                var paymentClient = new PaymentClient();
                var payment = await paymentClient.GetAsync(paymentId);

                if (payment != null)
                {
                    // Registramos el estado en nuestro diccionario
                    _pagos[paymentId] = payment.Status;
                }

                return Ok(); // Siempre responder 200 a Mercado Pago
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        [HttpGet("estado/{paymentId}")]
        public IActionResult EstadoPago(long paymentId)
        {
            if (_pagos.TryGetValue(paymentId, out var estado))
            {
                return Ok(new { PaymentId = paymentId, Estado = estado });
            }
            return NotFound(new { mensaje = "Pago aún procesándose" });
        }

        [HttpGet("success_bridge")]
        public IActionResult SuccessBridge()
        {
            var queryParams = Request.QueryString.Value;

            //modificar la url cuando se pase a produccion
            // 2. Construimos la URL local de tu Angular
            string angularUrl = "http://localhost:4200/pagos/success";

            return Redirect($"{angularUrl}{queryParams}");
        }

        [HttpGet("failure_bridge")]
        public IActionResult FailureBridge()
        {
            //modificar la url cuando se pase a produccion
            return Redirect("http://localhost:4200/pagos/failure" + Request.QueryString.Value);
        }
    }

    /*private readonly IConfiguration _config;


    public PagoController(IConfiguration config)
    {
        _config = config;
    }


    [HttpPost("crear")]
    public async Task<IActionResult> CrearPago()
    {
        MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];
        // Tarjetas de prueba: Visa: 4509 9535 6623 3704, CVV: 123, Fecha: 11/25

        var preference = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
            new() { Title = "Consulta médica demo", Quantity = 1, CurrencyId = "CLP", UnitPrice = 1000 }
            },
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "http://localhost:4200/success",
                Failure = "http://localhost:4200/failure"
            },
            AutoReturn = "approved"
        };


        var client = new PreferenceClient();
        var result = await client.CreateAsync(preference);


        return Ok(new { url = result.InitPoint });
    }


    */
    /*[HttpPost("webhook")]
    public IActionResult Webhook([FromBody] object data)
    {
        // 1. Validar el webhook
        // data contiene info del pago
        Console.WriteLine("Webhook recibido: " + data.ToString());

        // 2. Aquí puedes parsear el JSON
        // Ejemplo: obtener payment_id
        // var json = JsonDocument.Parse(data.ToString());
        // var paymentId = json.RootElement.GetProperty("data").GetProperty("id").GetInt32();

        // 3. Consultar Mercado Pago API para validar el pago
        // (opcional pero recomendado)

        // 4. Actualizar base de datos con estado de pago

        return Ok(); // siempre responder 200 OK a Mercado Pago
    }*/
}


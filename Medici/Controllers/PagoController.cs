using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
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
        private static Dictionary<int, string> _pagos = new();

        public PagoController(IConfiguration config)
        {
            _config = config;
        }

        // 1️⃣ Crear preferencia / URL de pago
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPago()
        {
            MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];

            var preference = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
            {
                new()
                {
                    Title = "Consulta médica demo",
                    Quantity = 1,
                    CurrencyId = "CLP",
                    UnitPrice = 1000
                }
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

            return Ok(new { url = result.InitPoint, preferenceId = result.Id });
        }

        // 2️⃣ Webhook para recibir notificaciones de pago
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement data)
        {
            try
            {
                // 2a️⃣ Obtener payment_id del webhook
                if (!data.TryGetProperty("data", out var dataProperty) ||
                    !dataProperty.TryGetProperty("id", out var idProperty))
                {
                    return BadRequest("payment_id no encontrado en webhook");
                }

                int paymentId = idProperty.GetInt32();
                Console.WriteLine($"Webhook recibido, PaymentId: {paymentId}");

                // 2b️⃣ Validar pago real con Mercado Pago (PASO 3)
                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];
                var paymentClient = new PaymentClient();
                Payment payment = await paymentClient.GetAsync(paymentId);

                // 2c️⃣ Actualizar "BD" simulada
                if (payment.Status == "approved")
                {
                    _pagos[paymentId] = "APROBADO";
                    Console.WriteLine($"Pago aprobado! PaymentId: {paymentId}");
                }
                else
                {
                    _pagos[paymentId] = payment.Status;
                    Console.WriteLine($"Pago no aprobado. Estado: {payment.Status}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Webhook: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // 3️⃣ Endpoint opcional para consultar estado de pago
        [HttpGet("estado/{paymentId}")]
        public IActionResult EstadoPago(int paymentId)
        {
            if (_pagos.ContainsKey(paymentId))
            {
                return Ok(new { PaymentId = paymentId, Estado = _pagos[paymentId] });
            }
            return NotFound("PaymentId no encontrado");
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
}

using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class ChatBL : IChatBL
    {

        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<List<ChatEN>> ListarChat(string codigo)
        {
            ChatDal oChatDal = new ChatDal();
            return await oChatDal.ListarChat(codigo);
        }

        public async Task<int> AgregarChat(ChatEN chat)
        {
            ChatDal oChatDal = new ChatDal();
            return await oChatDal.AgregarChat(chat);
        }


        /*public async Task<int> GuardarUsuarioExamen(ChatRequestEN request)
        {
            ChatDal oChatDal = new ChatDal();
            return await oChatDal.GuardarUsuarioExamen(request);
        }*/


        public async Task<string> GuardarUsuarioExamen(ChatRequestEN request)
        {
            ChatDal oChatDal = new ChatDal();
            UsuarioDAL oUsuarioDal = new UsuarioDAL();
            PdfDal oPdfDal = new PdfDal();
            
            string resp = "";
            try
            {
                foreach (var item in request.chat)
                {
                    int agregado = await oChatDal.AgregarChat(item);

                    if (agregado <= 0)
                    {
                        return "se cae al AgregarChat";
                    }
                }

                int usuarioCorr = await oUsuarioDal.AgregarUsuario(request.usuario);
                if (usuarioCorr <= 0)
                {
                    resp = "Se cae al agregar usuario";
                    return resp;
                }

                int GuardarUsuarioExamen = await oChatDal.GuardarUsuarioExamen(JsonSerializer.Serialize(request.examenFonasa), usuarioCorr, request.usuario.chatGptKey);

                if (GuardarUsuarioExamen <= 0)
                {
                    resp = "Se cae al guardar el examen";
                    return resp;
                }

                UsuarioExamenEN datosUsuario = await oPdfDal.ObtenerDatosExamenCodigo(GuardarUsuarioExamen);

                if (datosUsuario == null)
                {
                    resp = "Se cae al ObtenerDatosExamenCodigo";
                    return resp;
                }


                resp = oPdfDal.GenerarPdfClienteBase64(datosUsuario);

                if (!string.IsNullOrWhiteSpace(resp))
                {
                    // ✅ AQUÍ EL CAMBIO: mandar al correo del paciente
                    var correoDestino = datosUsuario.email; // o request.usuario.email

                    if (!string.IsNullOrWhiteSpace(correoDestino))
                    {
                        PdfBL oPdfBl = new PdfBL();
                        await oPdfBl.EnviarDocumentoPDF(correoDestino, resp);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No se envió correo: email destino vacío.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);
            }
            return resp;
        }

        public async Task<string> GptEnviarMensaje(string mensaje)
        {
            string resp = "";
            try
            {

                HttpClient _httpClient = new HttpClient();
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                string fullUrl = config["ChatGepesito:urlGpt"];
                var json = new
                {
                    //model = "gpt-4o-mini",
                    //messages = new[]
                    //{
                    //    new { role = "user", content = mensaje }
                    //},
                    //max_tokens = 300



                    model = "gpt-5-nano",
                    messages = new[]
                    {
                        new { role = "user", content = mensaje }
                    }
                };

                var contentUrl = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config["ChatGepesito:apiKey"]);
                var responseToken = await _httpClient.PostAsync(fullUrl, contentUrl);
                resp = await responseToken.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);
            }
            return resp;
        }

        public async Task<string> ClaudeEnviarMensajeV2(string mensaje)
        {
            string resp = "";
            try
            {
                HttpClient _httpClient = new HttpClient();
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                // Leemos la configuración específica de Claude
                string fullUrl = config["ClaudeAPI:urlGpt"];

                var json = new
                {
                    // Claude 3 Haiku es súper rápido y económico. También puedes usar "claude-3-5-sonnet-20241022"
                    model = "claude-3-haiku-20240307",
                    max_tokens = 1024, // Claude exige que le digas el límite de tokens
                    messages = new[]
                    {
                        new { role = "user", content = mensaje }
                    }
                };

                var contentUrl = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");

                // Los Headers de Claude son distintos a los de OpenAI
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", config["ClaudeAPI:apiKey"]);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", config["ClaudeAPI:anthropicVersion"]);

                var responseToken = await _httpClient.PostAsync(fullUrl, contentUrl);
                resp = await responseToken.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Claude V2: " + ex);
            }
            return resp;
        }

        /*
        public async Task<string> GptEnviarMensaje(string mensaje)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string fullUrl = config["ChatGepesito:urlGpt"];

            var json = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "user", content = mensaje }
        },
                max_tokens = 150
            };

            var contentUrl = new StringContent(
                JsonSerializer.Serialize(json),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", config["ChatGepesito:apiKey"]);

            for (int intento = 1; intento <= 3; intento++)
            {
                var response = await _httpClient.PostAsync(fullUrl, contentUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(2000 * intento); // backoff exponencial
                    continue;
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }

            throw new Exception("Rate limit excedido tras varios intentos");
        }

        */

    }
}

using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class ChatBL : IChatBL
    {
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

                if (resp != "")
                {
                    PdfBL oPdfBl = new PdfBL();
                    oPdfBl.EnviarDocumentoPDF(resp);
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
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "user", content = mensaje }
                    },
                    max_tokens = 300
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
    }
}

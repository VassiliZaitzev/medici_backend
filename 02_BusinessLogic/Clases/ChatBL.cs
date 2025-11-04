using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
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


        public async Task<int> GuardarUsuarioExamen(ChatRequestEN request)
        {
            ChatDal oChatDal = new ChatDal();
            UsuarioDAL oUsuarioDal = new UsuarioDAL();

            try
            {
                foreach (var item in request.chat)
                {
                    int agregado = await oChatDal.AgregarChat(item);

                    if (agregado <= 0)
                    {
                        return 0;
                    }
                }

                int usuarioCorr = await oUsuarioDal.AgregarUsuario(request.usuario);
                if (usuarioCorr <= 0)
                {
                    return 0;
                }

                int GuardarUsuarioExamen = await oChatDal.GuardarUsuarioExamen(JsonSerializer.Serialize(request.examenFonasa), usuarioCorr, request.usuario.chatGptKey);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);
            }
            return 1;
        }
    }
}

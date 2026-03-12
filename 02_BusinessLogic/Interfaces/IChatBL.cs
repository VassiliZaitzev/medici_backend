using _00_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Interfaces
{
    public interface IChatBL
    {
        public Task<List<ChatEN>> ListarChat(string codigo);
        public Task<int> AgregarChat(ChatEN chat);
        public Task<string> GuardarUsuarioExamen(ChatRequestEN request);

        public Task<string> GptEnviarMensaje(string mensaje);


        // 🔥 NUEVO: Método V2 (Claude)
        public Task<string> ClaudeEnviarMensajeV2(string mensaje);
    }
}

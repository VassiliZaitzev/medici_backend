using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class ChatBL : IChatBL
    {
        public async Task<List<ChatEN>> ListarChat()
        {
            ChatDal oChatDal = new ChatDal();
            return await oChatDal.listarChat();
        }
    }
}

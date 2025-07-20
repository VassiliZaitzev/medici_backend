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
    }
}

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
    public  class SexoBL : ISexoBL
    {
        public async Task<List<SexoEN>> ObtenerSexo()
        {
            SexoDal sexo = new SexoDal();
            return await sexo.ObtenerSexo();
        }
    }
}

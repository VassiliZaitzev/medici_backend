using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using static QuestPDF.Helpers.Colors;

namespace _02_BusinessLogic.Clases
{
    public class ExamenBL : IExamenBL
    {        
        public async Task<List<ExamenEN>> obtenerExamen()
        {
            ExamenDal oExamenDal = new ExamenDal();
            return await oExamenDal.obtenerExamenes();
        }

        public async Task<List<ExamenFonasaEN>> obtenerExamenesFonasa()
        {
            ExamenDal oExamenDal = new ExamenDal();
            return await oExamenDal.obtenerExamenesFonasa();
        }
    }
}

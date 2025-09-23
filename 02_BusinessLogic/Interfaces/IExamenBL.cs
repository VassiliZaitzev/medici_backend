using _00_Entities;

namespace _02_BusinessLogic.Interfaces
{
    public interface IExamenBL
    {
        public Task<List<ExamenEN>> obtenerExamen();
    }
}

using _00_Entities;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Interfaces
{
    public interface IContactBL
    {
        Task<int> AgregarContacto(ContactEN contacto);
    }
}

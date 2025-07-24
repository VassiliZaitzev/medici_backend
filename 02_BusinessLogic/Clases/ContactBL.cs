using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class ContactBL : IContactBL
    {
        private readonly ContactDal _dal = new ContactDal();

        public async Task<int> AgregarContacto(ContactEN contacto)
        {
            return await _dal.AgregarContactoAsync(contacto);
        }
    }
}

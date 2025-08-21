using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class ContactBL : IContactBL
    {
        private readonly ContactDal _dal;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactBL> _logger;

        public ContactBL(ContactDal dal, IEmailService emailService, ILogger<ContactBL> logger)
        {
            _dal = dal;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> AgregarContacto(ContactEN contacto)
        {
            // 1) Inserta en BD
            var id = await _dal.AgregarContactoAsync(contacto);

            // 2) Si insertó, envía correo (simple por ahora)
            if (id > 0)
            {
                try
                {
                    var subject = "Hemos recibido tu consulta";
                    var body = $@"
                        Hola {contacto.Nombre},

                        Hemos recibido tu consulta y la estamos revisando.
                        Tu código de seguimiento es: {id}.

                        Te responderemos a esta misma dirección.

                        Saludos,
                        Equipo Medici
                        ";
                    // Usa EmailService (envío HTML o texto; aquí enviamos como HTML simple)
                    await _emailService.SendEmailAsync(contacto.Email, subject, body.Replace("\n", "<br/>"));
                }
                catch (Exception ex)
                {
                    // No romper la creación si falla el mail, solo log
                    _logger.LogError(ex, "Error enviando email de confirmación para contacto {Id}", id);
                }
            }

            return id;
        }
    }
}

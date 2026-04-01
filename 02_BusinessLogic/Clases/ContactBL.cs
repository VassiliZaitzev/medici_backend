using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
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
            var id = await _dal.AgregarContactoAsync(contacto);
            if (id > 0)
            {
                try
                {
                    int resCopy = await SendCopy(contacto);
                    var subject = "Hemos recibido tu consulta";

                    var body = $@"
                                <html>
                                <head>
                                    <style>
                                        body {{
                                            font-family: 'Segoe UI', Arial, sans-serif;
                                            background-color: #f7f9f8;
                                            margin: 0;
                                            padding: 0;
                                        }}
                                        .container {{
                                            max-width: 600px;
                                            margin: 30px auto;
                                            background-color: #ffffff;
                                            border-radius: 10px;
                                            box-shadow: 0 3px 8px rgba(0,0,0,0.1);
                                            overflow: hidden;
                                        }}
                                        .header {{
                                            background-color: #00897b;
                                            color: white;
                                            text-align: center;
                                            padding: 20px 10px;
                                        }}
                                        .header h1 {{
                                            margin: 0;
                                            font-size: 22px;
                                        }}
                                        .content {{
                                            padding: 25px 30px;
                                            color: #333333;
                                            line-height: 1.6;
                                        }}
                                        .content p {{
                                            margin-bottom: 15px;
                                        }}
                                        .highlight {{
                                            background-color: #e0f2f1;
                                            padding: 12px 18px;
                                            border-left: 4px solid #00897b;
                                            border-radius: 6px;
                                            font-size: 14px;
                                        }}
                                        .footer {{
                                            text-align: center;
                                            font-size: 13px;
                                            color: #888888;
                                            padding: 15px;
                                            background-color: #f0f0f0;
                                        }}
                                        .footer a {{
                                            color: #00897b;
                                            text-decoration: none;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>
                                            <h1>Hemos recibido tu consulta</h1>
                                        </div>

                                        <div class='content'>
                                            <p>Hola {contacto.Nombre},</p>

                                            <p>Hemos recibido tu consulta y la estamos revisando.</p>

                                            <div class='highlight'>
                                                Tu código de seguimiento es: <strong>{id}</strong>.
                                            </div>

                                            <p>Te responderemos a esta misma dirección. ({contacto.Email})</p>

                                            <p>Saludos,<br/>Equipo Medicy</p>
                                        </div>

                                        <div class='footer'>
                                            © 2025 Medicy · Todos los derechos reservados<br/>
                                            <a href='mailto:contacto@medicy.cl'>contacto@medicy.cl</a> | <a href='https://www.medicy.cl'>www.medicy.cl</a>
                                        </div>
                                    </div>
                                </body>
                                </html>";

                    await _emailService.SendEmailAsync(contacto.Email, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando email de confirmación para contacto {Id}", id);
                }
            }

            return id;
        }




        public async Task<int> SendCopy(ContactEN contacto)
        {
            int result = 0;
            try
            {
                var subject = "Nuevo requerimiento de contacto.";

                var body = $@"
                                <html>
                                <head>
                                    <style>
                                        body {{
                                            font-family: 'Segoe UI', Arial, sans-serif;
                                            background-color: #f7f9f8;
                                            margin: 0;
                                            padding: 0;
                                        }}
                                        .container {{
                                            max-width: 600px;
                                            margin: 30px auto;
                                            background-color: #ffffff;
                                            border-radius: 10px;
                                            box-shadow: 0 3px 8px rgba(0,0,0,0.1);
                                            overflow: hidden;
                                        }}
                                        .header {{
                                            background-color: #00897b;
                                            color: white;
                                            text-align: center;
                                            padding: 20px 10px;
                                        }}
                                        .header h1 {{
                                            margin: 0;
                                            font-size: 22px;
                                        }}
                                        .content {{
                                            padding: 25px 30px;
                                            color: #333333;
                                            line-height: 1.6;
                                        }}
                                        .content p {{
                                            margin-bottom: 15px;
                                        }}
                                        .highlight {{
                                            background-color: #e0f2f1;
                                            padding: 12px 18px;
                                            border-left: 4px solid #00897b;
                                            border-radius: 6px;
                                            font-size: 14px;
                                        }}
                                        .footer {{
                                            text-align: center;
                                            font-size: 13px;
                                            color: #888888;
                                            padding: 15px;
                                            background-color: #f0f0f0;
                                        }}
                                        .footer a {{
                                            color: #00897b;
                                            text-decoration: none;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>
                                            <h1>Nuevo requerimiento de contacto.</h1>
                                        </div>

                                        <div class='content'>

                                            <p>Se ha recibido un nuevo mensaje desde el formulario de contacto:</p>
                                            <p><strong>Nombre:</strong> {contacto.Nombre}</p>
                                            <p><strong>Email:</strong> {contacto.Email}</p>
                                            <div class='highlight'>
                                                <strong>Mensaje recibido:</strong><br/>
                                                {contacto.Mensaje}
                                            </div>


                                            <p>Saludos,<br/>Equipo Medicy</p>
                                        </div>

                                        <div class='footer'>
                                            © 2025 Medicy · Todos los derechos reservados<br/>
                                            <a href='mailto:contacto@medicy.cl'>contacto@medicy.cl</a> | <a href='https://www.medicy.cl'>www.medicy.cl</a>
                                        </div>
                                    </div>
                                </body>
                                </html>";

                await _emailService.SendEmailAsync("contacto@medicy.cl", subject, body);
                result = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de confirmación para usuario");
            }

            return result;
        }
        

    }
}

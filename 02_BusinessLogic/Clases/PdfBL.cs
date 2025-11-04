using _00_Entities;
using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using System;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace _02_BusinessLogic.Clases
{
    public class PdfBL : IPdfBL
    {
        
        public async Task EnviarDocumentoPDF(string pdfBase64)
        {
            EmailService emailService = new EmailService();
            try
            {
                var subject = "Orden de Examen Medicy";

 
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
                            <h1>Orden de Examen Medicy</h1>
                        </div>
                        <div class='content'>
                            <p>Estimado(a) paciente,</p>
                            <p>Hemos recibido su solicitud de examen y adjuntamos en este correo el documento correspondiente para su revisión.</p>
                            <div class='highlight'>
                                <strong>Por favor, revise el archivo adjunto.</strong><br/>
                                Este documento contiene toda la información necesaria para la realización de su examen.
                            </div>
                            <p>Si tiene alguna duda o existe algun problema con documento adjunto, puede comunicarse con nuestro equipo de atención al paciente.</p>
                            <p>Muchas gracias por confiar en <strong>Medicy</strong>.</p>
                        </div>
                        <div class='footer'>
                            © 2025 Medicy · Todos los derechos reservados<br/>
                            <a href='mailto:contacto@medici.cl'>contacto@medici.cl</a> | <a href='https://www.medici.cl'>www.medici.cl</a>
                        </div>
                    </div>
                </body>
                </html>";

                await emailService.SendEmailWithAttachmentAsync(
                    "byj.johnson@gmail.com",
                    subject,
                    body,
                    "OrdenExamen.pdf",
                    pdfBase64
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<UsuarioExamenEN> ObtenerDatosExamenCodigo(int codigo)
        {
            PdfDal oPdfDal = new PdfDal();
            return await oPdfDal.ObtenerDatosExamenCodigo(codigo);
        }

        public string GenerarPdfClienteBase64(UsuarioExamenEN examen)
        {
            PdfDal oPdfDal = new PdfDal();
            return oPdfDal.GenerarPdfClienteBase64(examen);
        }

    }
}

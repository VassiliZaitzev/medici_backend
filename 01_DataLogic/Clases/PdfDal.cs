using _00_Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class PdfDal
    {
        //public DocumentMetadata GetMetadata() => DocumentMetadata.Default;


        public async Task<UsuarioExamenEN> ObtenerDatosExamenCodigo(int codigo)
        {
            UsuarioExamenEN examenes = null;
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {
                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("LISTAR_USUARIO_EXAMEN", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pUSUARIO_EXAMEN_CORR", codigo);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    examenes = new UsuarioExamenEN();
                    examenes.usuarioExamenCorr = reader.IsDBNull("USUARIO_EXAMEN_CORR") ? 0 : reader.GetInt32("USUARIO_EXAMEN_CORR");
                    examenes.usuarioCorr = reader.IsDBNull("USUARIO_CORR") ? 0 : reader.GetInt32("USUARIO_CORR");
                    if (reader.IsDBNull("EXAMENES"))
                    {
                        examenes.examenes = null;
                    }
                    else
                    {
                        // Leer como byte[]
                        byte[] bytes = (byte[])reader["EXAMENES"];

                        // Convertir a string usando la codificación correcta (usualmente UTF-8)
                        string json = System.Text.Encoding.UTF8.GetString(bytes);

                        // Deserializar a lista de objetos
                        examenes.examenes = JsonSerializer.Deserialize<List<ExamenFonasaRequestEN>>(json);
                    }

                    examenes.codigoUsuario = reader.IsDBNull("CODIGO_USUARIO") ? null : reader.GetString("CODIGO_USUARIO");
                    examenes.vigente = reader.IsDBNull("VIGENTE") ? 0 : reader.GetInt32("VIGENTE");
                    examenes.nombre = reader.IsDBNull("NOMBRE") ? null : reader.GetString("NOMBRE");
                    examenes.email = reader.IsDBNull("EMAIL") ? null : reader.GetString("EMAIL");
                    examenes.edad = reader.IsDBNull("EDAD") ? 0 : reader.GetInt32("EDAD");
                    examenes.rut = reader.IsDBNull("RUT") ? null : reader.GetString("RUT");
                    examenes.sexoCorr = reader.IsDBNull("SEXO_CORR") ? 0 : reader.GetInt32("SEXO_CORR");
                    examenes.Descripcion = reader.IsDBNull("DESCRIPCION") ? null : reader.GetString("DESCRIPCION");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

            }

            return examenes;
        }



        public string Titulo { get; set; } = "Órden de exámenes";
        public string Fecha { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public string GenerarPdfClienteBase64(UsuarioExamenEN examen)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Element(c => ComposeHeader(c, examen));
                    page.Content().Element(c => ComposeContent(c, examen));
                    page.Footer().Element(ComposeFooter);
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return Convert.ToBase64String(stream.ToArray());
        }

        void ComposeHeader(IContainer container, UsuarioExamenEN examen)
        {
            string basePath = AppContext.BaseDirectory;
            string proyectoRaiz = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
            string logoPath = Path.Combine(proyectoRaiz, "Assets", "medici1.png");

            if (!File.Exists(logoPath))
                throw new FileNotFoundException($"No se encontró la imagen en: {logoPath}");

            byte[] LogoBytes = File.ReadAllBytes(logoPath);

            container.Row(row =>
            {
                row.RelativeItem(3).Height(50).AlignMiddle().AlignLeft().Element(col =>
                {
                    col.Image(LogoBytes);
                });

                row.RelativeItem(6).AlignCenter().AlignMiddle().Text(Titulo)
                    .FontSize(16).SemiBold().FontColor(Colors.Black);

                row.RelativeItem(3).Column(col =>
                {
                    col.Item().Text($"Folio: {examen.codigoUsuario}").FontSize(10).FontColor(Colors.Grey.Darken2);
                    col.Item().Text($"Fecha: {Fecha}").FontSize(10).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        void ComposeContent(IContainer container, UsuarioExamenEN examen)
        {

            TextInfo textInfo = new CultureInfo("es-ES", false).TextInfo;

            container.PaddingVertical(20).Column(col =>
            {
                col.Spacing(10);

                // Información general del usuario
                col.Item().Text($"Nombre: {examen.nombre}").FontSize(10);
                col.Item().Text($"RUT: {examen.rut}").FontSize(10);
                col.Item().Text($"Email: {examen.email}").FontSize(10);
                col.Item().Text($"Edad: {examen.edad}").FontSize(10);
                col.Item().Text($"Código de Usuario: {examen.codigoUsuario}").FontSize(10);
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                foreach (var e in examen.examenes)
                {
                    col.Item().Element(header =>
                        header.PaddingTop(10)
                              .Text($"EXAMEN DE TIPO {textInfo.ToTitleCase(e.tipo)}")
                              .FontSize(12)
                              .Bold()
                    );

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Código
                            columns.RelativeColumn(6); // Nombre + Utilidad
                        });

                        table.Header(header =>
                        {
                            header.Cell().PaddingBottom(10).Text("Código").FontSize(10).Bold();
                            header.Cell().PaddingBottom(10).Text("Nombre").FontSize(10).Bold();
                        });

                        foreach (var detalle in e.detalles)
                        {
                            // Celda Código
                            table.Cell().Text(detalle.codigo).FontSize(9).Bold();

                            // Celda Nombre + Utilidad
                            table.Cell().Element(cell =>
                            {
                                cell.Column(colInner =>
                                {
                                    colInner.Spacing(2);
                                    colInner.Item().Element(e => e.Text(detalle.nombre).FontSize(9));
                                    colInner.Item().Element(e =>
                                        e.PaddingBottom(10)
                                         .Text(detalle.utilidad)
                                         .FontSize(9)
                                         .FontColor(Colors.Grey.Darken1)
                                         .Italic()
                                    );

                                });
                            });
                        }
                    });
                }



            });
        }

        void ComposeFooter(IContainer container)
        {
            string basePath = AppContext.BaseDirectory;
            string proyectoRaiz = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
            string signPath = Path.Combine(proyectoRaiz, "Assets", "signature2.png");

            if (!File.Exists(signPath))
                throw new FileNotFoundException($"No se encontró la imagen en: {signPath}");

            byte[] LogoBytes = File.ReadAllBytes(signPath);

            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem(0.6f).AlignMiddle().Column(col =>
                    {
                        col.Item().PaddingLeft(15).PaddingBottom(4)
                           .Text("Nombre Profesional: James Johnson")
                           .FontSize(9).AlignLeft();

                        col.Item().PaddingLeft(15).PaddingBottom(4)
                           .Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                           .FontSize(9).AlignLeft();

                        col.Item().PaddingLeft(15)
                           .Text($"Hora: {DateTime.Now:HH:mm}")
                           .FontSize(9).AlignLeft();
                    });

                    row.RelativeItem(0.05f);

                    row.RelativeItem(0.35f).AlignMiddle().Column(col =>
                    {
                        col.Item().Image(LogoBytes);
                        col.Item().LineHorizontal(1);
                        col.Item().Text("Firma del profesional")
                                 .FontSize(9)
                                 .AlignCenter();
                    });
                });

                column.Item().PaddingTop(15);
                column.Item().AlignCenter().Text(txt =>
                {
                    txt.Span("Medicy © - Cuidamos tu salud · ").FontSize(10).Italic();
                    txt.Span(DateTime.Now.ToString("HH:mm:ss")).FontSize(10);
                });
            });
        }


    }
}

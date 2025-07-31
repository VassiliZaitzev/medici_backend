using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mysqlx.Cursor;

namespace _01_DataLogic.Clases
{
    public class PdfDal
    {
        //public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public string GenerarPdfClienteBase64()
        {
            //var cliente = _dal.ObtenerClientePorId(idCliente);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);

     
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);

            return Convert.ToBase64String(stream.ToArray());
        }

        public string Titulo { get; set; } = "Órden de exámenes";
        public string Fecha { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public string Contenido { get; set; } = "Aquí va el contenido principal del documento.";
        public byte[]? LogoBytes { get; set; } = null;

        void ComposeHeader(IContainer container)
        {
            string basePath = AppContext.BaseDirectory;
            string proyectoRaiz = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));

            // Construir ruta a Assets
            string logoPath = Path.Combine(proyectoRaiz, "Assets", "medici1.png");            

            if (!File.Exists(logoPath))
                throw new FileNotFoundException($"No se encontró la imagen en: {logoPath}");

            byte[] LogoBytes = File.ReadAllBytes(logoPath);

            container.Row(row =>
            {
                // Columna 1: Imagen o ícono
                row.RelativeItem(3).Height(50).AlignMiddle().AlignLeft().Element(col =>
                {
                    if (LogoBytes != null)
                        col.Image(LogoBytes);
                    else
                        col.Text("📷").FontSize(30); // emoji de imagen por defecto
                });

                // Columna 2: Título
                row.RelativeItem(6).AlignCenter().AlignMiddle().Text(Titulo)
                    .FontSize(16).SemiBold().FontColor(Colors.Black);

                // Columna 3: Fecha
                //row.RelativeColumn(3).AlignRight().AlignMiddle().Text($"Fecha: {Fecha}").FontSize(12).FontColor(Colors.Grey.Darken2);
                row.RelativeItem(3).Column(col =>
                {
                    col.Item().Text("Folio: 99cc11a5").FontSize(10).FontColor(Colors.Grey.Darken2);
                    //col.Item().Text("Fecha: "+ Fecha).FontSize(10).FontColor(Colors.Grey.Darken2);
                });
            });
        }


        //public void ComposeHeader(IContainer container)
        //{
        //    container.Row(row =>
        //    {
        //        row.RelativeColumn(1).AlignMiddle().AlignLeft().Text("📄").FontSize(36);
        //        row.RelativeColumn(4).Column(col =>
        //        {
        //            col.Item().Text("asdasdasd").SemiBold().FontSize(20);
        //            col.Item().Text($"Fecha:").FontSize(12).FontColor(Colors.Grey.Darken2);
        //        });
        //    });
        //}

        public void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                col.Spacing(10);
                col.Item().Text("Contenido");
            });
        }

        public void ComposeFooter(IContainer container)
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

                    row.RelativeItem(0.05f); // separador

                    row.RelativeItem(0.35f).AlignMiddle().Column(col =>
                    {
                        col.Item()
                           .Image(LogoBytes);

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Firma del profesional")
                                 .FontSize(9)
                                 .AlignCenter();
                    });
                });

                column.Item().PaddingTop(15);

                column.Item().AlignCenter().Text(txt =>
                {
                    txt.Span("Medicy © ss -  Cuidamos tu salud · ").FontSize(10).Italic();
                    txt.Span(DateTime.Now.ToString("HH:mm:ss")).FontSize(10);
                });
            });


        }
    }
}

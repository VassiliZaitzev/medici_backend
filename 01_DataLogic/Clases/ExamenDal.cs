using _00_Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace _01_DataLogic.Clases
{
    public class ExamenDal
    {
        public async Task<List<ExamenEN>> obtenerExamenes()
        {
            List<ExamenEN> examen = [];
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {
                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("LISTAR_EXAMEN", connection);
                command.CommandType = CommandType.StoredProcedure;

                using var reader = await command.ExecuteReaderAsync();

                examen = new List<ExamenEN>();
                ExamenEN obj;
                while (await reader.ReadAsync())
                {
                    obj = new ExamenEN();
                    obj.examenCorr = reader.IsDBNull("EXAMEN_CORR") ? 0 : reader.GetInt32("EXAMEN_CORR");
                    obj.tipoExamenCorr = reader.IsDBNull("TIPO_EXAMEN_CORR") ? 0 : reader.GetInt32("TIPO_EXAMEN_CORR");
                    obj.descripcion = reader.IsDBNull("DESCRIPCION") ? null : reader.GetString("DESCRIPCION");
                    obj.detalle = reader.IsDBNull("DETALLE") ? null : reader.GetString("DETALLE");
                    obj.valor = reader.IsDBNull("VALOR") ? 0 : reader.GetInt32("VALOR");
                    obj.vigencia = reader.IsDBNull("VIGENCIA") ? 0 : reader.GetInt32("VIGENCIA");
                    examen.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

            }

            return examen;
        }


        public async Task<List<ExamenFonasaEN>> obtenerExamenesFonasa()
        {
            List<ExamenFonasaEN> examen = [];
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {
                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("LISTAR_EXAMEN_FONASA", connection);
                command.CommandType = CommandType.StoredProcedure;

                using var reader = await command.ExecuteReaderAsync();

                examen = new List<ExamenFonasaEN>();
                ExamenFonasaEN obj;
                while (await reader.ReadAsync())
                {
                    obj = new ExamenFonasaEN();
                    obj.examenCorr = reader.IsDBNull("EXAMEN_CORR") ? 0 : reader.GetInt32("EXAMEN_CORR");
                    obj.codigo = reader.IsDBNull("CODIGO") ? null : reader.GetString("CODIGO");
                    obj.glosa = reader.IsDBNull("GLOSA") ? null : reader.GetString("GLOSA");
                    obj.codigoConcatenado = reader.IsDBNull("CODIGO_CONCATENADO") ? null : reader.GetString("CODIGO_CONCATENADO");
                    obj.grupo = reader.IsDBNull("GRUPO") ? 0 : reader.GetInt32("GRUPO");
                    examen.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

            }

            return examen;
        }
    }
}

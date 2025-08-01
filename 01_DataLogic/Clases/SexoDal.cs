using _00_Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class SexoDal
    {

        public async Task<List<SexoEN>> ObtenerSexo()
        {
            List<SexoEN> sexo = [];
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {
                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("LISTAR_SEXO", connection);
                command.CommandType = CommandType.StoredProcedure;
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var obj = new SexoEN();
                    int idxId = reader.GetOrdinal("ID_SEXO");
                    int idxDesc = reader.GetOrdinal("DESCRIPCION");
                    int idxVig = reader.GetOrdinal("VIGENCIA");
                    int idxFechaC = reader.GetOrdinal("FECHA_CREA");
                    int idxFechaM = reader.GetOrdinal("FECHA_MODIFICA");

                    obj.Sexo_Corr = reader.IsDBNull(idxId) ? 0 : reader.GetInt32(idxId);
                    obj.Descripcion = reader.GetString(idxDesc);
                    obj.Vigencia = reader.IsDBNull(idxVig) ? (byte)0 : reader.GetByte(idxVig);
                    obj.Fecha_Crea = reader.IsDBNull(idxFechaC)
                                          ? DateTime.MinValue
                                          : reader.GetDateTime(idxFechaC);
                    obj.Fecha_Modifica = reader.IsDBNull(idxFechaM)
                                          ? DateTime.MinValue
                                          : reader.GetDateTime(idxFechaM);

                    sexo.Add(obj);
                }
                return sexo;

            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

                throw;
            }
        }
    }

}

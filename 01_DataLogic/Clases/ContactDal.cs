using _00_Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class ContactDal
    {
        public async Task<int> AgregarContactoAsync(ContactEN c)
        {
                int result = 0;
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("INSERTAR_CONTACTO", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new MySqlParameter("p_NOMBRE_CONTACTO", c.Nombre));
                command.Parameters.Add(new MySqlParameter("p_EMAIL", c.Email));
                command.Parameters.Add(new MySqlParameter("p_MENSAJE", c.Mensaje));

                var outputParam = new MySqlParameter("p_ID_CONTACTO", MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await command.ExecuteNonQueryAsync();

                result = Convert.ToInt32(outputParam.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return result;
        }
    }
}

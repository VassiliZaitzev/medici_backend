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
    public class UsuarioDAL
    {
        public async Task<int> AgregarUsuario(UsuarioEN usuario)
        {
            int result = 0;
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {


                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("INSERTAR_USUARIO", connection);
                command.CommandType = CommandType.StoredProcedure;
                int genero = 3;

                if (usuario.genero.ToLower().StartsWith("m"))
                {
                    genero = 1;
                }
                else if (usuario.genero.ToLower().StartsWith("f"))
                {
                    genero = 2;
                }
                else
                {
                    genero = 3;
                }

                command.Parameters.Add(new MySqlParameter("pNOMBRE", usuario.nombre));
                command.Parameters.Add(new MySqlParameter("pAPELLIDO_PAT", usuario.apellidoPaterno));
                command.Parameters.Add(new MySqlParameter("pAPELLIDO_MAT", usuario.apellidoMaterno));
                command.Parameters.Add(new MySqlParameter("pEMAIL", usuario.email));
                command.Parameters.Add(new MySqlParameter("pEDAD", usuario.edad));
                command.Parameters.Add(new MySqlParameter("pRUT", usuario.rut));
                command.Parameters.Add(new MySqlParameter("pSEXO_CORR", genero));
                command.Parameters.Add(new MySqlParameter("pCODIGO_CLIENTE", usuario.chatGptKey));

                var outputParam = new MySqlParameter("pUSUARIO_CORR", MySqlDbType.Int64)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);


                await command.ExecuteNonQueryAsync();


                var idChat = Convert.ToInt64(outputParam.Value);
                result = (int)idChat;

            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

            }

            return result;
        }
    }
}

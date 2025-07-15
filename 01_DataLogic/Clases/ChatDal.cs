using _00_Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class ChatDal
    {
        public async Task<List<ChatEN>> listarChat()
        {
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();
            
            List<ChatEN> chat = [];

            try
            {

                using var conn = new NpgsqlConnection(config["ConnectionStrings:PgSqlTEST"]);
                await conn.OpenAsync();


                using var cmd = new NpgsqlCommand("SELECT * FROM medici.obtener_chat()", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                chat = new List<ChatEN>();
                ChatEN obj;
                while (await reader.ReadAsync())
                {
                    obj = new ChatEN();
                    obj.idChat = reader.GetInt32(0);
                    obj.codigoCliente = reader.GetInt32(1);
                    obj.mensaje = reader.GetString(2);
                    obj.responsable = reader.GetString(3);
                    chat.Add(obj);
                }                                                                                

            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);

            }

            return chat;
        }



        //public async Task<IActionResult> Post([FromBody] Chat chat)
        //{
        //    using var conn = new NpgsqlConnection(_connectionString);
        //    await conn.OpenAsync();

        //    using var cmd = new NpgsqlCommand("SELECT medici.insertar_chat(@codigo, @mensaje, @responsable)", conn);
        //    cmd.Parameters.AddWithValue("codigo", chat.CodigoCliente);
        //    cmd.Parameters.AddWithValue("mensaje", chat.Mensaje);
        //    cmd.Parameters.AddWithValue("responsable", chat.Responsable);

        //    await cmd.ExecuteNonQueryAsync();
        //    return Ok(new { message = "Insertado correctamente" });
        //}
    }
}

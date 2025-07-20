using _00_Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
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
        public async Task<List<ChatEN>> ListarChat(string codigo)
        {
            List<ChatEN> chat = [];
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            try
            {
                using var connection = new MySqlConnection(config["ConnectionStrings:medicyMySql"]);
                await connection.OpenAsync();

                using var command = new MySqlCommand("LISTAR_CHAT", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pCODIGO_CLIENTE", codigo);

                using var reader = await command.ExecuteReaderAsync();

                chat = new List<ChatEN>();
                ChatEN obj;
                while (await reader.ReadAsync())
                {
                    obj = new ChatEN();
                    obj.idChat = reader.IsDBNull("ID_CHAT") ? 0 : reader.GetInt32("ID_CHAT");
                    obj.codigoCliente = reader.IsDBNull("CODIGO_CLIENTE") ? null : reader.GetString("CODIGO_CLIENTE");
                    obj.mensaje = reader.IsDBNull("MENSAJE") ? null : reader.GetString("MENSAJE");
                    obj.idTipoMensaje = reader.IsDBNull("ID_TIPO_MENSAJE") ? null : reader.GetInt32("ID_TIPO_MENSAJE");
                    obj.fecha = reader.IsDBNull("FECHA") ? null : reader.GetDateTime("FECHA");
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
